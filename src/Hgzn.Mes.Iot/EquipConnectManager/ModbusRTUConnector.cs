using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using SqlSugar;
using StackExchange.Redis;
using System.IO.Ports;
using System.Text.Json;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Modbus.Device;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Microsoft.Extensions.Configuration;

/// <summary>
/// ModbusRTU通信连接器实现类
/// 继承自设备连接器基类，实现Modbus RTU协议(基于串口)的通信功能
/// </summary>
public class ModbusRTUConnector : EquipConnectorBase
{
    // 串口通信对象
    private SerialPort _serialPort = null!;

    // Modbus主站对象（用于发起Modbus请求）
    private IModbusSerialMaster _modbusMaster = null!;

    // 运行状态标志
    private bool _isRunning = false;

    // 设备连接配置信息
    private EquipConnect _equipConnect = null!;

    // ModbusRTU连接参数配置
    private ModbusRtuConnInfo _connInfo = null!;

    // 氧浓度读取相关配置
    private Timer _oxygenReadingTimer = null!;
    private int _readCount = 0;
    private const int READ_INTERVAL_MS = 5000; // 每5秒读取一次氧浓度
    private const int FORWARD_RATE = 10; // 每10次读取转发1次
    private readonly Dictionary<string, string> _ipToRoomMapping;
    private readonly IConfiguration _configuration;

    // 线程同步锁，防止并发访问串口
    private readonly SemaphoreSlim _modbusLock = new SemaphoreSlim(1, 1);
    private bool _isReading = false; // 标记是否正在读取

    /// <summary>
    /// 构造函数
    /// </summary>
    public ModbusRTUConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient sqlSugarClient,
        IConfiguration configuration,
        string uri, EquipConnType connType)
        : base(connectionMultiplexer, mqttExplorer, sqlSugarClient, uri, connType)
    {
        _equipConnect = sqlSugarClient.Queryable<EquipConnect>().First(x => x.Id == Guid.Parse(uri));
        _configuration = configuration;

        // 初始化IP到房间号的映射表
        _ipToRoomMapping = new Dictionary<string, string>
        {
            { "10.125.157.151", "122" },  // 01机器
            { "10.125.157.152", "209" },  // 02机器
            { "10.125.157.153", "303" },  // 03机器
            { "10.125.157.154", "203" },  // 04机器
            { "10.125.157.155", "206" },  // 05机器
            { "10.125.157.156", "306" },  // 06机器
            { "10.125.157.157", "213" },  // 07机器
            { "10.125.157.158", "215" },  // 08机器
            { "10.125.157.159", "202" },  // 09机器
            { "10.125.157.160", "108" },  // 10机器
            { "10.125.157.161", "112" },  // 11机器
            { "10.125.157.162", "119" },  // 12机器
            { "10.125.157.163", "109" },  // 13机器
            { "10.125.157.164", "302" },  // 14机器（未安装，但预留）
            { "10.125.157.165", "103" },   // 15机器
            // 下面3个ip用于测试
            { "192.168.100.100", "301" },   // 测试
            { "192.168.100.101", "301" },   // 测试
            { "192.168.100.102", "301" }   // 测试
        };
    }

    /// <summary>
    /// 关闭连接（释放资源）
    /// </summary>
    public override async Task CloseConnectionAsync()
    {
        LoggerAdapter.LogInformation($"AG - ModbusRTU开始关闭连接");

        // 停止定时器
        if (_oxygenReadingTimer != null)
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU停止氧浓度读取定时器");
            _oxygenReadingTimer.Dispose();
            _oxygenReadingTimer = null!;
        }

        // 等待正在进行的读取操作完成
        if (_isReading)
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU等待正在进行的读取操作完成");
            await Task.Delay(100); // 等待最多100ms
        }

        if (_modbusMaster != null)
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU释放Modbus主站对象");
            _modbusMaster.Dispose();
            _modbusMaster = null!;
        }

        if (_serialPort != null && _serialPort.IsOpen)
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU关闭串口连接");
            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null!;
        }

        _isRunning = false;
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);

        LoggerAdapter.LogInformation($"AG - ModbusRTU连接关闭完成");
    }

    /// <summary>
    /// 启动连接器
    /// </summary>
    public override async Task StartAsync(Guid uri)
    {
        if (_modbusMaster != null && _serialPort != null && _serialPort.IsOpen)
        {
            _isRunning = true;
            await UpdateStateAsync(ConnStateType.Run);
            await UpdateOperationAsync(ConnStateType.Run);
        }
    }

    /// <summary>
    /// 停止连接器
    /// </summary>
    public override async Task StopAsync(Guid uri)
    {
        await CloseConnectionAsync();
    }

    /// <summary>
    /// 建立ModbusRTU连接
    /// </summary>
    public override async Task<bool> ConnectAsync(ConnInfo connInfo)
    {
        if (connInfo?.ConnString is null)
            throw new ArgumentNullException(nameof(connInfo));

        LoggerAdapter.LogInformation($"AG - ModbusRTU开始连接,连接字符串:{connInfo.ConnString}");

        // 反序列化为ModbusRtuConnInfo类型
        try
        {
            _connInfo = JsonSerializer.Deserialize<ModbusRtuConnInfo>(
                connInfo.ConnString, Options.CustomJsonSerializerOptions)
                ?? throw new ArgumentNullException("conn");

            LoggerAdapter.LogInformation($"AG - ModbusRTU配置解析成功,串口:COM{_connInfo.PortName},波特率:{_connInfo.BaudRate},从站地址:{_connInfo.slaveAddress}");
        }
        catch (Exception e)
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU配置解析失败: {e.Message}");
        }

        try
        {
            // 将int类型的校验位和停止位转换为枚举类型
            Parity parity = _connInfo.Parity switch
            {
                "None" => Parity.None,
                "Odd" => Parity.Odd,
                "Even" => Parity.Even,
                "Mark" => Parity.Mark,
                "Space" => Parity.Space,
                _ => Parity.None
            };

            StopBits stopBits = _connInfo.StopBits switch
            {
                "One" => StopBits.One,
                "Two" => StopBits.Two,
                "OnePointFive" => StopBits.OnePointFive,
                _ => StopBits.One
            };

            // 创建并配置串口对象
            // 跨平台串口名称处理：
            // Windows: COM1, COM5 等
            // Linux: /dev/ttyUSB0, /dev/ttyS0 等
            string portName = GetPlatformSerialPortName(_connInfo.PortName.ToString());
            
            _serialPort = new SerialPort(
                portName,                        // 串口号（跨平台）
                int.Parse(_connInfo.BaudRate),      // 波特率
                parity,                 // 校验位
                _connInfo.DataBits,     // 数据位
                stopBits);             // 停止位

            // 设置超时时间（增加超时时间以避免写入超时）
            _serialPort.ReadTimeout = Math.Max(_connInfo.ReadTimeout, 3000);  // 至少3秒
            _serialPort.WriteTimeout = Math.Max(_connInfo.WriteTimeout, 3000); // 至少3秒

            // 打开串口
            LoggerAdapter.LogInformation($"AG - ModbusRTU尝试打开串口:COM{_connInfo.PortName}");
            _serialPort.Open();
            LoggerAdapter.LogInformation($"AG - ModbusRTU串口打开成功");

            // 清空串口缓冲区，避免旧数据干扰
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            LoggerAdapter.LogInformation($"AG - ModbusRTU已清空串口缓冲区");

            // 创建Modbus RTU主站
            LoggerAdapter.LogInformation($"AG - ModbusRTU创建主站对象");
            _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);

            // 配置默认重试参数
            _modbusMaster.Transport.Retries = 3;
            _modbusMaster.Transport.WaitToRetryMilliseconds = 100;
            LoggerAdapter.LogInformation($"AG - ModbusRTU主站配置完成,重试次数:3,重试间隔:100ms");

            _isRunning = true;

            // 启动氧浓度读取定时器
            LoggerAdapter.LogInformation($"AG - ModbusRTU启动氧浓度读取定时器,间隔:{READ_INTERVAL_MS}ms,转发率:每{FORWARD_RATE}次转发1次");
            _oxygenReadingTimer = new Timer(ReadOxygenConcentration, null, READ_INTERVAL_MS, READ_INTERVAL_MS);

            await UpdateStateAsync(ConnStateType.On);
            await UpdateOperationAsync(ConnStateType.On);

            LoggerAdapter.LogInformation($"Modbus RTU连接已建立，串口:{"COM" + _connInfo.PortName} 从站ID:{_connInfo.slaveAddress}");
            LoggerAdapter.LogInformation($"AG - ModbusRTU连接建立完成,状态更新为On");
            return true;
        }
        catch (Exception ex)
        {
            await CloseConnectionAsync();
            LoggerAdapter.LogError($"Modbus RTU连接失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 发送数据到设备
    /// </summary>
    public override async Task SendDataAsync(byte[] buffer)
    {
        if (!_isRunning || _modbusMaster == null || _serialPort == null || !_serialPort.IsOpen)
        {
            LoggerAdapter.LogWarning("Modbus RTU未连接，无法发送数据");
            return;
        }

        try
        {
            // 示例：写入单个寄存器（前两个字节）
            if (buffer.Length >= 2)
            {
                ushort value = (ushort)((buffer[0] << 8) | buffer[1]);
                await WriteSingleRegisterAsync(0, value); // 默认写入地址0
            }
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"Modbus RTU写入失败: {ex.Message}");
        }
    }

    /* ========== 氧浓度读取和转发方法 ========== */

    /// <summary>
    /// 定时读取氧浓度的回调方法
    /// </summary>
    private async void ReadOxygenConcentration(object? state)
    {
        // 防止重入：如果上一次读取还在进行中，跳过本次
        if (_isReading)
        {
            LoggerAdapter.LogWarning($"AG - ModbusRTU上一次读取尚未完成，跳过本次读取");
            return;
        }

        if (!_isRunning || _modbusMaster == null)
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度读取跳过,运行状态:{_isRunning},主站对象:{_modbusMaster != null}");
            return;
        }

        // 尝试获取锁，如果获取失败则跳过本次读取
        if (!await _modbusLock.WaitAsync(0))
        {
            LoggerAdapter.LogWarning($"AG - ModbusRTU无法获取锁，跳过本次读取");
            return;
        }

        _isReading = true;

        try
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU开始读取氧浓度,寄存器地址:0x0006,读取次数:{_readCount + 1}");

            // 读取氧浓度寄存器（根据文档，从地址0x0006开始读取通道1的浓度值）
            var oxygenValues = await ReadHoldingRegistersAsync(0x0006, 1); // 读取通道1的16位整数浓度值

            if (oxygenValues.Length > 0)
            {
                ushort rawValue = oxygenValues[0];
                // 根据文档，结果为实际浓度值扩大100倍，如25.4寄存器值为2540
                float actualConcentration = rawValue / 100.0f;

                LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度读取成功,原始值:{rawValue},实际浓度:{actualConcentration:F2}%");

                _readCount++;
                LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度读取计数:{_readCount}/{FORWARD_RATE}");

                // 根据转发率决定是否转发数据
                if (_readCount >= FORWARD_RATE)
                {
                    LoggerAdapter.LogInformation($"AG - ModbusRTU达到转发条件,开始转发氧浓度数据");
                    await ForwardOxygenDataAsync(actualConcentration);
                    _readCount = 0; // 重置计数器
                    LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度转发完成,计数器已重置");
                }
            }
            else
            {
                LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度读取返回空数据,数组长度:{oxygenValues.Length}");
            }
        }
        catch (TimeoutException tex)
        {
            LoggerAdapter.LogError($"读取氧浓度超时: {tex.Message}");
            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度读取超时，可能是设备无响应或串口通信问题");
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"读取氧浓度失败: {ex.Message}");
            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度读取异常详情:{ex}");
        }
        finally
        {
            _isReading = false;
            _modbusLock.Release();
        }
    }

    /// <summary>
    /// 转发氧浓度数据到MQTT
    /// </summary>
    private async Task ForwardOxygenDataAsync(float concentration)
    {
        try
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU开始转发氧浓度数据,浓度值:{concentration:F2}%");

            // 根据配置获取本地IP地址，然后映射到房间号
            var localIp = GetLocalIpAddress();
            var roomNumber = GetRoomNumberByIp(localIp);
            LoggerAdapter.LogInformation($"AG - ModbusRTU获取本地IP:{localIp},映射房间号:{roomNumber}");

            // 检查是否异常
            const float NORMAL_MIN = 19.5f;
            const float NORMAL_MAX = 23.5f;
            bool isAbnormal = concentration < NORMAL_MIN || concentration > NORMAL_MAX;
            int abnormalType = 0;
            if (concentration < NORMAL_MIN) abnormalType = 1; // 过低
            else if (concentration > NORMAL_MAX) abnormalType = 2; // 过高

            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度异常检查,正常范围:{NORMAL_MIN}-{NORMAL_MAX}%,当前值:{concentration:F2}%,异常:{isAbnormal},异常类型:{abnormalType}");

            // 构建氧浓度数据
            var oxygenData = new
            {
                RoomNumber = roomNumber,
                Concentration = concentration,
                Timestamp = DateTime.Now,
                DeviceId = _equipConnect.EquipId,
                LocalIp = localIp,
                IsAbnormal = isAbnormal,
                AbnormalType = abnormalType
            };

            // 将数据序列化为字节数组
            var jsonData = JsonSerializer.Serialize(oxygenData, Options.CustomJsonSerializerOptions);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonData);
            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度数据序列化完成,JSON长度:{jsonData.Length},字节数组长度:{buffer.Length}");

            // 构建MQTT主题（使用特殊的氧浓度设备类型）
            var topic = IotTopicBuilder.CreateIotBuilder()
                            .WithPrefix(TopicType.Iot)
                            .WithDirection(MqttDirection.Up)
                            .WithTag(MqttTag.Transmit)
                            .WithDeviceType("OxygenConcentration") // 特殊的氧浓度设备类型
                            .WithUri(_equipConnect.Id.ToString()).Build();

            LoggerAdapter.LogInformation($"AG - ModbusRTU构建MQTT主题:{topic}");

            // 使用支持断点续传的发布方法
            if (_mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
            {
                LoggerAdapter.LogInformation($"AG - ModbusRTU使用断点续传发布MQTT消息");
                await mqttWithOffline.PublishWithOfflineSupportAsync(topic, buffer, priority: 0, maxRetryCount: 3);
            }
            else
            {
                LoggerAdapter.LogInformation($"AG - ModbusRTU使用普通方式发布MQTT消息");
                await _mqttExplorer.PublishAsync(topic, buffer);
            }

            LoggerAdapter.LogInformation($"氧浓度数据已转发: 房间号={roomNumber}, 浓度={concentration:F2}%");
            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度数据转发完成");
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"转发氧浓度数据失败: {ex.Message}");
            LoggerAdapter.LogInformation($"AG - ModbusRTU氧浓度数据转发异常详情:{ex}");
        }
    }

    /// <summary>
    /// 获取本地IP地址（从配置文件）
    /// </summary>
    private string GetLocalIpAddress()
    {
        var localIp = _configuration.GetValue<string>("LocalIpAddress");
        return string.IsNullOrEmpty(localIp) ? "10.125.157.151" : localIp;
    }

    /// <summary>
    /// 根据IP地址获取房间号
    /// </summary>
    private string GetRoomNumberByIp(string ipAddress)
    {
        return _ipToRoomMapping.TryGetValue(ipAddress, out var roomNumber)
            ? roomNumber
            : "未知";
    }

    /* ========== 跨平台串口处理方法 ========== */

    /// <summary>
    /// 获取跨平台的串口名称
    /// Windows: COM1, COM5 等
    /// Linux: /dev/ttyUSB0, /dev/ttyS0 等
    /// </summary>
    /// <param name="portName">配置中的端口名称（可能是数字如"5"，或完整路径如"/dev/ttyUSB0"）</param>
    /// <returns>平台对应的串口名称</returns>
    private string GetPlatformSerialPortName(string portName)
    {
        // 如果已经是完整的设备路径（Linux格式），直接返回
        if (portName.StartsWith("/dev/"))
        {
            LoggerAdapter.LogInformation($"AG - ModbusRTU使用完整设备路径: {portName}");
            return portName;
        }

        // 判断当前操作系统
        if (OperatingSystem.IsWindows())
        {
            // Windows 平台：添加 COM 前缀
            string windowsPort = $"COM{portName}";
            LoggerAdapter.LogInformation($"AG - ModbusRTU Windows平台，串口名称: {windowsPort}");
            return windowsPort;
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux 平台：根据端口号映射到设备路径
            // 优先检查 USB 串口
            string usbPort = $"/dev/ttyUSB{portName}";
            if (File.Exists(usbPort))
            {
                LoggerAdapter.LogInformation($"AG - ModbusRTU Linux平台，找到USB串口: {usbPort}");
                return usbPort;
            }

            // 检查标准串口
            string standardPort = $"/dev/ttyS{portName}";
            if (File.Exists(standardPort))
            {
                LoggerAdapter.LogInformation($"AG - ModbusRTU Linux平台，找到标准串口: {standardPort}");
                return standardPort;
            }

            // 检查 ACM 设备（Arduino/CH340等）
            string acmPort = $"/dev/ttyACM{portName}";
            if (File.Exists(acmPort))
            {
                LoggerAdapter.LogInformation($"AG - ModbusRTU Linux平台，找到ACM设备: {acmPort}");
                return acmPort;
            }

            // 如果都找不到，尝试使用 ttyUSB0 作为默认值
            LoggerAdapter.LogWarning($"AG - ModbusRTU Linux平台未找到设备，尝试使用: {usbPort}");
            return usbPort;
        }
        else
        {
            // 其他平台（macOS等）
            LoggerAdapter.LogWarning($"AG - ModbusRTU 未知平台，使用原始端口名: {portName}");
            return portName;
        }
    }

    /* ========== Modbus寄存器操作方法 ========== */

    /// <summary>
    /// 读取保持寄存器（内部方法，调用者需确保已获取锁）
    /// </summary>
    /// <param name="startAddress">起始地址</param>
    /// <param name="numberOfPoints">读取数量</param>
    /// <returns>寄存器值数组</returns>
    private async Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        LoggerAdapter.LogInformation($"AG - ModbusRTU读取保持寄存器,从站地址:{_connInfo.slaveAddress},起始地址:0x{startAddress:X4},数量:{numberOfPoints}");

        var result = await _modbusMaster.ReadHoldingRegistersAsync(
            _connInfo.slaveAddress,      // 使用配置中的从站地址
            startAddress,
            numberOfPoints);

        LoggerAdapter.LogInformation($"AG - ModbusRTU保持寄存器读取完成,返回数据长度:{result.Length}");
        return result;
    }

    /// <summary>
    /// 读取保持寄存器（公共方法，带锁保护）
    /// </summary>
    public async Task<ushort[]> ReadHoldingRegistersWithLockAsync(ushort startAddress, ushort numberOfPoints)
    {
        await _modbusLock.WaitAsync();
        try
        {
            return await ReadHoldingRegistersAsync(startAddress, numberOfPoints);
        }
        finally
        {
            _modbusLock.Release();
        }
    }

    /// <summary>
    /// 读取输入寄存器
    /// </summary>
    public async Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort numberOfPoints)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        return await _modbusMaster.ReadInputRegistersAsync(
            _connInfo.slaveAddress,
            startAddress,
            numberOfPoints);
    }

    /// <summary>
    /// 写入单个寄存器（指定从站地址）
    /// </summary>
    public async Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        await _modbusMaster.WriteSingleRegisterAsync(slaveAddress, registerAddress, value);
    }

    /// <summary>
    /// 写入单个寄存器（使用配置中的从站地址）
    /// </summary>
    public async Task WriteSingleRegisterAsync(ushort registerAddress, ushort value)
    {
        await WriteSingleRegisterAsync(_connInfo.slaveAddress, registerAddress, value);
    }

    /// <summary>
    /// 写入多个寄存器（指定从站地址）
    /// </summary>
    public async Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] values)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        await _modbusMaster.WriteMultipleRegistersAsync(slaveAddress, startAddress, values);
    }

    /// <summary>
    /// 写入多个寄存器（使用配置中的从站地址）
    /// </summary>
    public async Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] values)
    {
        await WriteMultipleRegistersAsync(_connInfo.slaveAddress, startAddress, values);
    }

    /// <summary>
    /// 读取线圈状态
    /// </summary>
    public async Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort numberOfPoints)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        return await _modbusMaster.ReadCoilsAsync(
            _connInfo.slaveAddress,
            startAddress,
            numberOfPoints);
    }

    /// <summary>
    /// 写入单个线圈
    /// </summary>
    public async Task WriteSingleCoilAsync(ushort coilAddress, bool value)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        await _modbusMaster.WriteSingleCoilAsync(
            _connInfo.slaveAddress,
            coilAddress,
            value);
    }
}
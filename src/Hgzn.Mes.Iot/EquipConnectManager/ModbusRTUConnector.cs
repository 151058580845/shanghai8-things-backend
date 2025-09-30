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
            { "10.125.157.165", "103" }   // 15机器
        };
    }

    /// <summary>
    /// 关闭连接（释放资源）
    /// </summary>
    public override async Task CloseConnectionAsync()
    {
        LoggerAdapter.LogDebug($"AG - ModbusRTU开始关闭连接");
        
        // 停止定时器
        if (_oxygenReadingTimer != null)
        {
            LoggerAdapter.LogDebug($"AG - ModbusRTU停止氧浓度读取定时器");
            _oxygenReadingTimer.Dispose();
            _oxygenReadingTimer = null!;
        }

        if (_modbusMaster != null)
        {
            LoggerAdapter.LogDebug($"AG - ModbusRTU释放Modbus主站对象");
            _modbusMaster.Dispose();
            _modbusMaster = null!;
        }

        if (_serialPort != null && _serialPort.IsOpen)
        {
            LoggerAdapter.LogDebug($"AG - ModbusRTU关闭串口连接");
            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null!;
        }

        _isRunning = false;
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
        
        LoggerAdapter.LogDebug($"AG - ModbusRTU连接关闭完成");
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

        LoggerAdapter.LogDebug($"AG - ModbusRTU开始连接,连接字符串:{connInfo.ConnString}");

        // 反序列化为ModbusRtuConnInfo类型
        try
        {
            _connInfo = JsonSerializer.Deserialize<ModbusRtuConnInfo>(
                connInfo.ConnString, Options.CustomJsonSerializerOptions)
                ?? throw new ArgumentNullException("conn");
            
            LoggerAdapter.LogDebug($"AG - ModbusRTU配置解析成功,串口:COM{_connInfo.PortName},波特率:{_connInfo.BaudRate},从站地址:{_connInfo.slaveAddress}");
        }
        catch (Exception e) 
        { 
            LoggerAdapter.LogDebug($"AG - ModbusRTU配置解析失败: {e.Message}");
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
            _serialPort = new SerialPort(
                "COM" + _connInfo.PortName,      // 串口号
                int.Parse(_connInfo.BaudRate),      // 波特率
                parity,                 // 校验位
                _connInfo.DataBits,     // 数据位
                stopBits);             // 停止位

            // 设置超时时间
            _serialPort.ReadTimeout = _connInfo.ReadTimeout;
            _serialPort.WriteTimeout = _connInfo.WriteTimeout;

            // 打开串口
            LoggerAdapter.LogDebug($"AG - ModbusRTU尝试打开串口:COM{_connInfo.PortName}");
            _serialPort.Open();
            LoggerAdapter.LogDebug($"AG - ModbusRTU串口打开成功");

            // 创建Modbus RTU主站
            LoggerAdapter.LogDebug($"AG - ModbusRTU创建主站对象");
            _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);

            // 配置默认重试参数
            _modbusMaster.Transport.Retries = 3;
            _modbusMaster.Transport.WaitToRetryMilliseconds = 100;
            LoggerAdapter.LogDebug($"AG - ModbusRTU主站配置完成,重试次数:3,重试间隔:100ms");

            _isRunning = true;

            // 启动氧浓度读取定时器
            LoggerAdapter.LogDebug($"AG - ModbusRTU启动氧浓度读取定时器,间隔:{READ_INTERVAL_MS}ms,转发率:每{FORWARD_RATE}次转发1次");
            _oxygenReadingTimer = new Timer(ReadOxygenConcentration, null, READ_INTERVAL_MS, READ_INTERVAL_MS);

            await UpdateStateAsync(ConnStateType.On);
            await UpdateOperationAsync(ConnStateType.On);

            LoggerAdapter.LogInformation($"Modbus RTU连接已建立，串口:{"COM" + _connInfo.PortName} 从站ID:{_connInfo.slaveAddress}");
            LoggerAdapter.LogDebug($"AG - ModbusRTU连接建立完成,状态更新为On");
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
        if (!_isRunning || _modbusMaster == null)
        {
            LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度读取跳过,运行状态:{_isRunning},主站对象:{_modbusMaster != null}");
            return;
        }

        try
        {
            LoggerAdapter.LogDebug($"AG - ModbusRTU开始读取氧浓度,寄存器地址:0x0006,读取次数:{_readCount + 1}");
            
            // 读取氧浓度寄存器（根据文档，从地址0x0006开始读取通道1的浓度值）
            var oxygenValues = await ReadHoldingRegistersAsync(0x0006, 1); // 读取通道1的16位整数浓度值
            
            if (oxygenValues.Length > 0)
            {
                ushort rawValue = oxygenValues[0];
                // 根据文档，结果为实际浓度值扩大100倍，如25.4寄存器值为2540
                float actualConcentration = rawValue / 100.0f;

                LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度读取成功,原始值:{rawValue},实际浓度:{actualConcentration:F2}%");

                _readCount++;
                LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度读取计数:{_readCount}/{FORWARD_RATE}");
                
                // 根据转发率决定是否转发数据
                if (_readCount >= FORWARD_RATE)
                {
                    LoggerAdapter.LogDebug($"AG - ModbusRTU达到转发条件,开始转发氧浓度数据");
                    await ForwardOxygenDataAsync(actualConcentration);
                    _readCount = 0; // 重置计数器
                    LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度转发完成,计数器已重置");
                }
            }
            else
            {
                LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度读取返回空数据,数组长度:{oxygenValues.Length}");
            }
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"读取氧浓度失败: {ex.Message}");
            LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度读取异常详情:{ex}");
        }
    }

    /// <summary>
    /// 转发氧浓度数据到MQTT
    /// </summary>
    private async Task ForwardOxygenDataAsync(float concentration)
    {
        try
        {
            LoggerAdapter.LogDebug($"AG - ModbusRTU开始转发氧浓度数据,浓度值:{concentration:F2}%");
            
            // 根据配置获取本地IP地址，然后映射到房间号
            var localIp = GetLocalIpAddress();
            var roomNumber = GetRoomNumberByIp(localIp);
            LoggerAdapter.LogDebug($"AG - ModbusRTU获取本地IP:{localIp},映射房间号:{roomNumber}");

            // 检查是否异常
            const float NORMAL_MIN = 19.5f;
            const float NORMAL_MAX = 23.5f;
            bool isAbnormal = concentration < NORMAL_MIN || concentration > NORMAL_MAX;
            int abnormalType = 0;
            if (concentration < NORMAL_MIN) abnormalType = 1; // 过低
            else if (concentration > NORMAL_MAX) abnormalType = 2; // 过高
            
            LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度异常检查,正常范围:{NORMAL_MIN}-{NORMAL_MAX}%,当前值:{concentration:F2}%,异常:{isAbnormal},异常类型:{abnormalType}");

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
            LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度数据序列化完成,JSON长度:{jsonData.Length},字节数组长度:{buffer.Length}");

            // 构建MQTT主题（使用特殊的氧浓度设备类型）
            var topic = IotTopicBuilder.CreateIotBuilder()
                            .WithPrefix(TopicType.Iot)
                            .WithDirection(MqttDirection.Up)
                            .WithTag(MqttTag.Transmit)
                            .WithDeviceType("OxygenConcentration") // 特殊的氧浓度设备类型
                            .WithUri(_equipConnect.Id.ToString()).Build();
            
            LoggerAdapter.LogDebug($"AG - ModbusRTU构建MQTT主题:{topic}");

            // 使用支持断点续传的发布方法
            if (_mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
            {
                LoggerAdapter.LogDebug($"AG - ModbusRTU使用断点续传发布MQTT消息");
                await mqttWithOffline.PublishWithOfflineSupportAsync(topic, buffer, priority: 0, maxRetryCount: 3);
            }
            else
            {
                LoggerAdapter.LogDebug($"AG - ModbusRTU使用普通方式发布MQTT消息");
                await _mqttExplorer.PublishAsync(topic, buffer);
            }

            LoggerAdapter.LogInformation($"氧浓度数据已转发: 房间号={roomNumber}, 浓度={concentration:F2}%");
            LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度数据转发完成");
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"转发氧浓度数据失败: {ex.Message}");
            LoggerAdapter.LogDebug($"AG - ModbusRTU氧浓度数据转发异常详情:{ex}");
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

    /* ========== Modbus寄存器操作方法 ========== */

    /// <summary>
    /// 读取保持寄存器
    /// </summary>
    /// <param name="startAddress">起始地址</param>
    /// <param name="numberOfPoints">读取数量</param>
    /// <returns>寄存器值数组</returns>
    public async Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints)
    {
        if (!_isRunning)
            throw new InvalidOperationException("Modbus RTU未连接");

        LoggerAdapter.LogDebug($"AG - ModbusRTU读取保持寄存器,从站地址:{_connInfo.slaveAddress},起始地址:0x{startAddress:X4},数量:{numberOfPoints}");
        
        var result = await _modbusMaster.ReadHoldingRegistersAsync(
            _connInfo.slaveAddress,      // 使用配置中的从站地址
            startAddress,
            numberOfPoints);
            
        LoggerAdapter.LogDebug($"AG - ModbusRTU保持寄存器读取完成,返回数据长度:{result.Length}");
        return result;
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
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

    /// <summary>
    /// 构造函数
    /// </summary>
    public ModbusRTUConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient sqlSugarClient,
        string uri, EquipConnType connType)
        : base(connectionMultiplexer, mqttExplorer, sqlSugarClient, uri, connType)
    {
        _equipConnect = sqlSugarClient.Queryable<EquipConnect>().First(x => x.Id == Guid.Parse(uri));
    }

    /// <summary>
    /// 关闭连接（释放资源）
    /// </summary>
    public override async Task CloseConnectionAsync()
    {
        if (_modbusMaster != null)
        {
            _modbusMaster.Dispose();
            _modbusMaster = null!;
        }

        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null!;
        }

        _isRunning = false;
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
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

        // 反序列化为ModbusRtuConnInfo类型
        try
        {
            _connInfo = JsonSerializer.Deserialize<ModbusRtuConnInfo>(
                connInfo.ConnString, Options.CustomJsonSerializerOptions)
                ?? throw new ArgumentNullException("conn");
        }
        catch (Exception e) { }

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
            _serialPort.Open();

            // 创建Modbus RTU主站
            _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);

            // 配置默认重试参数
            _modbusMaster.Transport.Retries = 3;
            _modbusMaster.Transport.WaitToRetryMilliseconds = 100;

            _isRunning = true;

            await UpdateStateAsync(ConnStateType.On);
            await UpdateOperationAsync(ConnStateType.On);

            LoggerAdapter.LogInformation($"Modbus RTU连接已建立，串口:{"COM" + _connInfo.PortName} 从站ID:{_connInfo.slaveAddress}");
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

        return await _modbusMaster.ReadHoldingRegistersAsync(
            _connInfo.slaveAddress,      // 使用配置中的从站地址
            startAddress,
            numberOfPoints);
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
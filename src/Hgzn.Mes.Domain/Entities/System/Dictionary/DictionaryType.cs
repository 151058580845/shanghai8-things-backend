using Hgzn.Mes.Domain.Entities.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hgzn.Mes.Domain.Entities.System.Dictionary
{
    public class DictionaryType : UniversalEntity, ISoftDelete, IOrder, IState, ISeedsGeneratable
    {
        [Description("排序")] public int OrderNum { get; set; } = 0;

        [Description("状态")] public bool State { get; set; } = true;

        [Description("字典名称")] public string DictName { get; set; } = string.Empty;

        [Description("字典类型")] public string DictType { get; set; } = string.Empty;

        [Description("描述")] public string? Remark { get; set; }

        [Description("创建时间")] public DateTime CreationTime { get; set; }

        [Description("创建者ID")] public Guid? CreatorId { get; set; }

        [Description("最后修改者ID")] public Guid? LastModifierId { get; set; }

        [Description("最后修改时间")] public DateTime? LastModificationTime { get; set; }

        [Description("删除时间")] public DateTime? DeleteTime { get; set; }

        [Description("逻辑删除")] public bool SoftDeleted { get; set; }

        [Description("与 DictionaryEntity 的一对多关系")]
        [NotMapped]
        public ICollection<DictionaryInfo>? DictionaryEntities { get; set; }


        #region EquipMaintenance

        public static readonly DictionaryType EquipMaintenanceType = new DictionaryType()
        {
            Id = Guid.Parse("d1c3f0fb-f716-4059-bd23-99a1bbfa503d"), // 使用 Guid.Parse 生成的 Guid
            DictName = "Equipment Maintenance Type",
            DictType = "EquipMaintenanceType",
            OrderNum = 1,
            Remark = "Type of equipment maintenance",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime()
        };

        public static readonly DictionaryInfo Check = new DictionaryInfo()
        {
            Id = Guid.Parse("08e8bafc-1a6d-4ce8-a921-e95fae5ac56b"), // 使用 Guid.Parse 生成的 Guid
            DictLabel = "点检",
            DictValue = "Check",
            ParentId = EquipMaintenanceType.Id,
            OrderNum = 73,
            Remark = "点检",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Maintenance = new DictionaryInfo()
        {
            Id = Guid.Parse("c0a70d2c-e9c1-4b5b-b6da-607db13ea1c0"), // 使用 Guid.Parse 生成的 Guid
            DictLabel = "保养",
            DictValue = "Maintenance",
            ParentId = EquipMaintenanceType.Id,
            OrderNum = 74,
            Remark = "保养",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region PlanStatus

        public static readonly DictionaryType PlanStatus = new DictionaryType()
        {
            Id = Guid.Parse("f1b1c3c9-6319-47ea-8c5e-f8d4e8faec51"), // 固定的 Guid 值
            DictName = "Plan Status",
            DictType = "PlanStatus",
            OrderNum = 2,
            Remark = "Status of the plan",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo NotStarted = new DictionaryInfo()
        {
            Id = Guid.Parse("2c1a3c4b-8d91-41c4-92a0-b65fbd79f207"), // 固定的 Guid 值
            DictLabel = "未开始",
            DictValue = "NotStarted",
            ParentId = PlanStatus.Id,
            OrderNum = 70,
            Remark = "未开始",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo InProgress = new DictionaryInfo()
        {
            Id = Guid.Parse("9c61c1e7-7fe2-46f7-b4ed-22b5203c14ed"), // 固定的 Guid 值
            DictLabel = "进行中",
            DictValue = "InProgress",
            ParentId = PlanStatus.Id,
            OrderNum = 71,
            Remark = "进行中",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Completed = new DictionaryInfo()
        {
            Id = Guid.Parse("72f9f8c1-3e2b-4a7a-9c7f-dc698a11d89f"), // 固定的 Guid 值
            DictLabel = "已完成",
            DictValue = "Completed",
            ParentId = PlanStatus.Id,
            OrderNum = 72,
            Remark = "已完成",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region Frequency

        public static readonly DictionaryType Frequency = new DictionaryType()
        {
            Id = Guid.Parse("a5b60a2b-9c5b-4cc7-97fa-8b7a1b6229c9"), // 固定的 Guid 值
            DictName = "Frequency",
            DictType = "Frequency",
            OrderNum = 3,
            Remark = "Frequency of data collection",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Hour = new DictionaryInfo()
        {
            Id = Guid.Parse("b27a7434-d4b1-4f8f-b052-3270c3a702dd"), // 固定的 Guid 值
            DictLabel = "小时",
            DictValue = "Hour",
            ParentId = Frequency.Id,
            OrderNum = 64,
            Remark = "小时",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day = new DictionaryInfo()
        {
            Id = Guid.Parse("fad38f88-c967-4c66-9274-b47c540edb71"), // 固定的 Guid 值
            DictLabel = "天",
            DictValue = "Day",
            ParentId = Frequency.Id,
            OrderNum = 65,
            Remark = "天",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Week = new DictionaryInfo()
        {
            Id = Guid.Parse("9bc1f67b-fd73-46d7-ae84-56d42015325f"), // 固定的 Guid 值
            DictLabel = "周",
            DictValue = "Week",
            ParentId = Frequency.Id,
            OrderNum = 66,
            Remark = "周",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Month = new DictionaryInfo()
        {
            Id = Guid.Parse("35ad989f-bc7c-4735-88c9-3bba1c65a7bc"), // 固定的 Guid 值
            DictLabel = "月",
            DictValue = "Month",
            ParentId = Frequency.Id,
            OrderNum = 67,
            Remark = "月",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Quarter = new DictionaryInfo()
        {
            Id = Guid.Parse("b576b5da-1d5c-4577-95ff-62d132e23a51"), // 固定的 Guid 值
            DictLabel = "季度",
            DictValue = "Quarter",
            ParentId = Frequency.Id,
            OrderNum = 68,
            Remark = "季度",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Year = new DictionaryInfo()
        {
            Id = Guid.Parse("11b726e2-2e12-4ca4-bc99-0fd0816a6886"), // 固定的 Guid 值
            DictLabel = "年",
            DictValue = "Year",
            ParentId = Frequency.Id,
            OrderNum = 69,
            Remark = "年",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region CodeRuleType

        public static readonly DictionaryType CodeRuleType = new DictionaryType()
        {
            Id = Guid.Parse("d0a70d2c-cb3f-4c88-9142-5a7c51512c8d"), // 固定的 Guid 值
            DictName = "Code Rule Type",
            DictType = "CodeRuleType",
            OrderNum = 4,
            Remark = "Type of coding rule",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo SerialNumber = new DictionaryInfo()
        {
            Id = Guid.Parse("c6fb647b-b6c2-4726-b3f9-4772f9d6f75f"), // 固定的 Guid 值
            DictLabel = "流水号",
            DictValue = "SerialNumber",
            ParentId = CodeRuleType.Id,
            OrderNum = 60,
            Remark = "流水号",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Constant1 = new DictionaryInfo()
        {
            Id = Guid.Parse("f4d6049f-2a95-4855-a122-7a7c5c6899fc"), // 固定的 Guid 值
            DictLabel = "常量",
            DictValue = "Constant",
            ParentId = CodeRuleType.Id,
            OrderNum = 61,
            Remark = "常量",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Date = new DictionaryInfo()
        {
            Id = Guid.Parse("8d659fd7-e63a-4670-a7ff-d2a0e52f02b5"), // 固定的 Guid 值
            DictLabel = "日期",
            DictValue = "Date",
            ParentId = CodeRuleType.Id,
            OrderNum = 62,
            Remark = "日期",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo AttributeValue = new DictionaryInfo()
        {
            Id = Guid.Parse("4c67a2ea-e6f1-44a4-bb36-779d918b537d"), // 固定的 Guid 值
            DictLabel = "基础元素值",
            DictValue = "AttributeValue",
            ParentId = CodeRuleType.Id,
            OrderNum = 63,
            Remark = "基础元素值",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region DateFormat

        public static readonly DictionaryType DateFormat = new DictionaryType()
        {
            Id = Guid.Parse("3e3d27d3-8b60-4b1a-b775-95e9e1b233d7"), // 固定的 Guid 值
            DictName = "Date Format",
            DictType = "DateFormat",
            OrderNum = 5,
            Remark = "Date format",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo YearMonthDay = new DictionaryInfo()
        {
            Id = Guid.Parse("7c728a49-d073-4a7a-a3b2-745d0c0d6e02"), // 固定的 Guid 值
            DictLabel = "年月日",
            DictValue = "yyyyMMdd",
            ParentId = DateFormat.Id,
            OrderNum = 59,
            Remark = "日期格式：年月日（yyyyMMdd）",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region EquipConnType

        public static readonly DictionaryType EquipConnType = new DictionaryType()
        {
            Id = Guid.Parse("9f19eaf3-d6d5-4414-85fe-3d6d64b6a89c"), // 固定的 Guid 值
            DictName = "Equipment Connection Protocol",
            DictType = "EquipConnType",
            OrderNum = 6,
            Remark = "Protocol for equipment connection",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo ModbusTcp = new DictionaryInfo()
        {
            Id = Guid.Parse("bdb9c13c-0620-423b-ae97-e39705294b77"), // 固定的 Guid 值
            DictLabel = "Modbus TCP",
            DictValue = "ModbusTcp",
            ParentId = EquipConnType.Id,
            OrderNum = 50,
            Remark = "Modbus TCP协议",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ModbusRtu = new DictionaryInfo()
        {
            Id = Guid.Parse("1fe64f24-6633-4a1f-a8f2-b51a50701fba"), // 固定的 Guid 值
            DictLabel = "Modbus RTU",
            DictValue = "ModbusRtu",
            ParentId = EquipConnType.Id,
            OrderNum = 51,
            Remark = "Modbus RTU协议",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ModbusUdp = new DictionaryInfo()
        {
            Id = Guid.Parse("6d76f498-e1d1-4636-8de5-803f65907ab5"), // 固定的 Guid 值
            DictLabel = "Modbus UDP",
            DictValue = "ModbusUdp",
            ParentId = EquipConnType.Id,
            OrderNum = 52,
            Remark = "Modbus UDP协议",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ModbusAscii = new DictionaryInfo()
        {
            Id = Guid.Parse("e01f1c48-e25a-4650-b4c0-d0bc53667ca6"), // 固定的 Guid 值
            DictLabel = "Modbus ASCII",
            DictValue = "ModbusAscii",
            ParentId = EquipConnType.Id,
            OrderNum = 53,
            Remark = "Modbus ASCII协议",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Http = new DictionaryInfo()
        {
            Id = Guid.Parse("0ab9ef3b-6497-4335-88de-9d31d1d6fbe9"), // 固定的 Guid 值
            DictLabel = "HTTP",
            DictValue = "Http",
            ParentId = EquipConnType.Id,
            OrderNum = 54,
            Remark = "HTTP协议",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Socket = new DictionaryInfo()
        {
            Id = Guid.Parse("e3820e1b-04b1-460a-8e5a-c9b1d7a6d6fe"), // 固定的 Guid 值
            DictLabel = "Socket",
            DictValue = "Socket",
            ParentId = EquipConnType.Id,
            OrderNum = 55,
            Remark = "Socket协议",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo SerialPort = new DictionaryInfo()
        {
            Id = Guid.Parse("7daaa2cd-bbfc-4a1d-888e-d0f3b0b788d9"), // 固定的 Guid 值
            DictLabel = "串口",
            DictValue = "SerialPort",
            ParentId = EquipConnType.Id,
            OrderNum = 56,
            Remark = "串口通信",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo TcpServer = new DictionaryInfo()
        {
            Id = Guid.Parse("722c4a1b-33d0-4cfc-8db2-d8f3f5c3b4b1"), // 固定的 Guid 值
            DictLabel = "TcpServer",
            DictValue = "TcpServer",
            ParentId = EquipConnType.Id,
            OrderNum = 57,
            Remark = "TCP服务器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo UdpServer = new DictionaryInfo()
        {
            Id = Guid.Parse("9485e0f8-495e-4fd6-9d0c-eaabef91066a"), // 固定的 Guid 值
            DictLabel = "UdpServer",
            DictValue = "UdpServer",
            ParentId = EquipConnType.Id,
            OrderNum = 58,
            Remark = "UDP服务器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region BaudRate

        public static readonly DictionaryType BaudRate = new DictionaryType()
        {
            Id = Guid.Parse("81a6c5d4-b9c1-4f93-b5be-c6f7c6dff0f3"), // 固定的 Guid 值
            DictName = "Baud Rate",
            DictType = "BaudRate",
            OrderNum = 7,
            Remark = "Communication baud rate",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo BaudRate2400 = new DictionaryInfo()
        {
            Id = Guid.Parse("31ac1ded-ff4a-427b-bbc9-2c72049eb510"), // 固定的 Guid 值
            DictLabel = "2400",
            DictValue = "2400",
            ParentId = BaudRate.Id,
            OrderNum = 41,
            Remark = "BaudRate2400",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BaudRate4800 = new DictionaryInfo()
        {
            Id = Guid.Parse("da7cbf0b-6ed0-4042-8306-493ca75e2b18"), // 固定的 Guid 值
            DictLabel = "4800",
            DictValue = "4800",
            ParentId = BaudRate.Id,
            OrderNum = 42,
            Remark = "BaudRate4800",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BaudRate9600 = new DictionaryInfo()
        {
            Id = Guid.Parse("d9f7cb86-596b-4d37-932a-dc6722dc54e7"), // 固定的 Guid 值
            DictLabel = "9600",
            DictValue = "9600",
            ParentId = BaudRate.Id,
            OrderNum = 43,
            Remark = "BaudRate9600",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BaudRate19200 = new DictionaryInfo()
        {
            Id = Guid.Parse("e1ee1250-e6b9-4422-bcb3-de3ad31250cf"), // 固定的 Guid 值
            DictLabel = "19200",
            DictValue = "19200",
            ParentId = BaudRate.Id,
            OrderNum = 44,
            Remark = "BaudRate19200",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BaudRate38400 = new DictionaryInfo()
        {
            Id = Guid.Parse("1375ca1b-b647-4615-aa01-68378b41c9dc"), // 固定的 Guid 值
            DictLabel = "38400",
            DictValue = "38400",
            ParentId = BaudRate.Id,
            OrderNum = 45,
            Remark = "BaudRate38400",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BaudRate57600 = new DictionaryInfo()
        {
            Id = Guid.Parse("2540c2a0-0c44-4aa5-bcc7-04a99df7b08a"), // 固定的 Guid 值
            DictLabel = "57600",
            DictValue = "57600",
            ParentId = BaudRate.Id,
            OrderNum = 46,
            Remark = "BaudRate57600",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BaudRate115200 = new DictionaryInfo()
        {
            Id = Guid.Parse("90345bc1-a7ab-419b-8372-64003874429e"), // 固定的 Guid 值
            DictLabel = "115200",
            DictValue = "115200",
            ParentId = BaudRate.Id,
            OrderNum = 47,
            Remark = "BaudRate115200",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        /* 字节序
        
        public static readonly DictionaryInfo ABCD1 = new DictionaryInfo()
        {
            Id = Guid.Parse("b12593a1-5d8f-4b6d-aee9-91e6250efb21"), // 固定的 Guid 值
            DictLabel = "ABCD",
            DictValue = "ABCD",
            ParentId = BaudRate.Id,
            OrderNum = 41,
            Remark = "ABCD排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BADC1 = new DictionaryInfo()
        {
            Id = Guid.Parse("b34c8319-78ab-4a3e-8b97-5c6f2e3fbc9d"), // 固定的 Guid 值
            DictLabel = "BADC",
            DictValue = "BADC",
            ParentId = BaudRate.Id,
            OrderNum = 42,
            Remark = "BADC排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo CDAB1 = new DictionaryInfo()
        {
            Id = Guid.Parse("53b5e9b4-0575-43c2-93cf-d199e7c8c9ea"), // 固定的 Guid 值
            DictLabel = "CDAB",
            DictValue = "CDAB",
            ParentId = BaudRate.Id,
            OrderNum = 43,
            Remark = "CDAB排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo DCBA1 = new DictionaryInfo()
        {
            Id = Guid.Parse("aab8035c-f1d7-4cc9-800e-d98758f1bc49"), // 固定的 Guid 值
            DictLabel = "DCBA",
            DictValue = "DCBA",
            ParentId = BaudRate.Id,
            OrderNum = 44,
            Remark = "DCBA排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        */

        #endregion

        #region DataOrderType

        public static readonly DictionaryType DataOrderType = new DictionaryType()
        {
            Id = Guid.Parse("48c7b6b8-9c96-4b49-b2de-b31f19f0f949"), // 固定的 Guid 值
            DictName = "Data Order",
            DictType = "DataOrderType",
            OrderNum = 8,
            Remark = "Data character order",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo ABCD = new DictionaryInfo()
        {
            Id = Guid.Parse("b39fa36d-62c1-4cc9-b67e-2767a6a86b71"), // 固定的 Guid 值
            DictLabel = "ABCD",
            DictValue = "ABCD",
            ParentId = DataOrderType.Id,
            OrderNum = 41,
            Remark = "ABCD排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BADC = new DictionaryInfo()
        {
            Id = Guid.Parse("91d3c69e-6a84-4136-ae96-d1c4b5d1dca1"), // 固定的 Guid 值
            DictLabel = "BADC",
            DictValue = "BADC",
            ParentId = DataOrderType.Id,
            OrderNum = 42,
            Remark = "BADC排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo CDAB = new DictionaryInfo()
        {
            Id = Guid.Parse("e933abf7-38a6-4425-bad5-c00f11b7c47e"), // 固定的 Guid 值
            DictLabel = "CDAB",
            DictValue = "CDAB",
            ParentId = DataOrderType.Id,
            OrderNum = 43,
            Remark = "CDAB排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo DCBA = new DictionaryInfo()
        {
            Id = Guid.Parse("d5b0a014-7d8a-4d3c-98b9-e4b6d8d825cf"), // 固定的 Guid 值
            DictLabel = "DCBA",
            DictValue = "DCBA",
            ParentId = DataOrderType.Id,
            OrderNum = 44,
            Remark = "DCBA排列",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region OddEvenCheck

        public static readonly DictionaryType OddEvenCheck = new DictionaryType()
        {
            Id = Guid.Parse("c8d5b789-e7f3-4414-81c6-70fce7ed5b8e"), // 固定的 Guid 值
            DictName = "Odd/Even Check",
            DictType = "OddEvenCheck",
            OrderNum = 9,
            Remark = "Parity check type",
            SoftDeleted = false,
            State = true
        };

        #endregion

        #region ModbusReadType

        public static readonly DictionaryType ModbusReadType = new DictionaryType()
        {
            Id = Guid.Parse("b520e2c5-7cd2-4ec5-8b84-24be0b60b8db"), // 固定的 Guid 值
            DictName = "Modbus Read Type",
            DictType = "ModbusReadType",
            OrderNum = 10,
            Remark = "Modbus read protocol type",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo ReadCoil = new DictionaryInfo()
        {
            Id = Guid.Parse("a0d9f0a7-b3f4-4b98-b6d7-126f8f212fd5"), // 固定的 Guid 值
            DictLabel = "ReadCoil",
            DictValue = "ReadCoil",
            ParentId = ModbusReadType.Id,
            OrderNum = 37,
            Remark = "读线圈",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ReadDiscrete = new DictionaryInfo()
        {
            Id = Guid.Parse("5a9a8589-cfbb-4428-842d-4b7adcb59c9f"), // 固定的 Guid 值
            DictLabel = "ReadDiscrete",
            DictValue = "ReadDiscrete",
            ParentId = ModbusReadType.Id,
            OrderNum = 38,
            Remark = "读离散量",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ReadInput = new DictionaryInfo()
        {
            Id = Guid.Parse("6b91e169-2d90-404b-bb7d-3ec5ed4bcf93"), // 固定的 Guid 值
            DictLabel = "ReadInput",
            DictValue = "ReadInput",
            ParentId = ModbusReadType.Id,
            OrderNum = 39,
            Remark = "读输入寄存器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ReadHoldingRegister = new DictionaryInfo()
        {
            Id = Guid.Parse("4492b728-3277-4c4f-a307-98ec2e957ac6"), // 固定的 Guid 值
            DictLabel = "ReadHoldingRegister",
            DictValue = "ReadHoldingRegister",
            ParentId = ModbusReadType.Id,
            OrderNum = 40,
            Remark = "读保持寄存器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region ModbusWriteType

        public static readonly DictionaryType ModbusWriteType = new DictionaryType()
        {
            Id = Guid.Parse("773db8e1-2062-4850-b6ac-019fced12831"), // 固定的 Guid 值
            DictName = "Modbus Write Type",
            DictType = "ModbusWriteType",
            OrderNum = 11,
            Remark = "Modbus write protocol type",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo WriteCoil = new DictionaryInfo()
        {
            Id = Guid.Parse("fd2b08b9-b945-4f56-9755-d28a1d4f1a92"), // 固定的 Guid 值
            DictLabel = "WriteCoil",
            DictValue = "WriteCoil",
            ParentId = ModbusWriteType.Id,
            OrderNum = 33,
            Remark = "写线圈",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo WriteDiscrete = new DictionaryInfo()
        {
            Id = Guid.Parse("4f1f6e58-1d94-466d-bae2-b990ccf8ec16"), // 固定的 Guid 值
            DictLabel = "WriteDiscrete",
            DictValue = "WriteDiscrete",
            ParentId = ModbusWriteType.Id,
            OrderNum = 34,
            Remark = "写离散量",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo WriteInput = new DictionaryInfo()
        {
            Id = Guid.Parse("5cc23728-7ad6-4599-b4d7-774bb17b9d94"), // 固定的 Guid 值
            DictLabel = "WriteInput",
            DictValue = "WriteInput",
            ParentId = ModbusWriteType.Id,
            OrderNum = 35,
            Remark = "写输入寄存器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo WriteHoldingRegister = new DictionaryInfo()
        {
            Id = Guid.Parse("3ea57d4f-ece0-4021-b6ff-fdabba9113d9"), // 固定的 Guid 值
            DictLabel = "WriteHoldingRegister",
            DictValue = "WriteHoldingRegister",
            ParentId = ModbusWriteType.Id,
            OrderNum = 36,
            Remark = "写保持寄存器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region DataType

        public static readonly DictionaryType DataType = new DictionaryType()
        {
            Id = Guid.Parse("22e5fd4f-282f-4f8f-9735-c62944e64c52"), // 固定的 Guid 值
            DictName = "Data Type",
            DictType = "DataType",
            OrderNum = 12,
            Remark = "Type of data format",
            SoftDeleted = false,
            State = true
        };
        
        public static readonly DictionaryInfo Short = new DictionaryInfo()
        {
            Id = Guid.Parse("d20c1d6e-d7f3-4d6f-b80a-e4e2fcd0b7e4"),  // 固定的 Guid 值
            DictLabel = "Short",
            DictValue = "Short",
            ParentId = DataType.Id,
            OrderNum = 35,
            Remark = "短整型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo UShort = new DictionaryInfo()
        {
            Id = Guid.Parse("0ed1fa65-0b19-4e5d-8c02-f1a8de703774"),  // 固定的 Guid 值
            DictLabel = "UShort",
            DictValue = "UShort",
            ParentId = DataType.Id,
            OrderNum = 36,
            Remark = "无符号短整型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };


        public static readonly DictionaryInfo Int = new DictionaryInfo()
        {
            Id = Guid.Parse("40b2f3a4-7a27-4cf4-88fa-25ec7e2c54b2"),
            DictLabel = "Int",
            DictValue = "Int",
            ParentId = DataType.Id,
            OrderNum = 31,
            Remark = "整型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo UInt = new DictionaryInfo()
        {
            Id = Guid.Parse("5e5783c5-3c19-45eb-b2f7-75e9fe38be1a"),
            DictLabel = "UInt",
            DictValue = "UInt",
            ParentId = DataType.Id,
            OrderNum = 32,
            Remark = "无符号整型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Long = new DictionaryInfo()
        {
            Id = Guid.Parse("3c93e37b-3a74-4aaf-a809-1e27ac5105a7"),
            DictLabel = "Long",
            DictValue = "Long",
            ParentId = DataType.Id,
            OrderNum = 33,
            Remark = "长整型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ULong = new DictionaryInfo()
        {
            Id = Guid.Parse("e68f0710-d465-4e44-b62f-bc3c9a78ab68"),
            DictLabel = "ULong",
            DictValue = "ULong",
            ParentId = DataType.Id,
            OrderNum = 34,
            Remark = "无符号长整型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Float = new DictionaryInfo()
        {
            Id = Guid.Parse("7fa174a2-d2f1-4d61-b453-fb32d0a329db"),
            DictLabel = "Float",
            DictValue = "Float",
            ParentId = DataType.Id,
            OrderNum = 35,
            Remark = "单精度浮点型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Double = new DictionaryInfo()
        {
            Id = Guid.Parse("d2b2391d-116f-4749-b1c2-80b016a65a5f"),
            DictLabel = "Double",
            DictValue = "Double",
            ParentId = DataType.Id,
            OrderNum = 36,
            Remark = "双精度浮点型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Decimal = new DictionaryInfo()
        {
            Id = Guid.Parse("4ed8eaff-62ad-4854-9b30-6f26410c7253"),
            DictLabel = "Decimal",
            DictValue = "Decimal",
            ParentId = DataType.Id,
            OrderNum = 37,
            Remark = "十进制浮点型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Char = new DictionaryInfo()
        {
            Id = Guid.Parse("b0ed0e45-1dbb-40d2-aed0-bfcd2c6c7783"),
            DictLabel = "Char",
            DictValue = "Char",
            ParentId = DataType.Id,
            OrderNum = 38,
            Remark = "字符型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo String = new DictionaryInfo()
        {
            Id = Guid.Parse("f58a50f1-d05f-47a7-8f0c-0e74e9415ad1"),
            DictLabel = "String",
            DictValue = "String",
            ParentId = DataType.Id,
            OrderNum = 39,
            Remark = "字符串型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Bool = new DictionaryInfo()
        {
            Id = Guid.Parse("d01558f3-71d4-4c4b-ae42-29d4043f15b1"),
            DictLabel = "Bool",
            DictValue = "Bool",
            ParentId = DataType.Id,
            OrderNum = 40,
            Remark = "布尔型",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region QualityOfServiceLevel

        public static readonly DictionaryType QualityOfServiceLevel = new DictionaryType()
        {
            Id = Guid.Parse("d8b45f8c-8a52-48d7-84b1-52e900fd826e"), // 固定的 Guid 值
            DictName = "Quality of Service Level",
            DictType = "QualityOfServiceLevel",
            OrderNum = 13,
            Remark = "Service quality level",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo AtMostOnce = new DictionaryInfo()
        {
            Id = Guid.Parse("7e990d4b-9b43-48e2-b682-bfc2b0cc1c79"), // 固定的 Guid 值
            DictLabel = "最多一次",
            DictValue = "AtMostOnce",
            ParentId = QualityOfServiceLevel.Id,
            OrderNum = 28,
            Remark = "最多一次交互",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo AtLeastOnce = new DictionaryInfo()
        {
            Id = Guid.Parse("4c97ad67-c2a1-4643-88bb-d92e7ecfe9d6"), // 固定的 Guid 值
            DictLabel = "最少一次",
            DictValue = "AtLeastOnce",
            ParentId = QualityOfServiceLevel.Id,
            OrderNum = 29,
            Remark = "最少一次交互",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ExactlyOnce = new DictionaryInfo()
        {
            Id = Guid.Parse("e548a221-bf69-47b2-b02b-9d98cba4d2ba"), // 固定的 Guid 值
            DictLabel = "只传一次",
            DictValue = "ExactlyOnce",
            ParentId = QualityOfServiceLevel.Id,
            OrderNum = 30,
            Remark = "只传一次交互",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region MqttSendType

        public static readonly DictionaryType MqttSendType = new DictionaryType()
        {
            Id = Guid.Parse("be9c2d3e-6c76-47e1-bda3-e5c476beff84"), // 固定的 Guid 值
            DictName = "MQTT Send Type",
            DictType = "MqttSendType",
            OrderNum = 14,
            Remark = "MQTT data sending type",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Constant = new DictionaryInfo()
        {
            Id = Guid.Parse("c9c3f2fe-b051-4d7d-b0a0-cf1dbda2b30b"), // 固定的 Guid 值
            DictLabel = "常量",
            DictValue = "Constant",
            ParentId = MqttSendType.Id,
            OrderNum = 26,
            Remark = "常量值",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Code = new DictionaryInfo()
        {
            Id = Guid.Parse("8cf0079d-5cf1-4a0f-960a-658967f3e302"), // 固定的 Guid 值
            DictLabel = "点位编码",
            DictValue = "Code",
            ParentId = MqttSendType.Id,
            OrderNum = 27,
            Remark = "点位编码标识",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region StopBits

        public static readonly DictionaryType StopBits = new DictionaryType()
        {
            Id = Guid.Parse("a58b28d3-5d75-4e69-a278-c1cdef6f7e31"), // 固定的 Guid 值
            DictName = "Stop Bits",
            DictType = "StopBits",
            OrderNum = 15,
            Remark = "Type of stop bits",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo None = new DictionaryInfo()
        {
            Id = Guid.Parse("4ad2a35f-9937-43ec-9d95-c4b62d75b99f"), // 固定的 Guid 值
            DictLabel = "None",
            DictValue = "None",
            ParentId = StopBits.Id,
            OrderNum = 22,
            Remark = "无",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo One = new DictionaryInfo()
        {
            Id = Guid.Parse("83d8a564-8e3c-48b1-b11a-e219dd0e2510"), // 固定的 Guid 值
            DictLabel = "One",
            DictValue = "One",
            ParentId = StopBits.Id,
            OrderNum = 23,
            Remark = "一个",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Two = new DictionaryInfo()
        {
            Id = Guid.Parse("1f7cfed2-bd28-45e1-b8a2-97e9d5c3f62b"), // 固定的 Guid 值
            DictLabel = "Two",
            DictValue = "Two",
            ParentId = StopBits.Id,
            OrderNum = 24,
            Remark = "两个",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo OnePointFive = new DictionaryInfo()
        {
            Id = Guid.Parse("a2e93bc1-4d3c-4699-88c3-f1d0551047ba"), // 固定的 Guid 值
            DictLabel = "OnePointFive",
            DictValue = "OnePointFive",
            ParentId = StopBits.Id,
            OrderNum = 25,
            Remark = "1.5",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region Parity

        public static readonly DictionaryType Parity = new DictionaryType()
        {
            Id = Guid.Parse("fe97729b-df77-4c6f-9a72-f97b4b8c3b42"), // 固定的 Guid 值
            DictName = "Parity",
            DictType = "Parity",
            OrderNum = 16,
            Remark = "Parity check type",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Odd = new DictionaryInfo()
        {
            Id = Guid.Parse("0d06ec6a-70a1-40ac-b36a-833773c2e8f2"), // 固定的 Guid 值
            DictLabel = "Odd",
            DictValue = "Odd",
            ParentId = Parity.Id,
            OrderNum = 26,
            Remark = "奇数",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Even = new DictionaryInfo()
        {
            Id = Guid.Parse("a9be7052-c9eb-4fe5-b8fe-0157cfdd5d5f"), // 固定的 Guid 值
            DictLabel = "Even",
            DictValue = "Even",
            ParentId = Parity.Id,
            OrderNum = 27,
            Remark = "偶数",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Mark = new DictionaryInfo()
        {
            Id = Guid.Parse("e222b6a5-8105-47e2-8c33-68a65a5c4d62"), // 固定的 Guid 值
            DictLabel = "Mark",
            DictValue = "Mark",
            ParentId = Parity.Id,
            OrderNum = 28,
            Remark = "标记",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Space = new DictionaryInfo()
        {
            Id = Guid.Parse("3fe24db6-7b33-4207-9ad7-b6da36e9a0a0"), // 固定的 Guid 值
            DictLabel = "Space",
            DictValue = "Space",
            ParentId = Parity.Id,
            OrderNum = 29,
            Remark = "空间",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo ParityNone = new DictionaryInfo()
        {
            Id = Guid.Parse("b1f4a635-6267-4c67-9e67-71798c870ec3"), // 固定的 Guid 值
            DictLabel = "None",
            DictValue = "None",
            ParentId = Parity.Id,
            OrderNum = 29,
            Remark = "空间",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region 房间类型

        public static readonly DictionaryType RoomType = new DictionaryType()
        {
            Id = Guid.Parse("e7cd9d19-b3f5-4235-88d9-ada7ad2de52c"), // 固定的 Guid 值
            DictName = "房间用途",
            DictType = "RoomType",
            OrderNum = 16,
            Remark = "房间用途",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Laboratory = new DictionaryInfo()
        {
            Id = Guid.Parse("c5d83dbe-0246-4b89-9d43-227f020c464d"), // 固定的 Guid 值
            DictLabel = "试验室",
            DictValue = "Laboratory",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "试验室",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo MonitoringRoom = new DictionaryInfo()
        {
            Id = Guid.Parse("7b8b3956-5a99-464a-8b32-73879e0d1f39"), // 固定的 Guid 值
            DictLabel = "监控室",
            DictValue = "MonitoringRoom",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "监控室",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo StorageRoom = new DictionaryInfo()
        {
            Id = Guid.Parse("083b63b9-4ca5-4879-88a6-4f1f6ec84a36"), // 固定的 Guid 值
            DictLabel = "杂物间",
            DictValue = "StorageRoom",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "杂物间",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Office = new DictionaryInfo()
        {
            Id = Guid.Parse("fe3fe9fc-9b67-4b1d-ae34-22669f689be4"), // 固定的 Guid 值
            DictLabel = "会议室",
            DictValue = "Office",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "会议室",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo BreakRoom = new DictionaryInfo()
        {
            Id = Guid.Parse("f303de2d-bd89-4ac5-8f0a-6eab017adba1"), // 固定的 Guid 值
            DictLabel = "休息室",
            DictValue = "BreakRoom",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "休息室",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Warehouse = new DictionaryInfo()
        {
            Id = Guid.Parse("963906f0-dcb7-42fe-bde8-2c7a0fcbe0ca"), // 固定的 Guid 值
            DictLabel = "仓库",
            DictValue = "Warehouse",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "仓库",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Restroom = new DictionaryInfo()
        {
            Id = Guid.Parse("ed8e08f9-cf62-4648-8aeb-06f7e27edec1"), // 固定的 Guid 值
            DictLabel = "卫生间",
            DictValue = "Restroom",
            ParentId = RoomType.Id,
            OrderNum = 26,
            Remark = "试验室",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region 设备状态

        public static readonly DictionaryType DeviceStatus = new DictionaryType()
        {
            Id = Guid.Parse("d92d83f5-cb9f-4a6f-a7de-b1f95b682f1f"), // 固定的 Guid 值
            DictName = "设备状态",
            DictType = "DeviceStatus",
            OrderNum = 1,
            Remark = "设备状态",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Normal = new DictionaryInfo()
        {
            Id = Guid.Parse("4fa00430-542e-4636-bad6-fc684af50836"), // 固定的 Guid 值
            DictLabel = "正常",
            DictValue = "Normal",
            ParentId = DeviceStatus.Id,
            OrderNum = 1,
            Remark = "设备正常运行",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Lost = new DictionaryInfo()
        {
            Id = Guid.Parse("8c6f2117-9f3a-4f6f-b26f-82be592fbd31"), // 固定的 Guid 值
            DictLabel = "丢失",
            DictValue = "Lost",
            ParentId = DeviceStatus.Id,
            OrderNum = 2,
            Remark = "设备丢失",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo InUse = new DictionaryInfo()
        {
            Id = Guid.Parse("bc9b3897-b6f1-4203-b6b1-82a46f3e6c6e"), // 固定的 Guid 值
            DictLabel = "使用中",
            DictValue = "InUse",
            ParentId = DeviceStatus.Id,
            OrderNum = 3,
            Remark = "设备正在使用中",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion 设备状态

        #region 设备重要度

        public static readonly DictionaryType DeviceLevel = new DictionaryType()
        {
            Id = Guid.Parse("21946842-CDC3-2A0C-0D2A-57AEF39D9533"), // 固定的 Guid 值
            DictName = "设备重要度",
            DictType = "DeviceLevel",
            OrderNum = 1,
            Remark = "设备重要度",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Important = new DictionaryInfo()
        {
            Id = Guid.Parse("D2F4AC22-D9F2-A1E5-2B39-C54472AE992D"), // 固定的 Guid 值
            DictLabel = "关键设备",
            DictValue = "Important",
            ParentId = DeviceLevel.Id,
            OrderNum = 1,
            Remark = "关键设备",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo General = new DictionaryInfo()
        {
            Id = Guid.Parse("75EDDC58-C510-10E8-78B3-F3AF7EEC7E7B"), // 固定的 Guid 值
            DictLabel = "一般设备",
            DictValue = "General",
            ParentId = DeviceLevel.Id,
            OrderNum = 2,
            Remark = "一般设备",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Basic = new DictionaryInfo()
        {
            Id = Guid.Parse("5AE693B2-196C-8CE5-9C12-FE486FCC98BC"), // 固定的 Guid 值
            DictLabel = "普通设备",
            DictValue = "Basic",
            ParentId = DeviceLevel.Id,
            OrderNum = 3,
            Remark = "普通设备",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion 设备状态

        #region 试验系统

        public static readonly DictionaryType testSystem = new DictionaryType()
        {
            Id = Guid.Parse("02D739AF-3A6C-D867-A96C-B5226116C543"), // 固定的 Guid 值
            DictName = "试验系统",
            DictType = "TestSystem",
            OrderNum = 1,
            Remark = "试验系统",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo testSystem1 = new DictionaryInfo()
        {
            Id = Guid.Parse("331ADCC8-201D-294F-3745-77BDC37FCA7A"), // 固定的 Guid 值
            DictLabel = "微波/毫米波复合半实物仿真系统",
            DictValue = "微波/毫米波复合半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "微波/毫米波复合半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem2 = new DictionaryInfo()
        {
            Id = Guid.Parse("543025D1-5FFC-7C3A-4DD7-BB70E02FEBD8"), // 固定的 Guid 值
            DictLabel = "微波寻的半实物仿真系统",
            DictValue = "微波寻的半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "微波寻的半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem3 = new DictionaryInfo()
        {
            Id = Guid.Parse("C2D80616-06C8-0BA5-CDFB-EC52A6E561E6"), // 固定的 Guid 值
            DictLabel = "射频/光学制导半实物仿真系统",
            DictValue = "射频/光学制导半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "射频/光学制导半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem4 = new DictionaryInfo()
        {
            Id = Guid.Parse("E11D4C2C-7A52-4BED-20C0-4E42D7A6E34D"), // 固定的 Guid 值
            DictLabel = "紧缩场射频光学半实物仿真系统",
            DictValue = "紧缩场射频光学半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "紧缩场射频光学半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem5 = new DictionaryInfo()
        {
            Id = Guid.Parse("1A1BA401-383C-0D30-595C-A9260E6076EF"), // 固定的 Guid 值
            DictLabel = "光学复合半实物仿真系统",
            DictValue = "光学复合半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "光学复合半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem6 = new DictionaryInfo()
        {
            Id = Guid.Parse("108E4CDC-DE38-442C-3DC1-D5F283C27464"), // 固定的 Guid 值
            DictLabel = "三通道控制红外制导半实物仿真系统",
            DictValue = "三通道控制红外制导半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "三通道控制红外制导半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem7 = new DictionaryInfo()
        {
            Id = Guid.Parse("4e4b8bfe-f9d8-4ec6-a318-87b9ad474544"), // 固定的 Guid 值
            DictLabel = "低温环境红外制导控制半实物仿真系统",
            DictValue = "低温环境红外制导控制半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "低温环境红外制导控制半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem8 = new DictionaryInfo()
        {
            Id = Guid.Parse("734077ee-b26a-41b6-9d6c-0915a0237ad9"), // 固定的 Guid 值
            DictLabel = "机械式制导控制半实物仿真系统",
            DictValue = "机械式制导控制半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "机械式制导控制半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem9 = new DictionaryInfo()
        {
            Id = Guid.Parse("62689350-5b9a-406e-8b16-5beca4d22b2c"), // 固定的 Guid 值
            DictLabel = "独立回路半实物仿真系统",
            DictValue = "独立回路半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "独立回路半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo testSystem10 = new DictionaryInfo()
        {
            Id = Guid.Parse("32c72aad-9df9-49b9-bb2d-aee068fc1edd"), // 固定的 Guid 值
            DictLabel = "独立回路/可见光制导半实物仿真系统",
            DictValue = "独立回路/可见光制导半实物仿真系统",
            ParentId = testSystem.Id,
            OrderNum = 1,
            Remark = "独立回路/可见光制导半实物仿真系统",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region 表格类型

        public static readonly DictionaryType FormTypes = new DictionaryType()
        {
            Id = Guid.Parse("62071D10-0BD0-95D1-9B76-FA46F6E4A512"), // 固定的 Guid 值
            DictName = "表格类型",
            DictType = "FormTypes",
            OrderNum = 1,
            Remark = "表格类型",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo ParserDevice = new DictionaryInfo()
        {
            Id = Guid.Parse("3B4EBD5B-C59D-03C9-1B55-9BCB19206783"), // 固定的 Guid 值
            DictLabel = "解析器",
            DictValue = "parserDevice",
            ParentId = FormTypes.Id,
            OrderNum = 1,
            Remark = "解析器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Amplifier = new DictionaryInfo()
        {
            Id = Guid.Parse("E30B041F-7F0B-8B56-C49D-33A2F698B7BD"), // 固定的 Guid 值
            DictLabel = "放大器",
            DictValue = "amplifier",
            ParentId = FormTypes.Id,
            OrderNum = 2,
            Remark = "放大器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Sketchy = new DictionaryInfo()
        {
            Id = Guid.Parse("B193C14A-61AC-8DE7-6B62-483216B91EE5"), // 固定的 Guid 值
            DictLabel = "粗控",
            DictValue = "sketchy",
            ParentId = FormTypes.Id,
            OrderNum = 3,
            Remark = "粗控",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        #region 设备采集类型

        public static readonly DictionaryType EquipProtocol = new DictionaryType()
        {
            Id = Guid.Parse("464402A8-4CD7-0139-D3FB-D0CAAA5C3B27"), // 固定的 Guid 值
            DictName = "设备采集类型",
            DictType = "EquipProtocol",
            OrderNum = 1,
            Remark = "设备采集类型",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo RfidReader = new DictionaryInfo()
        {
            Id = Guid.Parse("663658FD-235E-AD58-052E-559688C13234"), // 固定的 Guid 值
            DictLabel = "RFID读写器",
            DictValue = "RfidReader",
            ParentId = EquipProtocol.Id,
            OrderNum = 1,
            Remark = "RFID读写器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo IotServer = new DictionaryInfo()
        {
            Id = Guid.Parse("98D2502F-24C0-0AF9-107A-35D03DE91970"), // 固定的 Guid 值
            DictLabel = "数据采集适配器",
            DictValue = "IotServer",
            ParentId = EquipProtocol.Id,
            OrderNum = 1,
            Remark = "数据采集适配器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };
        public static readonly DictionaryInfo RKServer = new DictionaryInfo()
        {
            Id = Guid.Parse("D4C0907B-DB6B-1587-B57F-DBE1087B1819"), // 固定的 Guid 值
            DictLabel = "温湿度计",
            DictValue = "RKServer",
            ParentId = EquipProtocol.Id,
            OrderNum = 1,
            Remark = "温湿度计",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };
        public static readonly DictionaryInfo CardIssuer = new DictionaryInfo()
        {
            Id = Guid.Parse("D0FBB4B8-A434-4227-94D2-65586C34191B"), // 固定的 Guid 值
            DictLabel = "发卡器",
            DictValue = "CardIssuer",
            ParentId = EquipProtocol.Id,
            OrderNum = 1,
            Remark = "发卡器",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };
        #endregion

        #region 采集工作日

        public static readonly DictionaryType CollectionWorkingDay = new DictionaryType()
        {
            Id = Guid.Parse("A5B796D5-3AAE-DE0C-F11E-AD6E713BAE96"), // 固定的 Guid 值
            DictName = "采集工作日",
            DictType = "CollectionWorkingDay",
            OrderNum = 1,
            Remark = "采集工作日",
            SoftDeleted = false,
            State = true
        };

        public static readonly DictionaryInfo Day1 = new DictionaryInfo()
        {
            Id = Guid.Parse("7C42BE48-3D4D-713A-C5A0-DC6998FF072D"), // 固定的 Guid 值
            DictLabel = "周一",
            DictValue = "Monday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 1,
            Remark = "周一",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day2 = new DictionaryInfo()
        {
            Id = Guid.Parse("F3E9C40C-5748-4F44-9EED-05644D2C28BC"), // 固定的 Guid 值
            DictLabel = "周二",
            DictValue = "Tuesday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 2,
            Remark = "周二",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day3 = new DictionaryInfo()
        {
            Id = Guid.Parse("346A7E94-0A60-4D9E-9020-FAD1A91D2401"),
            DictLabel = "周三",
            DictValue = "Wednesday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 3,
            Remark = "周三",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day4 = new DictionaryInfo()
        {
            Id = Guid.Parse("4A240A5F-F5D4-4F0C-987F-639FB48A6720"),
            DictLabel = "周四",
            DictValue = "Thursday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 4,
            Remark = "周四",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day5 = new DictionaryInfo()
        {
            Id = Guid.Parse("5A3F87F1-F8C1-4C16-A15D-3204FBC9BE2F"),
            DictLabel = "周五",
            DictValue = "Friday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 5,
            Remark = "周五",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day6 = new DictionaryInfo()
        {
            Id = Guid.Parse("B7FB680C-8F1F-46F3-9BC1-4BEABDE2A0B0"),
            DictLabel = "周六",
            DictValue = "Saturday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 6,
            Remark = "周六",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        public static readonly DictionaryInfo Day7 = new DictionaryInfo()
        {
            Id = Guid.Parse("D799021D-0409-40A7-AE46-07B35F67E9C9"),
            DictLabel = "周日",
            DictValue = "Sunday",
            ParentId = CollectionWorkingDay.Id,
            OrderNum = 7,
            Remark = "周日",
            SoftDeleted = false,
            State = true,
            CreationTime = DateTime.Now.ToLocalTime(),
            ListClass = "info"
        };

        #endregion

        public static DictionaryType[] Seeds { get; } =
        {
            EquipMaintenanceType,
            PlanStatus,
            Frequency,
            CodeRuleType,
            DateFormat,
            EquipConnType,
            BaudRate,
            DataOrderType,
            OddEvenCheck,
            ModbusReadType,
            ModbusWriteType,
            DataType,
            QualityOfServiceLevel,
            MqttSendType,
            StopBits,
            Parity,
            RoomType,
            DeviceStatus,
            DeviceLevel,
            testSystem,
            FormTypes,
            EquipProtocol,
            CollectionWorkingDay
        };
    }
}
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Dictionary
{
    public class DictionaryInfo : UniversalEntity, ISoftDelete, IOrder, IState, IAudited, ISeedsGeneratable
    {
        [Description("排序")]
        public int OrderNum { get; set; } = 0;

        [Description("状态")]
        public bool State { get; set; } = true;

        [Description("描述")]
        public string? Remark { get; set; }

        [Description("tag类型")]
        public string? ListClass { get; set; }

        [Description("tagClass")]
        public string? CssClass { get; set; }

        [Description("字典类型")]
        public Guid ParentId { get; set; }

        [Description("字典标签")]
        public string DictLabel { get; set; } = null!;

        [Description("字典值")]
        public string DictValue { get; set; } = string.Empty;

        [Description("是否为该类型的默认值")]
        public bool IsDefault { get; set; }

        [Description("创建时间")]
        public DateTime CreationTime { get; set; }

        [Description("创建者ID")]
        public Guid? CreatorId { get; set; }

        [Description("最后修改者ID")]
        public Guid? LastModifierId { get; set; }

        [Description("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        [Description("软删除标志")]
        public bool SoftDeleted { get; set; }

        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; }
        public static DictionaryInfo[] Seeds { get; } =
        {
            DictionaryType.Check,
            DictionaryType.Maintenance,
            DictionaryType.NotStarted,
            DictionaryType.InProgress,
            DictionaryType.Completed,
            DictionaryType.Hour,
            DictionaryType.Day,
            DictionaryType.Week,
            DictionaryType.Month,
            DictionaryType.Quarter,
            DictionaryType.Year,
            DictionaryType.SerialNumber,
            DictionaryType.Constant1,
            DictionaryType.Date,
            DictionaryType.AttributeValue,
            DictionaryType.YearMonthDay,
            DictionaryType.ModbusTcp,
            DictionaryType.ModbusRtu,
            DictionaryType.ModbusUdp,
            DictionaryType.ModbusAscii,
            DictionaryType.Http,
            DictionaryType.Socket,
            DictionaryType.SerialPort,
            DictionaryType.TcpServer,
            DictionaryType.UdpServer,
            DictionaryType.ABCD1,
            DictionaryType.BADC1,
            DictionaryType.CDAB1,
            DictionaryType.DCBA1,
            DictionaryType.ABCD,
            DictionaryType.BADC,
            DictionaryType.CDAB,
            DictionaryType.DCBA,
            DictionaryType.ReadCoil,
            DictionaryType.ReadDiscrete,
            DictionaryType.ReadInput,
            DictionaryType.ReadHoldingRegister,
            DictionaryType.WriteCoil,
            DictionaryType.WriteDiscrete,
            DictionaryType.WriteInput,
            DictionaryType.WriteHoldingRegister,
            DictionaryType.Short,
            DictionaryType.UShort,
            DictionaryType.Int,
            DictionaryType.UInt,
            DictionaryType.Long,
            DictionaryType.ULong,
            DictionaryType.Float,
            DictionaryType.Double,
            DictionaryType.Decimal,
            DictionaryType.Char,
            DictionaryType.String,
            DictionaryType.Bool,


            DictionaryType.AtMostOnce,
            DictionaryType.AtLeastOnce,
            DictionaryType.ExactlyOnce,
            DictionaryType.Constant,
            DictionaryType.Code,
            DictionaryType.None,
            DictionaryType.One,
            DictionaryType.Two,
            DictionaryType.OnePointFive,
            DictionaryType.Odd,
            DictionaryType.Even,
            DictionaryType.Mark,
            DictionaryType.Space,
            DictionaryType.ParityNone,
            DictionaryType.Laboratory,
            DictionaryType.MonitoringRoom,
            DictionaryType.StorageRoom,
            DictionaryType.Office,
            DictionaryType.BreakRoom,
            DictionaryType.Warehouse,
            DictionaryType.Restroom,

            DictionaryType.Normal,
            DictionaryType.Lost,
            DictionaryType.InUse,

            DictionaryType.testSystem1,
            DictionaryType.testSystem2,
            DictionaryType.testSystem3,
            DictionaryType.testSystem4,
            DictionaryType.testSystem5,
            DictionaryType.testSystem6,

            DictionaryType.ParserDevice,
            DictionaryType.Amplifier,
            DictionaryType.Sketchy,

            DictionaryType.RfidReader,
            DictionaryType.IotServer,
            DictionaryType.RKServer,
            DictionaryType.CardIssuer,

            DictionaryType.Basic,
            DictionaryType.General,
            DictionaryType.Important,

            DictionaryType.Day1,
            DictionaryType.Day2,
            DictionaryType.Day3,
            DictionaryType.Day4,
            DictionaryType.Day5,
            DictionaryType.Day6,
            DictionaryType.Day7
        };

    }
}
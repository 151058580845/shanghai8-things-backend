using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class CollectionConfigReadDto : ReadDto
    {
        /// <summary>
        /// 采集间隔时间（多久采集一次）
        /// </summary>
        public int IntervalTime { get; set; } = 1000;
        /// <summary>
        /// 开始时间(日期时间)
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间(日期时间)
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 一天中的时间，采集开始时间
        /// </summary>
        public DateTime CollectionStartTime { get; set; }
        /// <summary>
        /// 一天中的时间，采集结束时间
        /// </summary>
        public DateTime CollectionEndTime { get; set; }

        /// <summary>
        /// 采集周期
        /// </summary>
        public CyclePeriodEnum CyclePeriod { get; set; }
        /// <summary>
        /// 采集频率(一周2次)
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// 传输模式
        /// </summary>
        public TransmissionMode TransmissionMode { get; set; }

        /// <summary>
        /// 传输目的地
        /// </summary>
        public TranDestination TranDestination { get; set; }
        /// <summary>
        /// 是否启用日志记录
        /// </summary>
        public bool? EnableLogging { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string? LogFilePath { get; set; }
    }

    public class CollectionConfigCreateDto : CreateDto
    {
        /// <summary>
        /// 采集间隔时间（多久采集一次）
        /// </summary>
        public int IntervalTime { get; set; } = 1000;
        /// <summary>
        /// 开始时间(日期时间)
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间(日期时间)
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 一天中的时间，采集开始时间
        /// </summary>
        public DateTime CollectionStartTime { get; set; }
        /// <summary>
        /// 一天中的时间，采集结束时间
        /// </summary>
        public DateTime CollectionEndTime { get; set; }

        /// <summary>
        /// 采集周期
        /// </summary>
        public CyclePeriodEnum CyclePeriod { get; set; }
        /// <summary>
        /// 采集频率(一周2次)
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// 传输模式
        /// </summary>
        public TransmissionMode TransmissionMode { get; set; }

        /// <summary>
        /// 传输目的地
        /// </summary>
        public TranDestination TranDestination { get; set; }
        /// <summary>
        /// 是否启用日志记录
        /// </summary>
        public bool? EnableLogging { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string? LogFilePath { get; set; }
    }

    public class CollectionConfigUpdateDto : UpdateDto
    {
        /// <summary>
        /// 采集间隔时间（多久采集一次）
        /// </summary>
        public int IntervalTime { get; set; } = 1000;
        /// <summary>
        /// 开始时间(日期时间)
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间(日期时间)
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 一天中的时间，采集开始时间
        /// </summary>
        public DateTime CollectionStartTime { get; set; }
        /// <summary>
        /// 一天中的时间，采集结束时间
        /// </summary>
        public DateTime CollectionEndTime { get; set; }

        /// <summary>
        /// 采集周期
        /// </summary>
        public CyclePeriodEnum CyclePeriod { get; set; }
        /// <summary>
        /// 采集频率(一周2次)
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// 传输模式
        /// </summary>
        public TransmissionMode TransmissionMode { get; set; }

        /// <summary>
        /// 传输目的地
        /// </summary>
        public TranDestination TranDestination { get; set; }
        /// <summary>
        /// 是否启用日志记录
        /// </summary>
        public bool? EnableLogging { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string? LogFilePath { get; set; }
    }

    public class CollectionConfigQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 采集周期
        /// </summary>
        public CyclePeriodEnum? CyclePeriod { get; set; }
    }
}

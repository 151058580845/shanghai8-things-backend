using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// 设备维护
    /// </summary>
    public enum EquipMaintenanceStatus
    {
        Preventive,//预防性维护
        Predictive,//预测性维护
        Corrective,//纠正性维护
        Emergency,//紧急维护
        Condition,//条件维护
        Scheduled,//周期性维护
        Replacement,//更换维护
        Servicing,//保养
        Engineering,//工程维护
        Operational,//操作维护
        /*
            1. 预防性维护（Preventive Maintenance）
            定义: 定期对设备进行检查和维护，以防止故障的发生。预防性维护基于时间间隔或使用周期来安排。
            例子: 定期更换油液、清洁滤网、检查电缆连接等。
            2. 预测性维护（Predictive Maintenance）
            定义: 通过监测设备的运行状态和性能数据来预测可能的故障，从而在故障发生之前进行维护。这种方法通常依赖于实时数据分析和诊断技术。
            例子: 使用振动分析、温度监测、声学监测等技术来预测设备故障。
            3. 纠正性维护（Corrective Maintenance）
            定义: 在设备出现故障或性能下降后进行的维护，以恢复设备到正常工作状态。这种维护通常是响应故障或问题的。
            例子: 更换损坏的部件、修复故障的电路、校正设备的偏差等。
            4. 紧急维护（Emergency Maintenance）
            定义: 在设备发生突发性故障或事故时进行的快速维护，以减少生产停机时间和损失。
            例子: 设备突然停止运行，立即进行修复以恢复正常生产。
            5. 条件维护（Condition-Based Maintenance）
            定义: 维护活动根据设备的实际运行条件来决定，而不是基于时间或使用周期。这种维护方式依赖于设备状态的实时监控。
            例子: 当设备的温度、压力或其他关键参数超出预定范围时进行维护。
            6. 周期性维护（Scheduled Maintenance）
            定义: 按照预定的时间表进行的维护，以确保设备在预定周期内得到适当的照顾。这种维护可以是基于设备的使用频率或时间间隔。
            例子: 每月检查设备的关键部件，或每季度进行详细的维护检查。
            7. 更换维护（Replacement Maintenance）
            定义: 定期更换设备的关键部件，即使这些部件还没有完全失效，以防止设备故障的发生。
            例子: 定期更换泵的密封件、滤网、刹车片等。
            8. 保养（Servicing）
            定义: 定期进行的设备保养活动，包括清洁、润滑和调整设备，以保持设备在最佳运行状态。
            例子: 清洁设备外壳、加油润滑部件、校正设备的工作参数等。
            9. 工程维护（Engineering Maintenance）
            定义: 针对设备的设计和工程改进进行的维护，以提高设备的性能和可靠性。
            例子: 对设备进行升级改造，增加新的功能，或优化现有的工程设计。
            10. 操作维护（Operational Maintenance）
            定义: 由设备操作员进行的日常维护活动，以确保设备的正常运行。
            例子: 操作员在班次开始前进行设备检查、监控设备运行状态等。
         */
    }
}

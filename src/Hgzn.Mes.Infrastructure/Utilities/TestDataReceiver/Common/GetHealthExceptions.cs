using Google.Protobuf;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common
{
    public static class GetHealthExceptions
    {
        /// <summary>
        /// 公共定义_雷达转台_ZXWL-SL-2
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetSL_2CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            if (ulDevHealthState[1] == (byte)General_Status.Abnormal) exceptions.Add("自检状态" + General_Status.Abnormal.GetDescription());
            if (ulDevHealthState[2] == (byte)General_Status.Abnormal) exceptions.Add("运行状态" + General_Status.Abnormal.GetDescription());
            return exceptions;
        }

        /// <summary>
        /// 公共定义_固定电源_ZXWL-SL-4
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        public static List<string> GetSL_4CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            if (ulDevHealthState[1] == (byte)General_Status.Abnormal) exceptions.Add("工作状态" + General_Status.Abnormal.GetDescription());
            return exceptions;
        }

        /// <summary>
        /// 公共定义_红外源_ZXWL-SL-7
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        public static List<string> GetSL_7CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            if (ulDevHealthState[1] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("露点温度状态" + General_Require.NotSatisfyRequirement.GetDescription());
            if (ulDevHealthState[2] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("真空度状态" + General_Require.NotSatisfyRequirement.GetDescription());
            if (ulDevHealthState[3] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("冷水机流量状态" + General_Require.NotSatisfyRequirement.GetDescription());
            if (ulDevHealthState[4] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("环境箱温度状态" + General_Require.NotSatisfyRequirement.GetDescription());
            if (ulDevHealthState[5] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("衬底温度状态" + General_Require.NotSatisfyRequirement.GetDescription());
            if (ulDevHealthState[6] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("功率电源状态" + General_Require.NotSatisfyRequirement.GetDescription());
            if (ulDevHealthState[7] == (byte)General_Require.NotSatisfyRequirement) exceptions.Add("控制电源状态" + General_Require.NotSatisfyRequirement.GetDescription());
            return exceptions;
        }

        /// <summary>
        /// 非公共定义(但是我看在挺多地方也通用)_红外转台_ZXWL-SL-3
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetSL_3CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            SL_3_OperationStatusEnum status = ulDevHealthState[1] switch
            {
                0 => SL_3_OperationStatusEnum.NormalStatus,
                1 => SL_3_OperationStatusEnum.SoftwareLimit,
                2 => SL_3_OperationStatusEnum.OverSpeed,
                3 => SL_3_OperationStatusEnum.Runaway,
                4 => SL_3_OperationStatusEnum.FeedbackError,
                _ => throw new ArgumentOutOfRangeException("statusBytes[1]", "无效的操作状态值")
            };
            if (status == SL_3_OperationStatusEnum.NormalStatus) return exceptions;
            exceptions.Add(status.GetDescription());
            return exceptions;
        }

        public static List<string> GetXT_202_SL_3CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            for (int i = 1; i < 6; i++)
            {
                XT_202_SL_3_OperationStatusEnum status = ulDevHealthState[i] switch
                {
                    0 => XT_202_SL_3_OperationStatusEnum.Normal,
                    1 => XT_202_SL_3_OperationStatusEnum.SoftwarePositiveLimit,
                    2 => XT_202_SL_3_OperationStatusEnum.SoftwareNegativeLimit,
                    3 => XT_202_SL_3_OperationStatusEnum.OverSpeed,
                    4 => XT_202_SL_3_OperationStatusEnum.DA_Limit,
                    5 => XT_202_SL_3_OperationStatusEnum.Runaway,
                    6 => XT_202_SL_3_OperationStatusEnum.Locked,
                    7 => XT_202_SL_3_OperationStatusEnum.HomingFailed,
                    8 => XT_202_SL_3_OperationStatusEnum.DriveFault,
                    9 => XT_202_SL_3_OperationStatusEnum.FeedbackError,
                    10 => XT_202_SL_3_OperationStatusEnum.ElectricalSwitchLimit,
                    11 => XT_202_SL_3_OperationStatusEnum.RelativePositionLimit,
                    _ => throw new ArgumentOutOfRangeException(nameof(status), $"无效的操作状态值: {ulDevHealthState[i]} (有效范围: 0-11)")
                };
                if (status == XT_202_SL_3_OperationStatusEnum.Normal) continue;
                exceptions.Add(status.GetDescription());
            }
            return exceptions;
        }

        /// <summary>
        /// 移动电源_ZXWL-SL-5
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        public static List<string> GetXT_0_SL_5HealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            if (ulDevHealthState[1] == (byte)General_Status.Abnormal) exceptions.Add("工作状态" + General_Status.Abnormal.GetDescription());
            return exceptions;
        }
    }
}

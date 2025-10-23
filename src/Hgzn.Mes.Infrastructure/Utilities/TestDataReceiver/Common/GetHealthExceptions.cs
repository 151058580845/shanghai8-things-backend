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
        /// 公共定义_阵列馈电_ZXWL-SL-1
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetXT_103_SL_1CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            if (ulDevHealthState[1] == (byte)XT_103_SL_1_OperationStatusEnum.Limit) exceptions.Add("横移轴状态" + XT_103_SL_1_OperationStatusEnum.Limit.GetDescription());
            if (ulDevHealthState[2] == (byte)XT_103_SL_1_OperationStatusEnum.Limit) exceptions.Add("升降轴状态" + XT_103_SL_1_OperationStatusEnum.Limit.GetDescription());
            if (ulDevHealthState[2] == (byte)XT_103_SL_1_OperationStatusEnum.Limit) exceptions.Add("前进轴状态" + XT_103_SL_1_OperationStatusEnum.Limit.GetDescription());
            if (ulDevHealthState[2] == (byte)XT_103_SL_1_OperationStatusEnum.Limit) exceptions.Add("云台状态" + XT_103_SL_1_OperationStatusEnum.Limit.GetDescription());
            return exceptions;
        }

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
        /// 112_雷达转台_ZXWL-SL-2
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetXT_112_SL_2CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            List<string> axis = new List<string>() { "滚转轴", "俯仰轴", "偏航轴" };
            for (int i = 1; i < axis.Count + 1; i++)
            {
                var statusFlags = (XT_112_SL_2_OperationStatusEnum)ulDevHealthState[i];
                if (statusFlags == XT_112_SL_2_OperationStatusEnum.NormalStatus) continue;
                exceptions.AddRange(Enum.GetValues(typeof(XT_112_SL_2_OperationStatusEnum))
                .Cast<XT_112_SL_2_OperationStatusEnum>()
                .Where(flag => flag != XT_112_SL_2_OperationStatusEnum.NormalStatus)
                .Where(flag => statusFlags.HasFlag(flag))
                .Select(flag => axis[i - 1] + flag.GetDescription()));
            }
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
        /// 121_红外转台_ZXWL-SL-3
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetXT_121_SL_3CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            List<string> axis = new List<string>() { "滚转轴", "偏航轴", "俯仰轴", "高低轴", "方位轴" };
            for (int i = 1; i < axis.Count + 1; i++)
            {
                var statusFlags = (XT_121_SL_3_OperationStatusEnum)ulDevHealthState[i];
                if (statusFlags == XT_121_SL_3_OperationStatusEnum.NormalStatus) continue;
                exceptions.AddRange(Enum.GetValues(typeof(XT_121_SL_3_OperationStatusEnum))
                .Cast<XT_121_SL_3_OperationStatusEnum>()
                .Where(flag => flag != XT_121_SL_3_OperationStatusEnum.NormalStatus)
                .Where(flag => statusFlags.HasFlag(flag))
                .Select(flag => axis[i - 1] + flag.GetDescription()));
            }
            return exceptions;
        }

        /// <summary>
        /// 109_红外转台_ZXWL-SL-3
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetXT_109_SL_3CommonHealthExceptionName(short[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            List<string> axis = new List<string>() { "滚转轴", "俯仰轴", "偏航轴", "高低轴", "方位轴" };
            for (int i = 0; i < axis.Count; i++)
            {
                var statusFlags = (XT_109_SL_3_OperationStatusEnum)ulDevHealthState[i];
                if (statusFlags == XT_109_SL_3_OperationStatusEnum.NormalStatus)
                    return exceptions;
                exceptions.AddRange(Enum.GetValues(typeof(XT_109_SL_3_OperationStatusEnum))
                .Cast<XT_109_SL_3_OperationStatusEnum>()
                .Where(flag => flag != XT_109_SL_3_OperationStatusEnum.NormalStatus)
                .Where(flag => statusFlags.HasFlag(flag))
                .Select(flag => axis[i] + flag.GetDescription()));
            }
            return exceptions;
        }

        /// <summary>
        /// 314_红外转台_ZXWL-SL-3
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetXT_314_SL_3CommonHealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            List<string> axis = new List<string>() { "消旋轴", "短臂轴", "长臂轴" };
            for (int i = 1; i < axis.Count + 1; i++)
            {
                var statusFlags = (XT_314_SL_3_OperationStatusEnum)ulDevHealthState[i];
                if (statusFlags == XT_314_SL_3_OperationStatusEnum.NormalStatus) continue;
                exceptions.AddRange(Enum.GetValues(typeof(XT_314_SL_3_OperationStatusEnum))
                .Cast<XT_314_SL_3_OperationStatusEnum>()
                .Where(flag => flag != XT_314_SL_3_OperationStatusEnum.NormalStatus)
                .Where(flag => statusFlags.HasFlag(flag))
                .Select(flag => axis[i - 1] + flag.GetDescription()));
            }
            return exceptions;
        }

        /// <summary>
        /// 202_红外转台_ZXWL-SL-3
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<string> GetXT_202_SL_3CommonHealthExceptionName(short[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0) General_StatusType.DeviceHealthNotAcquired.GetDescription();
            List<string> axis = new List<string>() { "滚转轴", "偏航轴", "俯仰轴", "高低轴", "方位轴" };
            for (int i = 0; i < axis.Count; i++)
            {
                var statusFlags = (XT_202_SL_3_OperationStatusEnum)ulDevHealthState[i];
                if (statusFlags == XT_202_SL_3_OperationStatusEnum.NormalStatus)
                    return exceptions;
                exceptions.AddRange(Enum.GetValues(typeof(XT_202_SL_3_OperationStatusEnum))
                .Cast<XT_202_SL_3_OperationStatusEnum>()
                .Where(flag => flag != XT_202_SL_3_OperationStatusEnum.NormalStatus)
                .Where(flag => statusFlags.HasFlag(flag))
                .Select(flag => axis[i] + flag.GetDescription()));
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

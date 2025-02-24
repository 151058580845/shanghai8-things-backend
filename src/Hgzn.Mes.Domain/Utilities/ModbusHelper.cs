using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Utilities
{
    /// <summary>
    /// Modbus 帮助类
    /// </summary>
    public static class ModbusHelper
    {
        #region 布尔类型

        /// <summary>
        /// 将读取到的位数据转换为布尔值数组
        /// </summary>
        /// <param name="bits">位数组</param>
        /// <returns>布尔值数组</returns>
        public static bool[] ConvertToBoolArray(byte[] bits)
        {
            bool[] bools = new bool[bits.Length * 8];

            for (int i = 0; i < bits.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bools[i * 8 + j] = (bits[i] & (1 << j)) != 0;
                }
            }

            return bools;
        }

        /// <summary>
        /// 将读取到的单个位转换为布尔值
        /// </summary>
        /// <param name="bits">位数组</param>
        /// <param name="index">位索引</param>
        /// <returns>布尔值</returns>
        public static bool ConvertToBool(byte[] bits, int index)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;

            return (bits[byteIndex] & (1 << bitIndex)) != 0;
        }

        #endregion

        #region 8 位整数

        /// <summary>
        /// 将读取到的寄存器数据转换为字节
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="offset">偏移量</param>
        /// <returns>字节值</returns>
        public static byte ConvertToByte(ushort[] registers, int offset = 0)
        {
            return (byte)(registers[offset] & 0xFF);
        }

        /// <summary>
        /// 将读取到的寄存器数据转换为有符号字节
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="offset">偏移量</param>
        /// <returns>有符号字节值</returns>
        public static sbyte ConvertToSByte(ushort[] registers, int offset = 0)
        {
            return (sbyte)(registers[offset] & 0xFF);
        }

        #endregion

        #region 16 位整数

        /// <summary>
        /// 将读取到的寄存器数据转换为 16 位有符号整数
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="offset">偏移量</param>
        /// <returns>16 位有符号整数</returns>
        public static short ConvertToInt16(ushort[] registers, int offset = 0)
        {
            return (short)registers[offset];
        }

        /// <summary>
        /// 将读取到的寄存器数据转换为 16 位无符号整数
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="offset">偏移量</param>
        /// <returns>16 位无符号整数</returns>
        public static ushort ConvertToUInt16(ushort[] registers, int offset = 0)
        {
            return registers[offset];
        }

        #endregion

        #region 32 位整数

        /// <summary>
        /// 将读取到的寄存器数据转换为 32 位有符号整数
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns>32 位有符号整数</returns>
        public static int ConvertToInt32(ushort[] registers, DataOrderType dataType, int offset = 0)
        {
            byte[] bytes = GetBytesFromRegisters(registers, dataType, 2, offset);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 将读取到的寄存器数据转换为 32 位无符号整数
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns>32 位无符号整数</returns>
        public static uint ConvertToUInt32(ushort[] registers, DataOrderType dataType, int offset = 0)
        {
            byte[] bytes = GetBytesFromRegisters(registers, dataType, 2, offset);
            return BitConverter.ToUInt32(bytes, 0);
        }

        #endregion

        #region 64 位整数

        /// <summary>
        /// 将读取到的寄存器数据转换为 64 位有符号整数
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns>64 位有符号整数</returns>
        public static long ConvertToInt64(ushort[] registers, DataOrderType dataType, int offset = 0)
        {
            byte[] bytes = GetBytesFromRegisters(registers, dataType, 4, offset);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// 将读取到的寄存器数据转换为 64 位无符号整数
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns>64 位无符号整数</returns>
        public static ulong ConvertToUInt64(ushort[] registers, DataOrderType dataType, int offset = 0)
        {
            byte[] bytes = GetBytesFromRegisters(registers, dataType, 4, offset);
            return BitConverter.ToUInt64(bytes, 0);
        }

        #endregion

        #region 浮点数

        /// <summary>
        /// 将读取到的寄存器数据转换为 32 位浮点数（float）
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns>32 位浮点数</returns>
        public static float ConvertToFloat(ushort[] registers, DataOrderType dataType, int offset = 0)
        {
            byte[] bytes = GetBytesFromRegisters(registers, dataType, 2, offset);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// 将读取到的寄存器数据转换为 64 位浮点数（double）
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns>64 位浮点数</returns>
        public static double ConvertToDouble(ushort[] registers, DataOrderType dataType, int offset = 0)
        {
            byte[] bytes = GetBytesFromRegisters(registers, dataType, 4, offset);
            return BitConverter.ToDouble(bytes, 0);
        }

        #endregion

        #region 字符串

        /// <summary>
        /// 将读取到的寄存器数据转换为字符串（ASCII 编码）
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="length">字符串长度（字节数）</param>
        /// <param name="offset">偏移量</param>
        /// <returns>字符串</returns>
        public static string ConvertToString(ushort[] registers, int length, int offset = 0)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length / 2; i++)
            {
                bytes[i * 2] = (byte)(registers[offset + i] >> 8);
                bytes[i * 2 + 1] = (byte)(registers[offset + i] & 0xFF);
            }

            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据字节序从寄存器数组中提取字节数组
        /// </summary>
        /// <param name="registers">寄存器数组</param>
        /// <param name="dataType">字节序类型</param>
        /// <param name="registerCount">需要的寄存器数量</param>
        /// <param name="offset">偏移量</param>
        /// <returns>字节数组</returns>
        private static byte[] GetBytesFromRegisters(ushort[] registers, DataOrderType dataType, int registerCount, int offset)
        {
            byte[] bytes = new byte[registerCount * 2];

            for (int i = 0; i < registerCount; i++)
            {
                bytes[i * 2] = (byte)(registers[offset + i] >> 8);
                bytes[i * 2 + 1] = (byte)(registers[offset + i] & 0xFF);
            }

            switch (dataType)
            {
                case DataOrderType.ABCD:
                    // 默认，无需调整
                    break;
                case DataOrderType.BADC:
                    for (int i = 0; i < registerCount; i++)
                    {
                        SwapBytes(bytes, i * 2);
                    }

                    break;
                case DataOrderType.CDAB:
                    SwapWords(bytes);
                    break;
                case DataOrderType.DCBA:
                    for (int i = 0; i < registerCount; i++)
                    {
                        SwapBytes(bytes, i * 2);
                    }

                    SwapWords(bytes);
                    break;
            }

            return bytes;
        }

        /// <summary>
        /// 交换字节数组中的字节顺序
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始索引</param>
        private static void SwapBytes(byte[] bytes, int index)
        {
            byte temp = bytes[index];
            bytes[index] = bytes[index + 1];
            bytes[index + 1] = temp;
        }

        /// <summary>
        /// 交换字节数组中的字顺序
        /// </summary>
        /// <param name="bytes">字节数组</param>
        private static void SwapWords(byte[] bytes)
        {
            int wordCount = bytes.Length / 2;
            for (int i = 0; i < wordCount / 2; i++)
            {
                int index1 = i * 2;
                int index2 = (wordCount - i - 1) * 2;
                byte temp1 = bytes[index1];
                byte temp2 = bytes[index1 + 1];

                bytes[index1] = bytes[index2];
                bytes[index1 + 1] = bytes[index2 + 1];

                bytes[index2] = temp1;
                bytes[index2 + 1] = temp2;
            }
        }

        #endregion
    }
}

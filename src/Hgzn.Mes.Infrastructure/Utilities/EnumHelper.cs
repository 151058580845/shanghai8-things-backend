using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public static class EnumHelper
    {
        public static New EnumToEnum<New>(this object oldEnum)
        {
            if (oldEnum is null)
            {
                throw new ArgumentNullException(nameof(oldEnum));
            }
            return (New)Enum.ToObject(typeof(New), oldEnum.GetHashCode());
        }

        public static TEnum StringToEnum<TEnum>(this string str)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), str);
        }

        public static List<EnumFieldInfo> GetEnumNames<TEnum>()
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("TEnum must be an enum");
            }
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            List<string> enumValues = new List<string>();
            List<EnumFieldInfo> enumFieldInfos = new List<EnumFieldInfo>();
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == enumType)
                {
                    var n = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                    enumFieldInfos.Add(new EnumFieldInfo()
                    {
                        Name = n.Description,
                        Value = ((int)fieldInfo.GetValue(null)!)
                    });
                }
            }
            return enumFieldInfos;
        }
    }
}

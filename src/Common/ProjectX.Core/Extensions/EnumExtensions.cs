using System;
using System.ComponentModel;
using System.Linq;

namespace ProjectX.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value) =>
            value.GetType().GetField(value.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .FirstOrDefault()?.Description
                    ?? value.ToString();
    }
}

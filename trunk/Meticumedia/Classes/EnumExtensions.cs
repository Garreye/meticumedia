// This extension taken from: http://www.blackwasp.co.uk/EnumDescription.aspx

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),
                                                       false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}

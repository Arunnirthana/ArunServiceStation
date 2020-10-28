using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ArunServiceStation.Models
{
    public class DropDown
    {
        
          
            public Provider Provider { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public static string GetEnumDescription(Enum value)
        {
           FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }

        public enum Provider
        {
            Benze,
            Toyoto,
            Tesla,
            Honda,
            BMW

        }

    public enum TimeSlot
    {
        [Description("1AM-2AM")]
        V1 = 1,
        [Description("2AM-3AM")]
        V2 = 2

    }

}
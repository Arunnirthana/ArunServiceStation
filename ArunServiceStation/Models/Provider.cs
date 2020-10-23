using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArunServiceStation.Models
{
    public class DropDown
    {
        
          
            public Provider Provider { get; set; }
        public TimeSlot TimeSlot { get; set; }
    }

        public enum Provider
        {
            Benze,
            Toyoto,
            Tesla,
            Honda,
            BMW,

        }
    public enum TimeSlot
    {
        "1-2",
        2,
        Tesla,
        Honda,
        BMW,

    }
}
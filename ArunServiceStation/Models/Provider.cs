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

      
    }

        public enum Provider
        {
            Benze,
            Toyoto,
            Tesla,
            Honda,
            BMW

        }

 

}
using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src.Utils
{
    public class EasyConvert
    {
        public static T Convert<T>(object value)
        {
            

            return (T)value;
        }
    }
}
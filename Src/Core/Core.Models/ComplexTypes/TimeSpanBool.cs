using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Models.ComplexTypes
{
    [ComplexType]
    public class TimeSpanBool
    {
        public bool Enabled { get; set; }

        public int TimeInSeconds { get; set; }  //consider changing this to TimeSpan, problem is TimeSpan is not xml serializable...
    }
}

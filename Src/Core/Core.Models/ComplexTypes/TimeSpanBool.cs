using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ComplexTypes
{
    [ComplexType]
    public class TimeSpanBool
    {
        public bool Enabled { get; set; }

        public TimeSpan Time { get; set; }
    }
}

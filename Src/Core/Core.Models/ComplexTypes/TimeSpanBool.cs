using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.ComplexTypes
{
    [ComplexType]
    public class TimeSpanBool
    {
        public bool Enabled { get; set; }

        public int TimeInSeconds { get; set; }  //consider changing this to TimeSpan, problem is TimeSpan is not xml serializable...
    }
}

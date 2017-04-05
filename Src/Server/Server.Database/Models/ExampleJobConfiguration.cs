using Scheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class ExampleJobConfiguration : JobConfiguration
    {
        public int RunTimeSeconds { get; set; }
        public int VariationSeconds { get; set; }
        public int NumberOfItems { get; set; }
        public int ItemVariation { get; set; }
    }
}

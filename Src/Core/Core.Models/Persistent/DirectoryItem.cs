using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Persistent
{
    public class DirectoryItem
    {
        [Key]
        public int DirectoryEntryId { get; set; }

        public string Domain { get; set; }

        public string Name { get; set; }

        public bool IsGroup { get; set; }
    }
}

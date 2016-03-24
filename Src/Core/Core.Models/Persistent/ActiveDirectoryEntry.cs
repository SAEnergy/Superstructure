using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Persistent
{
    public class ActiveDirectoryEntry
    {
        [Key]
        public int ActiveDirectoryEntryId { get; set; }

        public string Domain { get; set; }

        public string Name { get; set; }
    }
}

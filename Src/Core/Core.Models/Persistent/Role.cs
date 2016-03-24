using Core.Models.ComplexTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Persistent
{
    public class Role
    {
        public int RoleId { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public Permission DefaultPermission { get; set; }

        public ICollection<Permission> Permissions { get; set; }

        public ICollection<ActiveDirectoryEntry> ActiveDirectoryEntries { get; set; }

        public AuditInfo AuditInfo { get; set; }
    }
}

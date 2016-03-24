using Core.Models.ComplexTypes;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Persistent
{
    public class Permission
    {
        public int PermissionId { get; set; }

        [StringLength(255)]
        public string ServiceName { get; set; }

        [StringLength(255)]
        public string OperationName { get; set; }

        [StringLength(255)]
        public string FriendlyName { get; set; }

        public bool IsRoot { get; set; }

        public bool Read { get; set; }

        public bool Write { get; set; }

        public bool Execute { get; set; }

        public AuditInfo AuditInfo { get; set; }
    }
}

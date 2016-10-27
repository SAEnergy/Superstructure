using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.ComplexTypes
{
    [ComplexType]
    public class AuditInfo
    {
        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsArchived { get; set; }

        public ArchiveState State { get; set; }
    }
}

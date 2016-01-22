using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.ComplexTypes
{
    [ComplexType]
    public class AuditInfo
    {
        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public ArchiveState State { get; set; }
    }
}

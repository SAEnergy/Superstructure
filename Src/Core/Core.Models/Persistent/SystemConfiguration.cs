using Core.Models.ComplexTypes;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Persistent
{
    public class SystemConfiguration
    {
        [Key]
        public int SystemConfigurationId { get; set; }

        [StringLength(255)]
        public string SectionName { get; set; }

        [StringLength(255)]
        public string ConfigName { get; set; }

        [StringLength(255)]
        public string Value { get; set; }

        public AuditInfo AuditInfo { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportingService.Models
{
    public class KPI
    {
        [Key]
        public int KPIID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string? Definition { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Target { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentValue { get; set; }

        [MaxLength(50)]
        public string? ReportingPeriod { get; set; }
    }
}
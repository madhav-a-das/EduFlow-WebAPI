using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportingService.Models
{
    public class AuditPackage
    {
        [Key]
        public int PackageID { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        [Column(TypeName = "text")]
        public string? ContentsJSON { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? PackageURI { get; set; }
    }
}
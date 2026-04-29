using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportingService.Models
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Scope { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string? ParametersJSON { get; set; }

        [Column(TypeName = "text")]
        public string? MetricsJSON { get; set; }

        public int GeneratedBy { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? ReportURI { get; set; }
    }
}
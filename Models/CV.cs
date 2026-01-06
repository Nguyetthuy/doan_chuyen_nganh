using System.ComponentModel.DataAnnotations;

namespace finder_work.Models
{
    public class CV
    {
        public int CVId { get; set; }
        
        [Required]
        public int ProfileId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string CVTitle { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string TemplateType { get; set; } = "Professional";
        
        [StringLength(20)]
        public string ApprovalStatus { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
        
        [StringLength(500)]
        public string? RejectionReason { get; set; }
        
        public int? ApprovedBy { get; set; }
        
        public DateTime? ApprovedDate { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsPublic { get; set; } = false;
        
        // Navigation properties
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual User? ApprovedByUser { get; set; }
    }
}
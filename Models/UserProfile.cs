namespace finder_work.Models
{
    public class UserProfile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? Summary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? Location { get; set; }
        public string ApprovalStatus { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
        public string? RejectionReason { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual User? ApprovedByUser { get; set; }
        public virtual ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public virtual ICollection<Degree> Degrees { get; set; } = new List<Degree>();
        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public virtual ICollection<UserProfileSkill> UserProfileSkills { get; set; } = new List<UserProfileSkill>();
        public virtual ICollection<UserProfileProfession> UserProfileProfessions { get; set; } = new List<UserProfileProfession>();
        public virtual ICollection<CV> CVs { get; set; } = new List<CV>();
    }
}
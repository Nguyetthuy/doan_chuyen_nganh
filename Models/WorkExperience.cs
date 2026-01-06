namespace finder_work.Models
{
    public class WorkExperience
    {
        public int ExperienceId { get; set; }
        public int ProfileId { get; set; }
        public int? WorkTypeId { get; set; }
        public string? CompanyName { get; set; }
        public string? WorkName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? WorkDescription { get; set; }

        // Navigation properties
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual WorkType? WorkType { get; set; }
    }
}
namespace finder_work.Models
{
    public class WorkType
    {
        public int WorkTypeId { get; set; }
        public string? WorkTypeName { get; set; }

        // Navigation properties
        public virtual ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
    }
}
namespace finder_work.Models
{
    public class Degree
    {
        public int DegreeId { get; set; }
        public int ProfileId { get; set; }
        public int? DegreeTypeId { get; set; }
        public string? DegreeName { get; set; }
        public string? Major { get; set; }
        public string? SchoolName { get; set; }
        public int? GraduationYear { get; set; }
        public string? DegreeImg { get; set; }

        // Navigation properties
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual DegreeType? DegreeType { get; set; }
    }
}
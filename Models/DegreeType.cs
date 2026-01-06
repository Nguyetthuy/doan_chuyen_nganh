namespace finder_work.Models
{
    public class DegreeType
    {
        public int DegreeTypeId { get; set; }
        public string? DegreeTypeName { get; set; }

        // Navigation properties
        public virtual ICollection<Degree> Degrees { get; set; } = new List<Degree>();
    }
}
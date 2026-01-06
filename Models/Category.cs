namespace finder_work.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }

        // Navigation properties
        public virtual ICollection<Profession> Professions { get; set; } = new List<Profession>();
    }
}
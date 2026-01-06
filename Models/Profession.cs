namespace finder_work.Models
{
    public class Profession
    {
        public int ProfessionId { get; set; }
        public int CategoryId { get; set; }
        public string? ProfessionName { get; set; }
        public string? ProfessionDescription { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<UserProfileProfession> UserProfileProfessions { get; set; } = new List<UserProfileProfession>();
    }
}
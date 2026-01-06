namespace finder_work.Models
{
    public class UserProfileProfession
    {
        public int ProfileId { get; set; }
        public int ProfessionId { get; set; }

        // Navigation properties
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual Profession Profession { get; set; } = null!;
    }
}
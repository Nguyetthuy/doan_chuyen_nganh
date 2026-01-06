namespace finder_work.Models
{
    public class UserProfileSkill
    {
        public int ProfileId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
}
namespace finder_work.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public int SkillTypeId { get; set; }
        public string? SkillName { get; set; }
        public string? SkillDescription { get; set; }

        // Navigation properties
        public virtual SkillType SkillType { get; set; } = null!;
        public virtual ICollection<UserProfileSkill> UserProfileSkills { get; set; } = new List<UserProfileSkill>();
    }
}
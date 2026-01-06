namespace finder_work.Models
{
    public class SkillType
    {
        public int SkillTypeId { get; set; }
        public string? SkillTypeName { get; set; }

        // Navigation properties
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
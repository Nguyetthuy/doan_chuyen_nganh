using finder_work.Models;
using Microsoft.EntityFrameworkCore;

namespace finder_work.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();
        public DbSet<WorkType> WorkTypes => Set<WorkType>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<SkillType> SkillTypes => Set<SkillType>();
        public DbSet<Degree> Degrees => Set<Degree>();
        public DbSet<DegreeType> DegreeTypes => Set<DegreeType>();
        public DbSet<Certificate> Certificates => Set<Certificate>();
        public DbSet<CertificateType> CertificateTypes => Set<CertificateType>();
        public DbSet<Profession> Professions => Set<Profession>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<UserProfileSkill> UserProfileSkills => Set<UserProfileSkill>();
        public DbSet<UserProfileProfession> UserProfileProfessions => Set<UserProfileProfession>();
        public DbSet<CV> CVs => Set<CV>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.Avata).HasMaxLength(255);
            });

            // UserProfile configuration
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfiles");
                entity.HasKey(e => e.ProfileId);
                entity.Property(e => e.Summary).HasMaxLength(200);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.ExpectedSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ApprovalStatus).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate).HasDefaultValueSql("GETUTCDATE()");
                
                // Allow multiple profiles per user - NO unique constraint on UserId
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // WorkExperience configuration
            modelBuilder.Entity<WorkExperience>(entity =>
            {
                entity.ToTable("WorkExperiences");
                entity.HasKey(e => e.ExperienceId);
                entity.Property(e => e.ExperienceId).HasColumnName("WorkExperienceId");
                entity.Property(e => e.ProfileId).HasColumnName("UserProfileId");
                entity.Property(e => e.CompanyName).HasMaxLength(100);
                entity.Property(e => e.WorkName).HasMaxLength(100);
                entity.Property(e => e.WorkDescription).HasMaxLength(255);
                
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.WorkExperiences)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.WorkType)
                    .WithMany(wt => wt.WorkExperiences)
                    .HasForeignKey(e => e.WorkTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // WorkType configuration
            modelBuilder.Entity<WorkType>(entity =>
            {
                entity.ToTable("WorkTypes");
                entity.HasKey(e => e.WorkTypeId);
                entity.Property(e => e.WorkTypeName).HasMaxLength(50);
            });

            // Skill configuration
            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skills");
                entity.HasKey(e => e.SkillId);
                entity.Property(e => e.SkillName).HasMaxLength(50);
                entity.Property(e => e.SkillDescription).HasMaxLength(200);
                
                entity.HasOne(e => e.SkillType)
                    .WithMany(st => st.Skills)
                    .HasForeignKey(e => e.SkillTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SkillType configuration
            modelBuilder.Entity<SkillType>(entity =>
            {
                entity.ToTable("SkillTypes");
                entity.HasKey(e => e.SkillTypeId);
                entity.Property(e => e.SkillTypeName).HasMaxLength(50);
            });

            // Degree configuration
            modelBuilder.Entity<Degree>(entity =>
            {
                entity.ToTable("Degrees");
                entity.HasKey(e => e.DegreeId);
                entity.Property(e => e.ProfileId).HasColumnName("UserProfileId");
                entity.Property(e => e.DegreeName).HasMaxLength(100);
                entity.Property(e => e.Major).HasMaxLength(50);
                entity.Property(e => e.SchoolName).HasMaxLength(100);
                entity.Property(e => e.DegreeImg).HasColumnName("Degreeing").HasMaxLength(255);
                
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.Degrees)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.DegreeType)
                    .WithMany(dt => dt.Degrees)
                    .HasForeignKey(e => e.DegreeTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // DegreeType configuration
            modelBuilder.Entity<DegreeType>(entity =>
            {
                entity.ToTable("DegreeTypes");
                entity.HasKey(e => e.DegreeTypeId);
                entity.Property(e => e.DegreeTypeName).HasMaxLength(50);
            });

            // Certificate configuration
            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("Certificates");
                entity.HasKey(e => e.CertificateId);
                entity.Property(e => e.ProfileId).HasColumnName("UserProfileId");
                entity.Property(e => e.CertificateName).HasMaxLength(100);
                entity.Property(e => e.IssueBy).HasColumnName("IssuedBy").HasMaxLength(100);
                entity.Property(e => e.CertificateImg).HasMaxLength(255);
                
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.Certificates)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.CertificateType)
                    .WithMany(ct => ct.Certificates)
                    .HasForeignKey(e => e.CertificateTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CertificateType configuration
            modelBuilder.Entity<CertificateType>(entity =>
            {
                entity.ToTable("CertificateTypes");
                entity.HasKey(e => e.CertificateTypeId);
                entity.Property(e => e.CertificateTypeName).HasMaxLength(50);
            });

            // Profession configuration
            modelBuilder.Entity<Profession>(entity =>
            {
                entity.ToTable("Professions");
                entity.HasKey(e => e.ProfessionId);
                entity.Property(e => e.ProfessionName).HasMaxLength(100);
                entity.Property(e => e.ProfessionDescription).HasMaxLength(200);
                
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Professions)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).HasMaxLength(50);
                entity.Property(e => e.CategoryDescription).HasMaxLength(100);
            });

            // UserProfileSkill configuration (Many-to-Many)
            modelBuilder.Entity<UserProfileSkill>(entity =>
            {
                entity.ToTable("UserProfileSkills");
                entity.HasKey(e => new { e.ProfileId, e.SkillId });
                entity.Property(e => e.ProfileId).HasColumnName("UserProfileId");
                
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.UserProfileSkills)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Skill)
                    .WithMany(s => s.UserProfileSkills)
                    .HasForeignKey(e => e.SkillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserProfileProfession configuration (Many-to-Many)
            modelBuilder.Entity<UserProfileProfession>(entity =>
            {
                entity.ToTable("UserProfileProfessions");
                entity.HasKey(e => new { e.ProfileId, e.ProfessionId });
                entity.Property(e => e.ProfileId).HasColumnName("UserProfileId");
                
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.UserProfileProfessions)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Profession)
                    .WithMany(pr => pr.UserProfileProfessions)
                    .HasForeignKey(e => e.ProfessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CV configuration
            modelBuilder.Entity<CV>(entity =>
            {
                entity.ToTable("CVs");
                entity.HasKey(e => e.CVId);
                entity.Property(e => e.ProfileId).HasColumnName("UserProfileId");
                entity.Property(e => e.CVTitle).HasMaxLength(200).IsRequired();
                entity.Property(e => e.TemplateType).HasMaxLength(50).HasDefaultValue("Professional");
                entity.Property(e => e.ApprovalStatus).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsPublic).HasDefaultValue(false);
                
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.CVs)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedBy)
                    .OnDelete(DeleteBehavior.SetNull);
                    
                // Indexes for performance
                entity.HasIndex(e => e.ProfileId);
                entity.HasIndex(e => e.ApprovalStatus);
                entity.HasIndex(e => e.IsPublic);
                entity.HasIndex(e => e.ApprovedDate);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(e => e.NotificationId);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.Type).HasMaxLength(50).HasDefaultValue("Info");
                entity.Property(e => e.ActionUrl).HasMaxLength(200);
                entity.Property(e => e.ActionText).HasMaxLength(100);
                entity.Property(e => e.IsRead).HasDefaultValue(false);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Indexes for performance
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => new { e.UserId, e.IsRead });
            });
        }
    }
}

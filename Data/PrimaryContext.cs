using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Models;

namespace ScholarshipInfoSystem.Data
{
    public class PrimaryContext : DbContext
    {
        public PrimaryContext(DbContextOptions<PrimaryContext> options) : base(options) { }

        // DbSet declarations for each of your entities
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<ScholarshipType> ScholarshipTypes { get; set; }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<ApplicantSport> ApplicantSports { get; set; }
        public DbSet<CourseCode> CourseCodes { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<HomeDetail> HomeDetails { get; set; }
        public DbSet<Referee> Referees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite keys
            modelBuilder.Entity<ApplicantSport>()
                .HasKey(a => new { a.ApplicantID, a.SportID });

            // Relationships
            modelBuilder.Entity<Applicant>()
                .HasOne(a => a.ContactDetail)
                .WithOne(cd => cd.Applicant)
                .HasForeignKey<ContactDetail>(cd => cd.ApplicantID);

            modelBuilder.Entity<Applicant>()
                .HasOne(a => a.HomeDetail)
                .WithOne(hd => hd.Applicant)
                .HasForeignKey<HomeDetail>(hd => hd.ApplicantID);

            modelBuilder.Entity<Applicant>()
                .HasMany(a => a.CourseCodes)
                .WithOne(cc => cc.Applicant)
                .HasForeignKey(cc => cc.ApplicantID);

            base.OnModelCreating(modelBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Models;

namespace ScholarshipInfoSystem.Data
{
    public class PrimaryContext : DbContext
    {
        public PrimaryContext(DbContextOptions<PrimaryContext> options) : base(options)
        {
        }

        // DbSet declarations for each of your entities
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<ScholarshipType> ScholarshipTypes { get; set; }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<ApplicantSport> ApplicantSports { get; set; }
        public DbSet<ScholarshipApplication> ScholarshipApplications { get; set; }
        public DbSet<ScholarshipOfferHistory> ScholarshipOfferHistories { get; set; }
        public DbSet<HomeDetail> HomeDetails { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<Referee> Referees { get; set; }
        public DbSet<CommitteeMember> CommitteeMembers { get; set; }
        public DbSet<MemberSport> MemberSports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite keys
            modelBuilder.Entity<ApplicantSport>()
                .HasKey(a => new { a.ApplicantID, a.SportID });

            modelBuilder.Entity<MemberSport>()
                .HasKey(m => new { m.MemberID, m.SportID });

            // Configure relationships

            // ApplicantSport relationships
            modelBuilder.Entity<ApplicantSport>()
                .HasOne(a => a.Applicant)
                .WithMany(a => a.ApplicantSports)
                .HasForeignKey(a => a.ApplicantID);

            modelBuilder.Entity<ApplicantSport>()
                .HasOne(a => a.Sport)
                .WithMany(s => s.ApplicantSports)
                .HasForeignKey(a => a.SportID);

            // MemberSport relationships
            modelBuilder.Entity<MemberSport>()
                .HasOne(m => m.CommitteeMember)
                .WithMany(cm => cm.MemberSports)
                .HasForeignKey(m => m.MemberID);

            modelBuilder.Entity<MemberSport>()
                .HasOne(m => m.Sport)
                .WithMany(s => s.MemberSports)
                .HasForeignKey(m => m.SportID);

            // Define other entity relationships if necessary

            base.OnModelCreating(modelBuilder);
        }
    }
}

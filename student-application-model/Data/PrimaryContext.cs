using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using student_application_model.Models;
using StudentApplicationModel.Models;

namespace StudentApplicationModel.Data
{
    public class PrimaryContext : DbContext
    {
        public PrimaryContext(DbContextOptions<PrimaryContext> options)
        : base(options)
        {
            Scholarships = Set<Scholarship>();
            ScholarshipTypes = Set<ScholarshipType>();
            ScholarshipApplications = Set<ScholarshipApplication>();
            ScholarshipOfferHistories = Set<ScholarshipOfferHistory>();
            Applicants = Set<Applicant>();
            ApplicantSports = Set<ApplicantSport>();
            ContactDetails = Set<ContactDetail>();
            HomeDetails = Set<HomeDetail>();
            Referees = Set<Referee>();
            CourseCodes = Set<CourseCode>();
            Campuses = Set<Campus>();
            Sports = Set<Sport>();
            UserSports = Set<UserSport>();
        }

        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<ScholarshipType> ScholarshipTypes { get; set; }
        public DbSet<ScholarshipApplication> ScholarshipApplications { get; set; }
        public DbSet<ScholarshipOfferHistory> ScholarshipOfferHistories { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<ApplicantSport> ApplicantSports { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<HomeDetail> HomeDetails { get; set; }
        public DbSet<Referee> Referees { get; set; }
        public DbSet<CourseCode> CourseCodes { get; set; }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<UserSport> UserSports { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSport>()
                .HasKey(us => new { us.UserID, us.SportID });

            modelBuilder.Entity<UserSport>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSport>()
                .HasOne(us => us.Sport)
                .WithMany(s => s.UserSports)
                .HasForeignKey(us => us.SportID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicantSport>()
                .HasKey(a => new { a.ApplicantID, a.SportID });

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

            modelBuilder.Entity<Applicant>()
                .HasMany(a => a.Referees)
                .WithOne(r => r.Applicant)
                .HasForeignKey(r => r.ApplicantID);


            modelBuilder.Entity<Sport>()
                .HasMany(s => s.ApplicantSports)
                .WithOne(a => a.Sport)
                .HasForeignKey(a => a.SportID);

            modelBuilder.Entity<Sport>()
                .HasMany(s => s.ScholarshipOfferHistories)
                .WithOne(h => h.Sport)
                .HasForeignKey(h => h.SportID);

            modelBuilder.Entity<ScholarshipType>()
                .HasMany(st => st.Scholarships)
                .WithOne(s => s.ScholarshipType)
                .HasForeignKey(s => s.ScholarshipTypeID);

            modelBuilder.Entity<Sport>(entity =>
            {
                entity.ToTable("Sports");
                entity.HasKey(e => e.SportID);
                entity.Property(e => e.SportName).IsRequired();
                entity.Property<int>("SportID").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ScholarshipType>()
                .Property(s => s.PaymentAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Applicant>()
                .HasIndex(a => a.CAONumber)
                .IsUnique();

            modelBuilder.Entity<ScholarshipOfferHistory>()
                .HasIndex(s => new { s.ApplicantID, s.ScholarshipID });

            modelBuilder.Ignore<CommitteeMember>();
            modelBuilder.Ignore<IdentityUser>();
        }
    }
}
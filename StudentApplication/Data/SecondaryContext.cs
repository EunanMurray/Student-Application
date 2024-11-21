using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;
using student_application_model.Models;

namespace ScholarshipInfoSystem.Data
{
    public class SecondaryContext : IdentityDbContext<IdentityUser>
    {
        public SecondaryContext(DbContextOptions<SecondaryContext> options) : base(options)
        {
        }

        public new DbSet<Role> Roles { get; set; }
        public new DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserSport> UserSports { get; set; }
        public new DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<CommitteeMemberSport> CommitteeMemberSports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<ApplicantSport>();

            // Tables excluded from migrations
            modelBuilder.Entity<UserRole>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<UserSport>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<Role>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<RoleClaim>().Metadata.SetIsTableExcludedFromMigrations(true);

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserID, ur.RoleID });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CommitteeMemberSport configuration
            modelBuilder.Entity<CommitteeMemberSport>(entity =>
            {
                entity.ToTable("CommitteeMemberSports");

                entity.HasKey(cms => new { cms.UserId, cms.SportId });

                // Configure the foreign key to AspNetUsers
                entity.HasOne<IdentityUser>()
                    .WithMany()
                    .HasForeignKey(cms => cms.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure SportId as a regular property (not a foreign key)
                entity.Property(cms => cms.SportId)
                    .IsRequired();

            });

            // UserSport configuration
            modelBuilder.Entity<UserSport>(entity =>
            {
                entity.HasKey(us => new { us.UserID, us.SportID });

                entity.HasOne(us => us.User)
                    .WithMany(u => u.UserSports)
                    .HasForeignKey(us => us.UserID);

                entity.HasOne(us => us.Sport)
                    .WithMany(s => s.UserSports)
                    .HasForeignKey(us => us.SportID);
            });

            // RoleClaim configuration
            modelBuilder.Entity<RoleClaim>(entity =>
            {
                entity.HasOne(rc => rc.Role)
                    .WithMany(r => r.RoleClaims)
                    .HasForeignKey(rc => rc.RoleID);
            });
        }
    }
}
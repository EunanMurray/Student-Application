using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;

namespace ScholarshipInfoSystem.Data
{
    public class SecondaryContext : IdentityDbContext<IdentityUser>
    {
        public SecondaryContext(DbContextOptions<SecondaryContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserSport> UserSports { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<ApplicantSport>();

            modelBuilder.Entity<UserRole>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<UserSport>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<Role>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<RoleClaim>().Metadata.SetIsTableExcludedFromMigrations(true);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserID, ur.RoleID });

            modelBuilder.Entity<UserSport>()
                .HasKey(us => new { us.UserID, us.SportID });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID);

            modelBuilder.Entity<UserSport>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSports)
                .HasForeignKey(us => us.UserID);

            modelBuilder.Entity<UserSport>()
                .HasOne(us => us.Sport)
                .WithMany(s => s.UserSports)
                .HasForeignKey(us => us.SportID);

            modelBuilder.Entity<RoleClaim>()
                .HasOne(rc => rc.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(rc => rc.RoleID);
        }
    }
}

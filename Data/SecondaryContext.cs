using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Models;

namespace ScholarshipInfoSystem.Data
{
    public class SecondaryContext : IdentityDbContext<IdentityUser>
    {
        public SecondaryContext(DbContextOptions<SecondaryContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<UserSport> UserSports { get; set; }

        public DbSet<ApplicantSport> ApplicantSports { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<CommitteeMember> CommitteeMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicantSport>()
                .HasKey(ur => new { ur.ApplicantID, ur.SportID });

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

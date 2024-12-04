using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;
using student_application_model.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Sports = Set<IdentitySport>();
        UserSports = Set<UserSport>();
        CommitteeMembers = Set<CommitteeMember>();
    }

    public DbSet<IdentitySport> Sports { get; set; }
    public DbSet<UserSport> UserSports { get; set; }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentitySport>(entity =>
        {
            entity.ToTable("Sports");
            entity.HasKey(e => e.SportID);
            entity.Property(e => e.SportID).ValueGeneratedNever();
            entity.Property(e => e.SportName).IsRequired();
        });

        builder.Entity<UserSport>(entity =>
        {
            entity.ToTable("UserSports");
            entity.HasKey(us => new { us.UserID, us.SportID });
            entity.HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(us => us.Sport)
                .WithMany(s => s.UserSports)
                .HasForeignKey(us => us.SportID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CommitteeMember>(entity =>
        {
            entity.ToTable("CommitteeMembers");
            entity.HasKey(cm => cm.MemberID);
            entity.HasOne(cm => cm.User)
                .WithOne()
                .HasForeignKey<CommitteeMember>(cm => cm.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Ignore<Sport>();
        builder.Ignore<ApplicantSport>();
        builder.Ignore<ScholarshipOfferHistory>();
    }
}
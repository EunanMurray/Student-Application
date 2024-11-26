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
    }
    public DbSet<SportIdentity> Sports { get; set; }
    public DbSet<UserSport> UserSports { get; set; }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SportIdentity>(entity =>
        {
            entity.ToTable("IdentitySports");
            entity.HasKey(e => e.SportID);
            entity.Property(e => e.SportName).IsRequired();
        });

        builder.Entity<UserSport>(entity =>
        {
            entity.ToTable("UserSports");
            entity.HasKey(us => new { us.UserID, us.SportID }); // Composite Key

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
    }
}
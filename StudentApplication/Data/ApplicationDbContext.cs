using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Sport>();
        builder.Ignore<ApplicantSport>();
        builder.Ignore<ScholarshipOfferHistory>();

        builder.Entity<CommitteeMember>(entity =>
        {
            entity.ToTable("CommitteeMembers");
            entity.HasKey(cm => cm.MemberID);
            entity.HasOne(cm => cm.User)
                .WithOne()
                .HasForeignKey<CommitteeMember>(cm => cm.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "AspNetRoles");
        });

        builder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable(name: "AspNetUsers");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
        });
    }
}
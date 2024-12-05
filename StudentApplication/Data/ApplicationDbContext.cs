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
        CommitteeMembers = Set<CommitteeMember>();
    }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Entity<IdentitySport>(entity =>
        //{
        //    entity.ToTable("Sports");
        //    entity.HasKey(e => e.SportID);
        //    entity.Property(e => e.SportID).ValueGeneratedNever();
        //    entity.Property(e => e.SportName).IsRequired();
        //});

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
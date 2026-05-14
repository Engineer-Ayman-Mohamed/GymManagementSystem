using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class MembershipConfigration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.StartDate)
            .IsRequired();

        builder.Property(m => m.EndDate)
            .IsRequired();

        builder.HasOne(m => m.Member)
            .WithMany(mem => mem.Memberships)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Plan)
            .WithMany(p => p.Memberships)
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.MemberId);
        builder.HasIndex(m => m.PlanId);
        builder.HasIndex(m => m.StartDate);
        builder.HasIndex(m => m.EndDate);

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("MembershipDatesCheck",
                "EndDate > StartDate");
        });
    }
}
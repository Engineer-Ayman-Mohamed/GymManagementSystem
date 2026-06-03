using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class PlanConfigration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnType("nvarchar")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.DurationDays)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(p => p.Memberships)
            .WithOne(m => m.Plan)
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("PlanDurationCheck",
                "DurationDays BETWEEN 1 AND 365");
        });

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);
    }
}
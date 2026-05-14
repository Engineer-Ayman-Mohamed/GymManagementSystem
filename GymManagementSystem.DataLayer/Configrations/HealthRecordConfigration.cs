using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class HealthRecordConfigration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Height)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(h => h.Weight)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(h => h.BloodType)
            .HasColumnType("varchar")
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(h => h.Note)
            .HasColumnType("nvarchar")
            .HasMaxLength(500);

        builder.HasOne(h => h.Member)
            .WithOne(m => m.HealthRecord)
            .HasForeignKey<HealthRecord>(h => h.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => h.MemberId)
            .IsUnique();

        builder.Property(h => h.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(h => h.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(h => h.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(h => h.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}
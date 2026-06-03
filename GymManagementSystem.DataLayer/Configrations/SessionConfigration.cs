using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class SessionConfigration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Description)
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(s => s.Capacity)
            .IsRequired();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.HasOne(s => s.Trainer)
            .WithMany(t => t.Sessions)
            .HasForeignKey(s => s.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Sessions)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.TrainerId);
        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => s.StartDate);
        builder.HasIndex(s => s.EndDate);

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("SessionCapacityCheck",
                "Capacity > 0");
            tb.HasCheckConstraint("SessionDatesCheck",
                "EndDate > StartDate");
        });

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(s => s.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
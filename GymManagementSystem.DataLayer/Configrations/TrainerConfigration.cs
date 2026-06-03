using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class TrainerConfigration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.ToTable("Trainers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Email)
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(t => t.Email)
            .IsUnique();

        builder.Property(t => t.Phone)
            .HasColumnType("varchar")
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(t => t.Phone)
            .IsUnique();

        builder.Property(t => t.DateOfBirth)
            .IsRequired();

        builder.Property(t => t.Gender)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(t => t.Address, address =>
        {
            address.Property(a => a.BuildingNumber)
                .HasColumnName("BuildingNumber")
                .IsRequired();

            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasColumnType("varchar")
                .HasMaxLength(30)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasColumnType("varchar")
                .HasMaxLength(30)
                .IsRequired();
        });

        builder.Property(t => t.Specialty)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.HireDate)
            .IsRequired();

        builder.HasMany(t => t.Sessions)
            .WithOne(s => s.Trainer)
            .HasForeignKey(s => s.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.HireDate);

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(t => t.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(t => t.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class MemberConfigration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.Email)
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(m => m.Email)
            .IsUnique();

        builder.Property(m => m.Phone)
            .HasColumnType("varchar")
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(m => m.Phone)
            .IsUnique();

        builder.Property(m => m.DateOfBirth)
            .IsRequired();

        builder.Property(m => m.Gender)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(m => m.Address, address =>
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

        builder.Property(m => m.Photo)
            .HasColumnType("varchar")
            .HasMaxLength(500);

        builder.Property(m => m.JoinDate)
            .IsRequired();

        builder.HasMany(m => m.Memberships)
            .WithOne(mp => mp.Member)
            .HasForeignKey(mp => mp.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Bookings)
            .WithOne(b => b.Member)
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.JoinDate);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(m => m.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
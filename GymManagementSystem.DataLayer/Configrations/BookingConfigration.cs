using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class BookingConfigration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingDate)
            .IsRequired();

        builder.Property(b => b.IsAttended)
            .HasDefaultValue(false);

        builder.HasOne(b => b.Member)
            .WithMany(m => m.Bookings)
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Session)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.MemberId);
        builder.HasIndex(b => b.SessionId);
        builder.HasIndex(b => b.BookingDate);

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(b => b.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(b => b.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(b => !b.IsDeleted);
        
        builder.Property(b => b.CheckedInAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired(false);
    }
}
using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DataLayer.Configrations;

public class CategoryConfigration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CategoryName)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.CreatedBy)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("System");

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);
    }
}
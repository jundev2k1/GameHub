using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class StaffExtensionConfig : IEntityTypeConfiguration<StaffExtension>
{
    public void Configure(EntityTypeBuilder<StaffExtension> builder)
    {
        builder.ToTable("staff_extension");
        builder.HasKey(b => b.Id);

        builder.Property(ba => ba.Id)
            .HasColumnName("staff_extension_id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(ba => ba.StaffId)
            .HasColumnName("staff_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(ba => ba.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(ba => ba.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(ba => ba.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne(su => su.Admin)
            .WithMany(u => u.StaffExtensions)
            .HasForeignKey(su => su.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(su => su.Staff)
            .WithOne(u => u.StaffExtension)
            .HasForeignKey<StaffExtension>(su => su.StaffId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
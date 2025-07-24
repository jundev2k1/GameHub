using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserPassportConfig : IEntityTypeConfiguration<UserPassport>
{
    public void Configure(EntityTypeBuilder<UserPassport> builder)
    {
        builder.Property(ps => ps.PassportImageId)
            .HasColumnName("passport_image_id")
            .IsRequired(false);

        builder.HasOne(ps => ps.PassportImage)
            .WithMany()
            .HasForeignKey(ps => ps.PassportImageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PassportNumber).IsUnique();
    }
}

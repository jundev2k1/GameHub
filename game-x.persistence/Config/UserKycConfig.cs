using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserKycConfig : IEntityTypeConfiguration<UserKyc>
{
    public void Configure(EntityTypeBuilder<UserKyc> builder)
    {
        builder.ToTable("user_kycs");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId).IsUnique();

        builder.Property(m => m.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(m => m.PublicId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.Property(x => x.ResidentialAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.IdNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FrontImageId)
            .IsRequired(false);

        builder.Property(x => x.BackImageId)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<short>()
            .IsRequired();

        builder.Property(x => x.RejectionReason)
            .IsRequired(false)
            .HasMaxLength(4000);

        builder.Property(x => x.SubmittedAt)
            .IsRequired(false)
            .HasColumnType("timestamp");

        builder.Property(x => x.DateReviewed)
            .IsRequired(false)
            .HasColumnType("timestamp");

        builder.Property(x => x.ReviewedBy)
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithOne(u => u.UserKyc)
            .HasForeignKey<UserKyc>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

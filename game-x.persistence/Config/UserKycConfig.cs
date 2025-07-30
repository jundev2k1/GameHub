using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserKycConfig : IEntityTypeConfiguration<UserKyc>
{
    public void Configure(EntityTypeBuilder<UserKyc> builder)
    {
        builder.ToTable("user_kycs");

        builder.HasKey(uk => uk.Id);
        builder.HasIndex(uk => uk.UserId).IsUnique();

        builder.Property(uk => uk.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(uk => uk.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(uk => uk.UserId)
            .IsRequired();

        builder.Property(uk => uk.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(uk => uk.DateOfBirth)
            .IsRequired();

        builder.Property(uk => uk.ResidentialAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(uk => uk.IdNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(uk => uk.FrontImageId)
            .IsRequired(false);

        builder.Property(uk => uk.BackImageId)
            .IsRequired(false);

        builder.Property(uk => uk.Status)
            .HasConversion<short>()
            .IsRequired();

        builder.Property(uk => uk.RejectionReason)
            .IsRequired(false)
            .HasMaxLength(4000);

        builder.Property(uk => uk.SubmittedAt)
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        builder.Property(uk => uk.DateReviewed)
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        builder.Property(uk => uk.ReviewedById)
            .IsRequired(false);

        builder.Property(uk => uk.RejectDetails)
            .IsRequired(false)
            .HasColumnType("jsonb");

        builder.HasOne(uk => uk.User)
            .WithOne(u => u.UserKyc)
            .HasForeignKey<UserKyc>(uk => uk.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uk => uk.ReviewedBy)
            .WithMany()
            .HasForeignKey(uk => uk.ReviewedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(uk => uk.PublicId).IsUnique();
    }
}

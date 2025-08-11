using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class UserBankAccountConfig : IEntityTypeConfiguration<UserBankAccount>
{
    public void Configure(EntityTypeBuilder<UserBankAccount> builder)
    {
        builder.ToTable("user_bank_accounts");

        builder.HasKey(uk => uk.Id);

        builder.Property(uk => uk.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(uba => uba.PublicId)
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(uk => uk.PublicId).IsUnique();

        builder.Property(uba => uba.BankName)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(uba => uba.BankCode)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(uba => uba.AccountName)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(uba => uba.AccountNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(uba => uba.ImageId)
            .IsRequired(false);

        builder.Property(uba => uba.Status)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(uba => uba.RejectionReason)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(true);

        builder.Property(uba => uba.SubmittedAt)
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        builder.Property(uba => uba.DateReviewed)
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        builder.Property(uba => uba.ReviewedById)
            .IsRequired(false);

        builder.Property(uba => uba.RejectDetails)
            .IsRequired(false)
            .HasColumnType("jsonb");

        builder.HasIndex(uba => uba.PublicId).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserBankAccounts)
            .HasForeignKey(uba => uba.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uba => uba.FiatCurrency)
            .WithMany()
            .HasForeignKey(uba => uba.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uba => uba.Image)
            .WithMany()
            .HasForeignKey(uba => uba.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(uba => uba.Status);

        builder.HasIndex(uba => uba.SubmittedAt);
    }
}

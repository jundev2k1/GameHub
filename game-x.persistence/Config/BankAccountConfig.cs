using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("bank_account");
        builder.HasKey(b => b.Id);

        builder.Property(ba => ba.Id)
            .HasColumnName("bank_account_id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(ba => ba.PublicId)
            .HasColumnName("bank_account_code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(ba => ba.BankAccountNumber)
            .HasColumnName("bank_account_number")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ba => ba.BankAccountName)
            .HasColumnName("bank_account_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ba => ba.BankName)
            .HasColumnName("bank_name")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(ba => ba.BranchName)
            .HasColumnName("branch_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ba => ba.CurrencyCode)
            .HasColumnName("currency_code")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(cu => cu.Value, cu => CurrencyUnit.Of(cu));

        builder.Property(ba => ba.AccountType)
            .HasColumnName("account_type")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(type => type.Value, type => AccountType.Of(type));

        builder.Property(ba => ba.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(s => s.Value, s => AccountStatus.Of(s));

        builder.Property(ba => ba.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(ba => ba.IsDefault)
            .HasColumnName("is_default")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ba => ba.IsExternal)
            .HasColumnName("is_external")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ba => ba.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(ba => ba.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne(ba => ba.Owner)
            .WithMany() 
            .HasForeignKey(ba => ba.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.PublicId)
              .IsUnique();
    }
}

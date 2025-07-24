using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class CounterTokenTokenConfig : IEntityTypeConfiguration<CounterToken>
{
    public void Configure(EntityTypeBuilder<CounterToken> builder)
    {
        builder.ToTable("counter_token");
        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Id)
            .HasColumnName("counter_token_id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(ct => ct.CounterId)
            .HasColumnName("counter_id")
            .IsRequired();

        builder.Property(ct => ct.Token)
            .HasColumnName("token")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(ct => ct.IsValid)
            .HasColumnName("is_valid")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ct => ct.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(ct => ct.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(ct => ct.Token).IsUnique();
        builder.HasIndex(ct => ct.CounterId).IsUnique();
    }
}

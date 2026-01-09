using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class S2SClientConfig : IEntityTypeConfiguration<S2SClient>
{
    public void Configure(EntityTypeBuilder<S2SClient> builder)
    {
        builder.ToTable("s2s_clients");

        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id)
            .IsRequired();

        builder.Property(sc => sc.ClientName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(sc => sc.ClientCode)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(sc => sc.IsActive)
            .IsRequired();

        builder.Property(sc => sc.Notes)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(sc => sc.CreatedAt)
            .IsRequired();

        builder.Property(sc => sc.UpdatedAt)
            .IsRequired();
    }
}

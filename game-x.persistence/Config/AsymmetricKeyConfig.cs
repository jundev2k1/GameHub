using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class AsymmetricKeyConfig : IEntityTypeConfiguration<AsymmetricKey>
{
    public void Configure(EntityTypeBuilder<AsymmetricKey> builder)
    {
        builder.HasIndex(x => new { x.Name, x.KeyType, x.Algorithm }).IsUnique();
    }
}

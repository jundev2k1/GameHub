using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class S2SCredentialMaterialConfig : IEntityTypeConfiguration<S2SCredentialMaterial>
{
    public void Configure(EntityTypeBuilder<S2SCredentialMaterial> builder)
    {
        builder.ToTable("s2s_credential_materials");

        builder.HasKey(scm => scm.Id);
        builder.Property(scm => scm.Id)
            .IsRequired();

        builder.Property(scm => scm.CredentialId)
            .IsRequired();

        builder.Property(scm => scm.Type)
            .IsRequired();

        builder.Property(scm => scm.Value)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(scm => scm.IsEncrypted)
            .IsRequired();

        builder.Property(scm => scm.CreatedAt)
            .IsRequired();

        builder.Ignore(scm => scm.UpdatedAt);

        builder.HasOne(scm => scm.Credential)
            .WithMany(sc => sc.Materials)
            .HasForeignKey(scm => scm.CredentialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

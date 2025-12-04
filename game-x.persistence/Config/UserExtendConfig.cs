using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserExtendConfig : IEntityTypeConfiguration<UserExtend>
{
    public void Configure(EntityTypeBuilder<UserExtend> builder)
    {
        builder.ToTable("user_extends");

        builder.HasKey(urex => urex.Id);

        builder.Property(urex => urex.Id)
            .IsRequired();

        builder.Property(urex => urex.GameProviderAccount)
            .HasColumnName("usrex_gp_account")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(urex => urex.GameProviderPassword)
            .HasColumnName("usrex_gp_password")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(urex => urex.GameProviderNickname)
            .HasColumnName("usrex_gp_nickname")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(urex => urex.GameProviderRebateset)
            .HasColumnName("usrex_gp_rebateset")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(urex => urex.GameBaccaratAccount)
            .HasColumnName("usrex_gb_account")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(urex => urex.GameBaccaratPassword)
            .HasColumnName("usrex_gb_password")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(urex => urex.GameBaccaratNickname)
            .HasColumnName("usrex_gb_nickname")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.HasOne(urex => urex.User)
            .WithOne(u => u.UserExtend)
            .HasForeignKey<UserExtend>(urex => urex.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

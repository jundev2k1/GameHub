using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserExtendConfig : IEntityTypeConfiguration<UserExtend>
{
    public void Configure(EntityTypeBuilder<UserExtend> builder)
    {
        builder.ToTable("user_extends");

        builder.HasKey(ue => ue.Id);

        builder.Property(ue => ue.Id)
            .IsRequired();

        builder.Property(ue => ue.GameProviderAccount)
            .HasColumnName("usrex_gp_account")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.GameProviderPassword)
            .HasColumnName("usrex_gp_password")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.GameProviderNickname)
            .HasColumnName("usrex_gp_nickname")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.GameProviderRebateset)
            .HasColumnName("usrex_gp_rebateset")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ue => ue.GameBaccaratUserId)
            .HasColumnName("usrex_gb_userid")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.GameBaccaratAccount)
            .HasColumnName("usrex_gb_account")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.GameBaccaratPassword)
            .HasColumnName("usrex_gb_password")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.GameBaccaratNickname)
            .HasColumnName("usrex_gb_nickname")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);
        
        builder.Property(ue => ue.Etl998ProviderAccount)
            .HasColumnName("usrex_etl998_account")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.Etl998ProviderNickname)
            .HasColumnName("usrex_etl998_nickname")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.Etl998ProviderPassword)
            .HasColumnName("usrex_etl998_password")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.Etl998ProviderTableLimit)
            .HasColumnName("usrex_etl998_table_limit")
            .IsRequired();

        builder.Property(ue => ue.SasSlotAccount)
            .HasColumnName("usrex_slot_account")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(u => u.SasSlotNickname)
            .HasColumnName("usrex_slot_nickname")
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);
        
        builder.Property(u => u.AtgUserName)
            .HasColumnName("atg_user_name")
            .IsRequired(false)
            .HasDefaultValue(string.Empty);

        builder.Property(ue => ue.AtgEmail)
            .HasColumnName("atg_email")
            .IsRequired(false)
            .HasDefaultValue(string.Empty);
        
        builder.Property(ue => ue.AtgFullname)
            .HasColumnName("atg_fullname")
            .IsRequired(false)
            .HasDefaultValue(string.Empty);
        
        builder.HasOne(u => u.User)
            .WithOne(u => u.UserExtend)
            .HasForeignKey<UserExtend>(ue => ue.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
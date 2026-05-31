using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.Nickname)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);
        
        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.CountryCode)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(UserStatus.Active);

        builder.Property(u => u.Notes)
            .IsRequired()
            .HasDefaultValue(string.Empty);
        
        builder.Property(x => x.MemberNumber)
            .HasDefaultValueSql("'User' || nextval('user_member_seq')")
            .ValueGeneratedOnAdd();
        
        builder.HasIndex(x => x.MemberNumber).IsUnique();
        
        builder.HasOne(uk => uk.Avatar)
            .WithMany()
            .HasForeignKey(u => u.AvatarId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

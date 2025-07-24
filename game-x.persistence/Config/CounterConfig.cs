using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class CounterConfig : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        builder.ToTable("counter");
        builder.HasKey(c => new { c.Id });

        builder.Property(c => c.Id)
            .HasColumnName("counter_id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(c => c.PublicId)
            .HasColumnName("counter_code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()"); ;

        builder.Property(c => c.CounterNumber)
            .HasColumnName("counter_number")
            .HasMaxLength(4)
            .IsRequired()
            .HasConversion(c => c.Value, c => CounterNumber.Of(c));

        builder.Property(c => c.CounterName)
            .HasColumnName("counter_name")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(c => c.Location)
            .HasColumnName("location")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(4000)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne(c => c.CounterToken)
            .WithOne(ct => ct.Counter)
            .HasForeignKey<CounterToken>(c => c.CounterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

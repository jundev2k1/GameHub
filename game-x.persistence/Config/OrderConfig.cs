using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("order");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("order_id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(o => o.PublicId)
            .HasColumnName("order_code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(o => o.OrderUid)
            .HasColumnName("order_uid")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(o => o.OrderType)
            .HasColumnName("order_type")
            .IsRequired()
            .HasConversion(o => o.Value, o => OrderType.Of(o));

        builder.Property(o => o.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(o => o.CounterId)
            .HasColumnName("counter_id")
            .IsRequired();

        builder.Property(o => o.StaffId)
            .HasColumnName("staff_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(o => o.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(o => o.PricePerUnit)
            .HasColumnName("price_per_unit")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(o => o.TotalPrice)
            .HasColumnName("total_price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(o => o.CurrencyUnit)
            .HasColumnName("currency_unit")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(o => o.Value, o => CurrencyUnit.Of(o));

        builder.Property(o => o.OrderStatus)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(o => o.Value, o => OrderStatus.Of(o));

        builder.Property(o => o.Notes)
            .HasColumnName("notes")
            .HasMaxLength(4000)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(o => o.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(o => o.PublicId).IsUnique();

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Counter)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CounterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Staff)
            .WithMany()
            .HasForeignKey(o => o.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

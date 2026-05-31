using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static IndexBuilder AddSoftDeleteIndex<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, object?>> indexExpression)
        where TEntity : class
    {
        return builder.HasIndex(indexExpression)
            .HasFilter("\"is_deleted\" = false");
    }
}
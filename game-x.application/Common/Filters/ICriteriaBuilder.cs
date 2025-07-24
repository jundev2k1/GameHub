using System.Linq.Expressions;

namespace game_x.application.Common.Filters;

public interface ICriteriaBuilder<TModel>
{
    IQueryable<TModel> Apply(
        IQueryable<TModel> query,
        IEnumerable<QueryFilter> filters,
        IEnumerable<QuerySort>? sorts = null,
        Func<string, Expression<Func<TModel, bool>>>? searchByKeyCondition = null,
        Dictionary<string, Func<object, Expression<Func<TModel, bool>>>>? options = null);
}

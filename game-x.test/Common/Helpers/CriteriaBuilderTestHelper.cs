using System.Linq.Expressions;
using game_x.application.Common.Filters;
using Moq;

namespace Test.Common.Helpers;

public static class CriteriaBuilderTestHelper
{
    /// <summary>
    ///     Configure this mock so that Apply(...) simply calls through to the real CriteriaBuilder.
    ///     That covers filters, sorts, parsing, etc.  No more hand‐rolled LINQS in your mocks!
    /// </summary>
    public static void Apply<TModel>(Mock<ICriteriaBuilder<TModel>> mock) where TModel : class
    {
        mock
            .Setup(cb => cb.Apply(
                It.IsAny<IQueryable<TModel>>(),
                It.IsAny<IEnumerable<QueryFilter>>(),
                It.IsAny<IEnumerable<QuerySort>>(),
                It.IsAny<Func<string, Expression<Func<TModel, bool>>>>(),
                It.IsAny<Dictionary<string, Func<object, Expression<Func<TModel, bool>>>>>()))
            .Returns((
                IQueryable<TModel> source,
                IEnumerable<QueryFilter> filters,
                IEnumerable<QuerySort> sorts,
                Func<string, Expression<Func<TModel, bool>>> searchExpr,
                Dictionary<string, Func<object, Expression<Func<TModel, bool>>>> options
            ) =>
            {
                var real = new CriteriaBuilder<TModel>();
                return real.Apply(source, filters, sorts, searchExpr, options);
            });
    }
}
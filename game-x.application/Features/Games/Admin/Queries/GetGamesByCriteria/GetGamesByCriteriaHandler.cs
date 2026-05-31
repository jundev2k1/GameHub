using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Games.Mapping;

namespace game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;

public sealed class GetGamesByCriteriaHandler(
    IUserAccessor userAccessor,
    ICriteriaBuilder<Game> builder,
    IGameRepo gameRepo) : IQueryHandler<GetGamesByCriteriaQuery, PaginationResult<GetGamesByCriteriaListItem>>
{
    public async Task<PaginationResult<GetGamesByCriteriaListItem>> Handle(GetGamesByCriteriaQuery request, CancellationToken ct = default)
    {
        var language = userAccessor.GetLanguage();
        var languageCode = LanguageCode.IsValid(language) ? LanguageCode.Of(language) : null;

        var result = await gameRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => languageCode != null
                    ? entity => entity.GameCode.ToLower().Contains(keyword.ToLower())
                        || entity.Translations.Any(t => t.LanguageCode.Equals(languageCode) && t.Name.ToLower().Contains(keyword.ToLower()))
                    : entity => entity.Name.ToLower().Contains(keyword.ToLower())
                        || entity.GameCode.ToLower().Contains(keyword.ToLower()),
                GameFilterExtensions.Options),
            request.PageIndex,
            request.PageSize,
            ct);
        return result.ToSearchResult(language);
    }
}

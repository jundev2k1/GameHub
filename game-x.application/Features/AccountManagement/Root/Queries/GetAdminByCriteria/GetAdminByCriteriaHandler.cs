using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.application.Mappers.Admin;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Root.Queries.GetAdminByCriteria;

public sealed class GetAdminByCriteriaHandler(
    IAppUserRepo appUserRepo,
    ICriteriaBuilder<AppUser> builder,
    AdminMapper adminMapper
)
    : IQueryHandler<GetAdminByCriteriaQuery, PaginationResult<AdminDto>>
{
    public async Task<PaginationResult<AdminDto>> Handle(GetAdminByCriteriaQuery request,
        CancellationToken ct = default)
    {
        var items = await appUserRepo.GetAdminByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user => user.UserName!.StartsWith(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var result = adminMapper.ToAdminDtos(items);
        return result;
    }
}
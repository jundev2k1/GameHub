using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.BankAccountVerifications.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountDetail;

public sealed class GetBankAccountDetailHandler(
    IUserBankAccountRepo userBankAccountRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetBankAccountDetailQuery, BankAccountProfileDto>
{
    public async Task<BankAccountProfileDto> Handle(GetBankAccountDetailQuery request, CancellationToken ct = default)
    {
        var targetBankAccount = await userBankAccountRepo
            .GetByIdAsync(request.BankAccountId, ct);

        var result = targetBankAccount.Adapt<BankAccountProfileDto>();
        result.ImageUrl = await fileManagerCache.GetFileUrl(targetBankAccount.Image, ct);
        return result;
    }
}

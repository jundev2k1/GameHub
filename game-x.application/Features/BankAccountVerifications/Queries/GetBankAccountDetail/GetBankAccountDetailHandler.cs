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
        result.ImageUrl = await GetImageUrl(targetBankAccount.Image);
        return result;
    }

    private async Task<string> GetImageUrl(MediaFile? file)
    {
        if (file is null) return string.Empty;

        var image = await fileManagerCache.GetFileUrl(file);
        return image!.Url;
    }
}

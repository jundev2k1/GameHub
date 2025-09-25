using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Features.Kyc.Queries.GetKycProfile;

public sealed class GetKycProfileHandler(
    IUserRepo userRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetKycProfileQuery, UserKycDto>
{
    public async Task<UserKycDto> Handle(GetKycProfileQuery request, CancellationToken ct = default)
    {
        var targetProfile = await userRepo.GetKycProfileAsync(request.UserId, ct);

        var result = targetProfile.Adapt<UserKycDto>();
        result.FrontImageUrl = await GetImageUrl(targetProfile.FrontImage);
        result.BackImageUrl = await GetImageUrl(targetProfile.BackImage);
        return result;
    }

    private async Task<string> GetImageUrl(MediaFile? file)
    {
        if (file is null) return string.Empty;

        var image = await fileManagerCache.GetFileUrl(file);
        return image!.Url;
    }
}

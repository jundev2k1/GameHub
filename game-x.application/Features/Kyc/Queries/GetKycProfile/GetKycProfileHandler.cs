using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Features.Kyc.Queries.GetKycProfile;

public sealed class GetKycProfileHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IFileStorageService fileStorage) : IQueryHandler<GetKycProfileQuery, UserKycDto>
{
    public async Task<UserKycDto> Handle(GetKycProfileQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetProfile = await userRepo.GetKycProfileAsync(userId, ct);

        var result = targetProfile.Adapt<UserKycDto>();
        result.FrontImageUrl = await GetImageUrl(targetProfile.FrontImage);
        result.BackImageUrl = await GetImageUrl(targetProfile.BackImage);
        return result;
    }

    private async Task<string> GetImageUrl(MediaFile? file)
    {
        if (file is null) return string.Empty;

        var url = await fileStorage.GenerateDownloadUrlAsync(
            bucketName: file.BucketName,
            objectName: file.ObjectName,
            expiry: TimeSpan.FromHours(1));
        return url;
    }
}

using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Staff.Queries.GetUserDetailByStaff;

public sealed class GetUserDetailByIdHandler(
    IAppUserRepo appUserRepo,
    IFileStorageService fileStorageService)
    : IQueryHandler<GetUserDetailByStaffQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserDetailByStaffQuery request, CancellationToken ct = default)
    {
        var userDetail = await appUserRepo.GetUserDetailByIdAsync(request.UserId, ct);
        if (userDetail is null)
            throw new NotFoundException(nameof(AppUser), nameof(AppUser.Id));

        var dto = userDetail.Adapt<UserDetailDto>();
        if ((userDetail.Passport is null) || (userDetail.Passport.PassportImage is null))
            return dto;

        var url = await fileStorageService.GenerateDownloadUrlAsync(
            userDetail.Passport.PassportImage.BucketName,
            userDetail.Passport.PassportImage.ObjectName,
            TimeSpan.FromMinutes(10),
            ct);
        dto.Passport.ImageUrl = url;
        return dto;
    }
}

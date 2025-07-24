using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.AccountManagement.Staff.Commands.CheckExistedUserInfoByStaff;

public sealed class CheckExistedUserInfoByStaffHandler(IUserRepo userRepo)
    : ICommandHandler<CheckExistedUserInfoByStaffCommand, CheckExistedUserInfoByStaffResult>
{
    public async Task<CheckExistedUserInfoByStaffResult> Handle(CheckExistedUserInfoByStaffCommand request, CancellationToken ct = default)
    {
        return new CheckExistedUserInfoByStaffResult(
            IsExistedEmail: await userRepo.IsExistEmailAsync(request.Email, ct),
            IsExistedPhoneNumber: await userRepo.IsExistPhoneNumberAsync(request.PhoneNumber, ct));
    }
}

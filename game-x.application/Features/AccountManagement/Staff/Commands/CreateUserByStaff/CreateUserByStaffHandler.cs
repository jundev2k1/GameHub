using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Staff.Commands.CreateUserByStaff;

public sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserPassportRepo userPassportRepo,
    IStaffUserRepo staffUserRepo,
    ICounterRepo counterRepo,
    IStaffCounterRepo staffCounterRepo,
    IUserAccessor userAccessor) : ICommandHandler<CreateUserByStaffCommand>
{
    public async Task<Unit> Handle(CreateUserByStaffCommand request, CancellationToken ct = default)
    {
        var staffId = userAccessor.GetUserId();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var staffCounter = await staffCounterRepo.GetTrackingLogAsync(staffId, ct);
  
            var targetCounter = await counterRepo.GetByIdAsync(staffCounter.Counter.PublicId, ct);
            if (!targetCounter.IsActive())
                throw new BadRequestException(MessageCode.Counter.CounterInvalid);

            var isExistEmail = await userRepo.IsExistEmailAsync(request.Email, ct);
            if (isExistEmail)
                throw new BadRequestException(MessageCode.User.EmailAlreadyExists);
            
            var isExistPhone = await userRepo.IsExistPhoneNumberAsync(request.PhoneNumber, ct);
            if (isExistPhone)
                throw new BadRequestException(MessageCode.User.PhoneAlreadyExists);
            
            var isExistPassport = await userPassportRepo.IsExistsByPassportNumberAsync(request.Passport.PassportNumber, ct);
            if (isExistPassport)
                throw new BadRequestException(MessageCode.Passport.PassportNumberAlreadyExists);

            var user = await CreateUserAsync(request, ct);

            await CreateStaffUserAsync(staffCounter.Counter.Id, staffId, user.Id, ct);
        }, ct);

        return Unit.Value;
    }

    private async Task<AppUser> CreateUserAsync(CreateUserByStaffCommand request, CancellationToken ct)
    {
        var user = new AppUser()
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CountryCode = request.CountryCode,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            Passport = CreateUserPassport(request.Passport)
        };
        await userRepo.AddUserAsync(user, request.Password, AppRole.Of(AppRoles.User), ct);
        return user;
    }

    private UserPassport CreateUserPassport(PassportDto passportDto)
    {
        return new UserPassport
        {
            PassportType = passportDto.PassportType,
            PassportNumber = passportDto.PassportNumber,
            Country = passportDto.Country,
            IssuedBy = passportDto.IssuedBy,
            FirstName = passportDto.FirstName,
            LastName = passportDto.LastName,
            Gender = passportDto.Gender,
            DateOfBirth = passportDto.DateOfBirth?.ToUniversalTime(),
            Nationality = passportDto.Nationality,
            PlaceOfBirth = passportDto.PlaceOfBirth,
            IssueDate = passportDto.IssueDate?.ToUniversalTime(),
            ExpirationDate = passportDto.ExpirationDate?.ToUniversalTime(),
            Remarks = passportDto.Remarks,
            IsVerified = false
        };
    }

    private async Task CreateStaffUserAsync(int counterId, string staffId, string userId, CancellationToken ct)
    {
        var staffCounter = StaffUser.Create(counterId, staffId, userId);
        await staffUserRepo.AddTrackingLogAsync(staffCounter, ct);
    }
}

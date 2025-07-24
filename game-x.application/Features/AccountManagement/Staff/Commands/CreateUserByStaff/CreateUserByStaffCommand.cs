
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Staff.Commands.CreateUserByStaff;

public record CreateUserByStaffCommand(
    string Email,
    string PhoneNumber,
    string Password,
    PassportDto Passport,
    string CountryCode) : ICommand;

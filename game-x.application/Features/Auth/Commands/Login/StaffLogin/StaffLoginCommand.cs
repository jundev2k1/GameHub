using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.StaffLogin;

public record StaffLoginCommand(Guid CounterId, string UserName, string Password) : ICommand<StaffLoginDto>;

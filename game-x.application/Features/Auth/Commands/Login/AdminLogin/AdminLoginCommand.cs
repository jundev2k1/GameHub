using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.AdminLogin;

public sealed class AdminLoginCommand : ICommand<AdminLoginDto>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}

using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.AdminLogin;

public record AdminLoginCommand(string UserName, string Password) : ICommand<AdminLoginDto>;

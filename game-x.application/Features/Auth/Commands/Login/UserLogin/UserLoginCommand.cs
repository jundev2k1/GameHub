using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.UserLogin;

public record UserLoginCommand(string UserName, string Password) : ICommand<UserLoginDto>;

namespace game_x.application.Features.Auth.Commands.Logout;

public record StaffLogoutCommand(Guid SessionKey) : ICommand;

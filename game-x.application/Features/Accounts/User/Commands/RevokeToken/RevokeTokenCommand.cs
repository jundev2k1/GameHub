namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public record RevokeTokenCommand(string UserId, Guid TokenId) : ICommand;

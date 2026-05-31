namespace game_x.application.Features.S2s.Commands.DeleteS2sClient;

public record DeleteS2sClientCommand(string ClientId) : ICommand;

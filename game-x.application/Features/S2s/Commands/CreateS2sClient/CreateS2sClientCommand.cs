namespace game_x.application.Features.S2s.Commands.CreateS2sClient;

public record CreateS2sClientCommand(string ClientName, string ClientCode, string Notes) : ICommand;

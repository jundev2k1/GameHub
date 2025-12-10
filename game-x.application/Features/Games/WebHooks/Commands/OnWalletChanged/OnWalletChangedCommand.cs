using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.WebHooks.Commands.OnWalletChanged;

public record OnWalletChangedCommand(
    [property: JsonIgnore] string? PartnerName,
    string Account) : ICommand;

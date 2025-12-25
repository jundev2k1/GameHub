using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;

namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchTransfer;

public record SearchTransferCommand(
    string AccountName, 
    string Password,
    string DateStart,
    string DateEnd,
    string CustomerOrderId) : ICommand<IReadOnlyCollection<SearchTransferResponse>>;
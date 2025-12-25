using game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;

namespace game_x.application.Features.Games.Client.Commands.Etl998.CancelTransfer;

public record CancelTransferCommand(
    string AccountName, 
    string Password,
    string DateStart,
    string DateEnd,
    string CustomerOrderId) : ICommand<IReadOnlyCollection<CancelTransferResponse>>;
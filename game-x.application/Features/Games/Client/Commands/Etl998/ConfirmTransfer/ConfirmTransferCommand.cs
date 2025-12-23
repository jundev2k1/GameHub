using game_x.share.ExternalApi.Etl998.Dtos.ConfirmTransfer;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ConfirmTransfer;

public record ConfirmTransferCommand(
    string AccountName, 
    string Password,
    decimal Credit,
    string CreditType,
    string CustomerOrderId) : ICommand<IReadOnlyCollection<ConfirmTransferResponse>>;
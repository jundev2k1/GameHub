using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;

namespace game_x.application.Features.Games.Client.Commands.Etl998.PrepareTransfer;

public record PrepareTransferCommand(
    string AccountName, 
    string Password,
    decimal Credit,
    string CreditType,
    string CustomerOrderId) : ICommand<IReadOnlyCollection<Etl998TransferResponse>>;
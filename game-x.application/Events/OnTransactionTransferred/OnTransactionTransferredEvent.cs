using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;

namespace game_x.application.Events.OnTransactionTransferred;

public record OnTransactionTransferredEvent(TransactionTransferSignalDto Dto) : IApplicationEvent;
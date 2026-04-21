using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.OnTransactionTransferred;

public sealed record OnTransactionTransferredEvent(TransactionTransferDto TxDto, TransactionTransferSignalDto SignalDto) : IApplicationEvent;
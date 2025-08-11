using game_x.application.Features.UserUsdtLedgers.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerDetail;

public record GetUserUsdtLedgerDetailQuery(Guid UsdtLedgerId) : IQuery<UserUsdtLedgerDto>;
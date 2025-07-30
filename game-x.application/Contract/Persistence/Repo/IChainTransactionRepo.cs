
namespace game_x.application.Contract.Persistence.Repo;

public interface IChainTransactionRepo
{
    Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default);

}
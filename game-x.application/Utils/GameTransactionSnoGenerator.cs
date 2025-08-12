using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Utils;

public sealed class GameTransactionSnoGenerator(IGameTransactionRepo gameTransactionRepo)
{
    public async Task<string> GenerateAsync(string prefix, CancellationToken ct = default)
    {
        string sno;
        do
        {
            sno = $"{prefix}{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(10000000, 99999999):D8}{Random.Shared.Next(1000, 9999):D4}";
        } while (await gameTransactionRepo.SnoExistsAsync(sno, ct));

        return sno;
    }
}
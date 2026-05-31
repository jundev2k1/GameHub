using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Utils;

public static class OrderNoGenerator
{
    /// <summary>
    ///     OTC Order Number Generator
    ///     Format：OTC-20250522-173245-X7T3LK
    /// </summary>
    public static string Otc()
        => $"OTC-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{RandomString(6)}";

    /// <summary>
    ///     Generates a random uppercase alphanumeric string of a specified length (excluding easily confused characters)
    /// </summary>
    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
        return new string([.. Enumerable.Range(0, length).Select(_ => chars[Random.Shared.Next(chars.Length)])]);
    }
    
    public static async Task<string> GenerateUniqueOtcOrderNoAsync(ITransactionRepo transactionRepo, CancellationToken ct)
    {
        for (int i = 0; i < 5; i++)
        {
            var candidate = Otc();
            var exists = await transactionRepo.ExistsByOrderNoAsync(candidate, ct);
            if (!exists) return candidate;
        }

        throw new BadRequestException(MessageCode.Transaction.TradeGenerationFailed, "Failed to create OTC order number");
    }
}
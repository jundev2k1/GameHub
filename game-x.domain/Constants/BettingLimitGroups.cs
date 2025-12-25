namespace game_x.domain.Constants;

public sealed record BettingLimitGroup(int LimitId, int MinBet, int MaxBet )
{
    /// <summary>Format required by ETL998: "limitId,minBet,maxBet"</summary>
    public string Value => $"{LimitId},{MinBet},{MaxBet}";
}

public static class BettingLimitGroups
{
    public static readonly IReadOnlyDictionary<int, BettingLimitGroup> All = new Dictionary<int, BettingLimitGroup>
        {
            [1] = new(1, 10, 3000),
            [2] = new(2, 10, 5000),
            [3] = new(3, 10, 8000),
            [4] = new(4, 10, 10000),
            [5] = new(5, 20, 20000),
            [6] = new(6, 20, 30000),
            [7] = new(7, 50, 50000),
            [8] = new(8, 500, 80000),
            [9] = new(9, 1000, 100000),
            [10] = new(10, 2000, 200000),
            [11] = new(11, 3000, 300000),
            [12] = new(12, 5000, 500000),
            [13] = new(13, 50000, 1000000),
            [14] = new(14, 100, 3000),
            [15] = new(15, 100, 5000),
            [16] = new(16, 100, 8000),
            [17] = new(17, 100, 10000),
            [18] = new(18, 100, 20000),
            [19] = new(19, 100, 30000),
            [20] = new(20, 100, 50000),
            [21] = new(21, 50, 10000),
        };
}
namespace game_x.infrastructure.ExternalApi.Etl998.Enums;

/// <summary>
/// A trial account can only be used once and will be closed after logout.
/// </summary>
public enum Etl998AccountType: short
{
    Test = 0,
    Real = 1
}
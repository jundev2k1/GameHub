using game_x.share.ExternalApi.Atg.Dtos.GameProvider;
using game_x.share.ExternalApi.Atg.Dtos.GetGameBalance;
using game_x.share.ExternalApi.Atg.Dtos.GetGames;
using game_x.share.ExternalApi.Atg.Dtos.Register;

namespace game_x.application.Contract.Infrastructure.ExternalApi.Atg;

public interface IAtgService
{
    Task<bool> RegisterAsync(AtgRegisterRequest request);
    Task<GameProviderResponse> GetGameProvidersAsync();
    Task<GetGameBalanceResponse> GetGameBalanceAsync(string username);

    Task<GetGameBalanceResponse> UpdateGameBalanceAsync(
        string username,
        decimal balance,
        string action,
        string transferId);
    Task<ICollection<GameItem>> GetGamesAsync();
    Task<string> PlayGameAsync(string username, string gameCode, string? fullname);
}
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;

namespace game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;

public interface IGameProviderService
{
    Task<LoginResponse> LoginAsync(LoginRequest data, string language, string ip);

    Task<RegisterResponse> RegisterAsync(RegisterRequest data, string language);
}

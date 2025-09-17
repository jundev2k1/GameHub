using game_x.application.Contract.Infrastructure.ExternalApi.Srs;

namespace game_x.infrastructure.ExternalApi.Srs;

public sealed class SrsService(ISrsApi srsApi) : ISrsService
{
    public async Task KickClientAsync(string id)
    {
        await srsApi.KickClientAsync(id);
    }
}

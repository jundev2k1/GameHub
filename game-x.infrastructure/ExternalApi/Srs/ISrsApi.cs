using Refit;

namespace game_x.infrastructure.ExternalApi.Srs;

public interface ISrsApi
{
    /// <summary>Kick client API</summary>
    [Delete("/api/v1/clients/{id}")]
    Task KickClientAsync(string id);
}

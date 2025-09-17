namespace game_x.application.Contract.Infrastructure.ExternalApi.Srs;

public interface ISrsService
{
    Task KickClientAsync(string id);
}

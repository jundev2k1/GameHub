namespace game_x.persistence.Interceptors;

public interface ISaveChangesInterceptor
{
    Task OnBeforeSaveAsync(GameXContext context, CancellationToken ct = default);
    Task OnAfterSaveAsync(GameXContext context, CancellationToken ct = default);
}

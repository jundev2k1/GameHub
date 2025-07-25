namespace game_x.persistence.Interceptors;

public sealed class TimestampInterceptor : ISaveChangesInterceptor
{
    public Task OnBeforeSaveAsync(GameXContext context, CancellationToken cancellationToken = default)
    {
        var entities = context.ChangeTracker.Entries<IEntity>()
            .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified);
        foreach (var entry in entities)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task OnAfterSaveAsync(GameXContext context, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

namespace game_x.application.Contract.Persistence.Repo;

public interface IMessageRepo
{
    Task AddAsync(Message msg, CancellationToken ct = default);
}
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public class MessageRepo(GameXContext context): IMessageRepo, IRepository
{
    public async Task AddAsync(Message msg, CancellationToken ct = default)
    {
        await context.Messages.AddAsync(msg, ct);
    }
}
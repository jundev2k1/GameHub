using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public class MessageMentionRepo(GameXContext context): IMessageMentionRepo, IRepository
{
    public async Task AddAsync(MessageMention msgMention, CancellationToken ct = default)
    {
        await context.MessageMentions.AddAsync(msgMention, ct);
    }
    
    public async Task BulkAddAsync(IEnumerable<MessageMention> mentions, CancellationToken ct = default)
    {
        var distinct = mentions
            .GroupBy(m => new { m.UserId, m.MessageId })
            .Select(g => g.First());

        await context.MessageMentions.AddRangeAsync(distinct, ct);
        await context.SaveChangesAsync(ct);
    }
}
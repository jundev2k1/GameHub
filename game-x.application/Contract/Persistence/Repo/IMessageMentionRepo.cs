namespace game_x.application.Contract.Persistence.Repo;

public interface IMessageMentionRepo
{
    Task AddAsync(MessageMention msgMention, CancellationToken ct = default);
    Task BulkAddAsync(IEnumerable<MessageMention> mentions, CancellationToken ct = default);
}
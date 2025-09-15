namespace game_x.application.Contract.Persistence.Identity;

public interface IConversationService
{
    Task<Guid> EnsureForPair(string userA, string userB, CancellationToken ct);

}
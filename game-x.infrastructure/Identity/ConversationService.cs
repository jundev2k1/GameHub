using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.infrastructure.Identity;

public sealed class ConversationService(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo): IConversationService, IServices
{
    public async Task<Guid> EnsureForPair(string userA, string userB, CancellationToken ct)
    {
        if (userA == userB)
            throw new BadRequestException(MessageCode.Chatting.FailToStartSelfDm);

        // Find a direct conversation with exactly two members, A and B
        var existing = await conversationRepo.FindForPairAsync(userA, userB, ct);
        if (existing != null) return existing.PublicId;

        // Create new if missing
        var conv = Conversation.Create(ConversationType.Direct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await conversationRepo.AddAsync(conv, ct);
            conv.Members.Add(ConversationMember.Create(conv, userA, RoleInConversation.Member));
            conv.Members.Add(ConversationMember.Create(conv, userB, RoleInConversation.Member));
        }, ct);
        
        return conv.PublicId;
    }
}
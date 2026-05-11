namespace game_x.persistence.Seeds.Seeders;

public sealed class ConversationSeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        var exists = await context.Conversations
            .AnyAsync(c => c.Type == ConversationType.Public, ct);
        if (!exists)
        {
            var conv = Conversation.Create(type: ConversationType.Public);
            context.Conversations.Add(conv);
        }
    }
}
using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Helper;
using Mapster;

namespace game_x.persistence.Repo;

public class MessageAttachmentRepo(GameXContext context): IMessageAttachmentRepo, IRepository
{
    public async Task AddAsync(MessageAttachment msgAttachment, CancellationToken ct = default)
    {
        await context.MessageAttachments.AddAsync(msgAttachment, ct);
    }
}
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Contract.Persistence.Repo;

public interface IMessageAttachmentRepo
{
    Task AddAsync(MessageAttachment msgAttachment, CancellationToken ct = default);
}
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Identity;

public interface IMessageService
{
    Task<CursorResult<ListedMessageDto>> GetByCursorAsync(Guid convId, int limit, string? cursor, CancellationToken ct);
    Task<ListedMessageDto> GetMessageDtoAsync(MessageDto msg, CancellationToken ct);
    Task CreateMessageAttachmentsAsync(
        Message msg,
        IReadOnlyList<FileUpload>? attachments,
        CancellationToken ct);
}
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Infrastructure.Attachments;


public interface IAttachmentBinder
{
    Task<(int? mediaFileId, AttachmentBindingStatus status, string? error)>
        BindAsync(AttachmentInputDto input, string addedByUserId, CancellationToken ct);
}
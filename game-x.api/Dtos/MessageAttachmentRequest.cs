namespace game_x.api.Dtos;

public record MessageAttachmentRequest(
    string? Text,
    Guid? ReplyToMessageId,
    string ClientLocalId,
    ICollection<IFormFile> Attachments);
namespace game_x.api.Dtos;

public record MessageAttachmentRequest(
    string? Text,
    Guid? ReplyToMessageId,
    ICollection<IFormFile> Attachments);

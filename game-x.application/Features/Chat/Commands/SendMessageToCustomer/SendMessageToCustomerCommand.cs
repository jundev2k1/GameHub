using game_x.application.Common.Files;

namespace game_x.application.Features.Chat.Commands.SendMessageToCustomer;

public sealed record SendMessageToCustomerCommand(
    Guid ConversationId,
    string Text,
    Guid? ReplyToMessageId,
    IReadOnlyList<FileUpload>? Attachments
) : IRequest<Unit>;
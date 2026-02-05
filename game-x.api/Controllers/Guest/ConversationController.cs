using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Features.Chat.Commands.SendMessage;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListMyConversationsForGuest;

namespace game_x.api.Controllers.Guest;

[Route("api/guest")]
[AllowAnonymous]
public class ConversationController : BaseApiController
{
    /// <summary>List conversations for current logged-in user</summary>
    [HttpPost("start")]
    public IActionResult StartGuest()
    {
        var guestId = Guid.NewGuid().ToString();
        return ApiResponseFactory.Ok(guestId);
    }
    
    /// <summary>
    /// List conversations for current logged-in user
    /// </summary>
    [HttpGet("conversations/me")]
    public async Task<IActionResult> GetSupportConversationAsync([FromHeader] string guestId)
    {
        var query = new ListMyConversationsForGuestQuery(GuestId: guestId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("conversations/{convId:guid}/messages")]
    public async Task<IActionResult> ListMessagesInConversationAsync(
        Guid convId,
        [FromHeader] string guestId,
        [AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMessagesInConversationQuery(
            ActorId: guestId,
            ConvId: convId,
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("messages")]
    public async Task<IActionResult> SendSupportMessagesAttachmentAsync([FromHeader] string guestId, [FromForm] MessageAttachmentRequest formData)
    {
        try
        {
            var command = formData.Adapt<SendSupportMessageCommand>() with
            {
                SenderActorId = guestId,
                ClientLocalId = formData.ClientLocalId,
                ReplyToMessageId = formData.ReplyToMessageId,
                Attachments = formData.Attachments.Select(FileUpload.FromFormFile).ToList()
            };
            var result = await Mediator.Send(command);
            return ApiResponseFactory.Ok(result);
        }
        catch
        {
            return ApiResponseFactory.BadRequest(
                code: MessageCode.Chatting.FailToSendMessage,
                errorDetail: new {formData.ClientLocalId});
        }
    }
    
    [HttpPost("conversations/{convId:guid}/messages")]
    public async Task<IActionResult> SendSupportMessagesAttachmentAsync(
        Guid convId,
        [FromHeader] string guestId, 
        [FromForm] MessageAttachmentRequest formData)
    {
        try
        {
            var command = formData.Adapt<SendMessageCommand>() with
            {
                ConversationId = convId,
                SenderActorId = guestId,
                ClientLocalId = formData.ClientLocalId,
                ReplyToMessageId = formData.ReplyToMessageId,
                Attachments = formData.Attachments.Select(FileUpload.FromFormFile).ToList(),
                Kind = MessageKind.Attachment
            };
            var result = await Mediator.Send(command);
            return ApiResponseFactory.Ok(result);
        }
        catch
        {
            return ApiResponseFactory.BadRequest(
                code: MessageCode.Chatting.FailToSendMessage,
                errorDetail: new {formData.ClientLocalId});
        }
    }
}
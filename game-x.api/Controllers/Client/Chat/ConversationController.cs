using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListMyConversationsForClient;
using game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

namespace game_x.api.Controllers.Client.Chat;

[Authorize(Roles = AppRoles.User)]
[Route("api/user")]
public class ConversationController(IUserAccessor userAccessor) : BaseApiController
{
    /// <summary>List conversations for current logged-in user</summary>
    [HttpGet("conversations/me")]
    public async Task<IActionResult> GetMyConversationAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMyConversationsForClientQuery(
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("conversations/{convId:guid}/messages")]
    public async Task<IActionResult> ListMessagesInConversationAsync(
        Guid convId,
        [AsParameters] CursorCriteriaRequest parameters)
    {
        var userId = userAccessor.GetUserId();
        var query = new ListMessagesInConversationQuery(
            ActorId: userId,
            ConvId: convId,
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("conversations/{convId:guid}/messages/{anchorId:guid}/window")]
    public async Task<IActionResult> ListWindowMessagesInConversationAsync(
        Guid convId,
        Guid anchorId,
        [FromQuery] int before = 30,
        [FromQuery] int after = 30,
        [FromQuery] string anchor = "self")
    {
        var query = new ListWindowMessagesInConversationQuery(
            ConvId: convId,
            AnchorId: anchorId,
            Before: before,
            After: after,
            Anchor: anchor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("messages")]
    public async Task<IActionResult> SendSupportMessagesAttachmentAsync([FromForm] MessageAttachmentRequest formData)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var command = formData.Adapt<SendSupportMessageCommand>() with
            {
                SenderActorId = userId,
                SenderUserId = userId,
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
                errorDetail: new {ClientLocalId = formData.ClientLocalId});
        }
    }
}
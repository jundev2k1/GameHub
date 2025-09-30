using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.Chat.Commands.HideToggleConversation;
using game_x.application.Features.Chat.Commands.MarkLatestMessageAsRead;
using game_x.application.Features.Chat.Commands.SendMessage;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using game_x.application.Features.Chat.Queries.GetAllUnReads;
using game_x.application.Features.Chat.Queries.ListHiddenConversationsForClient;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListMyConversationsForClient;
using game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

namespace game_x.api.Controllers.Client.Chat;

[Authorize(Roles = AppRoles.User)]
[Route("api/user")]
public class ConversationController(IUserAccessor userAccessor
) : BaseApiController
{
    [HttpPost("conversations/{convId:guid}/hide-toggle")]
    public async Task<IActionResult> HideToggleAsync(Guid convId, HideToggleConversationCommand cmd)
    {
        cmd.ConversationId = convId;
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>List conversations for current logged-in user</summary>
    [HttpGet("conversations/unread")]
    public async Task<IActionResult> GetAllUnReadsAsync()
    {
        var result = await Mediator.Send(new GetAllUnReadsQuery());
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("conversations/{convId:guid}/messages/read-latest")]
    public async Task<IActionResult> MarkAsReadLatestAsync(Guid convId)
    {
        var result = await Mediator.Send(new MarkLatestMessageAsReadCommand(convId));
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>List conversations for current logged-in user</summary>
    [HttpGet("conversations/me")]
    public async Task<IActionResult> GetMyConversationsAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMyConversationsForClientQuery(
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("conversations/hidden")]
    public async Task<IActionResult> GetHiddenConversationsAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListHiddenConversationsForClientQuery(
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
        [AsParameters] AnchorWindowRequest parameters)
    {
        var query = new ListWindowMessagesInConversationQuery(
            ConvId: convId,
            AnchorId: anchorId,
            Before: parameters.Before,
            After: parameters.After,
            Anchor: parameters.Anchor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [Obsolete("Use SendMessagesAttachmentAsync instead")]
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
                errorDetail: new {formData.ClientLocalId});
        }
    }
    
    [HttpPost("conversations/{convId:guid}/messages")]
    public async Task<IActionResult> SendMessagesAttachmentAsync(Guid convId, [FromForm] MessageAttachmentRequest formData)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var command = formData.Adapt<SendMessageCommand>() with
            {
                ConversationId = convId,
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
                errorDetail: new {formData.ClientLocalId});
        }
    }
}
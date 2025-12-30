using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Features.Chat.Commands.ClaimConversationById;
using game_x.application.Features.Chat.Commands.DeleteMessage;
using game_x.application.Features.Chat.Commands.SendMessage;
using game_x.application.Features.Chat.Commands.SendMessageToCustomer;
using game_x.application.Features.Chat.Queries.ListMessageInPublicConversation;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListSupportConversations;
using game_x.application.Features.Chat.Queries.ListUnassignedQueue;
using game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

namespace game_x.api.Controllers.BackOffice.Chat;

[Route("api/back-office/conversations")]
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
public class ConversationController : BaseApiController
{
    /// <summary>
    /// List unassigned support conversations. Cursor-based pagination.
    /// </summary>
    [HttpGet("queue")]
    public async Task<IActionResult> GetConversationQueueAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListUnassignedQueueQuery(
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("{convId}/claim")]
    public async Task<IActionResult> ClaimConversationAsync(Guid convId, [FromBody] ClaimConversationByIdCommand command, CancellationToken ct)
    {
        command.ConversationId = convId;
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary> List conversations for current logged-in user </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetSupportConversationAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListSupportConversationsQuery(
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>List messages of the public conversation.</summary>
    [HttpGet("public-messages")]
    public async Task<IActionResult> GetPublicMessagesAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMessageInPublicConversationQuery(
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpDelete("{convId:guid}/messages/{messageId:guid}")]
    public async Task<IActionResult> ListMessagesInConversationAsync(
        Guid convId, Guid messageId)
    {
        var result = await Mediator.Send(new DeleteMessageCommand(convId, messageId));
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{convId:guid}/messages")]
    public async Task<IActionResult> ListMessagesInConversationAsync(
        Guid convId,
        [AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMessagesInConversationQuery(
            ConvId: convId,
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("{convId:guid}/messages/{anchorId:guid}/window")]
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
    
    [HttpPost("{convId:guid}/messages")]
    public async Task<IActionResult> SendSupportMessagesAttachmentAsync(
        Guid convId,
        [FromForm] MessageAttachmentRequest formData)
    {
        try
        {
            var command = formData.Adapt<SendMessageToCustomerCommand>() with
            {
                ConversationId = convId,
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
                errorDetail: new
                {
                    formData.ClientLocalId,
                    ConversationId = convId,
                });
        }
    }
    
    [HttpPost("{convId:guid}/messages-v2")]
    public async Task<IActionResult> SendMessageAttachmentsAsync(Guid convId, [FromForm] MessageAttachmentRequest formData)
    {
        try
        {
            var command = formData.Adapt<SendMessageCommand>() with
            {
                ConversationId = convId,
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
                errorDetail: new
                {
                    formData.ClientLocalId,
                    ConversationId = convId,
                });
        }
    }
}
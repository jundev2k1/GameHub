using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Common.Filters;
using game_x.application.Exceptions;
using game_x.application.Features.Games.Admin.Commands.UpdateGame;
using game_x.application.Features.Games.Admin.Commands.UpdateGameTranslations;
using game_x.application.Features.Games.Admin.Commands.UploadGameMediaSource;
using game_x.application.Features.Games.Admin.Commands.UpsertGameMedias;
using game_x.application.Features.Games.Admin.Queries.GetGameDetail;
using game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;
using game_x.application.Features.Games.Admin.Queries.GetGameTransactionDetail;
using game_x.application.Features.Games.Admin.Queries.GetGameTransactions;
using game_x.share.Helper;

namespace game_x.api.Controllers.BackOffice.Game;

[Route("/api/back-office/games")]
public sealed class GameController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGameListAsync([AsParameters] GetGamesByCriteriaRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.Types.IsNotNullOrEmpty())
            paramExtends.Add("types", parameters.Types!);
        if (parameters.Categories.IsNotNullOrEmpty())
            paramExtends.Add("categories", parameters.Categories!);
        if (parameters.Tags.IsNotNullOrEmpty())
            paramExtends.Add("tags", parameters.Tags!);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGamesByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameDetailAsync(Guid id)
    {
        var query = new GetGameDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{gameId}")]
    public async Task<IActionResult> UpdateGameAsync(Guid gameId, [FromForm] UpdateGameRequest request)
    {
        GameCategoryItem[]? categories = null;
        if (request.Categories != null
            && request.Categories.IsNotNullOrEmpty()
            && !JsonHelper.TryParseJson(request.Categories, out categories))
            throw new BadRequestException("Categories is in the wrong format.");
        GameTypeItem[]? types = null;
        if (request.Types != null
            && request.Types.IsNotNullOrEmpty()
            && !JsonHelper.TryParseJson(request.Types, out types))
            throw new BadRequestException("Types is in the wrong format.");
        GameTagItem[]? tags = null;
        if (request.Tags != null
            && request.Tags.IsNotNullOrEmpty()
            && !JsonHelper.TryParseJson(request.Tags, out tags))
            throw new BadRequestException("Tags is in the wrong format.");

        var command = new UpdateGameCommand(
            gameId,
            request.Name.Trim(),
            request.Description?.Trim() ?? string.Empty,
            request.Note?.Trim() ?? string.Empty,
            request.Priority,
            request.IsActive,
            request.Thumbnail != null ? FileUpload.FromFormFile(request.Thumbnail) : null,
            categories,
            types,
            tags);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("{gameId}/translations")]
    public async Task<IActionResult> UpsertGameTranslationsAsync(
        [FromRoute] Guid gameId,
        [FromBody] UpdateGameTranslationsCommand command)
    {
        await Mediator.Send(command with { GameId = gameId });
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("{gameId}/medias")]
    public async Task<IActionResult> UpsertGameMediasAsync(
        [FromRoute] Guid gameId,
        [FromBody] UpsertGameMediasCommand command)
    {
        await Mediator.Send(command with { Id = gameId });
        return ApiResponseFactory.NoContent();
    }

    [HttpPut("games/{gameId:guid}/medias/{mediaId:guid}/video/source")]
    public async Task<IActionResult> UploadMediaSourceAsync(
        [FromRoute] Guid gameId,
        [FromRoute] Guid mediaId,
        CancellationToken ct = default)
    {
        var fileName = Request.Headers["X-File-Name"].ToStringOrEmpty();
        if (fileName.IsNullOrWhiteSpace())
            throw new BadRequestException("Missing X-File-Name header.");

        var contentLength = Request.ContentLength ?? 0;
        var contentType = Request.ContentType ?? "application/octet-stream";
        var upload = FileUpload.FromStream(
            Request.Body,
            fileName,
            contentType,
            contentLength);
        var command = new UploadGameMediaSourceCommand(gameId, mediaId, upload);
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] GetGameTransactionsRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.Statuses.IsNotNullOrEmpty())
            paramExtends.Add("statuses", parameters.Statuses);
        if (parameters.Platforms.IsNotNullOrEmpty())
            paramExtends.Add("platforms", parameters.Platforms);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGameTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetGameTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}

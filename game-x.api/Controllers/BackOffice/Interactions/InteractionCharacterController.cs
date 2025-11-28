using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Common.Filters;
using game_x.application.Features.Interactions.Characters.Commands.CreateCharacter;
using game_x.application.Features.Interactions.Characters.Commands.CreatePoseCharacter;
using game_x.application.Features.Interactions.Characters.Commands.DeleteCharacter;
using game_x.application.Features.Interactions.Characters.Commands.DeletePose;
using game_x.application.Features.Interactions.Characters.Commands.UpdateCharacter;
using game_x.application.Features.Interactions.Characters.Commands.UpdateDefaultPoseCharacter;
using game_x.application.Features.Interactions.Characters.Commands.UpdatePoseCharacter;
using game_x.application.Features.Interactions.Characters.Queries.GetCharacterDetail;
using game_x.application.Features.Interactions.Characters.Queries.GetCharactersByCriteria;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.BackOffice.Interactions;

[Route("/api/back-office/interaction-characters")]
public sealed class InteractionCharacterController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetCharactersByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetCharactersByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCharacterDetailAsync(Guid id)
    {
        var query = new GetCharacterDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateCharacterAsync(CreateCharacterRequest request)
    {
        var command = new CreateCharacterCommand(
            request.Name.Trim(),
            request.Description.Trim(),
            request.Notes.Trim(),
            FileUpload.FromFormFile(request.DefaultPose));
        await Mediator.Send(command);

        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCharacterAsync(Guid id, UpdateCharacterCommand command)
    {
        await Mediator.Send(command with { Id = id });

        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/default-pose")]
    public async Task<IActionResult> UpdateDefaultPoseAsync(Guid id, [FromForm] UploadImageRequest request)
    {
        var command = new UpdateDefaultPoseCharacterCommand(id, FileUpload.FromFormFile(request.Image));
        await Mediator.Send(command);

        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacterAsync(Guid id)
    {
        var command = new DeleteCharacterCommand(id);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("{id:guid}/poses")]
    public async Task<IActionResult> CreatePoseAsync(Guid id, [FromForm] CreatePoseRequest request)
    {
        var command = new CreatePoseCharacterCommand(
            id,
            request.Name.Trim(),
            request.Description.Trim(),
            request.Notes.Trim(),
            FileUpload.FromFormFile(request.Pose));
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}/poses/{poseId:guid}")]
    public async Task<IActionResult> UpdatePoseAsync(Guid id, Guid poseId, [FromForm] UpdatePoseRequest request)
    {
        var command = new UpdatePoseCharacterCommand(
            id,
            poseId,
            request.Name?.Trim() ?? string.Empty,
            request.Description?.Trim() ?? string.Empty,
            request.Notes?.Trim() ?? string.Empty,
            request.Pose is not null ? FileUpload.FromFormFile(request.Pose) : null);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("poses/{poseId:guid}")]
    public async Task<IActionResult> DeletePoseAsync(Guid poseId)
    {
        var command = new DeletePoseCommand(poseId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}

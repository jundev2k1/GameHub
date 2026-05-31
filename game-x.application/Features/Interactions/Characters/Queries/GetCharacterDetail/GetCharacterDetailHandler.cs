using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Interactions.Characters.Dtos;

namespace game_x.application.Features.Interactions.Characters.Queries.GetCharacterDetail;

public sealed class GetCharacterDetailHandler(
    IInteractionCharacterRepo characterRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetCharacterDetailQuery, InteractionCharacterDetailDto>
{
    public async Task<InteractionCharacterDetailDto> Handle(GetCharacterDetailQuery request, CancellationToken ct = default)
    {
        var targetCharacter = await characterRepo.GetById(request.Id, ct);
        var dto = targetCharacter.Adapt<InteractionCharacterDetailDto>();
        var fileUrl = await fileManagerCache.GetFileUrl(dto.DefaultPoseId, ct);
        dto.FileUrl = fileUrl ?? string.Empty;

        foreach(var pose in dto.PoseSettings)
        {
            var poseUrl = await fileManagerCache.GetFileUrl(pose.PoseId, ct);
            pose.FileUrl = poseUrl ?? string.Empty;
        }

        return dto;
    }
}

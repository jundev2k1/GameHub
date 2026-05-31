using game_x.application.Features.Interactions.Characters.Dtos;

namespace game_x.application.Features.Interactions.Characters.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<InteractionCharacter, InteractionCharacterListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<InteractionCharacter, InteractionCharacterDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.FileName, src => src.DefaultPose.FileName)
            .Map(dest => dest.PoseSettings, src => src.Poses.Select(p => p.Adapt<InteractionCharacterPoseItemDto>()));

        cfg.NewConfig<InteractionCharacterPose, InteractionCharacterPoseItemDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.FileName, src => src.Pose.FileName);
    }
}

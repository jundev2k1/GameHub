using game_x.application.Features.Games.Dtos;
using game_x.application.Features.NavigationItems.Dtos;

namespace game_x.application.Features.NavigationItems.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<NavigationItemTranslation, NavigationItemTranslationInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.NavigationId, src => src.NavigationItemId)
            .Map(dest => dest.LanguageCode, src => src.LanguageCode.Value)
            .Map(dest => dest.Title, src => src.Title);

        cfg.NewConfig<NavigationItem, NavigationItemDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.TargetLocalId, src => src.TargetId)
            .Map(dest => dest.TargetId, src => (Guid?)null)
            .Map(dest => dest.NavigationTranslations, src => src.Translations.ToDictionary(t => t.LanguageCode.Value, t => t.Adapt<NavigationItemTranslationInfo>()));

        cfg.NewConfig<NavigationItem, NavigationItemDetailDto>()
            .Inherits<NavigationItem, NavigationItemDto>();
    }
}

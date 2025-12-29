using game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        RegisterGameMappings(cfg);
        RegisterGamePlatformMappings(cfg);
        RegisterGameCategoryMappings(cfg);
        RegisterGameTypeMappings(cfg);
        RegisterGameTagMappings(cfg);
        RegisterGameTransactionMappings(cfg);
        RegisterGameRecommendMappings(cfg);
    }

    private static void RegisterGameMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<MediaFile, ThumbnailInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.BucketName, src => src.BucketName.Value)
            .Map(dest => dest.ObjectName, src => src.ObjectName.Value);

        cfg.NewConfig<Game, GameInfoDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Thumbnail, src => src.Thumbnail.Adapt<ThumbnailInfo>())
            .Map(
                dest => dest.Categories,
                src => src.GameCategoryMappings
                    .Select(x => x.Adapt<GameCategoryInfo>())
                    .OrderBy(g => g.IsPrimary)
                    .ThenByDescending(g => g.Priority))
            .Map(
                dest => dest.GameTypes,
                src => src.GameTypeMappings
                    .Select(x => x.Adapt<GameTypeInfo>())
                    .OrderBy(g => g.IsPrimary)
                    .ThenByDescending(g => g.Priority))
            .Map(
                dest => dest.GameTags,
                src => src.GameTagMappings
                    .Select(x => x.Adapt<GameTagInfo>())
                    .OrderBy(g => g.IsPrimary)
                    .ThenByDescending(g => g.Priority))
            .Map(dest => dest.PlatformId, src => src.Platform.PublicId)
            .Map(dest => dest.PlatformName, src => src.Platform.Name);

        cfg.NewConfig<Game, GetGamesByCriteriaListItem>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.PlatformId, src => src.Platform.PublicId)
            .Map(dest => dest.PlatformName, src => src.Platform.Name);
    }

    private static void RegisterGamePlatformMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GamePlatform, GamePlatformDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId);
    }

    private static void RegisterGameCategoryMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameCategory, GameCategoryDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<GameCategoryMapping, GameCategoryInfo>()
            .Map(dest => dest.LocalId, src => src.Category.Id)
            .Map(dest => dest.Id, src => src.Category.PublicId)
            .Map(dest => dest.Name, src => src.Category.Name);
    }

    private static void RegisterGameTypeMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameType, GameTypeDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<GameTypeMapping, GameTypeInfo>()
            .Map(dest => dest.LocalId, src => src.Type.Id)
            .Map(dest => dest.Id, src => src.Type.PublicId)
            .Map(dest => dest.Name, src => src.Type.Name);
    }

    private static void RegisterGameTagMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameTag, GameTagDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Icon, src => src.Icon.Value)
            .Map(dest => dest.Color, src => src.Color.Value);

        cfg.NewConfig<GameTagMapping, GameTagInfo>()
            .Map(dest => dest.LocalId, src => src.Tag.Id)
            .Map(dest => dest.Id, src => src.Tag.PublicId)
            .Map(dest => dest.Name, src => src.Tag.Name)
            .Map(dest => dest.Icon, src => src.Tag.Icon.Value)
            .Map(dest => dest.Color, src => src.Tag.Color.Value);
    }

    private static void RegisterGameTransactionMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Transaction, TransactionExternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.GamePlatformId, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.PublicId : Guid.Empty)
            .Map(dest => dest.GamePlatformName, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.Name : null)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.BalanceAfter, src => src.BalanceAfter);

        cfg.NewConfig<Transaction, ListTransactionExternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.GamePlatformId, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.PublicId : Guid.Empty)
            .Map(dest => dest.GamePlatformName, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.Name : String.Empty)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network);
        
        cfg.NewConfig<Transaction, TransactionExternalDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.BalanceAfter, src => src.BalanceAfter)
            .Map(dest => dest.GamePlatformId, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.PublicId : Guid.Empty)
            .Map(dest => dest.GamePlatformName, src =>  src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.Name : null);
    }

    private static void RegisterGameRecommendMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameRecommend, GameRecommendDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Items, src => src.Items.Select(i => i.Adapt<GameRecommendItemDto>()));

        cfg.NewConfig<GameRecommendItem, GameRecommendItemDto>()
            .Map(dest => dest.LocalGameId, src => src.GameId)
            .Map(dest => dest.GameId, src => src.Game.PublicId)
            .Map(dest => dest.GameName, src => src.Game.Name)
            .Map(dest => dest.LocalPlatformId, src => src.Game.PlatformId)
            .Map(dest => dest.PlatformId, src => src.Game.Platform.PublicId)
            .Map(dest => dest.PlatformName, src => src.Game.Platform.Name);
    }
}

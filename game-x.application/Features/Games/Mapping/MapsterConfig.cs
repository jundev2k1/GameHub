using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        RegisterGameMappings(cfg);
        RegisterGameTransactionMappings(cfg);
    }

    private static void RegisterGameMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Game, GameInfoDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(
                dest => dest.Categories,
                src => src.GameCategoryMappings
                    .Select(x => x.Adapt<GameCategoryInfo>())
                    .OrderBy(g => g.IsPrimary)
                    .ThenBy(g => g.Priority))
            .Map(
                dest => dest.GameTypes,
                src => src.GameTypeMappings
                    .Select(x => x.Adapt<GameTypeInfo>())
                    .OrderBy(g => g.IsPrimary)
                    .ThenBy(g => g.Priority))
            .Map(dest => dest.PlatformId, src => src.Platform.PublicId)
            .Map(dest => dest.PlatformName, src => src.Platform.Name);

        cfg.NewConfig<GameCategoryMapping, GameCategoryInfo>()
            .Map(dest => dest.LocalId, src => src.Category.Id)
            .Map(dest => dest.Id, src => src.Category.PublicId)
            .Map(dest => dest.Name, src => src.Category.Name)
            .Map(dest => dest.Priority, src => src.Category.Priority);

        cfg.NewConfig<GameTypeMapping, GameTypeInfo>()
            .Map(dest => dest.LocalId, src => src.Type.Id)
            .Map(dest => dest.Id, src => src.Type.PublicId)
            .Map(dest => dest.Name, src => src.Type.Name)
            .Map(dest => dest.Priority, src => src.Type.Priority);

        cfg.NewConfig<GamePlatform, GamePlatformDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<GameCategory, GameCategoryDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<GameType, GameTypeDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId);
    }

    private static void RegisterGameTransactionMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<TransactionExternal, TransactionExternalDto>()
            .Map(dest => dest.Id, src => src.Transaction.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.Transaction.CryptoToken!.PublicId)
            .Map(dest => dest.GamePlatformId, src => src.GamePlatform!.PublicId)
            .Map(dest => dest.GamePlatformName, src => src.GamePlatform!.Name)
            .Map(dest => dest.Symbol, src => src.Transaction.CryptoToken!.Symbol)
            .Map(dest => dest.Network, src => src.Transaction.CryptoToken!.Network)
            .Map(dest => dest.BalanceAfter, src => src.Transaction.BalanceAfter);

        cfg.NewConfig<TransactionExternal, TransactionExternalDetailDto>()
            .Map(dest => dest.Id, src => src.Transaction.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.Transaction.CryptoToken!.PublicId)
            .Map(dest => dest.Symbol, src => src.Transaction.CryptoToken!.Symbol)
            .Map(dest => dest.Network, src => src.Transaction.CryptoToken!.Network)
            .Map(dest => dest.BalanceAfter, src => src.Transaction.BalanceAfter)
            .Map(dest => dest.GamePlatformId, src => src.GamePlatform!.PublicId)
            .Map(dest => dest.GamePlatformName, src => src.GamePlatform!.Name);
    }
}

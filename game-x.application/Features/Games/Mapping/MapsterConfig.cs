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
        RegisterPlatformTranslationMappings(cfg);
        RegisterGameTranslationMappings(cfg);
        RegisterCategoryTranslationMappings(cfg);
        RegisterTypeTranslationMappings(cfg);
        RegisterTagTranslationMappings(cfg);
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
            .Map(dest => dest.PlatformName, src => src.Platform.Name)
            .Map(
                dest => dest.GameTranslations,
                src => src.Translations.ToDictionary(t => t.LanguageCode.Value, t => t.Adapt<GameTranslationInfo>()));

        cfg.NewConfig<Game, GetGamesByCriteriaListItem>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.PlatformId, src => src.Platform.PublicId)
            .Map(dest => dest.PlatformName, src => src.Platform.Name)
            .AfterMapping((src, dest) =>
            {
                var lang = MapContext.Current?.Parameters["language"]?.ToString();
                if (lang is null || src.Translations.Count == 0) return;

                var targetLang = src.Translations.FirstOrDefault(t => t.LanguageCode.Value == lang);
                if (targetLang is null) return;

                dest.Name = targetLang.Name;
                dest.Description = targetLang.Description;
                dest.Note = targetLang.Note;
            });
    }

    private static void RegisterGamePlatformMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GamePlatform, GamePlatformDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.PlatformTranslations, src => src.Translations.ToDictionary(t => t.LanguageCode.Value, t => t.Adapt<GamePlatformTranslationInfo>()));
    }

    private static void RegisterGameCategoryMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameCategory, GameCategoryDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CategoryTranslations, src => src.Translations.ToDictionary(t => t.LanguageCode.Value, t => t.Adapt<GameCategoryTranslationInfo>()));

        cfg.NewConfig<GameCategoryMapping, GameCategoryInfo>()
            .Map(dest => dest.LocalId, src => src.Category.Id)
            .Map(dest => dest.Id, src => src.Category.PublicId)
            .Map(dest => dest.Name, src => src.Category.Name)
            .Map(dest => dest.Description, src => src.Category.Description);
    }

    private static void RegisterGameTypeMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameType, GameTypeDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.TypeTranslations, src => src.Translations.ToDictionary(t => t.LanguageCode.Value, t => t.Adapt<GameTypeTranslationInfo>()));

        cfg.NewConfig<GameTypeMapping, GameTypeInfo>()
            .Map(dest => dest.LocalId, src => src.Type.Id)
            .Map(dest => dest.Id, src => src.Type.PublicId)
            .Map(dest => dest.Name, src => src.Type.Name)
            .Map(dest => dest.Description, src => src.Type.Description);
    }

    private static void RegisterGameTagMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameTag, GameTagDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Icon, src => src.Icon.Value)
            .Map(dest => dest.Color, src => src.Color.Value)
            .Map(dest => dest.TagTranslations, src => src.Translations.ToDictionary(t => t.LanguageCode.Value, t => t.Adapt<GameTagTranslationInfo>()));

        cfg.NewConfig<GameTagMapping, GameTagInfo>()
            .Map(dest => dest.LocalId, src => src.Tag.Id)
            .Map(dest => dest.Id, src => src.Tag.PublicId)
            .Map(dest => dest.Name, src => src.Tag.Name)
            .Map(dest => dest.Icon, src => src.Tag.Icon.Value)
            .Map(dest => dest.Color, src => src.Tag.Color.Value)
            .Map(dest => dest.Description, src => src.Tag.Description);
    }

    private static void RegisterGameTransactionMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Transaction, TransactionExternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Nickname, src => src.User.Nickname)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.GamePlatformId, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.PublicId : Guid.Empty)
            .Map(dest => dest.GamePlatformName, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.Name : null)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.BalanceAfter, src => src.BalanceAfter);

        cfg.NewConfig<Transaction, ListTransactionExternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Nickname, src => src.User.Nickname)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.GamePlatformId, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.PublicId : Guid.Empty)
            .Map(dest => dest.GamePlatformName, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.Name : string.Empty)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network);
        
        cfg.NewConfig<Transaction, TransactionExternalDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Nickname, src => src.User.Nickname)
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
            .Map(dest => dest.IsGameActive, src => src.Game.IsActive);
    }

    private static void RegisterPlatformTranslationMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GamePlatformTranslation, GamePlatformTranslationInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.PlatformId, src => src.PlatformId)
            .Map(dest => dest.LanguageCode, src => src.LanguageCode.Value);
    }

    private static void RegisterGameTranslationMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameTranslation, GameTranslationInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.GameId, src => src.GameId)
            .Map(dest => dest.LanguageCode, src => src.LanguageCode.Value);
    }

    private static void RegisterCategoryTranslationMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameCategoryTranslation, GameCategoryTranslationInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.LanguageCode, src => src.LanguageCode.Value);
    }

    private static void RegisterTypeTranslationMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameTypeTranslation, GameTypeTranslationInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.TypeId, src => src.TypeId)
            .Map(dest => dest.LanguageCode, src => src.LanguageCode.Value);
    }

    private static void RegisterTagTranslationMappings(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameTagTranslation, GameTagTranslationInfo>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.TagId, src => src.TagId)
            .Map(dest => dest.LanguageCode, src => src.LanguageCode.Value);
    }
}

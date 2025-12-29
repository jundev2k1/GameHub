using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Extensions;
using MediatR;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGame;

public sealed class UpdateGameHandler(
    IGameProviderCacheService gameProviderCache,
    IUnitOfWork unitOfWork,
    IGameRepo gameRepo,
    IMediaFileRepo mediaFileRepo,
    IFileStorageService fileStorage) : ICommandHandler<UpdateGameCommand>
{
    public async Task<Unit> Handle(UpdateGameCommand request, CancellationToken ct = default)
    {
        // Check if input is valid
        ValidateInputs(request);

        // Update game
        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Insert category/type/tag if there are any updates
            await InsertMappingsAsync(request, ct);

            // Execute update game
            await gameRepo.UpdateGameAsync(request.Id, async game =>
            {
                game.UpdateGame(
                    name: request.Name.Trim(),
                    desc: request.Description.Trim(),
                    note: request.Note.Trim(),
                    priority: request.Priority,
                    isActive: request.IsActive);

                // Handle upload if new thumbnail is provided
                if (request.Thumbnail != null)
                    await HandleUploadNewThumbnail(game, request.Thumbnail, ct);
            }, ct);
        }, ct);

        // Refresh cache
        await gameProviderCache.RefreshGameList();

        return Unit.Value;
    }

    private void ValidateInputs(UpdateGameCommand input)
    {
        if (input.Categories != null)
        {
            var invalidCateIds = input.Categories
                .Where(item => !gameProviderCache.CategoryList
                    .Any(cate => cate.Id == item.Id))
                .Select(item => item.Id.ToString())
                .ToArray();
            if (invalidCateIds.Length > 0)
                throw new BadRequestException($"Invalid category ids: {invalidCateIds.JoinToString(", ")}");
        }

        if (input.Types != null)
        {
            var invalidTypeIds = input.Types
                .Where(item => !gameProviderCache.GameTypeList
                    .Any(cate => cate.Id == item.Id))
                .Select(item => item.Id.ToString())
                .ToArray();
            if (invalidTypeIds.Length > 0)
                throw new BadRequestException($"Invalid type ids: {invalidTypeIds.JoinToString(", ")}");
        }

        if (input.Tags != null)
        {
            var invalidTagIds = input.Tags
                .Where(item => !gameProviderCache.GameTagList
                    .Any(cate => cate.Id == item.Id))
                .Select(item => item.Id.ToString())
                .ToArray();
            if (invalidTagIds.Length > 0)
                throw new BadRequestException($"Invalid tag ids: {invalidTagIds.JoinToString(", ")}");
        }
    }

    private async Task InsertMappingsAsync(UpdateGameCommand request, CancellationToken ct)
    {
        var targetGame = await gameRepo.GetAsync(request.Id, ct);

        ICollection<GameCategoryMapping>? categories = request.Categories != null
            ? [.. GetCategoryMappings(request, targetGame)]
            : null;
        if (categories != null)
        {
            await gameRepo.DeleteAllCategoryMappingsAsync(request.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await gameRepo.AddRangeGameCategoriesAsync(categories, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        ICollection<GameTypeMapping>? types = request.Types != null
            ? [.. GetTypeMappings(request, targetGame)]
            : null;
        if (types != null)
        {
            await gameRepo.DeleteAllTypeMappingsAsync(request.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await gameRepo.AddRangeGameTypesAsync(types, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        ICollection<GameTagMapping>? tags = request.Tags != null
            ? [.. GetTagMappings(request, targetGame)]
            : null;
        if (tags != null)
        {
            await gameRepo.DeleteAllTagMappingsAsync(request.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await gameRepo.AddRangeGameTagsAsync(tags, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

    private async Task HandleUploadNewThumbnail(Game game, FileUpload fileUpload, CancellationToken ct)
    {
        var bucketName = BucketName.Game;
        var newFileName = DateTime.Now.ToString("yyyyMMdd_hhMMss") + fileUpload.Extension;
        var objectName = ObjectName.GameResource(game.PublicId, newFileName);

        // Remove old thumbnail
        if (game.ThumbnailId.HasValue)
        {
            await mediaFileRepo.RemoveAsync(game.ThumbnailId.Value, ct);
        }

        // Create image information
        var thumbnail = MediaFile.Create(
            bucketName,
            objectName,
            fileUpload.FileName,
            MimeType.Of(fileUpload.ContentType),
            Convert.ToInt32(fileUpload.Length));
        game.UpdateThumbnail(thumbnail);

        // Upload to MinIO
        await fileStorage.UploadFileAsync(
            fileUpload.Content,
            thumbnail.BucketName,
            thumbnail.ObjectName,
            thumbnail.MimeType,
            ct);
    }

    private IEnumerable<GameCategoryMapping> GetCategoryMappings(
        UpdateGameCommand request,
        Game targetGame)
    {
        foreach (var category in request.Categories!)
        {
            var targetCategory = gameProviderCache.CategoryList
                .FirstOrDefault(cate => cate.Id == category.Id)
                ?? throw new BadRequestException($"Category ({category.Id}) was not found.");

            yield return GameCategoryMapping.Create(
                gameId: targetGame.Id,
                categoryId: targetCategory.LocalId,
                isPrimary: category.IsPrimary,
                priority: category.Priority);
        }
    }

    private IEnumerable<GameTypeMapping> GetTypeMappings(
        UpdateGameCommand request,
        Game targetGame)
    {
        foreach (var type in request.Types!)
        {
            var targetType = gameProviderCache.GameTypeList
                .FirstOrDefault(t => t.Id == type.Id)
                ?? throw new BadRequestException($"Game Type ({type.Id}) was not found.");

            yield return GameTypeMapping.Create(
                gameId: targetGame.Id,
                typeId: targetType.LocalId,
                isPrimary: type.IsPrimary,
                priority: type.Priority);
        }
    }

    private IEnumerable<GameTagMapping> GetTagMappings(
        UpdateGameCommand request,
        Game targetGame)
    {
        foreach (var tag in request.Tags!)
        {
            var targetTag = gameProviderCache.GameTagList
                .FirstOrDefault(t => t.Id == tag.Id)
                ?? throw new BadRequestException($"Tag ({tag.Id}) was not found.");

            yield return GameTagMapping.Create(
                gameId: targetGame.Id,
                tagId: targetTag.LocalId,
                isPrimary: tag.IsPrimary,
                priority: tag.Priority);
        }
    }
}

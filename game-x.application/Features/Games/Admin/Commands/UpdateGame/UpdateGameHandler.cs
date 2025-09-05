using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Extensions;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGame;

public sealed class UpdateGameHandler(
    IGameProviderCacheService gameProviderCache,
    IUnitOfWork unitOfWork,
    IGameRepo gameRepo) : ICommandHandler<UpdateGameCommand>
{
    public async Task<Unit> Handle(UpdateGameCommand request, CancellationToken ct = default)
    {
        // Check if input is valid
        ValidateInputs(request);

        // Update game
        await gameRepo.UpdateGameAsync(request.Id, async game =>
        {
            game.UpdateGame(
                name: request.Name.Trim(),
                desc: request.Description.Trim(),
                note: request.Note.Trim(),
                priority: request.Priority,
                isActive: request.IsActive,
                categories: [.. GetCategoryMappings(request, game)],
                types: [.. GetTypeMappings(request, game)],
                tags: [.. GetTagMappings(request, game)]);

            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache
        await gameProviderCache.RefreshGameList();

        return Unit.Value;
    }

    private void ValidateInputs(UpdateGameCommand input)
    {
        var invalidCateIds = input.Categories
            .Where(item => !gameProviderCache.CategoryList
                .Any(cate => cate.Id == item.Id))
            .Select(item => item.Id.ToString())
            .ToArray();
        if (invalidCateIds.Length > 0)
            throw new BadRequestException($"Invalid category ids: {invalidCateIds.JoinToString(", ")}");

        var invalidTypeIds = input.Types
            .Where(item => !gameProviderCache.GameTypeList
                .Any(cate => cate.Id == item.Id))
            .Select(item => item.Id.ToString())
            .ToArray();
        if (invalidTypeIds.Length > 0)
            throw new BadRequestException($"Invalid type ids: {invalidTypeIds.JoinToString(", ")}");

        var invalidTagIds = input.Tags
            .Where(item => !gameProviderCache.GameTagList
                .Any(cate => cate.Id == item.Id))
            .Select(item => item.Id.ToString())
            .ToArray();
        if (invalidCateIds.Length > 0)
            throw new BadRequestException($"Invalid tag ids: {invalidTagIds.JoinToString(", ")}");
    }

    private IEnumerable<GameCategoryMapping> GetCategoryMappings(
        UpdateGameCommand request,
        Game targetGame)
    {
        foreach (var category in request.Categories)
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
        foreach (var type in request.Types)
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
        foreach (var tag in request.Tags)
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

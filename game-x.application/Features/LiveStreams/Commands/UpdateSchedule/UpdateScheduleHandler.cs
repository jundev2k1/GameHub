using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.UpdateSchedule;

public sealed class UpdateScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : ICommandHandler<UpdateScheduleCommand>
{
    public async Task<Unit> Handle(UpdateScheduleCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await liveStreamRepo.UpdateAsync(request.Id, async liveStream =>
            {
                ValidateInput(request, liveStream);

                var categoryMappings = await CreateCategoryItems(request.Categories, ct);
                liveStream.Update(
                    request.Title,
                    request.StartTime,
                    request.EndTime,
                    request.Description,
                    request.Notes,
                    categoryMappings);
            }, ct);
        }, ct);
        return Unit.Value;
    }

    /// <summary>
    /// Validate input data
    /// </summary>
    /// <param name="request">Request input</param>
    /// <param name="targetSetting">Target setting from db</param>
    private static void ValidateInput(UpdateScheduleCommand request, LivestreamSchedule targetSetting)
    {
        if (targetSetting.Status != LiveStreamStatus.Scheduled)
            throw new BadRequestException("Only scheduled livestream can be updated.");

        if (!request.StartTime.HasValue && !request.EndTime.HasValue)
            return;

        if (request.StartTime.HasValue && request.EndTime is null)
        {
            var isValidTime = request.StartTime >= DateTime.UtcNow && request.StartTime < targetSetting.EndTime;
            if (!isValidTime) throw new BadRequestException("Start time invalid.");
        }

        if (request.StartTime is null && request.EndTime.HasValue)
        {
            var isValidTime = request.EndTime >= DateTime.UtcNow && request.EndTime > targetSetting.StartTime;
            if (!isValidTime) throw new BadRequestException("End time invalid.");
        }
    }

    /// <summary>
    /// Create category mapping items for update
    /// </summary>
    /// <param name="categories">A collection of categories from request input</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A collection of categories for update or null</returns>
    private async Task<List<LiveStreamCategoryMapping>?> CreateCategoryItems(
        ScheduleCategoryDto[]? categories,
        CancellationToken ct)
    {
        // Don't check category inputs if null
        if (categories is null) return null;

        // Get and validate categories from db
        var actualCategories = await liveStreamCategoryRepo.GetByIdsAsync(
            [.. categories.Select(c => c.Id)], ct);
        if (categories.Length != actualCategories.Length)
            throw new BadRequestException("One or more categories do not exists or are invalid");

        var categoryList = new List<LiveStreamCategoryMapping>();
        foreach (var category in categories)
        {
            var targetCategory = actualCategories.FirstOrDefault(c => c.PublicId == category.Id)
                ?? throw new BadRequestException($"CategoryId ({category.Id}) was not found.");

            var categoryMapping = LiveStreamCategoryMapping.Create(
                default,
                targetCategory.Id,
                category.IsPrimary,
                category.Priority);
            categoryList.Add(categoryMapping);
        }
        return categoryList;
    }
}

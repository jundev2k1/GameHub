using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.CreateSchedule;

public sealed class CreateScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : ICommandHandler<CreateScheduleCommand>
{
    public async Task<Unit> Handle(CreateScheduleCommand request, CancellationToken ct = default)
    {
        var categoryIds = request.Categories
            .Select(c => c.Id)
            .ToArray();
        var categories = await liveStreamCategoryRepo.GetByIdsAsync(categoryIds, ct);
        if (categoryIds.Length != categories.Length)
            throw new BadRequestException("One or more categories do not exists or are invalid");

        var categoryMappings = CreateCategoryItems(request.Categories, categories).ToList();
        var liveStreamEntity = LivestreamSchedule.Create(
            request.Title,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Note,
            categoryMappings);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await liveStreamRepo.CreateAsync(liveStreamEntity, ct);
        }, ct);

        return Unit.Value;
    }

    private static IEnumerable<LiveStreamCategoryMapping> CreateCategoryItems(
        ScheduleCategoryDto[] categories,
        LiveStreamCategory[] actualCategories)
    {
        foreach (var category in categories)
        {
            var targetCategory = actualCategories.FirstOrDefault(c => c.PublicId == category.Id)
                ?? throw new BadRequestException($"CategoryId ({category.Id}) was not found.");

            var categoryMapping = LiveStreamCategoryMapping.Create(
                default,
                targetCategory.Id,
                category.IsPrimary,
                category.Priority);
            yield return categoryMapping;
        }
    }
}

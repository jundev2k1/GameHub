using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Categories.Dtos;
using game_x.application.Features.LiveStreams.Schedules.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.share.Extensions;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.CreateSchedule;

public sealed class CreateScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamCategoryRepo liveStreamCategoryRepo,
    IUserRepo userRepo,
    IFileManagerCacheService fileManagerCache,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<CreateScheduleCommand>
{
    public async Task<Unit> Handle(CreateScheduleCommand request, CancellationToken ct = default)
    {
        // Check overlapping times before assigning talent
        if (request.TalentId.IsNotNullOrEmpty())
            await CheckOverlapTime(request.TalentId!, request.StartTime, request.EndTime);

        // check if exist categories
        var categoryIds = request.Categories
            .Select(c => c.Id)
            .ToArray();
        var categories = await liveStreamCategoryRepo.GetByIdsAsync(categoryIds, ct);
        if (categoryIds.Length != categories.Length)
            throw new BadRequestException("One or more categories do not exists or are invalid");

        // Create livestream entity
        var categoryMappings = CreateCategoryItems(request.Categories, categories).ToList();
        var liveStreamEntity = LivestreamSchedule.Create(
            request.Title,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Note,
            categoryMappings);

        // Assign talent if provided
        if (request.TalentId.IsNotNullOrEmpty())
        {
            var talent = await userRepo.GetUserByIdAsync(request.TalentId!, ct);
            if (!talent.IsTalent)
                throw new BadRequestException($"This user is not a Talent.");

            liveStreamEntity.AssignStream(talent.Id);
        }

        // Create livestream within a transaction
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await liveStreamRepo.CreateAsync(liveStreamEntity, ct);
        }, ct);

        var schedule = await liveStreamRepo.GetByStreamKeyAsync(liveStreamEntity.StreamKey, ct);
        await InitStreamInfo(schedule, ct);

        return Unit.Value;
    }

    private static IEnumerable<LiveStreamCategoryMapping> CreateCategoryItems(
        LiveStreamScheduleCategoryDto[] categories,
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

    private async Task CheckOverlapTime(string talentId, DateTime startTime, DateTime endTime)
    {
        var streams = await liveStreamRepo.GetsByTalentIdAsync(talentId);
        foreach (var stream in streams)
        {
            var isOverlaps = stream.Status is not (LiveStreamStatus.Cancelled or LiveStreamStatus.Ended)
                && startTime <= stream.EndTime && endTime >= stream.StartTime;
            if (isOverlaps)
            {
                throw new BadRequestException(
                    MessageCode.System.ValidateFailed,
                    new
                    {
                        IsOverlapTime = true,
                        stream.Title,
                        stream.StartTime,
                        stream.EndTime
                    });
            }
        }
    }

    private async Task InitStreamInfo(LivestreamSchedule streamSetting, CancellationToken ct)
    {
        var streamInfo = new LiveStreamStatusDto
        {
            LocalId = streamSetting.Id,
            Id = streamSetting.PublicId,
            Title = streamSetting.Title,
            Description = streamSetting.Description ?? string.Empty,
            ThumbnailId = streamSetting.ThumbnailId,
            StreamKey = streamSetting.StreamKey,
            LiveAt = streamSetting.StartAt ?? DateTime.UtcNow,
            OfflineAt = streamSetting.EndAt,
            StartTime = streamSetting.StartTime,
            EndTime = streamSetting.EndTime,
            ClientId = null,
            AssignedTo = streamSetting.AssignedTo?.Adapt<UserSummaryInfo>(),
            Categories = [.. streamSetting.CategoryMappings.Select(cm => cm.Adapt<LiveStreamCategorySummaryDto>())]
        };
        if (streamSetting.AssignedTo != null && streamSetting.AssignedTo.Avatar != null)
        {
            var avatar = await fileManagerCache.GetFileInfo(streamSetting.AssignedTo.Avatar, ct);
            streamInfo.AssignedTo!.Avatar = avatar?.Url;
        }

        liveStreamManager.InitLiveStream(streamInfo);
    }
}

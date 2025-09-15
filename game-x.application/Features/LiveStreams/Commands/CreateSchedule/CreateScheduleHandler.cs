using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.share.Extensions;

namespace game_x.application.Features.LiveStreams.Commands.CreateSchedule;

public sealed class CreateScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamCategoryRepo liveStreamCategoryRepo,
    IUserRepo userRepo) : ICommandHandler<CreateScheduleCommand>
{
    public async Task<Unit> Handle(CreateScheduleCommand request, CancellationToken ct = default)
    {
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
            if ((talent.UserKyc == null) || talent.UserKyc.Status != KycStatus.Approved)
                throw new BadRequestException("User must be kyc verified.");
            if (talent.UserBankAccounts.Count == 0
                || !talent.UserBankAccounts.Any(ba => ba.Status == UserBankAccountStatus.Approved))
                throw new BadRequestException("Must have at least 1 verified bank account");
            liveStreamEntity.AssignStream(talent.Id);
        }

        // Create livestream within a transaction
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

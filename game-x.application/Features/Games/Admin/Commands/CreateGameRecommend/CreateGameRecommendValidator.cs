namespace game_x.application.Features.Games.Admin.Commands.CreateGameRecommend;

public sealed class CreateGameRecommendValidator : AbstractValidator<CreateGameRecommendCommand>
{
    public CreateGameRecommendValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(CreateGameRecommendCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameRecommendCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage($"{nameof(CreateGameRecommendCommand.Status)} is invalid.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage($"{nameof(CreateGameRecommendCommand.StartDate)} must be earlier than or equal to end date.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage($"{nameof(CreateGameRecommendCommand.EndDate)} must be later than or equal to start date.");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items is required.")
            .Must(items => items.Select(i => i.GameId).Distinct().Count() == items.Length)
            .WithMessage("Duplicate GameId is not allowed in Items.")
            .Must(items => items.All(i => i.Priority > 0))
            .WithMessage("Game item priority must be greater than or equal 0.");
    }
}

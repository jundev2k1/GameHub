namespace game_x.application.Features.Games.Admin.Commands.UpdateGameRecommend;

public sealed class UpdateGameRecommendValidator : AbstractValidator<UpdateGameRecommendCommand>
{
    public UpdateGameRecommendValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateGameRecommendCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameRecommendCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage($"{nameof(UpdateGameRecommendCommand.Status)} is invalid.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage($"{nameof(UpdateGameRecommendCommand.StartDate)} must be earlier than or equal to end date.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage($"{nameof(UpdateGameRecommendCommand.EndDate)} must be later than or equal to start date.");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items is required.")
            .Must(items => items.Select(i => i.GameId).Distinct().Count() == items.Length)
            .WithMessage("Duplicate GameId is not allowed in Items.")
            .Must(items => items.All(i => i.Priority > 0))
            .WithMessage("Game item priority must be greater than or equal 0.");
    }
}

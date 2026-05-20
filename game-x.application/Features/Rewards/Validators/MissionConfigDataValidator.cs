using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Validators;

public sealed class MissionConfigDataValidator : AbstractValidator<MissionConfigData>
{
    public MissionConfigDataValidator()
    {
        RuleFor(x => x.RequiredProgress)
            .GreaterThan(0)
            .WithMessage("RequiredProgress must be greater than 0.");

        RuleFor(x => x.ProgressMode)
            .IsInEnum()
            .WithMessage("ProgressMode is invalid.");
        
        RuleFor(x => x.MinimumValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimumValue.HasValue);

        RuleFor(x => x.MaximumValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaximumValue.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinimumValue.HasValue ||
                !x.MaximumValue.HasValue ||
                x.MaximumValue >= x.MinimumValue)
            .WithMessage("MaximumValue must be greater than or equal to MinimumValue.");

        RuleFor(x => x.ProgressCooldownSeconds)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ProgressCooldownSeconds cannot be negative.");

        RuleFor(x => x.RequiredIntervalDays)
            .GreaterThan(0)
            .WithMessage("RequiredIntervalDays must be greater than 0.");

        RuleFor(x => x.RequiredShareConversions)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RewardExpireMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("RewardExpireMinutes cannot be negative.");
   
        RuleFor(x => x.MaxCompletionPerUser)
            .GreaterThanOrEqualTo(0)
            .WithMessage("MaxCompletionPerUser cannot be negative.");

        RuleFor(x => x.Metadata)
            .Must(metadata =>
                metadata == null ||
                metadata.Keys.All(k => !string.IsNullOrWhiteSpace(k)))
            .WithMessage("Metadata keys cannot be empty.");

        RuleFor(x => x.Metadata)
            .Must(metadata =>
                metadata == null ||
                metadata.Values.All(v => !string.IsNullOrWhiteSpace(v)))
            .WithMessage("Metadata values cannot be empty.");
    }
}
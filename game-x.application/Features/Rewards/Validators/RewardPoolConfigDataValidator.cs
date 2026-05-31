using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Validators;

public sealed class RewardPoolConfigDataValidator : AbstractValidator<RewardPoolConfigData>
{
    public RewardPoolConfigDataValidator()
    {
        #region Basic

        RuleFor(x => x.Theme)
            .MaximumLength(100)
            .WithMessage("The maximum length of theme is 100 characters.");

        RuleFor(x => x.AnimationType)
            .IsInEnum()
            .WithMessage("The animation type is invalid.");

        RuleFor(x => x.SpinDurationMs)
            .InclusiveBetween(500, 30000)
            .WithMessage("Spin duration must be between 500ms and 30000ms.");

        #endregion

        #region Spin Rules

        RuleFor(x => x.RequiredItemType)
            .IsInEnum()
            .WithMessage("The required item type is invalid.");

        RuleFor(x => x.RequiredItemAmount)
            .GreaterThan(0)
            .WithMessage("The required item amount must be greater than zero.");

        RuleFor(x => x.DailySpinLimitPerUser)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The required item amount must be greater than zero.");

        RuleFor(x => x.RequiredCatalogItemId)
            .NotNull()
            .When(x =>
                x.RequiredItemType == CatalogItemCategory.Ticket)
            .WithMessage("RequiredCatalogItemId is required for item-based spin requirements.");

        #endregion

        #region Reward Flow

        RuleFor(x => x.RewardExpireMinutes)
            .GreaterThanOrEqualTo(0);

        #endregion

        #region UI / Client

        // bools don't need validation

        #endregion

        #region Anti Abuse

        RuleFor(x => x.SpinCooldownSeconds)
            .GreaterThanOrEqualTo(0);

        #endregion

        #region Metadata

        RuleFor(x => x.Metadata)
            .Must(metadata => metadata == null || metadata.Count <= 100)
            .WithMessage("Metadata cannot contain more than 100 entries.");

        RuleForEach(x => x.Metadata)
            .ChildRules(meta =>
            {
                meta.RuleFor(x => x.Key)
                    .NotEmpty()
                    .MaximumLength(100);

                meta.RuleFor(x => x.Value)
                    .MaximumLength(1000);
            });

        #endregion

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.EnableJackpotEffect && x.AnimationType != RewardPoolType.Roulette)
                    return false;

                return true;
            })
            .WithMessage("Jackpot effect is only supported for Roulette animation.");
    }
}
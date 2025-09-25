using game_x.application.Features.LiveStreams.Streaming.Enum;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.PerformAction;

public sealed class PerformActionValidator : AbstractValidator<PerformActionCommand>
{
    private readonly static PerformActionEnum[] BanActions = [PerformActionEnum.Kick, PerformActionEnum.Mute, PerformActionEnum.BlockDonation];

    public PerformActionValidator()
    {
        RuleFor(x => x.Action)
            .IsInEnum();

        When(x => BanActions.Contains(x.Action), () =>
        {
            RuleFor(x => x.BlockTime)
                .NotNull().WithMessage("Block time is required when blocking a viewer")
                .GreaterThan(0).WithMessage("Block time must be greater than 0");

            RuleFor(x => x.Reason)
                .NotNull().WithMessage("Reason is required when blocking a viewer")
                .IsInEnum().WithMessage("Reason is required when blocking a viewer");
        });
    }
}

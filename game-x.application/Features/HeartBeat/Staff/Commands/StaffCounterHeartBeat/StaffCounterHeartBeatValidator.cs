namespace game_x.application.Features.HeartBeat.Staff.Commands.StaffCounterHeartBeat;

public sealed class StaffCounterHeartBeatValidator : AbstractValidator<StaffCounterHeartBeatCommand>
{
    public StaffCounterHeartBeatValidator()
    {
        RuleFor(hb => hb.SessionKey)
            .NotEmpty().WithMessage($"{nameof(StaffCounterHeartBeatCommand.SessionKey)} must be not empty.");
    }
}

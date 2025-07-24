namespace game_x.application.Features.CounterManagement.Staff.ResolveQrCodeCounter;

public sealed class ResolveQrCodeCounterValidator : AbstractValidator<ResolveQrCodeCounterCommand>
{
    public ResolveQrCodeCounterValidator()
    {
        RuleFor(x => x.QrCode)
            .NotEmpty().WithMessage($"{nameof(ResolveQrCodeCounterCommand.QrCode)} is required.");
    }
}

namespace game_x.application.Features.CounterManagement.Admin.Commands.GenerateQrCodeCounter;

public sealed class GenerateQrCodeCounterValidator : AbstractValidator<GenerateQrCodeCounterCommand>
{
    public GenerateQrCodeCounterValidator()
    {
        RuleFor(x => x.CounterId)
            .NotEmpty().WithMessage($"{nameof(GenerateQrCodeCounterCommand.CounterId)} is not empty.");
    }
}

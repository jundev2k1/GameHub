namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed class SendSupportMessageValidator : AbstractValidator<SendSupportMessageCommand>
{
    public SendSupportMessageValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage($"{nameof(SendSupportMessageCommand.Text)} is required.");
    }
}
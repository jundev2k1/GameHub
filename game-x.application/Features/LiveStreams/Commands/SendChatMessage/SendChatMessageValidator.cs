namespace game_x.application.Features.LiveStreams.Commands.SendChatMessage;

public sealed class SendChatMessageValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageValidator()
    {
        RuleFor(x => x.StreamKey)
            .NotEmpty().WithMessage("Stream key cannot empty.");

        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("Message Id cannot empty.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(255).WithMessage("Message cannot exceed 255 characters.");
    }
}

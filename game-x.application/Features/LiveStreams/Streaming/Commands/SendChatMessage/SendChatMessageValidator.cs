namespace game_x.application.Features.LiveStreams.Streaming.Commands.SendChatMessage;

public sealed class SendChatMessageValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageValidator()
    {
        RuleFor(x => x.StreamKey)
            .NotEmpty().WithMessage("Stream key cannot empty.");

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id cannot empty.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("Id must be an uuid string.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(255).WithMessage("Message cannot exceed 255 characters.");
    }
}

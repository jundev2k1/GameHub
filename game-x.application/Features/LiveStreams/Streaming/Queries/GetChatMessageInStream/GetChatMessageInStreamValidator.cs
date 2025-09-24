namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetChatMessageInStream;

public sealed class GetChatMessageInStreamValidator : AbstractValidator<GetChatMessageInStreamQuery>
{
    public GetChatMessageInStreamValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
    }
}

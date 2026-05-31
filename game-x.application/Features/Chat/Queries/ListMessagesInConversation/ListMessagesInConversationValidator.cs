namespace game_x.application.Features.Chat.Queries.ListMessagesInConversation;

public sealed class ListMessagesInConversationValidator : AbstractValidator<ListMessagesInConversationQuery>
{
    public ListMessagesInConversationValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal 1")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal 100");
    }
}
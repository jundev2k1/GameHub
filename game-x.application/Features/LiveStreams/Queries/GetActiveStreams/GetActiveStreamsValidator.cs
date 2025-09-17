using game_x.application.Common.Filters;

namespace game_x.application.Features.LiveStreams.Queries.GetActiveStreams;

public sealed class GetActiveStreamsValidator : AbstractValidator<GetActiveStreamsQuery>
{
    public GetActiveStreamsValidator()
    {
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Start time must be greater than current time")
            .When(x => x.StartTime.HasValue);

        RuleFor(x => x.StartTime)
            .GreaterThan(x => x.EndTime).WithMessage("Start time must be less than end time")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("End time must be greater than current time")
            .When(x => x.EndTime.HasValue);

        RuleFor(x => x.EndTime)
            .LessThan(x => x.StartTime).WithMessage("End time must be greater than start time")
            .When(x => x.EndTime.HasValue && x.StartTime.HasValue);

        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("Page index must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal 1")
            .LessThanOrEqualTo(1000).WithMessage("Page size must be less than or equal 1000");
    }
}

namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;

public sealed class SearchRecordValidator : AbstractValidator<SearchRecordCommand>
{
    public SearchRecordValidator()
    {
        RuleFor(x => x.DateStart)
            .Must(BeUtc)
            .WithMessage("DateStart must be UTC DateTime");

        RuleFor(x => x.DateEnd)
            .Must(BeUtc)
            .WithMessage("DateEnd must be UTC DateTime")
            .GreaterThanOrEqualTo(x => x.DateStart)
            .WithMessage("DateEnd must be greater than or equal DateStart");
        
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("Page index must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal 1")
            .LessThanOrEqualTo(1000).WithMessage("Page size must be less than or equal 1000");
    }
    
    private static bool BeUtc(DateTime dateTime) => dateTime.Kind == DateTimeKind.Utc;
}
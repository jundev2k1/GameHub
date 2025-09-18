using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.UpdateSchedule;

public sealed class UpdateScheduleValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.")
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("Note cannot exceed 4000 characters.")
            .When(x => x.Notes is not null);

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Start time must be before the end time.")
            .GreaterThan(DateTime.Now).WithMessage("Start time must be in the future.")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after the start time.")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue); ;

        RuleForEach(x => x.Categories)
            .Must(HaveValidCategory).WithMessage("Each category must have a valid Id, IsPrimary and Priority.")
            .When(x => x.Categories is not null);
    }

    private bool HaveValidCategory(ScheduleCategoryDto category)
    {
        return category != null
            && category.Id != Guid.Empty
            && category.Priority >= 0;
    }
}

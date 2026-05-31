namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItem;

public sealed class UpdateNavigationItemValidator : AbstractValidator<UpdateNavigationItemCommand>
{
    public UpdateNavigationItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage($"{nameof(UpdateNavigationItemCommand.Title)} is required.")
            .MaximumLength(255).WithMessage($"{nameof(UpdateNavigationItemCommand.Title)} must be less than 255 characters.");

        RuleFor(x => x.Slug)
            .NotNull()
            .When(x => x.TargetType != NavigationTargetType.ExternalLink)
            .WithMessage($"{nameof(UpdateNavigationItemCommand.Slug)} is required when link is internal.")
            .MaximumLength(255)
            .WithMessage($"{nameof(UpdateNavigationItemCommand.Slug)} must be less than 255 characters.");

        RuleFor(x => x.TargetType)
            .IsInEnum().WithMessage($"{nameof(UpdateNavigationItemCommand.TargetType)} is invalid.");

        RuleFor(x => x.TargetId)
            .NotEmpty()
            .When(x => x.TargetType == NavigationTargetType.Category)
            .WithMessage($"{nameof(UpdateNavigationItemCommand.TargetId)} is required for Category target types.");

        RuleFor(x => x.CustomUrl)
            .NotNull()
            .When(x => x.TargetType == NavigationTargetType.ExternalLink)
            .WithMessage($"{nameof(UpdateNavigationItemCommand.CustomUrl)} is required for External Link target type.")
            .Must(Link => Uri.TryCreate(Link, UriKind.Absolute, out _))
            .When(x => x.TargetType == NavigationTargetType.ExternalLink && !string.IsNullOrEmpty(x.CustomUrl))
            .WithMessage($"{nameof(UpdateNavigationItemCommand.CustomUrl)} must be a valid absolute URL.")
            .MaximumLength(2000)
            .WithMessage($"{nameof(UpdateNavigationItemCommand.CustomUrl)} must be less than 2000 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(UpdateNavigationItemCommand.Priority)} must be greater than or equal 0.");
    }
}

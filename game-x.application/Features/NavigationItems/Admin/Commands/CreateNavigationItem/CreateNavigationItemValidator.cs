namespace game_x.application.Features.NavigationItems.Admin.Commands.CreateNavigationItem;

public sealed class CreateNavigationItemValidator : AbstractValidator<CreateNavigationItemCommand>
{
    public CreateNavigationItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage($"{nameof(CreateNavigationItemCommand.Title)} is required.")
            .MaximumLength(255).WithMessage($"{nameof(CreateNavigationItemCommand.Title)} must be less than 255 characters.");

        RuleFor(x => x.Slug)
            .NotNull()
            .When(x => x.TargetType != NavigationTargetType.ExternalLink)
            .WithMessage($"{nameof(CreateNavigationItemCommand.Slug)} is required when link is internal.")
            .MaximumLength(255)
            .WithMessage($"{nameof(CreateNavigationItemCommand.Slug)} must be less than 255 characters.");

        RuleFor(x => x.TargetType)
            .IsInEnum().WithMessage($"{nameof(CreateNavigationItemCommand.TargetType)} is invalid.");

        RuleFor(x => x.TargetId)
            .NotEmpty()
            .When(x => x.TargetType == NavigationTargetType.Category)
            .WithMessage($"{nameof(CreateNavigationItemCommand.TargetId)} is required for Category target types.");

        RuleFor(x => x.CustomUrl)
            .NotNull()
            .When(x => x.TargetType == NavigationTargetType.ExternalLink)
            .WithMessage($"{nameof(CreateNavigationItemCommand.CustomUrl)} is required for External Link target type.")
            .Must(Link => Uri.TryCreate(Link, UriKind.Absolute, out _))
            .When(x => x.TargetType == NavigationTargetType.ExternalLink && !string.IsNullOrEmpty(x.CustomUrl))
            .WithMessage($"{nameof(CreateNavigationItemCommand.CustomUrl)} must be a valid absolute URL.")
            .MaximumLength(2000)
            .WithMessage($"{nameof(CreateNavigationItemCommand.CustomUrl)} must be less than 2000 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(CreateNavigationItemCommand.Priority)} must be greater than or equal 0.");
    }
}

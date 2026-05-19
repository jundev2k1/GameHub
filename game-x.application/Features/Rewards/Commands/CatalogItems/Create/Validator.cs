namespace game_x.application.Features.Rewards.Commands.CatalogItems.Create;

public sealed class CreateCatalogItemValidator : AbstractValidator<CreateCatalogItemCommand>
{
    public CreateCatalogItemValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
        
        RuleFor(x => x.Description)
            .MaximumLength(4096)
            .When(x => x.Description != null)
            .WithMessage("Maximum length exceeded.");
        
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Category is invalid.");
        
        RuleFor(x => x.IconType)
            .IsInEnum()
            .WithMessage("IconType is invalid.");
        
        RuleFor(x => x.IconValue)
            .MaximumLength(2048)
            .When(x => x.IconValue != null)
            .WithMessage("Maximum length exceeded.");
        
        RuleFor(x => x.MonetaryValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MonetaryValue.HasValue);
    }
}
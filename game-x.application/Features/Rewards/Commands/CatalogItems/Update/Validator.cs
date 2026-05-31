namespace game_x.application.Features.Rewards.Commands.CatalogItems.Update;

public sealed class UpdateCatalogItemValidator : AbstractValidator<UpdateCatalogItemCommand>
{
    public UpdateCatalogItemValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .MaximumLength(128)
            .WithMessage("Code must not exceed 128 characters.")
            .When(x => x.Code != null);
        
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Category is invalid.")
            .When(x => x.Category != null);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(256)
            .WithMessage("Name must not exceed 256 characters.")
            .When(x => x.Name != null);
        
        RuleFor(x => x.Description)
            .MaximumLength(4096)
            .When(x => x.Description != null)
            .WithMessage("Description must not exceed 4096 characters.");
        
        RuleFor(x => x.MonetaryValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MonetaryValue != null)
            .WithMessage("Amount must be greater than or equal to 0.");
        
        RuleFor(x => x.IconType)
            .IsInEnum()
            .WithMessage("IconType is invalid.")
            .When(x => x.IconType != null);
        
        RuleFor(x => x.IconValue)
            .NotEmpty()
            .WithMessage("IconValue is required.")
            .MaximumLength(2048)
            .WithMessage("IconValue must not exceed 256 characters.")
            .When(x => x.IconValue != null);
        
        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SortOrder != null)
            .WithMessage("SortOrder must be greater than or equal to 0.");
    }
}
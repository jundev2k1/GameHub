namespace game_x.application.Features.Rewards.Commands.AddUserCatalogItem;

public sealed class AddUserCatalogItemValidator : AbstractValidator<AddUserCatalogItemCommand>
{
    public AddUserCatalogItemValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}
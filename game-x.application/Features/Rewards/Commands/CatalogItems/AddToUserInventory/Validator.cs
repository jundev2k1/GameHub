namespace game_x.application.Features.Rewards.Commands.CatalogItems.AddToUserInventory;

public sealed class AddItemToUserInventoryValidator : AbstractValidator<AddItemToUserInventoryCommand>
{
    public AddItemToUserInventoryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}
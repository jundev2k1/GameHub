namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Update;

public sealed class UpdateRewardDefinitionValidator : AbstractValidator<UpdateRewardDefinitionCommand>
{
    public UpdateRewardDefinitionValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .MaximumLength(128)
            .WithMessage("Code must not exceed 128 characters.")
            .When(x => x.Code != null);
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type is invalid.")
            .When(x => x.Type != null);
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(2048)
            .WithMessage("Title must not exceed 2048 characters.")
            .When(x => x.Title != null);
        
        RuleFor(x => x.Description)
            .MaximumLength(4096)
            .When(x => x.Description != null)
            .WithMessage("Description must not exceed 4096 characters.");
        
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Amount != null)
            .WithMessage("Amount must be greater than or equal to 0.");
        
        RuleFor(x => x.Metadata)
            .MaximumLength(4096)
            .When(x => x.Metadata != null)
            .WithMessage("Metadata must not exceed 4096 characters.");
    }
}
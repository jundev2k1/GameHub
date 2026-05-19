namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Create;

public sealed class CreateRewardDefinitionValidator : AbstractValidator<CreateRewardDefinitionCommand>
{
    public CreateRewardDefinitionValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type is invalid.");
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(256)
            .WithMessage("Title must not exceed 128 characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(4096)
            .When(x => x.Description != null)
            .WithMessage("Description must not exceed 4096 characters.");
        
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.Amount.HasValue)
            .WithMessage("Amount must be greater than 0.");
        
        RuleFor(x => x.Metadata)
            .MaximumLength(4096)
            .When(x => x.Metadata != null)
            .WithMessage("Metadata must not exceed 4096 characters.");
    }
}
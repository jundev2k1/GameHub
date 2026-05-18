namespace game_x.application.Features.Rewards.Commands.UpdateRewardPool;

public sealed class UpdateRewardPoolValidator : AbstractValidator<UpdateRewardPoolCommand>
{
    public UpdateRewardPoolValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .MaximumLength(128)
            .WithMessage("Code must not exceed 128 characters.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type is invalid.");
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(2048)
            .WithMessage("Title must not exceed 2048 characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(4096)
            .When(x => x.Description != null)
            .WithMessage("Maximum length exceeded.");
        
        // RuleFor(x => x.Config)
        //     .NotNull()
        //     .SetValidator(new RewardPoolConfigDataValidator());
    }
}
using game_x.application.Features.Rewards.Validators;

namespace game_x.application.Features.Rewards.Commands.Missions.Create;

public sealed class CreateMissionValidator : AbstractValidator<CreateMissionCommand>
{
    public CreateMissionValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type is invalid.");
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");
        
        RuleFor(x => x.Description)
            .MaximumLength(4096)
            .When(x => x.Description != null)
            .WithMessage("Maximum length exceeded.");
        
        RuleFor(x => x.ResetType)
            .IsInEnum()
            .WithMessage("ResetType is invalid.");

        RuleFor(x => x.ConfigData)
            .NotNull()
            .SetValidator(new MissionConfigDataValidator());
    }
}
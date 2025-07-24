namespace game_x.application.Features.Accounts.User.Commands.UserSelfUpdate;

public sealed class UserSelfUpdateValidator : AbstractValidator<UserSelfUpdateCommand>
{
    public UserSelfUpdateValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("PhoneNumber must not exceed 20 characters.");
    }
}

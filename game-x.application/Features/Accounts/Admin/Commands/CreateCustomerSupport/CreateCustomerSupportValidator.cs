namespace game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

public sealed class CreateCustomerSupportValidator : AbstractValidator<CreateCustomerSupportCommand>
{
    public CreateCustomerSupportValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}

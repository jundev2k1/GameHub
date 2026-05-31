namespace game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

public sealed class CreateCustomerSupportValidator : AbstractValidator<CreateCustomerSupportCommand>
{
    public CreateCustomerSupportValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Nickname)
            .NotEmpty().WithMessage("Nickname must be not empty.")
            .MaximumLength(20).WithMessage("Nickname must be not greater than 20 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("Notes must be not greater than 40000 characters.");
    }
}

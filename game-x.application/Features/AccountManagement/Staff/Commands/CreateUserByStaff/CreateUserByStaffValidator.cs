namespace game_x.application.Features.AccountManagement.Staff.Commands.CreateUserByStaff;
public sealed class CreateUserByStaffValidator : AbstractValidator<CreateUserByStaffCommand>
{
    public CreateUserByStaffValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d+$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.CountryCode).NotEmpty().WithMessage("CountryCode is required.");

        RuleFor(x => x.Passport.PassportType).IsInEnum().WithMessage("Invalid passport type.");
        RuleFor(x => x.Passport.PassportNumber).NotEmpty().WithMessage("Passport number is required.");
        RuleFor(x => x.Passport.Country).NotEmpty().WithMessage("Country is required.");
        RuleFor(x => x.Passport.IssuedBy).NotEmpty().WithMessage("Issued by is required.");
        RuleFor(x => x.Passport.FirstName).NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.Passport.LastName).NotEmpty().WithMessage("Last name is required.");
        RuleFor(x => x.Passport.Gender).NotEmpty().WithMessage("Gender is required.");
        RuleFor(x => x.Passport.DateOfBirth).NotNull().WithMessage("Date of birth is required.");
        RuleFor(x => x.Passport.Nationality).NotEmpty().WithMessage("Nationality is required.");
        RuleFor(x => x.Passport.PlaceOfBirth).NotEmpty().WithMessage("Place of birth is required.");
        RuleFor(x => x.Passport.IssueDate).NotNull().WithMessage("Issue date is required.");
        RuleFor(x => x.Passport.ExpirationDate).NotNull().WithMessage("Expiration date is required.");
    }
}
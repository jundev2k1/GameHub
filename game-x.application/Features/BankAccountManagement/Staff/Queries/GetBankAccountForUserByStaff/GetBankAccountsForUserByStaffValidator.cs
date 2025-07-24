namespace game_x.application.Features.BankAccountManagement.Staff.Queries.GetBankAccountForUserByStaff;

public sealed class GetBankAccountsForUserByStaffValidator : AbstractValidator<GetBankAccountsForUserByStaffQuery>
{
    public GetBankAccountsForUserByStaffValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("UserId must be a valid GUID.");
    }
}


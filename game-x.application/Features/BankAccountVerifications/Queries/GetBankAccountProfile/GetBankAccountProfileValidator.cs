namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountProfile;

public sealed class GetBankAccountProfileValidator : AbstractValidator<GetBankAccountProfileQuery>
{
    public GetBankAccountProfileValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Currency code must not be empty.")
            .Must(CurrencyUnit.IsValid).WithMessage("Invalid currency code.");
    }
}

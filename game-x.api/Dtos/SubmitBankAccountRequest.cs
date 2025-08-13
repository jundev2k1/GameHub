namespace game_x.api.Dtos;

public record SubmitBankAccountRequest(
    string BankName,
    string BankCode,
    string AccountName,
    string AccountNumber,
    string CurrencyCode,
    IFormFile Image);

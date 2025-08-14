namespace game_x.api.Dtos;

public record ReSubmitBankAccountRequest(
    string? BankName,
    string? BankCode,
    string? AccountName,
    string? AccountNumber,
    IFormFile? Image);

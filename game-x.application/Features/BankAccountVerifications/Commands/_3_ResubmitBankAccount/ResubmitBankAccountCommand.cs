using game_x.application.Common.Files;

namespace game_x.application.Features.BankAccountVerifications.Commands._3_ResubmitBankAccount;

public record ResubmitBankAccountCommand(
    string? FullName,
    DateTime? DateOfBirth,
    string? Address,
    string? IdNumber,
    KycType Type,
    FileUpload? FrontImage,
    FileUpload? BackImage) : ICommand;

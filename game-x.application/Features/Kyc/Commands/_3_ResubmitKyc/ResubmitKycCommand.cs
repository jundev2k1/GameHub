using game_x.application.Common.Files;

namespace game_x.application.Features.Kyc.Commands._3_ResubmitKyc;

public record ResubmitKycCommand(
    string? FullName,
    DateTime? DateOfBirth,
    string? Address,
    string? IdNumber,
    KycType Type,
    FileUpload? FrontImage,
    FileUpload? BackImage) : ICommand;

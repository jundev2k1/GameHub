using game_x.application.Common.Files;

namespace game_x.application.Features.Kyc.Commands._1_SubmitKyc;

public record SubmitKycCommand(
    string FullName,
    DateTime DateOfBirth,
    string Address,
    string IdNumber,
    FileUpload FrontImage,
    FileUpload BackImage) : ICommand;

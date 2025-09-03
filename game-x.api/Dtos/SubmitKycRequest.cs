namespace game_x.api.Dtos;

public record SubmitKycRequest(
    string FullName,
    DateTime DateOfBirth,
    string Address,
    string IdNumber,
    KycType Type,
    IFormFile FrontImage,
    IFormFile BackImage);

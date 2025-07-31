namespace game_x.api.Dtos;

public record SubmitKycRequest(
    string FullName,
    DateTime DateOfBirth,
    string Address,
    string IdNumber,
    IFormFile FrontPhoto,
    IFormFile BackPhoto);

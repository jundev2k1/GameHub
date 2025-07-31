namespace game_x.api.Dtos;

public record ReSubmitKycRequest(
    string? FullName,
    DateTime? DateOfBirth,
    string? Address,
    string? IdNumber,
    IFormFile? FrontPhoto,
    IFormFile? BackPhoto);

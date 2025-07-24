using Microsoft.AspNetCore.Http;

namespace game_x.application.Features.AccountManagement.Dtos;

public record UploadPassportImageRequest(IFormFile File);

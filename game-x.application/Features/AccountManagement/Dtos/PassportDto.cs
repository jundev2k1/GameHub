namespace game_x.application.Features.AccountManagement.Dtos;

public class PassportDto
{
    public PassportType PassportType { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? IssuedBy { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? PlaceOfBirth { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Remarks { get; set; }
    public string? ImageUrl { get; set; }
}

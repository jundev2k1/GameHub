using FluentValidation.TestHelper;
using game_x.application.Features.Auth.Client.Commands.UserLogin;

namespace Test.UnitTests.Application.Auth.Client.Commands.UserLogin;

public sealed class UserLoginValidatorTests
{
    private readonly UserLoginValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("user@")]
    public void Validate_InvalidEmail_HasValidationError(string? badEmail)
    {
        var command = new UserLoginCommand(badEmail!, "validPassword");
        
        var result = _validator.TestValidate(command);
        
        result
            .ShouldHaveValidationErrorFor(c => c.Email)
            .WithErrorMessage("Email format is invalid");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidPassword_HasValidationError(string? password)
    {
        var command = new UserLoginCommand("email@example.com", password!);

        var result = _validator.TestValidate(command);

        result
            .ShouldHaveValidationErrorFor(c => c.Password)
            .WithErrorMessage("Password is required.");
    }
    
    [Fact]
    public void Should_NotHaveValidationError_When_InputIsValid()
    {
        var command = new UserLoginCommand("email@example.com", "StrongPass123!");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
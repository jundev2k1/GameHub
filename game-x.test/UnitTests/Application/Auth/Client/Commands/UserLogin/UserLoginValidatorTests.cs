using FluentValidation.TestHelper;
using game_x.application.Features.Auth.Client.Commands.UserLogin;

namespace Test.UnitTests.Application.Auth.Client.Commands.UserLogin;

public sealed class UserLoginValidatorTests
{
    private readonly UserLoginValidator _validator = new();

    [Theory]
    [InlineData("", "validPassword", nameof(UserLoginCommand.Email))]
    [InlineData(null, "validPassword", nameof(UserLoginCommand.Email))]
    [InlineData("email@example.com", "", nameof(UserLoginCommand.Password))]
    [InlineData("email@example.com", null, nameof(UserLoginCommand.Password))]
    [InlineData("", "", nameof(UserLoginCommand.Email))]
    public void Should_HaveValidationError_When_FieldIsEmpty(
        string? email,
        string? password,
        string expectedInvalidProperty)
    {
        var command = new UserLoginCommand(email!, password!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(expectedInvalidProperty);
    }

    [Fact]
    public void Should_NotHaveValidationError_When_InputIsValid()
    {
        var command = new UserLoginCommand("email@example.com", "StrongPass123!");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
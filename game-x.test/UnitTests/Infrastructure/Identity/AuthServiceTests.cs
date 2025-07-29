using game_x.domain.Entities;
using Moq;
using Microsoft.AspNetCore.Identity;
using game_x.application.Exceptions;
using game_x.domain.Constants;
using FluentAssertions;
using game_x.infrastructure.Identity;
using Microsoft.AspNetCore.Http;

namespace Test.UnitTests.Infrastructure.Identity;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockUserManager();
        _signInManagerMock = MockSignInManager(_userManagerMock.Object);
        _authService = new AuthService(
            userManager: _userManagerMock.Object, 
            signInManager: _signInManagerMock.Object);
    }

    // --- helpers ---
    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }
    
    private static Mock<SignInManager<User>> MockSignInManager(UserManager<User> userManager)
    {
        return new Mock<SignInManager<User>>(userManager,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            null!, null!, null!, null!);
    }
    
    // --- Test cases ---
    [Fact]
    public async Task TryLoginAsync_ValidCredentials_ReturnsUser()
    {
        var user = new User { UserName = "test" };
        _userManagerMock.Setup(u => u.FindByNameAsync("test"))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(s =>
                s.PasswordSignInAsync("test", "123", false, true))
            .ReturnsAsync(SignInResult.Success);

        var result = await _authService.TryLoginAsync("test", "123");

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task TryLoginAsync_UnknownUser_ThrowsBadRequest()
    {
        _userManagerMock.Setup(u => u.FindByNameAsync("invalid"))
            .ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _authService.TryLoginAsync("invalid", "123"));

        ex.ErrorCode.Should().Be(MessageCode.User.UserInvalidCredentials);
    }

    [Fact]
    public async Task TryLoginAsync_AccountLockedOut_ThrowsBadRequest()
    {
        var user = new User { UserName = "locked", LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30) };

        _userManagerMock.Setup(u => u.FindByNameAsync("locked"))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(s =>
                s.PasswordSignInAsync("locked", "123", false, true))
            .ReturnsAsync(SignInResult.LockedOut);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _authService.TryLoginAsync("locked", "123"));

        ex.ErrorCode.Should().Be(MessageCode.User.UserLocked);
        ((DateTimeOffset)(ex.ErrorDetail?.GetType().GetProperty("lockoutEnd")?.GetValue(ex.ErrorDetail)!))
            .Should().Be(user.LockoutEnd!.Value);
    }

    [Theory]
    [InlineData("notAllowed", MessageCode.User.UserNotAllowed, nameof(SignInResult.NotAllowed))]
    [InlineData("2fa", MessageCode.User.UserRequiresTwoFactor, nameof(SignInResult.TwoFactorRequired))]
    [InlineData("fail", MessageCode.User.UserInvalidCredentials, nameof(SignInResult.Failed))]
    public async Task TryLoginAsync_SignInFails_ThrowsExpected(string username, Enum expectedCode, string resultType)
    {
        var user = new User { UserName = username };
        _userManagerMock.Setup(u => u.FindByNameAsync(username))
            .ReturnsAsync(user);
    
        var result = resultType switch
        {
            nameof(SignInResult.NotAllowed) => SignInResult.NotAllowed,
            nameof(SignInResult.TwoFactorRequired) => SignInResult.TwoFactorRequired,
            nameof(SignInResult.Failed) => SignInResult.Failed,
            _ => throw new ArgumentException()
        };
    
        _signInManagerMock.Setup(s =>
                s.PasswordSignInAsync(username, "123", false, true))
            .ReturnsAsync(result);
    
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _authService.TryLoginAsync(username, "123"));

        ex.ErrorCode.Should().Be(expectedCode);
    }
}

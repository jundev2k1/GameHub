using Moq;
using game_x.application.Features.Auth.Client.Commands.UserLogin;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Exceptions;
using FluentAssertions;
using game_x.application.Common.Abstractions.Events;
using game_x.domain.Constants;
using game_x.domain.Entities;
using game_x.domain.Enum;
using game_x.domain.ValueObjects;
using game_x.application.Contract.Infrastructure.Caching;

namespace Test.UnitTests.Application.Auth.Client.Commands.UserLogin;

public sealed class UserLoginHandlerTests
{
    private readonly Mock<IUserAccessor> _userAccessorMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IRefreshTokenManagerCacheService> _refreshTokenManagerMock = new();
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly Mock<IApplicationEventDispatcher> _eventDispatcherMock = new();
    private readonly UserLoginHandler _handler;

    public UserLoginHandlerTests()
    {
        _handler = new UserLoginHandler(
            userAccessor: _userAccessorMock.Object,
            jwtTokenGenerator: _jwtTokenGeneratorMock.Object,
            tokenService: _tokenServiceMock.Object,
            refreshTokenManager: _refreshTokenManagerMock.Object,
            authService: _authServiceMock.Object,
            eventDispatcher: _eventDispatcherMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenLoginIsSuccessful()
    {
        // Arrange
        var command = new UserLoginCommand("test@example.com", "password");
        var user = new User
        {
            Id = Guid.NewGuid().ToString(), 
            UserName = command.Email, 
            Email = command.Email, 
            Nickname = "user", 
            Status = UserStatus.Active,
            EmailConfirmed = true
        };
        var roles = AppRole.Of(AppRoles.User);
        var tokenInfo = new JwtTokenDto { Token = "token123", ExpiresAt = DateTime.UtcNow.AddMinutes(30) };

        _authServiceMock.Setup(x => x.TryLoginAsync("test@example.com", "password", true))
            .ReturnsAsync(user);

        _authServiceMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user))
            .ReturnsAsync(tokenInfo);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Email.Should().Be(user.Email);
        result.UserId.Should().Be(user.Id);
        result.Token.Should().Be(tokenInfo.Token);
    }

    [Theory]
    // User was invalid
    [InlineData(UserStatus.Inactive, MessageCode.User.UserInvalid)]
    // User was deleted
    [InlineData(UserStatus.Active, MessageCode.User.UserDisabled, AppRoles.User, true, true)]
    // Account is not a user account
    [InlineData(UserStatus.Active, null, AppRoles.Admin, false)]
    // User's email is not confirmed
    [InlineData(UserStatus.Active, MessageCode.User.UserNotConfirmed, AppRoles.User, false)]
    public async Task Handle_ShouldThrowExpectedException_BasedOnInvalidStates(
        UserStatus status,
        object? errorCode,
        string role = AppRoles.User,
        bool emailConfirmed = true,
        bool isDeleted = false)
    {
        // Arrange
        var command = new UserLoginCommand("email@example.com", "StrongPass123!");
        var user = new User
        {
            Id = Guid.NewGuid().ToString(), 
            UserName = command.Email, 
            Email = command.Email, 
            Nickname = "user", 
            Status = status,
            EmailConfirmed = emailConfirmed,
            IsDeleted = isDeleted
        };
        var roles = AppRole.Of(role);
        var tokenInfo = new JwtTokenDto { Token = "token123", ExpiresAt = DateTime.UtcNow.AddMinutes(30) };
        
        _authServiceMock.Setup(x => x.TryLoginAsync(command.Email, command.Password, true))
            .ReturnsAsync(user);
        
        _authServiceMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);
        
        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user))
            .ReturnsAsync(tokenInfo);
    
        // Act & Assert
        if (status == UserStatus.Inactive)
        {
            var ex = await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command));
            ex.ErrorCode.Should().Be(errorCode);
        }
        else if (!roles.IsUser)
        {
            await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command));
        }
        else if (!emailConfirmed)
        {
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command));
            ex.ErrorCode.Should().Be(errorCode);
        }
    }
}

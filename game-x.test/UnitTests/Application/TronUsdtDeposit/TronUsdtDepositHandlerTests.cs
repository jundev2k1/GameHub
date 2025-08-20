using FluentAssertions;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;
using game_x.domain.Constants;
using game_x.domain.Entities;
using game_x.domain.Enum;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Test.UnitTests.Application.TronUsdtDeposit;

public sealed class TronUsdtDepositHandlerTests
{
    private readonly Mock<IUxmService> _uxmServiceMock = new();
    private readonly Mock<IChainTransactionRepo> _chainTransactionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAsymmetricCryptoService> _asymmetricCryptoServiceMock = new();
    private readonly Mock<IUserAccessor> _userAccessorMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IConfigurationSection> _configurationSectionMock = new();
    private readonly Mock<IAsymmetricKeyCacheService> _asymmetricKeyCacheServiceMock = new();
    private readonly Mock<ICryptoTokenRepo> _cryptoTokenRepoMock = new();
    private readonly CreateDepositChainTransactionHandler _handler;

    public TronUsdtDepositHandlerTests()
    {
        _handler = new CreateDepositChainTransactionHandler(
            uxmService: _uxmServiceMock.Object,
            chainTransactionRepo:  _chainTransactionRepoMock.Object,
            unitOfWork: _unitOfWorkMock.Object,
            asymmetricCryptoService: _asymmetricCryptoServiceMock.Object,
            userAccessor: _userAccessorMock.Object,
            configuration: _configurationMock.Object,
            asymmetricKeyCacheService: _asymmetricKeyCacheServiceMock.Object,
            cryptoTokenRepo: _cryptoTokenRepoMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResponse_WhenDepositIsSuccessful()
    {
        // Arrange
        var request = new TronUsdtDepositCommand(100m, "Test deposit", Guid.NewGuid());
        var userId = "user123";
        var merchantNumber = "MERCHANT001";
        var cryptoToken = new CryptoToken { PublicId = Guid.NewGuid(), Symbol = CryptoTokenSymbol.Usdt };
        var privateKey = "private-key";
        var publicKey = "public-key";
        var signature = "signature";

        var uxmResponseData = new UxmDepositOrderResponseData("UXM123", 100m, "TRX123ADDRESS");
        var uxmResponse = new SecureResponse<UxmDepositOrderResponseData>(uxmResponseData, "uxm-signature");

        _userAccessorMock.Setup(x => x.GetUserId()).Returns(userId);
        _configurationSectionMock.Setup(x => x.Value).Returns(merchantNumber);
        _configurationMock.Setup(x => x.GetSection("GameXSettings:MerchantNumber")).Returns(_configurationSectionMock.Object);
        _cryptoTokenRepoMock.Setup(x => x.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cryptoToken);
        _asymmetricKeyCacheServiceMock.Setup(x => x.GameXPrivateKey).Returns(privateKey);
        _asymmetricKeyCacheServiceMock.Setup(x => x.UxmPublicKey).Returns(publicKey);
        _asymmetricCryptoServiceMock.Setup(x => x.Sign(privateKey, It.IsAny<UxmDepositOrderRequestData>()))
            .Returns(signature);
        _uxmServiceMock.Setup(x => x.CreateDepositOrderAsync(It.IsAny<SecureRequest<UxmDepositOrderRequestData>>()))
            .ReturnsAsync(uxmResponse);
        _asymmetricCryptoServiceMock.Setup(x => x.VerifySignature(publicKey, uxmResponse.Data, uxmResponse.Signature))
            .Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderUid.Should().Be("UXM123");
        result.Amount.Should().Be(100m);
        result.To.Should().Be("TRX123ADDRESS");

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenCryptoTokenNotFound()
    {
        // Arrange
        var request = new TronUsdtDepositCommand(100m, "Test deposit", Guid.NewGuid());
        var userId = "user123";

        _userAccessorMock.Setup(x => x.GetUserId()).Returns(userId);
        _cryptoTokenRepoMock.Setup(x => x.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CryptoToken?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(request, CancellationToken.None));
        exception.ErrorCode.Should().Be(MessageCode.Crypto.CryptoTokenNotFound);

        _unitOfWorkMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenUxmSignatureIsInvalid()
    {
        // Arrange
        var request = new TronUsdtDepositCommand(100m, "Test deposit", Guid.NewGuid());
        var userId = "user123";
        var merchantNumber = "MERCHANT001";
        var cryptoToken = new CryptoToken { PublicId = Guid.NewGuid(), Symbol = CryptoTokenSymbol.Usdt };
        var publicKey = "public-key";

        var uxmResponseData = new UxmDepositOrderResponseData("UXM123", 100m, "TRX123ADDRESS");
        var uxmResponse = new SecureResponse<UxmDepositOrderResponseData>(uxmResponseData, "valid-signature");

        _userAccessorMock.Setup(x => x.GetUserId()).Returns(userId);
        _configurationSectionMock.Setup(x => x.Value).Returns(merchantNumber);
        _configurationMock.Setup(x => x.GetSection("GameXSettings:MerchantNumber")).Returns(_configurationSectionMock.Object);
        _cryptoTokenRepoMock.Setup(x => x.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cryptoToken);
        _uxmServiceMock.Setup(x => x.CreateDepositOrderAsync(It.IsAny<SecureRequest<UxmDepositOrderRequestData>>()))
            .ReturnsAsync(uxmResponse);
        _asymmetricKeyCacheServiceMock.Setup(x => x.UxmPublicKey).Returns(publicKey);
        _asymmetricCryptoServiceMock.Setup(x => x.VerifySignature(publicKey, uxmResponse.Data, uxmResponse.Signature))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(request, CancellationToken.None));
        exception.Message.Should().Be("Invalid signature from UXM.");

        _unitOfWorkMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRollbackTransaction_WhenExceptionOccurs()
    {
        // Arrange
        var request = new TronUsdtDepositCommand(100m, "Test deposit", Guid.NewGuid());

        _cryptoTokenRepoMock.Setup(x => x.GetBySymbolAndNetworkAsync(It.IsAny<string>(), It.IsAny<NetworkType>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
        _unitOfWorkMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

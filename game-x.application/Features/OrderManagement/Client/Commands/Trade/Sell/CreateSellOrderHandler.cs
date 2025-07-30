using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace game_x.application.Features.OrderManagement.Client.Commands.Trade.Sell;

public sealed class CreateSellOrderHandler(
    IUxmService uxmService,

    IUserRepo userRepo,
    IAuthService authService,
    IUnitOfWork unitOfWork,
    IAsymmetricKeyRepo asymmetricKeyRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUserAccessor userAccessor,
    IConfiguration configuration,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CreateSellOrderCommand, CreateOrderResponseDto>
{
    public async Task<CreateOrderResponseDto> Handle(CreateSellOrderCommand request, CancellationToken ct)
    {
        // 1. Gọi API UXM
        var uxmRequest = await CreateUxmRequest(request, ct);

        var apiResponse = await uxmService.CreateProxyBuyOrderAsync(uxmRequest);
        // if (!apiResponse.IsSuccessStatusCode || apiResponse.Content == null)
        //     throw new BadRequestException("UXM API failed.");

        var result = apiResponse;

        // 2. Xác minh chữ ký từ UXM
        var uxmPublicKey = await asymmetricKeyRepo
            .GetByCompositeKeyAsync(AsymmetricKeyNames.Uxm, AsymmetricKeyType.Public, AsymmetricType.ECDSA, ct);

        var isValid = asymmetricCryptoService
            .VerifySignature(uxmPublicKey.KeyValue, result.Data, result.Signature);

        if (!isValid)
            throw new BadRequestException("Invalid signature from UXM.");

        // 3. Trả về kết quả (ví dụ trả thẳng data)
        return new CreateOrderResponseDto
        {
            OrderUid = result.Data.OrderUid,
            Amount = result.Data.Amount,
            To = result.Data.To
        };
    }

    private async Task<SecureRequest<CreateOrderBuyRequestData>> CreateUxmRequest(
        CreateSellOrderCommand request,

        CancellationToken ct = default)
    {
        // Line ~50: Get Galaxy Pay private key
        var privateKeyPem = await asymmetricKeyRepo
            .GetByCompositeKeyAsync(AsymmetricKeyNames.GameX, AsymmetricKeyType.Private, AsymmetricType.ECDSA, ct);


        // Line ~58: Create request data
        var requestData = new CreateOrderBuyRequestData(
            request.MerchantNumber,
            request.amount,
            request.orderNumber,
            request.userId,
            request.remark
        );

        // Line ~64: Generate signature
        var signature = asymmetricCryptoService.Sign(privateKeyPem.KeyValue, requestData);

        // Line ~66: Return secure request
        return new SecureRequest<CreateOrderBuyRequestData>
        {
            Data = requestData,
            Signature = signature
        };
    }

}


using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnOrderCreated;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace game_x.application.Features.OrderManagement.Staff.Commands.Trade.Sell;

public sealed class CreateSellOrderHandler(
    OrderMapper orderMapper,
    IUxmService uxmService,
    IUserRepo userRepo,
    IAuthService authService,
    IOrderRepo orderRepo,
    IUnitOfWork unitOfWork,
    IAsymmetricKeyRepo asymmetricKeyRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IStaffCounterRepo staffCounterRepo,
    IBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor,
    IConfiguration configuration,
    IApplicationEventDispatcher eventDispatcher)
    : ICommandHandler<CreateSellOrderCommand, CreateOrderResponseDto>
{
    public async Task<CreateOrderResponseDto> Handle(CreateSellOrderCommand request, CancellationToken ct = default)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 1. Create a local Order at Galaxy Pay
            var localOrder = await CreateLocalOrder(request, ct);

            // 2. Call to UXM server to create Order
            var uxmRequest = await CreateUxmRequest(request, localOrder.PublicId, ct);
            var result = await uxmService.CreateProxySellOrderAsync(uxmRequest);

            // 3. Verify UXM signature
            var uxmPublicKey = await asymmetricKeyRepo
                .GetByCompositeKeyAsync(AsymmetricKeyNames.Uxm, KeyType.Public, AsymmetricType.ECDSA, ct);
            var isValid = asymmetricCryptoService.VerifySignature(
                publicKeyPem: uxmPublicKey.KeyValue,
                data: result.Data,
                signatureBase64: result.Signature);
            if (!isValid) throw new BadRequestException();

            // 4. Update Order after UXM returns response
            localOrder.UpdateOrderUid(result.Data.OrderUid);
            var metadata = new Dictionary<string, object>
            {
                { "entryCode", result.Data.EntryCode }
            };
            localOrder.UpdateMetadata(JsonSerializer.Serialize(metadata));

            // 5. Commit local Order to Galaxy Pay DB
            await unitOfWork.CommitAsync(ct);

            // 6. Notice the member for updating order's status
            await eventDispatcher.Publish(new OnOrderCreatedEvent(localOrder), ct);

            return orderMapper.ToCreateOrderDto(result.Data, isValid);
        }
        catch (BadRequestException)
        {
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException("Invalid signature.");
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<Order> CreateLocalOrder(CreateSellOrderCommand request, CancellationToken ct = default)
    {
        var ownerUser = await userRepo.GetUserByIdAsync(request.MemberId, ct: ct);
        var role = await authService.GetRolesAsync(ownerUser);
        if (!role.IsUser) throw new ForbiddenException("Only user can create sell order.");

        var staffId = userAccessor.GetUserId();
        var counter = await staffCounterRepo.GetTrackingLogAsync(staffId, ct);

        var order = Order.Create(
            type: OrderType.Sell,
            userId: ownerUser.Id,
            counterId: counter.CounterId,
            staffId: staffId,
            quantity: 1,
            pricePerUnit: request.FiatAmount,
            currencyUnit: CurrencyUnit.Of(request.FiatType),
            status: OrderStatus.Created);
        await orderRepo.AddAsync(order, ct);
        return order;
    }

    private async Task<SecureRequest<CreateOrderSellRequestData>> CreateUxmRequest(
        CreateSellOrderCommand request,
        Guid publicId,
        CancellationToken ct)
    {
        // Get bank account information
        var bankInfo = await bankAccountRepo.GetByIdAsync(request.BankAccountId, ct);
        if (bankInfo.OwnerId != request.MemberId)
            throw new BadRequestException("Bank account does not belong to the user.");

        // Get Galaxy Pay private key
        var privateKeyPem = await asymmetricKeyRepo
            .GetByCompositeKeyAsync(AsymmetricKeyNames.GalaxyPay, KeyType.Private, AsymmetricType.ECDSA, ct);

        // Get merchant number from configuration
        var merchantNumber = configuration.GetValue<string>("GalaxySettings:MerchantNumber")
            ?? throw new Exception("MerchantNumber is not yet configured.");

        // Create UXM request data
        var requestData = orderMapper.ToCreateOrderSellRequestData(
            request,
            publicId.ToString(),
            merchantNumber,
            bankInfo);
        var uxmRequest = new SecureRequest<CreateOrderSellRequestData>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(privateKeyPem.KeyValue, requestData)
        };
        return uxmRequest;
    }
}

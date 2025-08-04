using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactionManagement.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Transactions;

namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtDeposit;

public sealed class CreateDepositChainTransactionHandler(
    IUxmService uxmService,
    IChainTransactionRepo chainTransactionRepo,
     // IUserRepo userRepo,
     // IAuthService authService,
     IUnitOfWork unitOfWork,
    IAsymmetricKeyRepo asymmetricKeyRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUserAccessor userAccessor,
    IConfiguration configuration
    //IApplicationEventDispatcher eventDispatcher
    ) : ICommandHandler<TronUsdtDepositCommand, CreateChainTransactionResponseDto>
{
    public async Task<CreateChainTransactionResponseDto> Handle(TronUsdtDepositCommand request, CancellationToken ct)

    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {



            var localChainTransaction = await CreateLocalChainTransaction(request, ct);

            // 1. Gọi API UXM
            var uxmRequest = await CreateUxmRequest(request, localChainTransaction.PublicId, ct);
            var apiResponse = await uxmService.CreateProxyChainTransactionDepositAsync(uxmRequest);
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

            // 6. Commit local Order to Galaxy Pay DB
            await unitOfWork.CommitAsync(ct);
            // 3. Trả về kết quả (ví dụ trả thẳng data)
            return new CreateChainTransactionResponseDto
            {
                OrderUid = result.Data.OrderUid,
                Amount = result.Data.Amount,
                To = result.Data.To
            };

        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }

    }
    private async Task<ChainTransaction> CreateLocalChainTransaction(TronUsdtDepositCommand request, CancellationToken ct = default)
    {
        // var ownerUser = await userRepo.GetUserByIdAsync(request.MemberId, ct);
        //var role = await authService.GetRolesAsync(ownerUser);
        //if (!role.IsUser) throw new ForbiddenException("Only user can create sell order.");

        var userId = userAccessor.GetUserId();


        var transaction = ChainTransaction.Create(
            userId: userId,
        orderNumber: "string",
        cryptoTokenId: 1,
        amount: request.amount,
        type: ChainTransactionType.Deposit,
        status: ChainTransactionStatus.Pending
        );

        await chainTransactionRepo.AddAsync(transaction, ct);
        return transaction;
    }
    private async Task<SecureRequest<CreateChainTransactionDepositRequestData>> CreateUxmRequest(
        TronUsdtDepositCommand request,
 Guid publicId,
        CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        // Line ~50: Get Galaxy Pay private key
        var privateKeyPem = await asymmetricKeyRepo
            .GetByCompositeKeyAsync(AsymmetricKeyNames.GameX, AsymmetricKeyType.Private, AsymmetricType.ECDSA, ct);


        // Get merchant number from configuration
        var merchantNumber = configuration.GetValue<string>("GameXSettings:MerchantNumber")
            ?? throw new Exception("MerchantNumber is not yet configured.");

        // Line ~58: Create request data
        var requestData = new CreateChainTransactionDepositRequestData(
            merchantNumber,
            request.amount,
          publicId.ToString(),
            userId,
            request.remark
        );

        // Line ~64: Generate signature
        var signature = asymmetricCryptoService.Sign(privateKeyPem.KeyValue, requestData);

        // Line ~66: Return secure request
        return new SecureRequest<CreateChainTransactionDepositRequestData>
        {
            Data = requestData,
            Signature = signature
        };
    }

}

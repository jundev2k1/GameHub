using game_x.api.Controllers;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayDepositSuccess;
using game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayWithdrawalFailed;
using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionCompleted;
using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionFailed;
using System.Text;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("api/webhooks/fastpay")]
public sealed class FastPayHookController(IAppLogger<FastPayHookController> logger) : BaseApiController
{
    [HttpPost("deposit-success")]
    public async Task<IActionResult> DepositSuccessAsync(CancellationToken ct = default)
    {
        logger.LogInformation("===== FastPay web hook: Deposit Success =====");

        Request.EnableBuffering();
        using var reader = new StreamReader(
            Request.Body,
            Encoding.UTF8,
            leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);

        Request.Body.Position = 0;

        logger.LogInformation("Raw Body: {Body}", rawBody);

        var request = JsonSerializer.Deserialize<SecureRequest<TransactionCompletedRequest>>(rawBody)
            ?? throw new BadRequestException("Invalid request body");

        var command = new FastPayDepositSuccessCommand(
            request.Data,
            request.Signature,
            rawBody);

        await Mediator.Send(command, ct);
        return ApiResponseFactory.NoContent();
    }

    [HttpPost("deposit-failed")]
    public async Task<IActionResult> DepositFailedAsync([FromBody] SecureRequest<TransactionFailedRequest> request, CancellationToken ct = default)
    {
        var command = new FastPayWithdrawalFailedCommand(request.Data, request.Signature);
        await Mediator.Send(command, ct);

        return ApiResponseFactory.NoContent();
    }

    [HttpPost("withdraw-success")]
    public async Task<IActionResult> WithdrawalSuccessAsync(CancellationToken ct = default)
    {
        logger.LogInformation("===== FastPay web hook: Withdrawal Success =====");

        Request.EnableBuffering();
        using var reader = new StreamReader(
            Request.Body,
            Encoding.UTF8,
            leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);

        Request.Body.Position = 0;

        logger.LogInformation("Raw Body: {Body}", rawBody);

        var request = JsonSerializer.Deserialize<SecureRequest<TransactionCompletedRequest>>(rawBody)
            ?? throw new BadRequestException("Invalid request body");

        var command = new FastPayDepositSuccessCommand(request.Data, request.Signature, rawBody);
        await Mediator.Send(command, ct);
        return ApiResponseFactory.NoContent();
    }

    [HttpPost("withdraw-failed")]
    public async Task<IActionResult> WithdrawalFailedAsync([FromBody] SecureRequest<TransactionFailedRequest> request, CancellationToken ct = default)
    {
        var command = new FastPayWithdrawalFailedCommand(request.Data, request.Signature);
        await Mediator.Send(command, ct);

        return ApiResponseFactory.NoContent();
    }
}

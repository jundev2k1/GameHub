namespace game_x.application.Features.Kyc.Queries.GetKycStatus;

public sealed class GetKycStatusHandler : IQueryHandler<GetKycStatusQuery, GetKycStatusResult>
{
    public async Task<GetKycStatusResult> Handle(GetKycStatusQuery request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return new GetKycStatusResult();
    }
}

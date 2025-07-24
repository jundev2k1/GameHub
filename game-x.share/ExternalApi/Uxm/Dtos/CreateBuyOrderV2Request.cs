namespace game_x.share.ExternalApi.Uxm.Dtos;

public class CreateBuyOrderV2Request
{
    public CreateBuyOrderV2ReqData Data { get; set; } = default!;

    public string Signature { get; set; } = default!;
}
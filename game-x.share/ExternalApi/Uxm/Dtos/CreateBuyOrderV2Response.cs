namespace game_x.share.ExternalApi.Uxm.Dtos;

public class CreateBuyOrderV2ResponseData
{
    public string EntryCode { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; }
    public string OrderUid { get; set; } = string.Empty;
    public required decimal FiatAmount { get; set; }
    public required decimal CryptoAmount { get; set; }
    public required int FiatType { get; set; }
    public required int CryptoType { get; set; }
    public required decimal Fee { get; set; }
    public required long Timestamp { get; set; }
}

public class CreateBuyOrderV2Response
{
    public required CreateBuyOrderV2ResponseData Data { get; set; }
    public required string Signature { get; set; }
}
namespace game_x.share.ExternalApi.Atg.Dtos.UpdateGameBalance;

public class UpdateGameBalanceRequest
{
    /// <summary>
    /// If the addend or subtractor is greater than 0, only the last two decimal places are supported.
    /// </summary>
    public required decimal Balance { get; set; }
    public required string Username { get; set; }
    /// <summary>
    /// Invalid withdrawal or debit codes are set to IN by default. Allowed values are: "IN", "OUT".
    /// </summary>
    public required string Action { get; set; }
    /// <summary>Business operator's transfer transaction number</summary>
    public required string TransferId { get; set; }
}
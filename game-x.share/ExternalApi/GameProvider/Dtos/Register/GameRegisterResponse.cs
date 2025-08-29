namespace game_x.share.ExternalApi.GameProvider.Dtos.Register;

public sealed class GameRegisterResponse : ResponseBase
{
    /// <summary>註冊用戶ID(唯一值)</summary>
    public int Id { get; set; }
}

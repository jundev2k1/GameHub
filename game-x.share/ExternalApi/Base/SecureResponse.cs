namespace game_x.share.ExternalApi.Base;

public record SecureResponse<T>(T Data, string Signature);

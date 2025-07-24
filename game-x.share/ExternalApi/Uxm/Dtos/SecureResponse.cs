namespace game_x.share.ExternalApi.Uxm.Dtos;

public record SecureResponse<T>(T Data, string Signature);

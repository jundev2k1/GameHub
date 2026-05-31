namespace game_x.share.ExternalApi.PaymentGateway.Dtos;

public record SecureResponse<T>(T Data, string Signature);

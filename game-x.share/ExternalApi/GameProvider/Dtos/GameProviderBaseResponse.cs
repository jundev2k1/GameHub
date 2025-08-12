namespace game_x.share.ExternalApi.GameProvider.Dtos;

public record GameProviderBaseResponse(
    bool issuccess,
    string? message = null,
    string? errorcode = null,
    string? errormessage = null
);
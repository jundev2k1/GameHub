namespace game_x.application.Features.LiveStreams.Gifts.Dtos;

public record LiveStreamGiftPriceInputDto(Guid CryptoTokenId, decimal TokenCost, bool IsActive);

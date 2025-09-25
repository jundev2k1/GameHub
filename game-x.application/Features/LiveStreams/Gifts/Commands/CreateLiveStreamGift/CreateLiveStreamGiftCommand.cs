using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.CreateLiveStreamGift;

public record CreateLiveStreamGiftCommand(
    string Name,
    string? Notes,
    int Priority,
    LiveStreamGiftPriceInputDto[] GiftPrices) : ICommand;

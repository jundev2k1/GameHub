using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.OrderManagement.Shared.Commands.Callback.Fiat;

public record UpdateOrderCallbackCommand(FiatCallbackRequest Data, string Signature) : ICommand<UpdateOrderCallbackResult>;

public record UpdateOrderCallbackResult(string Message);

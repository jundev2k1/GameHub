using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.OrderManagement.Admin.Commands.UpdateOrderInfoByAdmin;

public sealed class UpdateOrderInfoByAdminHandler(IUnitOfWork unitOfWork, IOrderRepo orderRepo)
    : ICommandHandler<UpdateOrderInfoByAdminCommand>
{
    public async Task<Unit> Handle(UpdateOrderInfoByAdminCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await orderRepo.UpdateAsync(request.OrderId, order =>
            {
                order.Update(
                    request.Quantity,
                    request.PriceOfUnit,
                    CurrencyUnit.Of(request.CurrencyUnit),
                    OrderStatus.Of(request.OrderStatus),
                    request.Notes);
            }, ct);
        }, ct);

        return Unit.Value;
    }
}

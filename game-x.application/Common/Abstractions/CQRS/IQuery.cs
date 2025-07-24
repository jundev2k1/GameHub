namespace game_x.application.Common.Abstractions.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

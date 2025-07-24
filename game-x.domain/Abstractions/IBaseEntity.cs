namespace game_x.domain.Abstractions;

public interface IBaseEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public interface IBaseEntity<T> : IBaseEntity
{
    T Id { get; set; }
}

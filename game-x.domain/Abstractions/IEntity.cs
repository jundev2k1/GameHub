namespace game_x.domain.Abstractions;

public interface IEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public interface IEntity<T> : IEntity
{
    T Id { get; set; }
}

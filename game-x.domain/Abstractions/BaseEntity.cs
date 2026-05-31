namespace game_x.domain.Abstractions;

public abstract class BaseEntity<T> : IEntity<T>
{
    public T Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
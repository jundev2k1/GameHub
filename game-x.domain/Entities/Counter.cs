namespace game_x.domain.Entities;

public sealed class Counter : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public CounterNumber CounterNumber { get; private set; } = default!;
    public string CounterName { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public CounterStatus Status { get; set; } = CounterStatus.Active;

    public CounterToken CounterToken { get; private set; } = default!;
    public ICollection<Order> Orders { get; private set; } = new List<Order>();

    public static Counter Create(
        CounterNumber counterNumber,
        string name,
        string location,
        string desc,
        bool isDeleted = false,
        List<Order>? orders = null)
    {
        var counter = new Counter
        {
            CounterNumber = counterNumber,
            CounterName = name,
            Location = location,
            Description = desc,
            IsDeleted = isDeleted,
            Orders = orders ?? new List<Order>()
        };
        return counter;
    }

    public void AttachToken(CounterToken counterToken)
    {
        CounterToken = counterToken;
    }

    public void Update(
        string name,
        CounterStatus status,
        string location,
        string desc)
    {
        CounterName = name;
        Status = status;
        Location = location;
        Description = desc;
    }

    public void UpdateStatus(CounterStatus status)
    {
        Status = status;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public bool IsActive()
    {
        return Status != CounterStatus.Inactive;
    }
}

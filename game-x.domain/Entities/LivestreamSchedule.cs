namespace game_x.domain.Entities;

public sealed class LivestreamSchedule : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public string Title { get; private set; } = string.Empty;
    public int? ThumbnailId { get; private set; }
    public MediaFile? Thumbnail { get; private set; }
    public string? Description { get; private set; }
    public string? Notes { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public DateTime? StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }
    public string StreamKey { get; private set; } = string.Empty;
    public string Token { get; private set; } = string.Empty;
    public LiveStreamStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? AssignedId { get; private set; }
    public User? AssignedTo { get; private set; }

    public ICollection<LiveStreamCategoryMapping> CategoryMappings { get; private set; } = [];

    public static LivestreamSchedule Create(
        string title,
        DateTime startTime,
        DateTime endTime,
        string? description = null,
        string? notes = null,
        List<LiveStreamCategoryMapping>? categories = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.", nameof(endTime));

        return new LivestreamSchedule
        {
            Title = title,
            StartTime = startTime,
            EndTime = endTime,
            Description = description,
            Notes = notes,
            Status = LiveStreamStatus.Scheduled,
            StreamKey = GenerateStreamKey(),
            Token = GenerateToken(),
            CategoryMappings = categories ?? [],
        };
    }

    public void Update(
        string? title = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? description = null,
        string? notes = null,
        List<LiveStreamCategoryMapping>? categories = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.", nameof(endTime));

        Title = title ?? Title;
        StartTime = startTime ?? StartTime;
        EndTime = endTime ?? EndTime;
        Description = description ?? Description;
        Notes = notes ?? Notes;

        if (categories != null)
            CategoryMappings = categories;
    }

    public void UpdateThumbnail(MediaFile thumbnail)
    {
        Thumbnail = thumbnail;
    }

    public void AssignStream(string assignedId)
    {
        if (string.IsNullOrWhiteSpace(assignedId))
            throw new ArgumentException("Assigned ID is required.", nameof(assignedId));

        AssignedId = assignedId;
    }

    public void StartStream()
    {
        if (Status != LiveStreamStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled streams can be started.");
        if (AssignedId.IsNullOrWhiteSpace())
            throw new InvalidOperationException("Stream must be assigned before starting.");

        Status = LiveStreamStatus.Live;
        StartAt = DateTime.UtcNow;
    }

    public void DisconnectStream()
    {
        if (Status != LiveStreamStatus.Live)
            throw new InvalidOperationException("Only live streams can be disconnected.");

        EndAt = DateTime.UtcNow;
    }

    public void EndStream(DateTime? endTime = null)
    {
        if (Status != LiveStreamStatus.Live)
            throw new InvalidOperationException("Only live streams can be ended.");

        Status = LiveStreamStatus.Ended;

        if (!EndAt.HasValue)
            EndAt = endTime ?? DateTime.UtcNow;
    }

    public void CancelStream(string reason)
    {
        if (Status == LiveStreamStatus.Ended)
            throw new InvalidOperationException("Ended streams cannot be canceled.");
        if (reason.IsNullOrWhiteSpace())
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));

        Status = LiveStreamStatus.Cancelled;
        CancellationReason = reason;
        EndAt = DateTime.UtcNow;
    }

    private static string GenerateStreamKey()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}

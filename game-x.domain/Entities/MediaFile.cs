namespace game_x.domain.Entities;

public sealed class MediaFile : BaseEntity<int>, IAuditable
{
    public BucketName BucketName { get; private set; } = default!;
    public ObjectName ObjectName { get; private set; } = default!;
    public string FileName { get; private set; } = string.Empty;
    public MimeType MimeType { get; private set; } = default!;
    public int SizeBytes { get; private set; }
    public string Metadata { get; private set; } = string.Empty;

    public static MediaFile Create(
        BucketName bucketName,
        ObjectName objectName,
        string fileName,
        MimeType mimeType,
        int sizeBytes,
        string metadata = "{}")
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        if (sizeBytes <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(sizeBytes));
        if (JsonHelper.IsJson(metadata) == false)
            throw new ArgumentException("Metadata must be a valid JSON string.", nameof(metadata));

        return new MediaFile
        {
            BucketName = bucketName,
            ObjectName = objectName,
            FileName = fileName,
            MimeType = mimeType,
            SizeBytes = sizeBytes,
            Metadata = metadata
        };
    }

    public void Update(BucketName bucketName, ObjectName objectName, string fileName, MimeType mimeType, int length)
    {
        BucketName = bucketName;
        ObjectName = objectName;
        FileName = fileName;
        MimeType = mimeType;
        SizeBytes = length;
    }
}

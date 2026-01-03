namespace OneUniBackend.Models;

public record StorageObjectRequest(
    string Folder,
    string ObjectKey,
    string ContentType,
    long ContentLength);

public record StorageObjectReference(string Folder, string ObjectKey);

public record PresignedUploadResult(
    string UploadUrl,
    DateTimeOffset ExpiresAt,
    string Folder,
    string ObjectKey,
    string? Signature = null,
    long? Timestamp = null,
    string? ApiKey = null,
    string? CloudName = null,
    string? ResourceType = null,
    string? PublicId = null);

using OneUniBackend.Models;

namespace OneUniBackend.Interfaces.Services;

public interface IStorageService
{
    Task<PresignedUploadResult> GenerateUploadUrlAsync(StorageObjectRequest request, CancellationToken cancellationToken = default);
    Task<string> GenerateDownloadUrlAsync(StorageObjectReference reference, TimeSpan lifetime, CancellationToken cancellationToken = default);
}


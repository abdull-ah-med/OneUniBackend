using System.Security.Cryptography;
using System.Text;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using OneUniBackend.Configuration;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Models;

namespace OneUniBackend.Services;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinarySettings _settings;
    private readonly ILogger<CloudinaryStorageService> _logger;

    public CloudinaryStorageService(
        Cloudinary cloudinary,
        IOptions<CloudinarySettings> settings,
        ILogger<CloudinaryStorageService> logger)
    {
        _cloudinary = cloudinary;
        _settings = settings.Value;
        _logger = logger;
    }

    public Task<PresignedUploadResult> GenerateUploadUrlAsync(StorageObjectRequest request, CancellationToken cancellationToken = default)
    {
        var resourceType = GetResourceType(request.ContentType);
        var publicId = $"{request.Folder}/{request.ObjectKey}";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.PresignExpiryMinutes);
        var paramsToSign = new SortedDictionary<string, object>
        {
            { "public_id", publicId },
            { "timestamp", timestamp }
        };

        var signature = GenerateSignature(paramsToSign);

        var uploadUrl = $"https://api.cloudinary.com/v1_1/{_settings.CloudName}/{resourceType}/upload";

        return Task.FromResult(new PresignedUploadResult(
            UploadUrl: uploadUrl,
            ExpiresAt: expiresAt,
            Folder: request.Folder,
            ObjectKey: request.ObjectKey,
            Signature: signature,
            Timestamp: timestamp,
            ApiKey: _settings.ApiKey,
            CloudName: _settings.CloudName,
            ResourceType: resourceType,
            PublicId: publicId));
    }

    public Task<string> GenerateDownloadUrlAsync(StorageObjectReference reference, TimeSpan lifetime, CancellationToken cancellationToken = default)
    {
        var publicId = $"{reference.Folder}/{reference.ObjectKey}";
        
        var expiresAt = DateTimeOffset.UtcNow.Add(lifetime).ToUnixTimeSeconds();
        
        var paramsToSign = new SortedDictionary<string, object>
        {
            { "public_id", publicId },
            { "timestamp", expiresAt }
        };
        
        var signature = GenerateSignature(paramsToSign);
        
        var extension = Path.GetExtension(reference.ObjectKey).ToLowerInvariant();
        var resourceType = extension switch
        {
            ".pdf" => "raw",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => "image",
            ".mp4" or ".mov" or ".avi" => "video",
            _ => "raw"
        };

        if (resourceType == "raw")
        {
            var url = $"https://res.cloudinary.com/{_settings.CloudName}/raw/upload/{publicId}";
            return Task.FromResult(url);
        }

        var imageUrl = $"https://res.cloudinary.com/{_settings.CloudName}/image/upload/{publicId}";
        return Task.FromResult(imageUrl);
    }

    private string GetResourceType(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" or "image/png" or "image/gif" or "image/webp" => "image",
            "video/mp4" or "video/quicktime" or "video/x-msvideo" => "video",
            _ => "raw"
        };
    }

    private string GenerateSignature(SortedDictionary<string, object> parameters)
    {
        var stringToSign = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        stringToSign += _settings.ApiSecret;

        using var sha1 = SHA1.Create();
        var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}


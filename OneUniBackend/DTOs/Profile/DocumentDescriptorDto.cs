using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Profile;

public class DocumentDescriptorDto
{
    public Guid DocumentId { get; set; }
    public DocumentType DocumentType { get; set; }
    public Guid? EducationalRecordId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
    public string? ObjectKey { get; set; }
    public string? Bucket { get; set; }
    public string? MimeType { get; set; }
    public int? FileSize { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public string? StorageProvider { get; set; }
}


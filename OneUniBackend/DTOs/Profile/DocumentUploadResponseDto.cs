using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Profile;

public class DocumentUploadResponseDto
{
    public Guid DocumentId { get; set; }
    public DocumentType DocumentType { get; set; }
    public Guid? EducationalRecordId { get; set; }
    
    // Upload endpoint
    public string UploadUrl { get; set; } = string.Empty;
    
    // Cloudinary-specific fields for signed upload
    public string? Signature { get; set; }
    public long? Timestamp { get; set; }
    public string? ApiKey { get; set; }
    public string? CloudName { get; set; }
    public string? ResourceType { get; set; }
    public string? PublicId { get; set; }
    
    // Common fields
    public string ObjectKey { get; set; } = string.Empty;
    public string Folder { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Profile;

public class DocumentUploadRequestDto
{
    public Guid? DocumentId { get; set; }
    public Guid? EducationalRecordId { get; set; }

    [Required]
    public DocumentType DocumentType { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    [Range(1, long.MaxValue)]
    public long FileSize { get; set; }

    public string? Checksum { get; set; }
    public JsonElement? Metadata { get; set; }
}


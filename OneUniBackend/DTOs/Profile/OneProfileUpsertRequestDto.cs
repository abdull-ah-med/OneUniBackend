using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Profile;

public class OneProfileUpsertRequestDto
{
    [Required]
    public StudentProfilePayloadDto StudentProfile { get; set; } = new();

    public List<EducationalRecordDto> EducationalRecords { get; set; } = new();

    public List<DocumentUploadRequestDto> Documents { get; set; } = new();
}


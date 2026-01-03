namespace OneUniBackend.DTOs.Profile;

public class OneProfileResponseDto
{
    public StudentProfilePayloadDto StudentProfile { get; set; } = new();
    public List<EducationalRecordDto> EducationalRecords { get; set; } = new();
    public List<DocumentDescriptorDto> Documents { get; set; } = new();
}

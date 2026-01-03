using OneUniBackend.DTOs.Profile;

namespace OneUniBackend.Interfaces.Services;

public interface IOneProfileService
{
    Task<OneProfileResponseDto> GetStudentProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(OneProfileResponseDto Response, IReadOnlyList<DocumentUploadResponseDto> Uploads)> UpsertStudentProfileAsync(Guid userId, OneProfileUpsertRequestDto request, CancellationToken cancellationToken = default);
}


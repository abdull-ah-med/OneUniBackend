using OneUniBackend.Entities;
using OneUniBackend.Enums;

namespace OneUniBackend.Interfaces.Repositories;

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<Document?> GetByIdForUserAsync(Guid documentId, Guid userId, CancellationToken cancellationToken = default);
    Task<Document?> GetByTypeForRecordAsync(Guid? educationalRecordId, Guid userId, DocumentType documentType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}


using Microsoft.EntityFrameworkCore;
using OneUniBackend.Data;
using OneUniBackend.Entities;
using OneUniBackend.Enums;
using OneUniBackend.Interfaces.Repositories;

namespace OneUniBackend.Repositories;

public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    public DocumentRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<Document?> GetByIdForUserAsync(Guid documentId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            d => d.DocumentId == documentId && d.UserId == userId,
            cancellationToken);
    }

    public async Task<Document?> GetByTypeForRecordAsync(Guid? educationalRecordId, Guid userId, DocumentType documentType, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            d => d.EducationalRecordId == educationalRecordId &&
                 d.UserId == userId &&
                 d.DocumentType == documentType,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}


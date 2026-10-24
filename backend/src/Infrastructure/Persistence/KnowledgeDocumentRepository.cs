using CosmicCore.Application.Knowledge;
using CosmicCore.Domain.Knowledge;
using Microsoft.EntityFrameworkCore;

namespace CosmicCore.Infrastructure.Persistence;

public sealed class KnowledgeDocumentRepository(CosmicCoreDbContext dbContext) : IKnowledgeDocumentRepository
{
    public async Task<IReadOnlyList<KnowledgeDocument>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.KnowledgeDocuments.AsNoTracking().OrderByDescending(x => x.UpdatedAt).ToListAsync(cancellationToken);

    public Task<KnowledgeDocument?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.KnowledgeDocuments.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(KnowledgeDocument document, CancellationToken cancellationToken) =>
        await dbContext.KnowledgeDocuments.AddAsync(document, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    public void Remove(KnowledgeDocument document) => dbContext.KnowledgeDocuments.Remove(document);
}

using CosmicCore.Domain.Knowledge;

namespace CosmicCore.Application.Knowledge;

public interface IKnowledgeDocumentRepository
{
    Task<IReadOnlyList<KnowledgeDocument>> ListAsync(CancellationToken cancellationToken);
    Task<KnowledgeDocument?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(KnowledgeDocument document, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Remove(KnowledgeDocument document);
}

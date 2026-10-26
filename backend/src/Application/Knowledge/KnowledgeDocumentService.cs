using CosmicCore.Domain.Knowledge;

namespace CosmicCore.Application.Knowledge;

public sealed record KnowledgeDocumentData(Guid Id, string Title, string Content, string? SourceUrl, DateTime CreatedAt, DateTime UpdatedAt);

public sealed class KnowledgeDocumentService(IKnowledgeDocumentRepository repository, TimeProvider timeProvider)
{
    public async Task<IReadOnlyList<KnowledgeDocumentData>> ListAsync(CancellationToken cancellationToken) =>
        (await repository.ListAsync(cancellationToken)).Select(ToData).ToArray();

    public async Task<KnowledgeDocumentData?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        (await repository.FindAsync(id, cancellationToken)) is { } document ? ToData(document) : null;

    public async Task<KnowledgeDocumentData> CreateAsync(string title, string content, string? sourceUrl, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var document = new KnowledgeDocument(Guid.NewGuid(), title, content, sourceUrl, now);
        await repository.AddAsync(document, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ToData(document);
    }

    public async Task<KnowledgeDocumentData?> UpdateAsync(Guid id, string title, string content, string? sourceUrl, CancellationToken cancellationToken)
    {
        var document = await repository.FindAsync(id, cancellationToken);
        if (document is null) return null;
        document.Update(title, content, sourceUrl, timeProvider.GetUtcNow().UtcDateTime);
        await repository.SaveChangesAsync(cancellationToken);
        return ToData(document);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await repository.FindAsync(id, cancellationToken);
        if (document is null) return false;
        repository.Remove(document);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static KnowledgeDocumentData ToData(KnowledgeDocument document) =>
        new(document.Id, document.Title, document.Content, document.SourceUrl, document.CreatedAt, document.UpdatedAt);
}

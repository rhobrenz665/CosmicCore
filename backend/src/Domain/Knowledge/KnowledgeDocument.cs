namespace CosmicCore.Domain.Knowledge;

public sealed class KnowledgeDocument
{
    private KnowledgeDocument() { }

    public KnowledgeDocument(Guid id, string title, string content, string? sourceUrl, DateTime createdAt)
    {
        Id = id;
        Rename(title);
        UpdateContent(content);
        SourceUrl = NormalizeSourceUrl(sourceUrl);
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string? SourceUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Update(string title, string content, string? sourceUrl, DateTime updatedAt)
    {
        Rename(title);
        UpdateContent(content);
        SourceUrl = NormalizeSourceUrl(sourceUrl);
        UpdatedAt = updatedAt;
    }

    private void Rename(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        var normalized = title.Trim();
        if (normalized.Length > 200) throw new ArgumentOutOfRangeException(nameof(title), "Title cannot exceed 200 characters.");
        Title = normalized;
    }

    private void UpdateContent(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        Content = content.Trim();
    }

    private static string? NormalizeSourceUrl(string? sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl)) return null;
        var normalized = sourceUrl.Trim();
        if (normalized.Length > 2048) throw new ArgumentOutOfRangeException(nameof(sourceUrl), "Source URL cannot exceed 2048 characters.");
        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Source URL must be an absolute HTTP or HTTPS URL.", nameof(sourceUrl));
        return uri.ToString();
    }
}

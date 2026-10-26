using CosmicCore.Application.Knowledge;
using CosmicCore.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuredConnectionString = builder.Configuration.GetConnectionString("CosmicCore");
SqliteConnectionStringBuilder connectionStringBuilder;
if (string.IsNullOrWhiteSpace(configuredConnectionString))
{
    var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    if (string.IsNullOrWhiteSpace(localApplicationData))
        throw new InvalidOperationException("Could not determine the local application data directory.");

    var dataDirectory = Path.Combine(localApplicationData, "CosmicCore");
    Directory.CreateDirectory(dataDirectory);
    connectionStringBuilder = new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(dataDirectory, "cosmiccore.db")
    };
}
else
{
    connectionStringBuilder = new SqliteConnectionStringBuilder(configuredConnectionString);
    if (connectionStringBuilder.DataSource != ":memory:" && !connectionStringBuilder.DataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
    {
        var databaseDirectory = Path.GetDirectoryName(Path.GetFullPath(connectionStringBuilder.DataSource));
        if (!string.IsNullOrEmpty(databaseDirectory))
            Directory.CreateDirectory(databaseDirectory);
    }
}

connectionStringBuilder.ForeignKeys = true;
connectionStringBuilder.DefaultTimeout = 5;

builder.Services.AddDbContext<CosmicCoreDbContext>(options => options.UseSqlite(connectionStringBuilder.ToString()));
builder.Services.AddScoped<IKnowledgeDocumentRepository, KnowledgeDocumentRepository>();
builder.Services.AddScoped<KnowledgeDocumentService>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

await using (var sqliteConnection = new SqliteConnection(connectionStringBuilder.ToString()))
{
    await sqliteConnection.OpenAsync();
    await using var pragma = sqliteConnection.CreateCommand();
    pragma.CommandText = "PRAGMA journal_mode=WAL;";
    await pragma.ExecuteScalarAsync();
}

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CosmicCoreDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("HealthCheck");

var documents = app.MapGroup("/api/knowledge-documents").WithTags("Knowledge documents");

documents.MapGet("/", async (KnowledgeDocumentService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.ListAsync(cancellationToken)));

documents.MapGet("/{id:guid}", async (Guid id, KnowledgeDocumentService service, CancellationToken cancellationToken) =>
    await service.FindAsync(id, cancellationToken) is { } document ? Results.Ok(document) : Results.NotFound());

documents.MapPost("/", async (KnowledgeDocumentRequest request, KnowledgeDocumentService service, CancellationToken cancellationToken) =>
{
    try
    {
        var document = await service.CreateAsync(request.Title, request.Content, request.SourceUrl, cancellationToken);
        return Results.Created($"/api/knowledge-documents/{document.Id}", document);
    }
    catch (ArgumentException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["request"] = [exception.Message] });
    }
});

documents.MapPut("/{id:guid}", async (Guid id, KnowledgeDocumentRequest request, KnowledgeDocumentService service, CancellationToken cancellationToken) =>
{
    try
    {
        var document = await service.UpdateAsync(id, request.Title, request.Content, request.SourceUrl, cancellationToken);
        return document is null ? Results.NotFound() : Results.Ok(document);
    }
    catch (ArgumentException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["request"] = [exception.Message] });
    }
});

documents.MapDelete("/{id:guid}", async (Guid id, KnowledgeDocumentService service, CancellationToken cancellationToken) =>
    await service.DeleteAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound());

app.Run();

public sealed record KnowledgeDocumentRequest(string Title, string Content, string? SourceUrl);

public partial class Program { }

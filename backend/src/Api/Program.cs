using CosmicCore.Application.Knowledge;
using CosmicCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CosmicCore")
    ?? string.Empty;
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Configure the CosmicCore connection string with ConnectionStrings__CosmicCore.");

builder.Services.AddDbContext<CosmicCoreDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IKnowledgeDocumentRepository, KnowledgeDocumentRepository>();
builder.Services.AddScoped<KnowledgeDocumentService>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

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

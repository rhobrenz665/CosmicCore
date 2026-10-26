using CosmicCore.Domain.Knowledge;
using Microsoft.EntityFrameworkCore;

namespace CosmicCore.Infrastructure.Persistence;

public sealed class CosmicCoreDbContext(DbContextOptions<CosmicCoreDbContext> options) : DbContext(options)
{
    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var document = modelBuilder.Entity<KnowledgeDocument>();
        document.ToTable("KnowledgeDocuments");
        document.HasKey(x => x.Id);
        document.Property(x => x.Id).ValueGeneratedNever();
        document.Property(x => x.Title).HasMaxLength(200).IsRequired();
        document.Property(x => x.Content).IsRequired();
        document.Property(x => x.SourceUrl).HasMaxLength(2048);
        document.Property(x => x.CreatedAt).IsRequired();
        document.Property(x => x.UpdatedAt).IsRequired();
        document.HasIndex(x => x.UpdatedAt);
    }
}

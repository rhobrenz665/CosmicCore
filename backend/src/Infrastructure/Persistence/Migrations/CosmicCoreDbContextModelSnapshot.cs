using CosmicCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CosmicCore.Infrastructure.Persistence.Migrations;

[DbContext(typeof(CosmicCoreDbContext))]
public sealed class CosmicCoreDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.13")
            .HasAnnotation("Relational:MaxIdentifierLength", 64);

        modelBuilder.Entity("CosmicCore.Domain.Knowledge.KnowledgeDocument", entity =>
        {
            entity.Property<Guid>("Id")
                .HasColumnType("TEXT")
                .ValueGeneratedNever();

            entity.Property<string>("Content")
                .IsRequired()
                .HasColumnType("TEXT");

            entity.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            entity.Property<string>("SourceUrl")
                .HasMaxLength(2048)
                .HasColumnType("TEXT");

            entity.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("TEXT");

            entity.Property<DateTime>("UpdatedAt")
                .HasColumnType("TEXT");

            entity.HasKey("Id");

            entity.HasIndex("UpdatedAt");

            entity.ToTable("KnowledgeDocuments");
        });
    }
}

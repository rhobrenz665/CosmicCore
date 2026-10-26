using CosmicCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CosmicCore.Infrastructure.Persistence.Migrations;

[DbContext(typeof(CosmicCoreDbContext))]
[Migration("20261026000000_InitialKnowledgeDocuments")]
public sealed class InitialKnowledgeDocuments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "KnowledgeDocuments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "TEXT", nullable: false),
                SourceUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_KnowledgeDocuments", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_KnowledgeDocuments_UpdatedAt",
            table: "KnowledgeDocuments",
            column: "UpdatedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "KnowledgeDocuments");
}

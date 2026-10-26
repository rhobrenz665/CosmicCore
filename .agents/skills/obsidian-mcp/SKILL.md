---
name: obsidian-mcp
description: Use when changing Obsidian vault storage, note synchronization/indexing, or MCP tools and transports.
---

# Obsidian and MCP

Follow the architecture in the repository root AGENTS.md. Markdown files in the configured Obsidian vault are canonical for note content. SQLite may hold a rebuildable index, but not a competing editable note body.

- Route REST and MCP operations through the same application use cases and validation. Protocol adapters must not contain domain rules or query storage directly.
- Restrict filesystem operations to the configured vault root; normalize paths and handle external Obsidian edits so the search index can be reconciled.
- Return compact, explicit MCP results. Validate all tool input. Keep destructive tools scoped and make confirmation behavior clear to the client.
- The initial MCP mode is local stdio. Keep transport-specific code isolated so HTTP can be added for remote use later.
- The built-in interviewer uses scoped application capabilities directly; external MCP clients use MCP. Do not make internal app features call the MCP server as an extra hop.
- Do not expose provider credentials or note/interview content in logs. Consult the relevant Notes KB page only when needed.

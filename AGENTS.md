# CosmicCore

CosmicCore is a local-first personal knowledge base and interview-preparation app.

## Architecture decisions

- Frontend: React + TypeScript. Backend: .NET 10 layered modular monolith.
- Obsidian Markdown files in the configured vault are the canonical note content. Do not persist a second editable copy of note bodies in SQLite.
- Use EF Core with SQLite for structured app data, including interview sessions and evaluations, plus a rebuildable note index when needed.
- REST and MCP are adapters over the same application use cases. MCP tools must not duplicate business rules or access the database/filesystem directly.
- The built-in interview feature calls scoped application capabilities directly. External MCP clients can use MCP CRUD tools. Do not route internal app calls through the MCP protocol.
- Deliver chat interviews before live voice interviews. Keep provider-specific AI integrations behind capability-specific backend interfaces; never call providers directly from the browser.
- Start local-first. Keep data files outside the repository. Before remote deployment, revisit authentication, authorization, secrets, transport security, backups, and database hosting.

## Working guidance

- Read only the project docs relevant to the change. Do not load the whole Notes KB for routine edits.
- Preserve existing API contracts unless the task calls for changing them; update the affected docs and clients together when they change.
- Use EF Core migrations for structured schema changes. SQLite has provider-specific query, concurrency, and migration limits; verify behavior rather than assuming provider portability.
- Keep HTTP/MCP endpoints thin. Put validation and application rules in shared application use cases.
- Keep note bodies, interview transcripts, and credentials out of logs. Treat vault paths and model/tool inputs as untrusted.
- Do not run unrelated builds, tests, or broad audits for a narrow change. Follow the requested verification scope and report what was run.

## Focused skills

- Use dotnet-ef for .NET application, EF Core, or SQLite schema work.
- Use react-ui for React routes, components, API clients, and client state.
- Use obsidian-mcp for vault integration or MCP tools/transports.
- Interview-specific guidance can be added when the chat and live workflows are implemented.

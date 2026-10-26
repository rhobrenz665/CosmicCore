---
name: react-ui
description: Use for React and TypeScript UI, routing, API client, and state changes in CosmicCore.
---

# React UI

Follow the architecture in the repository root AGENTS.md. Use the existing React, TypeScript, React Router, and TanStack Query stack unless the task requires a deliberate change.

- Keep HTTP calls in a small typed API layer and server state in TanStack Query; do not fetch directly inside page components or duplicate server data in Zustand.
- Keep components focused on presentation and user interaction. Handle loading, empty, error, and mutation states for changed flows.
- The browser calls the .NET REST API. It does not connect directly to SQLite, the Obsidian filesystem, MCP servers, or AI provider credentials.
- Keep interview chat and live voice as distinct UI modes over backend-owned session state.
- Consult the relevant Notes KB page only when needed; avoid loading unrelated project notes.

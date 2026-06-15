# Agent Guide

## Purpose

Seiyuu.moe is a web app for finding shared anime work between Japanese voice actors, comparing anime casts, and browsing seasonal role summaries.

## Read First

- `README.md` for the public project summary and basic setup commands.
- `docs/index.md` for the local documentation map.
- `docs/PROJECT.md` for product scope, features, and terminology.
- `docs/ARCHITECTURE.md` for repository layout, backend boundaries, and frontend structure.

## Repository Shape

- `App/` contains the Nuxt/Vue/Vuetify frontend.
- `API/` contains the .NET backend, background jobs, infrastructure, and backend tests.
- `Changelog.md` contains historical release notes.
- `docs/` contains durable repo knowledge for agents and maintainers.

## Local Commands

Frontend commands run from `App/`:

```bash
npm install
npm run dev
npm run lint
npm run generate
```

Backend commands run from the repository root unless noted:

```bash
dotnet restore API/SeiyuuMoe.API/SeiyuuMoe.API.csproj
dotnet build API/SeiyuuMoe.API/SeiyuuMoe.API.csproj --configuration Release
dotnet test API/SeiyuuMoe.Tests.Unit/SeiyuuMoe.Tests.Application.csproj
dotnet test API/SeiyuuMoe.Tests.Component/SeiyuuMoe.Tests.Component.csproj
```

There is currently no top-level solution file. Prefer targeted `dotnet` commands against the relevant project file. Some integration, database, and E2E tests may require external services or local configuration.

## Working Rules

- Keep changes scoped to the relevant side of the repo unless the behavior crosses the frontend/API boundary.
- In .NET code, use braces for `if`/`else` blocks even when the body is one line.
- Preserve the current backend layering: domain model and repository contracts in `SeiyuuMoe.Domain`, use-case/query logic in application projects, external services and persistence in infrastructure projects, and HTTP wiring in `SeiyuuMoe.API`.
- For frontend work, follow the existing Nuxt directory conventions and reuse shared components under `App/components/shared/` when practical.
- Do not commit generated Mac resource-fork files such as `._*`.

## Documentation Rule

The canonical docs for this repo are `README.md`, `docs/index.md`, `docs/PROJECT.md`, and `docs/ARCHITECTURE.md`. If a change touches user-facing behavior, public API behavior, architecture boundaries, verification commands, or anything already documented, update the relevant doc in the same change.

## Harness Posture

This repo currently needs a lightweight M1-style harness: a short `AGENTS.md` map plus a small `docs/` layer. Do not add execution-plan infrastructure, core beliefs, quality scorecards, or custom harness automation until repeated work in this repo shows that those would remove a real bottleneck.

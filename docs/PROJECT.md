# Project Overview

Seiyuu.moe helps users discover connections between Japanese voice actors and anime casts. The app combines a Nuxt frontend with a .NET backend that stores anime, seiyuu, character, role, and season data.

## User-Facing Scope

- Search for common anime works between up to six seiyuu.
- Compare anime casts and find shared seiyuu between selected titles.
- Browse seasonal summaries showing seiyuu with the most roles in a season.
- Filter results by main roles and TV series where supported.
- View results in compact, expanded, card, mixed-table, and timeline-style presentations depending on the feature.

## Key Terms

- Seiyuu: a Japanese voice actor.
- Anime: an anime title stored with metadata and associated character roles.
- Character: a role played by a seiyuu in an anime.
- Season: a year and season grouping used for seasonal summaries.
- Main role: a role flagged as primary enough to be included by the main-roles-only filters.
- MAL: MyAnimeList, used as an upstream data source through Tenrai.
- Tenrai: the unofficial MyAnimeList REST API used by the backend infrastructure and background jobs (drop-in successor to Jikan v4, available at https://api.tenrai.org/v1).

## External Services

- The frontend talks to the backend API configured in `App/nuxt.config.js`.
- The backend uses Tenrai for MAL data access.
- Infrastructure projects include AWS S3 and SQS integrations for background job workflows.
- Database access is implemented with Entity Framework Core and Pomelo MySQL.

## Current Harness State

The repo has enough moving parts to need a small documentation layer, but not enough harness signal to justify execution plans, scorecards, or custom guardrail automation. Prefer updating these docs as real workflows and recurring decisions become clearer.

# Architecture

Seiyuu.moe is split into a Nuxt frontend under `App/` and a .NET backend under `API/`.

## Frontend

`App/` is a Nuxt/Vue app using Vuetify, Vuex, Axios, Vuelidate, Font Awesome, and PWA support.

- `pages/` contains route-level views for home, seiyuu comparison, anime comparison, seasonal summaries, and about pages.
- `components/` contains feature components grouped by `anime`, `seiyuu`, `season`, `about`, and shared table/UI components.
- `store/` contains Vuex state for season summary filters.
- `plugins/` wires Vuetify, Vuelidate, Font Awesome, Google Analytics, and shared mixin helpers.
- `nuxt.config.js` defines universal rendering, global styles, modules, environment defaults, and webpack customizations.

## Backend Projects

`API/Directory.Build.props` sets the shared target framework to `net10.0`, and `API/Directory.Packages.props` centrally manages package versions.

- `SeiyuuMoe.Domain` contains entities, value objects, comparison entities, repository interfaces, schedule items, publisher contracts, and MAL update data contracts.
- `SeiyuuMoe.Application` contains query handlers, DTOs, validators, extension methods, and application-level Autofac registrations.
- `SeiyuuMoe.Infrastructure` contains external-service implementations for Jikan, AWS S3/SQS, configuration, and logging.
- `SeiyuuMoe.Infrastructure.Database` contains Entity Framework Core repositories, converters, configuration, and migrations.
- `SeiyuuMoe.API` contains ASP.NET Core controllers, startup, hosting, and dependency-injection wiring.
- `SeiyuuMoe.MalBackgroundJobs.Application` contains background-job handlers and helpers for MAL/Jikan update workflows.
- `SeiyuuMoe.MalBackgroundJobs.Lambda` exposes background jobs as AWS Lambda functions.
- `SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner` provides local runners for Lambda-style workflows.

## Backend Flow

HTTP requests enter through controllers in `SeiyuuMoe.API`. Startup configures ASP.NET Core, CORS, forwarded headers, Autofac, application services, infrastructure services, and database services. Application handlers perform query and comparison work using domain models and infrastructure/database services. Background jobs use a similar domain and infrastructure base to update MAL-derived data outside the request path.

## Tests

Backend tests are split by scope:

- `SeiyuuMoe.Tests.Unit` covers application handlers and extension logic.
- `SeiyuuMoe.Tests.Component` covers larger handler workflows, including background-job behavior with test doubles.
- `SeiyuuMoe.Tests.E2E` covers API controller flows.
- `Tests/SeiyuuMoe.Tests.Domain` covers domain comparison entities.
- `Tests/SeiyuuMoe.Tests.Infrastructure.Database` covers repository persistence behavior.
- `Tests/SeiyuuMoe.Tests.Integration` covers Jikan client integration.
- `SeiyuuMoe.Tests.Common` contains shared builders, stubs, helpers, and test data.

Run the narrowest relevant test project first. Database, integration, and E2E tests may require local configuration or external services.

## Boundaries To Preserve

- Keep HTTP concerns in `SeiyuuMoe.API`.
- Keep domain concepts and repository contracts in `SeiyuuMoe.Domain`.
- Keep external service and persistence details in infrastructure projects.
- Keep user-facing comparison and summary behavior aligned across frontend presentation components and backend query results.
- When adding background-job behavior, update both the application handler layer and the Lambda/local runner surface as needed.

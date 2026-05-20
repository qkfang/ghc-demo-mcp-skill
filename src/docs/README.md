# MuleSoft Legacy — Business Logic Reference

This folder captures the **business logic, behaviour, and integrations** of the
legacy MuleSoft application located in [`/mulesoft`](../../mulesoft). It is
intended to be the reference context for any follow-on migration work (for
example, the .NET re-implementation under `src/app` and the SQL deliverable
under `src/sql`).

The legacy application is a small **"Book My Show"** style movie ticketing API,
built on Mule Runtime 4.4.0 (Anypoint Studio) and a remote MySQL database. It
exposes two HTTP endpoints — one to list available movies and one to book
tickets — backed by a handful of DataWeave transformations, one validation
rule, and a CRUD sequence against two MySQL tables.

## Document index

| # | Document | Purpose |
|---|----------|---------|
| 1 | [`01-overview.md`](./01-overview.md) | System overview, architecture, runtime, source-file map |
| 2 | [`02-api-endpoints.md`](./02-api-endpoints.md) | HTTP endpoints, request/response shapes, status codes |
| 3 | [`03-business-rules.md`](./03-business-rules.md) | Pricing tiers, availability validation, booking flow |
| 4 | [`04-data-model.md`](./04-data-model.md) | MySQL tables, columns, inferred schema |
| 5 | [`05-integrations-and-config.md`](./05-integrations-and-config.md) | External systems, connectors, configuration properties |
| 6 | [`06-transformations.md`](./06-transformations.md) | DataWeave transformations and payload mapping |
| 7 | [`07-error-handling.md`](./07-error-handling.md) | APIKit + flow-level error handlers and HTTP status mapping |
| 8 | [`08-migration-notes.md`](./08-migration-notes.md) | Migration guidance, gaps, assumptions, and open questions |

## Scope and sources

All facts in these documents are derived from the files under
[`/mulesoft`](../../mulesoft), primarily:

- `mulesoft/src/main/mule/interface.xml` — APIKit listener, router, global error handler.
- `mulesoft/src/main/mule/implementation.xml` — `GetMovies` and `BookTickets` flows.
- `mulesoft/src/main/mule/global.xml` — HTTP listener and database connector configuration.
- `mulesoft/src/main/resources/config.yaml` — runtime configuration values.
- `mulesoft/src/main/resources/application-types.xml` — auto-generated payload/attribute types.
- `mulesoft/src/main/resources/order_details.json`, `sample.json` — DataWeave example payloads.
- `mulesoft/pom.xml`, `mulesoft/mule-artifact.json` — runtime version and dependencies.

The RAML contract itself is **not** present in the repository — it is pulled from
Anypoint Exchange as a Maven dependency
(`dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml:zip`). Behaviour related
to the contract has been inferred from the APIKit-generated flow names, the
auto-generated type definitions in `application-types.xml`, and the example
payloads. See [`08-migration-notes.md`](./08-migration-notes.md) for open
questions caused by this gap.

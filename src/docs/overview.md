# Overview — Book My Show (Legacy MuleSoft Application)

## Purpose

The legacy MuleSoft application under [`mulesoft/`](../../mulesoft) implements a small
movie‑ticket booking API called **Book My Show**. It exposes an HTTP API (defined by
a RAML contract published on Anypoint Exchange) that allows clients to:

1. List movies that currently have seats available.
2. Book one or more tickets for a given movie, with tiered pricing and automatic
   inventory updates.

The implementation is a classic 3‑layer MuleSoft API:

- **Experience / Interface layer** — APIkit router that dispatches incoming HTTP
  requests to the implementation flows (`mulesoft/src/main/mule/interface.xml`).
- **Process / Implementation layer** — Business flows (`GetMovies`, `BookTickets`)
  that talk to the database and apply business rules
  (`mulesoft/src/main/mule/implementation.xml`).
- **System layer** — Global connectors for HTTP listener and the MySQL database
  (`mulesoft/src/main/mule/global.xml`), parameterised via
  `mulesoft/src/main/resources/config.yaml`.

## Runtime topology

```mermaid
flowchart LR
    Client[HTTP Client]
    subgraph Mule["Mule Runtime (port 8081)"]
        direction TB
        Listener[/"HTTP Listener<br/>/api/*"/]
        Router["APIkit Router<br/>(movie.raml)"]
        GetMovies[["Flow: GetMovies"]]
        BookTickets[["Flow: BookTickets"]]
        Console[/"HTTP Listener<br/>/console/*"/]
    end
    DB[(MySQL<br/>remotemysql.com)]

    Client -->|HTTP| Listener --> Router
    Router -->|GET /movies| GetMovies
    Router -->|POST /movies/{m_id}| BookTickets
    Client -->|HTTP| Console
    GetMovies -->|SELECT| DB
    BookTickets -->|SELECT/INSERT/UPDATE| DB
```

## Key facts at a glance

| Aspect                | Value                                                                |
|-----------------------|----------------------------------------------------------------------|
| HTTP listener         | `0.0.0.0:8081` (`http.port` in `config.yaml`)                        |
| API base path         | `/api/*`                                                             |
| API console path      | `/console/*`                                                         |
| API contract          | RAML `dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml`         |
| Persistence           | MySQL (`remotemysql.com:3306`, database `Jk8nbsjqRg`)                |
| Tables                | `movie_table`, `order_table`                                         |
| Business rules        | Inventory check, tiered pricing (≤5/≤10/>10 tickets)                 |
| Error handling        | APIkit standard error mapping + custom validation error response     |

## Intended migration target

The contents of this folder are the source-of-truth artifacts for migrating the
application to a modern **.NET** stack. Each document below captures one aspect
of the legacy system in a form that is implementation‑agnostic so the new .NET
service can re‑implement the same behaviour with confidence:

- [`api-endpoints.md`](api-endpoints.md) — HTTP API surface and payloads.
- [`business-logic.md`](business-logic.md) — Flow‑by‑flow business rules.
- [`data-model.md`](data-model.md) — Database entities and relationships.
- [`integrations.md`](integrations.md) — External systems and connectors.
- [`dataweave-transformations.md`](dataweave-transformations.md) — DataWeave
  scripts behaviour.

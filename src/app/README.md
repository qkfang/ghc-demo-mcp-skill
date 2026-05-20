# Movie API Azure Functions (.NET 10)

This folder contains the .NET 10 Azure Functions migration of the legacy MuleSoft movie APIs.

## Build and test

```bash
dotnet build src/app/MovieApi.slnx
dotnet test src/app/MovieApi.slnx
```

## HTTP APIs

All routes use the Azure Functions default `/api` prefix.

- `GET /api/movies`
  - Returns movies with `m_available > 0`.
- `POST /api/movies/{m_id}?no_tickets={count}`
  - Applies ticket pricing tiers from the legacy flow:
    - `<= 5` tickets: `100` each
    - `<= 10` tickets: `90` each
    - `> 10` tickets: `80` each
  - Returns the created order payload on success.
  - Returns `{ "message": "Bad request" }` for invalid request data.
  - Returns `{ "message": "Resource not found" }` for unknown movie IDs.
  - Returns legacy-style insufficient-ticket error text when demand exceeds availability.

## Notes

- Current persistence is an in-memory repository to keep interfaces ready for upcoming database integration work.
- Source migration references:
  - `mulesoft/src/main/mule/interface.xml`
  - `mulesoft/src/main/mule/implementation.xml`

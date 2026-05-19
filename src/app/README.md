# .NET 10 Azure Function App (MuleSoft API Migration)

## Prerequisites
- .NET SDK 10
- Azure Functions Core Tools v4 (`func`)

## Build
```bash
dotnet build /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/app/MovieApi.slnx
```

## Run locally
```bash
cd /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/app/Movie.Api.Functions
func start
```

## Test
```bash
dotnet test /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/app/MovieApi.slnx
```

## Implemented endpoints
- `GET /api/movies`
- `POST /api/movies/{m_id}?no_tickets={count}`

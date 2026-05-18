# .NET 10 Azure Functions App

This folder contains the migrated MuleSoft APIs implemented as an Azure Functions app using .NET 10 isolated worker.

## Project layout

- `App.Functions/` - Azure Functions HTTP API implementation
- `App.Functions.Tests/` - unit + integration-style tests for mappings and HTTP functions
- `App.slnx` - solution file

## Endpoints

- `GET /api/movies`
- `POST /api/movies/{m_id}?no_tickets={count}`

## Local run

```bash
cd src/app/App.Functions
dotnet restore
dotnet build
func start
```

## Test

```bash
cd src/app
dotnet test App.slnx
```

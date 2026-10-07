# VideoGateway (ASP.NET Core 8)

A C# Web API that sits in front of the existing Flask translation pipeline.
The Python code is unchanged; the gateway adds an asynchronous job layer.

```
Browser ──> ASP.NET Core gateway (:5100) ──HttpClient──> Flask pipeline (:5000)
              │  upload validation
              │  job queue (Channel<T>) + BackgroundService worker
              └─ job store (IJobStore) + status polling + download
```

## Run

1. Start the Flask app from the repo root: `python server.py`
2. In another terminal: `cd gateway && dotnet run --urls http://localhost:5100`
3. Open http://localhost:5100

No FFmpeg/ImageMagick handy? `python tools/mock_flask.py` is a stand-in with the same routes.

## API

| Method | Route | Purpose |
|---|---|---|
| POST | `/api/jobs` | multipart `video` + `language`; returns 202 with a job |
| GET | `/api/jobs/{id}` | poll status: Pending, Processing, Done, Failed |
| GET | `/api/jobs` | job history |
| GET | `/api/jobs/{id}/download` | translated video (409 until Done) |
| GET | `/api/languages` | proxied from Flask |

## Design notes

- `IJobStore` is a repository interface; `InMemoryJobStore` is the current implementation.
- `TranslationWorker` is a `BackgroundService` reading from a `Channel<Guid>`, so uploads return immediately.
- `FlaskClient` is a typed `HttpClient` registered through `IHttpClientFactory`.
- Config lives in `appsettings.json` (Flask URL, timeout, upload limits).

## Next steps (not yet implemented)

- Persist jobs with EF Core + SQLite/SQL Server (swap in a new `IJobStore`)
- JWT authentication so users only see their own jobs
- Swagger/OpenAPI docs, unit tests with xUnit

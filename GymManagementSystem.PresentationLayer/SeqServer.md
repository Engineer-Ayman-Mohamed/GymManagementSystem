# Seq Setup Guide
## Running Seq (Docker)
```bash
docker run -d --name seq `
  -e ACCEPT_EULA=Y `
  -e SEQ_FIRSTRUN_NOAUTHENTICATION=true `
  -p 5341:80 `
  datalust/seq
Accessing the Dashboard
Open http://localhost:5341 — no login required.
Verifying Logs Are Arriving
Open the Seq dashboard and run a query:
SourceContext = "Serilog.AspNetCore.RequestLoggingMiddleware"
This shows all HTTP requests your app handles.
Useful Queries
Query	What it shows
Level = "Error"	All errors
SourceContext = "GymManagementSystem.BusinessLayer.Services.CleanUpDeletedRowsServices"	Cleanup job logs
@Exception is not null	All exceptions with stack traces
Method = "GET" and StatusCode >= 400	Failed requests
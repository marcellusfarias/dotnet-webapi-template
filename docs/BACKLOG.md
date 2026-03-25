## Backlog

### Observability

* Define the log shipping strategy — evaluate a sidecar collector (e.g., Promtail, Fluentd) vs. the Docker Loki log driver

### API

* Configure CORS policy
* Make request buffering opt-in per endpoint instead of globally enabled for all requests

### Authentication

* Implement refresh token support
* Increase maximum password length from 32 to at least 64 characters (NIST SP 800-63B recommendation)

### Domain

* Enforce the `EmailAddress` value object on the `User` entity instead of using a plain `string`
* Implement a `SaveChanges` interceptor to automatically populate `BaseAuditableEntity` fields (`CreatedAt`, `LastModifiedAt`, etc.)

### Infrastructure

* Avoid swallowing the original exception in repository methods — distinguish between known DB errors (e.g., unique constraint violation) and unexpected failures instead of wrapping all as `DatabaseException`

### Application

* Introduce `ICurrentUserService` — a scoped service that exposes the authenticated user's ID and claims from the JWT, so controllers and services don't need to resolve identity manually

### Code Quality

* Fix the `BuildServiceProvider()` call in `Program.cs` — it creates a second DI container and causes singletons to be instantiated twice
* Move `SanitizeForLog` out of individual controllers and into `ApiControllerBase`

### Template

* Convert the project into a proper `dotnet new` template so the solution name, namespaces and project names are replaced automatically on creation

### AI

Create folder structure for AI agents and implement a simple agent that can perform a task (e.g., fetch data from an API, process it, and return results)


### Future or nice-to-have

* Output caching with Redis
* Container resource limits
* Run containers as non-root users
* Change the logic of deploy. Instead of SSH into the VPS with Github actions, we should make the VPS pull the changes, so we can restrict IPs that can actually SSH into VPS by using Tailgate
* Observability:
  * Set up an observability stack covering metrics, logs, and traces (Prometheus, Grafana, Loki or equivalent)
  * Instrument the application with OpenTelemetry
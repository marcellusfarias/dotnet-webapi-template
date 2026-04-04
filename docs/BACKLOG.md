## Backlog

### API

* Configure CORS policy
* Make request buffering opt-in per endpoint instead of globally enabled for all requests

### Authentication

* Implement refresh token support

### Infrastructure

* Avoid swallowing the original exception in repository methods — distinguish between known DB errors (e.g., unique constraint violation) and unexpected failures instead of wrapping all as `DatabaseException`

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
  * Add support for traces and metrics
  * Configure alerts
  * Instrument the application with OpenTelemetry
  * We currently use Seq. That means it's a push strategy rather than pull when compared with Loki/Graphana. We may eventually lose some logs if Seq is down, which is unlikely. If we want to be really thourough, we can install Serilog, as it buffers the logs into a local file while Seq is down. 
  * Create init only script for Seq in case we ever delete the volume we don't need to configure it all again
* Implement a `SaveChanges` interceptor to automatically populate `BaseAuditableEntity` fields (`CreatedAt`, `LastModifiedAt`, etc.)

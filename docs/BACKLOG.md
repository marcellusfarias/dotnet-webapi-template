## Backlog

### API

* Configure CORS policy

### Template

* Convert the project into a proper `dotnet new` template so the solution name, namespaces and project names are replaced automatically on creation

### UI

Create basic setup of Angular project

### AI

Create folder structure for Skills


### Future or nice-to-have

#### Auth

* Refresh token — token families: instead of revoking all user tokens on theft detection, track a `FamilyId` per login session and revoke only that family. Leaves other devices unaffected. Also enables a "max concurrent sessions" limit.

#### Application layer

* Refactor exception handling so logging is done by the one that is handling the exception, instead of the one that is throwing it
* Remove FluentValidation from the API project and use DataAnnotations for validating instead
* Add to README how to install seqcli

#### Infrastructure layer

* Container resource limits
* Run containers as non-root users
* Change the logic of deploy. Instead of SSH into the VPS with Github actions, we should make the VPS pull the changes, so we can restrict IPs that can actually SSH into VPS by using Tailgate
* Implement a `SaveChanges` interceptor to automatically populate `BaseAuditableEntity` fields (`CreatedAt`, `LastModifiedAt`, etc.)

#### Observability

* Observability:
  * Add support for traces and metrics
  * Configure alerts
  * Instrument the application with OpenTelemetry
  * We currently use Seq. That means it's a push strategy rather than pull when compared with Loki/Graphana. We may eventually lose some logs if Seq is down, which is unlikely. If we want to be really thourough, we can install Serilog, as it buffers the logs into a local file while Seq is down. 
  * Create init only script for Seq in case we ever delete the volume we don't need to configure it all again

#### Others

* Output caching with Redis

## Backlog

### Infrastructure

* Configure CertBot in VPS
* Configure Docker Swarm in VPS
* Create docker secrets in VPS
  * Postgresql secrets
  * PGAdmin secrets
  * JWT key
* Create overlay network
* Setup resource limits for containers
* Setup replicas and health checks (automatic restarting) for containers
* Use rollout deployingment with update_config
* Setup monitoring (grafana, etc)
* Run containers as non-root users
* Backup volumes

### Before releasing

This list is orderned by priority.

#### Security

* Security issues:
    * Log injection/Remove sensitive information from logging (check logs in release)
    * Add Lockout (already has rate limiting)
    * MFA

#### Docker & Hosting

* Create docker secret for Admin Credentials (application + postgres). .Net [Host](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication?view=aspnetcore-7.0).
* Research best way to configure which environment is running. Interesting [link](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0#determining-the-environment-at-runtime). Another [link](https://stackoverflow.com/questions/32548948/how-to-get-the-development-staging-production-hosting-environment-in-configurese).

Finish implementing ValueObjects

Finish compatibily between docker swarm and fargate. Check secrets, HTTPS certs

### After releasing 

#### API Documentation

Add deeper level of details in the API documentation. Add sections like
* Authentication
* Rate Limits

Read articles about how to create a great API documentation. [Example](https://swagger.io/blog/api-documentation/best-practices-in-api-documentation/).

#### Caching

* Output caching (Redis).

#### Configuration

Fix JwtOptionsSetup

#### Logging

Create logging service that handles the GUID automatically. Should be simple and have transient lifetime. 

#### Github Actions

* Configure Github actions pipeline.

#### ASP.NET

* Health checks.
* Minimal APIs

#### Testing
* Research how to simulate a DatabaseException on IntegrationTests.
* Research how to simulate a cancellation on IntegrationTests.
* Research how to test RateLimiting.

#### Other topics
* Benchmarks & Performance Tests.
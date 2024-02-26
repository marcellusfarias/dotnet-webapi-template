
## Backlog

### Before releasing

This list is orderned by priority.

#### Security

* Security issues":
    * Log injection/Remove sensitive information from logging (check logs in release)
    * Add Lockout (already has rate limiting)
    * MFA

#### Docker & Hosting

* Create docker secret for Admin Credentials (application + postgres). .Net [Host](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication?view=aspnetcore-7.0).
* Research best way to configure which environment is running. Interesting [link](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0#determining-the-environment-at-runtime). Another [link](https://stackoverflow.com/questions/32548948/how-to-get-the-development-staging-production-hosting-environment-in-configurese).

Upgrade to .Net 8 

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

## Backlog

### Infrastructure

* Configure CertBot in VPS
* Setup resource limits for containers
* Setup replicas and health checks (automatic restarting) for containers
* Use rollout deployment with update_config
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

Finish implementing ValueObjects

### After releasing 

#### API Documentation

Add deeper level of details in the API documentation. Add sections like
* Authentication
* Rate Limits

#### Caching

* Output caching (Redis).

#### Configuration

Fix JwtOptionsSetup

#### Logging

Create logging service that handles the GUID automatically. Should be simple and have transient lifetime. 

#### Testing
* Create e2e test for RateLimiting.

#### Other topics
* Benchmarks & Performance Tests.
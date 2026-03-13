## Backlog

### Infrastructure

* Roll out new versions incrementally across replicas
* Halt the rollout and remove unhealthy containers if a health check fails during deployment
* Fail the pipeline if any service is not healthy after deployment

### Observability

* Set up an observability stack covering metrics, logs, and traces (Prometheus, Grafana, Loki or equivalent)
* Instrument the application with OpenTelemetry
* Define the log shipping strategy — evaluate a sidecar collector (e.g., Promtail, Fluentd) vs. the Docker Loki log driver
* Introduce a correlation ID middleware to generate and propagate `X-Correlation-ID` across requests

### API

* Implement a `/health` endpoint
* Configure CORS policy

### Authentication

* Implement refresh token support

### AI

Create folder structure for AI agents and implement a simple agent that can perform a task (e.g., fetch data from an API, process it, and return results)


### Future

* Output caching with Redis
* Container resource limits
* Run containers as non-root users

# SPHERITHM Deployment Guide

**Document ID:** DOC-010  
**Status:** Draft  
**Version:** 0.1  
**Last Updated:** 18 November 2026

---

## 1. Purpose

This document defines the planned deployment approach for SPHERITHM, a secure scientific data platform for ingesting, validating, preserving, processing, and tracing scientific datasets.

This guide describes the intended deployment architecture and operational procedures. It does not claim that deployment infrastructure or application services have already been implemented.

## 2. Deployment Status

The project is currently in its documentation and architecture definition stage.

The following are planned components, not yet confirmed as implemented:

- ASP.NET Core API using .NET 10
- React and TypeScript frontend
- MySQL relational database
- S3-compatible object storage using MinIO
- Python processing worker using Astropy
- Docker Compose development environment
- Kubernetes deployment baseline
- OpenTelemetry-based observability

Deployment commands in this document are reference commands. Execute them only after the corresponding configuration files and application components have been implemented.

## 3. Deployment Objectives

The deployment design must support:

1. Secure ingestion of scientific files.
2. Preservation of original uploaded files.
3. Controlled processing through isolated workers.
4. Traceable processing results and provenance.
5. Authentication and role-based authorization.
6. Separation of application, database, and object-storage responsibilities.
7. Repeatable development and deployment environments.
8. Monitoring, backup, and recovery.

## 4. Planned Deployment Architecture

The planned logical deployment consists of:

| Component | Responsibility |
|---|---|
| React frontend | User interface |
| ASP.NET Core API | Authentication, authorization, ingestion, orchestration |
| MySQL | Users, metadata, jobs, audit records |
| MinIO | Original files and processing artifacts |
| Python worker | FITS validation and scientific processing |
| OpenTelemetry | Logs, metrics, and traces |

### 4.1 Request Flow

1. A user accesses the frontend.
2. The frontend communicates with the API over HTTPS.
3. The API authenticates the user and checks permissions.
4. The API validates the upload request.
5. The original file is stored in object storage.
6. Metadata and job information are recorded in MySQL.
7. A processing worker handles the approved job.
8. Processing results and provenance are recorded.
9. Authorized users retrieve results through the API.

### 4.2 Trust Boundaries

The deployment must maintain separate security boundaries between:

- External users and the frontend.
- Frontend and backend API.
- Backend API and persistent storage.
- Backend API and processing workers.
- Processing workers and uploaded scientific files.

Uploaded files must be treated as untrusted input.

## 5. Repository Deployment Structure

The intended repository structure includes:

```text
SPHERITHM/
├── src/
│   ├── backend/
│   ├── frontend/
│   └── worker/
├── tests/
├── docs/
├── docker/
├── deployment/
├── docker-compose.yml
├── .env.example
└── .dockerignore
```

## 6. Environment Strategy

SPHERITHM is expected to use separate configuration for:

- Local development
- Automated testing
- Staging
- Production

Environment-specific configuration must not be hardcoded into application source code.

Production secrets must not be committed to source control.

### 6.1 Configuration Categories

| Category | Examples |
|---|---|
| API | Environment, allowed origins, public URL |
| Database | Connection string |
| Object storage | Endpoint, bucket, access credentials |
| Authentication | Signing key, token configuration |
| Worker | Processing limits, timeout settings |
| Observability | Exporter endpoint, log level |

## 7. Environment Variables and Secrets

A future `.env.example` file should document required configuration keys without containing real credentials.

Example variable names:

```text
ASPNETCORE_ENVIRONMENT=
ConnectionStrings__DefaultConnection=
ObjectStorage__Endpoint=
ObjectStorage__Bucket=
ObjectStorage__AccessKey=
ObjectStorage__SecretKey=
Jwt__Issuer=
Jwt__Audience=
Jwt__SigningKey=
Worker__TimeoutSeconds=
OTEL_EXPORTER_OTLP_ENDPOINT=
```

These are illustrative configuration names. They must be aligned with the actual application configuration during implementation.

Security requirements:

- Never commit production secrets.
- Do not use example credentials in production.
- Restrict access to secret-management systems.
- Rotate credentials when exposure is suspected.
- Use separate credentials for separate environments.
- Avoid printing secrets in application logs.

## 8. Containerization Strategy

Docker is planned as the standard local deployment mechanism.

Each service should have a dedicated container image where appropriate.

Planned services:

- API
- Frontend
- MySQL
- MinIO
- Python worker

Container images should:

- Use supported base images.
- Prefer pinned versions.
- Run as non-root users where practical.
- Avoid unnecessary packages.
- Expose only required ports.
- Use health checks where supported.
- Avoid embedding credentials.
- Be scanned before release.

Container definitions have not yet been implemented.

## 9. Docker Compose Deployment

Docker Compose is planned for local development and integration testing.

A future Compose configuration should define:

- Service dependencies.
- Persistent database storage.
- Persistent object storage.
- Environment configuration.
- Internal service networking.
- Health checks.
- Port mappings.
- Restart policies where appropriate.

Database and object-storage administration interfaces should not be publicly exposed by default.

### 9.1 Planned Commands

The following commands become executable only after a valid Compose configuration exists in the repository:

```powershell
docker compose config
docker compose build
docker compose up -d
docker compose ps
docker compose logs --tail 100
docker compose down
```

Do not run these commands as deployment verification until `docker-compose.yml` has been created and reviewed.

## 10. Database Deployment

MySQL is planned as the relational persistence layer.

It is expected to store:

- User and role information.
- Dataset metadata.
- Upload records.
- Processing job information.
- Processing status.
- Provenance metadata.
- Audit events.

Deployment requirements:

- Use persistent storage.
- Restrict network access.
- Use a dedicated application database account.
- Avoid using database root credentials in the application.
- Apply schema changes through controlled migrations.
- Back up data before production migrations.
- Test migration and rollback procedures.

Database migrations must be implemented and tested before being treated as operational procedures.

## 11. Object Storage Deployment

MinIO is planned as the S3-compatible object-storage service.

It is expected to preserve:

- Original uploaded FITS files.
- Validated dataset objects.
- Processing outputs.
- Supporting artifacts where required.

Storage requirements:

- Keep original uploaded files unchanged.
- Use controlled bucket access.
- Restrict application credentials to required operations.
- Enable encryption at rest where supported by the deployment environment.
- Use encrypted transport.
- Define retention and backup policies.
- Record object identifiers and checksums in application metadata.

The application must not trust a client-provided filename as a storage path.

## 12. Processing Worker Deployment

The Python worker is planned to perform scientific file validation and processing using Astropy.

The worker must be isolated from the public-facing application.

Required controls include:

- Non-root execution.
- Restricted filesystem access.
- CPU and memory limits.
- Processing timeouts.
- Controlled temporary storage.
- Restricted network access.
- Explicit input and output locations.
- Structured error reporting.

A worker must not be allowed to execute arbitrary commands supplied through uploaded files or user input.

## 13. Network and Access Control

The deployment should follow least-privilege network access.

Expected access rules:

| Component | Expected access |
|---|---|
| Public users | Frontend over HTTPS |
| Frontend | API over HTTPS |
| API | MySQL and object storage |
| Worker | Approved processing inputs and outputs |
| Database | Authorized application services |
| Object storage | Authorized application and worker services |

Administrative interfaces must be restricted to authorized operators.

Production services must not expose internal database or storage ports to the public internet.

## 14. TLS and Transport Security

Production traffic must use HTTPS.

Requirements:

- Use certificates from a trusted certificate authority.
- Redirect HTTP requests to HTTPS where applicable.
- Protect internal service communication according to deployment risk.
- Renew certificates before expiration.
- Disable obsolete TLS versions and insecure cipher configurations.
- Never transmit authentication secrets over unencrypted connections.

Local development certificates must not be treated as production certificates.

## 15. Application Security

The deployed application must enforce:

- JWT-based authentication.
- Role-based authorization.
- Resource ownership checks.
- Tenant isolation where multi-tenancy is implemented.
- Upload size limits.
- Input validation.
- Rate limiting.
- Secure error handling.
- Audit logging for security-relevant actions.

Authorization must be enforced by the backend API, not only by frontend controls.

## 16. Health Checks and Observability

The deployment should provide health and operational visibility.

Planned monitoring areas:

- API availability.
- Database connectivity.
- Object-storage availability.
- Worker availability.
- Processing job failures.
- Upload and processing duration.
- Resource consumption.
- Authentication and authorization failures.

OpenTelemetry is planned for application telemetry.

Health endpoints must not disclose secrets, internal configuration, or sensitive infrastructure details.

## 17. Logging and Audit

Application logs and audit records serve different purposes.

Operational logs should support troubleshooting.

Audit records should capture security-relevant activity, including:

- Authentication events.
- Authorization failures.
- Dataset uploads.
- Dataset access.
- Processing job creation.
- Processing completion or failure.
- Administrative actions.
- Configuration changes where applicable.

Logs must not contain passwords, access tokens, signing keys, or unnecessary sensitive scientific data.

## 18. Backup and Recovery

A production deployment must define backup and recovery procedures for both relational and object data.

Backup scope:

- MySQL database.
- Original object-storage files.
- Processing outputs where required.
- Deployment configuration.
- Required metadata and provenance.

Recovery planning must define:

- Backup frequency.
- Retention period.
- Backup encryption.
- Access restrictions.
- Recovery Point Objective (RPO).
- Recovery Time Objective (RTO).
- Restore verification.

A backup is not considered reliable until restoration has been tested.

## 19. Deployment Verification

Before a deployment is accepted, verify:

1. Required services start successfully.
2. The API health endpoint responds correctly.
3. Database connectivity works.
4. Object storage is accessible using application credentials.
5. Authentication works.
6. Role-based authorization is enforced.
7. Upload restrictions are enforced.
8. Original files are preserved.
9. Processing jobs complete or fail safely.
10. Audit records are generated.
11. Logs do not expose secrets.
12. Backup and restoration procedures are verified.

Verification results must be recorded as implementation evidence.

## 20. CI/CD and Release Management

The planned CI/CD pipeline should include:

- Source compilation.
- Unit tests.
- Integration tests.
- API tests.
- Worker tests.
- Dependency vulnerability checks.
- Static analysis.
- Container image scanning.
- Build artifact generation.
- Deployment approval.

Production deployment should use reviewed and versioned artifacts.

A release must be traceable to its source revision and test results.

## 21. Kubernetes Deployment Baseline

Kubernetes is a future deployment target, not a prerequisite for the initial local development environment.

A future baseline may include:

- Deployments for stateless services.
- Services for internal communication.
- ConfigMaps for non-secret configuration.
- Secrets or an external secret manager for credentials.
- Persistent volumes for stateful services where appropriate.
- Resource requests and limits.
- Readiness and liveness probes.
- Network policies.
- Ingress with TLS.
- Controlled rollout and rollback.

Stateful services require appropriate persistence, backup, and recovery planning. Kubernetes manifests must not be considered implemented until they exist and have been tested.

## 22. Operational Responsibilities

Operational procedures should define responsibility for:

- Application releases.
- Database maintenance.
- Object-storage maintenance.
- Secret rotation.
- Certificate renewal.
- Backup monitoring.
- Incident response.
- Vulnerability remediation.
- Access reviews.

Administrative access must be limited to authorized personnel.

## 23. Open Deployment Decisions

The following decisions remain open and must be resolved during implementation:

1. Final container base images.
2. Production hosting environment.
3. Production secret-management solution.
4. Object-storage backup strategy.
5. Database backup and recovery tooling.
6. Monitoring and telemetry backend.
7. Production ingress and TLS termination.
8. Kubernetes hosting and operational model.
9. RPO and RTO targets.
10. Production retention requirements.

## 24. Document Status and Implementation Boundary

This document defines the initial deployment design.

It does not demonstrate that:

- Docker images have been created.
- Docker Compose services have been configured.
- MySQL has been deployed.
- MinIO has been deployed.
- The Python worker has been implemented.
- Kubernetes manifests exist.
- A production environment is operational.

Each capability must be implemented, tested, documented, reviewed, and committed before being marked complete in the project tracker.

---

**End of DOC-010 — Deployment Guide**
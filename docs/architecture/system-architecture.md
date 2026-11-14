# SPHERITHM — System Architecture

**Document ID:** DOC-006
**Version:** 0.1
**Status:** Draft
**Owner:** Project Owner
**Last Updated:** 13-11-2026

---

## 1. Purpose

This document defines the logical and technical architecture of SPHERITHM, a security-first scientific data platform designed to support controlled ingestion, validation, preservation, processing, and traceability of scientific datasets.

It establishes the major system components, their responsibilities, communication patterns, data flow, trust boundaries, and deployment baseline.

This document describes the planned architecture. It does not claim that the components or security controls have already been implemented.

## 2. Architecture Goals

The architecture is designed to:

- Preserve original scientific files without modifying the uploaded source.
- Validate datasets before they enter processing workflows.
- Maintain integrity through cryptographic checksums.
- Separate API responsibilities from scientific processing workloads.
- Isolate processing workers from the public-facing application.
- Enforce authentication, authorization, and tenant-level data access.
- Record processing history, audit events, and data provenance.
- Support reproducible processing and controlled result access.
- Provide a deployment model suitable for local development and a small production-like environment.
- Keep the system modular, testable, observable, and maintainable.

## 3. Architectural Principles

1. **Security by design:** Security controls are considered at each system boundary.
2. **Least privilege:** Components and users receive only the permissions required for their responsibilities.
3. **Separation of concerns:** API, application logic, domain rules, infrastructure, and processing workloads remain distinct.
4. **Immutable source preservation:** Original uploaded datasets are preserved as received.
5. **Traceability:** Important actions and processing outputs are linked to identifiable source data and execution records.
6. **Defense in depth:** Multiple independent controls protect data and processing workflows.
7. **Explicit trust boundaries:** External inputs and inter-service communication are validated.
8. **Reproducibility:** Processing metadata, software versions, parameters, and output references are recorded.
9. **Fail safely:** Invalid input and failed processing must not silently produce trusted scientific results.
10. **Incremental delivery:** Architecture and implementation evolve through reviewed and documented changes.

## 4. System Context

SPHERITHM interacts with the following external actors and systems:

| Actor or System | Interaction |
|---|---|
| Platform Administrator | Manages platform configuration, users, and access policies |
| Data Curator | Uploads datasets and reviews validation results |
| Research Analyst | Requests processing and accesses authorized results |
| Auditor | Reviews audit records and provenance information |
| External Client | Communicates with the platform through authenticated APIs |
| Object Storage | Stores original datasets and generated artifacts |
| Scientific Processing Runtime | Processes validated datasets in an isolated worker environment |

External scientific datasets are treated as untrusted input until validation and integrity checks have completed.

## 5. High-Level Architecture

The planned architecture uses a layered API and a separately isolated scientific processing worker.

```text
+------------------------------------------------------+
|                    Platform Users                    |
| Administrator | Curator | Analyst | Auditor          |
+---------------------------+--------------------------+
                            |
                            | HTTPS
                            v
+------------------------------------------------------+
|             React / TypeScript Frontend              |
| Authentication UI | Dataset UI | Jobs | Results      |
+---------------------------+--------------------------+
                            |
                            | Authenticated REST API
                            v
+------------------------------------------------------+
|                ASP.NET Core Web API                  |
| Authentication | Authorization | Request Validation|
| Rate Limiting | API Contracts | Audit Integration    |
+---------------------------+--------------------------+
                            |
                            v
+------------------------------------------------------+
|                 Application Layer                    |
| Dataset Use Cases | Validation Workflow | Job Control|
| Provenance | Access Policies | Result Management     |
+---------------------------+--------------------------+
                            |
                            v
+------------------------------------------------------+
|                    Domain Layer                      |
| Dataset | Validation | Processing Job | Provenance   |
| User Access Rules | Domain Invariants                |
+---------------------------+--------------------------+
                            |
                            v
+------------------------------------------------------+
|                Infrastructure Layer                   |
| MySQL | Object Storage | Queue Adapter | Telemetry    |
+---------------------------+--------------------------+
                            |
             +--------------+--------------+
             |                             |
             v                             v
+---------------------------+  +-----------------------+
| MySQL Structured Storage  |  | S3-Compatible Storage |
| Users | Datasets | Jobs   |  | Original FITS Files   |
| Audit | Provenance        |  | Processing Artifacts  |
+---------------------------+  +-----------------------+
                                           |
                                           | Controlled job input
                                           v
                              +-------------------------+
                              | Isolated Python Worker  |
                              | FITS Validation         |
                              | Astropy Processing     |
                              | Output Generation      |
                              +------------+------------+
                                           |
                                           v
                              +-------------------------+
                              | Results and Provenance  |
                              +-------------------------+
```

## 6. Major Components and Responsibilities

### 6.1 Frontend

**Technology:** React and TypeScript.

Responsibilities:

- Provide user-facing platform workflows.
- Display datasets, validation results, processing jobs, and outputs.
- Submit authenticated API requests.
- Display authorization and processing status.
- Avoid storing secrets or enforcing security decisions exclusively in the browser.

The frontend is an untrusted client from the API security perspective.

### 6.2 ASP.NET Core Web API

**Technology:** .NET 10 and ASP.NET Core.

Responsibilities:

- Expose versioned REST API endpoints.
- Authenticate incoming requests.
- Enforce authorization policies.
- Validate request contracts and input constraints.
- Apply rate limits and upload restrictions.
- Coordinate application use cases.
- Return controlled responses without exposing internal secrets or stack traces.

### 6.3 Application Layer

Responsibilities:

- Coordinate dataset ingestion and validation workflows.
- Manage processing job creation and state transitions.
- Apply use-case-level authorization checks.
- Coordinate storage, processing, and provenance operations.
- Ensure workflows follow defined business rules.

### 6.4 Domain Layer

Responsibilities:

- Define core business entities and value objects.
- Enforce dataset and processing-job invariants.
- Define permitted state transitions.
- Represent validation outcomes and provenance relationships.
- Remain independent of database and web framework implementations.

### 6.5 Infrastructure Layer

Responsibilities:

- Implement persistence and external service integrations.
- Provide MySQL repositories.
- Provide object-storage access.
- Implement job dispatch and processing communication adapters.
- Integrate audit persistence and observability infrastructure.

### 6.6 Python Processing Worker

**Technology:** Python and Astropy.

Responsibilities:

- Consume authorized processing jobs.
- Read approved input objects from storage.
- Validate required scientific file structures.
- Execute supported scientific processing operations.
- Generate output artifacts and processing metadata.
- Report success, failure, and execution information.

The worker is isolated from direct public client access.

## 7. Data Storage Architecture

### 7.1 MySQL

MySQL is the planned structured database.

It stores:

- User and role references.
- Dataset metadata.
- Dataset ownership and tenant associations.
- Validation records.
- Processing job records and states.
- Processing configuration references.
- Provenance metadata.
- Audit event records or references.

Large scientific file contents are not stored as database binary payloads in the initial architecture.

### 7.2 S3-Compatible Object Storage

MinIO is the planned local S3-compatible object-storage implementation.

It stores:

- Original uploaded scientific files.
- Validated source-object references.
- Processing outputs.
- Derived scientific artifacts.
- Supporting manifests where appropriate.

Original source objects must be preserved separately from generated outputs. Object versioning, retention, and immutability settings will be evaluated before production use.

### 7.3 Data Integrity

The platform will calculate SHA-256 checksums for uploaded source objects.

The checksum and associated metadata will be recorded so later integrity checks can detect unexpected changes.

A checksum provides integrity verification; it does not independently prove the authenticity or scientific correctness of a dataset.

## 8. Dataset and Processing Data Flow

The intended workflow is:

1. An authenticated user submits a dataset through the frontend.
2. The API authenticates the request and checks authorization, ownership, and upload limits.
3. The application layer creates a dataset ingestion record.
4. The source file is stored in a controlled object-storage location.
5. The platform calculates and records a SHA-256 checksum.
6. The validation workflow checks file structure, required metadata, dimensions, and supported format constraints.
7. Validation results are recorded.
8. Only datasets that satisfy the required validation policy can be submitted for processing.
9. The application layer creates a processing job.
10. A controlled job-dispatch mechanism makes the job available to an isolated worker.
11. The worker retrieves authorized input data and performs the requested operation.
12. Outputs and execution metadata are stored.
13. The platform records provenance linking the source dataset, processing job, parameters, software version, and output artifacts.
14. Authorized users retrieve results and relevant provenance information through the API.

Failed validation or processing must produce an explicit failure state. Failed or incomplete outputs must not be presented as successfully verified scientific results.

## 9. Authentication and Authorization Architecture

The platform will use token-based authentication with JWT access tokens and a controlled refresh-token lifecycle.

Authorization will use role-based access control.

Initial platform roles:

| Role | Primary Responsibilities |
|---|---|
| Platform Administrator | Platform configuration and user administration |
| Data Curator | Dataset ingestion and validation review |
| Research Analyst | Authorized processing and result access |
| Auditor | Read-only audit and provenance review |

Authorization must be enforced by the API and application layer, not solely by frontend visibility.

Tenant isolation and dataset ownership checks must be applied to every operation that accesses protected resources.

## 10. Security Boundaries

The architecture identifies these principal trust boundaries:

| Boundary | Security Concern | Planned Controls |
|---|---|---|
| User to Frontend | Untrusted user input | Input handling and safe rendering |
| Frontend to API | Unauthorized or malformed requests | HTTPS, JWT validation, authorization, rate limits |
| API to Application | Invalid workflow operations | Contracts, validation, policy enforcement |
| Application to Database | Unauthorized or inconsistent data access | Least-privilege credentials, parameterized access, tenant checks |
| Application to Object Storage | Unauthorized object operations | Scoped credentials, controlled object references |
| Job Dispatch to Worker | Malicious or altered job input | Authenticated dispatch, job validation, restricted access |
| Worker Runtime | Malformed scientific files and unsafe processing | Isolation, resource limits, restricted filesystem and network |
| Worker to Result Storage | Unauthorized or incomplete output | Scoped write access, output validation, provenance records |

All external input, including scientific files and job parameters, must be treated as untrusted.

## 11. Processing Worker Isolation

The Python worker is a separate execution boundary because scientific file parsing and processing involve complex input formats and third-party libraries.

The planned baseline includes:

- Non-root execution.
- Dedicated worker identity and restricted credentials.
- CPU, memory, and execution-time limits.
- Restricted filesystem access.
- Restricted network access.
- Controlled input and output locations.
- No direct public exposure.
- Explicit job-state reporting.
- Dependency pinning and vulnerability review.
- Cleanup of temporary processing data.

The worker must not be trusted merely because the API created the job. It must validate job metadata and input references before processing.

## 12. Audit and Provenance

The platform will maintain records for security-relevant and scientific workflow events.

Audit events may include:

- Authentication and authorization outcomes.
- Dataset upload and validation.
- Dataset access and ownership changes.
- Processing job creation and state transitions.
- Processing failures and output publication.
- Administrative configuration changes.
- Provenance and artifact access.

Provenance records will link source datasets to processing executions and generated outputs.

Audit records must avoid storing passwords, access tokens, secret keys, or unnecessary sensitive content.

## 13. Observability

OpenTelemetry is the planned instrumentation approach.

The platform will support:

- Structured application logs.
- Request and processing traces.
- Service and worker metrics.
- Correlation identifiers across API and job workflows.
- Error and failure monitoring.
- Health and readiness checks.

Observability data must be designed to avoid leaking credentials, tokens, or sensitive dataset content.

## 14. Deployment Architecture

The initial deployment baseline is Docker Compose for local development and controlled demonstration environments.

Planned services include:

- Frontend.
- ASP.NET Core API.
- MySQL.
- MinIO object storage.
- Python processing worker.
- Supporting telemetry services where required.

A Kubernetes deployment baseline may be added for small-cluster demonstration and orchestration learning.

Kubernetes support does not imply production-scale high availability, multi-region disaster recovery, or automatic security certification.

## 15. Technology Stack

| Area | Planned Technology |
|---|---|
| Backend API | .NET 10 / ASP.NET Core |
| Application and Domain | C# |
| Frontend | React / TypeScript |
| Structured Database | MySQL |
| Object Storage | MinIO / S3-compatible API |
| Scientific Processing | Python / Astropy |
| Containerization | Docker |
| Local Orchestration | Docker Compose |
| Cluster Deployment Baseline | Kubernetes |
| Observability | OpenTelemetry |
| Backend Testing | xUnit |
| Processing Testing | pytest |
| CI/CD | GitHub Actions |
| Integrity | SHA-256 |

Technology versions and deployment choices will be confirmed in implementation and relevant ADRs.

## 16. Quality Attributes

The architecture targets the following quality attributes:

- **Security:** Explicit trust boundaries and least-privilege access.
- **Integrity:** Checksums and controlled source preservation.
- **Traceability:** Auditable dataset and processing history.
- **Reproducibility:** Recorded processing parameters and software versions.
- **Maintainability:** Layered architecture and modular components.
- **Testability:** Defined component boundaries and automated tests.
- **Observability:** Correlated logs, metrics, and traces.
- **Reliability:** Explicit workflow states and failure handling.
- **Portability:** Containerized services and S3-compatible object storage.

These are architectural objectives, not claims of measured performance or completed compliance.

## 17. Architecture Constraints and Scope

The initial architecture is intentionally limited to a manageable portfolio implementation.

Out of scope for the initial baseline:

- Commercial-scale distributed scientific computing.
- Multi-region high availability and disaster recovery.
- A complete astronomy discovery or visualization platform.
- AI/ML-based scientific discovery.
- Full enterprise identity federation.
- Production certification or guarantees of absolute security.
- Unrestricted execution of user-supplied code.

## 18. Open Architecture Decisions

The following decisions require explicit evaluation and documentation through ADRs:

- Processing job queue and dispatch mechanism.
- Refresh-token storage, rotation, and revocation design.
- Tenant isolation strategy.
- Object-storage retention and immutability configuration.
- Upload size and processing resource limits.
- FITS validation policy and supported file variants.
- Worker-to-storage and worker-to-job-status communication.
- Audit event retention and integrity protection.
- Production secret management.
- Kubernetes deployment scope.

Until these decisions are approved, the architecture describes intended boundaries and responsibilities rather than final implementation details.

## 19. Requirement Traceability

This architecture supports the requirements defined in DOC-004 and mapped in DOC-005.

The traceability matrix is the authoritative reference for mapping individual requirement IDs to components, planned implementation work, and verification methods.

Architecture changes that affect requirements must be reflected in the relevant requirements and traceability records.

## 20. Document Status

This document is an initial architecture baseline.

It is subject to review and revision as implementation details are confirmed. Any significant architectural change should be evaluated, documented, and linked to an Architecture Decision Record where appropriate.

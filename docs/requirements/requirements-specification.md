# SPHERITHM — Requirements Specification

| Document ID | DOC-004 |
|---|---|
| Version | 0.1 |
| Status | Draft |
| Created | 13 November 2026 |
| Product | SPHERITHM — Secure Scientific Data Platform |

## 1. Purpose

This document defines the functional, non-functional, security and operational requirements for SPHERITHM.

It establishes a testable baseline for system design, implementation, verification and requirements traceability.

The requirements describe intended system behaviour. They do not imply that the capabilities have already been implemented.

## 2. Product Context

SPHERITHM is an independent security-first scientific data platform designed to ingest, validate, preserve, process and track scientific datasets.

The initial scientific use case focuses on astronomical FITS data, including publicly available SPHEREx datasets.

SPHERITHM is an independent portfolio project and is not an official NASA, Caltech or IPAC product.

## 3. Requirement Conventions

### 3.1 Requirement identifiers

| Prefix | Category |
|---|---|
| FR | Functional Requirement |
| NFR | Non-Functional Requirement |
| SEC | Security Requirement |
| OPS | Operational Requirement |

Each requirement has a unique identifier that must remain stable when the requirement is revised.

### 3.2 Priority definitions

- **Must:** Required for the initial usable platform baseline.
- **Should:** Important capability that may follow the initial baseline.
- **Could:** Desirable capability that is not essential to the initial baseline.

### 3.3 Acceptance criteria

Acceptance criteria describe observable conditions that can be verified through testing, inspection or documented evidence.

A requirement must not be marked complete merely because its design or documentation exists. Implementation and verification evidence are required.

## 4. Functional Requirements

### 4.1 Identity and Access Management

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| FR-IAM-001 | The platform shall authenticate registered users before granting access to protected application functions. | Must | Unauthenticated requests to protected endpoints are rejected. Valid credentials establish an authenticated session. |
| FR-IAM-002 | The platform shall assign users role-based permissions. | Must | Users with different roles receive only the permissions assigned to their roles. |
| FR-IAM-003 | The platform shall support the initial roles Platform Administrator, Data Curator, Research Analyst and Auditor. | Must | Each role can be assigned and its permissions can be verified through authorization tests. |
| FR-IAM-004 | The platform shall allow authorized administrators to manage user access and role assignments. | Must | Authorized changes are persisted and unauthorized changes are rejected. |

### 4.2 Dataset Ingestion and Preservation

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| FR-DATA-001 | The platform shall allow authorized users to submit scientific dataset files for ingestion. | Must | An authorized user can submit a supported file and receive an ingestion identifier. |
| FR-DATA-002 | The platform shall enforce configurable upload size limits. | Must | Files exceeding the configured limit are rejected with a recorded reason. |
| FR-DATA-003 | The platform shall calculate a SHA-256 checksum for every successfully received original file. | Must | The calculated checksum is stored with the dataset record and can be independently verified. |
| FR-DATA-004 | The platform shall preserve the original uploaded file separately from processing outputs. | Must | Processing operations do not overwrite or modify the preserved original object. |
| FR-DATA-005 | The platform shall maintain a dataset record containing its identifier, original filename, size, checksum, owner and ingestion timestamp. | Must | The dataset record contains the required fields and can be retrieved by an authorized user. |
| FR-DATA-006 | The platform shall detect an existing file with the same SHA-256 checksum within the applicable ownership or workspace boundary. | Should | A repeated upload is identified as a checksum match without silently replacing the original dataset. |

### 4.3 FITS Validation and Metadata

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| FR-FITS-001 | The platform shall validate whether an uploaded file has a readable FITS structure before scientific processing. | Must | Valid test FITS files pass structural validation; malformed test files are rejected or marked invalid. |
| FR-FITS-002 | The platform shall inspect FITS headers and identify available HDUs. | Must | The validation output records the available HDU identifiers and relevant header information. |
| FR-FITS-003 | The platform shall extract available scientific metadata, including observation identifiers, observation timestamps and detector information where present. | Must | Extracted values are recorded when present, and missing values are reported as unavailable rather than invented. |
| FR-FITS-004 | The platform shall record FITS validation results and associated validation errors. | Must | Each validation attempt produces a retrievable result with its outcome and error details. |
| FR-FITS-005 | The platform shall preserve relevant FITS metadata without changing the original file. | Must | Extracted metadata is stored separately and the original file checksum remains unchanged. |

### 4.4 Processing and Results

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| FR-PROC-001 | The platform shall allow authorized users to request supported processing operations on validated datasets. | Must | A valid request creates a processing job linked to the relevant dataset. |
| FR-PROC-002 | The platform shall execute scientific processing through a separate Python-based worker component. | Must | Processing is performed by the worker component rather than directly inside the API request process. |
| FR-PROC-003 | The platform shall maintain processing job states, including queued, running, completed and failed. | Must | Job state transitions are recorded and can be retrieved through the platform. |
| FR-PROC-004 | The platform shall store processing outputs separately from original scientific data. | Must | A completed job produces separately identifiable output artifacts. |
| FR-PROC-005 | The platform shall record processing parameters, software or worker version, input dataset identity and execution outcome. | Must | A processing record contains sufficient information to identify the input and describe the execution. |
| FR-PROC-006 | The platform shall report processing failures without silently marking failed jobs as successful. | Must | A failed processing job has a failed state and a recorded failure reason. |

### 4.5 Dataset Access and Audit

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| FR-AUD-001 | The platform shall record security-relevant user and administrative actions. | Must | Selected actions produce audit records containing actor, action, timestamp and outcome. |
| FR-AUD-002 | The platform shall allow authorized users to retrieve dataset metadata and processing history. | Must | Authorized requests return the records associated with the requested dataset. |
| FR-AUD-003 | The platform shall restrict dataset and processing-result access according to user permissions and ownership or workspace boundaries. | Must | Cross-boundary access attempts are rejected and recorded where appropriate. |
| FR-AUD-004 | The platform shall provide authorized auditors with read-only access to relevant audit records. | Should | Auditor accounts can view permitted audit records but cannot modify them. |

### 4.6 Application Interface and API

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| FR-API-001 | The platform shall expose documented API endpoints for supported authentication, dataset, validation and processing operations. | Must | Implemented endpoints have documented request, response and error contracts. |
| FR-API-002 | The platform shall return consistent error responses for invalid requests and failed operations. | Must | API tests verify appropriate status codes and structured error responses. |
| FR-UI-001 | The platform shall provide a web interface for authorized users to access supported platform functions. | Must | An authenticated user can access the functions permitted by their role through the interface. |
| FR-UI-002 | The platform shall display dataset ingestion, validation and processing status. | Must | The interface displays the latest available status associated with each relevant record. |

## 5. Non-Functional Requirements

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| NFR-PERF-001 | The API shall support configurable request and upload limits. | Must | Automated tests verify that configured limits are enforced. |
| NFR-PERF-002 | Long-running scientific processing shall not require the API request to remain open until processing completes. | Must | Processing requests return a job identifier while execution continues independently. |
| NFR-REL-001 | The platform shall preserve dataset records and processing states across normal application restarts. | Must | Integration tests verify persistence after service restart. |
| NFR-REL-002 | Interrupted processing jobs shall not be incorrectly reported as completed. | Must | Restart and failure tests verify that interrupted jobs are identifiable and recoverable or explicitly failed. |
| NFR-SCALE-001 | The processing worker component shall be independently deployable from the API component. | Must | The API and worker can be started, stopped and deployed as separate services. |
| NFR-MAINT-001 | The backend shall use a modular architecture that separates API, application, domain and infrastructure responsibilities. | Must | Code review and architecture tests verify the intended separation. |
| NFR-MAINT-002 | The platform shall maintain automated tests for critical business and security behaviour. | Must | CI executes the configured test suites and reports their results. |
| NFR-OBS-001 | The platform shall produce structured application logs for important operations and failures. | Must | Logs contain timestamps, severity and relevant correlation identifiers without exposing secrets. |
| NFR-PORT-001 | The initial platform shall support reproducible local deployment using container-based infrastructure. | Must | A documented deployment procedure starts the required services in a clean supported environment. |
| NFR-DOC-001 | Public-facing APIs and operational procedures shall have maintained technical documentation. | Must | Documentation is available in the repository and reviewed alongside relevant changes. |

## 6. Security Requirements

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| SEC-IAM-001 | Protected API operations shall require valid authentication. | Must | Security tests verify rejection of missing, invalid and expired credentials. |
| SEC-IAM-002 | Authorization shall be enforced by the backend and shall not rely solely on frontend visibility or controls. | Must | Direct API requests cannot bypass role or ownership restrictions. |
| SEC-IAM-003 | Refresh-token handling shall prevent reuse of invalidated or revoked refresh tokens. | Should | Tests verify token rotation and rejection of revoked or reused tokens. |
| SEC-DATA-001 | Dataset access shall be checked against the authenticated user's authorized ownership or workspace boundary. | Must | Tests verify that one user cannot retrieve another unauthorized user's dataset. |
| SEC-DATA-002 | The platform shall validate uploaded file names, file sizes and file content before processing. | Must | Tests verify rejection of disallowed names, excessive sizes and invalid file content. |
| SEC-DATA-003 | Original dataset integrity shall be verifiable using stored cryptographic checksums. | Must | Recalculation of the original file checksum matches the stored checksum unless integrity failure is detected. |
| SEC-PROC-001 | Scientific processing shall execute outside the API process with defined resource limits. | Must | Worker configuration enforces documented CPU, memory and execution-time limits. |
| SEC-PROC-002 | Processing workers shall operate with the minimum filesystem and network permissions required for their tasks. | Must | Worker configuration and tests demonstrate restricted access. |
| SEC-API-001 | The API shall validate input data and reject malformed or unsupported requests. | Must | API tests verify invalid input rejection without unhandled application failures. |
| SEC-API-002 | The platform shall apply configurable rate limits to sensitive or resource-intensive endpoints. | Should | Tests verify that requests exceeding configured limits are restricted. |
| SEC-SECRET-001 | Secrets, credentials, private keys and tokens shall not be committed to source control. | Must | Repository scanning and review find no committed active secrets. |
| SEC-SECRET-002 | Production secrets shall be supplied through an appropriate secret-management mechanism rather than hard-coded application configuration. | Must | Deployment configuration obtains secrets externally and does not require committed secret values. |
| SEC-AUD-001 | Security-relevant audit records shall be protected against unauthorized modification. | Must | Authorization tests verify that ordinary users cannot modify or delete audit records. |
| SEC-DEP-001 | Dependencies and container images shall be checked for known security vulnerabilities as part of the development pipeline. | Should | CI produces dependency or image scanning results and records findings. |

## 7. Operational Requirements

| ID | Requirement | Priority | Acceptance Criteria |
|---|---|---|---|
| OPS-DEP-001 | The platform shall provide a documented local deployment procedure. | Must | A developer can follow the documented procedure to start the required components. |
| OPS-DEP-002 | Application services shall expose health information suitable for service monitoring. | Must | Health checks distinguish healthy services from unavailable or unhealthy dependencies where applicable. |
| OPS-BKP-001 | The project shall document backup and restoration procedures for persistent metadata and stored datasets. | Should | A documented restoration exercise demonstrates recovery of selected test data. |
| OPS-MON-001 | The platform shall support collection of service logs, metrics and distributed tracing information. | Should | A local demonstration shows available telemetry for a representative request or processing job. |
| OPS-CI-001 | The repository shall use an automated pipeline to build the backend and execute applicable automated tests. | Must | A pipeline run reports build and test outcomes. |
| OPS-CI-002 | The development pipeline shall identify formatting, test or security-check failures before changes are accepted. | Should | Configured checks report failures and their results are visible in the pipeline. |
| OPS-DATA-001 | Test datasets and sample scientific files shall be kept separate from application source code. | Must | Repository structure and ignore rules prevent accidental inclusion of large raw datasets in normal source commits. |

## 8. Data and Scientific Integrity Constraints

1. Original scientific files must remain unchanged after ingestion.
2. Every successfully ingested original file must have a recorded SHA-256 checksum.
3. Extracted metadata must reflect the actual file contents.
4. Missing or unavailable metadata must not be replaced with invented values.
5. Processing results must identify their input dataset and processing configuration.
6. Scientific measurements must not be presented as validated discoveries without appropriate scientific validation.
7. The initial implementation must prioritize FITS inspection, validation, metadata extraction and traceable processing over unsupported scientific interpretation.

## 9. Initial Technology Constraints

The planned technology direction is:

| Component | Planned Technology |
|---|---|
| Backend API | .NET 10 / ASP.NET Core |
| Frontend | React / TypeScript |
| Structured database | MySQL |
| Object storage | S3-compatible storage, with MinIO for local development |
| Scientific processing | Python with Astropy |
| Local infrastructure | Docker Compose |
| Orchestration baseline | Kubernetes |
| Observability | OpenTelemetry |
| Backend testing | xUnit |
| Processing testing | pytest |
| CI/CD | GitHub Actions |

These are planned technology choices. They remain subject to implementation and validation.

## 10. Scope Boundaries

The initial requirements baseline does not include:

- A commercial EDR, SIEM or XDR platform.
- A complete astronomical discovery or automated scientific interpretation engine.
- An AI/ML-based discovery engine.
- Large-scale distributed scientific computing.
- Multi-region high availability or disaster recovery.
- Mobile, macOS or Linux endpoint agents.
- Commercial billing and subscription management.

Any proposed scope expansion must be reviewed and recorded before implementation.

## 11. Requirement Verification Rules

A requirement may be marked **Done** only when:

1. Its implementation exists in the repository.
2. Appropriate verification has been performed.
3. Test or inspection evidence is available.
4. Relevant documentation has been updated.
5. The change has been reviewed.
6. The change has been committed to Git.

A planned requirement must not be reported as implemented.

## 12. Requirement Change Control

Changes to this specification shall be reviewed before implementation.

Each change must preserve requirement identifiers where possible. Removed or replaced requirements must be documented rather than silently deleted.

Related architecture decisions, implementation tasks and test cases shall be linked through the Requirements Traceability Matrix.

## 13. Current Status

This document is version 0.1 and represents the initial requirements baseline for SPHERITHM.

The requirements are documented for planning and review. They are not evidence that the corresponding application capabilities have been implemented.

**Document status: Draft — Pending review.**

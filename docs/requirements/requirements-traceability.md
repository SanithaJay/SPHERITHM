# SPHERITHM — Requirements Traceability Matrix

| Document ID | DOC-005 |
|---|---|
| Version | 0.1 |
| Status | Draft |
| Created | 13 November 2026 |
| Related Document | DOC-004 — Requirements Specification |

## 1. Purpose

This Requirements Traceability Matrix (RTM) connects each requirement defined in DOC-004 to its responsible architecture component, planned implementation work and verification approach.

The matrix provides traceability across requirements, architecture, development and testing.

## 2. Traceability Conventions

### 2.1 Requirement categories

| Prefix | Category |
|---|---|
| FR | Functional Requirement |
| NFR | Non-Functional Requirement |
| SEC | Security Requirement |
| OPS | Operational Requirement |

### 2.2 Status definitions

| Status | Meaning |
|---|---|
| Not Started | Implementation or verification has not started. |
| In Progress | Work is actively underway. |
| Implemented | Code or configuration exists but verification is pending. |
| Verified | Acceptance criteria have been tested and passed. |
| Blocked | Work cannot proceed because of an identified blocker. |
| Rework | Previously implemented work requires correction. |

Implementation status and verification status are tracked independently.

### 2.3 Architecture component abbreviations

| Component | Responsibility |
|---|---|
| API | ASP.NET Core API |
| Application | Application services and use cases |
| Domain | Core business entities and rules |
| Infrastructure | Database, storage and external integrations |
| IAM | Authentication and authorization |
| Frontend | React and TypeScript web interface |
| Worker | Python scientific processing worker |
| FITS | FITS validation and metadata extraction |
| Storage | S3-compatible object storage |
| Audit | Security audit and event records |
| Database | MySQL persistence |
| CI/CD | Build, testing and deployment pipeline |
| Observability | Logs, metrics, tracing and health checks |
| Infrastructure | Docker Compose and Kubernetes deployment |

These are logical component references. They do not imply that all components have been implemented.

## 3. Functional Requirements Traceability

### 3.1 Identity and Access Management

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| FR-IAM-001 | API, IAM, Application | Implement user authentication and protected API access. | Authentication tests for valid, invalid and missing credentials. | Not Started | Not Started |
| FR-IAM-002 | IAM, Application | Implement role-based permission evaluation. | Authorization tests for permitted and denied operations. | Not Started | Not Started |
| FR-IAM-003 | IAM, Domain, Database | Define Platform Administrator, Data Curator, Research Analyst and Auditor roles. | Verify role definitions and role-specific permissions. | Not Started | Not Started |
| FR-IAM-004 | API, IAM, Database, Audit | Implement authorized user and role administration. | Integration tests for role assignment and unauthorized modification. | Not Started | Not Started |

### 3.2 Dataset Ingestion and Preservation

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| FR-DATA-001 | API, Application, Storage | Implement authenticated scientific dataset upload. | API integration test for successful and rejected uploads. | Not Started | Not Started |
| FR-DATA-002 | API, Application, Storage | Implement configurable upload size limits. | Boundary tests for permitted and oversized files. | Not Started | Not Started |
| FR-DATA-003 | Application, Storage, Database | Implement SHA-256 checksum generation and persistence. | Independently calculate and compare file checksums. | Not Started | Not Started |
| FR-DATA-004 | Storage, Infrastructure | Separate immutable original objects from processing outputs. | Verify original object remains unchanged after processing. | Not Started | Not Started |
| FR-DATA-005 | Domain, Application, Database | Implement dataset metadata and ownership records. | Integration tests for required metadata persistence and retrieval. | Not Started | Not Started |
| FR-DATA-006 | Application, Storage, Database | Implement checksum-based duplicate detection within ownership or workspace boundaries. | Upload identical files and verify duplicate detection without replacement. | Not Started | Not Started |

### 3.3 FITS Validation and Metadata

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| FR-FITS-001 | Worker, FITS | Implement FITS structural validation using Astropy. | Test valid, malformed and unsupported FITS samples. | Not Started | Not Started |
| FR-FITS-002 | Worker, FITS | Implement FITS header and HDU inspection. | Compare extracted HDU information with known FITS test fixtures. | Not Started | Not Started |
| FR-FITS-003 | Worker, FITS, Database | Extract available observation identifiers, timestamps and detector metadata. | Compare extracted values against source FITS headers. | Not Started | Not Started |
| FR-FITS-004 | Worker, Application, Database | Record validation outcomes and error details. | Test successful and failed validation and retrieve their records. | Not Started | Not Started |
| FR-FITS-005 | Worker, Storage, Database | Store extracted metadata separately from original scientific files. | Verify metadata persistence and unchanged original checksum. | Not Started | Not Started |

### 3.4 Processing and Results

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| FR-PROC-001 | API, Application, Worker | Implement authorized processing job submission. | Verify valid job creation and rejection of unauthorized requests. | Not Started | Not Started |
| FR-PROC-002 | Worker, Infrastructure | Implement separate Python processing worker. | Verify processing runs independently of the API process. | Not Started | Not Started |
| FR-PROC-003 | Application, Worker, Database | Implement processing job lifecycle and state persistence. | Test queued, running, completed and failed state transitions. | Not Started | Not Started |
| FR-PROC-004 | Worker, Storage, Database | Store processing artifacts separately from original datasets. | Verify output object identity and original-file integrity. | Not Started | Not Started |
| FR-PROC-005 | Worker, Application, Database | Record processing parameters, worker version, input identity and outcome. | Verify required provenance fields for a completed processing job. | Not Started | Not Started |
| FR-PROC-006 | Worker, Application, Database | Implement explicit processing failure handling. | Simulate worker failure and verify failed state and error details. | Not Started | Not Started |

### 3.5 Dataset Access and Audit

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| FR-AUD-001 | API, Application, Audit, Database | Record security-relevant user and administrative actions. | Perform selected actions and verify corresponding audit records. | Not Started | Not Started |
| FR-AUD-002 | API, Application, Database | Implement authorized dataset metadata and processing-history retrieval. | Integration tests for authorized record retrieval. | Not Started | Not Started |
| FR-AUD-003 | IAM, Application, Database | Enforce dataset ownership and workspace access restrictions. | Attempt unauthorized cross-boundary dataset access. | Not Started | Not Started |
| FR-AUD-004 | IAM, Audit, API | Provide read-only audit access for authorized auditors. | Verify audit retrieval is permitted and audit modification is denied. | Not Started | Not Started |

### 3.6 Application Interface and API

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| FR-API-001 | API, Application, Contracts | Implement and document supported API endpoints and contracts. | API integration tests and API documentation review. | Not Started | Not Started |
| FR-API-002 | API, Application, Contracts | Implement consistent structured error responses. | Test invalid requests and expected status codes and response structures. | Not Started | Not Started |
| FR-UI-001 | Frontend, API, IAM | Implement authenticated role-aware web interface. | UI tests for authenticated access and role-based visibility. | Not Started | Not Started |
| FR-UI-002 | Frontend, API, Application | Display dataset, validation and processing status. | UI integration tests against representative status responses. | Not Started | Not Started |

## 4. Non-Functional Requirements Traceability

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| NFR-PERF-001 | API, Infrastructure | Configure request and upload limits. | Automated tests at and beyond configured limits. | Not Started | Not Started |
| NFR-PERF-002 | API, Application, Worker | Implement asynchronous processing jobs. | Verify job submission returns without waiting for processing completion. | Not Started | Not Started |
| NFR-REL-001 | Database, Storage, Infrastructure | Persist dataset and job records across service restarts. | Restart services and verify persisted records. | Not Started | Not Started |
| NFR-REL-002 | Worker, Application, Database | Handle interrupted processing jobs safely. | Simulate interruption and verify job state is not incorrectly completed. | Not Started | Not Started |
| NFR-SCALE-001 | API, Worker, Infrastructure | Support independent API and worker deployment. | Deploy and operate API and worker as separate services. | Not Started | Not Started |
| NFR-MAINT-001 | Domain, Application, Infrastructure, API | Implement modular backend architecture. | Architecture review and automated architecture checks where applicable. | Not Started | Not Started |
| NFR-MAINT-002 | Tests, CI/CD | Establish automated backend and security test suites. | Verify CI executes configured tests and reports results. | Not Started | Not Started |
| NFR-OBS-001 | API, Worker, Observability | Implement structured application logging. | Inspect logs for timestamps, severity and correlation identifiers. | Not Started | Not Started |
| NFR-PORT-001 | Infrastructure, Docker | Provide reproducible container-based local deployment. | Deploy services using documented instructions in a clean environment. | Not Started | Not Started |
| NFR-DOC-001 | Documentation, API, Infrastructure | Maintain API and operational documentation. | Documentation review against implemented API and deployment behaviour. | Not Started | Not Started |

## 5. Security Requirements Traceability

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| SEC-IAM-001 | API, IAM | Enforce authentication for protected API operations. | Security tests for missing, invalid and expired credentials. | Not Started | Not Started |
| SEC-IAM-002 | API, IAM, Application | Enforce server-side authorization. | Direct API authorization bypass tests. | Not Started | Not Started |
| SEC-IAM-003 | IAM, Database | Implement refresh-token rotation and revocation. | Test rotation, invalidation and reuse rejection. | Not Started | Not Started |
| SEC-DATA-001 | IAM, Application, Database | Enforce ownership and workspace boundary checks. | Cross-user and cross-workspace access tests. | Not Started | Not Started |
| SEC-DATA-002 | API, Application, Storage, FITS | Validate uploaded filenames, sizes and file contents. | Test malformed files, invalid names and excessive sizes. | Not Started | Not Started |
| SEC-DATA-003 | Application, Storage | Verify original dataset integrity using SHA-256. | Modify a test copy and verify checksum mismatch detection. | Not Started | Not Started |
| SEC-PROC-001 | Worker, Infrastructure | Isolate processing and configure resource limits. | Inspect worker isolation configuration and execute resource-limit tests. | Not Started | Not Started |
| SEC-PROC-002 | Worker, Infrastructure | Restrict worker filesystem and network permissions. | Review permissions and test access to prohibited resources. | Not Started | Not Started |
| SEC-API-001 | API, Contracts, Application | Validate API request data and reject malformed inputs. | API negative tests and malformed-input tests. | Not Started | Not Started |
| SEC-API-002 | API, Infrastructure | Configure rate limits for sensitive and resource-intensive endpoints. | Automated tests for requests exceeding configured limits. | Not Started | Not Started |
| SEC-SECRET-001 | Repository, CI/CD | Prevent secrets from being committed to source control. | Secret scanning and repository review. | Not Started | Not Started |
| SEC-SECRET-002 | Infrastructure, API | Supply deployment secrets through external secret configuration. | Inspect deployment configuration and verify no hard-coded production secrets. | Not Started | Not Started |
| SEC-AUD-001 | Audit, Database, IAM | Protect audit records against unauthorized modification. | Attempt unauthorized audit update and deletion operations. | Not Started | Not Started |
| SEC-DEP-001 | CI/CD, Infrastructure | Integrate dependency and container vulnerability scanning. | Verify pipeline scan execution and review generated findings. | Not Started | Not Started |

## 6. Operational Requirements Traceability

| Requirement ID | Architecture Component | Planned Implementation Work | Verification Method | Implementation Status | Verification Status |
|---|---|---|---|---|---|
| OPS-DEP-001 | Infrastructure, Documentation | Document and configure local deployment. | Execute deployment procedure and verify required services start. | Not Started | Not Started |
| OPS-DEP-002 | API, Worker, Observability | Implement service health checks. | Test healthy and unavailable service and dependency conditions. | Not Started | Not Started |
| OPS-BKP-001 | Database, Storage, Documentation | Document backup and restoration procedures. | Restore selected test records and files in a controlled environment. | Not Started | Not Started |
| OPS-MON-001 | API, Worker, Observability | Integrate logs, metrics and distributed tracing. | Demonstrate telemetry for a representative API request and processing job. | Not Started | Not Started |
| OPS-CI-001 | CI/CD, API, Tests | Implement automated backend build and test pipeline. | Execute pipeline and inspect build and test results. | Not Started | Not Started |
| OPS-CI-002 | CI/CD, Repository | Configure formatting, testing and security checks. | Introduce controlled test failures and verify pipeline reporting. | Not Started | Not Started |
| OPS-DATA-001 | Repository, Storage, Infrastructure | Separate test samples and scientific data from source code. | Review repository structure, ignore rules and Git-tracked files. | Not Started | Not Started |

## 7. Traceability Summary

| Requirement Category | Requirement Count |
|---|---:|
| Functional Requirements | 29 |
| Non-Functional Requirements | 10 |
| Security Requirements | 14 |
| Operational Requirements | 7 |
| **Total** | **60** |

Every requirement in DOC-004 is represented in this matrix.

## 8. Traceability Lifecycle

The expected traceability chain is:

Requirement
→ Architecture Component
→ Implementation Work
→ Test Case
→ Verification Evidence
→ Git Commit

Each requirement must retain its original identifier throughout its lifecycle.

Implementation status must be updated when development work is completed. Verification status must be updated only after the corresponding acceptance criteria have been tested.

## 9. Evidence Management

Evidence may include:

- Source code paths and relevant Git commits.
- Unit and integration test results.
- API security test results.
- FITS validation test reports.
- Worker isolation and resource-limit test results.
- CI pipeline execution results.
- Deployment and restoration test records.
- Architecture and security review records.

Evidence references shall be added as work progresses. Planned evidence must not be presented as completed evidence.

## 10. Change Management

When a requirement changes:

1. Review the corresponding requirement in DOC-004.
2. Preserve its identifier where possible.
3. Update the related architecture and implementation references.
4. Update verification methods when acceptance criteria change.
5. Record relevant decisions in architecture decision records where appropriate.
6. Review and commit the updated documentation.

## 11. Current Status

This matrix represents the initial requirements traceability baseline for SPHERITHM.

All 60 requirements are mapped to planned architecture components, implementation work and verification methods.

No implementation or verification completion is claimed by this document.

**Document status: Draft — Pending review.**

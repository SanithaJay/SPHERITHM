# SPHERITHM
## Secure Scientific Data Platform

**Trust every file. Trace every result.**

SPHERITHM is an independent, security-first scientific data platform designed to secure the lifecycle of scientific datasets — from ingestion and validation to controlled processing, storage, provenance and audit.

The initial implementation uses real astronomical FITS observations, including publicly available SPHEREx data, as its scientific engineering use case.

> SPHERITHM is an independent project. It is not an official NASA product and is not endorsed by NASA, Caltech, IPAC or the SPHEREx mission team.

## 1. The problem

Scientific data platforms must handle large and complex files, automated processing and access by multiple users. These workflows introduce security and reliability challenges:

- Malformed or untrusted scientific files can expose parsers and processing services to risk.
- Unrestricted processing can exhaust CPU, memory, storage or execution time.
- Weak authorization can expose datasets across users or organizations.
- Missing integrity checks make it difficult to verify original data.
- Incomplete provenance makes scientific results difficult to reproduce.
- Insufficient audit records make security investigations harder.

SPHERITHM is designed to address these challenges through a unified, security-focused application architecture.

## 2. Product objectives

1. Securely ingest scientific datasets.
2. Validate file structure, size and metadata before processing.
3. Preserve original inputs and calculate SHA-256 integrity hashes.
4. Process untrusted files in a separately controlled worker.
5. Enforce authentication, role-based authorization and organization-level isolation.
6. Record processing parameters, software versions and output integrity.
7. Maintain security-relevant audit events.
8. Provide a usable interface for datasets, jobs, results and audit records.
9. Support reproducible deployment and automated security verification.

## 3. Initial scientific use case

The first supported scientific format is FITS, including multi-frame observation cubes.

Planned capabilities include:

- FITS HDU and structure inspection.
- Science, variance, quality-flag and supported auxiliary-extension handling.
- Observation, detector and frame metadata extraction.
- Timestamp and sequence inspection.
- World Coordinate System (WCS) metadata inspection.
- Controlled background processing.
- Validated source-position measurements.
- Result provenance and integrity verification.

Scientific measurements will not be represented as discoveries without documented methods, appropriate validation and supporting evidence.

## 4. Architecture

SPHERITHM uses a layered architecture that separates user interaction, application services, scientific processing and data storage.

```text
Users
  |
  v
React + TypeScript Dashboard
  |
  v
ASP.NET Core API
  |
  +------ MySQL Metadata Database
  |
  +------ S3-Compatible Object Storage
  |
  +------ Authentication and Authorization
  |
  +------ Audit and Provenance Services
  |
  v
Job Management Service
  |
  v
Isolated Python / Astropy Worker
  |
  v
FITS Validation and Scientific Processing
  |
  v
Verified Results
  |
  v
Provenance and Audit Records
```

### Architecture Responsibilities

| Component | Responsibility |
|---|---|
| React Dashboard | User interface and platform interaction |
| ASP.NET Core API | Application services and business rules |
| MySQL | Users, permissions, datasets and job metadata |
| Object Storage | Original scientific files and generated outputs |
| Job Management | Processing requests and job lifecycle |
| Python Worker | FITS inspection and scientific computations |
| Audit Services | Security events and traceability |
| Provenance Services | Input hashes, processing parameters and result lineage |

The architecture will be refined through documented architecture decisions and threat modelling.

## 5. Security Engineering

Security is a core product requirement.

### 5.1 Identity and Access Security

- Secure authentication.
- Role-based access control (RBAC).
- Organization-level resource isolation.
- Resource ownership verification.
- Protected API endpoints.
- Secure session and token handling.

### 5.2 Scientific File Security

- Upload size and content validation.
- FITS structure inspection before processing.
- SHA-256 integrity verification.
- Preservation of original input files.
- Controlled access to uploaded datasets.
- Safe handling of malformed or unexpected files.

### 5.3 Processing Security

- Separate scientific processing worker.
- Restricted worker permissions.
- CPU, memory and execution-time limits.
- Controlled filesystem access.
- Restricted network access.
- Validation of processing inputs and outputs.

### 5.4 Platform Security

- Secure configuration and secret management.
- Input validation and error handling.
- Audit logging for security-relevant actions.
- Dependency vulnerability scanning.
- Secret scanning.
- Automated authorization and negative security tests.

### 5.5 Security Verification

Security controls will be considered complete only when implemented, tested and documented.

SPHERITHM will not claim absolute security. Security claims must be supported by verifiable implementation and test evidence.

## 6. Technology Stack

| Component | Technology |
|---|---|
| Backend | C# / ASP.NET Core / .NET 10 |
| Frontend | React / TypeScript |
| Scientific Processing | Python / Astropy |
| Database | MySQL |
| Object Storage | S3-compatible storage |
| Local Deployment | Docker Compose |
| Orchestration | Kubernetes |
| Backend Testing | xUnit |
| Processing Testing | pytest |
| Observability | OpenTelemetry |
| Continuous Integration | GitHub Actions |

Exact dependency versions will be pinned during implementation.

## 7. Repository Structure

The following is the planned repository structure. Additional files and folders will be added as implementation progresses.

```text
SPHERITHM/
|-- .github/
|   |-- workflows/
|-- data/
|   |-- manifests/
|   |-- samples/
|-- docs/
|   |-- product/
|   |-- requirements/
|   |-- architecture/
|   |-- security/
|   |-- adr/
|   |-- api/
|   |-- operations/
|   |-- research/
|-- infrastructure/
|   |-- docker/
|   |-- kubernetes/
|   |-- monitoring/
|-- scripts/
|-- src/
|   |-- backend/
|   |   |-- Spherithm.Api/
|   |   |-- Spherithm.Application/
|   |   |-- Spherithm.Domain/
|   |   |-- Spherithm.Infrastructure/
|   |   |-- Spherithm.Contracts/
|   |-- processing/
|   |   |-- worker/
|   |-- frontend/
|-- tests/
|   |-- unit/
|   |-- integration/
|   |-- api-security/
|   |-- processing/
|-- README.md
|-- SECURITY.md (planned)
|-- CONTRIBUTING.md (planned)
|-- .gitignore
|-- .editorconfig (planned)
```
## 8. Development Principles

- Define requirements before implementation.
- Document security boundaries and trust assumptions.
- Preserve original scientific inputs.
- Trace processing results to input hashes and parameters.
- Test normal and adversarial conditions.
- Never commit secrets or private datasets.
- Require implementation, testing, documentation and review before marking a feature complete.

## 9. Project Status

**Status: Initial project setup**

The product specification, requirements baseline, architecture and threat model are being established.

No application feature is currently claimed as implemented.

## 10. Scientific Data Sources

The initial scientific use case uses public data and services from the NASA/IPAC Infrared Science Archive.

- IRSA: https://irsa.ipac.caltech.edu/
- SPHEREx mission: https://science.nasa.gov/mission/spherex/

Dataset-specific observation identifiers, retrieval details, provenance and applicable usage terms will be recorded before public distribution.

## 11. Roadmap

- [ ] Product specification and requirements baseline
- [ ] Architecture and threat model
- [ ] Backend foundation
- [ ] Authentication and authorization
- [ ] Secure FITS ingestion
- [ ] Integrity verification and original-data preservation
- [ ] Isolated scientific processing
- [ ] Provenance and audit
- [ ] Frontend dashboard
- [ ] Automated security and integration tests
- [ ] Docker deployment
- [ ] Kubernetes deployment baseline
- [ ] CI security checks
- [ ] Release documentation

## 12. License

The project license will be selected after reviewing dependency licenses, intended distribution and scientific data terms.

Until a license is added, no additional permission to redistribute this repository's contents is granted.

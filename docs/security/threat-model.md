# SPHERITHM Threat Model

**Document ID:** DOC-007
**Version:** 0.1
**Status:** Draft
**Owner:** Project Owner
**Last Updated:** 14-11-2026

---

## 1. Purpose

This document identifies security threats affecting the SPHERITHM platform, evaluates their potential impact, and defines mitigation and verification measures.

The threat model establishes a security-focused foundation for the design, implementation, testing, and operation of SPHERITHM.

It is intended to:

- Identify critical assets and security boundaries.
- Identify potential threat actors and attack scenarios.
- Analyse threats using the STRIDE methodology.
- Define security controls and mitigation strategies.
- Record residual risks requiring further review.
- Connect security design decisions to implementation and verification activities.

## 2. Scope

This threat model covers the planned SPHERITHM platform, including:

- React and TypeScript web application.
- ASP.NET Core REST API built on .NET 10.
- MySQL structured database.
- S3-compatible object storage for original datasets and processing artifacts.
- Python-based scientific processing worker using Astropy.
- Authentication, authorization, tenant isolation, and audit capabilities.
- Container-based local deployment and a small-cluster Kubernetes baseline.
- CI/CD workflows and application dependencies.

### 2.1 Security Scope

The analysis considers threats involving:

- Unauthorized access to platform resources.
- Authentication and authorization failures.
- Cross-tenant data exposure.
- Malicious or malformed scientific files.
- Unsafe FITS parsing and processing.
- Data tampering, loss, or unauthorized deletion.
- Processing worker compromise or resource exhaustion.
- API abuse and application-layer attacks.
- Compromise of credentials, secrets, or dependencies.
- Insufficient audit records and provenance integrity.

### 2.2 Out of Scope

The following are outside the current project scope:

- Complete enterprise EDR, XDR, or SIEM capabilities.
- Advanced threat intelligence and automated threat hunting.
- Large-scale distributed scientific computing.
- Multi-region high availability and disaster recovery.
- Commercial billing and payment processing.
- Full public-key infrastructure and certificate lifecycle management.
- Native mobile, macOS, and Linux endpoint agents.

Out-of-scope items may be reconsidered in future project versions.

## 3. System Overview

SPHERITHM is a security-first scientific data platform designed to support controlled ingestion, validation, preservation, processing, and traceability of scientific datasets.

The initial scientific data use case focuses on astronomical FITS files.

The intended high-level workflow is:

1. An authorized user accesses the web application.
2. The API authenticates the user and checks authorization.
3. The user submits a scientific dataset.
4. The platform validates the upload and records its metadata.
5. The original dataset is preserved in object storage.
6. A processing job is created and managed by the API.
7. An isolated Python worker receives controlled job input.
8. The worker validates and processes the scientific data.
9. Processing outputs and execution metadata are stored.
10. The platform records audit and provenance information.
11. Authorized users access permitted results.

The system separates user interaction, application logic, structured persistence, object storage, and scientific processing to establish clear security boundaries.

## 4. Security Objectives

SPHERITHM uses the following security objectives as the basis for threat analysis.

### 4.1 Confidentiality

- Prevent unauthorized access to datasets, metadata, processing outputs, and user information.
- Enforce authorization and tenant isolation.
- Protect credentials, tokens, and application secrets.

### 4.2 Integrity

- Detect unauthorized modification of uploaded datasets and processing outputs.
- Preserve original scientific files.
- Maintain reliable metadata, audit records, and processing provenance.
- Verify file integrity using cryptographic checksums where applicable.

### 4.3 Availability

- Protect the platform against resource exhaustion and abusive requests.
- Apply upload limits and processing resource constraints.
- Prevent one processing job from unnecessarily affecting other platform operations.

### 4.4 Accountability and Traceability

- Record security-relevant user and system activities.
- Associate processing outputs with their input datasets and execution records.
- Maintain sufficient provenance to support reproducibility and investigation.

### 4.5 Isolation

- Separate application-facing services from scientific file processing.
- Restrict worker permissions and access to resources.
- Prevent untrusted scientific input from gaining unnecessary access to the host or other platform components.


## 5. Assets and Data Classification

SPHERITHM processes scientific datasets and supporting platform information. Assets must be protected according to their confidentiality, integrity, and availability requirements.

### 5.1 Primary Assets

| Asset | Description | Protection Priority |
|---|---|---|
| Original scientific files | Uploaded FITS files and other supported datasets | High |
| Processing outputs | Generated images, measurements, and derived results | High |
| Dataset metadata | Dataset identifiers, headers, dimensions, and metadata | High |
| User accounts | User identities and account information | High |
| Authentication credentials | Password hashes, access tokens, and refresh tokens | Critical |
| Application secrets | Database credentials, storage keys, and signing keys | Critical |
| Audit records | Security events and user activity records | High |
| Processing provenance | Job configuration, input references, and execution records | High |
| Platform configuration | Application and infrastructure settings | High |

### 5.2 Data Classification

| Classification | Examples | Required Protection |
|---|---|---|
| Public | Publicly available scientific datasets | Integrity and availability controls |
| Internal | Operational metadata and processing information | Authenticated access |
| Confidential | User information, private datasets, and audit records | Authorization and restricted access |
| Restricted | Password hashes, tokens, signing keys, and application secrets | Strict access control and secure storage |

### 5.3 Protection Requirements

- Preserve uploaded original files against unauthorized modification or deletion.
- Restrict private datasets and processing outputs to authorized users.
- Never expose credentials, tokens, or application secrets through logs or API responses.
- Apply access controls to metadata and audit records.
- Maintain integrity checks and provenance for scientific processing.
- Avoid storing sensitive information unnecessarily.


## 6. Trust Boundaries

Trust boundaries separate components with different levels of access and security responsibility. Data crossing these boundaries must be authenticated, authorized, and validated.

### 6.1 Identified Trust Boundaries

| Boundary | Components | Main Security Concern |
|---|---|---|
| TB-01: User to Frontend | Browser and React application | Untrusted user input and session security |
| TB-02: Frontend to API | React application and ASP.NET Core API | Authentication, authorization, and API abuse |
| TB-03: API to Database | API and MySQL | Unauthorized queries and data exposure |
| TB-04: API to Object Storage | API and S3-compatible storage | Unauthorized file access and object manipulation |
| TB-05: API to Processing Worker | API, job queue, and Python worker | Untrusted job parameters and worker compromise |
| TB-06: Worker to File System | Python worker and processing environment | Path traversal and unauthorized file access |
| TB-07: Platform to External Services | Application and external dependencies | Supply-chain and service communication risks |
| TB-08: Deployment Environment | Containers, host system, and orchestration | Privilege escalation and configuration exposure |

### 6.2 Boundary Security Requirements

- Authenticate users before granting access to protected API operations.
- Enforce authorization on the server; frontend controls must not be treated as security boundaries.
- Validate all API inputs and uploaded files before processing.
- Apply least-privilege credentials to database and object-storage connections.
- Restrict worker access to only the files and resources required for its assigned job.
- Prevent workers from accessing application secrets or unrelated datasets.
- Restrict container privileges and host filesystem access.
- Protect communication between platform components using appropriate transport security.
- Treat external libraries, services, and scientific files as untrusted until validated.

### 6.3 Untrusted Input Sources

The following inputs must be treated as untrusted:

- User-provided filenames, metadata, and upload contents.
- FITS headers, dimensions, extensions, and data arrays.
- API request parameters and job configuration.
- External dependency packages and service responses.
- Files generated by processing workers until validated.


## 7. Threat Actors

SPHERITHM must account for threats from both external and internal sources. Threat actors may attempt to access data, compromise services, manipulate scientific results, or disrupt platform availability.

### 7.1 External Threat Actors

| Threat Actor | Motivation | Potential Actions |
|---|---|---|
| Unauthenticated attacker | Unauthorized access or disruption | API probing, credential attacks, upload abuse |
| Malicious authenticated user | Access beyond assigned permissions | Cross-tenant access attempts, unauthorized downloads |
| Malicious file provider | Exploit file-processing weaknesses | Upload malformed or crafted FITS files |
| Automated bot or scanner | Discover exposed services | Repeated requests, endpoint enumeration, resource exhaustion |
| Supply-chain attacker | Compromise application dependencies | Introduce malicious or vulnerable packages |

### 7.2 Internal Threat Actors

| Threat Actor | Motivation | Potential Actions |
|---|---|---|
| Compromised user account | Misuse legitimate access | Unauthorized dataset access or modification |
| Privileged insider | Abuse administrative permissions | Change access policies, manipulate records |
| Compromised processing worker | Gain access beyond assigned job | Read unrelated files, consume resources, access secrets |
| Misconfigured service identity | Exploit excessive permissions | Access databases or storage beyond operational needs |

### 7.3 Threat Assumptions

- Users may submit malformed, oversized, or intentionally malicious files.
- Authenticated users may attempt to bypass authorization boundaries.
- Application dependencies may contain vulnerabilities.
- Processing workers may fail or become compromised.
- Configuration errors may expose services, credentials, or stored data.
- Privileged accounts may be misused or compromised.


## 8. Threat Modelling Methodology

SPHERITHM uses a structured threat-modelling approach to identify risks across application components, data flows, trust boundaries, and processing operations.

### 8.1 Method

The following activities are used:

1. Identify system assets and sensitive information.
2. Map application components, data flows, and trust boundaries.
3. Identify external and internal threat actors.
4. Analyse potential threats using the STRIDE framework.
5. Assess the potential impact and likelihood of identified threats.
6. Define preventive, detective, and corrective security controls.
7. Map security controls to verification activities.
8. Review the threat model when architecture, dependencies, or major features change.

### 8.2 STRIDE Categories

| Category | Meaning | SPHERITHM Example |
|---|---|---|
| Spoofing | Impersonating a user or component | Stolen credentials used to impersonate a researcher |
| Tampering | Unauthorized modification of data or configuration | Modification of an uploaded dataset or processing result |
| Repudiation | Denying an action without reliable evidence | User denies initiating a dataset deletion |
| Information Disclosure | Exposure of protected information | Unauthorized access to private datasets or secrets |
| Denial of Service | Disrupting service availability | Oversized uploads or resource-intensive processing jobs |
| Elevation of Privilege | Gaining permissions beyond those assigned | A researcher gains administrative access |

### 8.3 Risk Assessment

Each identified threat will be evaluated using:

- Likelihood: Low, Medium, or High.
- Impact: Low, Medium, or High.
- Risk rating: derived from the likelihood and impact assessment.

Risk ratings are qualitative and must be supported by documented reasoning. They are not claims of measured exploit probability.

### 8.4 Control and Verification

Each significant threat should have:

- A unique threat identifier.
- An affected asset or component.
- A threat description and potential consequence.
- An assigned risk rating.
- One or more mitigation controls.
- A verification method.
- A documented residual-risk status.


## 9. STRIDE Threat Analysis

The following table identifies representative threats across the SPHERITHM architecture. Risk ratings are preliminary and must be reassessed as implementation details become available.

| ID | STRIDE Category | Threat | Affected Component | Preliminary Risk |
|---|---|---|---|---|
| TM-001 | Spoofing | Stolen credentials used to access another user's account | Authentication API | High |
| TM-002 | Spoofing | Forged or replayed service identity | Internal services | Medium |
| TM-003 | Tampering | Unauthorized modification of original scientific files | Object storage | High |
| TM-004 | Tampering | Manipulation of processing parameters or outputs | Processing pipeline | High |
| TM-005 | Repudiation | User denies performing a sensitive operation | API and audit system | Medium |
| TM-006 | Information Disclosure | Cross-tenant dataset or metadata exposure | API and database | High |
| TM-007 | Information Disclosure | Application secrets exposed through logs or configuration | API and deployment | High |
| TM-008 | Denial of Service | Oversized uploads exhaust storage or API resources | Upload service | High |
| TM-009 | Denial of Service | Crafted files consume excessive worker resources | Processing worker | High |
| TM-010 | Elevation of Privilege | Researcher accesses administrator operations | Authorization layer | High |
| TM-011 | Elevation of Privilege | Compromised worker accesses host resources | Worker environment | High |

### 9.1 Initial Mitigation Mapping

| Threat IDs | Planned Mitigations |
|---|---|
| TM-001, TM-002 | Secure authentication, token validation, expiration, and service identity controls |
| TM-003, TM-004 | Original-file preservation, checksum verification, controlled processing, and output provenance |
| TM-005 | User-attributed audit events and protected audit storage |
| TM-006 | Server-side authorization, tenant ownership checks, and database access controls |
| TM-007 | Secret management, restricted configuration access, and log redaction |
| TM-008, TM-009 | Upload limits, request throttling, processing quotas, and worker resource limits |
| TM-010 | Role-based authorization and administrative operation checks |
| TM-011 | Least privilege, container isolation, restricted filesystem access, and worker resource boundaries |

## 10. Threat Scenarios and Mitigations

This section describes representative attack scenarios and the controls planned to reduce their likelihood and impact.

### 10.1 Unauthorized Dataset Access

**Scenario:** An authenticated user changes a dataset identifier in an API request to retrieve another user's private dataset.

**Potential Impact:** Confidentiality breach and unauthorized access to scientific data.

**Mitigations:**
- Enforce authorization on every dataset operation.
- Verify dataset ownership or explicitly granted access.
- Apply tenant isolation in application and database queries.
- Avoid relying on unpredictable identifiers as an authorization mechanism.

**Verification:** Attempt cross-user and cross-tenant dataset access using integration tests.

### 10.2 Malicious or Malformed FITS Upload

**Scenario:** An attacker uploads a malformed or deliberately crafted FITS file to exploit parsing weaknesses.

**Potential Impact:** Processing failure, resource exhaustion, or compromise of the processing environment.

**Mitigations:**
- Enforce upload size and request limits.
- Validate file structure before scientific processing.
- Use a maintained FITS parsing library.
- Process untrusted files in an isolated worker.
- Apply execution time and resource limits.

**Verification:** Test invalid, truncated, oversized, and malformed FITS inputs.

### 10.3 Unauthorized Modification of Original Data

**Scenario:** A user or compromised processing component attempts to overwrite an uploaded original dataset.

**Potential Impact:** Loss of scientific integrity and unreliable results.

**Mitigations:**
- Preserve original uploads separately from derived outputs.
- Restrict write permissions after ingestion.
- Calculate and verify cryptographic checksums.
- Record dataset and processing provenance.

**Verification:** Attempt unauthorized modification and compare recorded checksums.

### 10.4 Processing Worker Compromise

**Scenario:** A crafted input exploits a processing dependency and attempts to access unrelated files or platform secrets.

**Potential Impact:** Data exposure, service compromise, or lateral movement.

**Mitigations:**
- Run workers with non-root identities.
- Restrict filesystem access to job-specific directories.
- Avoid exposing API credentials and application secrets to workers.
- Apply CPU, memory, and execution-time limits.
- Restrict unnecessary network access.

**Verification:** Inspect worker permissions and test access to prohibited paths and resources.

### 10.5 Resource Exhaustion

**Scenario:** Repeated uploads or expensive processing requests consume available storage, memory, or CPU.

**Potential Impact:** Reduced platform availability or service interruption.

**Mitigations:**
- Enforce upload and request limits.
- Apply rate limiting and job quotas.
- Limit worker CPU, memory, and execution time.
- Monitor storage and processing capacity.

**Verification:** Conduct controlled upload and processing limit tests.


## 11. Authentication and Authorization Threats

Authentication and authorization protect user accounts, platform operations, and tenant-owned resources.

### 11.1 Identified Threats

- Credential theft or brute-force attempts against user accounts.
- Theft, leakage, or misuse of access and refresh tokens.
- Unauthorized access caused by missing server-side permission checks.
- Privilege escalation through incorrect role assignments.
- Cross-tenant access caused by incomplete ownership validation.
- Exposure of authentication secrets through logs or configuration.

### 11.2 Planned Security Controls

- Use secure password hashing and established authentication mechanisms.
- Validate token signatures, issuer, audience, and expiration.
- Apply short-lived access tokens and controlled refresh-token handling.
- Enforce role-based authorization on protected API operations.
- Validate resource ownership and tenant boundaries server-side.
- Protect signing keys and other authentication secrets.
- Record security-relevant authentication and authorization events.

### 11.3 Verification

- Test invalid, expired, and tampered tokens.
- Test access to endpoints using insufficient roles.
- Test cross-tenant resource access.
- Verify that secrets and tokens are not exposed in logs or API responses.

## 12. File Ingestion and FITS Processing Threats

Scientific file ingestion is a major security boundary because uploaded files are untrusted input.

### 12.1 Identified Threats

- Malformed or corrupted FITS files.
- Oversized files and decompression or parsing resource exhaustion.
- Path traversal through filenames or file metadata.
- Parser vulnerabilities in scientific dependencies.
- Unexpected FITS extensions, dimensions, or metadata values.
- Malicious content targeting the processing environment.

### 12.2 Planned Security Controls

- Apply upload size, request, and file-count limits.
- Generate server-controlled storage names rather than trusting supplied filenames.
- Validate file structure, dimensions, and supported formats.
- Reject invalid or unsupported files with controlled error responses.
- Keep uploaded originals separate from temporary processing files.
- Use maintained scientific parsing dependencies.
- Execute parsing and processing inside a restricted worker environment.
- Apply processing time, CPU, and memory limits.

### 12.3 Verification

- Test malformed, truncated, and unsupported FITS files.
- Test oversized uploads and invalid dimensions.
- Test filenames containing traversal patterns.
- Verify that rejected files cannot reach normal processing.
- Verify that worker resource limits are enforced.

## 13. Storage and Data Integrity Threats

SPHERITHM stores original scientific datasets, derived outputs, metadata, and audit information. Unauthorized modification, disclosure, or deletion can undermine scientific reliability.

### 13.1 Identified Threats

- Unauthorized reading or downloading of stored objects.
- Modification or replacement of original datasets.
- Accidental or malicious deletion of stored data.
- Inconsistency between database metadata and stored objects.
- Exposure of storage credentials.
- Unauthorized modification or deletion of audit and provenance records.

### 13.2 Planned Security Controls

- Restrict database and object-storage access using least-privilege identities.
- Separate original datasets from generated processing outputs.
- Calculate cryptographic checksums for uploaded files.
- Verify checksums when validating stored originals.
- Apply authorization before returning stored objects.
- Protect storage credentials and configuration.
- Record relevant storage and dataset operations in the audit system.
- Define backup and recovery expectations for important platform data.

### 13.3 Verification

- Test unauthorized object access and deletion.
- Verify checksum detection after controlled file modification.
- Test database and object-storage permission boundaries.
- Verify that dataset metadata references the correct stored object.
- Test audit protection against unauthorized modification.


## 14. Worker Isolation Threats

The Python processing worker handles untrusted scientific files and must operate with restricted permissions.

### 14.1 Identified Threats

- Worker compromise through vulnerable parsing dependencies.
- Unauthorized access to host files or unrelated datasets.
- Exposure of application credentials or storage secrets.
- Resource exhaustion through expensive processing jobs.
- Unauthorized communication with external services.
- Manipulation of processing outputs or job status.

### 14.2 Planned Security Controls

- Run workers using non-root identities.
- Restrict filesystem access to job-specific working directories.
- Avoid passing unnecessary application secrets to workers.
- Apply CPU, memory, process, and execution-time limits.
- Restrict network access to required destinations.
- Validate job parameters before execution.
- Validate worker outputs before making them available.
- Record job execution status and processing provenance.

### 14.3 Verification

- Inspect worker identity and filesystem permissions.
- Test access to prohibited directories.
- Verify resource limits using controlled processing workloads.
- Test invalid job parameters and worker failure handling.
- Verify that worker outputs are associated with the correct input dataset.

## 15. API and Application Threats

The ASP.NET Core API is the primary application boundary for authentication, authorization, dataset management, and processing requests.

### 15.1 Identified Threats

- Injection attacks through unvalidated request data.
- Broken object-level authorization.
- Unauthorized administrative operations.
- Abuse of upload and processing endpoints.
- Sensitive information exposed through error responses.
- Cross-origin configuration weaknesses.
- Vulnerable application dependencies.

### 15.2 Planned Security Controls

- Validate request data using server-side validation.
- Use parameterized database operations and safe data-access practices.
- Enforce authorization for every protected resource operation.
- Apply request size limits and rate limiting.
- Return controlled error responses without exposing internal details.
- Configure CORS with explicitly permitted origins.
- Maintain and review application dependencies.
- Protect API documentation and administrative endpoints according to deployment requirements.

### 15.3 Verification

- Test invalid and unexpected API inputs.
- Test authorization across user roles and tenants.
- Verify upload and request limits.
- Review error responses for sensitive information.
- Test configured CORS policies.
- Perform dependency vulnerability checks.

## 16. Audit and Provenance Threats

Audit records and processing provenance provide accountability and support investigation and scientific reproducibility.

### 16.1 Identified Threats

- Missing records for security-relevant operations.
- Unauthorized modification or deletion of audit records.
- Incorrect attribution of actions to users or services.
- Incomplete links between original datasets and processing outputs.
- Exposure of sensitive information through logs.
- Inconsistent timestamps or processing records.

### 16.2 Planned Security Controls

- Record relevant authentication, authorization, upload, deletion, and processing events.
- Associate events with authenticated user or service identities.
- Record timestamps and relevant resource identifiers.
- Avoid recording passwords, access tokens, or application secrets.
- Restrict access to audit records.
- Preserve relationships between input datasets, processing jobs, and outputs.
- Use consistent timestamp handling across platform components.

### 16.3 Verification

- Verify that required operations generate audit events.
- Test unauthorized access to audit records.
- Confirm that sensitive credentials are not logged.
- Verify dataset-to-job-to-output provenance relationships.
- Test audit behavior when application or worker operations fail.


## 17. Deployment and Operational Security

SPHERITHM deployment configurations must preserve the security controls defined in this threat model.

### 17.1 Deployment Risks

- Exposed services or ports that are not required.
- Default credentials or insecure deployment configurations.
- Secrets included in source code, container images, or deployment files.
- Containers running with unnecessary privileges.
- Unpatched operating system images or application dependencies.
- Inadequate monitoring of service failures and security events.

### 17.2 Planned Controls

- Keep deployment services and ports limited to required functionality.
- Use environment-specific configuration and protected secrets.
- Avoid committing credentials or private keys to version control.
- Use non-root container identities wherever practical.
- Maintain and update container images and dependencies.
- Provide health checks and operational logging.
- Document secure deployment settings and required configuration.

### 17.3 Verification

- Review exposed ports and service configuration.
- Scan repositories for accidental secret exposure.
- Review container permissions and configuration.
- Check dependency and container image vulnerabilities.
- Verify deployment instructions include security-relevant configuration.

## 18. Residual Risks

Security controls reduce risk but cannot eliminate every threat. The following residual risks remain subject to implementation, testing, and deployment conditions.

| Risk | Residual Concern | Planned Treatment |
|---|---|---|
| Scientific parser vulnerabilities | Valid files may exploit parser defects | Dependency maintenance, isolation, resource limits |
| Compromised user credentials | Attackers may operate using legitimate accounts | Token controls, authorization, monitoring |
| Infrastructure compromise | Host or container runtime vulnerabilities may affect services | Updates, hardening, restricted privileges |
| Insider misuse | Authorized users may misuse granted access | Least privilege and audit records |
| Resource exhaustion | Valid workloads may consume substantial resources | Quotas, limits, monitoring |
| Storage failure | Data loss or unavailability | Backups and recovery procedures |
| Dependency supply-chain risk | Third-party packages may be compromised | Dependency review and vulnerability scanning |
| Configuration mistakes | Deployment errors may expose services or secrets | Configuration review and deployment validation |

Residual risks must be reassessed before production deployment and whenever significant architecture or security changes occur.

## 19. Security Verification Plan

Security controls must be verified through implementation-level tests and reviews.

| Verification ID | Verification Activity | Target Area | Evidence |
|---|---|---|---|
| SV-001 | Authentication and invalid-token testing | Identity and API | Test results |
| SV-002 | Role and permission enforcement testing | Authorization | Test results |
| SV-003 | Cross-user and cross-tenant access testing | Dataset access | Integration tests |
| SV-004 | Malformed and oversized upload testing | File ingestion | Test results |
| SV-005 | Path traversal and filename handling testing | File storage | Test results |
| SV-006 | Checksum integrity testing | Original files | Test results |
| SV-007 | Worker isolation and resource limit testing | Processing worker | Test results |
| SV-008 | API input validation and error handling | ASP.NET Core API | Test results |
| SV-009 | Audit event and provenance verification | Audit system | Test results |
| SV-010 | Dependency vulnerability assessment | Dependencies and containers | Scan reports |
| SV-011 | Secret exposure review | Repository and configuration | Review evidence |
| SV-012 | Deployment security configuration review | Docker and Kubernetes | Review evidence |

Verification activities are considered complete only when the relevant controls are implemented, tested, and supported by recorded evidence. Planned controls must not be represented as implemented security capabilities.

## 20. Review and Document Status

### 20.1 Review Requirements

This threat model must be reviewed:

- Before the first production deployment.
- When new components, integrations, or processing capabilities are introduced.
- When authentication, authorization, storage, or worker isolation designs change.
- When significant vulnerabilities or security incidents affect the platform.
- During periodic security reviews.

### 20.2 Document Status

| Field | Value |
|---|---|
| Document ID | DOC-007 |
| Version | 0.1 |
| Status | Draft |
| Owner | Project Owner |
| Last Updated | 18-11-2026 |
| Review Status | Initial threat model; security controls require implementation and verification |

This document describes planned security controls and identified risks. It does not certify SPHERITHM as secure, and completion of this document alone does not establish that the listed controls have been implemented.


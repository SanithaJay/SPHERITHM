# SPHERITHIM — Architecture Decision Records (ADRs)

**Project:** SPHERITHIM — Secure Scientific Data Platform  
**Document ID:** DOC-008  
**Version:** 0.1  
**Status:** Draft  
**Created:** 18 November 2026  
**Owner:** Project Owner

---

## 1. Purpose

This document records the key architectural decisions made during SPHERITHIM development.

Each Architecture Decision Record (ADR) documents the decision, its context, alternatives considered, consequences, and security implications.

These decisions establish the planned technical direction. Implementation and validation will occur during subsequent development phases.

## 2. ADR Status Definitions

- **Proposed:** Under consideration and awaiting confirmation.
- **Accepted:** Selected architectural direction; implementation may still be pending.
- **Deprecated:** Previously accepted decision no longer recommended.
- **Superseded:** Replaced by a newer architectural decision.

---

## ADR-001: Backend Technology — ASP.NET Core with .NET 10

**Status:** Accepted  
**Date:** 18 November 2026

### Context

SPHERITHIM requires a backend platform to manage authentication, authorization, dataset ingestion, metadata, processing orchestration, audit records, and API communication.

The backend must support maintainable architecture, secure API development, automated testing, and integration with the selected storage and processing technologies.

### Decision

Use ASP.NET Core Web API with .NET 10 as the primary backend technology.

The backend will follow a layered architecture separating:

- API and presentation boundary
- Application/use-case logic
- Domain models and business rules
- Infrastructure and external integrations

### Alternatives Considered

- Node.js with TypeScript
- Python-based web backend
- Java with Spring Boot

### Consequences

- Strong typing and compile-time validation
- Structured dependency injection and configuration
- Support for authentication, authorization, middleware, and API development
- Clear separation of responsibilities and testability
- Requires familiarity with the .NET ecosystem

### Security Considerations

- Centralized authentication and authorization
- Input validation at API boundaries
- Secure configuration and secrets handling
- Consistent error handling
- Explicit authorization checks for dataset access

---

## ADR-002: Frontend Technology — React with TypeScript

**Status:** Accepted  
**Date:** 18 November 2026

### Context

SPHERITHIM requires a web interface for dataset management, validation status, metadata inspection, processing operations, results, and audit/provenance information.

The frontend must communicate with backend APIs and provide a maintainable interface for different user roles.

### Decision

Use React with TypeScript for the web dashboard.

The frontend will be structured into reusable components, application pages, API integration modules, and appropriate state management.

### Alternatives Considered

- Angular
- Vue.js
- Server-rendered Razor pages
- Plain JavaScript frontend

### Consequences

- Reusable component-based UI
- Type-safe frontend development
- Clear API integration boundaries
- Independent frontend and backend development
- Requires frontend build tooling and dependency management

### Security Considerations

- Avoid storing sensitive authentication tokens insecurely
- Enforce permissions on the backend, not only in the UI
- Validate data received from APIs
- Avoid rendering untrusted content unsafely
- Protect authenticated routes and sensitive actions

---

## ADR-003: Relational Database — MySQL

**Status:** Accepted  
**Date:** 18 November 2026

### Context

SPHERITHIM requires persistent structured data for user accounts, roles, dataset records, ownership, metadata, processing jobs, audit records, and related entities.

These entities require reliable relationships, constraints, and transactional operations.

### Decision

Use MySQL as the primary relational database.

Structured application data will be stored using relational tables, appropriate keys, constraints, indexes, and transactions.

### Alternatives Considered

- PostgreSQL
- SQL Server
- MongoDB as the primary application database

### Consequences

- Relational integrity for structured data
- Support for transactions and constraints
- Familiar SQL querying and data management
- Requires schema design and database migration management

### Security Considerations

- Least-privilege database accounts
- Parameterized database operations
- Restricted network exposure
- Secure credential management
- Backups and restoration planning
- Avoid unnecessary storage of sensitive information

---

## ADR-004: Scientific File Storage — S3-Compatible Object Storage

**Status:** Accepted  
**Date:** 18 November 2026

### Context

SPHERITHIM processes scientific datasets, including FITS files, which may be large and must be preserved independently of application metadata.

Original files, processing outputs, and derived artifacts require scalable storage without placing large file contents directly into relational database records.

### Decision

Use S3-compatible object storage.

MinIO will be used as the local development and self-hosted object storage implementation.

The relational database will store object references and associated metadata rather than the binary contents of large scientific files.

### Alternatives Considered

- Local filesystem storage
- Storing binary files directly in MySQL
- Cloud-specific object storage services

### Consequences

- Separation of metadata and file content
- Object-based storage for original files and derived outputs
- S3-compatible integration interface
- Easier migration to compatible object storage providers
- Requires object lifecycle, access, integrity, and backup management

### Security Considerations

- Private buckets by default
- Restrict object access through controlled application workflows
- Avoid publicly exposed object URLs
- Validate uploads before processing
- Preserve original files against unintended modification
- Apply least-privilege storage credentials

---

## ADR-005: Scientific Processing — Python with Astropy

**Status:** Accepted  
**Date:** 18 November 2026

### Context

SPHERITHIM requires scientific data validation and processing capabilities, particularly for astronomical FITS files.

Python provides an established scientific ecosystem and libraries for inspecting, validating, and processing astronomical data.

### Decision

Use Python for the scientific processing component.

Astropy will be the primary scientific library for FITS-related operations, including file inspection, metadata access, and supported scientific processing workflows.

The Python processing component will operate separately from the main ASP.NET Core API.

### Alternatives Considered

- Implementing all FITS processing in C#/.NET
- Using custom FITS parsing and processing code
- Using other Python scientific libraries without Astropy

### Consequences

- Access to established astronomy libraries
- Separation between application business logic and scientific computation
- Independent testing of scientific processing functionality
- Requires Python environment and dependency management
- Requires defined communication and data exchange contracts with the backend

### Security Considerations

- Treat scientific files as untrusted input
- Validate file structure and metadata
- Avoid unsafe parsing and unrestricted file access
- Pin and maintain processing dependencies
- Limit processing resources
- Never assume that a valid FITS file is automatically safe to process

---

## ADR-006: Processing Architecture — Isolated Scientific Worker

**Status:** Accepted  
**Date:** 18 November 2026

### Context

Scientific file processing may involve malformed inputs, expensive operations, memory consumption, long execution times, or failures in processing libraries.

Executing scientific processing directly inside the main API process would increase the impact of processing failures on the application.

### Decision

Use a separate scientific processing worker.

The ASP.NET Core backend will manage processing requests, validate authorization, coordinate job execution, and track job status.

The Python worker will perform the assigned scientific processing operations independently of the main API process.

The worker will be designed with explicit input/output contracts and resource restrictions.

### Alternatives Considered

- Processing directly inside API request handlers
- Running Python processing as unrestricted local scripts
- Using a single combined API and processing application

### Consequences

- Separation of API responsibilities and scientific computation
- Improved fault isolation
- Independent processing testability
- Better foundation for asynchronous processing
- Additional orchestration and communication complexity
- Requires worker lifecycle, job status, timeout, and failure handling

### Security Considerations

- Restrict worker filesystem access
- Limit CPU, memory, and execution time
- Avoid unrestricted network access
- Run under a non-privileged identity
- Validate job inputs and outputs
- Prevent processing jobs from accessing unrelated datasets
- Treat worker results as untrusted until validated

---

## 3. Decision Summary

| ADR | Decision | Selected Technology |
|---|---|---|
| ADR-001 | Backend | ASP.NET Core / .NET 10 |
| ADR-002 | Frontend | React / TypeScript |
| ADR-003 | Structured database | MySQL |
| ADR-004 | Scientific file storage | S3-compatible / MinIO |
| ADR-005 | Scientific processing | Python / Astropy |
| ADR-006 | Processing isolation | Separate scientific worker |

## 4. Related Documents

- DOC-003 — Product Vision
- DOC-004 — Requirements Specification
- DOC-005 — Requirements Traceability Matrix
- DOC-006 — System Architecture
- DOC-007 — Threat Model

## 5. Review and Change Management

Architecture decisions may be reviewed when implementation evidence, security requirements, performance measurements, or operational constraints justify a change.

Any material change to an accepted decision should be documented with its rationale, alternatives, impacts, and relevant references.

**Document Status:** Draft — Initial architectural decisions documented; implementation pending.

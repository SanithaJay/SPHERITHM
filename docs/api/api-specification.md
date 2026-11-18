# SPHERITHM API Specification

**Document ID:** DOC-009  
**Version:** 0.1  
**Status:** Draft  
**Last Updated:** 18 November 2026  
**Related Documents:** DOC-003 Product Vision, DOC-004 Requirements Specification, DOC-005 Requirements Traceability Matrix, DOC-006 System Architecture, DOC-007 Threat Model, DOC-008 Architecture Decision Records

---

## 1. Purpose

This document defines the initial HTTP API contract for SPHERITHM, a secure scientific data platform.

It describes the API conventions, authentication model, authorization expectations, endpoint groups, request and response structures, error handling, and security requirements.

This is a design specification. It does not claim that the endpoints have been implemented.

## 2. API Design Principles

The API shall follow these principles:

- Use REST-style resource-oriented endpoints.
- Use HTTPS for all non-local deployments.
- Use JSON for standard request and response bodies.
- Use explicit versioning through the `/api/v1` route prefix.
- Apply authentication and authorization to protected operations.
- Enforce tenant and resource ownership boundaries.
- Validate all client-supplied input.
- Return consistent error responses.
- Record security-relevant actions in the audit trail.
- Avoid exposing internal paths, secrets, stack traces, or sensitive implementation details.

## 3. Base URL and Versioning

Development base URL:

`https://localhost:{port}/api/v1`

Production base URL:

`https://{configured-host}/api/v1`

The actual host, port, and deployment configuration shall be defined during implementation and deployment.

Breaking API changes shall require a new major API version.

## 4. Authentication

Protected endpoints shall use bearer-token authentication.

### 4.1 Access Token

The client shall send the access token using:

`Authorization: Bearer <access-token>`

The API shall validate token signature, issuer, audience, expiry, and applicable claims.

### 4.2 Refresh Token

Refresh tokens shall be used to obtain replacement access tokens.

The implementation shall define secure storage, expiry, rotation, revocation, and replay protection.

### 4.3 Authentication Endpoints

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| POST | `/auth/login` | Authenticate a user | Anonymous |
| POST | `/auth/refresh` | Obtain a replacement access token | Refresh token |
| POST | `/auth/logout` | Revoke the current refresh session | Authenticated |

## 5. Authorization and Roles

The initial platform roles are:

| Role | Responsibility |
|---|---|
| Platform Administrator | Platform configuration and administrative operations |
| Data Curator | Dataset registration, upload, and curation |
| Research Analyst | Authorized dataset access and processing |
| Auditor | Read-only access to permitted audit and provenance records |

Authorization shall be enforced by the API, not solely by the frontend.

Every resource operation shall verify the caller's permissions and applicable tenant or ownership boundary.

The exact role-to-permission mapping shall be maintained alongside the implementation and requirements traceability matrix.

## 6. API Endpoint Catalogue

The following endpoints represent the initial planned API surface. HTTP methods and request contracts may be refined during implementation.

### 6.1 Health and Service Information

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| GET | `/health` | Report service health | Deployment-configured |
| GET | `/health/ready` | Report readiness | Deployment-configured |
| GET | `/health/live` | Report liveness | Deployment-configured |

Health responses shall not disclose secrets, connection strings, internal infrastructure details, or sensitive diagnostic information.

### 6.2 User and Access Management

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| GET | `/users/me` | Retrieve current user's profile | Authenticated |
| GET | `/users` | List users within authorized scope | Platform Administrator |
| GET | `/users/{userId}` | Retrieve an authorized user | Platform Administrator |
| PATCH | `/users/{userId}/status` | Change user status | Platform Administrator |
| GET | `/roles` | List available platform roles | Authenticated |

External identity integration, if enabled, shall be governed by the identity-provider configuration and applicable security requirements.

### 6.3 Dataset Management

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| POST | `/datasets` | Register dataset metadata | Data Curator |
| GET | `/datasets` | List authorized datasets | Authenticated |
| GET | `/datasets/{datasetId}` | Retrieve dataset details | Authorized user |
| PATCH | `/datasets/{datasetId}` | Update permitted dataset metadata | Data Curator |
| DELETE | `/datasets/{datasetId}` | Request dataset removal under retention policy | Authorized administrator |

Dataset listing and retrieval shall enforce tenant isolation and resource-level authorization.

Deletion shall not silently bypass retention, preservation, or audit requirements.

### 6.4 Dataset Upload and Validation

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| POST | `/datasets/{datasetId}/uploads` | Initiate or submit a dataset upload | Data Curator |
| GET | `/datasets/{datasetId}/uploads/{uploadId}` | Retrieve upload status | Authorized user |
| POST | `/datasets/{datasetId}/uploads/{uploadId}/validate` | Request dataset validation | Data Curator |
| GET | `/datasets/{datasetId}/validation` | Retrieve validation results | Authorized user |

The implementation shall define upload size limits, accepted content types, streaming behavior, checksum calculation, and temporary-storage handling.

Validation shall treat uploaded files as untrusted input.

### 6.5 Processing Jobs

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| POST | `/datasets/{datasetId}/jobs` | Submit an authorized processing job | Research Analyst |
| GET | `/jobs` | List authorized processing jobs | Authenticated |
| GET | `/jobs/{jobId}` | Retrieve job status and metadata | Authorized user |
| POST | `/jobs/{jobId}/cancel` | Request cancellation of a job | Authorized user |
| GET | `/jobs/{jobId}/results` | Retrieve available job results | Authorized user |

Processing shall be performed by an isolated worker component rather than directly within the API request process.

The API shall enforce job ownership, permitted operations, and resource limits.

### 6.6 Results and Provenance

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| GET | `/datasets/{datasetId}/results` | List authorized dataset results | Authorized user |
| GET | `/results/{resultId}` | Retrieve result metadata | Authorized user |
| GET | `/results/{resultId}/download` | Download an authorized result | Authorized user |
| GET | `/datasets/{datasetId}/provenance` | Retrieve dataset provenance | Authorized user |
| GET | `/jobs/{jobId}/provenance` | Retrieve processing provenance | Authorized user |

Provenance records shall identify relevant input datasets, processing parameters, software or worker version, timestamps, and output references where applicable.

### 6.7 Audit Records

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| GET | `/audit-events` | Search permitted audit events | Platform Administrator, Auditor |
| GET | `/audit-events/{eventId}` | Retrieve an authorized audit event | Platform Administrator, Auditor |

Audit records shall be protected against unauthorized modification and access.

Sensitive credentials, access tokens, and secret values shall not be recorded in audit payloads.

## 7. Standard Request Conventions

### 7.1 Content Type

JSON request bodies shall use:

`Content-Type: application/json`

File-upload endpoints may use an appropriate streaming or multipart content type as defined by their implementation contract.

### 7.2 Identifiers

Resource identifiers shall be opaque identifiers. Clients shall not rely on sequential identifiers or infer authorization from identifier possession.

### 7.3 Pagination

Collection endpoints shall support bounded pagination.

Initial query parameters:

| Parameter | Purpose |
|---|---|
| `page` | Requested page number |
| `pageSize` | Number of records requested |
| `sortBy` | Permitted sort field |
| `sortDirection` | `asc` or `desc` |

The API shall enforce a maximum page size.

### 7.4 Filtering

Only documented filter fields shall be accepted. Filtering shall not allow clients to bypass tenant, ownership, or role restrictions.

## 8. Standard Response Structures

### 8.1 Successful Resource Response

Example:

```json
{
  "data": {
    "id": "resource-id",
    "status": "available"
  },
  "requestId": "request-correlation-id"
}

### 8.3 Collection Response

Example:

```json
{
  "data": [],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 0,
    "totalPages": 0
  },
  "requestId": "request-correlation-id"
}
```

### 8.4 Error Response

Errors shall use a consistent structure based on the Problem Details standard.

Example:

```json
{
  "type": "about:blank",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more request fields are invalid.",
  "instance": "/api/v1/datasets",
  "traceId": "request-correlation-id"
}
```

Error responses shall not expose stack traces, internal filesystem paths, database details, secrets, or raw exception messages.

## 9. HTTP Status Codes

| Status | Meaning | Typical Use |
|---|---|---|
| 200 | OK | Successful retrieval or operation |
| 201 | Created | Resource created |
| 202 | Accepted | Asynchronous job accepted |
| 204 | No Content | Successful operation without response body |
| 400 | Bad Request | Invalid request |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Authenticated caller lacks permission |
| 404 | Not Found | Resource not found or not disclosed |
| 409 | Conflict | Resource state conflict |
| 413 | Content Too Large | Upload exceeds configured limit |
| 415 | Unsupported Media Type | Unsupported content type |
| 422 | Unprocessable Content | Semantically invalid input |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Unexpected server-side failure |
| 503 | Service Unavailable | Service or dependency unavailable |

The API shall use status codes consistently and avoid disclosing whether inaccessible resources exist when doing so would reveal sensitive information.

## 10. API Security Requirements

The API implementation shall address the following controls:

1. Enforce authentication on protected endpoints.
2. Enforce role-based and resource-level authorization.
3. Validate tenant ownership on every applicable resource operation.
4. Validate request bodies, query parameters, identifiers, and file metadata.
5. Apply upload size and request rate limits.
6. Restrict accepted file types and validate actual file content.
7. Prevent path traversal and unsafe file handling.
8. Avoid trusting client-supplied storage paths or object keys.
9. Protect original uploaded files from unauthorized modification.
10. Use secure token expiry, refresh, rotation, and revocation.
11. Apply appropriate CORS and security-header policies.
12. Avoid logging credentials, tokens, and sensitive file contents.
13. Record relevant security and administrative events.
14. Apply safe exception handling and consistent error responses.
15. Protect API documentation and administrative endpoints according to deployment policy.

## 11. File Upload and FITS Handling

FITS uploads shall be considered untrusted until validation is complete.

The implementation shall define:

- Maximum upload size and permitted file extensions.
- Actual file-format validation rather than extension-only checks.
- FITS header and structural validation.
- Dimension and metadata limits.
- Checksum generation and verification.
- Safe temporary-storage handling.
- Preservation of the original uploaded object.
- Controlled transfer to the isolated processing worker.
- Cleanup of failed or expired temporary uploads.

The API shall not execute uploaded content or load arbitrary client-supplied paths.

## 12. Asynchronous Processing Contract

Long-running scientific processing shall use asynchronous job submission.

A successful submission shall return an accepted response containing a job identifier and initial status.

Illustrative response:

```json
{
  "data": {
    "jobId": "job-id",
    "status": "queued"
  },
  "requestId": "request-correlation-id"
}
```

Initial job states:

- `queued`
- `running`
- `succeeded`
- `failed`
- `cancelled`

The implementation shall define valid state transitions, retry behavior, cancellation semantics, timeout handling, and worker-failure recovery.

## 13. Observability and Auditability

API operations shall support correlation through a request or trace identifier.

Operational telemetry shall capture relevant service health, latency, failures, and dependency status without exposing sensitive data.

Security-relevant actions shall generate audit events with, where applicable:

- Actor identifier.
- Action.
- Target resource.
- Timestamp.
- Outcome.
- Correlation identifier.

Audit events shall not contain passwords, bearer tokens, refresh tokens, or raw sensitive file contents.

## 14. Rate Limiting and Resource Protection

The API shall apply configurable rate limits and resource protections.

Higher-risk operations such as login, upload initiation, job submission, and administrative actions shall receive appropriate controls.

The implementation shall define limits based on deployment capacity and documented requirements.

## 15. API Documentation and Contract Testing

The implementation shall provide machine-readable API documentation, such as OpenAPI, for the implemented endpoints.

API contract tests shall verify:

- Route and HTTP method behavior.
- Request validation.
- Response schemas.
- Authentication and authorization.
- Tenant and ownership isolation.
- Error status and response formats.
- Upload constraints.
- Processing-job lifecycle behavior.

Documentation shall not describe planned endpoints as implemented.

## 16. Design Decisions and Open Items

The following items require confirmation during implementation:

1. Final access-token and refresh-token lifetimes.
2. Exact role-to-permission mapping.
3. Dataset and upload lifecycle states.
4. Upload streaming and multipart strategy.
5. Maximum upload size and FITS validation limits.
6. Pagination defaults and maximum page size.
7. Rate-limit thresholds.
8. Retention and deletion policy.
9. Exact OpenAPI generation and publication approach.
10. Whether real-time job updates will use polling or a later event-based interface.

These decisions shall be reconciled with the architecture, requirements, threat model, and ADR records before implementation is considered complete.

## 17. Document Status

This document defines the initial API design baseline for SPHERITHM.

All endpoints are planned contract definitions unless implementation and tests demonstrate otherwise.

Changes to endpoint behavior, security requirements, or resource ownership rules shall be reflected in this document and the requirements traceability matrix.

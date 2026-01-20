# Business Logic Documentation Outline

## 1. EXECUTIVE SUMMARY

### 1.1 System Purpose
Healthcare practice management platform providing appointment scheduling, billing, patient records, and clinical documentation with multi-tenant architecture.

### 1.2 Key Use Cases (top 5)
- Appointment scheduling and slot management across multiple practices and providers
- Patient registration and health insurance verification (self-pay, insurance, corporate)
- Clinical documentation with AI-assisted chief complaint analysis and ICD-10 coding
- Invoice generation, price calculation, and payment processing with sponsor management
- Multi-tenant organization and practice setup with NABIDH compliance

### 1.3 User Types/Actors
- Super Admin (SSA) - system-level administration across all tenants
- Admin - tenant-level organization and practice management
- Provider/Practitioner - doctors, staff performing clinical services
- Patient - individuals receiving healthcare services
- Guardian - managing care for dependents/minors
- Billing Staff - financial operations and claims management

## 2. DOMAIN MODEL & ENTITIES

### 2.1 Appointment
Scheduled healthcare encounter with status lifecycle, participants, slots, billing details, and recurring support.

### 2.2 Patient
Individual receiving care with demographics, identifications (Emirates ID), insurance/corporate sponsors, guardians, addresses, and health records.

### 2.3 Provider
Healthcare practitioner with licenses, qualifications, specialties, professional details, and availability schedules.

### 2.4 Practice
Healthcare facility/clinic with licensing, NABIDH authority, contact details, billing configuration, staff, and exam rooms.

### 2.5 Organization
Multi-tenant top-level entity containing practices, settings, and organizational hierarchy with role-based access.

### 2.6 Invoice
Financial document for services rendered with line items, diagnosis codes, sponsor allocation, payment tracking, and claim association.

### 2.7 Billing Profile
Provider/practice fee schedules, service pricing, billing groups, procedure codes, and agreement-based price calculations.

### 2.8 Slot
Available time blocks for appointment booking with status, specialty, room, and provider association.

### 2.9 Chief Complaint
Patient's primary concern with AI-powered NLP analysis, healthcare entity extraction, ICD-10 suggestions, and RAG search integration.

### 2.10 Health Insurance Sponsor
Insurance company/corporate agreement providing coverage with eligibility dates, policy numbers, and benefit structures.

### 2.11 Encounter
Clinical visit session with assessment plans, diagnoses (ICD-10), procedures, medications, radiology orders, and lab requests.

### 2.12 Tenant
Isolated organizational workspace with separate data, users, and configuration in multi-tenant architecture.

### 2.13 User Identity
Authentication entity with roles, permissions, claims, organization associations, and tenant context.

### 2.14 Audit Log
Immutable record of entity changes (create/update/delete) with user, tenant, IP, masked sensitive fields.

### 2.15 Claim
Insurance/sponsor reimbursement request with submission status, remittance advice, and invoice linkage.

## 3. CORE BUSINESS FLOWS

### 3.1 Patient Registration Flow
New patient onboarding with demographics, identification validation, insurance/corporate verification, guardian assignment for minors, address capture.

### 3.2 Appointment Booking Flow
Slot search by provider/specialty/date, patient selection, sponsor identification, service selection, price calculation, booking confirmation, notification.

### 3.3 Patient Check-In Flow
Appointment status transition from booked to arrived/checked-in, eligibility verification, payment collection, flag display.

### 3.4 Clinical Documentation Flow
Encounter creation, chief complaint entry with AI analysis, assessment plan with diagnoses, procedure/medication orders, clinical note generation.

### 3.5 Invoice Generation Flow
Service capture from appointment/encounter, price calculation based on billing profile and sponsor, line item creation, diagnosis pointer mapping.

### 3.6 Payment Processing Flow
Invoice review, payment collection (multiple methods), receipt generation, outstanding balance tracking, sponsor vs patient allocation.

### 3.7 Claim Submission Flow
Invoice finalization, claim generation with standard codes, submission to sponsor/payer, status tracking, remittance processing.

### 3.8 Practice Setup Flow
Organization creation, practice registration with licensing and NABIDH authority, staff/provider onboarding, billing configuration, room setup.

### 3.9 Provider Onboarding Flow
Professional detail capture, license validation, qualification verification, specialty assignment, appointment profile creation, schedule configuration.

### 3.10 Appointment Cancellation Flow
Cancel request with reason code, status update, notification to participants, slot release, invoice adjustment or void.

### 3.11 Recurring Appointment Management
Series creation with recurrence rules, bulk slot reservation, individual instance modification, series-wide updates.

### 3.12 AI Chief Complaint Analysis Flow
Free-text complaint input, NLP entity extraction (symptoms, conditions, anatomical sites), RAG search for similar cases, ICD-10 suggestions.

### 3.13 Multi-Tenant User Access Flow
Tenant identification from claims, organization context resolution, role-based permission check, data isolation enforcement.

### 3.14 Price Calculation Flow
Service selection, sponsor identification, billing profile lookup, agreement-based pricing, discount application, tax calculation.

### 3.15 Patient Merge Flow
Duplicate detection, merge initiation, data consolidation, appointment/invoice reassignment, audit trail preservation.

## 4. DATA PROCESSING

### 4.1 Appointment Slot Generation
Schedule parsing, slot creation based on appointment profiles, availability windows, duration configuration, room/provider assignment.

### 4.2 Billing Price Calculation
Multi-factor pricing: billing profile + location + provider + sponsor agreement + service type + discount rules.

### 4.3 Healthcare Entity Extraction
NLP processing of chief complaints to identify symptoms, conditions, anatomical sites, severity, duration, temporal context.

### 4.4 Vector Embedding Search
Chief complaint document indexing in Azure Cognitive Search with 1536-dimension embeddings for semantic similarity.

### 4.5 Audit Log Processing
Change detection in Entity Framework, property masking for sensitive fields, Dapr pub/sub publishing to audit service.

### 4.6 Invoice Line Item Calculation
Service pricing lookup, quantity/units processing, diagnosis pointer assignment, patient vs sponsor share allocation.

### 4.7 Eligibility Date Validation
Sponsor coverage verification based on appointment date, policy period, benefit expiration checks.

### 4.8 Appointment Status Transitions
Lifecycle management: Proposed → Booked → Confirmed → CheckedIn → Arrived → Seen → Fulfilled → Completed/Cancelled.

### 4.9 Recurring Appointment Expansion
Pattern parsing (daily/weekly/monthly), date range calculation, exception handling, instance creation.

### 4.10 Multi-Database Support
Connection string management for SQL Server, PostgreSQL, MySQL with provider-specific configurations.

## 5. EXTERNAL INTEGRATIONS

### 5.1 NABIDH Integration
UAE National Backbone for Integrated Dubai Health - practice registration with assigning authority for patient identification.

### 5.2 Azure Cognitive Search Integration
Vector search for chief complaint analysis, semantic retrieval, ICD-10 code suggestions from indexed medical knowledge.

### 5.3 Dapr Pub/Sub Integration
Event publishing for audit logs, domain events (TenantCreated, PracticeCreated, OrganizationCreated), inter-service messaging.

### 5.4 Microservice HTTP APIs
11 Refit-based clients for: Billing, EmailSender, Identity, Master, Notifications, Organization, Patient, Person, Practice, Provider, Scheduling.

### 5.5 gRPC Service Communication
9 protobuf definitions for high-performance inter-service calls across appointment, billing, identity, patient, provider domains.

### 5.6 Email Notification Service
Appointment reminders, invoice delivery, password resets, system notifications via IEmailSenderApi.

### 5.7 Notification Service
Real-time alerts, job notifications, stats updates via INotificationsApi and SignalR patterns.

### 5.8 File Storage Service
Document upload/retrieval for patient photos, ID cards, practice stamps, organization images.

### 5.9 Master Data Service
Lookup values for gender, language, marital status, countries, medical codes via IMasterApi.

## 6. HEALTHCARE COMPLIANCE

### 6.1 PHI Handling
Protected Health Information masking in audit logs via MaskValueAuditAttribute, encryption at rest, secure transmission.

### 6.2 Audit Requirements
Comprehensive entity change tracking (create/update/delete) with user, tenant, IP address, timestamp for compliance reporting.

### 6.3 Data Retention
Soft delete pattern via ISoftDelete, archived status tracking, tenant data isolation, historical record preservation.

### 6.4 NABIDH Compliance
UAE healthcare standard integration for practice registration, patient identification, data exchange requirements.

### 6.5 Role-Based Access Control
Granular permissions (View, Create, Update, Delete) per resource, tenant-scoped data access, organization role claims.

### 6.6 Emirates ID Validation
Patient identity verification using UAE national ID system, card data capture (front/back images).

### 6.7 Medical Coding Standards
ICD-10 for diagnoses, SNOMED for clinical findings, CPT/procedure codes for billing, standard terminology systems.

## 7. ERROR HANDLING

### 7.1 Common Error Patterns
- CustomException with status codes (400/401/403/404/500)
- NotFoundException for missing entities
- UnauthorizedException for authentication failures
- ForbiddenException for permission denials
- ConflictException for duplicate/constraint violations
- InternalServerException for system errors

### 7.2 Business Rules
- Appointment cannot be booked on occupied slots
- Patient must have valid sponsor for appointment
- Provider must have active license for practice
- Invoice requires at least one service line item
- Eligibility dates must cover appointment date
- Minor patients require guardian assignment
- Audit log entries are immutable
- Tenant isolation must be enforced
- Recurring appointment series maintains referential integrity
- Price calculation requires valid billing profile

### 7.3 Validation Patterns
FluentValidation for request DTOs, required field enforcement, data type constraints, business rule validation at application layer.

### 7.4 Exception Middleware
Centralized exception handling with error ID generation, structured logging with Serilog, user context capture, localized messages.

### 7.5 Error Response Format
Standardized ErrorResult with source, exception message, error ID, support message, status code, message collection.

## 8. TECHNICAL DEBT

### 8.1 Current Limitations
- Cache implementation abstraction defined but not implemented in this codebase
- Malaffi healthcare integration not found (NABIDH present)
- Mixed gRPC and HTTP API patterns for inter-service communication
- .NET 7.0 approaching end-of-support lifecycle
- No distributed transaction coordination visible
- File storage implementation not in core library

### 8.2 Known Issues
- Multiple proto file locations (root Protos/ and Scrips.Core/Protos/)
- InvoiceDto complexity with nested medical order structures
- Potential N+1 queries in appointment participant loading
- Audit log publishing without failure handling strategy
- Connection string security relies on manual configuration
- No retry policies visible for external service calls

### 8.3 Architectural Considerations
- Repository pattern interfaces defined but implementations in separate projects
- Event notification handlers defined but consumer implementations external
- Job service abstraction without visible background processor
- Specification pattern usage limited to filtering scenarios
- Multi-database support adds complexity without clear switching strategy

### 8.4 Documentation Gaps
- API versioning strategy (VersionedApiController) without version routing details
- Tenant resolution mechanism not fully documented
- Permission enforcement at controller level not visible
- MediatR command/query handlers not in core library
- Domain event publication timing unclear

# Patient Management & Demographics - Complete Documentation

## OVERVIEW

**Purpose:** Manage patient registration, demographics, identification, insurance eligibility, guardian relationships, and contact information

**Key Entities:** Patient (EditPatientResponse), Guardian, HealthInsuranceSponsor, PatientCorporateSponsor, EmiratesIdCardData, PatientAddress

**Key Workflows:** Patient registration, insurance eligibility verification, guardian management, patient merge, demographics updates

**PHI Scope:** YES - Contains ALL patient demographics (name, DOB, Emirates ID, email, phone, address, MRN, insurance)

---

## ENTITIES

### EditPatientResponse

**Location:** Models/Patient/EditPatientResponse.cs:6

**Purpose:** Complete patient demographic and identification data with insurance, guardians, and emergency contacts

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| PatientId | string | Unique patient identifier | NO |
| FirstName | string | Patient given name | **YES** |
| MiddleName | string | Patient middle name | **YES** |
| LastName | string | Patient family name | **YES** |
| BirthDate | DateTime? | Date of birth | **YES** |
| Age | string | Calculated age | **YES** |
| Gender | string | Gender ID (lookup) | **YES** |
| GenderName | MasterResponse | Gender display | **YES** |
| Email | string | Email address | **YES** |
| PhoneNumber | string | Primary phone | **YES** |
| MaritalStatus | string | Marital status ID | **YES** |
| Language | string | Preferred language | NO |
| LanguageName | MasterResponse | Language display | NO |
| TimeZone | string | Patient timezone | NO |
| PhotoUrl | string | Patient photo | **YES** |
| PatientMrn | string | Medical Record Number | **YES** |
| ExternalMRN | string | MRN from external system | **YES** |
| IsAdult | bool | Adult vs minor classification | NO |
| OrganizationId | string | Multi-tenant organization | NO |
| PracticeId | string | Primary care practice | NO |
| Identification | UpdateIdenitification | Emirates ID, passport, etc. | **YES** |
| GuardianIdenitification | UpdateGuardianIdenitification | Guardian ID for minors | **YES** |
| IdFrontImage | string | ID card front photo | **YES** |
| IdBackImage | string | ID card back photo | **YES** |
| FrontIdCardUrl | string | ID card URL | **YES** |
| BackIdCardUrl | string | ID card URL | **YES** |
| UpdatePatientContactRequest | UpdatePatientContactRequest | Addresses | **YES** |
| EmergencyContactResponse | List | Emergency contacts | **YES** |
| HealthInsuranceResponse | List | Insurance policies | **YES** |
| CorporateAgreementResponse | List | Corporate health benefits | **YES** |
| ChildPatients | List\<PatientDetail\> | Dependents (guardian-child) | **YES** |
| ParentPatient | PatientDetail | Parent (child-guardian) | **YES** |
| PatientEducation | PatientEducationResponse | Education level | **YES** |
| PatientOccupation | PatientOccupationResponse | Employment | **YES** |
| IsActive | bool | Active patient status | NO |
| IsMerged | bool | Duplicate merged into another | NO |
| IsArchived | bool? | Soft delete flag | NO |
| IsMissingData | bool? | Incomplete registration | NO |
| LastUpdated | DateTime | Audit timestamp | NO |
| UpdatedBy | string | Audit user | NO |

**Relationships:**
- Practice: Many patients to one practice
- Organization: Many patients to one organization
- HealthInsurance: One patient to many insurance policies
- CorporateAgreement: One patient to many corporate sponsors
- Guardian: Minor patients to one guardian (parent)
- EmergencyContact: One patient to many emergency contacts
- ChildPatients: One guardian to many child patients

---

### HealthInsuranceResponse

**Location:** Models/Patient/HealthInsuranceResponse.cs (referenced in EditPatientResponse.cs:46)

**Purpose:** Insurance policy coverage details with eligibility dates and member identification

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Inferred structure) | | | |
| InsuranceId | Guid | Insurance sponsor ID | NO |
| PolicyNumber | string | Policy number | **YES** |
| MemberID | string | Member identification | **YES** |
| GroupNumber | string | Group/employer ID | **YES** |
| EffectiveDate | DateTime | Coverage start | **YES** |
| ExpirationDate | DateTime | Coverage end | **YES** |
| Relationship | string | Subscriber/Dependent | **YES** |
| IsPrimary | bool | Primary vs secondary | NO |

**Relationships:**
- Patient: Many insurance policies to one patient
- InsuranceSponsor: Many policies to one insurance company
- Appointment: Policy referenced during booking for pricing

---

### PatientCorporateResponse

**Location:** Models/Patient/PatientCorporateResponse.cs (referenced in EditPatientResponse.cs:47)

**Purpose:** Corporate employee health benefit agreement details

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Inferred structure) | | | |
| CorporateId | Guid | Corporate sponsor ID | NO |
| EmployeeID | string | Company employee number | **YES** |
| Department | string | Employee department | **YES** |
| AgreementEffectiveDate | DateTime | Benefit start date | **YES** |
| AgreementExpirationDate | DateTime | Benefit end date | **YES** |

**Relationships:**
- Patient: Many corporate agreements to one patient
- CorporateSponsor: Many agreements to one corporate entity

---

### GuardianDto

**Location:** Models/Patient/GuardianDto.cs (referenced in IPatientApi.cs:28)

**Purpose:** Legal representative for minor patients with relationship and consent

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Inferred structure) | | | |
| GuardianUserId | Guid | Guardian user ID | NO |
| Relationship | string | Parent/Legal Guardian | **YES** |
| PatientId | Guid | Minor patient ID | **YES** |
| ConsentProvided | bool | Consent for treatment | **YES** |
| ConsentDate | DateTime | When consent given | NO |

**Relationships:**
- Patient: One guardian to many child patients
- User: One user account to guardian role

---

### EmiratesIdCardData

**Location:** Models/Identity/EmiratesIdCardData.cs (referenced in outline)

**Purpose:** UAE national identification card structure for validation

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Inferred structure) | | | |
| EmiratesIDNumber | string | 15-digit Emirates ID | **YES** |
| FullName | string | Name from ID card | **YES** |
| Nationality | string | Citizen country | **YES** |
| DateOfBirth | DateTime | DOB from ID | **YES** |
| Gender | string | Gender from ID | **YES** |
| IssueDate | DateTime | ID issue date | NO |
| ExpiryDate | DateTime | ID expiration | NO |

**Relationships:**
- Patient: One-to-one with patient identification

**Compliance:** MANDATORY for UAE patients, validation format: XXX-XXXX-XXXXXXX-X

---

### PatientAddressListResponse

**Location:** Referenced in EditPatientResponse (UpdatePatientContactRequest)

**Purpose:** Patient contact addresses for billing and correspondence

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| AddressLine1 | string | Street address | **YES** |
| AddressLine2 | string | Apartment/unit | **YES** |
| City | string | City | **YES** |
| State | string | State/emirate | **YES** |
| PostalCode | string | ZIP/postal code | **YES** |
| Country | string | Country | **YES** |
| AddressType | string | Home/Work/Billing | NO |

**Relationships:**
- Patient: One patient to many addresses

---

## WORKFLOWS

### Workflow 1: Patient Registration

**Entry Point:** IPatientApi.cs:8 (POST assumed for create)

**Trigger:** New patient enrollment at practice

**Steps:**

1. **Capture Demographics** - FirstName, LastName, DOB, Gender, Phone, Email
   - File: EditPatientResponse.cs:21-30

2. **Emirates ID Validation** - Parse and validate 15-digit format
   - Format: XXX-XXXX-XXXXXXX-X
   - File: EmiratesIdCardData.cs (referenced)
   - Validation: Checksum, expiration date

3. **Minor Patient Check** - If age < 18, require guardian
   - IsAdult flag = false
   - File: EditPatientResponse.cs:17

4. **Guardian Assignment** - GET /api/Patients/GetGuardianDetails?userId={id}
   - Link parent/guardian to minor patient
   - File: IPatientApi.cs:28

5. **Insurance Verification** - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}
   - Validate insurance eligibility
   - File: IPatientApi.cs:16

6. **Corporate Coverage Check** - GET /api/Patients/{patientId}/PatientCorporateSponsor/{sponsorId}
   - If employee, verify corporate benefit
   - File: IPatientApi.cs:22

7. **Address Entry** - Capture home, work, billing addresses
   - File: UpdatePatientContactRequest in EditPatientResponse.cs:44

8. **Generate MRN** - Assign unique Medical Record Number
   - PatientMrn field
   - File: EditPatientResponse.cs:38

9. **Create User Account** - Link to Identity Service
   - For patient portal access
   - File: IIdentityApi.cs (user creation)

**Error Handling:**
- Duplicate Emirates ID: Check for existing patient, prompt for merge
- Invalid insurance: Allow registration, flag for billing follow-up
- Minor without guardian: Block registration until guardian assigned
- Missing required fields: Return 400 Bad Request with field list

**Healthcare Compliance:**
- **PHI Created:** ALL patient demographics
- **Audit Logged:** YES - Patient creation logged via AuditableBaseDbContext.cs:25
- **Consent:** Required for minor patients (guardian consent), data sharing consent
- **Retention:** 10 years from last encounter

---

### Workflow 2: Insurance Eligibility Verification

**Entry Point:** IPatientApi.cs:16

**Trigger:** Appointment booking, check-in, or manual verification

**Steps:**

1. **Select Sponsor** - Patient chooses insurance or corporate sponsor
   - HealthInsuranceResponse vs PatientCorporateResponse

2. **Policy Lookup** - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}
   - Input: PatientId, SponsorId
   - Output: HealthInsuranceResponse
   - File: IPatientApi.cs:16

3. **Eligibility Date Validation** - Check EffectiveDate ≤ Today ≤ ExpirationDate
   - Active vs expired coverage

4. **Benefit Structure Retrieval** - Coverage limits, copay amounts
   - Used for price calculation

5. **Primary vs Secondary** - IsPrimary flag determines billing order
   - Primary insurance billed first, secondary for remaining balance

**Error Handling:**
- Insurance not found: Return 404 Not Found
- Expired coverage: Return 400 Bad Request with expiration date
- Eligibility service timeout: Allow booking, manual verification later

**Healthcare Compliance:**
- **PHI Accessed:** Patient ID, policy number, member ID
- **Audit Logged:** YES - Insurance access logged
- **Real-Time Verification:** Not implemented (would require external eligibility service integration)

---

### Workflow 3: Guardian Management for Minors

**Entry Point:** IPatientApi.cs:28

**Trigger:** Minor patient registration or guardian update

**Steps:**

1. **Minor Patient Identification** - IsAdult = false (age < 18)
   - File: EditPatientResponse.cs:17

2. **Guardian User Selection** - Search for existing user or create new
   - GET /api/Patients/GetGuardianDetails?userId={id}
   - File: IPatientApi.cs:28

3. **Relationship Assignment** - Parent, Legal Guardian, Other
   - GuardianDto relationship field

4. **Consent Capture** - Guardian provides consent for treatment
   - ConsentProvided = true, ConsentDate = now

5. **Link Child Patients** - ChildPatients list on guardian record
   - One guardian to many children
   - File: EditPatientResponse.cs:51

6. **Portal Access** - Guardian can access child patients' records
   - Permission check in consuming service

**Error Handling:**
- Adult patient assigned guardian: Return 400 Bad Request
- Guardian not found: Create new user account
- Multiple guardians: Flag for manual review (custody arrangements)

**Healthcare Compliance:**
- **PHI Accessed:** Minor patient data, guardian relationship
- **Audit Logged:** YES - Guardian assignment logged
- **Consent Requirements:** MANDATORY for treatment, data sharing
- **Age Threshold:** 18 years (UAE standard)

---

### Workflow 4: Patient Merge (Duplicate Resolution)

**Entry Point:** Inferred from IsMerged field (EditPatientResponse.cs:48)

**Trigger:** Duplicate patient records identified

**Steps:**

1. **Duplicate Detection** - Match on Emirates ID, name+DOB, phone
   - Fuzzy matching algorithm (not visible in DTO library)

2. **Manual Review** - Staff confirms duplicates are same person
   - Compare photos, demographics, addresses

3. **Select Primary Record** - Determine which patient ID to keep
   - Usually first registration or most complete record

4. **Data Consolidation:**
   - Merge addresses, insurance policies, emergency contacts
   - Link all appointments, invoices, encounters to primary patient
   - Preserve all historical data

5. **Mark Duplicate** - IsMerged = true on duplicate record
   - File: EditPatientResponse.cs:48

6. **Redirect References** - Update all foreign keys
   - Appointments, invoices, prescriptions point to primary patient

7. **Archive Duplicate** - IsArchived = true
   - File: EditPatientResponse.cs:49
   - Cannot delete (audit trail requirement)

**Error Handling:**
- Records not actually duplicates: Unmerge operation required
- Conflicting data: Staff chooses correct value per field
- Active appointments: Reassign to primary patient before merge

**Healthcare Compliance:**
- **PHI Accessed:** Both patient records
- **Audit Logged:** YES - Merge operation logged with both patient IDs
- **Data Integrity:** ALL historical data must be preserved
- **Reversibility:** Merge should be reversible if error

---

## INTEGRATIONS

(Brief - details in 06b-integrations-details.md)

**Patient Service (IPatientApi.cs:6-30):**
- Patient CRUD, insurance, guardians, addresses
- 5 endpoints: GetPatient, PatientAddressList, HealthInsuranceSponsor, PatientCorporateSponsor, GetGuardianDetails
- Purpose: Core patient data management

**Billing Service (IBillingApi.cs:29):**
- Patient balance inquiry
- Endpoint: AvailableBalance (GET)
- Purpose: Outstanding balance for check-in

**Identity Service (IIdentityApi.cs:8):**
- User-patient linkage for portal access
- Endpoint: ContactDetails (POST)
- Purpose: User demographics and authentication

---

## COMPLIANCE SUMMARY

**PHI Fields in This Domain:**
- **Identification:** FirstName, LastName, DOB, Emirates ID, MRN, ExternalMRN, PhotoUrl, ID card images
- **Contact:** Email, PhoneNumber, Address (all address fields)
- **Demographics:** Gender, MaritalStatus, Age
- **Relationships:** Guardian, ChildPatients, EmergencyContacts
- **Financial:** Insurance policy number, member ID, employee ID
- **Clinical Context:** PatientEducation, PatientOccupation (social determinants of health)

**Audit Coverage:**
- ✅ Patient Create/Update/Delete - Logged via AuditableBaseDbContext.cs:25
- ✅ Insurance access - Logged
- ✅ Guardian assignment - Logged
- ✅ Patient merge - Logged with both patient IDs
- ⚠️ Address access - May not be explicitly logged (read operations)
- ⚠️ Photo view - Should be logged but verify in consuming service

**PHI Masking Requirements:**
- **`[MaskValueAudit]` REQUIRED on:**
  - FirstName, LastName, Email, PhoneNumber
  - Emirates ID, MRN, PolicyNumber, MemberID
  - Address fields
- **File:** MaskValueAuditAttribute.cs:13
- **Verification:** Audit Section 6.1 found this was NOT consistently applied (critical issue)

**Access Controls:**
- Multi-tenancy: OrganizationId, PracticeId enforce tenant isolation
- Provider access: Can view patients assigned to their practice
- Patient access: Can view only own record (via user-patient link)
- Guardian access: Can view child patients only
- Staff access: Can view all patients within practice
- **Role-based:** ScripsPermissions.cs defines granular permissions

**Retention:**
- Patient records: 10 years from last encounter (UAE healthcare standard)
- Emirates ID: 7 years (regulatory requirement)
- Insurance history: 7 years (financial records)
- Cannot delete patients: Use IsArchived soft delete (ISoftDelete.cs:5)

**Consent Requirements:**
- **Treatment consent:** Required for all services (guardian for minors)
- **Data sharing consent:** Required for NABIDH health information exchange
- **Marketing consent:** Optional for promotional communications
- **Research consent:** Optional for anonymized research participation

---

## CRITICAL FINDINGS

**Risk #1: MaskValueAudit Attribute Not Applied**
- Audit found `[MaskValueAudit]` not consistently applied to patient DTOs
- Impact: CRITICAL - PHI logged in plain text in audit logs
- Evidence: Section 6.1 audit, Technical Debt #4
- Mitigation: Add attributes to ALL PHI fields, verify with unit tests

**Risk #2: No Consent Tracking Found**
- GuardianDto references consent but no consent management workflow visible
- Impact: HIGH - Cannot prove consent obtained (HIPAA 164.508)
- Mitigation: Implement consent tracking module

**Risk #3: Patient Merge Reversibility**
- IsMerged flag but no unmerge workflow visible
- Impact: MEDIUM - Cannot correct merge errors easily
- Mitigation: Design unmerge workflow with data restoration

**Risk #4: Emirates ID Validation Not Enforced**
- EmiratesIdCardData structure exists but validation not visible in DTO library
- Impact: MEDIUM - Invalid IDs may be stored
- Mitigation: Implement server-side Emirates ID format and checksum validation

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)

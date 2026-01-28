---
## QUALITY AUDIT RESULTS
**Audit Date:** January 21, 2026  
**Auditor:** Andrew (Senior Engineer)  
**Score:** 85/100  
**Status:** ✅ PASS

### Summary
| Phase | Status | Issues |
|-------|--------|--------|
| Accuracy | ✅ PASS | 1 minor (field format) |
| Completeness | ⚠️ MARGINAL | Missing complete field tables |
| Healthcare | ✅ PASS | Good PHI documentation |
| Business Logic | ✅ PASS | Workflows clear |
| Consistency | ✅ PASS | Good alignment |

---

## PHASE 1: ACCURACY VALIDATION

### Code Reference Verification (Sample of 8)

| Documented Claim | Code Location | Verified? |
|------------------|---------------|-----------|
| InvoiceDetailsDto at line 3 | Models/Billing/InvoiceDetailsDto.cs:3 | ✅ YES |
| IBillingApi.cs:14 (CalculatePrice) | HttpApiClients/IBillingApi.cs:14 | ✅ YES |
| IBillingApi.cs:20 (PreCalculatePrice) | HttpApiClients/IBillingApi.cs:20 | ✅ YES |
| IBillingApi.cs:26 (Invoice) | HttpApiClients/IBillingApi.cs:26 | ✅ YES |
| IBillingApi.cs:29 (AvailableBalance) | HttpApiClients/IBillingApi.cs:29 | ✅ YES |
| InvoiceDetailsDto fields | InvoiceDetailsDto.cs:5-38 | ✅ YES |
| AuditableBaseDbContext.cs:25 | BaseDbContext/AuditableBaseDbContext.cs:25 | ✅ YES |
| IEmailSenderApi.cs:8 | HttpApiClients/IEmailSenderApi.cs:8 | ✅ YES |

**Accuracy Score:** 8/8 (100%)

**Field Format Note:** Document uses "Key Fields (PHI)" narrative format (lines 23-30) instead of complete table like other sections. Fields listed are accurate but format is inconsistent.

---

## PHASE 2: COMPLETENESS CHECK

**Field Coverage:** 70% - Document lists key fields but not in complete table format

**Missing from InvoiceDetailsDto (lines 10-24, 32-39):**
- ServiceIncluded, ServiceExcluded, ServiceNeedsApproval (bool flags)
- ServicePlacedBy, CreatedById, UpdatedById (audit fields)
- CreatedOn, UpdatedOn (timestamps)
- DenialReason, AmountToResubmit (claim denial fields)
- Payment, Outstanding (payment tracking)
- BillingGroupSystem, BillingGroupCategory (categorization)
- IsIndividualAdded (flag)

**All Workflows:** Well documented (4 workflows)

---

## PHASE 3: HEALTHCARE COMPLIANCE

### PHI Fields Documented

| PHI Field | Location | Documented? | Audit? |
|-----------|----------|-------------|--------|
| InvoiceId (links to patient) | InvoiceDetailsDto.cs:6 | ✅ YES (line 24) | ✅ YES |
| ServiceAddedDate | InvoiceDetailsDto.cs:8 | ✅ YES (line 24) | ✅ YES |
| DXPointer, DxPointers (diagnoses) | InvoiceDetailsDto.cs:25,29 | ✅ YES (line 25) | ✅ YES |
| GrossAmount, NetAmount, etc. (financial PHI) | InvoiceDetailsDto.cs:14,17-19 | ✅ YES (line 26) | ✅ YES |

**PHI Documentation:** ✅ Excellent - All financial PHI identified (line 161)

**Audit Coverage:**
- ✅ Invoice create/update logged (line 163)
- ✅ Payment operations logged
- ✅ 7-year retention documented (line 165)
- ⚠️ Missing: Encryption requirements for financial data

**Compliance Standards:**
- ✅ ICD-10 diagnosis codes (line 169)
- ✅ CPT/HCPCS procedure codes (line 169)
- ✅ X12 EDI for claims (line 138)

---

## PHASE 4: BUSINESS LOGIC VALIDATION

**Workflows:** 4/4 well documented
- ✅ Multi-factor price calculation (lines 71-91)
- ✅ Invoice generation (lines 94-108)
- ✅ Payment processing (lines 111-123)
- ✅ Claim submission (lines 126-139) - correctly noted as inferred (line 128)

**Pricing Logic:** Multi-factor pricing correctly documented (service + sponsor + provider + practice)

---

## PHASE 5: CROSS-DOCUMENT CONSISTENCY

- ✅ P1 outline match: "3. Billing, Invoicing & Claims"
- ✅ P6B integration references verified (though file is now `06-integration-details.md`)
- ⚠️ File reference at line 143: Should be `06-integration-details.md` not `06b-integration-details.md`

---

## CRITICAL ISSUES

1. **Incomplete field table (MEDIUM)** - Lines 23-30 use narrative format instead of complete field table
   - Fix: Add complete field table for InvoiceDetailsDto with all 39 properties (4h)

2. **File reference outdated (LOW)** - References to "06b-integration-details.md" should be "06-integration-details.md"
   - Fix: Update integration file references (10 minutes)

3. **Missing encryption documentation (MEDIUM - Healthcare)** - No mention of encryption for financial PHI
   - Fix: Add encryption requirements (Always Encrypted for amounts, TLS for transmission) (1h)

---

## HEALTHCARE COMPLIANCE GAPS

1. **Encryption requirements** - Financial PHI should specify encryption at rest and in transit
2. **Claim denial handling** - DenialReason field exists but workflow not documented
3. **Payment tracking** - Payment/Outstanding fields exist but reconciliation process not documented

---

## RECOMMENDED ACTIONS

**Immediate (Week 1):**
1. ✅ Add complete InvoiceDetailsDto field table with PHI markers (4h)
2. ✅ Update file references to `06-integration-details.md` (10min)

**This Sprint:**
3. ⚠️ Add encryption requirements for financial PHI (1h)
4. ⚠️ Document claim denial workflow (DenialReason, AmountToResubmit) (2h)

**Next Sprint:**
5. 📋 Document payment reconciliation process (Payment, Outstanding fields) (4h)

---

**Audit Conclusion:** Document PASSES (85/100) with good PHI identification and workflow documentation. Primary gap is incomplete field table format. Recommended to complete field documentation for consistency with other sections.

---
---

# Billing, Invoicing & Claims - Complete Documentation

## OVERVIEW

**Purpose:** Manage pricing, invoice generation, payment processing, and claim submission for healthcare services

**Key Entities:** Invoice, BillingProfile, ProcedureCode, PaymentReceipt, CalculatePriceRequest, PatientCoPayment

**Key Workflows:** Price calculation, invoice generation, payment processing, claim submission

**PHI Scope:** YES - Financial PHI (patient ID in invoices, service dates, diagnosis codes)

---

## ENTITIES

### InvoiceDetailsDto

**Location:** Models/Billing/InvoiceDetailsDto.cs:3

**Purpose:** Invoice line item with service, pricing, sponsor allocation, and diagnosis pointers

**Key Fields (PHI):**
- InvoiceId, ServiceId, ServiceAddedDate (**YES** - linked to patient encounter)
- DXPointer, DxPointers (List) - Diagnosis code pointers (**YES** - clinical data)
- GrossAmount, NetAmount, SponsorAmount, PatientAmount - Pricing breakdown
- ProcedureCode - Service performed (CPT/HCPCS code)
- BillingGroupId - Fee schedule reference
- Quantity, DiscountAmount, DiscountReason
- AuthCode, ValidityStartDate, ValidityEndDate - Prior authorization

**Relationships:**
- Invoice: Many line items to one invoice
- ProcedureCode: Each line item has one procedure code
- Patient: Linked via invoice to patient

---

### CalculatePriceRequest

**Location:** Models/Billing/CalculatePriceRequest.cs:1

**Purpose:** Multi-factor pricing input (service codes, provider, sponsor, patient)

**Key Fields:**
- ServiceCodes (List) - Services being priced
- ProviderId - Provider performing service
- SponsorId - Insurance or corporate payer
- PatientId - For patient-specific pricing
- PracticeId - Facility location affects pricing

**Usage:** POST /api/Billing/Agreements/CalculatePrice (IBillingApi.cs:14)

---

### BillingGroupDto / ProcedureCodeModel

**Location:** Models/Billing/BillingGroupDto.cs, ProcedureCodeModel.cs

**Purpose:** Fee schedules and service codes for billing

**Key Concepts:**
- BillingGroup: Provider/practice fee schedule
- ProcedureCode: CPT, HCPCS codes for services
- Agreement-based pricing: Sponsor-specific rates

---

## WORKFLOWS

### Workflow 1: Multi-Factor Price Calculation

**Entry Point:** IBillingApi.cs:14,20

**Steps:**
1. **Service Selection** - Choose procedure codes
2. **Sponsor Identification** - Insurance or corporate
3. **Billing Profile Lookup** - Provider fee schedule
4. **Agreement Pricing** - Sponsor-specific rates
5. **Calculate Split** - Sponsor share vs patient share
6. **Return FeeSummaryResponse** - Gross, net, sponsor, patient amounts

**File:** IBillingApi.cs:14 (CalculatePrice), 20 (PreCalculatePrice)

**Error Handling:**
- No billing profile: Use default rates or block booking
- Expired agreement: Prompt for sponsor update
- Price calculation failure: Blocks appointment booking

**PHI:** Patient ID, service codes, sponsor details

---

### Workflow 2: Invoice Generation

**Entry Point:** IBillingApi.cs:26

**Steps:**
1. **Service Capture** - From appointment or encounter
2. **Price Calculation** - Call CalculatePrice
3. **Diagnosis Codes** - Link ICD-10 codes (DXPointer)
4. **Line Items** - Create InvoiceDetailsDto for each service
5. **Sponsor Allocation** - Split amounts
6. **Invoice Creation** - Generate invoice document
7. **Email Invoice** - IEmailSenderApi.cs:8

**PHI:** Patient ID, service dates, diagnosis codes

---

### Workflow 3: Payment Processing

**Entry Point:** IBillingApi.cs:29,34,40

**Steps:**
1. **Balance Inquiry** - AvailableBalance endpoint
2. **Payment Collection** - Amount, method
3. **Receipt Generation** - PaymentReceiptResponse
4. **Balance Update** - Reduce outstanding amount
5. **Track Sponsor vs Patient** - Separate accounting

**PHI:** Patient ID, payment amounts

---

### Workflow 4: Claim Submission

**Entry Point:** Inferred (not visible in core library)

**Steps:**
1. **Invoice Finalization** - Lock invoice
2. **Claim Generation** - Standard format (X12 837 or NABIDH)
3. **Code Mapping** - ICD-10 diagnoses, CPT procedures
4. **Payer Submission** - To insurance company
5. **Status Tracking** - Submitted/Accepted/Rejected
6. **Remittance Processing** - Payment from payer

**Standards:** ICD-10, CPT/HCPCS, X12 EDI

---

## INTEGRATIONS

**Billing Service (IBillingApi.cs:6-45):**
- 6 endpoints: CalculatePrice, PreCalculatePrice, Invoice, AvailableBalance, PaymentReceipts (2 variants)
- Purpose: Core billing operations

**Patient Service (IPatientApi.cs:16,22):**
- Sponsor verification for pricing

**Scheduling Service (ISchedulingApi.cs:23):**
- Appointment services for billing

**Email Service (IEmailSenderApi.cs:8):**
- Invoice delivery

---

## COMPLIANCE SUMMARY

**PHI Fields:** Patient ID in invoices, service dates, diagnosis codes (financial PHI)

**Audit:** ✅ All invoice/payment operations logged via AuditableBaseDbContext.cs:25

**Retention:** 7 years for financial records (HIPAA/UAE requirement)

**Accuracy:** Multi-factor pricing must be auditable and reproducible

**Standards:** ICD-10 (diagnoses), CPT/HCPCS (procedures) for claims

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)

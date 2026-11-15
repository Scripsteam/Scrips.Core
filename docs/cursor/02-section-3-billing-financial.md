Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (318 lines) was replaced by this shorter file.

# Section 3: Billing and Financial

## 1. Scope [UPDATED]

Wire contracts for invoices, claims, agreements and payments. The owning service is Scrips.Billing; the claim submission service is Scrips.EClaim. This repo has no pricing or claim logic. [CONFIRMED: only DTOs, enums and proto here]

## 2. Files [UPDATED]

| Kind | Count | Location |
|---|---|---|
| DTOs | 22 | `Scrips.Core/Models/Billing/*.cs` |
| Enums | 6 | `Scrips.Core/Enums/Billing/*.cs` |
| Refit routes | 7 | `IBillingApi.cs:8, 14, 20, 26, 29, 34, 40` |
| gRPC service `BillingManagement` | 22 rpcs (21 in March) | `Scrips.Core/Protos/BillingManagement.proto` (352 lines) |

DTOs: AvailableBalanceResponse, Balance, BaseCalculatePrice, BillingGroupDto, BillingProfileDto, BillingProfileServiceDto, CalculatePriceRequest, ClonedBillingGroupDto, FeeSummaryResponse, IndividualServiceDto, InvoiceDetailsDto, InvoiceDto, InvoiceLineItemDxPointer, InvoiceLineItemDxPointerCode, PatientCoPaymentDto, PaymentReceiptResponse, PreCalculatePriceRequest, ProcedureCodeDto, ProcedureCodeModel, ProviderBillingProfileDto, ProviderSettingsDto, ServiceLocationFeeDto. [CONFIRMED]
`BillingProfileServiceDto`, `ProcedureCodeDto`, `ServiceLocationFeeDto`, `InvoiceDetailsDto` and `BillingProfileDto` each exist in two folders (`Models/Billing` and `Models/Practice` or `Models/Scheduling`). [CONFIRMED: `git ls-files`]

## 3. Enums [UPDATED]

| Enum | Values | File |
|---|---|---|
| `ClaimStatusType` | Initial, Hold, Reviewed, Submitted, Resubmitted, Remitted, RevisedAndReviewed, Reconciled, DenialAddressed (9) | `Enums/Billing/ClaimStatus.cs` |
| `ClaimRequestStatus` | Sending, Submitted, Error, ConnectionError, ReadyForSubmission, DenialAddressed | `ClaimRequestStatus.cs` |
| `InvoiceUpdateType` | Primary, PreClaim | `InvoiceUpdateType.cs` |
| `RemittanceAdviceAllocationType` | AutoAllocated, File | `RemittanceAdviceAllocationType.cs` |
| `RemittanceAdviceStatus` | Pending, Processed | `RemittanceAdviceStatus.cs` |
| `SponsorType` | SelfPay, Insurance, Corporate | `SponsorType.cs` |
| `EnumInvoiceStatus` (in `Enums/Scheduling`) | Draft, Issued, Balanced, Canceled, Claimed, Reclaimed | `Enums/Scheduling/InvoiceStatusEnum.cs` |

The enum file `ClaimStatus.cs` declares the type `ClaimStatusType`. [CONFIRMED] All six billing enums carry `JsonStringEnumConverter` and `[Display(Name=...)]`. [CONFIRMED]

## 4. HTTP routes (Refit `IBillingApi`) [UPDATED]

`GET /api/Billing/ProviderBillingProfiles`, `POST /api/Billing/Agreements/CalculatePrice`, `POST /api/Billing/Agreements/PreCalculatePrice`, `GET /api/Billing/Invoice/{invoiceId}`, `GET /api/Billing/Payment/AvailableBalance`, `GET /api/Billing/Payment/PaymentReceipts`, `GET /api/Billing/Payment/PaymentReceipts2`. [CONFIRMED: `IBillingApi.cs`]

## 5. gRPC contract `BillingManagement` [UPDATED]

22 rpcs: `InitiateOrganization`, `GetAgreements`, `GetBillingProfile`, `GetBillingProfiles`, `GetBillingGroupByOrganizationId`, `UpdateBillingProfileServiceAddedDateByEncounterId`, `UpdateBillingProfile`, `GetOrganizationBillingCurrency`, `GetInsurancePayerDetail`, `CreateInvoice`, `CreateInvoice2`, `UpdateInvoice`, `UpdateInvoiceEncounter`, `UpdateInvoiceProvider` (new), `GetInvoiceByAppointmentId`, `GetInvoicesByAppointmentIds`, `GetAgreementByCompanyId`, `GetAgreementDataByCompanyId`, `GetCashierShiftDetails`, `CreateClaim`, `SetIsResivedTrueInClaim` (sic), `GetInvoiceClaimValidationData`. [CONFIRMED]

### New since March [NEW]
- `UpdateInvoiceProvider` (PR #125, 2026-11-12): internal, Scheduling to Billing, sets the provider on the appointment's DRAFT invoices; issued, paid or cancelled invoices are never changed; idempotent. [CONFIRMED: `BillingManagement.proto:29-31`] Use: a group-session lead change moves member invoices to the new lead (comment at `BillingManagement.proto:301`).
- `resourceId` and `resourceName` on both invoice request messages (`BillingManagement.proto:210-211, 235-236`; commit 5844f64, PROD-2153), so a device booking can be invoiced. [CONFIRMED]

## 6. Gaps and risks [NEW]

- Money and codes use plain DTO types. Whether amounts use `decimal` or `double` in each DTO was not re-checked in this pass. [NOT VERIFIED]
- Claim and eClaim credentials are not in the Billing proto. They are returned by the Practice proto (see `02-section-5-provider-practice-org.md`). [CONFIRMED]
- Any statement about NABIDH or DHA/DOH claim rules is out of scope for this file. [NEEDS HUMAN REVIEW]

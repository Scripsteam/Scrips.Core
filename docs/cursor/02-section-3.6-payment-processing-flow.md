# Section 3.6: Payment Processing Flow

## PURPOSE

This section documents patient payment collection and processing, including co-payment collection at check-in, balance payment at check-out, credit card processing, receipt generation, and outstanding balance tracking for complete revenue capture.

## ENTRY POINTS

| File | Method | Line | Purpose |
|------|--------|------|---------|
| InvoiceDto.cs | Financial totals | 37-48 | Patient payment amounts |
| IBillingApi.cs | PaymentAvailableBalance | 29-32 | Patient account balance |
| IBillingApi.cs | PaymentReceipts | 34-38 | Payment history |

## PROCESSING LOGIC (Step-by-Step)

### Step 1: Co-Payment Collection (Check-In)
**File:** InvoiceDto.cs:43-45
- Display co-payment amount from insurance
- Collect payment: Cash, credit card, or deposit account
- Generate receipt
- Record as Deposited on future invoice
- Amount applied when invoice finalized

### Step 2: Invoice Finalization
**File:** InvoiceDto.cs:37-48
- Invoice finalized after encounter
- Patient share calculated: PatientShareTotal
- Subtract deposits: PatientPayable = PatientShareTotal - Deposited
- Display remaining balance to patient

### Step 3: Balance Due Display
**File:** InvoiceDto.cs:43-45
- Show patient total charges
- Show sponsor coverage amount
- Show patient responsibility
- Show deposited amount
- Show remaining balance due

### Step 4: Payment Method Selection
- Patient selects payment method:
  - Cash
  - Credit/debit card
  - Bank transfer
  - Payment plan
  - Outstanding (bill later)

### Step 5: Payment Processing
- **Cash**: Accept cash, provide change
- **Card**: Process via payment gateway
  - Enter card details
  - Process transaction
  - Authorization/decline response
  - Print card receipt
- **Transfer**: Provide bank details
- **Payment Plan**: Setup installment schedule

### Step 6: Payment Recording
**File:** [External payment service]
- Create payment record
- Link to InvoiceId
- Payment amount, method, date
- Reference number (transaction ID)
- User who processed payment

### Step 7: Invoice Balance Update
**File:** InvoiceDto.cs:45
- Subtract payment from PatientOutstanding
- If fully paid: PatientOutstanding = 0
- Update PatientPaymentStatus
- If full: Status = Paid
- If partial: Status = Partial

### Step 8: Receipt Generation
**File:** IBillingApi.cs:34-38 (PaymentReceipts)
- Generate payment receipt
- Invoice number, payment amount, method
- Date, time, user
- Practice information and branding
- Patient copy provided

### Step 9: Available Balance Check
**File:** IBillingApi.cs:29-32
- Query patient's advance deposit account
- Available balance for future visits
- Deposit credits from overpayments

### Step 10: Outstanding Balance Tracking
**File:** InvoiceDto.cs:45
- If balance remains unpaid
- PatientOutstanding tracked
- Statement sent to patient
- Follow-up for collection

## OUTPUTS

**Success:**
- Payment processed and recorded
- Invoice balance updated
- Receipt provided to patient
- PatientOutstanding reduced or cleared

**Failure:**
- Card declined: Request alternate payment
- Insufficient funds: Partial payment or outstanding
- Payment gateway error: Manual processing or retry

## BUSINESS RULES

- Rule 1: Co-payment collected at check-in (best practice)
- Rule 2: Remaining balance collected at check-out
- Rule 3: Payments applied to oldest invoices first
- Rule 4: Overpayments credited to patient deposit account
- Rule 5: Payment plans require signed agreement
- Rule 6: Card payments processed through secure gateway
- Rule 7: Cash payments require receipt
- Rule 8: Outstanding balances generate patient statements
- Rule 9: Payment history tracked for audit
- Rule 10: Refunds require manager approval

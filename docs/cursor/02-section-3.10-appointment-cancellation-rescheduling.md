# Section 3.10: Appointment Cancellation & Rescheduling

## PURPOSE

This section documents appointment cancellation and rescheduling workflows, including status transitions, slot release, cancellation reasons tracking, rescheduling with new slot selection, notification to patients and providers, and no-show handling.

## ENTRY POINTS

| File | Method | Line | Purpose |
|------|--------|------|---------|
| AppointmentResponse.cs | AppointmentStatus | 10 | Status for cancellation |
| AppointmentStatus.cs | Cancelled, NoShow | 16, 18 | Cancellation statuses |
| SlotResponse.cs | Status | 26 | Release slot to Free |

## PROCESSING LOGIC (Step-by-Step)

### Step 1: Cancellation Initiation
**File:** AppointmentStatus.cs:16
- Patient or staff initiates cancellation
- Retrieve appointment by ID
- Verify current status (must be Booked or Confirmed)

### Step 2: Cancellation Reason Capture
**File:** AppointmentResponse.cs:62
- Select cancellation reason:
  - Patient requested
  - Provider unavailable
  - Emergency
  - No longer needed
  - Duplicate appointment
- Optional free-text note

### Step 3: Cancellation Policy Check
- Check appointment date vs current date
- Late cancellation if within 24 hours (may incur fee)
- Same-day cancellation (may incur fee)
- Advance cancellation (no fee)

### Step 4: Appointment Status Update
**File:** AppointmentStatus.cs:16
- Status: Booked → Cancelled
- Record cancellation timestamp
- Record user who cancelled
- Store cancellation reason

### Step 5: Slot Release
**File:** SlotResponse.cs:26
- Linked slot Status: Busy → Free
- Slot available for rebooking
- Slot appears in availability search

### Step 6: Encounter Handling (if created)
**File:** AppointmentResponse.cs:47-48
- If encounter created (patient checked in)
- Mark encounter as cancelled
- Or keep for documentation if clinical work done

### Step 7: Payment Refund (if applicable)
**File:** InvoiceDto.cs:44
- If co-payment collected
- Process refund based on policy
- Or credit to patient deposit account

### Step 8: Notification Sending
- Notify patient of cancellation
- Email and SMS confirmation
- Reschedule invitation if applicable
- Notify provider if same-day cancellation

### Step 9: Rescheduling (if requested)
**File:** ISchedulingApi.cs:8-12
- Search available slots
- Patient/staff selects new slot
- Create new appointment
- Link to original cancelled appointment

### Step 10: No-Show Handling
**File:** AppointmentStatus.cs:18
- If patient doesn't arrive within grace period
- Staff marks as NoShow
- No-show fee may apply
- No-show count tracked per patient

## OUTPUTS

**Success:**
- Appointment cancelled or rescheduled
- Slot released and available
- Patient and provider notified
- Cancellation recorded for analytics

**Failure:**
- Appointment already fulfilled: Cannot cancel
- Same-day cancellation: May incur fee
- Multiple no-shows: Patient may require prepayment

## BUSINESS RULES

- Rule 1: Only pending/booked/confirmed appointments can be cancelled
- Rule 2: Fulfilled appointments cannot be cancelled (historical record)
- Rule 3: Cancelled appointment releases slot immediately
- Rule 4: Late cancellations (<24 hours) may incur fee
- Rule 5: No-show if patient doesn't arrive within 30 minutes of end time
- Rule 6: Multiple no-shows may trigger policy (prepayment, suspension)
- Rule 7: Cancellation reason required for analytics
- Rule 8: Co-payment refunds processed per practice policy
- Rule 9: Recurring appointment cancellations can cancel series or single
- Rule 10: Rescheduling creates new appointment, keeps cancellation record

## CANCELLATION POLICY EXAMPLES

**Advance Cancellation (>24 hours):**
- No fee
- Full refund if payment made
- Encouraged for slot availability

**Late Cancellation (<24 hours):**
- May incur cancellation fee ($25-50)
- Refund minus fee
- Policy varies by practice

**Same-Day Cancellation:**
- Higher fee ($50-100)
- Or no refund
- Provider time lost

**No-Show:**
- Full no-show fee charged
- No refund
- May require prepayment for future appointments

## NO-SHOW MANAGEMENT

**Automatic Detection:**
- Appointment end time + 30 minutes passed
- No check-in recorded
- System auto-marks as NoShow
- Or staff manually marks

**No-Show Fees:**
- Fee charged to patient
- Added to patient balance
- Or deducted from deposit

**No-Show Tracking:**
- Count no-shows per patient
- Thresholds trigger actions:
  - 1 no-show: Warning
  - 2 no-shows: Require prepayment
  - 3 no-shows: Require manager approval for booking

## RESCHEDULING WORKFLOW

**Step 1:** Patient requests reschedule
**Step 2:** Cancel original appointment (reason: Rescheduled)
**Step 3:** Search available slots
**Step 4:** Select new date/time
**Step 5:** Create new appointment
**Step 6:** Link appointments (original → rescheduled)
**Step 7:** Send confirmation for new appointment

## RECURRING APPOINTMENT CANCELLATION

**Options:**
- Cancel single instance
- Cancel all future occurrences
- Cancel series (all instances)

**Single Instance:**
- Only selected date cancelled
- Other dates unchanged

**Series Cancellation:**
- All linked appointments cancelled
- All slots released
- Can rebook individual dates later

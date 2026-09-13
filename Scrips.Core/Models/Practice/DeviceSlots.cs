namespace Scrips.Core.Models.Practice;

/// <summary>
/// The Devices calendar's card feed: one practice, one date range, every live device with its
/// presence, its products and its next free slots.
///
/// Deliberately NOT <see cref="ProviderSearchRequest"/> (ruling 2026-09-13, PROD-2181). Teaching
/// Doctor/CalendarSlots3 to return devices would have mixed machines into every existing provider
/// consumer -- that request carries no actor discriminator -- and would have charged every device row
/// for a photo SAS URI, a licence-expiry check and four practitioner-only objects it has no use for.
/// The engine underneath IS shared: presence, availability and slot-chopping are all actor-generic.
/// </summary>
public class DeviceSlotsRequest
{
    /// <summary>Organization id.</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>Practice whose devices are being asked for.</summary>
    public Guid PracticeId { get; set; }

    /// <summary>Range start, UTC. Callers send a full-day range the way calendarSlots does.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Range end, UTC.</summary>
    public DateTime EndDate { get; set; }

    /// <summary>Case-insensitive match on device name or subtype. Empty: every live device.</summary>
    public string SearchText { get; set; }

    /// <summary>Restrict to these device ids. Empty: every live device at the practice.</summary>
    public List<Guid> Devices { get; set; } = new();

    /// <summary>How many free slots to carry on each card. The strip shows 3.</summary>
    public int SlotCount { get; set; } = 3;
}

/// <summary>One free slot on a device, in UTC.</summary>
public class DeviceSlotResponse
{
    /// <summary>Slot start, UTC.</summary>
    public DateTime Start { get; set; }

    /// <summary>Slot end, UTC.</summary>
    public DateTime End { get; set; }
}

/// <summary>One product of the device -- the quick-book chip on its card.</summary>
public class DeviceSlotsAppointmentProfile
{
    /// <summary>Profile id.</summary>
    public Guid Id { get; set; }

    /// <summary>Display name.</summary>
    public string ProfileName { get; set; }

    /// <summary>Slot length, minutes. Drives the chip and the slot duration.</summary>
    public int MinutesDuration { get; set; }

    /// <summary>Chip colour.</summary>
    public string Color { get; set; }

    /// <summary>Appointment type code.</summary>
    public string AppointmentTypeId { get; set; }

    /// <summary>Whether this is the device's default product.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Whether patients may book it in-app.</summary>
    public bool InAppBooking { get; set; }

    /// <summary>PROD-2154: the product's default billing profile.</summary>
    public Guid? BillingProfileId { get; set; }

    /// <summary>PROD-2165: the product's priced service. Takes precedence over BillingProfileId.</summary>
    public Guid? ServiceId { get; set; }
}

/// <summary>
/// One window the device is present, in UTC. WeedayId uses the SAME Mon=1..Sun=7 convention the
/// practitioner path emits, because the frontend's working-hours label matches on it and reads
/// Start/End as the fallback for StartTime/EndTime -- so the existing helper works on a device row
/// with no translation.
/// </summary>
public class DeviceWorkingWindow
{
    /// <summary>Mon=1 .. Sun=7.</summary>
    public int WeedayId { get; set; }

    /// <summary>Window start, UTC.</summary>
    public DateTime Start { get; set; }

    /// <summary>Window end, UTC.</summary>
    public DateTime End { get; set; }
}

/// <summary>
/// One device card.
///
/// No Speciality, LicenceNo, ExpirationDate, Issue or Photo: a machine has none of them, and an empty
/// practitioner field on a device row reads to the frontend as "credentials missing" -- which would
/// draw a warning badge on a machine.
/// </summary>
public class DeviceSlotsResponse
{
    /// <summary>Device id (Practice.Resources).</summary>
    public Guid Id { get; set; }

    /// <summary>Device name -- the card's first line.</summary>
    public string Name { get; set; }

    /// <summary>The card's second line. Null when the device has no technician.</summary>
    public string TechnicianName { get; set; }

    /// <summary>The assigned technician, if any.</summary>
    public Guid? TechnicianId { get; set; }

    /// <summary>Free-text device subtype (e.g. "MRI").</summary>
    public string Subtype { get; set; }

    /// <summary>Practice the device belongs to.</summary>
    public Guid PracticeId { get; set; }

    /// <summary>The organization's time zone, as the slots' frame of reference.</summary>
    public string TimeZone { get; set; }

    /// <summary>Empty when the device has no active Work profile: known, but not bookable.</summary>
    public List<DeviceWorkingWindow> WorkingHours { get; set; } = new();

    /// <summary>Live products of the device, shortest first.</summary>
    public List<DeviceSlotsAppointmentProfile> AppointmentProfile { get; set; } = new();

    /// <summary>The first SlotCount free slots.</summary>
    public List<DeviceSlotResponse> Slots { get; set; } = new();

    /// <summary>Every free slot in the range, not just the ones carried in Slots.</summary>
    public int TotalSlots { get; set; }
}

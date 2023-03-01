using System.Text.Json.Serialization;

namespace Scrips.Core.Enums.Scheduling;

/// <summary>
///     This example value set defines a set of reasons for the cancellation of an appointment.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppointmentCancellationReason
{
    /// <summary>
    ///     Patient.
    /// </summary>
    Pat,

    /// <summary>
    ///     Patient: Canceled via automated reminder system.
    /// </summary>
    PatCrs,

    /// <summary>
    ///     Patient: Canceled via Patient Portal.
    /// </summary>
    PatCpp,

    /// <summary>
    ///     Patient: Deceased.
    /// </summary>
    PatDec,

    /// <summary>
    ///     Patient: Feeling Better.
    /// </summary>
    PatFb,

    /// <summary>
    ///     Patient: Lack of Transportation.
    /// </summary>
    PatLt,

    /// <summary>
    ///     Patient: Member Terminated.
    /// </summary>
    PatMt,

    /// <summary>
    ///     Patient: Moved.
    /// </summary>
    PatMv,

    /// <summary>
    ///     Patient: Pregnant.
    /// </summary>
    PatPreg,

    /// <summary>
    ///     Patient: Scheduled from Wait List.
    /// </summary>
    PatSwl,

    /// <summary>
    ///     Patient: Unhappy/Changed Provider.
    /// </summary>
    PatUcp,

    /// <summary>
    ///     Provider.
    /// </summary>
    Prov,

    /// <summary>
    ///     Provider: Personal.
    /// </summary>
    ProvPers,

    /// <summary>
    ///     Provider: Discharged.
    /// </summary>
    ProvDch,

    /// <summary>
    ///     Provider: Edu/Meeting.
    /// </summary>
    ProvEdu,

    /// <summary>
    ///     Provider: Hospitalized.
    /// </summary>
    ProvHosp,

    /// <summary>
    ///     Provider: Labs Out of Acceptable Range.
    /// </summary>
    ProvLabs,

    /// <summary>
    ///     Provider: MRI Screening Form Marked Do Not Proceed.
    /// </summary>
    ProvMri,

    /// <summary>
    ///     Provider: Oncology Treatment Plan Changes.
    /// </summary>
    ProvOnc,

    /// <summary>
    ///     Equipment Maintenance/Repair.
    /// </summary>
    Maint,

    /// <summary>
    ///     Prep/Med Incomplete.
    /// </summary>
    MedsInc,

    /// <summary>
    ///     Other.
    /// </summary>
    Other,

    /// <summary>
    ///     Other: CMS Therapy Cap Service Not Authorized.
    /// </summary>
    OthCms,

    /// <summary>
    ///     Other: Error.
    /// </summary>
    OthErr,

    /// <summary>
    ///     Other: Financial.
    /// </summary>
    OthFin,

    /// <summary>
    ///     Other: Improper IV Access/Infiltrate IV.
    /// </summary>
    OthIv,

    /// <summary>
    ///     Other: No Interpreter Available.
    /// </summary>
    OthInt,

    /// <summary>
    ///     Other: Prep/Med/Results Unavailable.
    /// </summary>
    OthMu,

    /// <summary>
    ///     Other: Room/Resource Maintenance.
    /// </summary>
    OthRoom,

    /// <summary>
    ///     Other: Schedule Order Error.
    /// </summary>
    OthOerr,

    /// <summary>
    ///     Other: Silent Walk In Error.
    /// </summary>
    OthSwie,

    /// <summary>
    ///     Other: Weather.
    /// </summary>
    OthWeath,

    /// <summary>
    ///     Other: PatIt.
    /// </summary>
    PatIt,

    /// <summary>
    ///     Other: Discharged.
    /// </summary>
    ProcDch,

    /// <summary>
    ///     Insurance Expired.
    /// </summary>
    InE,

    /// <summary>
    ///     Need to Reschedule.
    /// </summary>
    NtR,

    /// <summary>
    ///     Time is not Convenient.
    /// </summary>
    TNC
}
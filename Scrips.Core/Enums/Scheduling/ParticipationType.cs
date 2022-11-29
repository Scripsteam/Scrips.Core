using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming

namespace Scrips.Core.Enums.Scheduling;

/// <summary>
///     Role of participant in the appointment
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParticipationType
{
    /// <summary>
    ///     The practitioner who is responsible for admitting a patient to a patient encounter.
    /// </summary>
    ADM,

    /// <summary>
    ///     The practitioner that has responsibility for overseeing a patient's care during a patient encounter.
    /// </summary>
    ATND,

    /// <summary>
    ///     A person or organization who should be contacted for follow-up questions about the act in place of the author.
    /// </summary>
    CALLBCK,

    /// <summary>
    ///     An advisor participating in the service by performing evaluations and making recommendations.
    /// </summary>
    CON,

    /// <summary>
    ///     The practitioner who is responsible for the discharge of a patient from a patient encounter.
    /// </summary>
    DIS,

    /// <summary>
    ///     Only with Transportation services. A person who escorts the patient.
    /// </summary>
    ESC,

    /// <summary>
    ///     A person having referred the subject of the service to the performer (referring physician). Typically, a referring
    ///     physician will receive a report.
    /// </summary>
    REF,

    /// <summary>
    ///     A person assisting in an act through his substantial presence and involvement This includes: assistants,
    ///     technicians, associates, or whatever the job titles may be.
    /// </summary>
    SPRF,

    /// <summary>
    ///     The principal or primary performer of the act.
    /// </summary>
    PPRF,

    /// <summary>
    ///     Indicates that the target of the participation is involved in some manner in the act, but does not qualify how.
    /// </summary>
    PART,

    /// <summary>
    ///     A translator who is facilitating communication with the patient during the encounter.
    /// </summary>
    Translator,

    /// <summary>
    ///     A person to be contacted in case of an emergency during the encounter.
    /// </summary>
    Emergency
}
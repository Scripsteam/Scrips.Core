namespace Scrips.Core.Shared.Ambient;

/// <summary>
/// PROD-3179 / PROD-3180: the string values carried on the ambient encounter contract
/// (MasterManagement.proto, GenerateDocumentationResponse). Listed ONCE here; the React client reads these exact
/// strings. This file has no dependencies so a consumer that compiles Core's protos from the submodule can also
/// compile this file from it (Video links it). Scrips.Master keeps a mirror pinned by a test.
/// </summary>
public static class AmbientWireValues
{
    /// <summary>codeStatus on diagnoses, labs, radiology, vaccinations, procedures and medications.</summary>
    public static class CodeStatus
    {
        /// <summary>Exactly one catalogue row; matchedCode / matchedSystem / matchedDisplay are set.</summary>
        public const string Matched = "matched";

        /// <summary>Two or more catalogue rows; no code.</summary>
        public const string Ambiguous = "ambiguous";

        /// <summary>No catalogue row; no code.</summary>
        public const string Unmatched = "unmatched";

        /// <summary>The lookup failed or missed its deadline; no code. Treat like unmatched.</summary>
        public const string Unchecked = "unchecked";

        /// <summary>No lookup was made. NOT matched.</summary>
        public const string NotChecked = "";

        public static readonly string[] All = { Matched, Ambiguous, Unmatched, Unchecked, NotChecked };
    }

    /// <summary>EncounterProcedure.status.</summary>
    public static class ProcedureStatus
    {
        public const string Performed = "performed";
        public const string Ordered = "ordered";

        public static readonly string[] All = { Performed, Ordered };
    }

    /// <summary>Order urgency (labs, radiology, ordered procedures).</summary>
    public static class Urgency
    {
        public const string Routine = "routine";
        public const string Urgent = "urgent";
        public const string Stat = "stat";

        /// <summary>No urgency: a performed procedure, or none was given.</summary>
        public const string NotGiven = "";

        public static readonly string[] All = { Routine, Urgent, Stat, NotGiven };
    }

    /// <summary>speaker on medications and findings.</summary>
    public static class Speaker
    {
        public const string Provider = "provider";
        public const string Patient = "patient";
        public const string Other = "other";

        public static readonly string[] All = { Provider, Patient, Other };
    }

    /// <summary>ClinicalFinding.assertion. "" = not stated, never read as absent or normal.</summary>
    public static class Assertion
    {
        public const string Present = "present";
        public const string Absent = "absent";
        public const string Normal = "normal";
        public const string NotStated = "";

        public static readonly string[] All = { Present, Absent, Normal, NotStated };
    }
}

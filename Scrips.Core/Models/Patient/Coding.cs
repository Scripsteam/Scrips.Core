namespace Scrips.Core.Models.Patient;

public class Coding
{
    public string Code { get; set; }
    public string Display { get; set; }
    public string System { get; set; }

    /// <summary>
    /// PROD-1656 — optional clinician-friendly display name resolved from the
    /// ClinicalDisplayName dictionary (Scrips.Master). Additive and nullable:
    /// consumers that do not know about the display-name layer are unaffected.
    /// Presentation-only — it must NEVER replace the official Display of an
    /// ICD-10/SNOMED coding and must never reach eClaims or HIE
    /// (NABIDH/Malaffi) payloads.
    /// </summary>
    public string PreferredDisplay { get; set; }
}
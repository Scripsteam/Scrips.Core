namespace Scrips.Core.Models.Provider;

public class QualificationModel
{
    public Guid? Id { get; set; }
    public Guid Issuer { get; set; }
    public string IssuerName { get; set; }
    public Guid degree { get; set; }
    public string degreeName { get; set; }
    public CodeModel code { get; set; }
    public PeriodYear Duration { get; set; }

}
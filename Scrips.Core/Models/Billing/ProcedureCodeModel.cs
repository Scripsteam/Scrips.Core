namespace Scrips.Core.Models.Billing;

public class ProcedureCodeModel
{
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public string System { get; set; }
    public string ShortDescription { get; set; }
    public string ShortName
    {
        get
        {
            return System == "DDC" ? DisplayName.Split(" - ")[0] : DisplayName;
        }
    }
}
namespace Scrips.Core.Models.Provider;

public class ProviderReasonVisitModel
{
    public Guid? Id { get; set; }
    public string Code { get; set; }
    public string Display { get; set; }
    public string System { get; set; }
    public bool IsDeleted { get; set; }
}

public class ProviderReasonCodeModel
{
    /// <summary>
    /// provider id is used as Code id
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// An explanation of the meaning of the concept
    /// </summary>
    public string Definition { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string Display { get; set; }
}
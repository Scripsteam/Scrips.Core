namespace Scrips.Core.Models.Provider;

public class ProviderListModel
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? PracticeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? Dob { get; set; }
    public Guid Genderid { get; set; }
    public Guid UserId { get; set; }
}
namespace Scrips.Core.Models.Scheduling;

public class GuardianDetails
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Guid GenderId { get; set; }
    public string GenderName { get; set; }
    public Guid RelationId { get; set; }
    public string RelationName { get; set; }
    public string Name { get; }
    public DateTime? BirthDate { get; set; }
    public string GuaardianIdentificationId { get; set; }
}
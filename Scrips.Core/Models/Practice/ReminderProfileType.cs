namespace Scrips.Core.Models.Practice;

public class ReminderProfileType
{
    public Guid Id { get; set; }
    public Guid ReminderId { get; set; }
    public string? Type { get; set; }
    public string? Time { get; set; }
    public string? WhenToSend { get; set; }
    public string? Number { get; set; }
}
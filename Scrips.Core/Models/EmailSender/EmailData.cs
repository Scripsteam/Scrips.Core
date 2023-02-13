namespace Scrips.Core.Models.EmailSender;

public class EmailData
{
    public string? ToEmail { get; set; }
    public string? ToName { get; set; }

    public Dictionary<string, string>? Data { get; set; }
}
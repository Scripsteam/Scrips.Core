namespace Scrips.Core.Models.AIChiefComplaint;

public class ChiefComplaintDto
{
    public Guid ComplaintId { get; set; }
    public string ComplaintName { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public Guid ComplaintCategoryId { get; set; }
}


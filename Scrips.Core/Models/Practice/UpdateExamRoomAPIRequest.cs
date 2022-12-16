using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Practice;

public class UpdateExamRoomApiRequest
{
    public UpdateExamRoomApiRequest()
    {
        DeletedId = new List<Guid>();
    }
    [Required]
    public Guid PracticeId { get; set; }
    [Required]
    public int NoOfExamHours { get; set; }

    public List<ExamRooms> ExamRooms { get; set; } = new();

    public List<Guid> DeletedId { get; set; }
}

public class ExamRooms
{
    public Guid ? ExamHourId { get; set; }
    public string ExamRoom { get; set; }
}

public class CreateExamRoomRequest
{
    public CreateExamRoomRequest()
    {
        DeletedId = new List<Guid>();
    }
    [Required]
    public Guid PracticeId { get; set; }
    [Required]
    public int NoOfExamHours { get; set; }

    public List<ExamRooms> ExamRooms { get; set; }

    public List<Guid> DeletedId { get; set; }
}
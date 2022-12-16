using System;

namespace Scrips.Core.Models.Scheduling;

public class QuestionnaireRequest
{
    /// <summary>
    /// Unique ID
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Questionnaire ID
    /// </summary>
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// Questionnaire Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Questionnaire Status
    /// </summary>
    public Enums.Scheduling.QuestionnaireStatus Status { get; set; }
}
namespace Scrips.Core.Models.Practice
{
    /// <summary>
    /// 
    /// </summary>
    public class Speciality
    {
        public System.Guid specialitySkillId { get; set; }

        public PracticeValueSet code { get; set; }
    }

    public class PracticeValueSet
    {
        public string code { get; set; }
        public string displayName { get; set; }

        public string System { get; set; } = "https://snomed.info/sct";
    }
}

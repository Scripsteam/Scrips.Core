using System;

namespace Scrips.Core.Models.Practice
{
    public class PracticeSetupDetails
    {
        public Guid OrganizationId { get; set; }
        public Guid PracticeSetupId { get; set; }
        public string PracticeImage { get; set; }
        public string PracticeName { get; set; }
        public string LicenseNumber { get; set; }
        public Guid LicenseAuthority { get; set; }
        public DateTime LicenseExpirationDate { get; set; }
        public string PracticeDescription { get; set; }
        public bool PrimaryPractice { get; set; }
        public bool BillingStatus { get; set; }

        public string BillingTaxId { get; set; }

        public string NabidhAssigningAuthority { get; set; }

        //contact/ details
        public ContactDetails ContactDetails { get; set; }

        //billing details
        public ContactDetails BillingDetails { get; set; }
    }

    public class PracticeHoliday
    {
        public string ProfileName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
    
}

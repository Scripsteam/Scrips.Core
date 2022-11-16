using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Provider
{
    public class PractitionerResponse
    {
        public Guid? identifire { get; set; }
        public Guid UserId { get; set; }
        public Guid OrgnizationId { get; set; }
        public Guid PracticeId { get; set; }
        public Guid? PractitionerRoleId { get; set; }
        public bool IsActive { get; set; }
        public Guid gender { get; set; }
        public string Email { get; set; }
        public DateTime? birthDate { get; set; }
        public string photo { get; set; }
        public string PhonNo { get; set; }
        public HumanName name { get; set; }
        public List<AddressModel> address { get; set; }
        public List<QualificationModel> qualification { get; set; }
        public List<Communication> communications { get; set; }
    }
}

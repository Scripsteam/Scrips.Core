using System;

namespace Scrips.Core.Models.Patient
{
    public class PatientCorporateResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string FrontPhoto { get; set; }
        public string BackPhoto { get; set; }
        public string PayerCode { get; set; }
        public bool IsVerified { get; set; }
        public PayerDetailModel PayerDetails { get; set; }
    }
}

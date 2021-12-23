using System;

namespace Scrips.Core.Models.Patient
{
    public class UpdateIdenitification
    {
        public string Id { get; set; }

        public string IDType { get; set; }

        public string IDNumber { get; set; }

        public DateTime? IDExpirationDate { get; set; }

        public bool IsReviewed { get; set; }

        public DateTime? SubmittedDate { get; set; }
    }
}

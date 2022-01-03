using System;

namespace Scrips.Core.Models.Patient
{
    public class UpdateIdenitification
    {
        public string Id { get; set; }

        public string IdType { get; set; }

        public string IdNumber { get; set; }

        public DateTime? IdExpirationDate { get; set; }

        public bool IsReviewed { get; set; }

        public DateTime? SubmittedDate { get; set; }
    }
}

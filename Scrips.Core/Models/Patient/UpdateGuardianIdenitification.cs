using System;

namespace Scrips.Core.Models.Patient
{
    public class UpdateGuardianIdenitification
    {
        public string Id { get; set; }

        public string RelationId { get; set; }

        public RelationResponse RelationShip { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }

        public string IDType { get; set; }

        public string IDNumber { get; set; }

        public DateTime? IDExpirationDate { get; set; }

        public bool IsReviewed { get; set; }

        public DateTime? SubmittedDate { get; set; }
    }
}

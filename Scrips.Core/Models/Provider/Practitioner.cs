using System;
using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Provider
{
    public class Practitioner : CommonProperty
    {
        [Key] public Guid Id { get; set; }

        /// <summary>
        /// Practice Management UserId.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Provider First Name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Provider Last Name.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Provider Middle Name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Provider Gender.
        /// </summary>
        [Required]
        public Guid GenderId { get; set; }

        /// <summary>
        /// Provider email address.
        /// </summary>
        [MaxLength(50)]
        public string EmailId { get; set; }

        /// <summary>
        /// Provider contact no.
        /// </summary>
        [MaxLength(20)]
        public string phoneNo { get; set; }

        /// <summary>
        /// Provider DOB.
        /// </summary>
        public DateTime? DOB { get; set; }

        /// <summary>
        /// Provider photo.
        /// </summary>
        public string photo { get; set; }

        public new bool IsActive { get; set; }

    }
}
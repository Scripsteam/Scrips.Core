using System;
using System.ComponentModel.DataAnnotations;

namespace Scrips.Core.Models.Provider
{
   public class CommonProperty
    {
        /// <summary>
        /// Common Properties.
        /// </summary>
        [Required]
        public Boolean IsActive { get; set; }

        /// <summary>
        /// Common Properties.
        /// </summary>
        [Required]
        public Boolean IsDeleted { get; set; }

        /// <summary>
        /// Common Properties.
        /// </summary>
        [Required]
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Common Properties.
        /// </summary>
        [Required]
        public DateTime CreatedTS { get; set; }

        /// <summary>
        /// Common Properties.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Common Properties.
        /// </summary>
        public DateTime UpdatedTS { get; set; }
    }
}

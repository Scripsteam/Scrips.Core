using Scrips.Core.Models.Practice;
using System;

namespace Scrips.Core.Models.Scheduling
{
    public class InvoiceDetailsDto
    {

        /// <summary>
        /// Invoice Details Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// ProcedureCode
        /// </summary>
        public ProcedureCodeDto ProcedureCode { get; set; }

        /// <summary>
        /// Is Service Individual Added
        /// </summary>
        public bool? IsIndividualAdded { get; set; }

        /// <summary>
        /// Service id
        /// </summary>
        public Guid? ServiceId { get; set; }
    }
}

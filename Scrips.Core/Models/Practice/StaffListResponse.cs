using System.Collections.Generic;

namespace Scrips.Core.Models.Practice
{
    public class StaffListResponse
    {
        public string RoleName { get; set; }
        public List<StaffSetupDetails> StaffList { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Identity
{
    public class GetUserDetailsRequest
    {
        public List<Guid> UserIdList { get; set; }
    }
}

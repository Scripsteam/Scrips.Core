﻿using System;
using System.Collections.Generic;

namespace Scrips.Core.Models.Practice
{
    public class StaffSetupDetails
    {
        public Guid OrganizationId { get; set; }
        public Guid StaffSetupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string  RoleName { get; set; }
        public int RoleId { get; set; }
        public bool? InvitationStatus { get; set; }
        public string PhotoUrl { get; set; }

        public DateTime? IsLastLogin { get; set; }

        public List<Practice> PracticeList { get; set; }
        public string PracticeName { get; set; }
    }

    public class Practice
    {
        public string PracticeName { get; set; }
    }
}
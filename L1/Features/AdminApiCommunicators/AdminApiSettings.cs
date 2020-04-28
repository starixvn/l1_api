using System;

namespace L1.Features.AdminApiCommunicators
{
    public class AdminApiSettings
    {
        public Uri BaseUrl { get; set; }

        public AdminApiEndPoints Endpoints { get; set; }
    }

    public class AdminApiEndPoints
    {
        public string UnderMaintenance { get; set; }
    }
}
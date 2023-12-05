using System;
using System.Collections.Generic;
using System.Linq;

namespace L1.Features.AdminApiCommunicators
{
    public class AdminApiSettings
    {
        public Uri BaseUrl { get; set; }

        public Uri BaseCloudUrl { get; set; }

        public AdminApiEndPoints Endpoints { get; set; }
    }

    public class AdminApiEndPoints
    {
        public string UnderMaintenance { get; set; }

        public string DailySummary { get; set; }

        public string RetrieveEndGameInfo { get; set; }

        public string NotifyHighStake { get; set; }
    }

    public class CloudSettings
    {
        public bool IsCloudService { get; set; }

        public IEnumerable<int> SeparatedGameIds { get; set; }

        public bool ShouldCallCloudService(int gameTypeId)
          => !IsCloudService && SeparatedGameIds.Contains(gameTypeId);
    }
}
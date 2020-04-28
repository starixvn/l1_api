using System;

namespace L1.Features.GameServerCommunicators
{
    public class GameServerSettings
    {
        public Uri BaseUrl { get; set; }

        public GameServerEndPoints Endpoints { get; set; }
    }

    public class GameServerEndPoints
    {
        public string RetrieveEndGameInfo { get; set; }

        public string DailySummary { get; set; }

        public string UnderMaintenance { get; set; }
    }
}
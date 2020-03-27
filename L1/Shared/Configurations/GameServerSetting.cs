using System;

namespace L1.Shared.Configurations
{
    public class GameServerSetting
    {
        public Uri BaseUrl { get; set; }

        public GameServerEndPoints Endpoints { get; set; }
    }

    public class GameServerEndPoints
    {
        public string RetrieveEndGameInfo { get; set; }

        public string DailySummary { get; set; }
    }
}
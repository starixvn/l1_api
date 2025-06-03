using System.Linq;

namespace L1.Shared.Constants
{
    public static class AppTypes
    {
        public const byte SabaClub = 1;
        public const byte SabaMobi = 2;
        public const byte MiniSabaClub = 3;
        public const byte MegaKeno = 4;
        public const byte SabaApp = 5;
        public const byte LivePage = 6;
        public const byte MobileDirect = 7;
        public const byte TodayPage = 8;


        private static readonly byte[] appTypes = { SabaClub, SabaMobi, MiniSabaClub, MegaKeno, SabaApp, LivePage, MobileDirect, TodayPage };

        public static bool IsValid(byte appType)
        {
            return appTypes.Contains(appType);
        }
    }
}
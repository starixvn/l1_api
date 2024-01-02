namespace L1.Features.SabaPlayCommunicators
{
    public class SabaPlayFormatter
    {
        public static string FormatPlayerId(string siteId, string playerId) => $"{siteId}~{playerId}";
    }
}
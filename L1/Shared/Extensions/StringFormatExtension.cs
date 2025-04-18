using L1.Shared.Constants;

namespace L1.Shared.Extensions
{
    public static class StringFormatExtensions
    {
        public static string FormatPlayerId(string operatorId, string siteId, string playerId)
        {
            if (operatorId == Operators.SabaPlay)
            {
                return string.IsNullOrWhiteSpace(siteId) ? playerId : $"{siteId}~{playerId}";
            }

            return playerId;
        }
    }
}

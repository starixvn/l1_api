using System;
using System.Collections.Generic;
using System.Linq;

namespace L1.Features.PartnerCommunicators
{
    public interface IPartnerPlayerIdConverter
    {
        string ConvertPlayerId(string operatorId, string playerId);
    }

    public class PartnerPlayerIdConverter : IPartnerPlayerIdConverter
    {
        private readonly Dictionary<string, Type> operatorPlayerIdTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            //Migrated SG to OW - ONEworks1 now uses string PlayerId
            { "ONEworks1", typeof(string) },
            { "SBO", typeof(int) },
            { "PFF", typeof(string) },
            { "ONEworksVGCash", typeof(string) },
            { "ONEworks_VG", typeof(string) },
            { "ONEworksSeal888", typeof(string) },
            { "SabaPlay", typeof(string) },
            { "ONEworksCash", typeof(string) },
            { "ONEworksCredit", typeof(string) },
        };

        public string ConvertPlayerId(string operatorId, string playerId)
        {
            var extractedPlayerId = ExtractOperatorPlayerId(playerId);

            if (operatorPlayerIdTypes.TryGetValue(operatorId, out var playerIdType) && playerIdType == typeof(int))
            {
                return int.Parse(extractedPlayerId).ToString();
            }

            return extractedPlayerId;
        }

        private static string ExtractOperatorPlayerId(string playerId)
        {
            return playerId.Split("~").LastOrDefault();
        }
    }
}
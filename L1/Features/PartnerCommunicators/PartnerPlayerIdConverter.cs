using System;
using System.Collections.Generic;
using System.Linq;

namespace L1.Features.PartnerCommunicators
{
    public interface IPartnerPlayerIdConverter
    {
        object ConvertPlayerId(string operatorId, string playerId);
    }

    public class PartnerPlayerIdConverter : IPartnerPlayerIdConverter
    {
        private readonly Dictionary<string, Type> operatorPlayerIdTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "ONEworks1", typeof(int) },
            { "SBO", typeof(int) },
            { "PFF", typeof(int) },
            { "ONEworksVGCash", typeof(string) },
            { "ONEworks_VG", typeof(string) },
            { "ONEworksSeal888", typeof(string) },
            { "SabaPlay", typeof(string) },
        };

        public object ConvertPlayerId(string operatorId, string playerId)
        {
            var extractedPlayerId = ExtractOperatorPlayerId(playerId);

            if (operatorPlayerIdTypes.TryGetValue(operatorId, out var playerIdType))
            {
                if (playerIdType == typeof(int))
                {
                    return int.Parse(extractedPlayerId);
                }
            }

            return extractedPlayerId;
        }

        private static string ExtractOperatorPlayerId(string playerId)
        {
            return playerId.Split("~").LastOrDefault();
        }
    }
}
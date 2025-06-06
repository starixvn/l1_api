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
            //TODO key is OperatorId? if so, should remove SG, add XD support
            { "ONEworks1", typeof(string) },
            { "SBO", typeof(int) },
            { "PFF", typeof(string) },
            { "SG", typeof(int) },
            { "ONEworksVGCash", typeof(string) },
            { "ONEworks_VG", typeof(string) },
            { "ONEworksSeal888", typeof(string) },
            { "SabaPlay", typeof(string) },
            { "ONEworksCash", typeof(string) },
            { "ONEworksCredit", typeof(string) },
        };

        public object ConvertPlayerId(string operatorId, string playerId)
        {
            var extractedPlayerId = ExtractOperatorPlayerId(playerId);

            if (operatorPlayerIdTypes.TryGetValue(operatorId, out var playerIdType) && playerIdType == typeof(int))
            {
                return int.Parse(extractedPlayerId);
            }

            return extractedPlayerId;
        }

        private static string ExtractOperatorPlayerId(string playerId)
        {
            return playerId.Split("~").LastOrDefault();
        }
    }
}
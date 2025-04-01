using System;
using L1.Features.PartnerCommunicators.PlaceBet;

namespace L1.Features.SGCommunicators.PlaceBet
{
    public class SGPlaceBetCall : PartnerPlaceBetCall
    {
        public SGPlaceBetCall(
            int playerId,
            string siteId,
            int gameRoundId,
            int gameTypeId,
            int choiceId,
            decimal amount,
            string currency,
            DateTimeOffset? roundEndTime,
            string ip,
            bool freeSpin,
            decimal contributionAmount,
            float contributionRate,
            bool freeBet,
            string operatorToken,
            string operatorId,
            string userId)
            : base(playerId, siteId, gameRoundId, gameTypeId, choiceId, amount, currency, roundEndTime, ip, freeSpin, contributionAmount, contributionRate, freeBet, operatorToken, operatorId, userId)
        {
        }
    }
}
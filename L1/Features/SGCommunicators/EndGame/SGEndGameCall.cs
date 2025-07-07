using System;
using L1.Features.PartnerCommunicators.EndGame;

namespace L1.Features.SGCommunicators.EndGame
{
    public class SGEndGameCall : PartnerEndGameCall
    {
        public SGEndGameCall(
            int playerId,
            string siteId,
            int gameRoundId,
            int gameTypeId,
            decimal totalAmount,
            decimal totalWin,
            decimal validBetAmount,
            DateTimeOffset endTime,
            bool freeBet,
            object metaData,
            string promotionId
            ) 
            : base(
                  playerId,
                  siteId,
                  gameRoundId,
                  gameTypeId,
                  totalAmount,
                  totalWin,
                  validBetAmount,
                  endTime,
                  freeBet,
                  metaData,
                  promotionId
                  )
        {
        }
    }
} 
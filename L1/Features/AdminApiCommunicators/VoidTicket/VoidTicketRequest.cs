using L1.Shared.Extensions;
using System;

namespace L1.Features.AdminApiCommunicators.VoidTicket
{
    public class VoidTicketRequest : AdminApiRequest
    {
        public VoidTicketRequest(string playerId, string operatorId, string siteId, long gameRoundId, int gameTypeId, string reason, bool freeBet = false, bool callApi = true)
        {
            PlayerId = StringFormatExtensions.FormatPlayerId(operatorId, siteId, playerId);
            OperatorId = operatorId;
            SiteId = siteId;
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            Reason = reason;
            FreeBet = freeBet;
            CallApi = callApi;
        }

        public string PlayerId { get; }

        public string OperatorId { get; }

        public string SiteId { get; }

        public long GameRoundId { get; set; }

        public int GameTypeId { get; }

        public string Reason { get; }

        public bool FreeBet { get; }

        public bool CallApi { get; }
    }
}
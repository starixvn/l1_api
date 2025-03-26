using System;
using System.Text.Json.Serialization;
using L1.Features.PartnerCommunicators.Award;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;

namespace L1.Features.PartnerCommunicators
{
    [JsonDerivedType(typeof(PartnerPlaceBetCall))]
    [JsonDerivedType(typeof(PartnerGetBalanceCall))]
    [JsonDerivedType(typeof(PartnerEnterPortalCall))]
    [JsonDerivedType(typeof(PartnerEndGameCall))]
    [JsonDerivedType(typeof(PartnerAwardCall))]
    [JsonDerivedType(typeof(HitJackpotCall))]
    [JsonDerivedType(typeof(PartnerVoidGameCall))]
    [JsonDerivedType(typeof(PartnerGetBetLimitCall))]
    public interface IPartnerCall
    {
        string Seq { get; set; }

        object PlayerId { get; set; }

        string SiteId { get; set; }
    }

    public class PartnerCall : IPartnerCall
    {
        public PartnerCall(object playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public PartnerCall(string seq, object playerId, string siteId)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Seq = seq;
            SiteId = siteId ?? throw new ArgumentNullException(nameof(siteId));
            PlayerId = playerId;
        }

        public string Seq { get; set; }

        public object PlayerId { get; set; }

        public string SiteId { get; set; }

        public DateTimeOffset TimeStamp { get; }
    }
}
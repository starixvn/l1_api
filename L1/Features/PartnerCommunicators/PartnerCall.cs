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
using L1.Features.SGCommunicators.Award;
using L1.Features.SGCommunicators.EndGame;
using L1.Features.SGCommunicators.EnterPortal;
using L1.Features.SGCommunicators.GetBalance;
using L1.Features.SGCommunicators.GetBetLimit;
using L1.Features.SGCommunicators.PlaceBet;
using L1.Features.SGCommunicators.VoidGame;

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

    [JsonDerivedType(typeof(SGPlaceBetCall), nameof(SGPlaceBetCall))]
    [JsonDerivedType(typeof(SGGetBalanceCall), nameof(SGGetBalanceCall))]
    [JsonDerivedType(typeof(SGEnterPortalCall), nameof(SGEnterPortalCall))]
    [JsonDerivedType(typeof(SGEndGameCall), nameof(SGEndGameCall))]
    [JsonDerivedType(typeof(SGAwardCall), nameof(SGAwardCall))]
    [JsonDerivedType(typeof(SGVoidGameCall), nameof(SGVoidGameCall))]
    [JsonDerivedType(typeof(SGGetBetLimitCall), nameof(SGGetBetLimitCall))]
    public interface IPartnerCall
    {
        string Seq { get; set; }

        object PlayerId { get; set; }

        string SiteId { get; set; }
    }

    [JsonDerivedType(typeof(SGPlaceBetCall))]
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
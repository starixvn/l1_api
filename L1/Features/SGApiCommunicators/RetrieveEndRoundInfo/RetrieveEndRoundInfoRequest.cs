namespace L1.Features.SGApiCommunicators
{
    public class RetrieveEndRoundInfoRequest : SGRequest
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public int PlayerId { get; set; }

        public string SiteId { get; set; }
    }
}
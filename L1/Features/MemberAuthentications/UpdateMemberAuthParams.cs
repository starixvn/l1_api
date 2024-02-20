namespace L1.Features.MemberAuthentications
{
    public class UpdateMemberAuthParams
    {
        public string Token { get; set; }

        public string Language { get; set; }

        public int? GameTypeId { get; set; }

        public string CurrencyGroupId { get; set; }
    }
}
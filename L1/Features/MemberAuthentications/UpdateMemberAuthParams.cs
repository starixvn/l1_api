namespace L1.Features.MemberAuthentications
{
    public class UpdateMemberAuthParams
    {
        public string Token { get; set; }

        public string Language { get; set; }

        public byte? GameTypeId { get; set; }

        public string CurrencyGroupId { get; set; }
    }
}
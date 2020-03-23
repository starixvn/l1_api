namespace L1.WebApi.Models
{
    public class MemberEnterPortal : MemberRequest
    {
        public string CharacterName { get; set; }

        public string Seq { get; set; }

        public bool FirstLogin { get; set; }
    }
}
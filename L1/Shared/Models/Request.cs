namespace L1.Shared.Models
{
    public interface IRequest
    {
        string Seq { get; set; }

        int MemberId { get; set; }

        string SiteId { get; set; }
    }

    public class Request : IRequest
    {
        public string Seq { get; set; }

        public int MemberId { get; set; }

        public string SiteId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace L1.WebApi.Models
{
    public class GetBalance
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public string Seq { get; set; }

        [Required]
        public string SiteId { get; set; }
    }
}
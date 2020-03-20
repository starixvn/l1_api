using System.ComponentModel.DataAnnotations;

namespace L1.WebApi.Models
{
    public class MemberLogin
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public string CharacterName { get; set; }

        [Required]
        public string Seq { get; set; }

        [Required]
        public string SiteId { get; set; }

        public bool FirstLogin { get; set; }
    }
}

namespace L1.WebApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class MemberLogin
    {
        [Required]
        public string MemberId { get; set; }

        [Required]
        public string CharacterName { get; set; }
    }
}

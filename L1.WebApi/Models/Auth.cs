using System.ComponentModel.DataAnnotations;

namespace L1.WebApi.Models
{
    public class Auth
    {
        [Required]
        public string OperatorId { get; set; }

        public string OperatorSecret { get; set; }
    }
}
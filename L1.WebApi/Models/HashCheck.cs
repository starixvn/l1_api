using System.ComponentModel.DataAnnotations;

namespace L1.WebApi.Models
{
    public class HashCheck
    {
        [Required]
        public string SecretKey { get; set; }

        [Required]
        public string Data { get; set; }
    }
}
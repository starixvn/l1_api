using System;
using System.ComponentModel.DataAnnotations;

namespace L1.WebApi.Models
{
    public class StartPromotion : MemberRequest
    {
        [Required]
        public string PromotionCode { get; set; }
        
        [Required]
        public string Start { get; set; }
        
        [Required]
        public string End { get; set; }
    }
} 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WFHMicrosite.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "PIN")]
        public string PIN { get; set; }

        public int? UserId { get; set; }
        public int ProductId { get; set; }
        public byte[] Logo { get; set; }
    }
}
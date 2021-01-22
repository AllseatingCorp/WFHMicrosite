using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WFHMicrosite.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        public int SiteId { get; set; }
        public int ProductId { get; set; }
        public string Language { get; set; }
    }
}
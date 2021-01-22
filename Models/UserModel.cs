using System;
using System.ComponentModel.DataAnnotations;

namespace WFHMicrosite.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string EmailAddress { get; set; }
        public string Language { get; set; }
        public int Quantity { get; set; }
        public string Pin { get; set; }
        public string TrackingNumber { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string AttnName { get; set; }

        [Display(Name = "Phone Number")]
        [Required]
        public string PhoneNumber { get; set; }

        [Display(Name = "Street Name")]
        [Required]
        public string Address1 { get; set; }

        [Display(Name = "Apt./House No.")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        [Required]
        public string City { get; set; }

        [Display(Name = "Province")]
        [Required]
        public string ProvinceState { get; set; }

        [Display(Name = "Postal Code")]
        [Required]
        public string PostalZip { get; set; }

        [Display(Name = "Country")]
        [Required]
        public string Country { get; set; }

        [Display(Name = "Instructions")]
        public string SpecialInstructions { get; set; }

        [Display(Name = "Commercial Property")]
        public bool Commercial { get; set; }

        public DateTime? Emailed { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? InProduction { get; set; }
        public DateTime? Shipped { get; set; }
    }
}

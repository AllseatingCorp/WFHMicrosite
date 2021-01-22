using System;
using System.Collections.Generic;

namespace WFHMicrosite.Models
{
    public partial class UserSelectionModel
    {
        public int UserSelectionId { get; set; }
        public int UserId { get; set; }
        public int ProductOptionId { get; set; }
        public string Type { get; set; }
    }
}

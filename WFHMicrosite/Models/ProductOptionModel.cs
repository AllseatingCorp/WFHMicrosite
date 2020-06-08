using System;
using System.Collections.Generic;

namespace WFHMicrosite.Models
{
    public class ProductOptionModel
    {
        public int ProductOptionId { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public byte[] Image { get; set; }
        public bool Default { get; set; }
    }
}

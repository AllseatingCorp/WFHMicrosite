using System;
using System.Collections.Generic;

namespace WFHMicrosite.Models
{
    public partial class ProductImageModel
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public int ProductOption1Id { get; set; }
        public int ProductOption2Id { get; set; }
        public string FileName { get; set; }
        public byte[] Image { get; set; }
    }
}

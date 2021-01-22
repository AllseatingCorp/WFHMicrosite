using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFHMicrosite.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Default { get; set; }
        public string InstallGuide { get; set; }
        public string UserGuide { get; set; }
        public string VideoUrl { get; set; }
    }
}

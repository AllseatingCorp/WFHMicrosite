using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFHMicrosite.Models
{
    public class UserProductModel
    {
        public UserModel User {get;set;}
        public ProductModel Product { get; set; }
        public List<ProductOptionModel> FabricOptions { get; set; }
        public List<ProductOptionModel> MeshOptions { get; set; }
        public List<UserSelectionModel> UserSelections { get; set; }
        public ProductImageModel ProductImage { get; set; }
        public int FabricId { get; set; }
        public int MeshId { get; set; }
        public string Status { get; set; }
    }
}

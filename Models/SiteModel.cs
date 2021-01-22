using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFHMicrosite.Models
{
    public class SiteModel
    {
        public int SiteId { get; set; }
        public string DealerCode { get; set; }
        public string Ponumber { get; set; }
        public string Language { get; set; }
        public string LogoFile { get; set; }
        public byte[] LogoImage { get; set; }
        public string LogoFile2 { get; set; }
        public byte[] LogoImage2 { get; set; }
        public string SitFitGuide { get; set; }
        public string Shipper { get; set; }
        public bool ManagersApproval { get; set; }
        public bool Completed { get; set; }
    }
}

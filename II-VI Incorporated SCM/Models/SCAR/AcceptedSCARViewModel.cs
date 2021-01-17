using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SCAR
{
    public class AcceptedSCARViewModel
    {
        public string SCARID { get; set; }
        public List<string> ListD { get; set; }
        public HttpPostedFileBase SCARFILE { get; set; }
    }
}
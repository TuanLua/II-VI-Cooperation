using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class EvidenceView
    {
        public HttpPostedFileBase EvidenceFile { get; set; }
        public bool IsPrint { get; set; }
    }
}
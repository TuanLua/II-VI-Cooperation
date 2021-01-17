using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class DISPOSITIONViewModel
    {
        public string Item { get; set; }
        public string Disposition { get; set; }
        public HttpPostedFileBase FileAttach { get; set; }
        public string Message { get;set; }
        public bool IsSave { get; set; }
        public string FileAttachName { get; set; }
        public string Type { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class SCRAP_REASONView
    {
        public int ID { get; set; }
        public string NCR_NUMBER { get; set; }
        public string ITEM { get; set; }
        public string TYPE { get; set; }
        public string ADDINS { get; set; }
        public string REASON { get; set; }
    }
}
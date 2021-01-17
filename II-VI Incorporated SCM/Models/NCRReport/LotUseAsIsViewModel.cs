using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCRReport
{
    public class LotUseAsIsViewModel
    {
        public string RECEIVER { get; set; }
        public DateTime DATE { get; set; }
        public DateTime? DATE_APPROVE { get; set; }
        public string PARTNUMBER { get; set; }
        public string ITEM_DESC { get; set; }
        public string VEN_NAME { get; set; }
        public double? QTY_DISDET { get; set; }
        public string NCRNUM { get; set; }
        public string REMARK_DISDET { get; set; }
    }
}
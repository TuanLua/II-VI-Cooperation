using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models
{
    public class ReciverPro
    {
        public string ITEM { get; set; }
        public string ITEM_DESC { get; set; }
        public string STOCK_UM { get; set; }
        public string VENDOR { get; set; }
        public string VEN_NAME { get; set; }
        public string PO_NUM { get; set; }
        public string PO_LINE { get; set; }
        public string QS_STAT { get; set; }
        public string RECEIVER1 { get; set; }
        public string REC_LINE { get; set; }
        public string LOT { get; set; }
        public string HALT { get; set; }
        public double QTY { get; set; }
        public System.DateTime POSTING_DATE { get; set; }
        public string USER_ { get; set; }
        public System.DateTime USER_DATE { get; set; }
        public string PC_NAME { get; set; }
        public string STATUS { get; set; }
        public Nullable<double> BATCH_NO { get; set; }
        public string REC_COMMENT { get; set; }
        public string CCN { get; set; }
    }
}
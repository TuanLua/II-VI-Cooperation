using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class NcrSearchViewModelProcess
    {
        public string ITEM { get; set; }
        public string ITEM_DESC { get; set; }
        public string STOCK_UM { get; set; }
        public string VENDOR { get; set; }
        public string VEN_NAME { get; set; }
        public string PO_NUM { get; set; }
        public string PO_LINE { get; set; }
        public string QS_STAT { get; set; }
        public string DRAW_REV { get; set; }
        public string RECEIVER { get; set; }
        public string REC_LINE { get; set; }
        public string LOT { get; set; }
        public string HALT { get; set; }
        public double QTY { get; set; }
        public System.DateTime POSTING_DATE { get; set; }
        public string USER_ { get; set; }
        public System.DateTime USER_DATE { get; set; }
        public string PC_NAME { get; set; }
        public double SAMPLING_VISUAL { get; set; }
        public double SAMPLING_MEASURE { get; set; }
        public string SEC { get; set; }
        public string SERIAL_CTL { get; set; }
        public string SKIP_LOT_PLAN { get; set; }
        public string SKIP_LOT_LEVEL { get; set; }
        public string INSP_STAT { get; set; }
        public string DOC_REQ { get; set; }
        public string STATUS { get; set; }
        public string QUAL { get; set; }
        public string SKIP_LEVEL { get; set; }
        public Nullable<int> LOT_SKIP { get; set; }
        public string AQL_VISUAL { get; set; }
        public string AQL_MEASURE { get; set; }
        public string TEST_ROSH_REQUIRED { get; set; }
        public string CCN { get; set; }
        public Nullable<double> PartialAcc { get; set; }
        public Nullable<double> PartialIns { get; set; }
        public Nullable<double> PartialRej { get; set; }
        public string ADDRESS { get; set; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string CTRY { get; set; }
        public string Inspector { get; set; }
    }
}
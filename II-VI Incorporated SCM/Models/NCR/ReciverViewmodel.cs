using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class ReciverViewmodel
    {
        public string ITEM { get; set; }
        public string PO_NUM { get; set; }
        public string RECEIVER1 { get; set; }
        public string LOT { get; set; }
        public double QTY { get; set; }
        public string AQL_VISUAL { get; set; }
        public string AQL_MEASURE { get; set; }
        public double SAMPLING_MEASURE { get; set; }
        public double SAMPLING_VISUAL { get; set; }
        public Nullable<double> REC_QTY { get; set; }
        public Nullable<double> REJ_QTY { get; set; }
        public string VENDOR { get; set; }
        public string VEN_NAME { get; set; }
        public string VEN_ADD { get; set; }
        public Nullable<double> INS_QTY { get; set; }
        public string ITEM_DESC { get; set; }
        public string DRAW_REV { get; set; }
        public string SKIP_LEVEL { get; set; }
        public string MODEL_NO { get; set; }
        public string INSPECTOR { get; set; }
        public string AQL { set; get; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string CTRY { get; set; }
        public double NC_Qty { get; set; }
        public string NCR_Num { get; set; }
        public double defective { get; set; }
        public List<INS_RESULT_DEFECTViewModel> Listdefect { get; set; }
    }
}
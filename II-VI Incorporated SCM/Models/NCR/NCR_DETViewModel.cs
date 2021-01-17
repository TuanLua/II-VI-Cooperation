using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    //public class NCR_DETViewModel 
    //{
    //    public string NCR_NUM { get; set; }
    //    public string SEC { get; set; }
    //    public string ITEM { get; set; }
    //    public double QTY { get; set; }
    //    public List<string> NC_DESC { get; set; }
    //    public List<string> DEFECT { get; set; }
    //    public string RESPONSE { get; set; }
    //    public string DISPOSITION { get; set; }
    //    public string REMARK { get; set; }
    //    public Nullable<System.DateTime> DATEAPPROVAL { get; set; }
    //    public string defect { get; set; }
    //    public string conform { get; set; }
    //}

    public class NCR_DETViewModel
    {
        public string NCR_NUM { get; set; }
        public string SEC { get; set; }
        public string ITEM { get; set; }
        public double QTY { get; set; }
        public List<string> NC_DESC { get; set; }
        // list string description
        public string NC_DESC_STRING { get; set; }
        public List<string> DEFECT { get; set; }
        // list string defect
        public string DEFECT_STRING { get; set; }
        public string PARTNUM { get; set; }
        public string RESPONSE { get; set; }
        public string RESPONSENAME { get; set; }
        public string DISPOSITION { get; set; }
        public string DISPOSITIONNAME { get; set; }
        public string REMARK { get; set; }
        public Nullable<System.DateTime> DATEAPPROVAL { get; set; }
       // public string defect { get; set; }
        public string conform { get; set; }
    }


    public partial class INS_RESULT_DEFECTViewModel
    {
        public string receiver { get; set; }
        public string rec_line { get; set; }
        public string PartialID { get; set; }
        public List<string> Non_Conformances { get; set; }
        public double NC_Qty { get; set; }
        public List<string> Defect { get; set; }
        public string Response { get; set; }
        public string Disposition { get; set; }
        public string NC_DESC_STRING { get; set; }
        public string DEFECT_STRING { get; set; }
        public string Remark { get; set; }
        public string NCR_Num { get; set; }
        public string Picture { get; set; }
        public string CCN { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SCAR
{
    public class SCARInfoViewModel
    {
        public int ID { get; set; }
        public string SCAR_ID { get; set; }
        public string VENDOR { get; set; }
        public int NON_QTY { get; set; }
        public string RMA { get; set; }
        public string NCR_NUMBER { get; set; }
        public List<string> LstNCRNum { get; set; }
        public string PO_NUMBER { get; set; }
        public string ITEM { get; set; }
        public string BUYER { get; set; }
        public string QUALITY { get; set; }
        public string VN_SCAR { get; set; }
        public string VN_NCR { get; set; }
        public string PROBLEM { get; set; }
        public DateTime DATEPROBLEM { get; set; }
        public DateTime DATERESPOND { get; set; }
        public string WRITTENBY { get; set; }
        public string WRITTENNAMEBY { get; set; }
        public DateTime WRITTENDATE { get; set; }
        public string STATUS { get; set; }
        public string VERSION { get; set; }
        public string MI_PART_NO { get; set; }
        public string LOT { get; set; }
        public string VEN_NAME { get; set; }
        public string CONTENT { get; set; }
        public string SUPPLIER_REPRESENTATIVE { get; set; }
        public Nullable<System.DateTime> DATE_D8 { get; set; }
        public string ACKNOWLEDGEMENT { get; set; }
        public string SCAR_STATUS { get; set; }
        public string EDIVENCE_D8 { get; set; }
        public string DEFECT { get; set; }
        public string RECURING_PROBLEM { get; set; }
        public string CATEGORY { get; set; }
    }
}
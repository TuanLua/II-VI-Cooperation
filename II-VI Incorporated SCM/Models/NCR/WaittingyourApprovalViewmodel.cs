using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class WaittingyourApprovalViewmodel
    {
        public string NCRNUM { get; set; }
        public DateTime? DateApproval { get; set; }
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string INSPECTORNAME { get; set; }
        public string FORM { get; set; }
        public string REV { get; set; }
        public string NCR_NUM { get; set; }
        public string SEC { get; set; }
        public string MI_PART_NO { get; set; }
        public string DRAW_REV { get; set; }
        public string PO_NUM { get; set; }
        public string RECEIVER { get; set; }
        public string LOT { get; set; }
        public string ITEM_DESC { get; set; }
        public string VENDOR { get; set; }
        public string VEN_NAME { get; set; }
        public string VEN_ADD { get; set; }
        public string INS_PLAN { get; set; }
        public string SKIP_LOT_LEVEL { get; set; }
        public string FAI { get; set; }
        public Nullable<double> REC_QTY { get; set; }
        public Nullable<double> INS_QTY { get; set; }
        public Nullable<double> REJ_QTY { get; set; }
        public string INSPECTOR { get; set; }
        public Nullable<System.DateTime> INS_DATE { get; set; }
        public string CORRECT_ACTION { get; set; }
        public string SCAR_NUM { get; set; }
        public Nullable<System.DateTime> SCAR_DATE { get; set; }
        public string SCAR_BY { get; set; }
        public string FOLLOW_UP_NOTES { get; set; }
        public string QA_PIC { get; set; }
        public Nullable<System.DateTime> QA_DATE { get; set; }
        public string EN_PIC { get; set; }
        public string MFG_PIC { get; set; }
        public Nullable<System.DateTime> MFG_DATE { get; set; }
        public Nullable<System.DateTime> DATESUBMIT { get; set; }
        public string USERSUBMIT { get; set; }
        public string PUR_PIC { get; set; }
        public string PUR_DATE { get; set; }
        public string MODEL_NO { get; set; }
        public string TYPE_NCR { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP_CODE { get; set; }
        public Nullable<bool> SAMPLE_INSP { get; set; }
        public string AQL { get; set; }
        public string LEVEL { get; set; }
        public Nullable<bool> PERCENT_INSP { get; set; }
        public Nullable<bool> FIRST_ARTICLE { get; set; }
        public string STATUS { get; set; }
        public string CCN { get; set; }
        public float QTY { get; set; }
        public int AGE { get; set; }
        public bool Selection { get; set; }
        public Nullable<bool> NOT_REQUIRED { get; set; }
        public Nullable<bool> REQUIRED { get; set; }
        public Nullable<bool> NOTIFICATION_ONLY { get; set; }
        public string ISSUED_REQUEST_NO { get; set; }
        public Nullable<System.DateTime> ISSUED_REQUEST_DATE { get; set; }
        public string RETURN_NUMBER { get; set; }
        public string REMOVED_FROM { get; set; }
        public string BOOK_INV { get; set; }
        public string ISSUE_MEMO_NO { get; set; }
        public Nullable<System.DateTime> ISSUE_MEMO_DATE { get; set; }
        public string NOTES { get; set; }
        public string SHIPPING_METHOD { get; set; }
        public string AQL_VISUAL { get; set; }
        public double defect { get; set; }
    }
}
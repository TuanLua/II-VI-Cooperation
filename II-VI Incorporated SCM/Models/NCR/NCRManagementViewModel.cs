using II_VI_Incorporated_SCM.Library.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class NCRManagementViewModel
    { 
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
        public DateTime? DateConform { get; set; }
        public string STATUS { get; set; }
        public string CCN { get; set; }
        public float QTY { get; set; }
        public int AGE { get; set; }
        public double? AGENCR { get; set; }
        public TimeSpan? AGing { get; set; }
        public bool Selection { get; set; }
        public string Taskname { get; set; }
        public string Description { get; set; }
        public string Assignee { get; set; }
        public string Priorty { get; set; }
        public string StatusTask { get; set; }
        public DateTime? DueDate { get; set; }
        public int? TaskID { get; set; }
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
        public double? Amount { get; set; }
        public DateTime DateApproval { get; set; }
        public Nullable<double> PartialRej { get; set; }
        public Nullable<double> PartialIns { get; set; }
        public Nullable<double> PERCENT { get; set; }
        public List<INS_RESULT_DEFECTViewModel> Listdefect { get; set; }
        public List<NCR_DETViewModel> Listdefectprocess { get; set; }
        public List<NCR_DETViewModel> NCRDETs { get; set; }

        public List<UserApproval> ListUSerAppr { get; set; }
        public List<NcrDisViewmodel> ListAdditional { get; set; }
        public List<NC_GROUP> ListNC_Group { get; set; }
        public List<RESPON> ListRespon { get; set; }
        public List<DISPOSITION> ListDispo { get; set; }

        public List<ADD_INS> ListAddition { get; set; }
        // public  List<NCR_DET> ListncNcrDets { get; set; }
        public string Notrequered { get; set; }
        public string requered { get; set; }
        public string notification { get; set; }
        public List<EvidenceView> ModelEvidence { get; set; }
        public List<NCR_EVI> OldEvidence { get; set; }
        public HttpPostedFileBase VNMaterialTraceability { get; set; }
        public string OldVNMaterialTraceability { get; set; }
        public List<string> EVIID { get; set; }
        public IEnumerable<NCR_DETViewModel> ListNCR_DET { get; set; }
        public List<UserApproveViewModel> UserApprove { get; set; }
        public List<string> RoleIDs { get; set; }
        public List<DISPOSITIONViewModel> DISPOSITION { get; set; }
        public List<DISPOSITIONViewModel> ADDIN { get; set; }

        public List<NCR_DISViewModel> NCRDISs { get; set; }
        public string Comment { get; set; }
        public string MRB_LOC { get; set; }
        public string Userapproval { get; set; }
        public long[] SizeOfOldEvidence { get; set; }

        public NCRManagementViewModel()
        {
            ModelEvidence = new List<EvidenceView>();
            DISPOSITION = new List<DISPOSITIONViewModel>();
            SizeOfOldEvidence = new long[] { };
            ADDIN = new List<DISPOSITIONViewModel>();
        }
    }

    public class UserApproval
    {

        public string Id { get; set; }
        public string IdUser { get; set; }
        public string FullName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAppr { get; set; }
        public string DateAppr { get; set; }
        public string Signature { get; set; }
    }

    public class ItemRemark
    {
        public string ITEM { get; set; }
        public string Remark { get; set; }
    }
    public class EditIQC
    {
        public string NCR_NUM { get; set; }
        public List<ItemRemark> Remark { get; set; }
    }

}
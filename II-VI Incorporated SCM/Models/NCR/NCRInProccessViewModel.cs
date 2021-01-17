using II_VI_Incorporated_SCM.Library.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class NCRInProccessViewModel
    {
        public string NCR_NUM { get; set; }
        public string SEC { get; set; }
        public string ITEM { get; set; }
        public double QTY { get; set; }
        public string NC_DESC { get; set; }
        public string DEFECT { get; set; }
        public string RESPONSE { get; set; }
        public string DISPOSITION { get; set; }
        public string REMARK { get; set; }
        public string ADD_INS { get; set; }
        public string INSPECTOR { get; set; }
        public Nullable<System.DateTime> INS_DATE { get; set; }
        public string MI_PART_NO { get; set; }
        public string DRAW_REV { get; set; }
        public string PO_NUM { get; set; }
        public string RECEIVER { get; set; }
        [Required]
        public string LOT { get; set; }
        public string ITEM_DESC { get; set; }
        public string VENDOR { get; set; }
        public string VEN_NAME { get; set; }
        public string VEN_ADD { get; set; }
        public string INS_PLAN { get; set; }
        public string SKIP_LOT_LEVEL { get; set; }
        public string FAI { get; set; }
        [Required]
        public double REC_QTY { get; set; }
        [Required]
        public double INS_QTY { get; set; }
        [Required]
        public double REJ_QTY { get; set; }
        [Required]
        public double Defect_QTY { get; set; }
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
        public string PUR_PIC { get; set; }
        public string PUR_DATE { get; set; }
        public string MODEL_NO { get; set; }
        public string TYPE_NCR { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP_CODE { get; set; }
        public bool SAMPLE_INSP { get; set; }
        public string AQL { get; set; }
        public string LEVEL { get; set; }
        public bool PERCENT_INSP { get; set; }
        public bool FIRST_ARTICLE { get; set; }
        public string File_Upload_Url { get; set; }
        public string STATUS { get; set; }
        public string CCN { get; set; }

        [Required]
        [MaximumFileSizeValidator(10)]
        [ValidFileTypeValidator("pdf", "PDF")]
        public HttpPostedFileBase VNMaterialTraceability { get; set; }
        public IEnumerable<NCR_DETViewModel> ListNCR_DET { get; set; }
        public string QUALITYASSURANCE { get; set; }
        public string AFG { get; set; }
        public string PURCHASING { get; set; }
        public string ENGINEERING { get; set; }
        public string Comment { get; set; }
        public List<EvidenceView> ModelEvidence { get; set; }
        public List<UserApproveViewModel> UserApprove { get; set; }

        public NCRInProccessViewModel()
        {
            ModelEvidence = new List<EvidenceView>();
            UserApprove = new List<UserApproveViewModel>();
        }
    }

    //public class NCRCreate
    //{
    //    public NCRInIQCViewModel IQC { get; set; }
    //    public NCRInProccessViewModel Process { get; set; }
    //}

    public class NCRInIQCViewModel
    {
        public string NCR_NUM1 { get; set; }
        public string SEC1 { get; set; }
        public string ITEM1 { get; set; }
        public double QTY1 { get; set; }
        public string NC_DESC1 { get; set; }
        public string DEFECT1 { get; set; }
        public string RESPONSE1 { get; set; }
        public string DISPOSITION1 { get; set; }
        public string REMARK1 { get; set; }
        public string ADD_INS1 { get; set; }
        public string INSPECTOR1 { get; set; }
        public Nullable<System.DateTime> INS_DATE1 { get; set; }
        public string MI_PART_NO1 { get; set; }
        public string DRAW_REV1 { get; set; }
        public string PO_NUM1 { get; set; }
        public string RECEIVER1 { get; set; }
        public string LOT1 { get; set; }
        public string ITEM_DESC1 { get; set; }
        public string VENDOR1 { get; set; }
        public string VEN_NAME1 { get; set; }
        public string VEN_ADD1 { get; set; }
        public string INS_PLAN1 { get; set; }
        public string SKIP_LOT_LEVEL1 { get; set; }
        public string FAI1 { get; set; }
        public double REC_QTY1 { get; set; }
        public Nullable<double> INS_QTY1 { get; set; }
        public Nullable<double> REJ_QTY1 { get; set; }
        public string CORRECT_ACTION1 { get; set; }
        public string SCAR_NUM1 { get; set; }
        public Nullable<System.DateTime> SCAR_DATE1 { get; set; }
        public string SCAR_BY1 { get; set; }
        public string FOLLOW_UP_NOTES1 { get; set; }
        public string QA_PIC1 { get; set; }
        public Nullable<System.DateTime> QA_DATE1 { get; set; }
        public string EN_PIC1 { get; set; }
        public string MFG_PIC1 { get; set; }
        public Nullable<System.DateTime> MFG_DATE1 { get; set; }
        public string PUR_PIC1 { get; set; }
        public string PUR_DATE1 { get; set; }
        public string MODEL_NO1 { get; set; }
        public string TYPE_NCR1 { get; set; }
        public string CITY1 { get; set; }
        public string STATE1 { get; set; }
        public string ZIP_CODE1 { get; set; }
        public bool SAMPLE_INSP1 { get; set; }
        public string AQL1 { get; set; }
        public string LEVEL1 { get; set; }
        public bool PERCENT_INSP1 { get; set; }
        public bool FIRST_ARTICLE1 { get; set; }
        public string File_Upload_Url1 { get; set; }
        public string STATUS1 { get; set; }

        [Required]
        [MaximumFileSizeValidator(10)]
        [ValidFileTypeValidator("pdf", "PDF")]
        public HttpPostedFileBase VNMaterialTraceability { get; set; }
        public IEnumerable<NCR_DETViewModel> nonComformity { get; set; }
        public string AQL_VISUAL1 { get; set; }
        public string SAMPLING_VISUAL1 { get; set; }
        public string SAMPLING_MESURE1 { get; set; }
        public string AQL_MEASURE1 { get; set; }
        public string CCN1 { get; set; }
        public string QUALITYASSURANCE { get; set; }
        public string AFG { get; set; }
        public string PURCHASING { get; set; }
        public string ENGINEERING { get; set; }
        public double defective { get; set; }
        public List<INS_RESULT_DEFECTViewModel> listdefectiqc { get; set; }
        public List<EvidenceView> ModelEvidence { get; set; }
        public List<UserApproveViewModel> UserApprove { get; set; }
        public string Comment { get; set; }

        public NCRInIQCViewModel()
        {
            ModelEvidence = new List<EvidenceView>();
            UserApprove = new List<UserApproveViewModel>();
        }
    }
}
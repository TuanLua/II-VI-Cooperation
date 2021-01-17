using II_VI_Incorporated_SCM.Library.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class ChangeItemViewmodel
    {
        public ChangeItemViewmodel()
        {

        }
        public ChangeItemViewmodel(string SubmitName)
        {
            this.Submitername = SubmitName;
        }

        public string CRNo { get; set; }
        public string REF_NUM { get; set; }
        public string Submitername { set; get; }
        public string Brief{ get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DateRequired { get; set; }
        public string Priority { get; set; }
        public string Reason { get; set; }
        public string OtherAtifact { get; set; }
        public string Comments { get; set; }
        // [MaximumFileSizeValidator(10)]
        public HttpPostedFileBase Attacment { get; set; }
       public DateTime? WHacnowledDate { get; set; }
        public string CRStatus { get; set; }
        public string ApprovalDate { get; set; }
        public string Chermaincomment { get; set; }
        public string Chermainname { get; set; }
        public string Linkactack { get; set; }
        public DateTime? DateChairMain { get; set; }
        public string WHMRBname { get; set; }
        public int idfile { get; set; }
        public DateTime ? DueDate { get; set; }
    }
}
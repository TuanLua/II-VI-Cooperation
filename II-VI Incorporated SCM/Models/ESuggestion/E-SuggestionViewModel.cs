using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.ESuggestion
{
    public class E_SuggestionViewModel
    {
        public string ESNum { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
    }
    public class E_SuggestionViewModelSimilar
    {
        public string ID { get; set; }
        public string Title { get; set; }
        //public DateTime Date { get; set; }
        public string Content { get; set; }
    }
    public class E_SuggestionCreateViewmodel
    {
        public string Sug_ID { get; set; }
        public string Sug_Type { get; set; }
        public string Sug_title { get; set; }
        public string Cur_prob { get; set; }
        public string Idea_impr { get; set; }
        public string Cur_Step { get; set; }
        public string Step_Status { get; set; }
        public Nullable<System.DateTime> Exp_Start_Date { get; set; }
        public Nullable<System.DateTime> Exp_End_date { get; set; }
        public Nullable<System.DateTime> Submit_date { get; set; }
        public string Submitter { get; set; }
        public string Requestor { get; set; }
        public string rqtor_name { get; set; }
        public string Req_Dept { get; set; }
        public string Req_Email { get; set; }
        public List<EvidenceView> ModelEvidence { get; set; }
        public List<EvidenceView> ModelEvidenceIdea { get; set; }
        public HttpPostedFileBase FileAttach { get; set; }
        public List<string> FileName { get; set; }
    }
    public class ESuggestinSponsorViewModel
    {
        public string Sug_ID { get; set; }
        public string ProLeader { get; set; }
        public DateTime? Imp_Start { get; set; }
        public DateTime? Imp_End { get; set; }
        public bool Tech_abi { get; set; }
        public string Tech_com { get; set; }
        public bool Eco_ben { get; set; }
        public string Eco_com { get; set; }
        public bool Legal_Rel { get; set; }
        public string Legal_com { get; set; }
        public bool Res_avai { get; set; }
        public string Res_com { get; set; }
        public bool Safe_Rel { get; set; }
        public string Safe_com { get; set; }
        public bool Fin_abi { get; set; }
        public string Fin_com { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string ProLeaderValue { get; set; }
    }
    public class BoardirectorViewmodel
    {
        public string Sug_ID { get; set; }
        public string Director { get; set; }
        public bool App_eva { get; set; }
        public string App_com { get; set; }
        public bool Stra_Link { get; set; }
        public string Stra_com { get; set; }
        public bool Apr_Status { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
    }
    public class ProcessingViewModel
    {
        public string Sug_ID { get; set; }
        public string Sug_title { get; set; }
        public string Imp_Method { get; set; }
        public string Sponsor { get; set; }
        public string Coacher { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public List<string> Board { get; set; }
    }
    public class LeaderViewmodel
    {
        public string Sug_ID { get; set; }
        public List<string> Member { get; set; }
        public string Status { get; set; }
    }
    public class CostSavingmodel
    {
        public string Sug_ID { get; set; }
        public float Jan { get; set; }
        public float Feb { get; set; }
        public float Mar { get; set; }
        public float Apr { get; set; }
        public float May { get; set; }
        public float Jun { get; set; }
        public float Jul { get; set; }
        public float Aug { get; set; }
        public float Sep { get; set; }
        public float Oct { get; set; }
        public float Nov { get; set; }
        public float Dec { get; set; }
        public string User_Input { get; set; }
        public DateTime Date_Input { get; set; }
    }
    public class SuggestionReport
    {
        public DateTime dtFrom { get; set; }
        public DateTime dtTo { get; set; }
        public string dept { get; set; }
        public string ImpMeth { get; set; }
    }
    public class SuggestionReportModel
    {
        public string Req_Dept { get; set; }
        public string Imp_Method { get; set; }
        public int QTY { get; set; }
    }
    public class RoleModel
    {
        public string User_ID { get; set; }
        public string User_Role { get; set; }
        public string FullName { get; set; }
    }
}
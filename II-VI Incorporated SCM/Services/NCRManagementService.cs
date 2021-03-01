using II_VI_Incorporated_SCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using II_VI_Incorporated_SCM.Models.NCR;
using System.IO;
using System.Security.Cryptography;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using Kendo.Mvc.UI;
using Microsoft.Owin.Security.Provider;
using II_VI_Incorporated_SCM.Models.RTV;
using Microsoft.AspNet.Identity;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using System.Security.Policy;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using II_VI_Incorporated_SCM.Extensions;

namespace II_VI_Incorporated_SCM.Services
{

    public interface INCRManagementService
    {

        List<NCRManagementViewModel> GetListNCRByVendor(string vendor);
        List<ADD_INS> getListAddition();
        List<NCR_HDR> GetListNCRApproval(string id);
        bool CheckDubplicateNCRCreate(string receiver);
        List<NCRManagementViewModel> GetListNCR();
        List<string> GetListLOTByPN(string pn,string CCN);
        NCRManagementViewModel GetCreateNCR(string NCR_NUM);
        void UpdateNCRForIQC(NCRManagementViewModel model);
        void UpdateNCRForProcess(NCRManagementViewModel model);
        List<NCR_DETViewModel> GetInresultProcess(string NCR_NUM,string CCN);
        void updatedateapprovalNCR(string ncrnum, bool final);
        Result UpdateUserApproval(string id, string userId);
        Result UpdateUserAprrovalDate(string Id, string ncrnum, string id, string password);
        //tuan lua add 10-04 
        List<NCRManagementViewModel> getListDispositionpartial(string iduser);

        //bool IsExistNCRNumber(string ncrnumber);
        bool CheckhistoryNCR(string NCRNUM);
        bool IsExistVendor(string vendor);
        VENDOR getvendor(string vendor);
        bool AddListNCRDIS1(List<NCR_DIS> lst);

        bool UpdateNCRDET(string ncrnum, List<NCR_DET> list);
        bool IsExistReceiver(string receiver);

        List<SelectListItem> GetdropdownVendors();
        string GetFile(string NCR_Num);
        string getUrlNcr(string NCR_Num);

        List<DropdownlistViewModelPrint> GetdropdownNCRnum(string vendor);
        // void CreateNCR(NCR_HDR model);
        bool AddUserApproval(APPROVAL model);
        NCR_EVI GetFileWithFileID(int fileId);

        ApprovalDetViewmodel GetListUserApprovalByNcrNum(string ncrnum);
        bool UpdateStatusNCR(string ncrnumber, string status, string userid);
        string GetStatusNRCByNCRNUM(string NCR_NUM);
        bool SaveNewDefect(List<NCR_DET> list);
        bool DeleteAllDefectByNcrNum(string ncrnum);
        string GetNameStatusById(string id);
        bool SaveNCRHDRByDispositionModel(DispositionModel model);
        int getIDfile(string NCR_Num);
        bool AddListNCRDIS(List<NCR_DIS> lst);
        List<AspNetUser> GetUser();
        void updateAgeNCRinListNCR(List<NCRManagementViewModel> listncr);
        List<NcrDisViewmodel> GetAdditional(string NCR_NUM);

        List<NcrSearchViewModelProcess> GetDataInputIQCInProcessByLOTAndPartNum(string lot, string partnum,string CCN);
        List<NCR_DISViewModel> GetNCRDISsHistory(string nCR_NUM, string CRno);
        List<string> GetListLOTByWOAndPartNum(string wo, string partnum);
        List<string> GetListSerialByWOAndPartNum(string serial, string partnum);
        List<NC_GROUP> GetListNC_GRP_DESC(string ccn);
        List<RESPON> getListRESPON();
        List<DISPOSITION> getListDispo();
        List<DISPOSITION> getListDispo_4_PrintNCR();
        string GetFullUserNameById(string id);
        string GetAutoNCRNUM(string code);
        List<sp_GetNCRAging_Result1> GetListNCRAge();

        List<NCR_DETViewModel> GetInresultProcessString(string NCR_NUM);
        List<NCRManagementViewModel> GetListNCR_RTV();
        List<string> GetListPOByLOT(string lot, string pn,string CCN);
        List<NcrSearchViewModelProcess> GetDataInputIQCInProcessByPONumAndLOT(string lot, string po, string pn,string CCN);
        List<INS_RESULT_DEFECTViewModel> GetListInresultIqc(string receiver);
        List<SCRAPCategory> GetSCRAPCategory();
        List<string> cutStringP(string str);

        bool checkexistsRTVProcess(string NCR_NUM);
        bool SaveRTVProcess(RTVProccessViewModel rtv);
        string[] GetCARequestNO(string nCR_NUM);
        string getRTVstatus(string NCR_NUM);
        RTVProccessViewModel getRTV(string NCR_NUM);
        bool UpdateRTVProcess(RTVProccessViewModel rtv);
        bool updatestatus(string NCR_NUM);
        string SentEmailSubmitEdit(string id);
        bool GetOPEOwner(string currentId, string ncrNum);
        bool CreateTaskManNCR(TaskManagementCreateModel model);
        bool DeleteAddIns(string ncrNum, string item);
        bool EditAddIns(string ncrNum, string item, double qty, string remark = "");
        CApproval CheckApproval(string ncrNum);
        void CloseNCR(string NCR_NUM, string uid);
        List<NCR_EVI> GetEVIByIds(List<string> eviIds);
        List<NCR_DISViewModel> GetNCRDISs(string nCR_NUM);
        List<DISPOSITIONViewModel> getOrderDisposition(string nCR_NUM);
        List<NCR_EVI> GetEVIByNCRNUM(List<string> lstNCRNUM);
        Result CreateNCRForIQC(NCR_HDR nCR_HDR, List<NCR_DET> nCR_DETs, List<NCR_EVI> nCR_EVIs, List<APPROVAL> aPPROVALs, 
            List<TaskManagementCreateModel> taskManagementCreateModels, VNMaterialTraceability vNMaterialTraceability, string RECEIVER);
        float GetSamplingSize(double lotsize);
        Result CreateNCRForInProcess(NCR_HDR nCR_HDR, List<NCR_DET> nCR_DETs, List<NCR_EVI> nCR_EVIs, List<APPROVAL> aPPROVALs,
            List<TaskManagementCreateModel> taskManagementCreateModels, VNMaterialTraceability vNMaterialTraceability, string RECEIVER, INS_RESULT_FINAL iNS_RESULT_FINAL);
        NCR_HDR GetNCR_HDR(string ncr_num);
        string GetNCR_DET_DESC(string ncr_num);
        List<NCR_EVI> GetUploadedEvidence(string ncrnum);
        string GetStringUploadedVNMaterialTraceability(string ncrnum);
        VNMaterialTraceability GetVNMaterialTraceabilityByID(string id);
        Result SaveEditNonComformity(List<NCR_DET> nCR_DETs, string NCR_NUM, List<NCR_EVI> nCR_EVIs, VNMaterialTraceability vNMaterialTraceability, List<string> EVIID, string NCRType);
        bool CheckScrap(string partNum);
        Result SubmitScrap(string nCR_NUM, string UserIdChange, string Comment,string WHMRBid);
        Result NormalSubmitNCR(List<APPROVAL> aPPROVALs, List<TaskManagementCreateModel> taskManagementCreateModels, string nCR_NUM, string UserIDChange, string Comment);
        Result FloorSubmitNCR(List<APPROVAL> aPPROVALs, List<TaskManagementCreateModel> taskManagementCreateModels, string nCR_NUM, string UserIDChange, string Comment, string MRB_Loc);
        bool CheckFloorNCR(string nCR_NUM);      
        List<UserApproval> GetApproverOfNCRForConfirm(string nCR_NUM);
        Result SubmitConfirmNCR(string nCR_NUM, List<APPROVAL> aPPROVALs, List<string> roleIDs, string uidConfirm);
        Result SubmitDispositionNCR(NCRManagementViewModel data, string UserID, List<OrderDisposition> orderDispositions, List<OrderDisposition> orderDispositionsADDIN);
        Result AssignNCR(string id, string nCRNUM, string approverId, string reason, string UserIdLogin);
        VNMaterialTraceability GetUploadedVNMaterialTraceability(string ncrnum);
        Result AddinsAddIns(string nCRNUM, string item, double qTY, string aDDINS, string remark, string iNSP, OrderDisposition orderDisposition);
        Result RejectNCR(string id, string nCRNUM, string approverId, string reason, string UserIdLogin);
        bool IsMegerNCR(string part, string lot, string ccn);
        Result MergeNCR(string ncrnum);
        List<GetListWaiitingDisposition_Result> GetListNCRWaitingDisposition(string iduser);
        //TL
        Result RejectNCRWH(string nCRNUM, string reason, string IDuser);
        Result RejectNCREng(string id, string nCRNUM, string approverId, string reason, string UserIdLogin);
        Result AssignNCREng(string id, string nCRNUM, string approverId, string reason, string UserIdLogin,string roleid);
        string GetDescriptionbyPartNum(string partnum, string ccn);
        bool CheckSignature(string Iduser);
        //tuan lua add
        NCRManagementViewModel GetNCRHistory(string NCR_NUM, string CRno);
        // List<NCR_DISViewModel> GetNCRDISsHistory(string nCR_NUM, string CRno);
        List<UserApproval> GetApproverOfNCRForConfirmHistory(string nCR_NUM, string CRno);
        List<NCR_DETViewModel> GetInresultProcessStringHistory(string NCR_NUM, string CRno);
        List<NCRAgingViewmodel> GetListNCRSCRAP();
        List<NCRAgingViewmodel> GetListNCRSubmitbyUser(string id);
        List<sp_GetNCRAging_Result1> GetListNCRwaitingyourapproval();
    }

    public class NCRManagementService : INCRManagementService
    {
        private IIVILocalDB _db;
        LogWriter log;
        private StackFrame CallStack;
        public NCRManagementService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
            CallStack = new StackFrame(1, true);
            log = new LogWriter("NCRManagementService " + CallStack.GetFileLineNumber());
        }

        //public bool IsExistNCRNumber(string ncrnumber)
        //{
        //    try
        //    {
        //        var tmp = _db.NCR_HDR.Where(o => o.NCR_NUM == ncrnumber).FirstOrDefault();
        //        return tmp != null;
        //    }
        //    catch (Exception)
        //    {
        //        return true;
        //    }
        //}


        // get RTV status
        public string getRTVstatus(string NCR_NUM)
        {
            try
            {
                var stt = _db.RTVProcesses.Where(o => o.NCR_NUMBER == NCR_NUM).FirstOrDefault().RTVStatus;
                return stt;
            }
            catch (Exception)
            {
                return "";
            }
        }


        public bool updatestatus(string NCR_NUM)
        {
            try
            {
                var data = _db.RTVProcesses.Where(o => o.NCR_NUMBER == NCR_NUM).FirstOrDefault();
                if (data.RTVStatus == II_VI_Incorporated_SCM.Services.StatusRTV.Process)
                {
                    data.RTVStatus = II_VI_Incorporated_SCM.Services.StatusRTV.Close;
                }
                else if (data.RTVStatus == II_VI_Incorporated_SCM.Services.StatusRTV.New)
                {
                    data.RTVStatus = II_VI_Incorporated_SCM.Services.StatusRTV.Process;
                }

                _db.SaveChanges();
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
        // get RTV information
        public RTVProccessViewModel getRTV(string NCR_NUM)
        {
            var rtv = _db.RTVProcesses.Where(o => o.NCR_NUMBER == NCR_NUM).Select(x => new RTVProccessViewModel()
            {
                NCR_NUMBER = x.NCR_NUMBER,
                Remark = x.Remark,
                CreditNote = x.CreditNote,
                Qty = x.Qty,
                RTVStatus = x.RTVStatus,
                TypeRTV = x.TypeRTV,
                Shipped = x.Shipped,
                CreditFile = x.CreditFile,
            }).FirstOrDefault();
            return rtv;
        }


        public bool checkexistsRTVProcess(string NCR_NUM)
        {
            if (_db.RTVProcesses.Where(x => x.NCR_NUMBER == NCR_NUM).FirstOrDefault() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetFile(string NCR_Num)
        {
            var data = _db.RTVProcesses.Where(x => x.NCR_NUMBER == NCR_Num).Select(x => x.CreditFile).FirstOrDefault();
            return data;
        }
        public bool SaveRTVProcess(RTVProccessViewModel rtv)
        {
            try
            {
                if (rtv != null)
                {
                    RTVProcess data = new RTVProcess
                    {
                        NCR_NUMBER = rtv.NCR_NUMBER,
                        Remark = rtv.Remark,
                        Qty = rtv.Qty,
                        RTVStatus = rtv.RTVStatus,
                        //  File_Upload = rtv.File_Upload,
                        CreditFile = rtv.CreditFile,
                        Shipped = rtv.Shipped,
                        TypeRTV = rtv.TypeRTV,
                        CreditNote = rtv.CreditNote
                    };
                    _db.RTVProcesses.Add(data);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public List<AspNetUser> GetUser()
        {
            List<AspNetUser> list = new List<AspNetUser>();
            list = _db.AspNetUsers.ToList();
            return list;
        }

        public string GetFullUserNameById(string id)
        {
            var data = _db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            return data != null ? data.FullName : "";
        }

        public bool IsExistReceiver(string receiver)
        {
            try
            {
                var tmp = _db.RECEIVERs.Where(o => o.RECEIVER1.Trim() == receiver).FirstOrDefault();
                return tmp != null;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool IsExistVendor(string vendor)
        {
            try
            {
                var tmp = _db.VENDORs.Where(o => o.VENDOR1.Trim() == vendor).FirstOrDefault();
                return tmp != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<NCR_HDR> GetListNCRApproval(string id)
        {
            List<NCR_HDR> ListNCR = new List<NCR_HDR>();
            ListNCR = _db.Database.SqlQuery<NCR_HDR>($"SELECT NCR.* FROM NCR_HDR ncr, APPROVAL appr WHERE NCR.STATUS = 'd' AND NCR.NCR_NUM = appr.NCR_NUMBER AND appr.UserId = '{id}' AND appr.isActive = 1").ToList();
            List<NCR_HDR> res = new List<NCR_HDR>();
            var iDChairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals("MRB Chairman")).Id;
            foreach (var ncr in ListNCR)
            {
                var isChairman = _db.APPROVALs.FirstOrDefault(x => x.NCR_NUMBER.Trim() == ncr.NCR_NUM.Trim() & x.UserId == id && x.RoleId == iDChairman) != null;

                var approvers = GetApproverOfNCRForConfirm(ncr.NCR_NUM.Trim());
                var approver = approvers.FirstOrDefault(x => x.IdUser.Equals(id));
                if (!approver.IsAppr && !isChairman)
                {
                    res.Add(ncr);
                }
                else if (!approver.IsAppr && isChairman)
                {
                    var count = approvers.Count(x => x.IsAppr == true);
                    if (count + 1 == approvers.Count)
                    {
                        res.Add(ncr);
                    }
                }
            }

            return res;
        }

        // get list ncrage where status = summitted
        public List<sp_GetNCRAging_Result1> GetListNCRAge()
        {
            //string query = $"select  a.NCR_NUM, MI_PART_NO,  b.FullName,a.PO_NUM, a.INS_DATE,A.DATESUBMIT,a.DateApproval AS DateApproval, DATEDIFF(DD, a.DATESUBMIT, a.DateApproval) as AGE ,c.NAME from ncr_hdr a, AspNetUsers b, status c where a.status = 'e'and a.USERSUBMIT = b.Id and a.STATUS = c.ID UNION select  NCR_NUM, MI_PART_NO,  b.FullName,a.PO_NUM, a.INS_DATE,A.DATESUBMIT,a.DateApproval AS DateApproval, DATEDIFF(DD, a.DATESUBMIT, GETDATE()) as AGE ,c.NAME from ncr_hdr a, AspNetUsers b, status c where a.status IN('B','c','d')and a.USERSUBMIT = b.Id and a.STATUS = c.ID order by AGE desc, a.DATESUBMIT desc, a.DateApproval desc";

            List<sp_GetNCRAging_Result1> ListNCR = _db.sp_GetNCRAging().ToList();
            List<sp_GetNCRAging_Result1> res = new List<sp_GetNCRAging_Result1>();
            var ncrfirst = ListNCR.Where(x => x.status.Trim() != "Waiting for Approval");
            res.AddRange(ncrfirst);
            var ncrs = ListNCR.Where(x => x.status.Trim() == "Waiting for Approval").Select(x => x.NCR_NUM.Trim()).ToArray();
            var ncrsuni = new HashSet<string>(ncrs).ToList();
            // var ListNCR = _db.Database.SqlQuery <NCRAgingViewmodel> (query).ToList();
            for (int i = 0; i < ncrsuni.Count; i++)
            {
                var ncr = ncrsuni[i];
                var app = GetApproverOfNCRForConfirm(ncr.Trim()).Where(x => x.IsAppr == false).Select(x => x.FullName).ToArray();
                var ncrnoneapproval = ListNCR.Where(x => x.NCR_NUM.Trim() == ncr.Trim() && app.Contains(x.Approver)).ToList();
                res.AddRange(ncrnoneapproval);
            }

            return res;
        }
        //get lisst waitting yeour approval 01/04/2019
        public List<sp_GetNCRAging_Result1> GetListNCRwaitingyourapproval()
        {

            List<sp_GetNCRAging_Result1> ListNCR = _db.sp_GetNCRAging().ToList();
            List<sp_GetNCRAging_Result1> res = new List<sp_GetNCRAging_Result1>();
            var ncrs = ListNCR.Where(x => x.status.Trim() == "Waiting for Approval").Select(x => x.NCR_NUM.Trim()).ToArray();
            var ncrsuni = new HashSet<string>(ncrs).ToList();
            for (int i = 0; i < ncrsuni.Count; i++)
            {
                var ncr = ncrsuni[i];
                var app = GetApproverOfNCRForConfirmNotChairMain(ncr.Trim()).Where(x => x.IsAppr == false).Select(x => x.FullName).ToArray();
                var ncrnoneapproval = ListNCR.Where(x => x.NCR_NUM.Trim() == ncr.Trim() && app.Contains(x.Approver)).ToList();
                res.AddRange(ncrnoneapproval);
            }
            return res;
        }


        public List<NCRManagementViewModel> GetListNCR()
        {
            var listNCR = from nc in _db.NCR_HDR
                          join stt in _db.STATUS on nc.STATUS equals stt.ID
                          join asp in _db.AspNetUsers on nc.USERSUBMIT equals asp.Id
                          into joined
                          from j in joined.DefaultIfEmpty()
                          select new NCRManagementViewModel
                          {
                              NCR_NUM = nc.NCR_NUM,
                              PO_NUM = nc.PO_NUM,
                              LOT = nc.LOT,
                              REC_QTY = nc.REC_QTY,
                              CCN = nc.CCN,                             
                              INS_QTY = nc.INS_QTY,
                              RECEIVER = nc.RECEIVER,
                              INSPECTOR = nc.INSPECTOR,
                              TYPE_NCR = nc.TYPE_NCR,
                              STATUS = stt.NAME,
                              MI_PART_NO = nc.MI_PART_NO,
                              INS_DATE = nc.INS_DATE,
                              VENDOR = nc.VENDOR,
                              DateConform = nc.ConfirmDate,
                              SEC = nc.SEC,
                              Amount = nc.Amount,
                              DATESUBMIT = nc.STATUS.Trim() != StatusInDB.Created ? nc.DATESUBMIT : null,
                              USERSUBMIT = nc.STATUS.Trim() != StatusInDB.Created ? j.FullName : ""
                          };
            return listNCR.ToList();
        }
        public List<NCRManagementViewModel> GetListNCRByVendor(string vendor)
        {

            var listNCR = from nc in _db.NCR_HDR
                          join stt in _db.STATUS on nc.STATUS equals stt.ID
                          join asp in _db.AspNetUsers on nc.USERSUBMIT equals asp.Id
                          into joined
                          from j in joined.DefaultIfEmpty()
                          where nc.VENDOR.Trim() == vendor.Trim()
                          select new NCRManagementViewModel
                          {
                              NCR_NUM = nc.NCR_NUM,
                              PO_NUM = nc.PO_NUM,
                              REC_QTY = nc.REC_QTY,
                              INS_QTY = nc.INS_QTY,
                              RECEIVER = nc.RECEIVER,
                              INSPECTOR = nc.INSPECTOR,
                              TYPE_NCR = nc.TYPE_NCR,
                              STATUS = stt.NAME,
                              MI_PART_NO = nc.MI_PART_NO,
                              INS_DATE = nc.INS_DATE,
                              VENDOR = nc.VENDOR,
                              SEC = nc.SEC,
                              DATESUBMIT = nc.DATESUBMIT,
                              USERSUBMIT = j.FullName
                          };
            return listNCR.ToList();
        }

        // get list NCR_RTV
        public List<NCRManagementViewModel> GetListNCR_RTV()
        {
            List<string> ListNCRNumber = new List<string>();
            ListNCRNumber.AddRange(_db.NCR_DET.Where(n => n.RESPONSE == CONFIRMITY_RESPON.ID_VENDOR && (n.DISPOSITION == CONFIRMITY_DISPN.ID_VENEXP || n.DISPOSITION == CONFIRMITY_DISPN.ID_VENMI || n.DISPOSITION == CONFIRMITY_DISPN.ID_DESCRIBE)).Select(n => n.NCR_NUM).ToList());
            ListNCRNumber.AddRange(_db.NCR_DIS.Where(n => n.ADD_INS.Trim() == CONFIRMITY_DISPN.ID_VENEXP.Trim() || n.ADD_INS == CONFIRMITY_DISPN.ID_VENMI).Select(n => n.NCR_NUM).ToList());
            // Remove dumplicate in list C#
            var distinctItems = ListNCRNumber.GroupBy(x => x).Select(y => y.First());
            var result = (from ncr in _db.NCR_HDR.Where(n => ListNCRNumber.Contains(n.NCR_NUM) && n.STATUS == StatusInDB.DispositionApproved)
                          join stt in _db.STATUS on ncr.STATUS equals stt.ID
                          join asp in _db.AspNetUsers on ncr.USERSUBMIT equals asp.Id
                              into joined
                          from j in joined.DefaultIfEmpty()
                          where (ncr.STATUS == stt.ID)
                          select (new NCRManagementViewModel
                          {
                              NCR_NUM = ncr.NCR_NUM,
                              MI_PART_NO = ncr.MI_PART_NO,
                              PO_NUM = ncr.PO_NUM,
                              INS_DATE = ncr.INS_DATE,
                              STATUS = stt.NAME,
                              DATESUBMIT = ncr.DATESUBMIT,
                              USERSUBMIT = j.FullName
                          })).ToList();
            return result;
        }
        public NCRManagementViewModel GetCreateNCR(string NCR_NUM)
        {
            var _log = new LogWriter("INCRManagementService - GetCreateNCR" + NCR_NUM);
            NCRManagementViewModel nCRManagementViewModel = new NCRManagementViewModel();
            try
            {
                var NCRCreate = from nc in _db.NCR_HDR
                                    // join defect in _db.INS_RESULT_DEFECT on nc.RECEIVER equals defect.receiver
                                    //join cs in _db.CS on nc.RECEIVER equals cs.RECEIVER
                                    // join change in _db.CHANGED_SPL on nc.RECEIVER equals change.RECEIVER 
                                where (nc.NCR_NUM.Trim() == NCR_NUM.Trim())
                                select new NCRManagementViewModel
                                {
                                    NCR_NUM = nc.NCR_NUM,
                                    SEC = nc.SEC,
                                    PO_NUM = nc.PO_NUM,
                                    REC_QTY = nc.REC_QTY,
                                    INS_QTY = nc.INS_QTY,
                                    RECEIVER = nc.RECEIVER,
                                    INSPECTOR = nc.INSPECTOR,
                                    TYPE_NCR = nc.TYPE_NCR,
                                    MI_PART_NO = nc.MI_PART_NO,
                                    INS_DATE = nc.INS_DATE,
                                    VENDOR = nc.VENDOR,
                                    VEN_NAME = nc.VEN_NAME,
                                    VEN_ADD = nc.VEN_ADD,
                                    ZIP_CODE = nc.ZIP_CODE,
                                    STATE = nc.STATE,
                                    CITY = nc.CITY,
                                    REJ_QTY = nc.REJ_QTY,
                                    DRAW_REV = nc.DRAW_REV,
                                    ITEM_DESC = nc.ITEM_DESC,
                                    MODEL_NO = nc.MODEL_NO,
                                    LOT = nc.LOT,
                                    STATUS = nc.STATUS,
                                    NOT_REQUIRED = nc.NOT_REQUIRED,
                                    REQUIRED = nc.REQUIRED,
                                    NOTIFICATION_ONLY = nc.NOTIFICATION_ONLY,
                                    ISSUED_REQUEST_NO = nc.ISSUED_REQUEST_NO,
                                    ISSUED_REQUEST_DATE = nc.ISSUED_REQUEST_DATE,
                                    REMOVED_FROM = nc.REMOVED_FROM,
                                    BOOK_INV = nc.BOOK_INV,
                                    ISSUE_MEMO_NO = nc.ISSUE_MEMO_NO,
                                    ISSUE_MEMO_DATE = nc.ISSUE_MEMO_DATE,
                                    NOTES = nc.NOTES,
                                    FOLLOW_UP_NOTES = nc.FOLLOW_UP_NOTES,
                                    SHIPPING_METHOD = nc.SHIPPING_METHOD,
                                    RETURN_NUMBER = nc.RETURN_NUMBER,
                                    AQL = nc.AQL,
                                    SAMPLE_INSP = nc.SAMPLE_INSP,
                                    PERCENT_INSP = nc.PERCENT_INSP,
                                    FIRST_ARTICLE = nc.FIRST_ARTICLE,
                                    //AQL_VISUAL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                    //AQL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                    Notrequered = nc.NOT_REQUIRED.Value == true ? "checked" : "",
                                    requered = nc.REQUIRED.Value == true ? "checked" : "",
                                    notification = nc.NOTIFICATION_ONLY.Value == true ? "checked" : "",
                                    //  QTY = nc.
                                    defect = nc.DEFECTIVE.Value, CCN = nc.CCN,
                                    Comment = nc.Comment,
                                    MRB_LOC = nc.MRB_LOC
                                };
                nCRManagementViewModel = NCRCreate.FirstOrDefault();
                string name = "";
                if (nCRManagementViewModel != null)
                {
                    string id = nCRManagementViewModel.INSPECTOR;
                    var data = _db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                    name = data == null ? "" : data.FullName;
                    nCRManagementViewModel.INSPECTOR = name;
                }
            }
            catch (Exception ex)
            {
                _log.LogWrite(ex.ToString());
            }

            //Get Approver
            //var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(NCR_NUM) & x.isActive == true).ToList();
            //foreach (var approver in Approvers)
            //{
            //    result.ListUSerAppr.Add(new UserApproval
            //    {

            //    });
            //}
            return nCRManagementViewModel;
        }
        public NCRManagementViewModel GetNCRHistory(string NCR_NUM, string CRno)
        {
            var _log = new LogWriter("INCRManagementService - GetCreateNCR" + NCR_NUM);
            NCRManagementViewModel nCRManagementViewModel = new NCRManagementViewModel();
            try
            {
                var NCRCreate = from nc in _db.NCR_HDR_History
                                    // join defect in _db.INS_RESULT_DEFECT on nc.RECEIVER equals defect.receiver
                                    //join cs in _db.CS on nc.RECEIVER equals cs.RECEIVER
                                    // join change in _db.CHANGED_SPL on nc.RECEIVER equals change.RECEIVER 
                                where (nc.NCR_NUM.Trim() == NCR_NUM.Trim() && CRno.Trim() == CRno.Trim())
                                select new NCRManagementViewModel
                                {
                                    NCR_NUM = nc.NCR_NUM,
                                    SEC = nc.SEC,
                                    PO_NUM = nc.PO_NUM,
                                    REC_QTY = nc.REC_QTY,
                                    INS_QTY = nc.INS_QTY,
                                    RECEIVER = nc.RECEIVER,
                                    INSPECTOR = nc.INSPECTOR,
                                    TYPE_NCR = nc.TYPE_NCR,
                                    MI_PART_NO = nc.MI_PART_NO,
                                    INS_DATE = nc.INS_DATE,
                                    VENDOR = nc.VENDOR,
                                    VEN_NAME = nc.VEN_NAME,
                                    VEN_ADD = nc.VEN_ADD,
                                    ZIP_CODE = nc.ZIP_CODE,
                                    STATE = nc.STATE,
                                    CITY = nc.CITY,
                                    REJ_QTY = nc.REJ_QTY,
                                    DRAW_REV = nc.DRAW_REV,
                                    ITEM_DESC = nc.ITEM_DESC,
                                    MODEL_NO = nc.MODEL_NO,
                                    LOT = nc.LOT,
                                    STATUS = nc.STATUS,
                                    NOT_REQUIRED = nc.NOT_REQUIRED,
                                    REQUIRED = nc.REQUIRED,
                                    NOTIFICATION_ONLY = nc.NOTIFICATION_ONLY,
                                    ISSUED_REQUEST_NO = nc.ISSUED_REQUEST_NO,
                                    ISSUED_REQUEST_DATE = nc.ISSUED_REQUEST_DATE,
                                    REMOVED_FROM = nc.REMOVED_FROM,
                                    BOOK_INV = nc.BOOK_INV,
                                    ISSUE_MEMO_NO = nc.ISSUE_MEMO_NO,
                                    ISSUE_MEMO_DATE = nc.ISSUE_MEMO_DATE,
                                    NOTES = nc.NOTES,
                                    FOLLOW_UP_NOTES = nc.FOLLOW_UP_NOTES,
                                    SHIPPING_METHOD = nc.SHIPPING_METHOD,
                                    RETURN_NUMBER = nc.RETURN_NUMBER,
                                    AQL = nc.AQL,
                                    SAMPLE_INSP = nc.SAMPLE_INSP,
                                    PERCENT_INSP = nc.PERCENT_INSP,
                                    FIRST_ARTICLE = nc.FIRST_ARTICLE,
                                    //AQL_VISUAL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                    //AQL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                    Notrequered = nc.NOT_REQUIRED.Value == true ? "checked" : "",
                                    requered = nc.REQUIRED.Value == true ? "checked" : "",
                                    notification = nc.NOTIFICATION_ONLY.Value == true ? "checked" : "",
                                    //  QTY = nc.
                                    defect = nc.DEFECTIVE.Value,
                                    CCN = nc.CCN,
                                    // lay CRO chu k phai comment nha =]]
                                    Comment = nc.CRNO
                                };
                nCRManagementViewModel = NCRCreate.FirstOrDefault();
                string name = "";
                if (nCRManagementViewModel != null)
                {
                    string id = nCRManagementViewModel.INSPECTOR;
                    var data = _db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                    name = data == null ? "" : data.FullName;
                    nCRManagementViewModel.INSPECTOR = name;
                }
            }
            catch (Exception ex)
            {
                _log.LogWrite(ex.ToString());
            }

            //Get Approver
            //var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(NCR_NUM) & x.isActive == true).ToList();
            //foreach (var approver in Approvers)
            //{
            //    result.ListUSerAppr.Add(new UserApproval
            //    {

            //    });
            //}
            return nCRManagementViewModel;
        }
        public List<NCR_DETViewModel> GetInresultProcess(string NCR_NUM, string CCN)
        {

            List<NCR_DETViewModel> listResult = new List<NCR_DETViewModel>();
            var listrDets = _db.NCR_DET.Where(m => m.NCR_NUM.Trim() == NCR_NUM.Trim()).OrderBy(m => m.ITEM).ToList();
            string defect = "";
            string conform = "";
            if (listrDets != null)
            {
                foreach (var item in listrDets)
                {
                    List<string> listNonConform = new List<string>();
                    List<string> listdefect = new List<string>();

                    if (item.DEFECT != null && item.DEFECT != "")
                    {
                        listdefect = item.DEFECT.Split(';').ToList();
                        foreach (var tmp in listdefect)
                        {
                            var data = _db.NCs.Where(x => x.NC_CODE == tmp && x.CCN == CCN).FirstOrDefault();
                            if (data != null)
                            {
                                defect = defect + data.NC_DESC + "; ";
                            }
                        }
                    }
                    if (item.NC_DESC != null && item.NC_DESC != "")
                    {
                        listNonConform = item.NC_DESC.Split(';').ToList();
                        foreach (var comfom in listNonConform)
                        {
                            var dete = _db.NC_GROUP.Where(x => x.NC_GRP_CODE == comfom && x.CCN == CCN).FirstOrDefault();
                            if (dete != null)
                            {
                                conform = comfom + dete.NC_GRP_CODE + "; ";
                            }
                        }
                    }
                    listResult.Add(new NCR_DETViewModel()
                    {
                        ITEM = item.ITEM,
                        DEFECT = listdefect,
                        DISPOSITION = item.DISPOSITION,
                        DISPOSITIONNAME = GetDispoNameByID(item.DISPOSITION),
                        NCR_NUM = item.NCR_NUM,
                        QTY = item.QTY,
                        //defect = defect + item.REMARK,
                        conform = conform,
                        //  NC_DESC = item.NC_DESC,
                        NC_DESC = listNonConform,
                        // PartialID = item.PartialID,
                        //  Picture = item.Picture,
                        //  receiver = item.receiver,
                        //  rec_line = item.rec_line,
                        REMARK = item.REMARK,
                        RESPONSE = item.RESPONSE,
                        RESPONSENAME = GetResponNameByID(item.RESPONSE),
                        DATEAPPROVAL = item.DATEAPPROVAL

                    });
                }
            }
            return listResult;
        }

        //update description string
        public void updatedescription(NCR_DETViewModel ncrview)
        {
            string desc = "";
            string defect = "";

            // lấy chuỗi mô tả dựa vào chuỗi mã desc (List<string> NC_DESC)

            foreach (var itemnc in ncrview.NC_DESC)
            {
                var dete = _db.NCs.Where(o => o.NC_CODE.Trim() == itemnc.Trim()).FirstOrDefault();
                if (dete != null)
                {
                    desc += dete.NC_DESC + "; ";
                }
                else
                {
                    desc += "";
                }
            }

            foreach (var item in ncrview.DEFECT)
            {
                var data = _db.NC_GROUP.Where(m => m.NC_GRP_CODE.Trim() == item.Trim()).FirstOrDefault();
                if (data != null)
                {
                    defect += data.NC_GRP_DESC + "; ";
                }
                else
                {
                    defect += "";
                }
            }


            ncrview.NC_DESC_STRING = desc;
            ncrview.DEFECT_STRING = defect;
        }

        // hàm cắt string id sang list id
        public List<string> cutString(string str)
        {
            List<string> list = new List<string>();
            //if (str.Contains(";"))
            //      {
            var temp = str.Split(';');
            list = temp.ToList();
            return list;
            //   }


        }
        public List<string> cutStringP(string str)
        {
            List<string> list = new List<string>();
            var temp = str.Split(',');
            list = temp.ToList();
            return list;
        }
        public void updatedescriptioniqc(INS_RESULT_DEFECTViewModel ncrview)
        {
            string desc = "";
            string defect = "";

            // lấy chuỗi mô tả dựa vào chuỗi mã desc (List<string> NC_DESC)

            foreach (var itemnc in ncrview.Non_Conformances)
            {
                var Nc = _db.NCs.FirstOrDefault(o => o.NC_CODE.Trim() == itemnc.Trim());
                if (Nc != null) desc += Nc.NC_DESC + "; ";
            }

            foreach (var item in ncrview.Defect)
            {
                var NC_Group = _db.NC_GROUP.FirstOrDefault(m => m.NC_GRP_CODE.Trim() == item.Trim());
                if (NC_Group != null) defect += NC_Group.NC_GRP_DESC + "; ";
                //    if()
            }


            ncrview.NC_DESC_STRING = desc;
            ncrview.DEFECT_STRING = defect;
        }

        public List<INS_RESULT_DEFECTViewModel> GetListInresultIqc(string receiver)
        {
            List<INS_RESULT_DEFECTViewModel> listResult = new List<INS_RESULT_DEFECTViewModel>();
            var listrDets = _db.INS_RESULT_DEFECT.Where(m => m.receiver.Trim() == receiver.Trim()).ToList();

            if (listrDets != null)
            {
                foreach (var item in listrDets)
                {
                    List<string> listdefect = new List<string>();
                    List<string> listnc_dest = new List<string>();
                    if (item.Defect != null && item.Defect != "")
                    {
                        listdefect = cutString(item.Defect);
                    }
                    if (item.Non_Conformances != null && item.Non_Conformances != "")
                    {
                        listnc_dest = cutString(item.Non_Conformances);
                    }
                    listResult.Add(new INS_RESULT_DEFECTViewModel()
                    {
                        NCR_Num = item.NCR_Num,
                        //ITEM = item.ITEM,
                        NC_Qty = item.NC_Qty,
                        Non_Conformances = listnc_dest,
                        NC_DESC_STRING = "",
                        Defect = listdefect,
                        DEFECT_STRING = "",
                        Response = item.Response,
                        Disposition = item.Disposition,
                        Remark = item.Remark,

                    });
                }
            }
            // update string mô tả
            foreach (var item in listResult)
            {
                updatedescriptioniqc(item);
            }
            return listResult;
        }


        public List<NCR_DETViewModel> GetInresultProcessString(string NCR_NUM)
        {
            List<NCR_DETViewModel> listResult = new List<NCR_DETViewModel>();
            var listrDets = _db.NCR_DET.Where(m => m.NCR_NUM.Trim() == NCR_NUM.Trim()).OrderBy(m => m.ITEM).ToList();

            if (listrDets != null)
            {
                foreach (var item in listrDets)
                {
                    List<string> listdefect = new List<string>();
                    List<string> listnc_dest = new List<string>();
                    if (!string.IsNullOrEmpty(item.DEFECT))
                    {
                        listdefect = cutString(item.DEFECT);
                    }
                    if (!string.IsNullOrEmpty(item.NC_DESC))
                    {
                        listnc_dest = cutString(item.NC_DESC);
                    }
                    listResult.Add(new NCR_DETViewModel()
                    {
                        NCR_NUM = item.NCR_NUM.Trim(),
                        ITEM = item.ITEM.Trim(),
                        QTY = item.QTY,
                        NC_DESC = listnc_dest,
                        NC_DESC_STRING = "",
                        DEFECT = listdefect,
                        DEFECT_STRING = "",
                        RESPONSE = item.RESPONSE,
                        RESPONSENAME = GetResponNameByID(item.RESPONSE),
                        DISPOSITION = !string.IsNullOrEmpty(item.DISPOSITION) ? item.DISPOSITION.Trim() : "",
                        DISPOSITIONNAME = !string.IsNullOrEmpty(item.DISPOSITION) ? GetDispoNameByID(item.DISPOSITION) : "",
                        REMARK = item.REMARK,
                        DATEAPPROVAL = item.DATEAPPROVAL,
                        SEC = item.SEC
                    });
                }
            }
            // update string mô tả
            foreach (var item in listResult)
            {
                updatedescription(item);
            }
            return listResult;
        }

        public List<NCR_DETViewModel> GetInresultProcessStringHistory(string NCR_NUM, string CRno)
        {
            List<NCR_DETViewModel> listResult = new List<NCR_DETViewModel>();
            var listrDets = _db.NCR_DET_History.Where(m => m.NCR_NUM.Trim() == NCR_NUM.Trim() && m.CRNO.Trim() == CRno.Trim()).OrderBy(m => m.ITEM).ToList();

            if (listrDets != null)
            {
                foreach (var item in listrDets)
                {
                    List<string> listdefect = new List<string>();
                    List<string> listnc_dest = new List<string>();
                    if (!string.IsNullOrEmpty(item.DEFECT))
                    {
                        listdefect = cutString(item.DEFECT);
                    }
                    if (!string.IsNullOrEmpty(item.NC_DESC))
                    {
                        listnc_dest = cutString(item.NC_DESC);
                    }
                    listResult.Add(new NCR_DETViewModel()
                    {
                        NCR_NUM = item.NCR_NUM.Trim(),
                        ITEM = item.ITEM.Trim(),
                        QTY = item.QTY,
                        NC_DESC = listnc_dest,
                        NC_DESC_STRING = "",
                        DEFECT = listdefect,
                        DEFECT_STRING = "",
                        RESPONSE = item.RESPONSE,
                        RESPONSENAME = GetResponNameByID(item.RESPONSE),
                        DISPOSITION = !string.IsNullOrEmpty(item.DISPOSITION) ? item.DISPOSITION.Trim() : "",
                        DISPOSITIONNAME = !string.IsNullOrEmpty(item.DISPOSITION) ? GetDispoNameByID(item.DISPOSITION) : "",
                        REMARK = item.REMARK,
                        DATEAPPROVAL = item.DATEAPPROVAL,
                        SEC = item.SEC
                    });
                }
            }
            // update string mô tả
            foreach (var item in listResult)
            {
                updatedescription(item);
            }
            return listResult;
        }





        public void UpdateNCRForIQC(NCRManagementViewModel model)
        {
            var NCRHDR = _db.NCR_HDR.SingleOrDefault(c => c.NCR_NUM == model.NCR_NUM);
            if (NCRHDR != null)
            {
                NCRHDR.PO_NUM = model.PO_NUM;
                //  NCRHDR.RECEIVER = model.RECEIVER;
                NCRHDR.LOT = model.LOT;
                NCRHDR.MI_PART_NO = model.MI_PART_NO;
            }
            _db.SaveChanges();

        }

        public void UpdateNCRForProcess(NCRManagementViewModel model)
        {
            var NCRHDR = _db.NCR_HDR.SingleOrDefault(c => c.NCR_NUM == model.NCR_NUM);
            if (NCRHDR != null)
            {
                NCRHDR.PO_NUM = model.PO_NUM;
                NCRHDR.LOT = model.LOT;
                NCRHDR.MI_PART_NO = model.MI_PART_NO;

            }

            _db.SaveChanges();
        }

        public bool AddUserApproval(APPROVAL model)
        {
            try
            {
                _db.APPROVALs.Add(model);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var _log = new LogWriter("AddUserApproval");
                _log.LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    var e = (DbEntityValidationException)ex;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
                return false;
            }
        }
        //11/04/2019--TL check history khi void nCR
        public bool CheckhistoryNCR(string NCRNUM)
        {
            var result = _db.NCR_HDR_History.Where(x => x.NCR_NUM.Trim() == NCRNUM.Trim()).ToList();
            if (result.Count > 0 && result.FirstOrDefault().UserConfirm != null && result.FirstOrDefault().ConfirmDate != null)
            {
                return true;
            }
            return false;
        }

        public bool UpdateStatusNCR(string ncrnumber, string status, string userid)
        {
            try
            {
                var ncr = _db.NCR_HDR.Where(o => o.NCR_NUM == ncrnumber).FirstOrDefault();
                ncr.STATUS = status;
                if (status == StatusInDB.Submitted && userid != "")
                {
                    ncr.USERSUBMIT = userid;
                    ncr.DATESUBMIT = DateTime.Now;
                }
                if (status == StatusInDB.WaitingForDispositionApproval && userid != "")
                {
                    ncr.USERDISPO = userid;
                    ncr.DATEDISPO = DateTime.Now;
                }
                if (status == StatusInDB.Void && userid != "")
                {
                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change status NCR",
                        CreateDate = DateTime.Now,
                        IsActive = false,
                        NCRNUM = ncrnumber,
                        Status = StatusInDB.Void,
                        UserId = userid
                    });

                    //Update NCRNum in result_defect

                    var cord = _db.INS_RESULT_DEFECT.Where(x => x.NCR_Num.Equals(ncrnumber.Trim())).ToList();
                    foreach (var item in cord)
                    {
                        item.NCR_Num = null;
                        _db.Entry(item).State = EntityState.Modified;
                    }

                }
                _db.Entry(ncr).State = EntityState.Modified;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        // update age
        public void updateAgeNCRinListNCR(List<NCRManagementViewModel> listncr)
        {
            DateTime? approvalday = null;
            foreach (var item in listncr)
            {
                DateTime? submitday = _db.NCR_HDR.Where(m => m.NCR_NUM == item.NCR_NUM && m.STATUS != StatusInDB.Created).FirstOrDefault().DATESUBMIT;
                var NCR_HDR = _db.NCR_HDR.Where(m => m.NCR_NUM.Trim() == item.NCR_NUM.Trim() && item.DateApproval == m.DateApproval).FirstOrDefault();
                if (NCR_HDR != null)
                {
                    approvalday = NCR_HDR.DateApproval;

                }
                if (submitday != null && approvalday != null)
                {
                    TimeSpan ts = new TimeSpan();
                    ts = (TimeSpan)(approvalday - submitday);
                    int age = (int)ts.TotalDays;
                    item.AGE = age;
                }
                else if (submitday != null && approvalday == null)
                {
                    TimeSpan ts = new TimeSpan();
                    ts = (TimeSpan)(DateTime.Now - submitday);
                    int age = (int)ts.TotalDays;
                    item.AGE = age;
                }
                else
                {
                    item.AGE = 0;
                }
                //_db.Entry(listncr).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public List<NcrSearchViewModelProcess> GetDataInputIQCInProcessByPONumAndLOT(
            string po, string lot, string pn, string CCN)
        {
            if (lot == "null")
            {
                lot = string.Empty;
            }
            var tmp = (from rec in _db.RECEIVERs
                       join ven in _db.VENDORs on rec.VENDOR.Trim() equals ven.VENDOR1.Trim()
                       join p in _db.PO_REV on rec.PO_NUM equals p.PONo into ps
                       from p in ps.DefaultIfEmpty()
                       where (rec.PO_NUM.Trim() == po.Trim() && rec.LOT.Trim() == lot.Trim() && rec.ITEM.Trim() == pn.Trim() && rec.CCN.Trim() == CCN.Trim() && ven.CCN == CCN  /*&& rec.PO_LINE.Trim() == p.POLine.Trim()*/)
                       select (new NcrSearchViewModelProcess()
                       {
                           RECEIVER = rec.RECEIVER1.Trim(),
                           ITEM = rec.ITEM.Trim(),
                           VEN_NAME = ven.VEN_NAM.Trim(),
                           ZIP = ven.ZIP.Trim(),
                           STATE = ven.STATE.Trim(),
                           CTRY = ven.CTRY.Trim(),
                           ADDRESS = ven.ADDRESS.Trim(),
                           PO_NUM = rec.PO_NUM.Trim(),
                           LOT = rec.LOT.Trim(),
                           ITEM_DESC = rec.ITEM_DESC.Trim(),
                           VENDOR = rec.VENDOR.Trim(),
                           POSTING_DATE = rec.POSTING_DATE,
                           DRAW_REV = p == null ? "" : p.Rev.Trim()
                           // Inspector = partial.Inspector
                       }));
            tmp.ToList();
            return tmp.Distinct().OrderByDescending(x => x.POSTING_DATE).ToList();
        }

        public ApprovalDetViewmodel GetListUserApprovalByNcrNum(string ncrnum)
        {
            try
            {
                if (ncrnum != null)
                {
                    var obj = from ap in _db.APPROVALs
                              join det in _db.NCR_DET on ap.NCR_NUMBER.Trim() equals det.NCR_NUM.Trim()
                              where (ap.NCR_NUMBER.Trim() == ncrnum.Trim())
                              select new ApprovalDetViewmodel()
                              {
                                  NCR_NUMBER = ap.NCR_NUMBER,
                                  QUALITY = ap.QUALITY,
                                  ENGIEERING = ap.ENGIEERING,
                                  MFG = ap.MFG,
                                  PURCHASING = ap.PURCHASING,
                                  ENGIEERING_DATE = det.ENGIEERING,
                                  PURCHASING_DATE = det.PURCHASING,
                                  QUALITY_DATE = det.QUALITY,
                                  MFG_DATE = det.MFG,

                              };
                    return obj.FirstOrDefault();
                }
                return new ApprovalDetViewmodel();

            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<string> GetListLOTByPN(string pn, string CCN)
        {
            try
            {

                var lot = _db.RECEIVERs.Where(n => n.ITEM == pn && n.CCN == CCN).Select(n => n.LOT.Trim()).Distinct();
                return lot.ToList();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        public string GetStatusNRCByNCRNUM(string NCR_NUM)
        {
            return _db.NCR_HDR.Where(m => m.NCR_NUM == NCR_NUM).FirstOrDefault().STATUS;
        }

        public bool UpdateNCRDET(string ncrnum, List<NCR_DET> list)
        {
            var det = _db.NCR_DET.Where(x => x.NCR_NUM.Trim() == ncrnum.Trim()).ToList();
            foreach (var item in det)
            {
                foreach (var tmp in list)
                {
                    item.REMARK = tmp.REMARK;
                }
            }
            _db.SaveChanges();
            return true;
        }

        public bool SaveNewDefect(List<NCR_DET> list)
        {

            try
            {
                foreach (var item in list)
                {
                    _db.NCR_DET.Add(item);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAllDefectByNcrNum(string NCR_NUM)
        {
            try
            {
                List<NCR_DET> lstDet = _db.NCR_DET.Where(m => m.NCR_NUM == NCR_NUM).ToList();
                foreach (var item in lstDet)
                {
                    _db.Entry(item).State = EntityState.Deleted;
                }
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetOPEOwner(string currentId, string ncrNum)
        {
            var ncr = _db.NCR_HDR.Where(x => x.NCR_NUM == ncrNum).FirstOrDefault();
            if (ncr != null)
            {
                var user = _db.AspNetUsers.Where(x => x.Id == ncr.INSPECTOR).FirstOrDefault();
                if (user != null)
                {
                    if (currentId == user.OPE || currentId == user.Id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string GetNameStatusById(string id)
        {
            try
            {
                return _db.STATUS.Where(o => o.ID == id).FirstOrDefault().NAME;
            }
            catch (Exception)
            {
                return "";
            }

        }

        public bool SaveNCRHDRByDispositionModel(DispositionModel model)
        {
            try
            {
                NCR_HDR save = _db.NCR_HDR.Where(o => o.NCR_NUM == model.NCR_NUM).FirstOrDefault();
                save.NOT_REQUIRED = model.NOT_REQUIRED;
                save.REQUIRED = model.REQUIRED;
                save.NOTIFICATION_ONLY = model.NOTIFICATION_ONLY;
                save.ISSUED_REQUEST_NO = model.ISSUED_REQUEST_NO;
                save.ISSUED_REQUEST_DATE = null;
                save.REMOVED_FROM = model.REMOVED_FROM;
                save.BOOK_INV = model.BOOK_INV;
                if (model.REQUIRED == true)
                {
                    save.ISSUE_MEMO_DATE = DateTime.Now;
                    save.ISSUED_REQUEST_DATE = DateTime.Now;
                }
                save.ISSUE_MEMO_NO = model.ISSUE_MEMO_NO;
                save.ISSUE_MEMO_DATE = null;
                save.NOTES = model.NOTES;
                save.SHIPPING_METHOD = model.SHIPPING_METHOD;
                save.RETURN_NUMBER = model.RETURN_NUMBER;
                _db.Entry(save).State = EntityState.Modified;
                foreach (var tmp in model.lstResDis)
                {
                    NCR_DET det = _db.NCR_DET.Where(o => o.NCR_NUM.Trim() == model.NCR_NUM.Trim() && o.ITEM.Trim() == tmp.item.Trim())
                        .FirstOrDefault();
                    if (det != null)
                    {
                        det.RESPONSE = tmp.respon;
                        det.DISPOSITION = tmp.display;
                        _db.Entry(det).State = EntityState.Modified;
                    }
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return false;
            }
        }

        public bool AddListNCRDIS(List<NCR_DIS> lst)
        {
            try
            {
                for (int i = 0; i < lst.Count - 1; i++)
                {
                    //re move thag cuoi cung 
                    _db.NCR_DIS.Add(lst[i]);
                    _db.SaveChanges();

                }

                return true;
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return false;
            }
        }

        public bool AddListNCRDIS1(List<NCR_DIS> lst)
        {
            //var listdis = _db.NCR_DIS.ToList();
            try
            {
                //    for (int j = 0; j < listdis.Count; j++)
                //    {
                //        for (int i = 0; i < lst.Count; i++)
                //        {
                //            if (lst[i].ADD_INS.Trim() != listdis[j].ADD_INS.Trim() && lst[i].NCR_NUM == listdis[j].NCR_NUM && lst[i].ITEM.Trim() == listdis[j].ITEM.Trim())
                //            {
                //                _db.NCR_DIS.Add(lst[i]);
                //                _db.SaveChanges();
                //            }
                //        }
                //    }

                // Sil Edit. Date: 07/05/2018
                if (lst != null)
                {
                    foreach (var item in lst)
                    {
                        if (_db.NCR_DIS.FirstOrDefault(x => x.NCR_NUM.Equals(item.NCR_NUM) & x.ADD_INS.Equals(item.ADD_INS) & x.ITEM.Equals(item.ITEM)) == null)
                        {
                            item.INS_DATE = DateTime.Now;
                            _db.NCR_DIS.Add(item);
                            _db.SaveChanges();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                new LogWriter("AddListNCRDIS1").LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    var e = ex as DbEntityValidationException;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        new LogWriter("AddListNCRDIS1").LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            new LogWriter("AddListNCRDIS1").LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
                return false;
            }

        }
        public bool AddListSCRAP(List<SCRAP_REASON> lst)
        {
            try
            {
                _db.SCRAP_REASON.AddRange(lst);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return false;
            }
        }

        public List<NcrSearchViewModelProcess> GetDataInputIQCInProcessByLOTAndPartNum(
            string lot, string partnum, string CCN)
        {
            var tmp = from rec in _db.RECEIVERs
                      join ven in _db.VENDORs on rec.VENDOR.Trim() equals ven.VENDOR1.Trim()
                      join p in _db.PO_REV on rec.PO_NUM equals p.PONo into ps
                      from p in ps.DefaultIfEmpty()
                      where (rec.ITEM.Trim() == partnum.Trim() && rec.LOT.Trim() == lot.Trim() && rec.ITEM.Trim() == p.Item.Trim() && rec.CCN == CCN && rec.PO_LINE == p.POLine)
                      select (new NcrSearchViewModelProcess()
                      {
                          RECEIVER = rec.RECEIVER1.Trim(),
                          ITEM = rec.ITEM.Trim(),
                          VEN_NAME = ven.VEN_NAM.Trim(),
                          ZIP = ven.ZIP.Trim(),
                          STATE = ven.STATE.Trim(),
                          CTRY = ven.CTRY.Trim(),
                          ADDRESS = ven.ADDRESS.Trim(),
                          PO_NUM = rec.PO_NUM.Trim(),
                          LOT = rec.LOT.Trim(),
                          ITEM_DESC = rec.ITEM_DESC.Trim(),
                          VENDOR = rec.VENDOR.Trim(),
                          POSTING_DATE = rec.POSTING_DATE,
                          DRAW_REV = p.Rev.Trim()

                          // Inspector = partial.Inspector
                      });
            return tmp.ToList();

        }

        public List<string> GetListLOTByWOAndPartNum(string wo, string partnum)
        {
            try
            {
                //var tmp = _db.V_WO_Issue
                //    .Where(o => o.WOCode.Trim() == wo.Trim() && o.MaterialID.Trim() == partnum.Trim())
                //    .Select(o => o.LotNumber).ToList();
                var  tmp = new List<string>();
                return tmp;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetListPOByLOT(string lot, string pn, string CCN)
        {
            try
            {
                var tmp = _db.RECEIVERs.Where(o => o.LOT.Trim() == lot.Trim() && o.ITEM.Trim() == pn.Trim()).Select(o => o.PO_NUM).ToList();
                return tmp;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetListSerialByWOAndPartNum(string serial, string partnum)
        {
            try
            {
                //var tmp = _db.V_Material_HadMake_FLupSerialNumber
                //    .Where(o => o.SerialNumber.Trim() == serial.Trim() && o.MaterialID.Trim() == partnum.Trim())
                //    .Select(o => o.LotNumber.Trim()).ToList();
                var tmp = new  List<string>();
                return tmp;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public void updatedateapprovalNCR(string ncrnum, bool final)
        {
            if (final)
            {
                var data = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim() == ncrnum.Trim()).ToList();
                foreach (var item in data)
                {
                    item.DATEAPPROVAL = DateTime.Now;
                    _db.SaveChanges();
                }
                var dete = _db.NCR_DET.Where(x => x.NCR_NUM.Trim() == ncrnum.Trim()).ToList();
                foreach (var item in dete)
                {
                    item.DATEAPPROVAL = DateTime.Now;
                    _db.SaveChanges();
                }
            }

        }
        public Result UpdateUserAprrovalDate(string Id, string ncrnum, string IdUser, string password = "")
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(ncrnum));
                    var REJ = NCR.REJ_QTY;
                    var DEF = NCR.DEFECTIVE;
                    bool IsAQL = NCR.SAMPLE_INSP == true;

                    var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(ncrnum)).ToList();
                    var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(ncrnum)).ToList();
                    var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(ncrnum) & x.isActive == true).ToList();
                    var IDTp = int.Parse(Id);
                    var Approver = Approvers.FirstOrDefault(x => x.ID == IDTp);
                    var QTY = DISs.Sum(x => x.QTY);
                    var RoleIDChairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.CHAIRMAN).Id;
                    var UserIdChairman = Approvers.FirstOrDefault(x => x.RoleId == RoleIDChairman).UserId;
                    int approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true).Select(x => x.UserId).Distinct().Count();

                    if (approverCount < Approvers.Count() - 1 && IdUser == UserIdChairman)
                    {
                        return new Result
                        {
                            success = false,
                            message = "Approval is unsuccessful, Please approve after MRB team!",
                            obj = ""
                        };
                    }

                    if (IsAQL)
                    {
                        if (NCR.REJ_QTY != QTY)
                        {
                            return new Result
                            {
                                success = false,
                                message = "Qty of ADDITIONALS INSTRUCTIONS must equal qty of Non-Comformity !",
                                obj = ""
                            };
                        }

                        foreach (var dis in DISs)
                        {
                            var UserDispositionApproval = new UserDispositionApproval
                            {
                                Id = Guid.NewGuid().ToString(),
                                Comment = "",
                                IsActive = true,
                                NCR_DIS_ID = dis.Id,
                                UserId = IdUser,
                                NCRNUM = NCR.NCR_NUM,
                                DateApprove = DateTime.Now,
                                DET_Item = dis.ITEM
                            };
                            _db.UserDispositionApprovals.Add(UserDispositionApproval);
                        }
                        _db.SaveChanges();

                        //int approverCount = _db.UserDispositionApprovals.Count(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true);
                        if (approverCount >= Approvers.Count() - 1 & IdUser == UserIdChairman)
                        {
                            foreach (var dis in DISs)
                            {
                                dis.DATEAPPROVAL = DateTime.Now;
                            }

                            foreach (var det in DETs)
                            {
                                det.DATEAPPROVAL = DateTime.Now;
                            }
                        }
                        _db.SaveChanges();
                        //int approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true).Select(x => x.UserId).Distinct().Count();
                        var DETsNOTNULL = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(ncrnum) & x.DATEAPPROVAL == null).ToList();

                        if (approverCount < Approvers.Count() - 1 & DETsNOTNULL.Count() >= 0 & IdUser == UserIdChairman)
                        {
                            tranj.Rollback();
                            return new Result
                            {
                                success = false,
                                message = "Approval is unsuccessful, Please approve after MRB team!",
                                obj = ""
                            };
                        }

                        if (approverCount >= Approvers.Count() - 1 & DETsNOTNULL.Count() <= 0 & IdUser == UserIdChairman)
                        {
                            NCR.STATUS = StatusInDB.DispositionApproved;
                            //tuan lua add 06-03-20189
                            NCR.DateApproval = DateTime.Now;
                        }

                        _db.SaveChanges();
                        tranj.Commit();
                        ///TODO: Send mail chairman when approver <= approvers
                        return new Result
                        {
                            success = true,
                            message = "Approval is successful",
                            obj = (approverCount == Approvers.Count() - 2 && IdUser != UserIdChairman) ? "EmailChairman" : (approverCount == Approvers.Count() - 1 && IdUser == UserIdChairman) ? "ApprovePartial" : ""
                        };
                    }
                    else
                    {
                        foreach (var det in DETs)
                        {
                            var DisOfDet = DISs.Where(x => x.ITEM.Trim().Equals(det.ITEM.Trim()) & x.DATEAPPROVAL == null).ToList();
                            var DisQTY = DisOfDet.Sum(x => x.QTY);
                            if (DisQTY == 0 /*&& det.DISPOSITION != CONFIRMITY_DISPN.ID_TBD && det.DATEAPPROVAL == null*/)
                            {
                                _db.UserDispositionApprovals.Add(new UserDispositionApproval
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Comment = "",
                                    IsActive = true,
                                    UserId = IdUser,
                                    NCRNUM = NCR.NCR_NUM,
                                    DET_Item = det.ITEM,
                                    DateApprove = DateTime.Now
                                });
                                approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.DET_Item == det.ITEM).Select(x => x.UserId).Distinct().Count();
                                _db.SaveChanges();

                                if (IdUser == UserIdChairman)
                                {
                                    int approverCountForDET = _db.UserDispositionApprovals.Count(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.DET_Item == det.ITEM);
                                    if (approverCountForDET == Approvers.Count())
                                    {
                                        foreach (var dis in DisOfDet)
                                        {
                                            dis.DATEAPPROVAL = DateTime.Now;
                                        }

                                        det.DATEAPPROVAL = DateTime.Now;
                                    }
                                }

                                _db.SaveChanges();
                            }
                            else if (DisQTY == det.QTY /*&& det.DISPOSITION != CONFIRMITY_DISPN.ID_TBD && det.DATEAPPROVAL == null*/)
                            {
                                foreach (var dis in DisOfDet)
                                {
                                    _db.UserDispositionApprovals.Add(new UserDispositionApproval
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Comment = "",
                                        IsActive = true,
                                        UserId = IdUser,
                                        NCRNUM = NCR.NCR_NUM,
                                        DET_Item = det.ITEM,
                                        NCR_DIS_ID = dis.Id,
                                        DateApprove = DateTime.Now
                                    });
                                    approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.NCR_DIS_ID == dis.Id).Select(x => x.UserId).Distinct().Count();

                                    _db.SaveChanges();
                                }

                                if (IdUser == UserIdChairman)
                                {
                                    int approverCountForDET = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.DET_Item == det.ITEM).Select(x => x.UserId).Distinct().Count();
                                    if (approverCountForDET == Approvers.Count())
                                    {
                                        foreach (var dis in DisOfDet)
                                        {
                                            dis.DATEAPPROVAL = DateTime.Now;
                                        }
                                        det.DATEAPPROVAL = DateTime.Now;
                                    }
                                }

                                _db.SaveChanges();
                            }
                            else if (DisQTY < det.QTY /*&& det.DISPOSITION != CONFIRMITY_DISPN.ID_TBD && det.DATEAPPROVAL == null*/)
                            {
                                var DISsNull = DisOfDet.Where(x => x.DATEAPPROVAL == null).ToList();
                                foreach (var dis in DISsNull)
                                {
                                    _db.UserDispositionApprovals.Add(new UserDispositionApproval
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Comment = "",
                                        IsActive = true,
                                        UserId = IdUser,
                                        NCRNUM = NCR.NCR_NUM,
                                        NCR_DIS_ID = dis.Id,
                                        DET_Item = dis.ITEM.Trim(),
                                        DateApprove = DateTime.Now
                                    });

                                    if (IdUser == UserIdChairman)
                                    {
                                        dis.DATEAPPROVAL = DateTime.Now;
                                    }
                                    approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.NCR_DIS_ID == dis.Id).Select(x => x.UserId).Distinct().Count();

                                    _db.SaveChanges();
                                }

                                var countDis = _db.NCR_DIS.Where(x => x.ITEM.Trim().Equals(det.ITEM.Trim()) & x.NCR_NUM.Trim().Equals(ncrnum.Trim())).Sum(x => x.QTY);

                                if (IdUser == UserIdChairman & countDis == det.QTY)
                                {
                                    det.DATEAPPROVAL = DateTime.Now;
                                }
                                _db.SaveChanges();
                            }
                            //comment lai cho TBD
                            //else if (det.DISPOSITION == CONFIRMITY_DISPN.ID_TBD && DisQTY < det.QTY && det.DATEAPPROVAL == null)
                            //{
                            //    var DISsNull = DisOfDet.Where(x => x.DATEAPPROVAL == null).ToList();
                            //    if (DISsNull.Count > 0)
                            //    {
                            //        foreach (var dis in DISsNull)
                            //        {
                            //            _db.UserDispositionApprovals.Add(new UserDispositionApproval
                            //            {
                            //                Id = Guid.NewGuid().ToString(),
                            //                Comment = "",
                            //                IsActive = true,
                            //                UserId = IdUser,
                            //                NCRNUM = NCR.NCR_NUM,
                            //                NCR_DIS_ID = dis.Id,
                            //                DET_Item = dis.ITEM.Trim(),
                            //                DateApprove = DateTime.Now
                            //            });

                            //            if (IdUser == UserIdChairman)
                            //            {
                            //                dis.DATEAPPROVAL = DateTime.Now;
                            //            }
                            //            approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.NCR_DIS_ID == dis.Id).Select(x => x.UserId).Distinct().Count();

                            //            _db.SaveChanges();
                            //        }
                            //        var countDis = _db.NCR_DIS.Where(x => x.ITEM.Trim().Equals(det.ITEM.Trim()) & x.NCR_NUM.Trim().Equals(ncrnum.Trim())).Sum(x => x.QTY);

                            //        if (IdUser == UserIdChairman & countDis == det.QTY)
                            //        {
                            //            det.DATEAPPROVAL = DateTime.Now;
                            //        }
                            //        _db.SaveChanges();
                            //    }
                            //}
                            //else if (DisQTY == det.QTY && det.DISPOSITION == CONFIRMITY_DISPN.ID_TBD && det.DATEAPPROVAL == null)
                            //{
                            //    foreach (var dis in DisOfDet)
                            //    {
                            //        _db.UserDispositionApprovals.Add(new UserDispositionApproval
                            //        {
                            //            Id = Guid.NewGuid().ToString(),
                            //            Comment = "",
                            //            IsActive = true,
                            //            UserId = IdUser,
                            //            NCRNUM = NCR.NCR_NUM,
                            //            DET_Item = det.ITEM,
                            //            NCR_DIS_ID = dis.Id,
                            //            DateApprove = DateTime.Now
                            //        });
                            //        approverCount = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.NCR_DIS_ID == dis.Id).Select(x => x.UserId).Distinct().Count();

                            //        _db.SaveChanges();
                            //    }

                            //    if (IdUser == UserIdChairman)
                            //    {
                            //        int approverCountForDET = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()) & x.IsActive == true & x.DET_Item == det.ITEM).Select(x => x.UserId).Distinct().Count();
                            //        if (approverCountForDET == Approvers.Count())
                            //        {
                            //            foreach (var dis in DisOfDet)
                            //            {
                            //                dis.DATEAPPROVAL = DateTime.Now;
                            //            }

                            //            //var disCount = DISs.Where(x => x.ITEM.Trim().Equals(det.ITEM.Trim())).Sum(x => x.QTY);
                            //            //if (det.QTY == disCount)
                            //            //   if (det.DISPOSITION != CONFIRMITY_DISPN.ID_TBD)
                            //            // {
                            //            det.DATEAPPROVAL = DateTime.Now;
                            //            // }
                            //            //         det.DATEAPPROVAL = DateTime.Now;
                            //        }
                            //    }

                            //    _db.SaveChanges();
                            //}
                        }

                        _db.SaveChanges();

                        var DETsNOTNULL = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(ncrnum) & x.DATEAPPROVAL == null).ToList();

                        if (approverCount < Approvers.Count() - 1 & DETsNOTNULL.Count() >= 0 & IdUser == UserIdChairman)
                        {
                            tranj.Rollback();
                            return new Result
                            {
                                success = false,
                                message = "Approval is unsuccessful, Please approve after MRB team!",
                                obj = ""
                            };
                        }

                        if (approverCount >= Approvers.Count() - 1 & DETsNOTNULL.Count() <= 0 & IdUser == UserIdChairman)
                        {
                            NCR.STATUS = StatusInDB.DispositionApproved;
                            NCR.DateApproval = DateTime.Now;
                        }

                        _db.SaveChanges();
                        tranj.Commit();
                        return new Result
                        {
                            success = true,
                            message = "Approval is successful",
                            obj = (approverCount == Approvers.Count() - 2 && IdUser != UserIdChairman) ? "EmailChairman" : (approverCount == Approvers.Count() - 1 && IdUser == UserIdChairman) ? "ApprovePartial" : ""
                        };
                    }
                }
                catch (Exception ex)
                {
                    var log = new LogWriter("UpdateUserAprrovalDate Exception");
                    log.LogWrite(ex.ToString() + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    tranj.Rollback();
                    return new Result
                    {
                        success = false,
                        message = "Approval is unsuccessful, Exception ",
                        obj = ""
                    };
                }
            }
        }
        public List<NcrDisViewmodel> GetAdditional(string NCR_NUM)
        {
            if (NCR_NUM != null)
            {
                var listrDets = from ap in _db.AspNetUsers
                                join dist in _db.NCR_DIS on ap.Id equals dist.INSPECTOR
                                join addIns in _db.ADD_INS on dist.ADD_INS equals addIns.ID
                                where (dist.NCR_NUM.Trim() == NCR_NUM.Trim())
                                select new NcrDisViewmodel
                                {
                                    ITEM = dist.ITEM.Trim(),
                                    QTY = dist.QTY,
                                    ADD_INS = dist.ADD_INS,
                                    ADD_INS_NAME = addIns.NAME,
                                    REMARK = dist.REMARK,
                                    INSPECTOR = ap.FullName,
                                    INS_DATE = dist.INS_DATE,
                                    DATEAPPROVAL = dist.DATEAPPROVAL
                                };
                return listrDets.OrderBy(m => m.ITEM).ToList();
            }
            else
            {
                return new List<NcrDisViewmodel>();
            }
        }

        public List<NC_GROUP> GetListNC_GRP_DESC(string ccn)
        {
            var listNGD = _db.NC_GROUP.Where(x => x.CCN == ccn).ToList();
            return listNGD != null ? listNGD : new List<NC_GROUP>();
        }

        public List<RESPON> getListRESPON()
        {
            var listRESPON = _db.RESPONs.ToList();
            return listRESPON;
        }
        public List<DISPOSITION> getListDispo()
        {
            var listDispo = _db.DISPOSITIONs.Where(x=>x.ID!="A").ToList();
            return listDispo;
        }
        public List<DISPOSITION> getListDispo_4_PrintNCR()
        {
            var listDispo = _db.DISPOSITIONs.ToList();
            return listDispo;
        }
        public List<ADD_INS> getListAddition()
        {
            var listadd = _db.ADD_INS.Where(x => x.ID != "A").ToList();
            return listadd;
        }
        public string NCRNUMFinal(string code)
        {
            List<NCR_HDR> list = new List<NCR_HDR>();
            if (code == "I")
            {
                list = _db.NCR_HDR.Where(x => x.SEC == "IQC").ToList();
                return list.Count != 0 ? list[list.Count - 1].NCR_NUM : "I0000000";
            }
            else
            {
                list = _db.NCR_HDR.Where(x => x.SEC == "PROCESS").ToList();
                return list.Count != 0 ? list[list.Count - 1].NCR_NUM : "P0000000";
            }

        }

        public bool CheckExistNCRNUM(string ncrNum)
        {
            return _db.NCR_HDR.Where(x => x.NCR_NUM == ncrNum).FirstOrDefault() == null ? false : true;
        }

        public string[] ArrayChar =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
            "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        public string GetAutoNCRNUM(string code)
        {
            string ncrNum = code + "0000001";
            string char1 = "", char2 = "", char3 = "", char4 = "", char5 = "", char6 = "", char7 = "";
            while (CheckExistNCRNUM(ncrNum))
            {
                ncrNum = NCRNUMFinal(code);
                if (ncrNum != "")
                {
                    var array = ncrNum.ToCharArray();
                    char1 = array[1].ToString();
                    char2 = array[2].ToString();
                    char3 = array[3].ToString();
                    char4 = array[4].ToString();
                    char5 = array[5].ToString();
                    char6 = array[6].ToString();
                    char7 = array[7].ToString();
                    if (Array.IndexOf(ArrayChar, array[7].ToString()) == 35)
                    {
                        char7 = "0";
                        if (Array.IndexOf(ArrayChar, array[6].ToString()) == 35)
                        {
                            char6 = "0";
                            if (Array.IndexOf(ArrayChar, array[5].ToString()) == 35)
                            {
                                char5 = "0";
                                if (Array.IndexOf(ArrayChar, array[4].ToString()) == 35)
                                {
                                    char4 = "0";
                                    if (Array.IndexOf(ArrayChar, array[3].ToString()) == 35)
                                    {
                                        char3 = "0";
                                        if (Array.IndexOf(ArrayChar, array[2].ToString()) == 35)
                                        {
                                            char2 = "0";
                                            if (Array.IndexOf(ArrayChar, array[1].ToString()) == 35)
                                            {
                                                char1 = "";
                                                char2 = "";
                                                char3 = "";
                                                char4 = "";
                                                char5 = "";
                                                char6 = "";
                                                char7 = "";
                                                break;
                                            }
                                        }
                                        else char2 = ArrayChar[Array.IndexOf(ArrayChar, array[2].ToString()) + 1];
                                    }
                                    else char3 = ArrayChar[Array.IndexOf(ArrayChar, array[3].ToString()) + 1];
                                }
                                else char4 = ArrayChar[Array.IndexOf(ArrayChar, array[4].ToString()) + 1];
                            }
                            else char5 = ArrayChar[Array.IndexOf(ArrayChar, array[5].ToString()) + 1];
                        }
                        else char6 = ArrayChar[Array.IndexOf(ArrayChar, array[6].ToString()) + 1];
                    }

                    else char7 = ArrayChar[Array.IndexOf(ArrayChar, array[7].ToString()) + 1];
                }
                ncrNum = code + char1 + char2 + char3 + char4 + char5 + char6 + char7;
            }
            return ncrNum;
        }

        public Result UpdateUserApproval(string id, string userId)
        {
            var _log = new LogWriter("UpdateUserApproval");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    _log.LogWrite("Parse ID");
                    int ID = int.Parse(id);
                    _log.LogWrite("get APPROVALs " + id);
                    var data = _db.APPROVALs.FirstOrDefault(x => x.ID == ID);
                    data.isActive = false;

                    _log.LogWrite("Add vew APPROVAL " + userId);
                    var ExistApprover = _db.APPROVALs.FirstOrDefault(x => x.RoleId.Equals(data.RoleId) & x.UserId.Equals(userId) & x.NCR_NUMBER.Equals(data.NCR_NUMBER) & x.isActive == true);
                    if (ExistApprover != null)
                    {
                        return new Result
                        {
                            success = false,
                            message = "Approve had Exist!",
                            obj = -1
                        };
                    }
                    var newApprover = new APPROVAL
                    {
                        CreateDate = DateTime.Now,
                        isActive = true,
                        NCR_NUMBER = data.NCR_NUMBER,
                        RoleId = data.RoleId,
                        UserId = userId.Trim()
                    };
                    _db.APPROVALs.Add(newApprover);

                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        obj = newApprover.ID
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception UpdateUserApproval!",
                        obj = -1
                    };
                }
            }

        }

        public List<SelectListItem> GetdropdownVendors()
        {
            List<SelectListItem> listvendor = _db.VENDORs.Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim(),
                Text = (x.VENDOR1.Trim()) + " " + (x.VEN_NAM),
            }).ToList();
            return listvendor;
        }

        public List<SelectListItem> GetdropdownVendorsVer2()
        {
            List<SelectListItem> listvendor = _db.VENDORs.Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim(),
                Text = (x.VENDOR1.Trim()) + " " + (x.VEN_NAM),
            }).ToList();
            return listvendor;
        }

        public bool UpdateRTVProcess(RTVProccessViewModel rtv)
        {
            try
            {
                var data = _db.RTVProcesses.Where(m => m.NCR_NUMBER == rtv.NCR_NUMBER).FirstOrDefault();
                if (data != null)
                {
                    data.Shipped = rtv.Shipped;
                    data.TypeRTV = rtv.TypeRTV;
                    data.Qty = rtv.Qty;
                    data.Remark = rtv.Remark;
                    data.CreditNote = rtv.CreditNote;
                    if (!string.IsNullOrWhiteSpace(rtv.CreditFile))
                    {
                        data.CreditFile = rtv.CreditFile;
                    }
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public List<DropdownlistViewModelPrint> GetdropdownNCRnum(string vendor)
        {
            var listvendor = _db.NCR_HDR.Where(x => x.VENDOR == vendor).Select(x => new DropdownlistViewModelPrint
            {
                NCR_NUM = x.NCR_NUM,
                MI_PART_NO = x.MI_PART_NO
            }).ToList();

            return listvendor;
        }

        public List<NCRManagementViewModel> GetListNCRSelect(List<string> list)
        {
            List<NCRManagementViewModel> listResult = new List<NCRManagementViewModel>();
            foreach (var item in list)
            {
                var data = _db.NCR_HDR.Where(x => x.NCR_NUM == item).FirstOrDefault();
                if (data != null)
                {
                    listResult.Add(new NCRManagementViewModel
                    {
                        NCR_NUM = data.NCR_NUM,
                    });
                }
            }
            return listResult;
        }

        public string getUrlNcr(string NCR_Num)
        {
            var data = _db.NCR_EVI.Where(x => x.NCR_NUM.Trim() == NCR_Num.Trim()).Select(x => x.EVI_PATH).FirstOrDefault();
            return data;
        }
        public int getIDfile(string NCR_Num)
        {
            var data = _db.NCR_EVI.Where(x => x.NCR_NUM.Trim() == NCR_Num.Trim()).Select(x => x.EVI_ID).FirstOrDefault();
            return data;
        }

        public AspNetUser getEmailbyuserid(string id)
        {
            var mail = _db.AspNetUsers.Where(x => x.Id.Trim() == id).FirstOrDefault();
            return mail;
        }
        public string SentEmailSubmitEdit(string id)
        {
            string mail = "";
            if (id != null)
            {
                mail = _db.AspNetUsers.Where(x => x.Id.Trim() == id).Select(u => u.Email).FirstOrDefault();
                _db.spSendEmail("IQC", mail, null, "Test Mail", null, "Test Email", null);
            }
            return mail;
        }
        public NCR_EVI GetFileWithFileID(int fileId)
        {
            return _db.NCR_EVI.Where(m => m.EVI_ID == fileId).FirstOrDefault();
        }
        public VENDOR getvendor(string vendor)
        {
            var data = _db.VENDORs.Where(m => m.VENDOR1.Trim() == vendor.Trim()).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// Add TaskManNCR
        /// By: Sil
        /// Date: 2018/06/27
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreateTaskManNCR(TaskManagementCreateModel model)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {

                    int currentTaskListID = 0;
                    //TaskList
                    _db.TASKLISTs.Add(model.TaskList);
                    currentTaskListID = model.TaskList.TopicID;
                    //TaskDetail
                    model.TaskDetail.TopicID = currentTaskListID;
                    TASKDETAIL taskDetail = new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = model.TaskDetail.TASKNAME,
                        OWNER = model.TaskDetail.OWNER,
                        ASSIGNEE = model.TaskDetail.ASSIGNEE,
                        STATUS = model.TaskDetail.STATUS
                    };
                    _db.TASKDETAILs.Add(taskDetail);

                    _db.SaveChanges();
                    tranj.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    var _log = new LogWriter("CreateTaskManNCR");
                    _log.LogWrite(ex.ToString());
                    if (ex is DbEntityValidationException)
                    {
                        var e = (DbEntityValidationException)ex;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                //    ve.PropertyName, ve.ErrorMessage);
                                _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                    }
                    return false;
                }
            }

        }


        #region Function
        /// <summary>
        /// Get ResponName By ResponID
        /// By: Sil
        /// Date: 07/04/2018
        /// </summary>
        /// <param name="RESPONSE"></param>
        /// <returns></returns>
        public string GetResponNameByID(string RESPONSE = "")
        {
            try
            {
                string name = _db.RESPONs.FirstOrDefault(m => m.ID == RESPONSE).NAME;
                return name;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Disposition Name By DispositionID
        /// By: Sil
        /// Date: 07/04/2018
        /// </summary>
        /// <param name="Dispo"></param>
        /// <returns></returns>
        public string GetDispoNameByID(string Dispo)
        {
            try
            {
                string name = _db.DISPOSITIONs.Where(m => m.ID == Dispo).FirstOrDefault().NAME;
                return name;
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
        }

        /// <summary>
        /// Get AddIns Name By AddInsID
        /// By: Sil
        /// Date: 07/04/2018
        /// </summary>
        /// <param name="AddIns"></param>
        /// <returns></returns>
        public string GetAddInsNameByID(string AddIns)
        {
            try
            {
                string name = _db.ADD_INS.Where(m => m.ID.Trim() == AddIns.Trim()).FirstOrDefault().NAME;
                return name;
            }
            catch (Exception ex)
            {
                new LogWriter("GetAddInsNameByID").LogWrite(ex.ToString());
                return string.Empty;
            }
        }

        public bool DeleteAddIns(string ncrNum, string item)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    var checkApp = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Equals(ncrNum.Trim()) & x.NCR_DIS_ID.Equals(item)).ToList();
                    if (checkApp.Count > 0) return false;
                    var addinsTemp = _db.NCR_DIS.FirstOrDefault(x => x.NCR_NUM.Equals(ncrNum) & x.Id.Equals(item));
                    // add on 22-01-2019 : unactive orderdisposition
                    var ors = _db.OrderDispositions.Where(x => x.ITEM.Equals(addinsTemp.ITEM.Trim()) & x.IsActive == true &
                    x.NCR_NUMBER.Equals(addinsTemp.NCR_NUM.Trim()) & x.Type.Equals("DIS")).ToList();
                    foreach (var or in ors)
                    {
                        or.IsActive = false;
                    }

                    if (addinsTemp != null)
                    {
                        _db.NCR_DIS.Remove(addinsTemp);
                        _db.SaveChanges();
                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    new LogWriter("DeleteAddIns").LogWrite(ex.ToString());
                    return false;
                }
            }
        }
        public bool EditAddIns(string ncrNum, string item, double qty, string remark = "")
        {
            try
            {
                var checkApp = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Equals(ncrNum.Trim()) & x.NCR_DIS_ID.Equals(item)).ToList();
                if (checkApp.Count > 0) return false;

                var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(ncrNum.Trim()));
                bool isAQL = NCR.SAMPLE_INSP == true;
                remark = remark == null ? "" : remark;

                var addinsTemp = _db.NCR_DIS.FirstOrDefault(x => x.NCR_NUM.Equals(ncrNum) & x.Id.Equals(item));



                if (!isAQL)
                {
                    double DETQTY = _db.NCR_DET.Where(x => x.ITEM.Trim().Equals(addinsTemp.ITEM.Trim()) & x.NCR_NUM.Trim().Equals(ncrNum.Trim())).Sum(x => x.QTY);
                    double DISQTY = qty;

                    var DIS = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(ncrNum.Trim()) & x.ITEM.Trim().Equals(addinsTemp.ITEM.Trim()) & x.Id != addinsTemp.Id).ToList();
                    if (DIS.Count > 0) DISQTY += DIS.Sum(x => x.QTY);
                    if (DETQTY < DISQTY)
                    {
                        return false;
                    }
                }
                //else
                //{
                //    double sum = qty;
                //    var DIS_QTY = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(ncrNum.Trim()) & x.Id != item.Trim());
                //    if (DIS_QTY != null) sum += DIS_QTY.Sum(x => x.QTY);
                //    if (sum != NCR.REJ_QTY) return false;
                //}


                if (addinsTemp != null)
                {
                    var qtytmp = addinsTemp.QTY;
                    var remarktmp = addinsTemp.REMARK;
                    addinsTemp.QTY = qty;
                    addinsTemp.REMARK = remark;
                    //return qty != qtytmp | remark != remarktmp ? 
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                var _log = new LogWriter("EditAddIns");
                _log.LogWrite(ex.ToString());
                _log.LogWrite(ex.InnerException.Message);
                return false;
            }
        }

        public CApproval CheckApproval(string ncrNum)
        {
            CApproval Approval = new CApproval();
            try
            {
                var UserApproval = _db.APPROVALs.FirstOrDefault(x => x.NCR_NUMBER.Equals(ncrNum));
                var listDET = _db.NCR_DET.Where(x => x.NCR_NUM.Equals(ncrNum)).ToList();
                var listDIS = _db.NCR_DIS.Where(x => x.NCR_NUM.Equals(ncrNum)).ToList();
                var CountDET = listDET.Sum(x => x.QTY);
                var CountDIS = listDIS.Sum(x => x.QTY);
                if ((listDET.Where(x => !x.DATEAPPROVAL.HasValue).Count() <= 0))
                {
                    Approval.CheckedAll(listDET.OrderByDescending(x => x.DATEAPPROVAL).First(), UserApproval);
                }
                else
                {
                    var lstDET = listDET.Where(x => !x.DATEAPPROVAL.HasValue).ToList();
                    var DetDesc = lstDET.FirstOrDefault(x => !x.RESPONSE.Equals(CONFIRMITY_RESPON.ID_DESCRIBE) & !x.DISPOSITION.Equals(CONFIRMITY_DISPNP.ID_DESCRIBE));
                    if (DetDesc != null)
                    {
                        var ItemDetDescTMP = int.Parse(DetDesc.ITEM);
                        var ItemCheckDetDesc = (++ItemDetDescTMP).ToString();
                        var DisOfDetDesc = listDIS.Where(x => x.ITEM.Equals(ItemCheckDetDesc)).ToList();
                        if (DisOfDetDesc.Count <= 0)
                        {

                            Approval.CheckedAll(DetDesc, UserApproval);
                        }
                    }
                    else
                    {
                        var CountTmp = CountDIS + listDET.Where(x => x.DATEAPPROVAL.HasValue).Sum(x => x.QTY);
                        if (CountDET > CountTmp & listDIS.FirstOrDefault(x => !x.DATEAPPROVAL.HasValue) == null)
                        {
                            Approval.SetCheck(false);
                        }
                        else
                        {
                            Approval.IsAllApproval = false;
                            Approval.QUALITY = listDIS.Where(x => !x.QUALITY.HasValue).Count() == 0;
                            Approval.ENGIEERING = listDIS.Where(x => !x.ENGIEERING.HasValue).Count() == 0;
                            Approval.MFG = UserApproval.MFG != null ? listDIS.Where(x => !x.MFG.HasValue).Count() == 0 : true;
                            Approval.PURCHASING = UserApproval.PURCHASING != null ? listDIS.Where(x => !x.PURCHASING.HasValue).Count() == 0 : true;
                            Approval.SetDate(listDIS.OrderByDescending(x => x.DATEAPPROVAL).First());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Approval.SetCheck(false);
                new LogWriter("CheckApproval").LogWrite(ex.ToString());
            }
            return Approval;

        }

        public void CloseNCR(string NCR_NUM, string uid)
        {
            var ncr = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(NCR_NUM));
            if (ncr != null)
            {
                ncr.STATUS = StatusInDB.Close;

                _db.NCR_History.Add(new NCR_History
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = "Change Status",
                    CreateDate = DateTime.Now,
                    IsActive = false,
                    NCRNUM = NCR_NUM,
                    Status = StatusInDB.Close,
                    UserId = uid
                });

                _db.SaveChanges();
            }
        }
        #endregion
        public bool CheckDubplicateNCRCreate(string receiver)
        {
            var check = _db.INS_RESULT_DEFECT.Where(x => x.receiver.Trim() == receiver.Trim()).FirstOrDefault();
            //if(check.NCR_Num == null)
            //{
            //    return false;
            //}
            //return true;
            return check != null ? check.NCR_Num != null : true;
        }

        public List<NCR_EVI> GetEVIByIds(List<string> eviIds)
        {
            var arr = eviIds.Select(int.Parse).ToArray();
            var evis = _db.NCR_EVI.Where(x => arr.Contains(x.EVI_ID)).ToList();
            return evis.Count > 0 ? evis : new List<NCR_EVI>();
        }

        public List<NCR_EVI> GetEVIByNCRNUM(List<string> lstNCRNUM)
        {
            var arr = lstNCRNUM.ToArray();
            var evis = _db.NCR_EVI.Where(x => arr.Contains(x.NCR_NUM.Trim())).ToList();
            return evis.Count > 0 ? evis : new List<NCR_EVI>();
        }

        public Result CreateNCRForIQC(NCR_HDR nCR_HDR, List<NCR_DET> nCR_DETs, List<NCR_EVI> nCR_EVIs, List<APPROVAL> aPPROVALs,
            List<TaskManagementCreateModel> taskManagementCreateModels, VNMaterialTraceability vNMaterialTraceability, string RECEIVER)
        {
            _db = new IIVILocalDB();
            var log = new LogWriter("NCRManagementService: 1995 - CreateNCR ");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var STDCOST = _db.STDCOSTs.FirstOrDefault(x => x.Part.Equals(nCR_HDR.MI_PART_NO) & x.CCN.Equals(nCR_HDR.CCN));
                    if (STDCOST == null)
                        return new Result
                        {
                            success = false,
                            message = "Part is not exist Std code !!!"
                        };
                    nCR_HDR.Amount = (nCR_HDR.REJ_QTY.Value * STDCOST.StdCost1.Value);
                    log.LogWrite("NCRManagementService: 2000");
                    _db.NCR_HDR.Add(nCR_HDR);

                    log.LogWrite("NCRManagementService: 2002");
                    _db.VNMaterialTraceabilities.Add(vNMaterialTraceability);

                    log.LogWrite("NCRManagementService: 2006");
                    if (nCR_DETs.Count > 0)
                    {
                        foreach (var det in nCR_DETs)
                        {
                            _db.NCR_DET.Add(det);
                        }
                    }

                    log.LogWrite("NCRManagementService: 2015");
                    if (nCR_EVIs.Count > 0)
                    {
                        foreach (var evi in nCR_EVIs)
                        {
                            _db.NCR_EVI.Add(evi);
                        }
                    }

                    log.LogWrite("NCRManagementService: 2024");
                    if (aPPROVALs.Count > 0)
                    {
                        foreach (var approver in aPPROVALs)
                        {
                            _db.APPROVALs.Add(approver);
                        }
                    }

                    log.LogWrite("NCRManagementService: 2033");
                    //if (taskManagementCreateModels.Count > 0)
                    //{
                    //    foreach (var task in taskManagementCreateModels)
                    //    {
                    //        int currentTaskListID = 0;
                    //        //TaskList
                    //        _db.TASKLISTs.Add(task.TaskList);
                    //        currentTaskListID = task.TaskList.TopicID;
                    //        //TaskDetail
                    //        task.TaskDetail.TopicID = currentTaskListID;
                    //        TASKDETAIL taskDetail = new TASKDETAIL
                    //        {
                    //            TopicID = currentTaskListID,
                    //            TASKNAME = task.TaskDetail.TASKNAME,
                    //            OWNER = task.TaskDetail.OWNER,
                    //            ASSIGNEE = task.TaskDetail.ASSIGNEE,
                    //            STATUS = task.TaskDetail.STATUS
                    //        };
                    //        _db.TASKDETAILs.Add(taskDetail);
                    //    }
                    //}

                    log.LogWrite("NCRManagementService: 2057");
                    if (!string.IsNullOrEmpty(RECEIVER))
                    {
                        var INS_RESULT_DEFECTs = _db.INS_RESULT_DEFECT.Where(x => x.NCR_Num == null && x.receiver.Trim() == RECEIVER.Trim()).ToList();

                        foreach (var INS_RESULT_DEFECT in INS_RESULT_DEFECTs)
                        {
                            _db.INS_RESULT_DEFECT.Attach(INS_RESULT_DEFECT);
                            INS_RESULT_DEFECT.NCR_Num = nCR_HDR.NCR_NUM.Trim();

                            _db.Entry(INS_RESULT_DEFECT).State = EntityState.Modified;
                        }
                    }

                    _db.SaveChanges();
                    tranj.Commit();
                    log.LogWrite("NCRManagementService: 2071 - Done CreateNCR");
                    return new Result
                    {
                        message = $@"Create NCR {nCR_HDR.NCR_NUM} success!",
                        success = true,
                        obj = nCR_HDR.NCR_NUM
                    };
                }
                catch (Exception ex)
                {
                    log.LogWrite("NCRManagementService: 2075:" + Environment.NewLine + ex.ToString());
                    if (ex is DbEntityValidationException)
                    {
                        var e = (DbEntityValidationException)ex;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                    }
                    return new Result
                    {
                        message = "Save NCR Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }

        public float GetSamplingSize(double rec_qty)
        {
            System.Data.Entity.Core.Objects.ObjectParameter objectParameter = new System.Data.Entity.Core.Objects.ObjectParameter("Sampling", typeof(float));
            _db.pAQL_Cal(rec_qty, objectParameter);
            return float.Parse(objectParameter.Value + "");
        }

        public Result CreateNCRForInProcess(NCR_HDR nCR_HDR, List<NCR_DET> nCR_DETs, List<NCR_EVI> nCR_EVIs, List<APPROVAL> aPPROVALs, List<TaskManagementCreateModel> taskManagementCreateModels, VNMaterialTraceability vNMaterialTraceability, string RECEIVER, INS_RESULT_FINAL iNS_RESULT_FINAL)
        {
            _db = new IIVILocalDB();
            var log = new LogWriter("NCRManagementService: 2128 - CreateNCRForInProcess ");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var STDCOST = _db.STDCOSTs.FirstOrDefault(x => x.Part.Equals(nCR_HDR.MI_PART_NO) & x.CCN.Equals(nCR_HDR.CCN));
                    if (STDCOST == null)
                        return new Result
                        {
                            success = false,
                            message = "Part is not exist Std code !!!"
                        };
                    nCR_HDR.Amount = (nCR_HDR.REJ_QTY.Value * STDCOST.StdCost1.Value);
                    log.LogWrite("NCRManagementService: 2133");
                    _db.NCR_HDR.Add(nCR_HDR);

                    log.LogWrite("NCRManagementService: 2136");
                    _db.VNMaterialTraceabilities.Add(vNMaterialTraceability);

                    log.LogWrite("NCRManagementService: 2139");
                    if (nCR_DETs.Count > 0)
                    {
                        //Tuan lua add-2019
                        var hashset = new HashSet<string>();
                        foreach (var det in nCR_DETs)
                        {
                            if (!hashset.Add(det.NC_DESC))
                            {
                                return new Result
                                {
                                    success = false,
                                    message = "DEFECT is Duplicate !!!"
                                };
                            }
                            else
                            {
                                _db.NCR_DET.Add(det);
                            }
                        }
                    }

                    log.LogWrite("NCRManagementService: 2148");
                    if (nCR_EVIs.Count > 0)
                    {
                        foreach (var evi in nCR_EVIs)
                        {
                            _db.NCR_EVI.Add(evi);
                        }
                    }

                    log.LogWrite("NCRManagementService: 2157");
                    if (aPPROVALs.Count > 0)
                    {
                        foreach (var approver in aPPROVALs)
                        {
                            _db.APPROVALs.Add(approver);
                        }
                    }

                    log.LogWrite("NCRManagementService: 2166");
                    //if (taskManagementCreateModels.Count > 0)
                    //{
                    //    foreach (var task in taskManagementCreateModels)
                    //    {
                    //        int currentTaskListID = 0;
                    //        //TaskList
                    //        _db.TASKLISTs.Add(task.TaskList);
                    //        currentTaskListID = task.TaskList.TopicID;
                    //        //TaskDetail
                    //        task.TaskDetail.TopicID = currentTaskListID;
                    //        TASKDETAIL taskDetail = new TASKDETAIL
                    //        {
                    //            TopicID = currentTaskListID,
                    //            TASKNAME = task.TaskDetail.TASKNAME,
                    //            OWNER = task.TaskDetail.OWNER,
                    //            ASSIGNEE = task.TaskDetail.ASSIGNEE,
                    //            STATUS = task.TaskDetail.STATUS
                    //        };
                    //        _db.TASKDETAILs.Add(taskDetail);
                    //    }
                    //}

                    log.LogWrite("NCRManagementService: 2189");
                    //if (!string.IsNullOrEmpty(RECEIVER))
                    //{
                    //    var INS_RESULT_DEFECTs = _db.INS_RESULT_DEFECT.Where(x => x.NCR_Num == null && x.receiver.Trim() == RECEIVER.Trim()).ToList();

                    //    foreach (var INS_RESULT_DEFECT in INS_RESULT_DEFECTs)
                    //    {
                    //        _db.INS_RESULT_DEFECT.Attach(INS_RESULT_DEFECT);
                    //        INS_RESULT_DEFECT.NCR_Num = nCR_HDR.NCR_NUM.Trim();
                    //    }
                    //}
                    log.LogWrite("NCRManagementService: 2200");
                    if (!string.IsNullOrEmpty(iNS_RESULT_FINAL.RECEIVER))
                        _db.INS_RESULT_FINAL.Add(iNS_RESULT_FINAL);

                    _db.SaveChanges();
                    tranj.Commit();
                    log.LogWrite("NCRManagementService: 2205 - Done CreateNCR");
                    return new Result
                    {
                        message = $@"Create NCR {nCR_HDR.NCR_NUM} success!",
                        success = true,
                        obj = nCR_HDR.NCR_NUM
                    };
                }
                catch (Exception ex)
                {
                    log.LogWrite("NCRManagementService: 2214:" + Environment.NewLine + ex.ToString());
                    if (ex is DbEntityValidationException)
                    {
                        var e = (DbEntityValidationException)ex;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                    }
                    return new Result
                    {
                        message = "Save NCR Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }

        public NCR_HDR GetNCR_HDR(string ncr_num)
        {
            return _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(ncr_num));
        }

        public string GetNCR_DET_DESC(string ncr_num)
        {
            string res = "";
            var data = _db.NCR_DET.Where(x => x.NCR_NUM.Equals(ncr_num)).ToList();
            foreach (var item in data)
            {
                if (item != null)
                {
                    res += $"{item.NC_DESC}; ";
                }
            }
            return res;
        }

        public List<NCR_EVI> GetUploadedEvidence(string ncrnum)
        {
            var lstEvidence = _db.NCR_EVI.Where(x => x.NCR_NUM.Equals(ncrnum)).OrderByDescending(x => x.EVI_ID).ToList();
            return lstEvidence;
        }

        public string GetStringUploadedVNMaterialTraceability(string ncrnum)
        {
            var oldFile = _db.VNMaterialTraceabilities.FirstOrDefault(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()));
            return oldFile != null ? oldFile.Id : "";
        }

        public VNMaterialTraceability GetUploadedVNMaterialTraceability(string ncrnum)
        {
            var oldFile = _db.VNMaterialTraceabilities.FirstOrDefault(x => x.NCRNUM.Trim().Equals(ncrnum.Trim()));
            return oldFile;
        }

        public VNMaterialTraceability GetVNMaterialTraceabilityByID(string id)
        {
            var VNMaterialTraceability = _db.VNMaterialTraceabilities.FirstOrDefault(x => x.Id.Equals(id));
            return VNMaterialTraceability;
        }

        public Result SaveEditNonComformity(List<NCR_DET> nCR_DETs, string NCR_NUM, List<NCR_EVI> nCR_EVIs, VNMaterialTraceability vNMaterialTraceability, List<string> EVIID, string NCRType)
        {
            var _log = new LogWriter("SaveEditNonComformity");

            _db = new IIVILocalDB();
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    _log.LogWrite("NCRManagementService: 2291");
                    if (vNMaterialTraceability.Id != null)
                        _db.VNMaterialTraceabilities.Add(vNMaterialTraceability);

                    _log.LogWrite("NCRManagementService: 2295");
                    if (EVIID != null)
                    {
                        var EVIIDDelete = EVIID.Select(x => int.Parse(x)).ToArray();
                        var EVIDelete = _db.NCR_EVI.Where(x => !EVIIDDelete.Contains(x.EVI_ID) & x.NCR_NUM.Equals(NCR_NUM)).ToList();
                        if (EVIDelete.Count > 0) _db.NCR_EVI.RemoveRange(EVIDelete);
                    }
                    else
                    {
                        var EVIDelete = _db.NCR_EVI.Where(x => x.NCR_NUM.Equals(NCR_NUM)).ToList();
                        if (EVIDelete.Count > 0) _db.NCR_EVI.RemoveRange(EVIDelete);
                    }
                    if (nCR_EVIs.Count > 0)
                    {
                        foreach (var evi in nCR_EVIs)
                        {
                            _db.NCR_EVI.Add(evi);
                        }
                    }

                    var DETs = _db.NCR_DET.Where(x => x.NCR_NUM == NCR_NUM).ToList();
                    if (NCRType == II_VI_Incorporated_SCM.Services.NCRType.PROCESS)
                    {
                        //Delete NCR_DET 
                        var lstDETDelete = DETs;
                        _db.NCR_DET.RemoveRange(lstDETDelete);
                        _log.LogWrite("Delete NCR_DET");
                        //Add new NCR_DET
                        //bat trung list khi add 
                        var hashset = new HashSet<string>();
                        foreach (var det in nCR_DETs)
                        {
                            if (!hashset.Add(det.NC_DESC))
                            {
                                return new Result
                                {
                                    success = false,
                                    message = "DEFECT is Duplicate !!!"
                                };
                            }
                            else
                            {
                                _db.NCR_DET.Add(det);
                            }
                        }
                        //   _db.NCR_DET.AddRange(nCR_DETs);
                        _log.LogWrite("Add new NCR_DET");
                    }
                    else
                    {
                        foreach (var item in nCR_DETs)
                        {
                            var tmp = DETs.FirstOrDefault(x => x.ITEM == item.ITEM);
                            tmp.REMARK = item.REMARK;
                        }
                    }
                    //Status of ncr is created
                    UpdateStatusNCR(NCR_NUM, StatusInDB.Created, "");

                    _db.SaveChanges(); _log.LogWrite("SaveChange");
                    tranj.Commit(); _log.LogWrite("Commit");
                    return new Result { success = true, message = "" };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    if (ex is DbEntityValidationException)
                    {
                        var e = (DbEntityValidationException)ex;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                    }

                    return new Result { success = false, message = $@"Exception: {CallStack.GetFileName()} - {CallStack.GetMethod() } - {CallStack.GetFileLineNumber()}" };
                }
            }
        }

        public bool CheckScrap(string partNum)
        {
            var StdCode = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(partNum));
            if (StdCode == null) return false;
            return StdCode.Amount <= int.Parse(ConfigurationManager.AppSettings["SCRAPMONEY"]);
        }

        public Result SubmitScrap(string nCR_NUM, string UserIdChange, string Comment, string WHMRBid)
        {
            var _log = new LogWriter("SubmitScrap");
            using (var tranj = _db.Database.BeginTransaction()) {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(nCR_NUM));
                    var TaskList = _db.TASKLISTs.FirstOrDefault(x => x.Topic.Trim() == nCR_NUM.Trim());
                    ////add task cho WHMRB set cho Yen. 
                    //var idYEN = _db.AspNetUsers.Where(x => x.Id == "d30a06df-66b1-41a4-aab2-7ecaaf1dbb15").FirstOrDefault();
                    //Thi.Nguyen 17-jan-20: WHMRBid will be add in viewer for easy editing
                    string idYEN = WHMRBid;
                    //End
                    if (NCR == null)
                        return new Result
                        {
                            success = false,
                            message = "Not Exist NCR " + nCR_NUM
                        };

                    NCR.STATUS = StatusInDB.Close;
                    NCR.DATESUBMIT = DateTime.Now;
                    NCR.USERSUBMIT = UserIdChange;
                    NCR.Comment = !string.IsNullOrEmpty(Comment) ? Comment + $" NC value less than {ConfigurationManager.AppSettings["SCRAPMONEY"]}$, allow SCRAP at production cost" :
                                        $"NC value less than {ConfigurationManager.AppSettings["SCRAPMONEY"]}$, allow SCRAP at production cost";
                    //!string.IsNullOrEmpty(Comment) ? Comment + $" NC value less than {ConfigurationManager.AppSettings["SCRAPMONEY"]}$, allow SCRAP at production cost" :
                    //  NCR.Comment= $"NC value less than {ConfigurationManager.AppSettings["SCRAPMONEY"]}$, allow SCRAP at production cost";
                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change Status",
                        NCRNUM = nCR_NUM,
                        Status = StatusInDB.Close,
                        UserId = UserIdChange,
                        CreateDate = DateTime.Now,
                        IsActive = false
                    });
                    if (TaskList == null)
                    {
                        TaskList = new TASKLIST
                        {
                            Topic = NCR.NCR_NUM,
                            TYPE = "NCR",
                            WRITEDATE = DateTime.Now,
                            WRITTENBY = UserIdChange,
                            Level=1,
                            Reference = NCR.NCR_NUM
                        };
                        _db.TASKLISTs.Add(TaskList);
                        _db.SaveChanges();
                    }
                    var currentTaskListID = TaskList.TopicID;
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = "GLOVIA transaction",
                        OWNER = idYEN,//idYEN.Id,
                        ASSIGNEE = idYEN,// idYEN.Id,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        Level = 1,
                    });
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        message = $"Scrap NCR {nCR_NUM} success !"
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        success = false,
                        message = "Exception SubmitScrap"
                    };
                }
            }
        }
        public bool CheckFloorNCR(string nCR_NUM)
        {
            var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(nCR_NUM));
            if (NCR.MRB_LOC!=null && !NCR.MRB_LOC.Contains("WH"))
                return true;
            else return false;
        }
        public Result FloorSubmitNCR(List<APPROVAL> aPPROVALs, List<TaskManagementCreateModel> taskManagementCreateModels, string nCR_NUM, string UserIDChange, string Comment,string MRB_Loc)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(nCR_NUM));
                    if (NCR == null)
                        return new Result
                        {
                            success = false,
                            message = "Not Exist NCR " + nCR_NUM
                        };
                    NCR.STATUS = StatusInDB.Submitted;
                    NCR.Comment = !string.IsNullOrEmpty(Comment) ? Comment : "";
                    NCR.USERSUBMIT = UserIDChange;
                    NCR.DATESUBMIT = DateTime.Now;
                    NCR.MRB_LOC = MRB_Loc;
                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change Status",
                        NCRNUM = nCR_NUM,
                        Status = StatusInDB.Submitted,
                        UserId = UserIDChange,
                        CreateDate = DateTime.Now,
                        IsActive = false
                    });

                    
                    _db.APPROVALs.AddRange(aPPROVALs);

                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result { success = true };
                }
                catch (Exception ex)
                {
                    var _log = new LogWriter("");
                    tranj.Rollback();
                    return new Result { success = false, message = "Exception: FloorSubmitNCR 2459 "+ ex.Message};
                }
            }
        }
        public Result NormalSubmitNCR(List<APPROVAL> aPPROVALs, List<TaskManagementCreateModel> taskManagementCreateModels, string nCR_NUM, string UserIDChange, string Comment)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(nCR_NUM));
                    if (NCR == null)
                        return new Result
                        {
                            success = false,
                            message = "Not Exist NCR " + nCR_NUM
                        };
                    NCR.STATUS = StatusInDB.Submitted;
                    NCR.Comment = !string.IsNullOrEmpty(Comment) ? Comment : "";
                    NCR.USERSUBMIT = UserIDChange;
                    NCR.DATESUBMIT = DateTime.Now;
                    NCR.MRB_LOC = "WH_MRB";
                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change Status",
                        NCRNUM = nCR_NUM,
                        Status = StatusInDB.Submitted,
                        UserId = UserIDChange,
                        CreateDate = DateTime.Now,
                        IsActive = false
                    });

                    //var UserApproved = _db.APPROVALs.Where(x=>x.NCR_NUMBER.Trim().Equals(nCR_NUM.Trim()) & x.isActive == true).ToList();
                    //foreach (var user in UserApproved)
                    //{
                    //    user.isActive = false;
                    //}

                    _db.APPROVALs.AddRange(aPPROVALs);

                    //if (taskManagementCreateModels.Count > 0)
                    //{
                    //    foreach (var task in taskManagementCreateModels)
                    //    {
                    //        int currentTaskListID = 0;
                    //        //TaskList
                    //        var taskadd = _db.TASKLISTs.Add(task.TaskList);
                    //        _db.SaveChanges();

                    //        currentTaskListID = taskadd.ID;
                    //        //TaskDetail
                    //        task.TaskDetail.TASKID = currentTaskListID;
                    //        TASKDETAIL taskDetail = new TASKDETAIL
                    //        {
                    //            TASKID = currentTaskListID,
                    //            TASKNAME = task.TaskDetail.TASKNAME,
                    //            OWNER = task.TaskDetail.OWNER,
                    //            ASSIGNEE = task.TaskDetail.ASSIGNEE,
                    //            STATUS = task.TaskDetail.STATUS
                    //        };
                    //        _db.TASKDETAILs.Add(taskDetail);
                    //        _db.SaveChanges();
                    //    }
                    //}

                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result { success = true };
                }
                catch (Exception ex)
                {
                    var _log = new LogWriter("");
                    tranj.Rollback();
                    return new Result { success = false, message = "Exception: NormalSubmitNCR 2459" };
                }
            }
        }
        public List<UserApproval> GetApproverOfNCRForConfirmNotChairMain(string nCR_NUM)
        {
            List<UserApproval> Result = new List<UserApproval>();
            var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCR_NUM));
            var REJ = NCR.REJ_QTY;
            var DEF = NCR.DEFECTIVE;
            bool IsAQL = NCR.SAMPLE_INSP == true;

            var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCR_NUM)).ToList();
            var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCR_NUM)).ToList();
            var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(nCR_NUM) & x.isActive == true && x.RoleId != "19848263-50d2-4dd8-8c02-83a2605ef094").ToList();
            var QTY = DISs.Sum(x => x.QTY);
            //var RoleIDChairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.CHAIRMAN).Id;
            //var UserIdChairman = Approvers.FirstOrDefault(x => x.RoleId == RoleIDChairman).UserId;
            var UserApprove = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCR_NUM) & x.IsActive == true).ToList();
            var arrIdUserApprove = Approvers.Select(x => x.UserId).ToArray();
            var arrIdGroupRole = Approvers.Select(x => x.RoleId).ToArray();
            var LstUser = _db.AspNetUsers.Where(x => arrIdUserApprove.Contains(x.Id)).ToList();
            var LstGroupRole = _db.ApplicationGroups.Where(x => arrIdGroupRole.Contains(x.Id)).ToList();

            if (arrIdGroupRole.Length == 0 || arrIdUserApprove.Length == 0)
            {
                return new List<UserApproval>();
                //if(string.IsNullOrEmpty(arrIdGroupRole[0]) || string.IsNullOrEmpty(arrIdUserApprove[0])) return new List<UserApproval>();
            }

            if (NCR.STATUS.Trim() != StatusInDB.WaitingForDispositionApproval & NCR.STATUS.Trim() != StatusInDB.DispositionApproved & NCR.STATUS.Trim() != StatusInDB.Close)
            {
                var DataApprovers = from a in Approvers
                                    select new UserApproval
                                    {
                                        Id = a.ID.ToString(),
                                        IdUser = a.UserId,
                                        FullName = LstUser.FirstOrDefault(x => x.Id.Equals(a.UserId)).FullName,
                                        IsAppr = false,
                                        RoleId = a.RoleId,
                                        RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(a.RoleId)).Name,
                                        Signature = LstUser.FirstOrDefault(x => x.Id.Equals(a.UserId)).Signature
                                    };
                return DataApprovers.ToList();
            }

            if (IsAQL)
            {
                var ApproverIDsForAQL = UserApprove.Select(x => x.UserId);
                var ApproverIDUniqueForAQL = new HashSet<string>(ApproverIDsForAQL).ToArray();
                foreach (var approverForAQL in Approvers)
                {
                    var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverForAQL.UserId) & x.IsActive == true).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                    var restmp = new UserApproval
                    {
                        Id = approverForAQL.ID.ToString(),
                        FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverForAQL.UserId)).FullName,
                        IdUser = approverForAQL.UserId,
                        RoleId = approverForAQL.RoleId,
                        RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverForAQL.RoleId)).Name,
                        Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverForAQL.UserId)).Signature,
                        IsAppr = approveTMP != null,
                        DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                    };
                    Result.Add(restmp);
                }

                return Result;
            }
            else
            {
                var DETsNULL = DETs.Where(x => x.DATEAPPROVAL == null).ToList();
                if (DETsNULL.Count <= 0)
                {
                    var ApproverIDsFor100Approval = UserApprove.Select(x => x.UserId);
                    var ApproverIDUniqueFor100Approval = new HashSet<string>(ApproverIDsFor100Approval).ToArray();
                    foreach (var approverFor100Approval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100Approval.UserId) & x.IsActive == true).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100Approval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).FullName,
                            IdUser = approverFor100Approval.UserId,
                            RoleId = approverFor100Approval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).Signature,
                            IsAppr = true,

                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }

                    return Result;
                }
                // if exsit NCR_DET => not done disposition
                // get approver not approve.
                // Comparse with NCR_DET non DIS OR have DIS
                var ApproverIDsFor100NonApproval = UserApprove.Select(x => x.UserId).ToList();
                var ApproverIDUniqueFor100NonApproval = new HashSet<string>(ApproverIDsFor100NonApproval).ToArray().ToList();

                //get det has additional
                //if not exist get any det to get approval
                var DET_DIS = DETsNULL
                    .Join(DISs, det => det.ITEM.Trim(), dis => dis.ITEM.Trim(), (det, dis) => new { det, dis })
                    .Where(x => x.dis.DATEAPPROVAL == null)
                    .Select(x => x.dis).ToList();

                var DETNONDIS = DETsNULL
                    .GroupJoin(DISs,
                                  n => n.ITEM.Trim(),
                                  m => m.ITEM.Trim(),
                                  (n, ms) => new { n, ms = ms.DefaultIfEmpty() })
                                  .Select(x => x.n).ToList();


                if (DET_DIS.Count > 0)
                {
                    var disTMP = DET_DIS[0];
                    foreach (var approverFor100NonApproval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100NonApproval.UserId) & x.IsActive == true & x.NCR_DIS_ID == disTMP.Id).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100NonApproval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).FullName,
                            IdUser = approverFor100NonApproval.UserId,
                            RoleId = approverFor100NonApproval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).Signature,
                            IsAppr = approveTMP != null,
                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }
                    return Result;
                }
                else
                {
                    var DETTMP = DETNONDIS[0];
                    foreach (var approverFor100NonApproval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100NonApproval.UserId) & x.IsActive == true & x.DET_Item.Trim() == DETTMP.ITEM).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100NonApproval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).FullName,
                            IdUser = approverFor100NonApproval.UserId,
                            RoleId = approverFor100NonApproval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).Signature,
                            IsAppr = approveTMP != null,
                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }
                    return Result;
                }
            }
        }
        public List<UserApproval> GetApproverOfNCRForConfirm(string nCR_NUM)
        {
            List<UserApproval> Result = new List<UserApproval>();
            var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCR_NUM));
            var REJ = NCR.REJ_QTY;
            var DEF = NCR.DEFECTIVE;
            bool IsAQL = NCR.SAMPLE_INSP == true;

            var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCR_NUM)).ToList();
            var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCR_NUM)).ToList();
            var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(nCR_NUM) & x.isActive == true).ToList();
            var QTY = DISs.Sum(x => x.QTY);
            //var RoleIDChairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.CHAIRMAN).Id;
            //var UserIdChairman = Approvers.FirstOrDefault(x => x.RoleId == RoleIDChairman).UserId;
            var UserApprove = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCR_NUM) & x.IsActive == true).ToList();
            var arrIdUserApprove = Approvers.Select(x => x.UserId).ToArray();
            var arrIdGroupRole = Approvers.Select(x => x.RoleId).ToArray();
            var LstUser = _db.AspNetUsers.Where(x => arrIdUserApprove.Contains(x.Id)).ToList();
            var LstGroupRole = _db.ApplicationGroups.Where(x => arrIdGroupRole.Contains(x.Id)).ToList();

            if (arrIdGroupRole.Length == 0 || arrIdUserApprove.Length == 0)
            {
                return new List<UserApproval>();
                //if(string.IsNullOrEmpty(arrIdGroupRole[0]) || string.IsNullOrEmpty(arrIdUserApprove[0])) return new List<UserApproval>();
            }

            if (NCR.STATUS.Trim() != StatusInDB.WaitingForDispositionApproval & NCR.STATUS.Trim() != StatusInDB.DispositionApproved & NCR.STATUS.Trim() != StatusInDB.Close)
            {
                var DataApprovers = from a in Approvers
                                    select new UserApproval
                                    {
                                        Id = a.ID.ToString(),
                                        IdUser = a.UserId,
                                        FullName = LstUser.FirstOrDefault(x => x.Id.Equals(a.UserId)).FullName,
                                        IsAppr = false,
                                        RoleId = a.RoleId,
                                        RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(a.RoleId)).Name,
                                        Signature = LstUser.FirstOrDefault(x => x.Id.Equals(a.UserId)).Signature
                                    };
                return DataApprovers.ToList();
            }

            if (IsAQL)
            {
                var ApproverIDsForAQL = UserApprove.Select(x => x.UserId);
                var ApproverIDUniqueForAQL = new HashSet<string>(ApproverIDsForAQL).ToArray();
                foreach (var approverForAQL in Approvers)
                {
                    var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverForAQL.UserId) & x.IsActive == true).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                    var restmp = new UserApproval
                    {
                        Id = approverForAQL.ID.ToString(),
                        FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverForAQL.UserId)).FullName,
                        IdUser = approverForAQL.UserId,
                        RoleId = approverForAQL.RoleId,
                        RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverForAQL.RoleId)).Name,
                        Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverForAQL.UserId)).Signature,
                        IsAppr = approveTMP != null,
                        DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                    };
                    Result.Add(restmp);
                }

                return Result;
            }
            else
            {
                var DETsNULL = DETs.Where(x => x.DATEAPPROVAL == null).ToList();
                if (DETsNULL.Count <= 0)
                {
                    var ApproverIDsFor100Approval = UserApprove.Select(x => x.UserId);
                    var ApproverIDUniqueFor100Approval = new HashSet<string>(ApproverIDsFor100Approval).ToArray();
                    foreach (var approverFor100Approval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100Approval.UserId) & x.IsActive == true).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100Approval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).FullName,
                            IdUser = approverFor100Approval.UserId,
                            RoleId = approverFor100Approval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).Signature,
                            IsAppr = true,

                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }

                    return Result;
                }
                // if exsit NCR_DET => not done disposition
                // get approver not approve.
                // Comparse with NCR_DET non DIS OR have DIS
                var ApproverIDsFor100NonApproval = UserApprove.Select(x => x.UserId).ToList();
                var ApproverIDUniqueFor100NonApproval = new HashSet<string>(ApproverIDsFor100NonApproval).ToArray().ToList();

                //get det has additional
                //if not exist get any det to get approval
                var DET_DIS = DETsNULL
                    .Join(DISs, det => det.ITEM.Trim(), dis => dis.ITEM.Trim(), (det, dis) => new { det, dis })
                    .Where(x => x.dis.DATEAPPROVAL == null)
                    .Select(x => x.dis).ToList();

                var DETNONDIS = DETsNULL
                    .GroupJoin(DISs,
                                  n => n.ITEM.Trim(),
                                  m => m.ITEM.Trim(),
                                  (n, ms) => new { n, ms = ms.DefaultIfEmpty() })
                                  // .Where(x=>x.n.DISPOSITION != "H")
                                  .Select(x => x.n).ToList();

                if (DET_DIS.Count > 0)
                {
                    var disTMP = DET_DIS[0];
                    foreach (var approverFor100NonApproval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100NonApproval.UserId) & x.IsActive == true & x.NCR_DIS_ID == disTMP.Id).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100NonApproval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).FullName,
                            IdUser = approverFor100NonApproval.UserId,
                            RoleId = approverFor100NonApproval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).Signature,
                            IsAppr = approveTMP != null,
                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }
                    return Result;
                }
                else
                {
                   // foreach (var item in DETNONDIS)
                  //  {
                     //   if (item.DISPOSITION == "H")
                     //   {
                     //       var ApproverIDsFor100Approval = UserApprove.Select(x => x.UserId);
                    //        var ApproverIDUniqueFor100Approval = new HashSet<string>(ApproverIDsFor100Approval).ToArray();
                    //        foreach (var approverFor100Approval in Approvers)
                     //       {
                     //           var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100Approval.UserId) & x.IsActive == true).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        //        var restmp = new UserApproval
                        //        {
                        //            Id = approverFor100Approval.ID.ToString(),
                        //            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).FullName,
                        //            IdUser = approverFor100Approval.UserId,
                        //            RoleId = approverFor100Approval.RoleId,
                        //            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.RoleId)).Name,
                        //            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).Signature,
                        //            IsAppr = true,

                        //            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        //        };
                        //        Result.Add(restmp);
                        //    }
                        //    return Result;
                        //}
                        //else
                        //{
                            
                            var DETTMP = DETNONDIS[0];
                            foreach (var approverFor100NonApproval in Approvers)
                            {
                                var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100NonApproval.UserId) & x.IsActive == true & x.DET_Item.Trim() == DETTMP.ITEM).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                                var restmp = new UserApproval
                                {
                                    Id = approverFor100NonApproval.ID.ToString(),
                                    FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).FullName,
                                    IdUser = approverFor100NonApproval.UserId,
                                    RoleId = approverFor100NonApproval.RoleId,
                                    RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.RoleId)).Name,
                                    Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).Signature,
                                    IsAppr = approveTMP != null,
                                    DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                                };
                                Result.Add(restmp);
                            }
                            return Result;
                    //    }

                    //}

                }
               // return Result;
            }
        }
    
        //tuan lua add
        public List<UserApproval> GetApproverOfNCRForConfirmHistory(string nCR_NUM,string CRno)
        {
            List<UserApproval> Result = new List<UserApproval>();
            var NCR = _db.NCR_HDR_History.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCR_NUM.Trim()) & x.CRNO.Trim() == CRno.Trim());
            var REJ = NCR.REJ_QTY;
            var DEF = NCR.DEFECTIVE;
            bool IsAQL = NCR.SAMPLE_INSP == true;

            var DETs = _db.NCR_DET_History.Where(x => x.NCR_NUM.Trim().Equals(nCR_NUM.Trim()) && x.CRNO.Trim() ==CRno.Trim()).ToList();
            var DISs = _db.NCR_DIS_History.Where(x => x.NCR_NUM.Trim().Equals(nCR_NUM.Trim()) && x.CRNO.Trim() == CRno.Trim()).ToList();
            var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCR_NUM.Trim()) & x.isActive == false ).ToList();
            var ApprovalEng = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCR_NUM.Trim()) & x.RoleId == "de752bd5-c496-4ea1-a479-d74d943cbcbe" & x.isActive==true).ToList();
            foreach (var item in ApprovalEng)
            {
                if (NCR.USERDISPO == item.UserId)
                {
                    Approvers.Add(item);
                }
            }
            var QTY = DISs.Sum(x => x.QTY);
            //var RoleIDChairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.CHAIRMAN).Id;
            //var UserIdChairman = Approvers.FirstOrDefault(x => x.RoleId == RoleIDChairman).UserId;
            var UserApprove = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCR_NUM.Trim()) & x.IsActive == false).ToList();
            var arrIdUserApprove = Approvers.Select(x => x.UserId).ToArray();
            var arrIdGroupRole = Approvers.Select(x => x.RoleId).ToArray();
            var LstUser = _db.AspNetUsers.Where(x => arrIdUserApprove.Contains(x.Id)).ToList();
            var LstGroupRole = _db.ApplicationGroups.Where(x => arrIdGroupRole.Contains(x.Id)).ToList();

            if (arrIdGroupRole.Length == 0 || arrIdUserApprove.Length == 0)
            {
                return new List<UserApproval>();
                //if(string.IsNullOrEmpty(arrIdGroupRole[0]) || string.IsNullOrEmpty(arrIdUserApprove[0])) return new List<UserApproval>();
            }

            if (NCR.STATUS.Trim() != StatusInDB.WaitingForDispositionApproval & NCR.STATUS.Trim() != StatusInDB.DispositionApproved & NCR.STATUS.Trim() != StatusInDB.Close)
            {
                var DataApprovers = from a in Approvers
                                    select new UserApproval
                                    {
                                        Id = a.ID.ToString(),
                                        IdUser = a.UserId,
                                        FullName = LstUser.FirstOrDefault(x => x.Id.Equals(a.UserId)).FullName,
                                        IsAppr = false,
                                        RoleId = a.RoleId,
                                        RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(a.RoleId)).Name,
                                        Signature = LstUser.FirstOrDefault(x => x.Id.Equals(a.UserId)).Signature
                                    };
                return DataApprovers.ToList();
            }

            if (IsAQL)
            {
                var ApproverIDsForAQL = UserApprove.Select(x => x.UserId);
                var ApproverIDUniqueForAQL = new HashSet<string>(ApproverIDsForAQL).ToArray();
                foreach (var approverForAQL in Approvers)
                {
                    var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverForAQL.UserId) & x.IsActive == false).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                    var restmp = new UserApproval
                    {
                        Id = approverForAQL.ID.ToString(),
                        FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverForAQL.UserId)).FullName,
                        IdUser = approverForAQL.UserId,
                        RoleId = approverForAQL.RoleId,
                        RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverForAQL.RoleId)).Name,
                        Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverForAQL.UserId)).Signature,
                        IsAppr = approveTMP != null,
                        DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                    };
                    Result.Add(restmp);
                }

                return Result;
            }
            else
            {
                var DETsNULL = DETs.Where(x => x.DATEAPPROVAL == null).ToList();
                if (DETsNULL.Count <= 0)
                {
                    var ApproverIDsFor100Approval = UserApprove.Select(x => x.UserId);
                    var ApproverIDUniqueFor100Approval = new HashSet<string>(ApproverIDsFor100Approval).ToArray();
                    foreach (var approverFor100Approval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100Approval.UserId) & x.IsActive == false).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100Approval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).FullName,
                            IdUser = approverFor100Approval.UserId,
                            RoleId = approverFor100Approval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100Approval.UserId)).Signature,
                            IsAppr = true,

                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }

                    return Result;
                }
                // if exsit NCR_DET => not done disposition
                // get approver not approve.
                // Comparse with NCR_DET non DIS OR have DIS
                var ApproverIDsFor100NonApproval = UserApprove.Select(x => x.UserId).ToList();
                var ApproverIDUniqueFor100NonApproval = new HashSet<string>(ApproverIDsFor100NonApproval).ToArray().ToList();

                //get det has additional
                //if not exist get any det to get approval
                var DET_DIS = DETsNULL
                    .Join(DISs, det => det.ITEM.Trim(), dis => dis.ITEM.Trim(), (det, dis) => new { det, dis })
                    .Where(x => x.dis.DATEAPPROVAL == null)
                    .Select(x => x.dis).ToList();

                var DETNONDIS = DETsNULL
                    .GroupJoin(DISs,
                                  n => n.ITEM.Trim(),
                                  m => m.ITEM.Trim(),
                                  (n, ms) => new { n, ms = ms.DefaultIfEmpty() })
                                  .Select(x => x.n).ToList();


                if (DET_DIS.Count > 0)
                {
                    var disTMP = DET_DIS[0];
                    foreach (var approverFor100NonApproval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100NonApproval.UserId) & x.IsActive == false & x.NCR_DIS_ID == disTMP.Id).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100NonApproval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).FullName,
                            IdUser = approverFor100NonApproval.UserId,
                            RoleId = approverFor100NonApproval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).Signature,
                            IsAppr = approveTMP != null,
                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }
                    return Result;
                }
                else
                {
                    var DETTMP = DETNONDIS[0];
                    foreach (var approverFor100NonApproval in Approvers)
                    {
                        var approveTMP = UserApprove.Where(x => x.UserId.Equals(approverFor100NonApproval.UserId) & x.IsActive == false & x.DET_Item.Trim() == DETTMP.ITEM).OrderByDescending(x => x.DateApprove).FirstOrDefault();
                        var restmp = new UserApproval
                        {
                            Id = approverFor100NonApproval.ID.ToString(),
                            FullName = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).FullName,
                            IdUser = approverFor100NonApproval.UserId,
                            RoleId = approverFor100NonApproval.RoleId,
                            RoleName = LstGroupRole.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.RoleId)).Name,
                            Signature = LstUser.FirstOrDefault(x => x.Id.Equals(approverFor100NonApproval.UserId)).Signature,
                            IsAppr = approveTMP != null,
                            DateAppr = approveTMP != null ? approveTMP.DateApprove.Value.GetDateTimeFormat() : ""
                        };
                        Result.Add(restmp);
                    }
                    return Result;
                }
            }
        }

        public Result SubmitConfirmNCR(string nCR_NUM, List<APPROVAL> aPPROVALs, List<string> roleIDs, string uidConfirm)
        {
            var _log = new LogWriter("SubmitConfirmNCR");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {

                    var NCR = _db.NCR_HDR.FirstOrDefault(x=> x.NCR_NUM.Equals(nCR_NUM));
                    NCR.STATUS = StatusInDB.WaitingForDisposition;
                    NCR.UserConfirm = uidConfirm;
                    NCR.ConfirmDate = DateTime.Now;

                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change Status",
                        NCRNUM = nCR_NUM,
                        Status = StatusInDB.WaitingForDisposition,
                        UserId = uidConfirm,
                        CreateDate = DateTime.Now,
                        IsActive = false
                    });

                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        message = "Confirm successfuly"
                    };
                }
                catch(Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString() + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    if (ex is DbEntityValidationException)
                    {
                        var e = (DbEntityValidationException)ex;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                    }
                    return new Result
                    {
                        success = false,
                        message = "Exception SubmitConfirmNCR"
                    };
                }
            }
        }

        public Result SubmitDispositionNCR(NCRManagementViewModel data, string UserID, List<OrderDisposition> orderDispositions, List<OrderDisposition> orderDispositionsADDIN)
        {
            var _log = new LogWriter("SubmitDispositionNCR");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    _log.LogWrite("SubmitConfirmNCR: Get NCR");
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(data.NCR_NUM));
                    NCR.NOT_REQUIRED = data.NOT_REQUIRED.HasValue ? data.NOT_REQUIRED : false;
                    NCR.REQUIRED = data.REQUIRED.HasValue ? data.REQUIRED : false;
                    NCR.NOTIFICATION_ONLY = data.NOTIFICATION_ONLY.HasValue ? data.NOTIFICATION_ONLY : false;
                    NCR.ISSUED_REQUEST_NO = data.ISSUED_REQUEST_NO;
                    NCR.ISSUED_REQUEST_DATE = null;
                    NCR.ISSUE_MEMO_DATE = null;
                    NCR.REMOVED_FROM = data.REMOVED_FROM;
                    NCR.BOOK_INV = data.BOOK_INV;
                    //if (data.REQUIRED == true)
                    //{
                    //    NCR.ISSUE_MEMO_DATE = DateTime.Now;
                    //    NCR.ISSUED_REQUEST_DATE = DateTime.Now;
                    //}
                    NCR.ISSUE_MEMO_NO = data.ISSUE_MEMO_NO;
                    NCR.NOTES = data.NOTES;
                    NCR.SHIPPING_METHOD = data.SHIPPING_METHOD;
                    NCR.RETURN_NUMBER = data.RETURN_NUMBER;

                    _db.Entry(NCR).State = EntityState.Modified;
                    _log.LogWrite("SubmitConfirmNCR Set DET");
                    foreach (var tmp in data.NCRDETs)
                    {
                        NCR_DET det = _db.NCR_DET.FirstOrDefault(o => o.NCR_NUM.Trim() == data.NCR_NUM.Trim() && o.ITEM.Trim() == tmp.ITEM.Trim());
                        if (det != null)
                        {
                            det.RESPONSE = tmp.RESPONSE;
                            det.DISPOSITION = tmp.DISPOSITION;
                            _db.Entry(det).State = EntityState.Modified;
                        }
                    }
                    _log.LogWrite("SubmitConfirmNCR Set DIS");
                    List<NCR_DIS> nCR_DISs = new List<NCR_DIS>();
                    if (data.NCRDISs != null)
                    {
                        foreach (var tmp in data.NCRDISs)
                        {
                            var Item = Convert.ToInt32(tmp.ITEM) - 1;
                            var NCR_DIS = new NCR_DIS
                            {
                                Id = Guid.NewGuid().ToString(),
                                ITEM = Item.ToString(),
                                ADD_INS = tmp.ADD_INS,
                                INSPECTOR = tmp.INSPECTOR,
                                NCR_NUM = NCR.NCR_NUM,
                                QTY = tmp.QTY,
                                REMARK = tmp.REMARK,
                                SEC = NCR.SEC,
                                REV = 0
                            };
                            nCR_DISs.Add(NCR_DIS);
                        }
                        _db.NCR_DIS.AddRange(nCR_DISs);
                    }
                    _log.LogWrite("SubmitConfirmNCR Set orderDispositions");
                    if (orderDispositions != null)
                        if(orderDispositions.Count > 0)
                            _db.OrderDispositions.AddRange(orderDispositions);
                    _log.LogWrite("SubmitConfirmNCR Set orderDispositions");
                    if (orderDispositionsADDIN != null)
                        if (orderDispositionsADDIN.Count > 0)
                            _db.OrderDispositions.AddRange(orderDispositionsADDIN);

                    _log.LogWrite("SubmitConfirmNCR Set UserApprove");

                    var ApproversDel = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(NCR.NCR_NUM) & !data.RoleIDs.Contains(x.ID.ToString())).ToList();
                    _db.APPROVALs.RemoveRange(ApproversDel);
                    _db.SaveChanges();

                    if (data.UserApprove != null)
                    {
                        foreach (var item in data.UserApprove)
                        {
                            if (string.IsNullOrEmpty(item.UserId) || string.IsNullOrEmpty(item.RoleId))
                                return new Result
                                {
                                    success = false,
                                    message = "Disposition unsuccessful: Approver is null",
                                    obj = ""
                                };

                            _db.APPROVALs.Add(new APPROVAL
                            {
                                Comment = "",
                                CreateDate = DateTime.Now,
                                isActive = true,
                                RoleId = item.RoleId,
                                UserId = item.UserId,
                                NCR_NUMBER = NCR.NCR_NUM.Trim()
                            });
                        }
                    }
                    _log.LogWrite("SubmitConfirmNCR Set NCR");
                    NCR.STATUS = StatusInDB.WaitingForDispositionApproval;
                    NCR.USERDISPO = UserID;
                    NCR.DATEDISPO = DateTime.Now;
                    _db.SaveChanges();
                    tranj.Commit();

                    return new Result
                    {
                        success = true,
                        message = "Disposition successfuly"
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString() + ex.InnerException.Message);
                    return new Result
                    {
                        success = false,
                        message = "Exception: " + ex.InnerException.Message,
                    };
                }
            }
        }

        public List<NCR_DISViewModel> GetNCRDISs(string nCR_NUM)
        {
            var NCR_DISs = (from diss in _db.NCR_DIS
                           where diss.NCR_NUM.Trim().Equals(nCR_NUM.Trim())
                           select new NCR_DISViewModel
                           {
                               Id = diss.Id,
                               ADD_INS = diss.ADD_INS,
                               DATEAPPROVAL = diss.DATEAPPROVAL,
                               INSPECTOR = diss.INSPECTOR,
                               ITEM = diss.ITEM,
                               QTY = diss.QTY,
                               REMARK = diss.REMARK
                           }).ToList();
            for (int i = 0; i < NCR_DISs.Count; i++)
            {
                NCR_DISs[i].ADD_INS = GetAddInsNameByID(NCR_DISs[i].ADD_INS);
                NCR_DISs[i].INSPECTOR = GetUserFullNameByID(NCR_DISs[i].INSPECTOR);
            }
            return NCR_DISs;
        }
        public List<NCR_DISViewModel> GetNCRDISsHistory(string nCR_NUM,string CRno)
        {
            var NCR_DISs = (from diss in _db.NCR_DIS_History
                            where diss.NCR_NUM.Trim().Equals(nCR_NUM.Trim() ) && (diss.CRNO.Trim()==CRno.Trim())
                            select new NCR_DISViewModel
                            {
                                Id = diss.Id,
                                ADD_INS = diss.ADD_INS,
                                DATEAPPROVAL = diss.DATEAPPROVAL,
                                INSPECTOR = diss.INSPECTOR,
                                ITEM = diss.ITEM,
                                QTY = diss.QTY,
                                REMARK = diss.REMARK
                            }).ToList();
            for (int i = 0; i < NCR_DISs.Count; i++)
            {
                NCR_DISs[i].ADD_INS = GetAddInsNameByID(NCR_DISs[i].ADD_INS);
                NCR_DISs[i].INSPECTOR = GetUserFullNameByID(NCR_DISs[i].INSPECTOR);
            }

            return NCR_DISs;
        }

        public List<DISPOSITIONViewModel> getOrderDisposition(string nCR_NUM)
        {
            var arr = new string[] {"DET", "DIS" };
            var data = from n in _db.OrderDispositions
                       where n.NCR_NUMBER.Trim().Equals(nCR_NUM.Trim()) & arr.Contains(n.Type.Trim()) & n.IsActive == true
                       select new DISPOSITIONViewModel
                       {
                           Item = n.ITEM,
                           Disposition = n.TypeOfDisposition == "USEASIS" ? "USE AS IS" : n.TypeOfDisposition,
                           Message = n.COST,
                           FileAttachName = n.FileName,
                           Type = n.Type
                       };
            return data.ToList();
        }

        public Result AssignNCR(string id, string nCRNUM, string approverId, string reason, string UserIdLogin)
        {
            var _log = new LogWriter("AssignNCR");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x=>x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
                    if(NCR == null)
                        return new Result
                        {
                            success = false,
                            message = $"NCR {nCRNUM} is not exist "
                        };

                    int ID = int.Parse(id);
                    var Approver = _db.APPROVALs.FirstOrDefault(x=>x.ID.Equals(ID));
                    if (Approver == null)
                        return new Result
                        {
                            success = false,
                            message = $"Approver {nCRNUM} is not exist "
                        };

                    Approver.isActive = false;
                    Approver.Comment = reason;


                    var lstApproved = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCRNUM.Trim()) & x.UserId.Equals(Approver.UserId) & x.IsActive == true).ToList();
                    foreach (var approved in lstApproved)
                    {
                        _db.UserDispositionApprovals.Add(new UserDispositionApproval
                        {
                            Id = Guid.NewGuid().ToString(),
                            DET_Item = approved.DET_Item,
                            NCR_DIS_ID = approved.NCR_DIS_ID,
                            Comment = "",
                            DateApprove = DateTime.Now,
                            NCRNUM = nCRNUM,
                            IsActive = true,
                            UserId = approverId,
                            ReAssignUserId = "",
                            NCR_STATUS = "d"
                        });

                        approved.IsActive = false;
                    }

                    _db.APPROVALs.Add(new APPROVAL
                    {
                        isActive =true,
                        RoleId = Approver.RoleId,
                        UserId = approverId,
                        CreateDate = DateTime.Now,
                        NCR_NUMBER = Approver.NCR_NUMBER
                    });

                    //NCR.STATUS = StatusInDB.Created;
                    //_db.NCR_History.Add(new NCR_History
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    Action = "Change Status",
                    //    NCRNUM = nCRNUM,
                    //    Status = StatusInDB.Created,
                    //    UserId = UserIdLogin,
                    //    CreateDate = DateTime.Now,
                    //    IsActive = false
                    //});

                    _db.UserDispositionApprovals.Add(new UserDispositionApproval
                    {
                        Id = Guid.NewGuid().ToString(),
                        Comment = reason,
                        DateApprove = DateTime.Now,
                        NCRNUM = nCRNUM,
                        IsActive =false,
                        UserId = Approver.UserId,
                        ReAssignUserId = approverId,
                        NCR_STATUS = "d"
                    });

                    //var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
                    //foreach (var approver in Approvers)
                    //{
                    //    approver.isActive = false;
                    //}

                    //var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();
                    //var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();

                    //_db.NCR_DIS.RemoveRange(DISs);
                    //_db.NCR_DET.RemoveRange(DETs);

                    _db.SaveChanges();
                    tranj.Commit();

                    return new Result
                    {
                        success = true
                    };
                }
                catch(Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    _log.LogWrite(ex.InnerException != null ? ex.InnerException.Message : "");
                    return new Result
                    {
                        success = false,
                        message = "Exception: " + ex.InnerException != null ? ex.InnerException.Message: ""
                    };
                }
            }
        }
        public Result AssignNCREng(string id, string nCRNUM, string approverId, string reason, string UserIdLogin,string roleid)
        {
            var _log = new LogWriter("AssignNCR");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
                    if (NCR == null)
                        return new Result
                        {
                            success = false,
                            message = $"NCR {nCRNUM} is not exist "
                        };

                    int ID = int.Parse(id);
                    var Approver = _db.APPROVALs.FirstOrDefault(x => x.ID.Equals(ID));
                    if (Approver == null)
                        return new Result
                        {
                            success = false,
                            message = $"Approver {nCRNUM} is not exist "
                        };

                    Approver.isActive = false;
                    Approver.Comment = reason;


                    var lstApproved = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCRNUM.Trim()) & x.UserId.Equals(Approver.UserId) & x.IsActive == true).ToList();
                    foreach (var approved in lstApproved)
                    {
                        _db.UserDispositionApprovals.Add(new UserDispositionApproval
                        {
                            Id = Guid.NewGuid().ToString(),
                            DET_Item = approved.DET_Item,
                            NCR_DIS_ID = approved.NCR_DIS_ID,
                            Comment = "",
                            DateApprove = DateTime.Now,
                            NCRNUM = nCRNUM,
                            IsActive = true,
                            UserId = approverId,
                            ReAssignUserId = "",
                            NCR_STATUS ="c"
                        });

                        approved.IsActive = false;
                    }

                    _db.APPROVALs.Add(new APPROVAL
                    {
                        isActive = true,
                        RoleId = roleid,
                        UserId = approverId,
                        CreateDate = DateTime.Now,
                        NCR_NUMBER = Approver.NCR_NUMBER
                    });

                    //NCR.STATUS = StatusInDB.Created;
                    //_db.NCR_History.Add(new NCR_History
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    Action = "Change Status",
                    //    NCRNUM = nCRNUM,
                    //    Status = StatusInDB.Created,
                    //    UserId = UserIdLogin,
                    //    CreateDate = DateTime.Now,
                    //    IsActive = false
                    //});

                    _db.UserDispositionApprovals.Add(new UserDispositionApproval
                    {
                        Id = Guid.NewGuid().ToString(),
                        Comment = reason,
                        DateApprove = DateTime.Now,
                        NCRNUM = nCRNUM,
                        IsActive = false,
                        UserId = Approver.UserId,
                        ReAssignUserId = approverId,
                        NCR_STATUS = "c"
                    });

                    //var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
                    //foreach (var approver in Approvers)
                    //{
                    //    approver.isActive = false;
                    //}

                    //var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();
                    //var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();

                    //_db.NCR_DIS.RemoveRange(DISs);
                    //_db.NCR_DET.RemoveRange(DETs);

                    _db.SaveChanges();
                    tranj.Commit();

                    return new Result
                    {
                        success = true
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    _log.LogWrite(ex.InnerException != null ? ex.InnerException.Message : "");
                    return new Result
                    {
                        success = false,
                        message = "Exception: " + ex.InnerException != null ? ex.InnerException.Message : ""
                    };
                }
            }
        }


        public Result AddinsAddIns(string nCRNUM, string item, double qTY, string aDDINS, string remark, string iNSP, OrderDisposition orderDisposition)
        {
            var _log = new LogWriter("AddinsAddIns");
            var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM));
            var checkApp = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM) & x.DATEAPPROVAL == null).ToList();
            var arrcheckApp = checkApp.Select(x => x.Id).ToArray();
            var UserApprove = _db.UserDispositionApprovals.Where(x => arrcheckApp.Contains(x.NCR_DIS_ID)).ToList();
            string itemtmp = (int.Parse(item) - 1).ToString();
            var DET = _db.NCR_DET.FirstOrDefault(x=>x.NCR_NUM.Equals(nCRNUM.Trim()) & x.ITEM.Equals(itemtmp));
            var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Equals(nCRNUM.Trim()) & x.ITEM.Trim().Equals(itemtmp)).ToList();
            if(DET.QTY == DISs.Sum(x => x.QTY))
            {
                return new Result
                {
                    success = false,
                    message = "",
                    obj = "Please check qty!!!"
                };
            }
            if (checkApp.Count > 0 & UserApprove.Count > 0) return new Result
            {
                success = false,
                message = "",
                obj = ""
            };

            if(NCR.SAMPLE_INSP == true)
            {
                var QTYTMP = qTY + DISs.Sum(x => x.QTY);
                if(NCR.REJ_QTY < QTYTMP) return new Result
                {
                    success = false,
                    message = "",
                    obj = ""
                };
            }
            else
            {
                var QTYTMP = qTY + DISs.Sum(x => x.QTY);
                if (NCR.DEFECTIVE < QTYTMP) return new Result
                {
                    success = false,
                    message = "",
                    obj = "Please check qty!!"
                };
            }

            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.NCR_DIS.Add(new NCR_DIS
                    {
                        Id = Guid.NewGuid().ToString(),
                        ADD_INS = aDDINS.Trim(),
                        NCR_NUM = nCRNUM.Trim(),
                        INSPECTOR = iNSP,
                        QTY = qTY,
                        ITEM = itemtmp,
                        REMARK = !string.IsNullOrEmpty(remark) ? remark : "",
                        SEC = "PROCESS"
                    });

                    if(orderDisposition != null)
                    {
                        _db.OrderDispositions.Add(orderDisposition);
                    }

                    _db.SaveChanges();
                    tran.Commit();
                    return new Result
                    {
                        success = true,
                        message = "",
                        obj = ""
                    };
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _log.LogWrite("AddinsAddIns: " + ex.ToString());
                    _log.LogWrite("AddinsAddIns Inner: " + ex.InnerException != null ? ex.InnerException.Message : "");
                    return new Result
                    {
                        success = false,
                        message = "",
                        obj = "Please check your input information!"
                    };
                }
            }
        }

        public Result RejectNCR(string id, string nCRNUM, string approverId, string reason, string UserIdLogin)
        {
            var _log = new LogWriter("RejectNCR");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var checkSubmitChange = _db.ChangeItems.Where(x => x.RefNumber.Trim() == nCRNUM.Trim()).FirstOrDefault();
                    if (checkSubmitChange != null)
                    {
                        var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
                        List<NCR_DET> DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM)).ToList();
                        List<NCR_DIS> DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM)).ToList();
                        List<UserDispositionApproval> userDispositionApprovals = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCRNUM)).ToList();
                        List<OrderDisposition> orderDisposition = _db.OrderDispositions.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM)).ToList();
                        if (NCR == null)
                            return new Result
                            {
                                success = false,
                                message = $"NCR {nCRNUM} is not exist "
                            };

                        if (NCR.PERCENT_INSP == true)
                        {
                            // int cAppr = _db.APPROVALs.Count(x => x.NCR_NUMBER.Equals(nCRNUM) & x.isActive == true);
                            var ckAppr = _db.NCR_DIS.Where(x => x.NCR_NUM.Equals(nCRNUM.Trim())).ToList();
                            foreach (var item in ckAppr)
                            {
                                if (item.DATEAPPROVAL != null)
                                {
                                    return new Result
                                    {
                                        success = false,
                                        message = $"NCR {nCRNUM} was partial approval, don't reject !!!"
                                    };
                                }
                            }

                        }
                        if (DETs.Count > 0)
                        {
                            //add det history
                            List<NCR_DET_History> lstdet = new List<NCR_DET_History>();
                            foreach (NCR_DET item in DETs)
                            {
                                NCR_DET_History historydet = new NCR_DET_History
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    NCR_NUM = item.NCR_NUM,
                                    DEFECT = item.DEFECT,
                                    DISPOSITION = item.DISPOSITION,
                                    DATEAPPROVAL = item.DATEAPPROVAL,
                                    QTY = item.QTY,
                                    SEC = item.SEC,
                                    NC_DESC = item.NC_DESC,
                                    RESPONSE = item.RESPONSE,
                                    REMARK = item.REMARK,
                                    ITEM = item.ITEM,
                                    CRNO="",
                                    CreateDate = DateTime.Now
                                };
                                lstdet.Add(historydet);
                                ///TODO: Reset ncr det: set null data such as: dateApproval, response, disposition
                                item.DATEAPPROVAL = null;
                                item.RESPONSE = null;
                                item.DISPOSITION = null;
                                _db.NCR_DET.Attach(item);
                                _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            };
                            _db.NCR_DET_History.AddRange(lstdet);


                            ///TODO: set isActive = false into OrderDisposition for NCR_NUM

                            foreach (OrderDisposition item in orderDisposition)
                            {
                                item.IsActive = false;
                            }

                        }

                        if (DISs.Count > 0)
                        {
                            List<NCR_DIS_History> lstdis = new List<NCR_DIS_History>();
                            foreach (NCR_DIS item in DISs)
                            {
                                NCR_DIS_History historydis = new NCR_DIS_History
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    NCR_NUM = item.NCR_NUM,
                                    ADD_INS = item.ADD_INS,
                                    INS_DATE = item.INS_DATE,
                                    ITEM = item.ITEM,
                                    QTY = item.QTY,
                                    REMARK = item.REMARK,
                                    SEC = item.SEC,
                                    INSPECTOR = item.INSPECTOR,
                                    DATEAPPROVAL = item.DATEAPPROVAL,
                                    CRNO="",
                                    CreateDate = DateTime.Now
                                };
                                lstdis.Add(historydis);
                            };
                            _db.NCR_DIS_History.AddRange(lstdis);
                            _db.NCR_DIS.RemoveRange(DISs);
                        }
                        ///TODO: change isActive = false into UserDispositionApproval for NCR_NUM
                        foreach (UserDispositionApproval item in userDispositionApprovals)
                        {
                            item.IsActive = false;
                            item.NCR_STATUS = "d";
                        }


                        //copy ncrhdr
                        NCR_HDR_History history = new NCR_HDR_History
                        {
                            ITEM_DESC = NCR.ITEM_DESC,
                            NCR_NUM = NCR.NCR_NUM,
                            AQL = NCR.AQL,
                            BOOK_INV = NCR.BOOK_INV,
                            CCN = NCR.CCN,
                            CITY = NCR.CITY,
                            CRNO="",
                            CORRECT_ACTION = NCR.CORRECT_ACTION,
                            Comment = NCR.Comment,
                            DateApproval = NCR.DateApproval,
                            DATEDISPO = NCR.DATEDISPO,
                            DRAW_REV = NCR.DRAW_REV,
                            DATESUBMIT = NCR.DATESUBMIT,
                            EN_PIC = NCR.EN_PIC,
                            FAI = NCR.FAI,
                            FIRST_ARTICLE = NCR.FIRST_ARTICLE,
                            FOLLOW_UP_NOTES = NCR.FOLLOW_UP_NOTES,
                            INSPECTOR = NCR.INSPECTOR,
                            INS_DATE = NCR.INS_DATE,
                            INS_PLAN = NCR.INS_PLAN,
                            INS_QTY = NCR.INS_QTY,
                            ISSUED_REQUEST_DATE = NCR.ISSUED_REQUEST_DATE,
                            ISSUED_REQUEST_NO = NCR.ISSUED_REQUEST_NO,
                            ISSUE_MEMO_DATE = NCR.ISSUE_MEMO_DATE,
                            ISSUE_MEMO_NO = NCR.ISSUE_MEMO_NO,
                            LEVEL = NCR.LEVEL,
                            LOT = NCR.LOT,
                            MFG_DATE = NCR.MFG_DATE,
                            MFG_PIC = NCR.MFG_PIC,
                            MI_PART_NO = NCR.MI_PART_NO,
                            MODEL_NO = NCR.MODEL_NO,
                            NOTES = NCR.NOTES,
                            NOTIFICATION_ONLY = NCR.NOTIFICATION_ONLY,
                            NOT_REQUIRED = NCR.NOT_REQUIRED,
                            PERCENT_INSP = NCR.PERCENT_INSP,
                            PO_NUM = NCR.PO_NUM,
                            PUR_DATE = NCR.PUR_DATE,
                            PUR_PIC = NCR.PUR_PIC,
                            QA_DATE = NCR.QA_DATE,
                            QA_PIC = NCR.QA_PIC,
                            RECEIVER = NCR.RECEIVER,
                            REC_QTY = NCR.REC_QTY,
                            REJ_QTY = NCR.REJ_QTY,
                            REMOVED_FROM = NCR.REMOVED_FROM,
                            REQUIRED = NCR.REQUIRED,
                            RETURN_NUMBER = NCR.RETURN_NUMBER,
                            SAMPLE_INSP = NCR.SAMPLE_INSP,
                            ZIP_CODE = NCR.ZIP_CODE,
                            SCAR_BY = NCR.SCAR_BY,
                            SCAR_DATE = NCR.SCAR_DATE,
                            SCAR_NUM = NCR.SCAR_NUM,
                            VEN_ADD = NCR.VEN_ADD,
                            SEC = NCR.SEC,
                            SHIPPING_METHOD = NCR.SHIPPING_METHOD,
                            SKIP_LOT_LEVEL = NCR.SKIP_LOT_LEVEL,
                            STATE = NCR.STATE,
                            STATUS = NCR.STATUS,
                            TYPE_NCR = NCR.TYPE_NCR,
                            USERDISPO = NCR.USERDISPO,
                            USERSUBMIT = NCR.USERSUBMIT,
                            VENDOR = NCR.VENDOR,
                            VEN_NAME = NCR.VEN_NAME,
                            Id = Guid.NewGuid().ToString(),
                            UserConfirm = NCR.UserConfirm,
                            ConfirmDate = NCR.ConfirmDate,
                            Amount = NCR.Amount,
                            DEFECTIVE = NCR.DEFECTIVE,
                            CreateDate = DateTime.Now
                        };
                        _db.NCR_HDR_History.Add(history);
                        //update user approval ngoai tru enginerring 13/02/2019 by tuanlua
                        // _db.ApplicationGroups.Where(x => x.Name.Trim() == "ENGINEERING").Select(x => x.Id).FirstOrDefault();
                        var ideng = NCR.USERDISPO;
                        var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
                        foreach (var approver in Approvers)
                        {
                            if (approver.UserId != ideng)
                            {
                                approver.isActive = false;
                            }
                        }
                        //update status NCRHDR = c
                        NCR.STATUS = StatusInDB.WaitingForDisposition;
                        NCR.REQUIRED = null;
                        NCR.NOT_REQUIRED = null;
                        NCR.NOTIFICATION_ONLY = null;
                 //       NCR.USERDISPO = null;
                        NCR.DATEDISPO = null;
                        NCR.DateApproval = null;
                        _db.Entry(NCR).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        tranj.Commit();
                        return new Result
                        {
                            success = true,
                        };
                    }
                    else
                    {
                        var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
                        if (NCR == null)
                            return new Result
                            {
                                success = false,
                                message = $"NCR {nCRNUM} is not exist "
                            };

                        if (NCR.PERCENT_INSP == true)
                        {
                            // int cAppr = _db.APPROVALs.Count(x => x.NCR_NUMBER.Equals(nCRNUM) & x.isActive == true);
                            var ckAppr = _db.NCR_DIS.Where(x => x.NCR_NUM.Equals(nCRNUM)).ToList();
                            foreach (var item in ckAppr)
                            {
                                if (item.DATEAPPROVAL != null)
                                {
                                    return new Result
                                    {
                                        success = false,
                                        message = $"NCR {nCRNUM} was partial approval, don't reject !!!"
                                    };
                                }
                            }

                        }

                        NCR.STATUS = StatusInDB.Created;
                        NCR.DateApproval = null;
                        NCR.REQUIRED = null;
                        NCR.NOT_REQUIRED = null;
                        NCR.NOTIFICATION_ONLY = null;
                        //   NCR.ConfirmDate = null;
                        //   NCR.UserConfirm = null;
                        //     NCR.DATESUBMIT = null;
                        //     NCR.USERSUBMIT = null;
                        int ID = int.Parse(id);
                        var Approver = _db.APPROVALs.FirstOrDefault(x => x.ID.Equals(ID));
                        if (Approver == null)
                            return new Result
                            {
                                success = false,
                                message = $"Approver {nCRNUM} is not exist "
                            };
                        Approver.Comment = reason;

                        _db.NCR_HDR_History.Add(new NCR_HDR_History
                        {
                            Amount = NCR.Amount,
                            AQL = NCR.AQL,
                            BOOK_INV = NCR.BOOK_INV,
                            CCN = NCR.CCN,
                            CITY = NCR.CITY,
                            Comment = NCR.Comment,
                            ConfirmDate = NCR.ConfirmDate,
                            CORRECT_ACTION = NCR.CORRECT_ACTION,
                            CRNO = "",
                            DateApproval = NCR.DateApproval,
                            DATEDISPO = NCR.DATEDISPO,
                            DATESUBMIT = NCR.DATESUBMIT,
                            DEFECTIVE = NCR.DEFECTIVE,
                            DRAW_REV = NCR.DRAW_REV,
                            EN_PIC = NCR.EN_PIC,
                            FAI = NCR.FAI,
                            FIRST_ARTICLE = NCR.FIRST_ARTICLE,
                            FOLLOW_UP_NOTES = NCR.FOLLOW_UP_NOTES,
                            INSPECTOR = NCR.INSPECTOR,
                            INS_DATE = NCR.INS_DATE,
                            INS_PLAN = NCR.INS_PLAN,
                            INS_QTY = NCR.INS_QTY,
                            ISSUED_REQUEST_DATE = NCR.ISSUED_REQUEST_DATE,
                            ISSUED_REQUEST_NO = NCR.ISSUED_REQUEST_NO,
                            ISSUE_MEMO_DATE = NCR.ISSUE_MEMO_DATE,
                            ISSUE_MEMO_NO = NCR.ISSUE_MEMO_NO,
                            ITEM_DESC = NCR.ITEM_DESC,
                            LEVEL = NCR.LEVEL,
                            LOT = NCR.LOT,
                            MFG_DATE = NCR.MFG_DATE,
                            MFG_PIC = NCR.MFG_PIC,
                            MI_PART_NO = NCR.MI_PART_NO,
                            MODEL_NO = NCR.MODEL_NO,
                            NCR_NUM = NCR.NCR_NUM,
                            NOTES = NCR.NOTES,
                            NOTIFICATION_ONLY = NCR.NOTIFICATION_ONLY,
                            NOT_REQUIRED = NCR.NOT_REQUIRED,
                            PERCENT_INSP = NCR.PERCENT_INSP,
                            PO_NUM = NCR.PO_NUM,
                            PUR_DATE = NCR.PUR_DATE,
                            PUR_PIC = NCR.PUR_PIC,
                            QA_DATE = NCR.QA_DATE,
                            QA_PIC = NCR.QA_PIC,
                            RECEIVER = NCR.RECEIVER,
                            REC_QTY = NCR.REC_QTY,
                            REJ_QTY = NCR.REJ_QTY,
                            REMOVED_FROM = NCR.REMOVED_FROM,
                            REQUIRED = NCR.REQUIRED,
                            RETURN_NUMBER = NCR.RETURN_NUMBER,
                            SAMPLE_INSP = NCR.SAMPLE_INSP,
                            SCAR_BY = NCR.SCAR_BY,
                            SCAR_DATE = NCR.SCAR_DATE,
                            SCAR_NUM = NCR.SCAR_NUM,
                            SEC = NCR.SEC,
                            SHIPPING_METHOD = NCR.SHIPPING_METHOD,
                            SKIP_LOT_LEVEL = NCR.SKIP_LOT_LEVEL,
                            STATE = NCR.STATE,
                            STATUS = NCR.STATUS,
                            TYPE_NCR = NCR.TYPE_NCR,
                            UserConfirm = NCR.UserConfirm,
                            USERDISPO = NCR.USERDISPO,
                            USERSUBMIT = NCR.USERSUBMIT,
                            VENDOR = NCR.VENDOR,
                            VEN_ADD = NCR.VEN_ADD,
                            VEN_NAME = NCR.VEN_NAME,
                            ZIP_CODE = NCR.ZIP_CODE,
                            Id = Guid.NewGuid().ToString(),
                            CreateDate = DateTime.Now
                        });

                        _db.NCR_History.Add(new NCR_History
                        {
                            Id = Guid.NewGuid().ToString(),
                            Action = "Change Status",
                            NCRNUM = nCRNUM,
                            Status = StatusInDB.Created,
                            UserId = UserIdLogin,
                            CreateDate = DateTime.Now,
                            IsActive = false
                        });

                        _db.UserDispositionApprovals.Add(new UserDispositionApproval
                        {
                            Id = Guid.NewGuid().ToString(),
                            Comment = reason,
                            DateApprove = DateTime.Now,
                            NCRNUM = nCRNUM,
                            IsActive = false,
                            UserId = approverId,
                            NCR_STATUS = "d"
                        });

                        var _Approv = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCRNUM.Trim())).ToList();
                        foreach (var _ap in _Approv)
                        {
                            _ap.IsActive = false;
                        }

                        var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
                        foreach (var approver in Approvers)
                        {
                            approver.isActive = false;
                        }

                        var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();
                        foreach (var det in DETs)
                        {
                            det.DISPOSITION = "";
                            det.RESPONSE = "";
                            det.REMARK = "";
                            det.DATEAPPROVAL = null;

                            _db.NCR_DET_History.Add(new NCR_DET_History
                            {
                                Id = Guid.NewGuid().ToString(),
                                CRNO = "",
                                DATEAPPROVAL = det.DATEAPPROVAL,
                                DEFECT = det.DEFECT,
                                DISPOSITION = det.DISPOSITION,
                                ITEM = det.ITEM,
                                NCR_NUM = det.NCR_NUM,
                                NC_DESC = det.NC_DESC,
                                QTY = det.QTY,
                                REMARK = det.REMARK,
                                RESPONSE = det.RESPONSE,
                                SEC = det.SEC,
                                 CreateDate = DateTime.Now
                            });
                        }

                        var Orders = _db.OrderDispositions.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim())).ToList();
                        foreach (var o in Orders)
                        {
                            o.IsActive = false;
                        }

                        var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();
                        foreach (var dis in DISs)
                        {
                            _db.NCR_DIS_History.Add(new NCR_DIS_History
                            {
                                Id = Guid.NewGuid().ToString(),
                                ADD_INS = dis.ADD_INS,
                                CRNO = "",
                                DATEAPPROVAL = dis.DATEAPPROVAL,
                                INSPECTOR = dis.INSPECTOR,
                                INS_DATE = dis.INS_DATE,
                                ITEM = dis.ITEM,
                                NCR_NUM = dis.NCR_NUM,
                                QTY = dis.QTY,
                                REMARK = dis.REMARK,
                                SEC = dis.SEC,
                                CreateDate = DateTime.Now
                            });
                        }

                        _db.NCR_DIS.RemoveRange(DISs);

                        _db.SaveChanges();
                        tranj.Commit();

                        return new Result
                        {
                            success = true
                        };
                    }
                   
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    _log.LogWrite(ex.InnerException != null ? ex.InnerException.Message : "");
                    return new Result
                    {
                        success = false,
                        message = "Exception: " + ex.InnerException != null ? ex.InnerException.Message : ""
                    };
                }
            }
        }
        public Result RejectNCRWH( string nCRNUM, string reason, string IDuser)
        {
            var _log = new LogWriter("RejectNCR");
            int id = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim() == nCRNUM.Trim() & x.isActive ==true).Select(x=>x.ID).FirstOrDefault();
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
                    if (NCR == null)
                        return new Result
                        {
                            success = false,
                            message = $"NCR {nCRNUM} is not exist "
                        };

                    if (NCR.PERCENT_INSP == true)
                    {
                        int cAppr = _db.APPROVALs.Count(x => x.NCR_NUMBER.Equals(nCRNUM) & x.isActive == true);
                        int ckAppr = _db.UserDispositionApprovals.Count(x => x.NCRNUM.Equals(nCRNUM) & x.IsActive == true);
                        if (ckAppr >= cAppr)
                            return new Result
                            {
                                success = false,
                                message = $"NCR {nCRNUM} was partial approval, don't reject !!!"
                            };
                    }

                    NCR.STATUS = StatusInDB.Created;
                    NCR.DateApproval = null;
                  //  NCR.ConfirmDate = null;
                 //   NCR.UserConfirm = null;
                 //   NCR.DATESUBMIT = null;
                 //   NCR.USERSUBMIT = null;
                    int ID = id;
                    var Approver = _db.APPROVALs.FirstOrDefault(x => x.ID.Equals(ID));
                    if (Approver == null)
                        return new Result
                        {
                            success = false,
                            message = $"Approver {nCRNUM} is not exist "
                        };
                    Approver.Comment = reason;

                    _db.NCR_HDR_History.Add(new NCR_HDR_History
                    {
                        Amount = NCR.Amount,
                        AQL = NCR.AQL,
                        BOOK_INV = NCR.BOOK_INV,
                        CCN = NCR.CCN,
                        CITY = NCR.CITY,
                        Comment = NCR.Comment,
                        ConfirmDate = NCR.ConfirmDate,
                        CORRECT_ACTION = NCR.CORRECT_ACTION,
                        CRNO = "",
                        DateApproval = NCR.DateApproval,
                        DATEDISPO = NCR.DATEDISPO,
                        DATESUBMIT = NCR.DATESUBMIT,
                        DEFECTIVE = NCR.DEFECTIVE,
                        DRAW_REV = NCR.DRAW_REV,
                        EN_PIC = NCR.EN_PIC,
                        FAI = NCR.FAI,
                        FIRST_ARTICLE = NCR.FIRST_ARTICLE,
                        FOLLOW_UP_NOTES = NCR.FOLLOW_UP_NOTES,
                        INSPECTOR = NCR.INSPECTOR,
                        INS_DATE = NCR.INS_DATE,
                        INS_PLAN = NCR.INS_PLAN,
                        INS_QTY = NCR.INS_QTY,
                        ISSUED_REQUEST_DATE = NCR.ISSUED_REQUEST_DATE,
                        ISSUED_REQUEST_NO = NCR.ISSUED_REQUEST_NO,
                        ISSUE_MEMO_DATE = NCR.ISSUE_MEMO_DATE,
                        ISSUE_MEMO_NO = NCR.ISSUE_MEMO_NO,
                        ITEM_DESC = NCR.ITEM_DESC,
                        LEVEL = NCR.LEVEL,
                        LOT = NCR.LOT,
                        MFG_DATE = NCR.MFG_DATE,
                        MFG_PIC = NCR.MFG_PIC,
                        MI_PART_NO = NCR.MI_PART_NO,
                        MODEL_NO = NCR.MODEL_NO,
                        NCR_NUM = NCR.NCR_NUM,
                        NOTES = NCR.NOTES,
                        NOTIFICATION_ONLY = NCR.NOTIFICATION_ONLY,
                        NOT_REQUIRED = NCR.NOT_REQUIRED,
                        PERCENT_INSP = NCR.PERCENT_INSP,
                        PO_NUM = NCR.PO_NUM,
                        PUR_DATE = NCR.PUR_DATE,
                        PUR_PIC = NCR.PUR_PIC,
                        QA_DATE = NCR.QA_DATE,
                        QA_PIC = NCR.QA_PIC,
                        RECEIVER = NCR.RECEIVER,
                        REC_QTY = NCR.REC_QTY,
                        REJ_QTY = NCR.REJ_QTY,
                        REMOVED_FROM = NCR.REMOVED_FROM,
                        REQUIRED = NCR.REQUIRED,
                        RETURN_NUMBER = NCR.RETURN_NUMBER,
                        SAMPLE_INSP = NCR.SAMPLE_INSP,
                        SCAR_BY = NCR.SCAR_BY,
                        SCAR_DATE = NCR.SCAR_DATE,
                        SCAR_NUM = NCR.SCAR_NUM,
                        SEC = NCR.SEC,
                        SHIPPING_METHOD = NCR.SHIPPING_METHOD,
                        SKIP_LOT_LEVEL = NCR.SKIP_LOT_LEVEL,
                        STATE = NCR.STATE,
                        STATUS = NCR.STATUS,
                        TYPE_NCR = NCR.TYPE_NCR,
                        UserConfirm = NCR.UserConfirm,
                        USERDISPO = NCR.USERDISPO,
                        USERSUBMIT = NCR.USERSUBMIT,
                        VENDOR = NCR.VENDOR,
                        VEN_ADD = NCR.VEN_ADD,
                        VEN_NAME = NCR.VEN_NAME,
                        ZIP_CODE = NCR.ZIP_CODE,
                        Id = Guid.NewGuid().ToString(),
                        CreateDate =DateTime.Now
                    });

                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change Status",
                        NCRNUM = nCRNUM,
                        Status = StatusInDB.Created,
                        UserId = IDuser,
                        CreateDate = DateTime.Now,
                        IsActive = false
                    });

                    _db.UserDispositionApprovals.Add(new UserDispositionApproval
                    {
                        Id = Guid.NewGuid().ToString(),
                        Comment = reason,
                        DateApprove = DateTime.Now,
                        NCRNUM = nCRNUM,
                        IsActive = false,
                        UserId = IDuser,
                        NCR_STATUS = "b"
                    });

                    var _Approv = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCRNUM.Trim())).ToList();
                    foreach (var _ap in _Approv)
                    {
                        _ap.IsActive = false;
                    }

                    var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
                    foreach (var approver in Approvers)
                    {
                        approver.isActive = false;
                    }
                    _db.SaveChanges();
                    tranj.Commit();

                    return new Result
                    {
                        success = true
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    _log.LogWrite(ex.InnerException != null ? ex.InnerException.Message : "");
                    return new Result
                    {
                        success = false,
                        message = "Exception: " + ex.InnerException != null ? ex.InnerException.Message : ""
                    };
                }
            }
        }
        public Result RejectNCREng(string id, string nCRNUM, string approverId, string reason, string UserIdLogin)
        {
            var _log = new LogWriter("RejectNCR");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {

                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
                    var checkSubmitChange = _db.ChangeItems.Where(x => x.RefNumber.Trim() == nCRNUM.Trim()).FirstOrDefault();
                    if (checkSubmitChange != null)
                    {
                        return new Result
                        {
                            success = false,
                            message = $"NCR {nCRNUM} was submit change item , don't reject !!!"
                        };
                    }
                        if (NCR == null)
                        return new Result
                        {
                            success = false,
                            message = $"NCR {nCRNUM} is not exist "
                        };

                    if (NCR.PERCENT_INSP == true)
                    {
                        int cAppr = _db.APPROVALs.Count(x => x.NCR_NUMBER.Equals(nCRNUM) & x.isActive == true);
                        int ckAppr = _db.UserDispositionApprovals.Count(x => x.NCRNUM.Equals(nCRNUM) & x.IsActive == true);
                        if (ckAppr >= cAppr)
                            return new Result
                            {
                                success = false,
                                message = $"NCR {nCRNUM} was partial approval, don't reject !!!"
                            };
                    }

                    NCR.STATUS = StatusInDB.Created;
                    NCR.DateApproval = null;
                    //   NCR.ConfirmDate = null;
                    //   NCR.UserConfirm = null;
                    //     NCR.DATESUBMIT = null;
                    //     NCR.USERSUBMIT = null;
                    int ID = int.Parse(id);
                    var Approver = _db.APPROVALs.FirstOrDefault(x => x.ID.Equals(ID));
                    if (Approver == null)
                        return new Result
                        {
                            success = false,
                            message = $"Approver {nCRNUM} is not exist "
                        };
                    Approver.Comment = reason;

                    _db.NCR_HDR_History.Add(new NCR_HDR_History
                    {
                        Amount = NCR.Amount,
                        AQL = NCR.AQL,
                        BOOK_INV = NCR.BOOK_INV,
                        CCN = NCR.CCN,
                        CITY = NCR.CITY,
                        Comment = NCR.Comment,
                        ConfirmDate = NCR.ConfirmDate,
                        CORRECT_ACTION = NCR.CORRECT_ACTION,
                        CRNO = "",
                        DateApproval = NCR.DateApproval,
                        DATEDISPO = NCR.DATEDISPO,
                        DATESUBMIT = NCR.DATESUBMIT,
                        DEFECTIVE = NCR.DEFECTIVE,
                        DRAW_REV = NCR.DRAW_REV,
                        EN_PIC = NCR.EN_PIC,
                        FAI = NCR.FAI,
                        FIRST_ARTICLE = NCR.FIRST_ARTICLE,
                        FOLLOW_UP_NOTES = NCR.FOLLOW_UP_NOTES,
                        INSPECTOR = NCR.INSPECTOR,
                        INS_DATE = NCR.INS_DATE,
                        INS_PLAN = NCR.INS_PLAN,
                        INS_QTY = NCR.INS_QTY,
                        ISSUED_REQUEST_DATE = NCR.ISSUED_REQUEST_DATE,
                        ISSUED_REQUEST_NO = NCR.ISSUED_REQUEST_NO,
                        ISSUE_MEMO_DATE = NCR.ISSUE_MEMO_DATE,
                        ISSUE_MEMO_NO = NCR.ISSUE_MEMO_NO,
                        ITEM_DESC = NCR.ITEM_DESC,
                        LEVEL = NCR.LEVEL,
                        LOT = NCR.LOT,
                        MFG_DATE = NCR.MFG_DATE,
                        MFG_PIC = NCR.MFG_PIC,
                        MI_PART_NO = NCR.MI_PART_NO,
                        MODEL_NO = NCR.MODEL_NO,
                        NCR_NUM = NCR.NCR_NUM,
                        NOTES = NCR.NOTES,
                        NOTIFICATION_ONLY = NCR.NOTIFICATION_ONLY,
                        NOT_REQUIRED = NCR.NOT_REQUIRED,
                        PERCENT_INSP = NCR.PERCENT_INSP,
                        PO_NUM = NCR.PO_NUM,
                        PUR_DATE = NCR.PUR_DATE,
                        PUR_PIC = NCR.PUR_PIC,
                        QA_DATE = NCR.QA_DATE,
                        QA_PIC = NCR.QA_PIC,
                        RECEIVER = NCR.RECEIVER,
                        REC_QTY = NCR.REC_QTY,
                        REJ_QTY = NCR.REJ_QTY,
                        REMOVED_FROM = NCR.REMOVED_FROM,
                        REQUIRED = NCR.REQUIRED,
                        RETURN_NUMBER = NCR.RETURN_NUMBER,
                        SAMPLE_INSP = NCR.SAMPLE_INSP,
                        SCAR_BY = NCR.SCAR_BY,
                        SCAR_DATE = NCR.SCAR_DATE,
                        SCAR_NUM = NCR.SCAR_NUM,
                        SEC = NCR.SEC,
                        SHIPPING_METHOD = NCR.SHIPPING_METHOD,
                        SKIP_LOT_LEVEL = NCR.SKIP_LOT_LEVEL,
                        STATE = NCR.STATE,
                        STATUS = NCR.STATUS,
                        TYPE_NCR = NCR.TYPE_NCR,
                        UserConfirm = NCR.UserConfirm,
                        USERDISPO = NCR.USERDISPO,
                        USERSUBMIT = NCR.USERSUBMIT,
                        VENDOR = NCR.VENDOR,
                        VEN_ADD = NCR.VEN_ADD,
                        VEN_NAME = NCR.VEN_NAME,
                        ZIP_CODE = NCR.ZIP_CODE,
                         Id = Guid.NewGuid().ToString(),
                         CreateDate =DateTime.Now
                    });

                    _db.NCR_History.Add(new NCR_History
                    {
                        Id = Guid.NewGuid().ToString(),
                        Action = "Change Status",
                        NCRNUM = nCRNUM,
                        Status = StatusInDB.Created,
                        UserId = UserIdLogin,
                        CreateDate = DateTime.Now,
                        IsActive = false
                    });

                    _db.UserDispositionApprovals.Add(new UserDispositionApproval
                    {
                        Id = Guid.NewGuid().ToString(),
                        Comment = reason,
                        DateApprove = DateTime.Now,
                        NCRNUM = nCRNUM,
                        IsActive = false,
                        UserId = approverId,
                        NCR_STATUS = "c"
                    });

                    var _Approv = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(nCRNUM.Trim())).ToList();
                    foreach (var _ap in _Approv)
                    {
                        _ap.IsActive = false;
                    }

                    var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
                    foreach (var approver in Approvers)
                    {
                        approver.isActive = false;
                    }

                    var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();
                    foreach (var det in DETs)
                    {
                        det.DISPOSITION = "";
                        det.RESPONSE = "";
                        det.REMARK = "";
                        det.DATEAPPROVAL = null;

                        _db.NCR_DET_History.Add(new NCR_DET_History
                        {
                            Id = Guid.NewGuid().ToString(),
                            CRNO = "",
                            DATEAPPROVAL = det.DATEAPPROVAL,
                            DEFECT = det.DEFECT,
                            DISPOSITION = det.DISPOSITION,
                            ITEM = det.ITEM,
                            NCR_NUM = det.NCR_NUM,
                            NC_DESC = det.NC_DESC,
                            QTY = det.QTY,
                            REMARK = det.REMARK,
                            RESPONSE = det.RESPONSE,
                            SEC = det.SEC,
                            CreateDate = DateTime.Now
                        });
                    }

                    var Orders = _db.OrderDispositions.Where(x => x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim())).ToList();
                    foreach (var o in Orders)
                    {
                        o.IsActive = false;
                    }

                    var DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();
                    foreach (var dis in DISs)
                    {
                        _db.NCR_DIS_History.Add(new NCR_DIS_History
                        {
                            Id = Guid.NewGuid().ToString(),
                            ADD_INS = dis.ADD_INS,
                            CRNO = "",
                            DATEAPPROVAL = dis.DATEAPPROVAL,
                            INSPECTOR = dis.INSPECTOR,
                            INS_DATE = dis.INS_DATE,
                            ITEM = dis.ITEM,
                            NCR_NUM = dis.NCR_NUM,
                            QTY = dis.QTY,
                            REMARK = dis.REMARK,
                            SEC = dis.SEC,
                            CreateDate = DateTime.Now
                        });
                    }

                    _db.NCR_DIS.RemoveRange(DISs);

                    _db.SaveChanges();
                    tranj.Commit();

                    return new Result
                    {
                        success = true
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    _log.LogWrite(ex.InnerException != null ? ex.InnerException.Message : "");
                    return new Result
                    {
                        success = false,
                        message = "Exception: " + ex.InnerException != null ? ex.InnerException.Message : ""
                    };
                }
            }
        }

        public bool IsMegerNCR(string part, string lot, string ccn)
        {
            var existNCR = _db.NCR_HDR.Where(x => x.MI_PART_NO.Equals(part) & x.LOT.Equals(lot)
            & x.STATUS.Trim() != StatusInDB.DispositionApproved & x.STATUS.Trim() != StatusInDB.Void & x.STATUS.Trim() != StatusInDB.Created).ToList();
            return existNCR.Count > 0;
        }

        public Result MergeNCR(string ncrnum)
        {
            //return null;
            var _log = new LogWriter("MergeNCR" + ncrnum);
            try
            {
                using (var tranj = _db.Database.BeginTransaction())
                {
                    _log.LogWrite("MergeNCR - get ncr" + ncrnum);
                    var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(ncrnum));
                    if (NCR == null) return new Result
                    {
                        success = false,
                        message = $"The NCR {ncrnum} not exist !"
                    };

                    var DETOFNCR = _db.NCR_DET.Where(x => x.NCR_NUM.Equals(NCR.NCR_NUM)).ToList();
                    int CurrentItem = DETOFNCR.Count; 
                    var NCRs = _db.NCR_HDR.Where(x => x.MI_PART_NO.Equals(NCR.MI_PART_NO) & x.LOT.Equals(NCR.LOT) & !x.NCR_NUM.Equals(ncrnum) 
                    & x.STATUS.Trim() != StatusInDB.DispositionApproved & x.STATUS.Trim() != StatusInDB.Void & x.STATUS.Trim() != StatusInDB.Created).ToList();
                    var arrNCRNUM = NCRs.Select(x => x.NCR_NUM.Trim()).ToArray();
                    var DETs = _db.NCR_DET.Where(x => arrNCRNUM.Contains(x.NCR_NUM.Trim())).ToList();
                     _log.LogWrite("get done NCRs: " + NCRs.Count);
                    if (NCRs.Count > 0)
                    {
                        var strNCRNUM = string.Join(", ", arrNCRNUM);
                        _log.LogWrite(strNCRNUM);
                        foreach (var ncr in NCRs)
                        {
                            ncr.STATUS = StatusInDB.Void;

                            var cord = _db.INS_RESULT_DEFECT.Where(x => x.NCR_Num.Equals(ncr.NCR_NUM.Trim())).ToList();
                            foreach (var item in cord)
                            {
                                item.NCR_Num = null;
                                _db.Entry(item).State = EntityState.Modified;
                            }

                            _db.NCR_History.Add(new NCR_History
                            {
                                Id = Guid.NewGuid().ToString(),
                                Action = "Change Status",
                                CreateDate = DateTime.Now
                            });
                            _log.LogWrite("void NCR " + ncr.NCR_NUM);

                            var NewDEFECTIVE = NCR.DEFECTIVE + ncr.DEFECTIVE;
                            var NewREJ_QTY = NCR.REJ_QTY + ncr.REJ_QTY;
                            var NewREC_QTY = NCR.REC_QTY + ncr.REC_QTY;
                            var NewINS_QTY = NCR.INS_QTY + ncr.INS_QTY;
                            var NewAmount = NCR.Amount + ncr.Amount;

                            _log.LogWrite($"Sum: DEF: {NewDEFECTIVE} ; REJ: {NewREJ_QTY} ; REC: {NewREC_QTY} ; INS: {NewINS_QTY} ; Amount: {NewAmount}");
                            NCR.DEFECTIVE = NewDEFECTIVE;
                            NCR.REJ_QTY = NewREJ_QTY;
                            NCR.REC_QTY = NewREC_QTY;
                            NCR.INS_QTY = NewINS_QTY;
                            NCR.Amount = NewAmount;

                            _log.LogWrite("Copy NCR_DET");
                            var OldDETs = DETs.Where(x => x.NCR_NUM.Equals(ncr.NCR_NUM)).ToList();
                            foreach (var det in OldDETs)
                            {
                                _log.LogWrite("Copy NCR_DET item: " + CurrentItem.ToString());

                                var checkDET = _db.NCR_DET.FirstOrDefault(x=>x.NCR_NUM.Equals(ncrnum) & x.NC_DESC.Equals(det.NC_DESC));
                                if (checkDET == null)
                                {
                                    _db.NCR_DET.Add(new NCR_DET
                                    {
                                        DATEAPPROVAL = null,
                                        DEFECT = det.DEFECT,
                                        DISPOSITION = det.DISPOSITION,
                                        ITEM = CurrentItem.ToString(),
                                        NCR_NUM = NCR.NCR_NUM,
                                        NC_DESC = det.NC_DESC,
                                        QTY = det.QTY,
                                        REMARK = det.REMARK,
                                        RESPONSE = det.RESPONSE,
                                        SEC = NCR.SEC
                                    });
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    checkDET.QTY = checkDET.QTY + det.QTY;
                                    _db.NCR_DET.Attach(checkDET);
                                    _db.SaveChanges();
                                }
                                CurrentItem++;
                            }

                            var EVIs = _db.NCR_EVI.Where(x => x.NCR_NUM.Trim().Equals(NCR.NCR_NUM)).ToList();
                            foreach (var evi in EVIs)
                            {
                                _db.NCR_EVI.Add(new NCR_EVI
                                {
                                    NCR_NUM = NCR.NCR_NUM.Trim(),
                                    EVI_PATH = evi.EVI_PATH,
                                    IsPrint = false,
                                    SEC = NCR.SEC.Trim()
                                });
                                _db.SaveChanges();
                            }
                        }
                    }

                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        message = $"NCR {string.Join(" ", arrNCRNUM)} was combined into NCR {NCR.NCR_NUM}"
                    };
                }
            }
            catch (Exception ex)
            {
                _log.LogWrite(ex.ToString());
                if (ex.InnerException != null) _log.LogWrite(ex.InnerException.Message);
                if (ex is DbEntityValidationException)
                {
                    var e = (DbEntityValidationException)ex;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
                return new Result
                {
                    success = false,
                    message = $"MergeNCR {ncrnum} exception: code 3312-"+DateTime.Now.ToString("ddMMyyyy")
                };
            }

        }

        public List<SCRAPCategory> GetSCRAPCategory()
        {
            return _db.SCRAPCategories.ToList();
        }

        public string[] GetCARequestNO(string nCR_NUM)
        {
            var scar = _db.SCARnNCRs.Where(x => x.NCRId.Trim() == nCR_NUM.Trim()).Select(x=>x.ScarId.Trim()).ToArray();
            return scar;
        }
        //tuan lua add 10-04-2019 get list dispo tung phan
             public List<NCRManagementViewModel> getListDispositionpartial(string iduser)
        {
            var ListNCRPartial = (from hdr in _db.NCR_HDR  
                                 join det in _db.NCR_DET on hdr.NCR_NUM equals det.NCR_NUM
                                  join us in _db.AspNetUsers on hdr.USERDISPO equals us.Id
                                  join dis in _db.NCR_DIS on det.NCR_NUM equals dis.NCR_NUM
                                   into joined
                          from j in joined.DefaultIfEmpty()
                          
                          where(hdr.STATUS.Trim() == StatusInDB.WaitingForDispositionApproval && det.DATEAPPROVAL == null && j.DATEAPPROVAL != null /*&& hdr.USERDISPO == iduser*/)
                                 select new NCRManagementViewModel
                                 {
                                     NCR_NUM = hdr.NCR_NUM,
                                     PO_NUM = hdr.PO_NUM,
                                    RECEIVER = hdr.RECEIVER,
                                     INSPECTOR = hdr.INSPECTOR,
                                    MI_PART_NO = hdr.MI_PART_NO,
                                     INS_DATE = hdr.INS_DATE,
                                     VENDOR = hdr.VENDOR,
                                    DATESUBMIT = hdr.DATESUBMIT,
                                    CCN = hdr.CCN,
                                    Amount = hdr.Amount,
                                    USERSUBMIT = us.FullName
                                 }).Distinct().ToList();

            var ncrsuni = new HashSet<NCRManagementViewModel>(ListNCRPartial);
            foreach (var item in ncrsuni.ToList())
            {
                var app = GetApproverOfNCRForConfirmNotChairMain(item.NCR_NUM.Trim()).Where(x => x.IsAppr == false).ToList();
               if(app.Count > 0)
                {
                    ncrsuni.Remove(item);
                }
                //    var NCR_DET = _db.NCR_DET.Where(x => x.NCR_NUM == item.NCR_NUM).ToList();
                //    var NCR_DIS = _db.NCR_DIS.Where(x => x.NCR_NUM == item.NCR_NUM).ToList();
                //    foreach (var item1 in NCR_DET)
                //    {
                //        foreach (var item2 in NCR_DIS)
                //        {
                //            if(item1.ITEM.Trim() == item2.ITEM.Trim() && item1.QTY == item2.QTY)
                //            {
                //                ListNCRPartial.Remove(item);
                //            }
                //        }
                //    }
            }
            

            return ncrsuni.ToList();
        }
        public List<GetListWaiitingDisposition_Result> GetListNCRWaitingDisposition(string iduser)
        {

            var ListNCR = _db.GetListWaiitingDisposition().ToList();
            return ListNCR;
        }
        //imeSpan span = DateTime.Now.Subtract(DateTime.Today);
       
        //tuanlua --29-01-2019
        public string GetDescriptionbyPartNum(string partnum ,string ccn)
        {
            var item = _db.DESCRIPTIONs.Where(x => x.ITEM.Trim() == partnum.Trim() & x.CCN == ccn).Select(x => x.ITEM_DESC).FirstOrDefault();
            if(item != null)
            {
                return item;
            }
            return null;
        }
        public bool CheckSignature(string Iduser)
        {
            var check = _db.AspNetUsers.Where(x => x.Id.Trim() == Iduser.Trim()).Select(x => x.Signature).FirstOrDefault();
            if (!string.IsNullOrEmpty(check))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region Private Function
        private string GetUserFullNameByID(string id)
        {
            var user = _db.AspNetUsers.FirstOrDefault(x=>x.Id.Equals(id));
            return user != null ? user.FullName : "";
        }

        #endregion
        //tuan lua add 09/03/2019
        public List<NCRAgingViewmodel> GetListNCRSCRAP()
        {

            string query = $" select  a.NCR_NUM, a.Amount, a.MI_PART_NO,  b.FullName,a.PO_NUM, a.INS_DATE,A.DATESUBMIT,c.NAME as status from ncr_hdr a, AspNetUsers b, status c where a.status='f'and a.USERSUBMIT=b.Id and a.STATUS=c.ID and a.Amount<=10 and a.Comment like '%NC value less than 10%' order by a.DATESUBMIT desc";
            var ListNCR = _db.Database.SqlQuery<NCRAgingViewmodel>(query).ToList();
            return ListNCR;
        }
        public List<NCRAgingViewmodel> GetListNCRSubmitbyUser(string id)
        {
          //  var isope = _db.AspNetUsers.Where(x => x.OPE != null).Select(x=>x.Id).ToList();
          //  var noneope = _db.AspNetUsers.Where(x => x.OPE == null).Select(x=>x.Id).ToList();
            var listNCRope =( from nc in _db.NCR_HDR
                          join b in _db.AspNetUsers on nc.INSPECTOR equals b.Id
                          where (nc.STATUS.Trim() == StatusInDB.Created && b.OPE == id)
                          select (new NCRAgingViewmodel
                          {
                              NCR_NUM = nc.NCR_NUM,
                              PO_NUM = nc.PO_NUM,
                               REC_QTY = nc.REC_QTY,
                              //  INS_QTY = nc.INS_QTY,
                              RECEIVER = nc.RECEIVER,
                              INSPECTOR = nc.INSPECTOR,
                              //  TYPE_NCR = nc.TYPE_NCR,
                              //  STATUS = stt.NAME, // 
                              MI_PART_NO = nc.MI_PART_NO,
                              INS_DATE = nc.INS_DATE,
                              VENDOR = nc.VENDOR,
                              // DATESUBMIT = nc.DATESUBMIT,
                              //  USERSUBMIT = j.FullName,
                              SEC = nc.SEC,
                              Amount = nc.Amount,
                              CCN = nc.CCN,
                              REJ_QTY=nc.REJ_QTY,
                              // VENDOR = vender.VEN_NAM,
                          })).ToList();
            var listNCRnone = (from nc in _db.NCR_HDR
                          join b in _db.AspNetUsers on nc.INSPECTOR equals b.Id
                          where (nc.STATUS.Trim() == StatusInDB.Created && b.Id == id)
                          select (new NCRAgingViewmodel
                          {
                              NCR_NUM = nc.NCR_NUM,
                              PO_NUM = nc.PO_NUM,
                              Amount = nc.Amount,
                              CCN = nc.CCN,
                              REC_QTY = nc.REC_QTY,
                              //  INS_QTY = nc.INS_QTY,
                              RECEIVER = nc.RECEIVER,
                              INSPECTOR = nc.INSPECTOR,
                              //  TYPE_NCR = nc.TYPE_NCR,
                              //  STATUS = stt.NAME, // 
                              MI_PART_NO = nc.MI_PART_NO,
                              INS_DATE = nc.INS_DATE,
                              VENDOR = nc.VENDOR,
                              // DATESUBMIT = nc.DATESUBMIT,
                              //  USERSUBMIT = j.FullName,
                              SEC = nc.SEC,
                              REJ_QTY = nc.REJ_QTY
                              // VENDOR = vender.VEN_NAM,
                          })).ToList();
            List<NCRAgingViewmodel> ListNCR = new List<NCRAgingViewmodel>();
            foreach (var item in listNCRope)
            {
                ListNCR.Add(item);
            }
            foreach (var item in listNCRnone)
            {
                ListNCR.Add(item);
            }
            ListNCR.Distinct();
            return ListNCR;
        }

    }

}

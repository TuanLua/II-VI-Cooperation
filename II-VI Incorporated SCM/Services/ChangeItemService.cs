using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IChangeItemService
    {
        string GetAutoCRNUM();
        List<ChangeItemViewmodel> Getlistchangeitem();
        ChangeItemViewmodel getinfoCR(string crNum);
        bool savechangeItem(ChangeItem model);
        string getinforChairmain(string NCR_NUM);
        List<AspNetUser> getinforMRBWH(string NCR_NUM);
        bool updatestatus(string crno, string status, string id);
        bool updatestatusreject(string crno, string status, string comment, string idchairmain);
        bool updatestatusacknow(string crno, string status, string id);
        List<string> getinforuserapproval(string NCR_NUM);
        List<NCR_DET> Getlistdetbyrefnumber(string rfnum);
        List<NCR_DIS> Getlistdisbyrefnumber(string rfnum);
        bool UpdatecopyncrDet(List<NCR_DET> model, string crnno);
        bool UpdatenullDET(string ncrnum);
        string getemailusersubmit(string crno);
        bool UpdatecopyncrDis(List<NCR_DIS> model, string crno);
        bool RemoveDis(List<NCR_DIS> model);
        bool ChangeIsActive(string NCR_NUM);
        bool SetisActiveDisposiotion(string NCR_Num);
        bool UpdateNCRHDRStatus(string NCR);
        bool UpdatecopyncrHDR(string ncrnum, string crno);
        List<AspNetUser> getfulluserapproval(string NCR_NUM);
        Result updateallchangeitem( string crno, string ncrnum);
        bool savefileEvident(NCR_EVI model);
        int getIDfile(string CRno);
        string getnameusersubmit(string crno);
        string getnameChairmain(string NCR_NUM);
        List<string> CheckDispoSubmitChange(string NCR);
        bool Checksubmitchangeitem(string NCR);
        List<ChangeItemViewmodel> Getlistchangeitembyncrnum(string NCRNUM);
    }

    public class ChangeItemService : IChangeItemService
    {
        private readonly IIVILocalDB _db;
        public ChangeItemService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public bool savechangeItem(ChangeItem model)
        {
            _db.ChangeItems.Add(model);
            _db.SaveChanges();
            return true;
        }
        public bool savefileEvident(NCR_EVI model)
        {
            _db.NCR_EVI.Add(model);
            _db.SaveChanges();
            return true;
        }
        public bool updatestatus(string crno, string status, string id)
        {
            ChangeItem crinfor = _db.ChangeItems.Where(x => x.CRID.Trim() == crno.Trim()).FirstOrDefault();
            crinfor.CRStatus = status;
            crinfor.ChairmanConfirmDate = DateTime.Now;
            crinfor.MRBChairmanID = id;
            _db.SaveChanges();
            return true;
        }
        public bool updatestatusacknow(string crno, string status, string id)
        {
            ChangeItem crinfor = _db.ChangeItems.Where(x => x.CRID.Trim() == crno.Trim()).FirstOrDefault();
            crinfor.CRStatus = status;
            crinfor.MRBWHConfirmDate = DateTime.Now;
            crinfor.MRBWHID = id;
            _db.SaveChanges();
            return true;
        }
        public bool updatestatusreject(string crno, string status, string comment, string idchairmain)
        {
            ChangeItem crinfor = _db.ChangeItems.Where(x => x.CRID.Trim() == crno.Trim()).FirstOrDefault();
            crinfor.CRStatus = status;
            crinfor.ChairmanComment = comment;
            crinfor.ChairmanConfirmDate = DateTime.Now;
            crinfor.MRBChairmanID = idchairmain;
            _db.SaveChanges();
            return true;
        }
        public ChangeItemViewmodel getinfoCR(string crNum)
        {
            ChangeItemViewmodel CRinfo = (from nc in _db.ChangeItems
                                          where (nc.CRID.Trim() == crNum.Trim())
                                          select new ChangeItemViewmodel
                                          {
                                              CRNo = nc.CRID,
                                              Brief = nc.Brief,
                                              Submitername = nc.UserSubmit,
                                              Comments = nc.Comment,
                                              CRStatus = nc.CRStatus,
                                              Priority = nc.CRPriority,
                                              DateSubmitted = nc.DateSubmit,
                                              DateRequired = nc.DueDate,
                                              REF_NUM = nc.RefNumber,
                                              Reason = nc.Reason,
                                              OtherAtifact = nc.OtherArtifactsImpacted,
                                              Linkactack = nc.Attachment,
                                              Chermaincomment = nc.ChairmanComment,
                                              DateChairMain = nc.ChairmanConfirmDate,
                                              Chermainname = nc.MRBChairmanID,
                                              WHMRBname = nc.MRBWHID,
                                              WHacnowledDate = nc.MRBWHConfirmDate
                                          }).FirstOrDefault();
            string name = _db.AspNetUsers.Where(x => x.Id == CRinfo.Chermainname).Select(x => x.FullName).FirstOrDefault();
            CRinfo.Chermainname = name;
            string nameuser = _db.AspNetUsers.Where(x => x.Id == CRinfo.Submitername).Select(x => x.FullName).FirstOrDefault();
            CRinfo.Submitername = nameuser;
            string nameWH = _db.AspNetUsers.Where(x => x.Id == CRinfo.WHMRBname).Select(x => x.FullName).FirstOrDefault();
            CRinfo.WHMRBname = nameWH;
            return CRinfo;
        }

        public List<ChangeItemViewmodel> Getlistchangeitem()
        {
            List<ChangeItemViewmodel> lstCR = (from a in _db.ChangeItems
                                               join u in _db.AspNetUsers on a.UserSubmit equals u.Id
                                               select (new ChangeItemViewmodel
                                               {
                                                   CRNo = a.CRID,
                                                   CRStatus = a.CRStatus,
                                                   Brief = a.Brief,
                                                   Comments = a.Comment,
                                                   DateRequired = a.DueDate,
                                                   DateSubmitted = a.DateSubmit,
                                                   Priority = a.CRPriority,
                                                   Reason = a.Reason,
                                                   REF_NUM = a.RefNumber,
                                                   Submitername = u.FullName,
                                                   DueDate = a.DueDate
                                               })).ToList();
            return lstCR;
        }
        public List<ChangeItemViewmodel> Getlistchangeitembyncrnum(string NCRNUM)
        {
            List<ChangeItemViewmodel> lstCR = (from a in _db.ChangeItems
                                               join u in _db.AspNetUsers on a.UserSubmit equals u.Id
                                               where(a.RefNumber.Trim() == NCRNUM.Trim())
                                               select (new ChangeItemViewmodel
                                               {
                                                   CRNo = a.CRID,
                                                   CRStatus = a.CRStatus,
                                                   Brief = a.Brief,
                                                   Comments = a.Comment,
                                                   DateRequired = a.DueDate,
                                                   DateSubmitted = a.DateSubmit,
                                                   Priority = a.CRPriority,
                                                   Reason = a.Reason,
                                                   REF_NUM = a.RefNumber,
                                                   Submitername = u.FullName,
                                                   DueDate = a.DueDate
                                               })).ToList();
            return lstCR;
        }
        #region nr auto
        public bool CheckExistCRNUM(string crNum)
        {
            return _db.ChangeItems.Where(x => x.CRID == crNum).FirstOrDefault() == null ? false : true;
        }
        public string[] ArrayChar =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
            "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };
        public string GetAutoCRNUM()
        {
            string crNum = "CR" + "000001";
            string char2 = "", char3 = "", char4 = "", char5 = "", char6 = "", char7 = "";
            while (CheckExistCRNUM(crNum))
            {
                if (crNum != "")
                {
                    char[] array = crNum.ToCharArray();
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
                                            char2 = "";
                                            char3 = "";
                                            char4 = "";
                                            char5 = "";
                                            char6 = "";
                                            char7 = "";
                                            break;
                                        }
                                        else
                                        {
                                            char2 = ArrayChar[Array.IndexOf(ArrayChar, array[2].ToString()) + 1];
                                        }
                                    }
                                    else
                                    {
                                        char3 = ArrayChar[Array.IndexOf(ArrayChar, array[3].ToString()) + 1];
                                    }
                                }
                                else
                                {
                                    char4 = ArrayChar[Array.IndexOf(ArrayChar, array[4].ToString()) + 1];
                                }
                            }
                            else
                            {
                                char5 = ArrayChar[Array.IndexOf(ArrayChar, array[5].ToString()) + 1];
                            }
                        }
                        else
                        {
                            char6 = ArrayChar[Array.IndexOf(ArrayChar, array[6].ToString()) + 1];
                        }
                    }

                    else
                    {
                        char7 = ArrayChar[Array.IndexOf(ArrayChar, array[7].ToString()) + 1];
                    }
                }
                crNum = "CR" + char2 + char3 + char4 + char5 + char6 + char7;
            }
            return crNum;
        }
        #endregion
        public string getinforChairmain(string NCR_NUM)
        {
            string idchairmain = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.CHAIRMAN)).Id;
            string mail = "";
            if (idchairmain != null)
            {
                string id = _db.APPROVALs.FirstOrDefault(x => x.RoleId == idchairmain && x.NCR_NUMBER.Trim() == NCR_NUM.Trim() && x.isActive == true).UserId;
                mail = _db.AspNetUsers.FirstOrDefault(x => x.Id == id).Email;

            }
            return mail;
        }
        public string getnameChairmain(string NCR_NUM)
        {
            string idchairmain = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.CHAIRMAN)).Id;
            string mail = "";
            if (idchairmain != null)
            {
                string id = _db.APPROVALs.FirstOrDefault(x => x.RoleId == idchairmain && x.NCR_NUMBER.Trim() == NCR_NUM.Trim() && x.isActive == true).UserId;
                mail = _db.AspNetUsers.FirstOrDefault(x => x.Id == id).FullName;

            }
            return mail;
        }

        public List<AspNetUser> getinforMRBWH(string NCR_NUM)
        {
            string idWHMRB = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.WHMRB)).Id;
          //  List<string> mail = new List<string>();
            if (idWHMRB != null)
            {
                string[] id = _db.ApplicationUserGroups.Where(x => x.ApplicationGroupId.Equals(idWHMRB)).Select(x => x.ApplicationUserId).ToArray();
                List<AspNetUser> users = _db.AspNetUsers.Where(x => id.Contains(x.Id)).ToList();
                //foreach (AspNetUser item in users)
                //{
                //    mail.Add(item.Email);
                //}
                return users;
            }
            return null;

        }
        public string getemailusersubmit(string crno)
        {
            string id = _db.ChangeItems.Where(x => x.CRID.Trim() == crno.Trim()).Select(x => x.UserSubmit).FirstOrDefault();
            string email = _db.AspNetUsers.FirstOrDefault(x => x.Id.Trim() == id.Trim()).Email;
            return email;
        }
        public string getnameusersubmit(string crno)
        {
            string id = _db.ChangeItems.Where(x => x.CRID.Trim() == crno.Trim()).Select(x => x.UserSubmit).FirstOrDefault();
            string name = _db.AspNetUsers.FirstOrDefault(x => x.Id.Trim() == id.Trim()).FullName;
            return name;
        }
        public List<string> getinforuserapproval(string NCR_NUM)
        {
            LogWriter log = new LogWriter(" getinforuserapproval ");
            List<string> mail = new List<string>();
            List<APPROVAL> list = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim() == NCR_NUM.Trim() && x.isActive == true).ToList();
            try
            {
                IEnumerable<string> iduserapproval = null;
                if (list.Count > 0)
                {
                    iduserapproval = list.Select(x => x.UserId);
                }
                if (iduserapproval.Count() > 0)
                {
                    foreach (string item in iduserapproval)
                    {
                        string email = _db.AspNetUsers.FirstOrDefault(x => x.Id == item).Email;
                        mail.Add(email);
                    }
                }
                return mail;

            }
            catch (Exception ex)
            {
                log.LogWrite("Acknowledge: " + Environment.NewLine + ex.ToString());
                return mail;
            }
        }
        public List<AspNetUser> getfulluserapproval(string NCR_NUM)
        {
            NCR_HDR NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(NCR_NUM.Trim()));
            if (NCR == null)
            {
                return new List<AspNetUser>();
            }

            List<APPROVAL> approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(NCR_NUM.Trim()) & x.isActive == true).ToList();
            IEnumerable<string> arrAppId = approvers.Select(x => x.UserId);
        //    IEnumerable<string> arrId = arrAppId.Concat(arrNId);

            List<AspNetUser> resApp = _db.AspNetUsers.Where(x => arrAppId.Contains(x.Id)).ToList();
            return resApp;

        }
        public List<NCR_DET> Getlistdetbyrefnumber(string rfnum)
        {
            List<NCR_DET> lstdet = _db.NCR_DET.Where(x => x.NCR_NUM.Trim() == rfnum.Trim()).ToList();
            return lstdet;
        }
        public List<NCR_DIS> Getlistdisbyrefnumber(string rfnum)
        {
            List<NCR_DIS> lstdis = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim() == rfnum.Trim()).ToList();
            return lstdis;
        }
        public bool UpdateNCRHDRStatus(string NCR)
        {
            try
            {
                NCR_HDR hdr = _db.NCR_HDR.Where(x => x.NCR_NUM.Trim() == NCR.Trim()).FirstOrDefault();
                hdr.STATUS = StatusInDB.WaitingForDisposition;
                hdr.DateApproval = null;
                _db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveDis(List<NCR_DIS> model)
        {
            try
            {

                if (model.Count > 0)
                {
                    _db.NCR_DIS.RemoveRange(model);
                    _db.SaveChanges();
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdatecopyncrDet(List<NCR_DET> model, string crno)
        {

            List<NCR_DET_History> lsthis = new List<NCR_DET_History>();
            try
            {
                foreach (NCR_DET item in model)
                {
                    NCR_DET_History history = new NCR_DET_History
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
                        CRNO = crno
                    };
                    lsthis.Add(history);
                };
                _db.NCR_DET_History.AddRange(lsthis);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdatenullDET(string ncrnum)
        {
            List<NCR_DET> lstdet = _db.NCR_DET.Where(x => x.NCR_NUM.Trim() == ncrnum.Trim()).ToList();
            try
            {
                if (lstdet.Count() > 1)
                {
                    foreach (NCR_DET item in lstdet)
                    {
                        item.DATEAPPROVAL = null;
                        item.RESPONSE = null;
                        item.DISPOSITION = null;
                    }
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdatecopyncrDis(List<NCR_DIS> model, string crno)
        {
            List<NCR_DIS_History> lsthis = new List<NCR_DIS_History>();
            try
            {

                foreach (NCR_DIS item in model)
                {
                    NCR_DIS_History history = new NCR_DIS_History
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
                        CRNO = crno,
                        CreateDate = DateTime.Now
                    };
                    lsthis.Add(history);
                };
                _db.NCR_DIS_History.AddRange(lsthis);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool ChangeIsActive(string NCR_NUM)
        {
            List<UserDispositionApproval> lst = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim() == NCR_NUM.Trim()).ToList();
            try
            {
                if (lst.Count() > 1)
                {
                    foreach (UserDispositionApproval item in lst)
                    {
                        item.IsActive = false;

                    }
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SetisActiveDisposiotion(string NCR_Num)
        {
            List<OrderDisposition> lst = _db.OrderDispositions.Where(x => x.NCR_NUMBER.Trim() == NCR_Num.Trim()).ToList();
            try
            {

                if (lst.Count() > 1)
                {
                    foreach (OrderDisposition item in lst)
                    {
                        item.IsActive = false;

                    }
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdatecopyncrHDR(string ncrnum, string crno)
        {
            NCR_HDR hdr = _db.NCR_HDR.Where(x => x.NCR_NUM.Trim() == ncrnum.Trim()).FirstOrDefault();
            try
            {

                NCR_HDR_History history = new NCR_HDR_History
                {
                    ITEM_DESC = hdr.ITEM_DESC,
                    NCR_NUM = hdr.NCR_NUM,
                    AQL = hdr.AQL,
                    BOOK_INV = hdr.BOOK_INV,
                    CCN = hdr.CCN,
                    CITY = hdr.CITY,
                    CORRECT_ACTION = hdr.CORRECT_ACTION,
                    Comment = hdr.Comment,
                    CRNO = crno,
                    DateApproval = hdr.DateApproval,
                    DATEDISPO = hdr.DATEDISPO,
                    DRAW_REV = hdr.DRAW_REV,
                    DATESUBMIT = hdr.DATESUBMIT,
                    EN_PIC = hdr.EN_PIC,
                    FAI = hdr.FAI,
                    FIRST_ARTICLE = hdr.FIRST_ARTICLE,
                    FOLLOW_UP_NOTES = hdr.FOLLOW_UP_NOTES,
                    INSPECTOR = hdr.INSPECTOR,
                    INS_DATE = hdr.INS_DATE,
                    INS_PLAN = hdr.INS_PLAN,
                    INS_QTY = hdr.INS_QTY,
                    ISSUED_REQUEST_DATE = hdr.ISSUED_REQUEST_DATE,
                    ISSUED_REQUEST_NO = hdr.ISSUED_REQUEST_NO,
                    ISSUE_MEMO_DATE = hdr.ISSUE_MEMO_DATE,
                    ISSUE_MEMO_NO = hdr.ISSUE_MEMO_NO,
                    LEVEL = hdr.LEVEL,
                    LOT = hdr.LOT,
                    MFG_DATE = hdr.MFG_DATE,
                    MFG_PIC = hdr.MFG_PIC,
                    MI_PART_NO = hdr.MI_PART_NO,
                    MODEL_NO = hdr.MODEL_NO,
                    NOTES = hdr.NOTES,
                    NOTIFICATION_ONLY = hdr.NOTIFICATION_ONLY,
                    NOT_REQUIRED = hdr.NOT_REQUIRED,
                    PERCENT_INSP = hdr.PERCENT_INSP,
                    PO_NUM = hdr.PO_NUM,
                    PUR_DATE = hdr.PUR_DATE,
                    PUR_PIC = hdr.PUR_PIC,
                    QA_DATE = hdr.QA_DATE,
                    QA_PIC = hdr.QA_PIC,
                    RECEIVER = hdr.RECEIVER,
                    REC_QTY = hdr.REC_QTY,
                    REJ_QTY = hdr.REJ_QTY,
                    REMOVED_FROM = hdr.REMOVED_FROM,
                    REQUIRED = hdr.REQUIRED,
                    RETURN_NUMBER = hdr.RETURN_NUMBER,
                    SAMPLE_INSP = hdr.SAMPLE_INSP,
                    ZIP_CODE = hdr.ZIP_CODE,
                    SCAR_BY = hdr.SCAR_BY,
                    SCAR_DATE = hdr.SCAR_DATE,
                    SCAR_NUM = hdr.SCAR_NUM,
                    VEN_ADD = hdr.VEN_ADD,
                    SEC = hdr.SEC,
                    SHIPPING_METHOD = hdr.SHIPPING_METHOD,
                    SKIP_LOT_LEVEL = hdr.SKIP_LOT_LEVEL,
                    STATE = hdr.STATE,
                    STATUS = hdr.STATUS,
                    TYPE_NCR = hdr.TYPE_NCR,
                    USERDISPO = hdr.USERDISPO,
                    USERSUBMIT = hdr.USERSUBMIT,
                    VENDOR = hdr.VENDOR,
                    VEN_NAME = hdr.VEN_NAME,
                    Id = Guid.NewGuid().ToString(),
                    UserConfirm = hdr.UserConfirm,
                    ConfirmDate = hdr.ConfirmDate,
                    Amount = hdr.Amount,
                    DEFECTIVE = hdr.DEFECTIVE,
                    CreateDate = DateTime.Now
                };
                _db.NCR_HDR_History.Add(history);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Result updateallchangeitem(string crno, string ncrnum)
        {
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                NCR_HDR NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(ncrnum));
                List<NCR_DET> DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(ncrnum)).ToList();
                List<NCR_DIS> DISs = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim().Equals(ncrnum)).ToList();
                List<UserDispositionApproval> userDispositionApprovals = _db.UserDispositionApprovals.Where(x => x.NCRNUM.Trim().Equals(ncrnum)).ToList();
                List<OrderDisposition> orderDisposition = _db.OrderDispositions.Where(x => x.NCR_NUMBER.Trim().Equals(ncrnum)).ToList();
                try
                {

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
                                CRNO = crno,
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
                                CRNO = crno,
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
                        item.NCR_STATUS = "Acknow";
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
                        CORRECT_ACTION = NCR.CORRECT_ACTION,
                        Comment = NCR.Comment,
                        CRNO = crno,
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
                    var Approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(ncrnum.Trim()) & x.isActive == true).ToList();
                    foreach (var approver in Approvers)
                    {
                        if(approver.UserId != ideng)
                        {
                            approver.isActive = false;
                        }
                    }
                    //update status NCRHDR = c
                    NCR.STATUS = StatusInDB.WaitingForDisposition;
                    NCR.REQUIRED = null;
                    NCR.NOT_REQUIRED = null;
                    NCR.NOTIFICATION_ONLY = null;
                   // NCR.USERDISPO = null;
                 //   NCR.DATEDISPO = null;
                    NCR.DateApproval = null;
                    _db.Entry(NCR).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        message = "Acknokledged sucess!",
                        obj = crno
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    LogWriter log = new LogWriter("Acknokledged Exception");
                    log.LogWrite(ex.ToString() + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    return new Result
                    {
                        success = false,
                        message = "Acknokledged is unsuccessful, Exception ",
                        obj= ex.Message
                    };
                }
            }
        }
        public int getIDfile(string CRno)
        {
            var data = _db.NCR_EVI.Where(x => x.NCR_NUM.Trim() == CRno.Trim()).Select(x => x.EVI_ID).FirstOrDefault();
            return data;
        }
        public List<string> CheckDispoSubmitChange(string NCR)
        {
            var isDispoTungPhan = _db.NCR_DIS.Where(x => x.NCR_NUM.Trim() == NCR.Trim()).ToList();
            List<string> Check =  new List<string>();
            if(isDispoTungPhan != null)
            {
                foreach (var item in isDispoTungPhan)
                {
                    if (item.REV == null)
                    {
                        Check.Add(item.NCR_NUM);
                    }
                }
                return Check;
            }
            else
            {
                return null;
            }
        }
        public bool Checksubmitchangeitem(string NCR)
        {
            var isDispoTungPhan = _db.ChangeItems.Where(x => x.RefNumber.Trim() == NCR.Trim() && ( x.CRStatus.Trim() == "Approve" || x.CRStatus.Trim() == "Created")).ToList();
            if (isDispoTungPhan.Count >0)
            {
              
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Models.SCAR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace II_VI_Incorporated_SCM.Services
{
    public interface ISCARService
    {
        VendorViewModel GetVendorInfomation(string ncrNum);
        List<getlistwaittingScar_Result> GetListwaittingSCAR();
        List<SelectListItem> GetDropdownCategory();
        string Getcategory(string Scarnum);
        List<string> getNCRnumbyScar(string ScarID);
        string GetdefectScar(string Scarnum);
        void GetInfoByNCRNUM(VendorViewModel model, string NCR_NUM);
        List<SelectListItem> GetListDefect(string NCR_NUM);
        List<SelectListItem> GetListSuppliers();
        VendorViewModel GetSupplierById(string id);
        bool GetNameRoleBuyer(string id);
        bool GetNameRoleSQE(string id);
        List<SCARInfoViewModel> GetListSCAR();
        void SaveSCARInfo(SCARINFO model);
        string Status(string SCARID);
        SCARInfoViewModel GetSCARInfoBySCARID(string SCARID);
        void UpdateStatusSCAR(string status, string SCARID);
        List<string> ListSuggestEmail(string SCARID);
        bool SentEmail(SentMailViewModel model, string path);
        void SaveD0(SCAR_RESULT_D0 model);
        void SaveD1(SCAR_RESULT_D1 model);
        void SaveD2(SCAR_RESULT_D2 model);
        void RemoveD3(int SCARID);
        void SaveD3(SCAR_RESULT_D3 model);
        void SaveD4(SCAR_RESULT_D4 model);
        void RemoveD5(int SCARID);
        void SaveD5(SCAR_RESULT_D5 model);
        void SaveD6(SCAR_RESULT_D6 model);
        void RemoveD6(int SCARID);
        void SaveD7(SCAR_RESULT_D7 model);
        Result SaveSCARnNCR(SCARINFO sCARINFO, List<string> NCRs);
        void SaveChange();
        bool CheckAcceptedStatus(int SCARID);
        List<bool> LoadCheckBoxD(int SCARID);
        bool SentMailRemind(SentMailViewModel model, string path);
        void SaveReasonReject(SCAR_INFO_BACKUP model);
        SCAR_INFO_BACKUP GetReasonReject(string SCARID, string reason);
        SCARINFO SaveEditSCAR(EditSCARViewModel model);
        bool SaveDataD8(D8ViewModel model);
        bool SaveDataD8Popup(string SUPPLIER_REPRESENTATIVE, DateTime DATE_D8, string ACKNOWLEDGEMENT, string SCAR_ID);
        SCAR_RESULT_D0 GetDataD0(int id);
        SCAR_RESULT_D1 GetDataD1(int id);
        SCAR_RESULT_D2 GetDataD2(int id);
        List<SCAR_RESULT_D3> GetDataD3(int id);
        SCAR_RESULT_D4 GetDataD4(int id);
        List<SCAR_RESULT_D5> GetDataD5(int id);
        List<SCAR_RESULT_D6> GetDataD6(int id);
        List<SCAR_RESULT_D7> GetDataD7(int id);
      SCARINFO GetDataD8(int id);
        string GetFullNameByUserId(string id);
        bool DeleteData7D(int SCAR_ID);
        List<SelectListItem> GetListNCRNumber(string ncrNum,string defect);
        bool SaveDataD8Edit(string ID, HttpPostedFileBase file, string content);
    }
    public class SCARService : ISCARService
    {
        private IIVILocalDB _db;
        private LogWriter log = new LogWriter("");
        public SCARService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public List<SelectListItem> GetListNCRNumber(string ncrNum,string defect)
        {
            var NCR = _db.NCR_HDR.Where(x => x.NCR_NUM == ncrNum).Select(x=>new { x.CCN ,x.VENDOR,x.MI_PART_NO}).FirstOrDefault();

            //var NCRs = _db.NCR_HDR.Where(x => x.VENDOR.Trim() == NCR.VENDOR.Trim() && x.MI_PART_NO.Trim() == NCR.MI_PART_NO && x.CCN.Trim() == NCR.CCN.Trim() && x.STATUS.Trim() == StatusInDB.DispositionApproved && x.REQUIRED == true).Select(x=>x.NCR_NUM).ToArray();
            var NCRs = _db.NCR_HDR.Where(x => x.VENDOR.Trim() == NCR.VENDOR.Trim() && x.MI_PART_NO.Trim() == NCR.MI_PART_NO && x.CCN.Trim() == NCR.CCN.Trim() && x.STATUS.Trim() == StatusInDB.DispositionApproved).Select(x => x.NCR_NUM).ToArray();
            var scarIds = _db.SCARINFOes.Where(x => x.DEFECTCODE.Equals(defect)).Select(x => x.SCAR_ID).ToArray();
            var Ncr_numsed = _db.SCARnNCRs.Where(x => scarIds.Contains(x.ScarId)).Select(x => x.NCRId).ToArray();
            //string query = $"select NCR_NUM from ncr_det where nc_desc like ( '%{defect.Trim()}%' ) and RESPONSE = 'A'  AND NCR_NUM IN ('{string.Join("','", NCRs)}') " +
            //                            $" and NCR_NUM NOT IN ('{string.Join("','", Ncr_numsed)}')";
            //Select NCR list that has the same defect(NC_GRP_CODE) not just NC_Code like above
            string query = $"select NCR_NUM from ncr_det a where EXISTs (select * from nc b where a.defect like '%'+b.NC_GRP_CODE+'%' and '%{defect.Trim()}%' like '%'+ b.NC_CODE+'%') and RESPONSE = 'A'  AND NCR_NUM IN ('{string.Join("','", NCRs)}') " +
                                        $" and NCR_NUM NOT IN ('{string.Join("','", Ncr_numsed)}')";
            var res = _db.Database.SqlQuery<string>(query).ToList();
            //var NCRSCAR = _db.SCARnNCRs.Where(x => x.NCRId.Trim() == ncrNum.Trim() && x.).FirstOrDefault();

            List<SelectListItem> listResult = new List<SelectListItem>();
            //if (!lstdefectncr.Contains(item1.Trim()))
            //{
             
            //}
            listResult = res.Select(x => new SelectListItem
            {
                Value = x.Trim(),
                Text = x.Trim(),
                Selected = ncrNum.Trim() == x.Trim()
            }).ToList();
            return listResult;
        }
        public VendorViewModel GetVendorInfomation(string ncrNum)
        {
            VendorViewModel data = new VendorViewModel();

            var vendorData = _db.NCR_HDR.Where(x => x.NCR_NUM == ncrNum).FirstOrDefault();
            if (vendorData != null)
            {
                string vendor = vendorData.VENDOR.Trim();
                var ven = _db.VENDORs.Where(x => x.VENDOR1.Trim() == vendor.Trim()).FirstOrDefault();
                if (ven != null)
                {
                    data.NCR_NUMBER = ncrNum;
                    data.ADDRESS = ven.ADDRESS;
                    data.CCN = ven.CCN;
                    data.CTRY = ven.CTRY;
                    data.PUR_LOC = ven.PUR_LOC;
                    data.STATE = ven.STATE;
                    data.VENDOR1 = ven.VENDOR1;
                    data.VEN_NAM = ven.VEN_NAM;
                    data.ZIP = ven.ZIP;
                    data.TEL = ven.TEL;
                    data.FAX = ven.FAX;
                    data.EMAIL = ven.EMAIL;
                    data.CONTACT = ven.CONTACT;
                }
            }

            return data;
        }

        public void GetInfoByNCRNUM(VendorViewModel model, string NCR_NUM)
        {
            var data = _db.NCR_HDR.Where(x => x.NCR_NUM == NCR_NUM).FirstOrDefault();
            if (data != null)
            {
                model.MI_PART_NO = data.MI_PART_NO;
                model.PO_NUMBER = data.PO_NUM;
                model.LOT = data.LOT;
            }
        }

        public List<SelectListItem> GetListDefect(string NCR_NUM)
        {
            List<SelectListItem> listResult = new List<SelectListItem>();

            List<string> listCode = new List<string>();
            var listDefect = _db.NCR_DET.Where(x => x.NCR_NUM == NCR_NUM).ToList();
           var scarIds = _db.SCARnNCRs.Where(x => x.NCRId.Equals(NCR_NUM)).Select(x => x.ScarId).ToArray();
           // var Ncr_numsed = _db.SCARnNCRs.Where(x => .Contains(x.ScarId)).Select(x => x.NCRId).ToArray();
            var lstdefectncr = _db.SCARINFOes.Where(x => scarIds.Contains(x.SCAR_ID)).Select(x => x.DEFECTCODE).ToArray();

          // var lstdefect = lstdefectncr.ToArray();
         //   listDefect.Concat(lstdefectncr);
            foreach (var item in listDefect)
            {
                string[] code = item.NC_DESC.Split(';');
                foreach (var item1 in code)
                {
                    if (!listCode.Contains(item1))
                    {
                        if (!lstdefectncr.Contains(item1.Trim()))
                        {
                            listCode.Add(item1);
                            //listCode.FindIndex
                            listResult.Add(new SelectListItem
                            {
                                Value = _db.NCs.Where(x => x.NC_CODE == item1.Trim()).FirstOrDefault().NC_CODE,
                                Text = _db.NCs.Where(x => x.NC_CODE == item1.Trim()).FirstOrDefault().NC_CODE + " " + _db.NCs.Where(x => x.NC_CODE == item1.Trim()).FirstOrDefault().NC_DESC,
                                //       Selected = _db.NCs.Where(x => x.NC_CODE == item1.Trim()).FirstOrDefault().NC_DESC == _db.NCs.Where(x => x.NC_CODE == item1.Trim()).FirstOrDefault().NC_DESC 
                            });
                        }
                    }
                }
            }
            if(listResult.Count > 1)
            {
                listResult[0].Selected = true;
            }
            return listResult;
        }

        public List<SelectListItem> GetListSuppliers()
        {
            List<SelectListItem> listResult = new List<SelectListItem>();
            var listvendor = _db.VENDORs.ToList();
            listResult = listvendor.Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim(),
                Text = x.VENDOR1.Trim() + " " + x.VEN_NAM.Trim(),
            }).ToList();
            return listResult;
        }

        public SCARInfoViewModel GetSCARInfoBySCARID(string SCARID)
        {
            SCARInfoViewModel data = new SCARInfoViewModel();

            //data = (from scar in _db.SCARINFOes
            //        join ncscar in _db.SCARnNCRs on scar.SCAR_ID equals ncscar.ScarId
            //        where (scar.SCAR_ID.Trim() == SCARID.Trim())
            //        select new SCARInfoViewModel
            //        {
            //            BUYER = scar.BUYER,
            //            DATEPROBLEM = scar.DATEPROBLEM,
            //            DATERESPOND = scar.DATERESPOND,
            //            ID = scar.ID,
            //            ITEM = scar.ITEM,
            //            NCR_NUMBER = scar.NCR_NUMBER,
            //            NON_QTY = scar.NON_QTY,
            //            PO_NUMBER = scar.PO_NUMBER,
            //            PROBLEM = scar.PROBLEM,
            //            QUALITY = scar.QUALITY,
            //            RMA = scar.RMA,
            //            STATUS = scar.STATUS,
            //            VENDOR = scar.VENDOR,
            //            VERSION = scar.VERSION.ToString(),
            //            VN_NCR = scar.VN_NCR,
            //            VN_SCAR = scar.VN_SCAR,
            //            WRITTENBY = scar.WRITTENBY,
            //            WRITTENDATE = scar.WRITTENDATE,
            //            SCAR_ID = scar.SCAR_ID,
            //            LOT = scar.NCR_NUMBER != null ? (_db.NCR_HDR.Where(d => d.NCR_NUM == scar.NCR_NUMBER).FirstOrDefault() != null ? _db.NCR_HDR.Where(d => d.NCR_NUM == scar.NCR_NUMBER).FirstOrDefault().LOT : "") : "",
            //            MI_PART_NO = scar.MI_PART_NO,
            //            ACKNOWLEDGEMENT = scar.ACKNOWLEDGEMENT,
            //            CONTENT = scar.CONTENT,
            //            DATE_D8 = scar.DATE_D8,
            //            SCAR_STATUS = scar.SCAR_STATUS,
            //            SUPPLIER_REPRESENTATIVE = scar.SUPPLIER_REPRESENTATIVE,
            //            EDIVENCE_D8 = scar.EDIVENCE_D8
            //            //VEN_NAME = x.VENDOR != null ? (_db.VENDORs.Where(d => d.VENDOR1 == x.VENDOR).FirstOrDefault() != null ? _db.VENDORs.Where(d => d.VENDOR1 == x.VENDOR).FirstOrDefault().VEN_NAM : "Unknown") : "Unknown",
            //        }).FirstOrDefault();

            data = _db.SCARINFOes.Where(x => x.SCAR_ID.Trim() == SCARID.Trim()).Select(x => new SCARInfoViewModel
            {
                BUYER = x.BUYER,
                DATEPROBLEM = x.DATEPROBLEM,
                DATERESPOND = x.DATERESPOND,
                ID = x.ID,
                ITEM = x.ITEM,
                NCR_NUMBER = x.NCR_NUMBER,
                NON_QTY = x.NON_QTY,
                PO_NUMBER = x.PO_NUMBER,
                PROBLEM = x.PROBLEM,
                QUALITY = x.QUALITY,
                RMA = x.RMA,
                STATUS = x.STATUS,
                VENDOR = x.VENDOR,
                VERSION = x.VERSION.ToString(),
                VN_NCR = x.VN_NCR,
                VN_SCAR = x.VN_SCAR,
                WRITTENBY = x.WRITTENBY,
                WRITTENDATE = x.WRITTENDATE,
                SCAR_ID = x.SCAR_ID,
                LOT = x.NCR_NUMBER != null ? (_db.NCR_HDR.Where(d => d.NCR_NUM == x.NCR_NUMBER).FirstOrDefault() != null ? _db.NCR_HDR.Where(d => d.NCR_NUM == x.NCR_NUMBER).FirstOrDefault().LOT : "") : "",
                MI_PART_NO = x.MI_PART_NO,
                ACKNOWLEDGEMENT = x.ACKNOWLEDGEMENT,
                CONTENT = x.CONTENT,
                DATE_D8 = x.DATE_D8,
                SCAR_STATUS = x.SCAR_STATUS,
                SUPPLIER_REPRESENTATIVE = x.SUPPLIER_REPRESENTATIVE,
                EDIVENCE_D8 = x.EDIVENCE_D8,
                RECURING_PROBLEM =x.RECURING_PROBLEM
                //VEN_NAME = x.VENDOR != null ? (_db.VENDORs.Where(d => d.VENDOR1 == x.VENDOR).FirstOrDefault() != null ? _db.VENDORs.Where(d => d.VENDOR1 == x.VENDOR).FirstOrDefault().VEN_NAM : "Unknown") : "Unknown",
            }).FirstOrDefault();
            var nNCR = _db.SCARnNCRs.Where(x => x.ScarId.Trim().Equals(data.SCAR_ID)).Select(x => x.NCRId).ToList();
            if(nNCR != null)
            {
                string checkISNCR = nNCR.FirstOrDefault();
                if (checkISNCR.Trim().Length > 7 && Regex.Matches(checkISNCR, @"[a-zA-Z]").Count > 0)
                {
                    data.NCR_NUMBER = string.Join(",", nNCR);
                }
            }
            return data != null ? data : new SCARInfoViewModel();
        }
        public string GetdefectScar(string Scarnum)
        {
            var defectid = _db.SCARINFOes.Where(x => x.SCAR_ID == Scarnum).Select(x => x.DEFECTCODE).FirstOrDefault();
            var result = _db.NCs.Where(x => x.NC_CODE == defectid).Select(x => x.NC_DESC).FirstOrDefault();
            if (result != null)
            {
                return result;

            }
            else{
                return null;
            }
        }
        public string Getcategory(string Scarnum)
        {
            var result = _db.SCARINFOes.Where(x => x.SCAR_ID == Scarnum).FirstOrDefault();
            if (result != null)
            {
             var cate=   result.CATEGORY;
                return cate;
            }
            else
            {
                return null;
            }
        }
        public VendorViewModel GetSupplierById(string id)
        {
            VendorViewModel model = new VendorViewModel();

            var data = _db.VENDORs.Where(x => x.VENDOR1 == id).FirstOrDefault();
            if (data != null)
            {
                model.ADDRESS = data.ADDRESS;
                model.CCN = data.CCN;
                model.CONTACT = data.CONTACT;
                model.CTRY = data.CTRY;
                model.EMAIL = data.EMAIL;
                model.FAX = data.FAX;
                model.PUR_LOC = data.PUR_LOC;
                model.STATE = data.STATE;
                model.TEL = data.TEL;
                model.VEN_NAM = data.VEN_NAM;
                model.VENDOR1 = data.VENDOR1;
                model.ZIP = data.ZIP;
            }

            return model;
        }

        public bool GetNameRoleSQE(string id)
        {
            string role = UserGroup.SQE;
            string idRole = _db.ApplicationGroups.Where(x => x.Name == role).FirstOrDefault().Id;
            var data = _db.ApplicationUserGroups.Where(x => x.ApplicationUserId == id && x.ApplicationGroupId == idRole).ToList();
            if (data.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetFullNameByUserId(string id)
        {
            if (id != "" && id != null)
            {
                var user = _db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                if (user != null)
                {
                    return user.FullName;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public bool GetNameRoleBuyer(string id)
        {
            string role = UserGroup.PURCHASING;
            string idRole = _db.ApplicationGroups.Where(x => x.Name == role).FirstOrDefault().Id;
            var data = _db.ApplicationUserGroups.Where(x => x.ApplicationUserId == id && x.ApplicationGroupId == idRole).ToList();
            if (data.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<getlistwaittingScar_Result> GetListwaittingSCAR()
        {
            var ListNCR = _db.getlistwaittingScar().ToList();
            return ListNCR;
        }
            public List<SCARInfoViewModel> GetListSCAR()
        {
            List<SCARInfoViewModel> listResult = new List<SCARInfoViewModel>();
            //var data = _db.SCARnNCRs.Where(x => x.ScarId.Equals(SCARId.Trim())).ToList();
            listResult = _db.SCARINFOes.Select(x => new SCARInfoViewModel
            {
                //LstNCRNum = GetNCRNumbersBySCARID(x.SCAR_ID),
               // NCR_NUMBER = x.NCR_NUMBER,
                BUYER = x.BUYER,
                DATEPROBLEM = x.DATEPROBLEM,
                DATERESPOND = x.DATERESPOND,
                ID = x.ID,
                ITEM = x.ITEM,
                NON_QTY = x.NON_QTY,
                PO_NUMBER = x.PO_NUMBER,
                PROBLEM = x.PROBLEM,
                QUALITY = x.QUALITY,
                RMA = x.RMA,
                STATUS = x.STATUS/* != "Accepted" ? x.STATUS : ((DateTime.Now - x.DATERESPOND).TotalDays < 1 ? "Accepted(Remind)" : "Accepted")*/,
                VENDOR = x.VENDOR,
                VERSION = x.VERSION.ToString(),
                VN_NCR = x.VN_NCR,
                VN_SCAR = x.VN_SCAR,
                WRITTENBY = x.WRITTENBY,
                WRITTENDATE = x.WRITTENDATE,
                SCAR_ID = x.ID.ToString().Length == 1 ? "VN00" + x.ID : (x.ID.ToString().Length == 2 ? "VN0" + x.ID : "VN" + x.ID),
                LOT = x.NCR_NUMBER != null ? (_db.NCR_HDR.Where(d => d.NCR_NUM == x.NCR_NUMBER).FirstOrDefault() != null ? _db.NCR_HDR.Where(d => d.NCR_NUM == x.NCR_NUMBER).FirstOrDefault().LOT : "") : "",
                MI_PART_NO = x.MI_PART_NO,
                VEN_NAME = x.VENDOR != null ? (_db.VENDORs.Where(d => d.VENDOR1 == x.VENDOR).FirstOrDefault() != null ? _db.VENDORs.Where(d => d.VENDOR1 == x.VENDOR).FirstOrDefault().VEN_NAM : "Unknown") : "Unknown",
                LstNCRNum = _db.SCARnNCRs.Where(a => a.ScarId == x.SCAR_ID).Select(a => a.NCRId).ToList(),
                CATEGORY = x.CATEGORY
            }).ToList();

            foreach (var item in listResult)
            {
                item.LstNCRNum = GetNCRNumbersBySCARID(item.SCAR_ID);

                //if (item.STATUS == "Accepted by Supplier")
                //{
                //    double a = (DateTime.Now - item.DATERESPOND).TotalDays;
                //    if ((DateTime.Now - item.DATERESPOND).TotalDays > -1)
                //    {
                //        item.STATUS = "Verification";
                //    }
                //}

                var user = _db.AspNetUsers.Where(x => x.Id == item.WRITTENBY).FirstOrDefault();
                if (user != null)
                {
                    item.WRITTENNAMEBY = user.FullName;
                }
                else
                {
                    item.WRITTENNAMEBY = "Unknown";
                }
            }

            return listResult;
        }

        public Result SaveSCARnNCR(SCARINFO sCARINFO, List<string> NCRs)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                if (sCARINFO == null)
                {
                    return new Result
                    {
                        success = false,
                        message = "SCAR not valid"
                    };
                }

                try
                {
                    //_db.SCARINFOes.Attach(sCARINFO);
                    var NewSCAR = _db.SCARINFOes.Add(sCARINFO);
                    _db.SaveChanges();

                    NewSCAR.SCAR_ID = NewSCAR.ID.ToString().Length == 1 ? "VN00" + NewSCAR.ID : (NewSCAR.ID.ToString().Length == 2 ? "VN0" + NewSCAR.ID : "VN" + NewSCAR.ID);
                    _db.SaveChanges();
                    if(NCRs != null)
                    {
                        foreach (var ncr in NCRs)
                        {
                            _db.SCARnNCRs.Add(new SCARnNCR
                            {
                                ScarId = NewSCAR.SCAR_ID,
                                NCRId = ncr.Trim(),
                                CreateDate = DateTime.Now
                            });
                        }
                    }
                    else
                    {
                        _db.SCARnNCRs.Add(new SCARnNCR
                        {
                            ScarId = NewSCAR.SCAR_ID,
                            NCRId = sCARINFO.VENDOR,
                            CreateDate = DateTime.Now
                        });

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
                    return new Result
                    {
                        success = false,
                        message = "Exception SaveSCARnNCR"
                    };
                }
            }
        }
        public void SaveSCARInfo(SCARINFO model)
        {
            _db.SCARINFOes.Add(model);
            _db.SaveChanges();
            _db.SCARINFOes.Attach(model);
            model.SCAR_ID = model.ID.ToString().Length == 1 ? "VN00" + model.ID : (model.ID.ToString().Length == 2 ? "VN0" + model.ID : "VN" + model.ID);
            _db.SaveChanges();
        }

        public void UpdateStatusSCAR(string status, string SCARID)
        {
            var data = _db.SCARINFOes.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            if (data != null)
            {
                _db.SCARINFOes.Attach(data);
                data.STATUS = status;
                _db.SaveChanges();
            }
        }

        public List<string> ListSuggestEmail(string SCARID)
        {
            List<string> listResult = new List<string>();
            string vendorid = "";
            var vendor = _db.SCARINFOes.Where(x => x.SCAR_ID.Trim() == SCARID.Trim()).FirstOrDefault();
            if(vendor != null)
            {
                 vendorid = vendor.VENDOR;
            }
            List<SCARINFO> listScar = _db.SCARINFOes.Where(x => x.VENDOR.Trim() == vendorid.Trim()).ToList();
            List<SCAREMAIL> listEmail = _db.SCAREMAILs.ToList();
            if (listEmail != null && listScar != null)
            {
                foreach (var item in listScar)
                {
                    foreach (var item1 in listEmail)
                    {
                        if (item.SCAR_ID == item1.SCARID)
                        {
                            listResult.Add(item1.SENTTO);
                        }
                    }
                }
            }

            return listResult.GroupBy(item => item).Select(group => group.Key).ToList();
        }
        public string Status(string SCARID)
        {
            var result = _db.SCARINFOes.Where(x => x.SCAR_ID == SCARID).Select(x => x.STATUS).FirstOrDefault();
            return result;
        }
        public bool SentMailRemind(SentMailViewModel model, string path)
        {
            try
            {
                _db.spSendEmail("IQC", model.SENTTO, model.CC, model.CONTENT, null, model.SUBJECT, path);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SentEmail(SentMailViewModel model, string path)
        {
            try
            {
                _db.spSendEmail("IQC", model.SENTTO, model.CC, model.CONTENT, null, model.SUBJECT, path);
                _db.SCAREMAILs.Add(new SCAREMAIL
                {
                    CC = model.CC,
                    CONTENT = model.CONTENT,
                    SCARID = model.SCAR_ID,
                    SCAR_ID = Convert.ToInt32(model.SCAR_ID.Substring(2)),
                    SENTTO = model.SENTTO,
                    SUBJECT = model.SUBJECT,
                    FILENAME = model.FILESCAR,
                    NCRFILE = model.NCRFILE
                });

                UpdateStatusSCAR("Sent to Supplier", model.SCAR_ID);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return false;
            }
        }

        public void SaveD0(SCAR_RESULT_D0 model)
        {
            _db = new IIVILocalDB();
            var check = _db.SCAR_RESULT_D0.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            if (check != null)
            {
                _db.SCAR_RESULT_D0.Remove(check);
            }
            _db.SCAR_RESULT_D0.Add(model);
            _db.SaveChanges();
        }

        public void SaveD1(SCAR_RESULT_D1 model)
        {
            _db = new IIVILocalDB();
            var check = _db.SCAR_RESULT_D1.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            if(check != null)
            {
                _db.SCAR_RESULT_D1.Remove(check);
            }
            _db.SCAR_RESULT_D1.Add(model);
            _db.SaveChanges();
        }

        public void SaveD2(SCAR_RESULT_D2 model)
        {
            _db = new IIVILocalDB();
            var check = _db.SCAR_RESULT_D2.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            if (check != null)
            {
                _db.SCAR_RESULT_D2.Remove(check);
            }
            _db.SCAR_RESULT_D2.Add(model);
            _db.SaveChanges();
        }
        //Thi.Nguyen_13-Dec-2019: Remove all before add
        public void RemoveD3(int SCARID)
        {
            _db = new IIVILocalDB();
            var scar = _db.SCAR_RESULT_D3.Where(x => x.SCAR_ID == SCARID).ToList();
            foreach (SCAR_RESULT_D3 d3 in scar)
            {
                _db.SCAR_RESULT_D3.Remove(d3);
            }
            _db.SaveChanges();
        }
        public void SaveD3(SCAR_RESULT_D3 model)
        {
            _db = new IIVILocalDB();
            //var check = _db.SCAR_RESULT_D3.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            //if (check != null)
            //{
            //    _db.SCAR_RESULT_D3.Remove(check);
            //}
            _db.SCAR_RESULT_D3.Add(model);
            _db.SaveChanges();
        }

        public void SaveD4(SCAR_RESULT_D4 model)
        {
            _db = new IIVILocalDB();
            var check = _db.SCAR_RESULT_D4.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            if (check != null)
            {
                _db.SCAR_RESULT_D4.Remove(check);
            }
            _db.SCAR_RESULT_D4.Add(model);
            _db.SaveChanges();
        }
        //Thi.Nguyen_13-Dec-2019: Remove all before add
        public void RemoveD5(int SCARID)
        {
            _db = new IIVILocalDB();
            var scar = _db.SCAR_RESULT_D5.Where(x => x.SCAR_ID == SCARID).ToList();
            foreach (SCAR_RESULT_D5 d5 in scar)
            {
                _db.SCAR_RESULT_D5.Remove(d5);
            }
            _db.SaveChanges();
        }
        public void SaveD5(SCAR_RESULT_D5 model)
        {
            _db = new IIVILocalDB();
            //var check = _db.SCAR_RESULT_D5.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            //if (check != null)
            //{
            //    _db.SCAR_RESULT_D5.Remove(check);
            //}
            _db.SCAR_RESULT_D5.Add(model);
            _db.SaveChanges();
        }
        //Thi.Nguyen_13-Dec-2019: Remove all before add
        public void RemoveD6(int SCARID)
        {
            _db = new IIVILocalDB();
            var scar = _db.SCAR_RESULT_D6.Where(x => x.SCAR_ID == SCARID).ToList();
            foreach(SCAR_RESULT_D6 d6 in scar)
            {
                _db.SCAR_RESULT_D6.Remove(d6);
            }           
            _db.SaveChanges();
        }
        public void SaveD6(SCAR_RESULT_D6 model)
        {
            _db = new IIVILocalDB();
            //var check = _db.SCAR_RESULT_D6.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            //if (check != null)
            //{
            //    _db.SCAR_RESULT_D6.Remove(check);
            //}
            _db.SCAR_RESULT_D6.Add(model);
            _db.SaveChanges();
        }

        public void SaveD7(SCAR_RESULT_D7 model)
        {
            _db = new IIVILocalDB();
            var check = _db.SCAR_RESULT_D7.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
            if (check != null)
            {
                _db.SCAR_RESULT_D7.Remove(check);
            }
            _db.SCAR_RESULT_D7.Add(model);
            _db.SaveChanges();
        }

        public void SaveChange()
        {
            _db.SaveChanges();
        }

        public bool CheckAcceptedStatus(int SCARID)
        {
            var d0 = _db.SCAR_RESULT_D0.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d1 = _db.SCAR_RESULT_D1.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d2 = _db.SCAR_RESULT_D2.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d3 = _db.SCAR_RESULT_D3.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d4 = _db.SCAR_RESULT_D4.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d5 = _db.SCAR_RESULT_D5.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d6 = _db.SCAR_RESULT_D6.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d7 = _db.SCAR_RESULT_D7.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();

            if (d0 == null || d1 == null || d2 == null || d3 == null || d4 == null || d5 == null || d6 == null || d7 == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<bool> LoadCheckBoxD(int SCARID)
        {
            List<bool> listResult = new List<bool>();
            var d0 = _db.SCAR_RESULT_D0.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d1 = _db.SCAR_RESULT_D1.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d2 = _db.SCAR_RESULT_D2.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d3 = _db.SCAR_RESULT_D3.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d4 = _db.SCAR_RESULT_D4.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d5 = _db.SCAR_RESULT_D5.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d6 = _db.SCAR_RESULT_D6.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            var d7 = _db.SCAR_RESULT_D7.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            //tuan lua add d8
            var d8 = _db.SCARINFOes.Where(x => x.ID == SCARID && x.EDIVENCE_D8 != null).FirstOrDefault();
            #region
            if (d0 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d1 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d2 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d3 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d4 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d5 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d6 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d7 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            if (d8 == null)
            {
                listResult.Add(false);
            }
            else
            {
                listResult.Add(true);
            }
            #endregion

            return listResult;
        }

        public SCAR_INFO_BACKUP GetReasonReject(string SCARID, string reason)
        {
            var data = _db.SCARINFOes.Where(x => x.SCAR_ID == SCARID).FirstOrDefault();
            if (data != null)
            {
                SCAR_INFO_BACKUP model = new SCAR_INFO_BACKUP
                {
                    BUYER = data.BUYER,
                    DATEPROBLEM = data.DATEPROBLEM,
                    DATEREJECT = DateTime.Now,
                    DATERESPOND = data.DATERESPOND,
                    ITEM = data.ITEM,
                    LOT = data.LOT,
                    MI_PART_NO = data.MI_PART_NO,
                    NCR_NUMBER = data.NCR_NUMBER,
                    NON_QTY = data.NON_QTY,
                    PO_NUMBER = data.PO_NUMBER,
                    PROBLEM = data.PROBLEM,
                    QUALITY = data.QUALITY,
                    REASON = reason,
                    RMA = data.RMA,
                    SCAR_ID = data.ID,
                    STATUS = data.STATUS,
                    VENDOR = data.VENDOR,
                    VN_NCR = data.VN_NCR,
                    VN_SCAR = data.VN_SCAR,
                    VERSION = data.VERSION,
                    WRITTENBY = data.WRITTENBY,
                    WRITTENDATE = data.WRITTENDATE,
                    RECURING_PROBLEM = data.RECURING_PROBLEM
                };

                _db.SCARINFOes.Attach(data);
                data.STATUS = "Created";
                data.VERSION = data.VERSION + 1;
                _db.SaveChanges();

                return model;
            }
            else
            {
                return new SCAR_INFO_BACKUP();
            }
        }

        public void SaveReasonReject(SCAR_INFO_BACKUP model)
        {
            if (model != null)
            {
                _db.SCAR_INFO_BACKUP.Add(model);
                _db.SaveChanges();
            }
        }
        public List<string>getNCRnumbyScar(string ScarID)
        {
            var result = _db.SCARnNCRs.Where(x => x.ScarId == ScarID).Select(x => x.NCRId).ToList();
            return result;
                }
        public SCARINFO SaveEditSCAR(EditSCARViewModel model)
        {
            if (model != null)
            {
                var data = _db.SCARINFOes.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
                if (data != null)
                {
                    try
                    {
                        _db.SCARINFOes.Attach(data);
                        data.PROBLEM = model.PROBLEM;
                        data.RECURING_PROBLEM = model.RECURING_PROBLEM;
                        data.DATEPROBLEM = model.DATEPROBLEM;
                        data.DATERESPOND = model.DATERESPOND;
                        _db.SaveChanges();
                        return data;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else { return null; }
            }
            else
            {
                return null;
            }
        }

        public bool SaveDataD8(D8ViewModel model)
        {
            try
            {
                var data = _db.SCARINFOes.Where(x => x.SCAR_ID == model.SCAR_ID).FirstOrDefault();
                if (data != null)
                {
                    _db.SCARINFOes.Attach(data);
                    data.CONTENT = model.CONTENTD8;
                    data.SUPPLIER_REPRESENTATIVE = model.SUPPLIER_REPRESENTATIVE;
                    data.SCAR_STATUS = model.SCAR_STATUS;
                    data.ACKNOWLEDGEMENT = model.ACKNOWLEDGEMENT;
                    data.DATE_D8 = model.DATE_D8;

                    //UploadFileD8. Sil. 06/29/2018
                    //string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                    //string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";

                    string EDIVENCE_D8 = string.Empty;
                    string filePath = ConfigurationManager.AppSettings["pathEviSCAR"];
                    string filePath1 = ConfigurationManager.AppSettings["pathEviSCAR"] + model.ID + "\\";
                    DateTime now = DateTime.Now;
                    string currentDate = now.Year + "-" + now.Month + "-" + now.Day + "-" + now.Hour + "-" + now.Minute + "-" + now.Second + "-" + now.Millisecond;
                    if (model.File_Upload != null)
                    {
                        string FolderPath = System.Web.HttpContext.Current.Server.MapPath(filePath1);
                        if (!Directory.Exists(FolderPath))
                        {
                            Directory.CreateDirectory(FolderPath);
                        }
                        string _FileName = Path.GetFileName(model.File_Upload.FileName);
                        _FileName = "D8_" + model.ID + "_" + currentDate + "_" + _FileName;
                        string _path = Path.Combine(FolderPath, _FileName);
                        model.File_Upload.SaveAs(_path);
                        EDIVENCE_D8 = _FileName;
                    }
                    data.EDIVENCE_D8 = EDIVENCE_D8;

                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveDataD8Edit(string ID,HttpPostedFileBase file,string content)
        {
            try
            {
                var data = _db.SCARINFOes.Where(x => x.SCAR_ID == ID).FirstOrDefault();
                string filePath = ConfigurationManager.AppSettings["pathEviSCAR"];
                string filePath1 = ConfigurationManager.AppSettings["pathEviSCAR"] + ID + "\\";
                DateTime now = DateTime.Now;
                string currentDate = now.Year + "-" + now.Month + "-" + now.Day + "-" + now.Hour + "-" + now.Minute;
                if (file != null && data!= null)
                {
                    string FolderPath = System.Web.HttpContext.Current.Server.MapPath(filePath1);
                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                    }
                    string _FileName = Path.GetFileName(file.FileName);
                    string filename = filePath + ID + "/" + _FileName;
                    string _path = Path.Combine(FolderPath, _FileName);
                    file.SaveAs(_path);
                    _db.SCARINFOes.Attach(data);
                    data.CONTENT = content;
                    data.EDIVENCE_D8 = filename;
                    _db.SaveChanges();
                    return true;
                }
                else 
                {
                    _db.SCARINFOes.Attach(data);
                    data.CONTENT = content;
                    _db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveDataD8Popup(string SUPPLIER_REPRESENTATIVE, DateTime DATE_D8, string ACKNOWLEDGEMENT, string SCAR_ID)
        {
            try
            {
                var data = _db.SCARINFOes.Where(x => x.SCAR_ID == SCAR_ID).FirstOrDefault();
                if (data != null)
                {
                    _db.SCARINFOes.Attach(data);
                    data.SUPPLIER_REPRESENTATIVE = SUPPLIER_REPRESENTATIVE;
                    data.ACKNOWLEDGEMENT = ACKNOWLEDGEMENT;
                    data.DATE_D8 = DATE_D8;
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public SCAR_RESULT_D0 GetDataD0(int id)
        {
            var data = _db.SCAR_RESULT_D0.Where(x => x.SCAR_ID == id).FirstOrDefault();
            if (data != null)
            {
                if (data.EDIVENCE != null)
                {
                    string[] a = data.EDIVENCE.Split('/');
                    data.EDIVENCE = "..";
                    for (int i = 1; i < a.Length; i++)
                    {
                        data.EDIVENCE = data.EDIVENCE + "/" + a[i];
                    }
                }
            }
            return data != null ? data : new SCAR_RESULT_D0();
        }

        public SCAR_RESULT_D1 GetDataD1(int id)
        {
            var data = _db.SCAR_RESULT_D1.Where(x => x.SCAR_ID == id).FirstOrDefault();
            if (data != null)
            {
                if (data.EDIVENCE != null)
                {
                    string[] a = data.EDIVENCE.Split('/');
                    data.EDIVENCE = "..";
                    for (int i = 1; i < a.Length; i++)
                    {
                        data.EDIVENCE = data.EDIVENCE + "/" + a[i];
                    }
                }
            }
            return data != null ? data : new SCAR_RESULT_D1();
        }

        public SCAR_RESULT_D2 GetDataD2(int id)
        {
            var data = _db.SCAR_RESULT_D2.Where(x => x.SCAR_ID == id).FirstOrDefault();
            if (data != null)
            {
                if (data.EDIVENCE != null)
                {
                    string[] a = data.EDIVENCE.Split('/');
                    data.EDIVENCE = "..";
                    for (int i = 1; i < a.Length; i++)
                    {
                        data.EDIVENCE = data.EDIVENCE + "/" + a[i];
                    }
                }
            }
            return data != null ? data : new SCAR_RESULT_D2();
        }
        public List<SCAR_RESULT_D3> GetDataD3(int id)
        {
            var data = _db.SCAR_RESULT_D3.Where(x => x.SCAR_ID == id).ToList();
            if (data != null)
            {
                var model = data.FirstOrDefault();
                if (model != null)
                {
                    if (model.EDIVENCE != null)
                    {
                        string[] a = model.EDIVENCE.Split('/');
                        model.EDIVENCE = "..";
                        for (int i = 1; i < a.Length; i++)
                        {
                            model.EDIVENCE = model.EDIVENCE + "/" + a[i];
                        }

                        foreach (var item in data)
                        {
                            item.EDIVENCE = model.EDIVENCE;
                        }
                    }
                }
            }
            return data != null ? data : new List<SCAR_RESULT_D3>();
        }

        public SCAR_RESULT_D4 GetDataD4(int id)
        {
            var data = _db.SCAR_RESULT_D4.Where(x => x.SCAR_ID == id).FirstOrDefault();
            if (data != null)
            {
                if (data.EDIVENCE != null)
                {
                    string[] a = data.EDIVENCE.Split('/');
                    data.EDIVENCE = "..";
                    for (int i = 1; i < a.Length; i++)
                    {
                        data.EDIVENCE = data.EDIVENCE + "/" + a[i];
                    }
                }
            }
            return data != null ? data : new SCAR_RESULT_D4();
        }

        public List<SCAR_RESULT_D5> GetDataD5(int id)
        {
            var data = _db.SCAR_RESULT_D5.Where(x => x.SCAR_ID == id).ToList();
            if (data != null)
            {
                var model = data.FirstOrDefault();
                if (model != null)
                {
                    if (model.EDIVENCE != null)
                    {
                        string[] a = model.EDIVENCE.Split('/');
                        model.EDIVENCE = "..";
                        for (int i = 1; i < a.Length; i++)
                        {
                            model.EDIVENCE = model.EDIVENCE + "/" + a[i];
                        }

                        foreach (var item in data)
                        {
                            item.EDIVENCE = model.EDIVENCE;
                        }
                    }
                }
            }
            return data != null ? data : new List<SCAR_RESULT_D5>();
        }

        public List<SCAR_RESULT_D6> GetDataD6(int id)
        {
            var data = _db.SCAR_RESULT_D6.Where(x => x.SCAR_ID == id).ToList();
            if (data != null)
            {
                var model = data.FirstOrDefault();
                if (model != null)
                {
                    if (model.EDIVENCE != null)
                    {
                        string[] a = model.EDIVENCE.Split('/');
                        model.EDIVENCE = "..";
                        for (int i = 1; i < a.Length; i++)
                        {
                            model.EDIVENCE = model.EDIVENCE + "/" + a[i];
                        }

                        foreach (var item in data)
                        {
                            item.EDIVENCE = model.EDIVENCE;
                        }
                    }
                }
            }
            return data != null ? data : new List<SCAR_RESULT_D6>();
        }

        public List<SCAR_RESULT_D7> GetDataD7(int id)
        {
            var data = _db.SCAR_RESULT_D7.Where(x => x.SCAR_ID == id).ToList();
            if (data != null)
            {
                var model = data.FirstOrDefault();
                if (model != null)
                {
                    if (model.EDIVENCE != null)
                    {
                        string[] a = model.EDIVENCE.Split('/');
                        model.EDIVENCE = "..";
                        for (int i = 1; i < a.Length; i++)
                        {
                            model.EDIVENCE = model.EDIVENCE + "/" + a[i];
                        }

                        foreach (var item in data)
                        {
                            item.EDIVENCE = model.EDIVENCE;
                        }
                    }
                }
            }
            return data != null ? data : new List<SCAR_RESULT_D7>();
        }
        public SCARINFO GetDataD8(int id)
        {
            var data = _db.SCARINFOes.Where(x => x.ID == id).FirstOrDefault();
            if (data != null)
            {
                if (data.EDIVENCE_D8 != null)
                {
                    string[] a = data.EDIVENCE_D8.Split('/');
                    data.EDIVENCE_D8 = "..";
                    for (int i = 1; i < a.Length; i++)
                    {
                        data.EDIVENCE_D8 = data.EDIVENCE_D8 + "/" + a[i];
                    }
                }
            }
            return data != null ? data : new SCARINFO();
        }

        public bool DeleteData7D(int SCAR_ID)
        {
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var D0 = _db.SCAR_RESULT_D0.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D1 = _db.SCAR_RESULT_D1.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D2 = _db.SCAR_RESULT_D2.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D3 = _db.SCAR_RESULT_D3.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D4 = _db.SCAR_RESULT_D4.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D5 = _db.SCAR_RESULT_D5.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D6 = _db.SCAR_RESULT_D6.Where(x => x.SCAR_ID.Equals(SCAR_ID));
                    var D7 = _db.SCAR_RESULT_D7.Where(x => x.SCAR_ID.Equals(SCAR_ID));

                    if (D0 != null) _db.SCAR_RESULT_D0.RemoveRange(D0);
                    if (D1 != null) _db.SCAR_RESULT_D1.RemoveRange(D1);
                    if (D2 != null) _db.SCAR_RESULT_D2.RemoveRange(D2);
                    if (D3 != null) _db.SCAR_RESULT_D3.RemoveRange(D3);
                    if (D4 != null) _db.SCAR_RESULT_D4.RemoveRange(D4);
                    if (D5 != null) _db.SCAR_RESULT_D5.RemoveRange(D5);
                    if (D6 != null) _db.SCAR_RESULT_D6.RemoveRange(D6);
                    if (D7 != null) _db.SCAR_RESULT_D7.RemoveRange(D7);

                    _db.SaveChanges();

                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }
        }

        private List<string> GetNCRNumbersBySCARID(string SCARId)
        {
            var data = _db.SCARnNCRs.Where(x => x.ScarId.Equals(SCARId.Trim())).ToList();
            if (data.Count <= 0) return new List<string>();
            //return string.Join(",", data.Select(x => x.NCRId).ToList());
            return data.Select(x => x.NCRId).Distinct().ToList();
        }
        public List<SelectListItem> GetDropdownCategory()
        {
            List<SelectListItem> data = (from a in _db.SCAR_Category
                                         select (new SelectListItem
                                         {
                                             Value = a.Description.ToString(),
                                             Text = a.Description.Trim(),
                                         })).ToList();
            return data;
        }
    }
}
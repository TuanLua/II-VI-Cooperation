using System.Web.Mvc;
using II_VI_Incorporated_SCM.Services;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Models;
using System;
using System.Web;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class WriteNcrForProcessController : Controller
    {
        private readonly ICCNService _iCCNService;
        private readonly IReciverService _IReciverService;
        private readonly INCRManagementService _INCRManagementService;
        private readonly IUserService _IUserService;
        private readonly IEmailService _emailService;

        public WriteNcrForProcessController(ICCNService iCCNService, IReciverService IReciverService, INCRManagementService INCRManagementService, IUserService IUserService, IEmailService emailService)
        {
            _iCCNService = iCCNService;
            _IReciverService = IReciverService;
            _INCRManagementService = INCRManagementService;
            _IUserService = IUserService;
            _emailService = emailService;
        }


        // GET: WriteNCRForProcess
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {

            return View();
        }
        public ActionResult IqcReciever()
        {
            return PartialView();
        }
        public ActionResult IqcPoNumBer()
        {
            return PartialView();
        }

        public ActionResult Deposition()
        {
            return PartialView();
        }

        public JsonResult SaveNCRForProcess(NCRInProccessViewModel model)
        {
            var log = new LogWriter("WriteNcrForProcessController - SaveNCRForProcess");
            DateTime? dateSample;
            dateSample = null;
            //double SizeOfFile = 0;
            //double MaxLengthUpload = double.Parse(ConfigurationManager.AppSettings["MaxLengthUpload"]);
            var iduser = User.Identity.GetUserId();
            log.LogWrite("WriteNcrForProcessController - SaveNCRForProcess: UserId: " + iduser);
            string ncrnumauto = _INCRManagementService.GetAutoNCRNUM("P");
            log.LogWrite("WriteNcrForProcessController - SaveNCRForProcess: ncrnumauto: " + ncrnumauto);
            try
            {
                var _log = new LogWriter("WriteNcrForProcessController - SaveNCRForProcess: 66");
                var isSubmitted = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE)
                                  & _IUserService.ISENGINEERING(model.UserApprove);
                _log.LogWrite("WriteNcrForProcessController - :120 - isSubmitted: " + isSubmitted);

                var vd = _INCRManagementService.getvendor(StaticText.IIVIVietnam);
                _log.LogWrite("WriteNcrForProcessController - :72");
                List<NCR_EVI> nCR_EVIs = new List<NCR_EVI>();
                List<NCR_DET> nCR_DETs = new List<NCR_DET>();
                List<APPROVAL> aPPROVALs = new List<APPROVAL>();
                List<TaskManagementCreateModel> taskManagementCreateModels = new List<TaskManagementCreateModel>();
                VNMaterialTraceability vNMaterialTraceability = new VNMaterialTraceability();
                INS_RESULT_FINAL INS_RESULT_FINAL = new INS_RESULT_FINAL();
                NCR_HDR ncr_hdr = new NCR_HDR
                {
                    AQL = model.AQL,
                    CORRECT_ACTION = model.CORRECT_ACTION,
                    DRAW_REV = model.DRAW_REV,
                    EN_PIC = model.EN_PIC,
                    FAI = model.FAI,
                    FIRST_ARTICLE = model.FIRST_ARTICLE,
                    FOLLOW_UP_NOTES = model.FOLLOW_UP_NOTES,
                    INSPECTOR = User.Identity.GetUserId(),
                    //INS_DATE = model.INS_DATE,
                    INS_DATE = DateTime.Now,
                    INS_PLAN = StaticText.INS_PLAN,
                    INS_QTY = model.INS_QTY,
                    ITEM_DESC = model.ITEM_DESC,
                    LEVEL = model.LEVEL,
                    LOT = model.LOT,
                    MFG_DATE = model.MFG_DATE,
                    MFG_PIC = model.MFG_PIC,
                    MI_PART_NO = model.MI_PART_NO,
                    MODEL_NO = model.MODEL_NO,
                    PERCENT_INSP = model.PERCENT_INSP,
                    PO_NUM = model.PO_NUM,
                    PUR_DATE = model.PUR_DATE,
                    PUR_PIC = model.PUR_PIC,
                    QA_DATE = model.QA_DATE,
                    QA_PIC = model.QA_PIC,
                    RECEIVER = model.RECEIVER,
                    REC_QTY = model.REC_QTY,
                    REJ_QTY = model.REJ_QTY,
                    SAMPLE_INSP = model.SAMPLE_INSP,
                    SCAR_BY = model.SCAR_BY,
                    SCAR_DATE = model.SCAR_DATE,
                    SCAR_NUM = model.SCAR_NUM,
                    SEC = NCRType.PROCESS,
                    SKIP_LOT_LEVEL = model.SKIP_LOT_LEVEL,
                    STATE = string.IsNullOrEmpty(model.STATE) ? vd.STATE : model.STATE,
                    STATUS = isSubmitted ? StatusInDB.Submitted : StatusInDB.Created,
                    DATESUBMIT = isSubmitted ? DateTime.Now : dateSample,
                    USERSUBMIT = isSubmitted ? iduser : null,
                    TYPE_NCR = NCRType.PROCESS,
                    VENDOR = string.IsNullOrEmpty(model.VENDOR) ? StaticText.IIVIVietnam : model.VENDOR,
                    VEN_ADD = string.IsNullOrEmpty(model.VENDOR) ? vd.ADDRESS : model.VEN_ADD,
                    VEN_NAME = string.IsNullOrEmpty(model.VENDOR) ? vd.VEN_NAM : model.VEN_NAME,
                    ZIP_CODE = string.IsNullOrEmpty(model.VENDOR) ? vd.ZIP : model.ZIP_CODE,
                    CITY = string.IsNullOrEmpty(model.VENDOR) ? vd.CTRY : model.CITY,
                    NCR_NUM = ncrnumauto,
                    CCN = model.CCN,
                    DEFECTIVE = double.Parse(model.Defect_QTY.ToString()),
                    Comment = model.Comment
                };

                if (model.VNMaterialTraceability != null)
                {
                    _log.LogWrite("WriteNcrForProcessController - :130 : SaveFile VNMaterialTraceability");
                    string File_Upload_Url = SaveFile(model.VNMaterialTraceability);
                    vNMaterialTraceability.Id = Guid.NewGuid().ToString();
                    vNMaterialTraceability.IsPrint = true;
                    vNMaterialTraceability.NCRNUM = ncrnumauto.Trim();
                    vNMaterialTraceability.VNMaterialTraceability1 = File_Upload_Url;
                    //SizeOfFile += model.VNMaterialTraceability.ContentLength;
                    //logelse.LogWrite("WriteNCRForIQCController - :171 - VNMaterialTraceability: " + Environment.NewLine + JsonConvert.SerializeObject(VNMaterialTraceability));
                }

                if (model.ModelEvidence != null || model.ModelEvidence.Count > 0)
                {
                    _log.LogWrite("WriteNcrForProcessController - :141 : model.ModelEvidence: " + model.ModelEvidence.Count);
                    foreach (var item in model.ModelEvidence)
                    {
                        //logelse.LogWrite("WriteNCRForIQCController - :179 : SaveFile VNMaterialTraceability " + Environment.NewLine + JsonConvert.SerializeObject(item));
                        if (item != null)
                        {
                            string File_Upload_Url = SaveFile(item.EvidenceFile);
                            //SizeOfFile += item.EvidenceFile.ContentLength;
                            NCR_EVI ncrevi = new NCR_EVI
                            {
                                EVI_PATH = File_Upload_Url,
                                NCR_NUM = ncr_hdr.NCR_NUM,
                                SEC = ncr_hdr.SEC,
                                IsPrint = true
                            };
                            nCR_EVIs.Add(ncrevi);
                        }
                        //_iCCNService.CreateNCR_EVI(ncrevi);
                    }
                }

                //Validate max length files upload
                //if (SizeOfFile > MaxLengthUpload)
                //{
                //    return Json(new { success = false, message = "Error. Size of files is over " + MaxLengthUpload });
                //}

                if (model.ListNCR_DET != null)
                {
                    foreach (var item in model.ListNCR_DET)
                    {
                        string defect = String.Join("; ", item.DEFECT.ToArray());
                        string desc = String.Join("; ", item.NC_DESC.ToArray());
                        NCR_DET ncrdet = new NCR_DET
                        {
                            ITEM = item.ITEM,
                            QTY = item.QTY,
                            SEC = NCRType.PROCESS,
                            REMARK = item.REMARK,
                            NCR_NUM = ncr_hdr.NCR_NUM,
                            DEFECT = defect,
                            NC_DESC = desc
                        };
                        if (ncrdet.DEFECT != null && ncrdet.NC_DESC != null)
                        {
                            nCR_DETs.Add(ncrdet);
                        }

                    }
                }

                if (model.RECEIVER != null && model.RECEIVER != "")
                {
                    _log.LogWrite("WriteNcrForProcessController - model.RECEIVER: " + model.RECEIVER);
                    string recline = _iCCNService.GetRecLineByReciever(model.RECEIVER);
                    string seq = _iCCNService.GetSEQInInsResultFinal(model.RECEIVER);
                    _log.LogWrite($@"WriteNcrForProcessController - recline: {recline} | seq: {seq}");
                    double sum = 0;
                    if (model.ListNCR_DET != null)
                    {
                        foreach (var item in model.ListNCR_DET)
                        {
                            sum = sum + item.QTY;
                        }
                    }
                    _log.LogWrite($@"WriteNcrForProcessController - INS_RESULT_FINAL");
                    INS_RESULT_FINAL = new INS_RESULT_FINAL
                    {
                        ACC_QTY = model.INS_QTY,
                        SEC = "NCR",
                        INSPECTOR = User.Identity.GetUserId(),
                        INS_DATE = DateTime.Now,
                        RECEIVER = model.RECEIVER,
                        REC_LINE = recline,
                        REC_QTY = model.REC_QTY,
                        REJ_QTY = (double)model.REJ_QTY,
                        CCN = model.CCN,
                        SEQ = seq,
                        DEC_QTY = sum
                    };
                    //_iCCNService.CreateINS_RESULT_FINAL(data);
                }

                if (isSubmitted)
                {
                    #region QTS change code add user approve
                    _log.LogWrite("WriteNcrForProcessController - :217: Add Approver");
                    foreach (var item in model.UserApprove)
                    {
                        var UserApprove = new APPROVAL
                        {
                            UserId = item.UserId.Trim(),
                            RoleId = item.RoleId.Trim(),
                            CreateDate = DateTime.Now,
                            isActive = true,
                            NCR_NUMBER = ncr_hdr.NCR_NUM
                        };
                        aPPROVALs.Add(UserApprove);

                        #region QTS - Craete TaskList TaskDetail
                        _log.LogWrite($@"WriteNcrForProcessController - :231: ISENGINEERING('{item.RoleId.Trim()}')");
                        if (_IUserService.ISENGINEERING(item.RoleId.Trim()))
                        {
                            TaskManagementCreateModel createTaskNCRModel = new TaskManagementCreateModel();

                            //createTaskNCRModel.TaskList = new TASKLIST();
                            //createTaskNCRModel.TaskList.Topic = ncrnumauto;
                            //createTaskNCRModel.TaskList.TYPE = "NCR";
                            //createTaskNCRModel.TaskList.WRITEDATE = DateTime.Now;
                            //createTaskNCRModel.TaskList.WRITTENBY = User.Identity.GetUserId();

                            //createTaskNCRModel.TaskDetail = new TaskDetailViewModel();
                            //createTaskNCRModel.TaskDetail.TASKNAME = "Waiting Disposition";
                            //createTaskNCRModel.TaskDetail.OWNER = User.Identity.GetUserId();
                            //createTaskNCRModel.TaskDetail.ASSIGNEE = item.UserId.Trim();
                            //createTaskNCRModel.TaskDetail.STATUS = "Created";
                            //taskManagementCreateModels.Add(createTaskNCRModel);
                        }
                        #endregion
                    }
                    #endregion
                }

                var res = _INCRManagementService.CreateNCRForInProcess(ncr_hdr, nCR_DETs, nCR_EVIs, aPPROVALs, taskManagementCreateModels, 
                    vNMaterialTraceability, model.RECEIVER, INS_RESULT_FINAL);

                if (res.success)
                {
                    string EmailCreateConfig = ConfigurationManager.AppSettings["a"];
                    string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                    string role = EmailCreateConfig.Split('|')[0];

                    var arrRoleName = EmailCreateConfig.Split('|')[1].Split(';').ToArray();
                    List<AspNetUser> Users = _IUserService.GetUsersByRoleName(arrRoleName);
                    string MailTemplate = EmailCreateConfig.Split('|')[2];
                    string linkNCR = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = ncrnumauto.Trim() }, Request.Url.Scheme);
                    if (isSubmitted)
                    {
                        if (role == "ALL")
                        {
                            foreach (var mail in Users)
                            {
                                _emailService.SendEmailCreateNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: ncrnumauto.Trim(), linkNCR: linkNCR, comment: model.Comment);
                            }
                        }
                        else if (role == "NCR")
                        {

                            var arrApproverId = model.UserApprove.Select(x => x.UserId).ToArray();
                            var MailOfUser = Users.Where(x => arrApproverId.Contains(x.Id)).ToList();
                            foreach (var mail in MailOfUser)
                            {
                                _emailService.SendEmailCreateNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: ncrnumauto.Trim(), linkNCR: linkNCR, comment: model.Comment);
                            }

                            string ins2 = User.Identity.GetUserId();
                            var OPEOWN2 = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(ins2)).OPE;
                            var MailOPE2 = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(OPEOWN2));

                            _emailService.SendEmailCreateNCR(mailTemplate: "1", mailPath: path, RecipientName: MailOPE2.FullName, email: MailOPE2.Email, NCRnumber: ncrnumauto.Trim(), linkNCR: linkNCR, comment: model.Comment);

                        }
                    }
                    else
                    {
                        var IsOPE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE);
                        if (IsOPE)
                        {
                            AspNetUser opeu = _IUserService.GetUserById(User.Identity.GetUserId());
                            _emailService.SendEmailCreateNCR(MailTemplate, mailPath: path, RecipientName: opeu.FullName, email: opeu.Email, NCRnumber: ncrnumauto.Trim(), linkNCR: linkNCR, comment: model.Comment);
                        }
                        else
                        {
                            string ins = User.Identity.GetUserId();
                            var OPEOWN = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(ins)).OPE;
                            var MailOPE = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(OPEOWN));

                            _emailService.SendEmailCreateNCR(MailTemplate, mailPath: path, RecipientName: MailOPE.FullName, email: MailOPE.Email, NCRnumber: ncrnumauto.Trim(), linkNCR: linkNCR, comment: model.Comment);
                        }
                    }

                }

                return Json(new { res.success, message = res.message, obj = res.obj });
            }
            catch (Exception ex)
            {
                var _log = new LogWriter("SaveNCRForIQC" + ex.ToString());
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
                return Json(new { success = false, message = ex });
            }
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute + "-" + date.Second + "-" + date.Millisecond;
            virtualPath = relativePath + returnPath;
            if (!Directory.Exists(virtualPath))
                Directory.CreateDirectory(virtualPath);
            string FileName = file.FileName;
            if (Request.Browser.Browser.Contains("InternetExplorer") || Request.Browser.Browser.Contains("IE"))
            {
                FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
            }
            file.SaveAs(Path.Combine(virtualPath, FileName));

            

            return returnPath + "/" + FileName;
        }

        public JsonResult GetdropdownDefect(string ccn)
        {
            var list = _IReciverService.GetDropdownlist(ccn);
            return Json(list);
        }

        public JsonResult GetdropdownDefectForPro(string ncrnum ,string CCN)
        {
            var list = _IReciverService.GetDropdownlistDefectForPro(ncrnum,CCN);
            return Json(list);
        }

        public JsonResult GetdropdownDeCript(List<string> id,string ccn)
        {
            var listnc = _IReciverService.GetDropdownlistDecript1(id,ccn);
            return Json(listnc);
        }

        public ActionResult EditNCRForProcess(string ncrnum)
        {
            var obj = _INCRManagementService.GetCreateNCR(ncrnum);
            //double defective = 0;
            if (obj != null)
            {
                obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(ncrnum,obj.CCN);
                //foreach (var item in obj.Listdefectprocess)
                //{
                //    defective = Convert.ToDouble(item.QTY + defective);
                //}
                //obj.defect = defective;r
                //Get Uploaded Eividence
                obj.OldEvidence = _INCRManagementService.GetUploadedEvidence(ncrnum);
                for (int i = 0; i < obj.OldEvidence.Count; i++)
                {
                    var evi = obj.OldEvidence[i];
                    obj.SizeOfOldEvidence = UtilitiesService.Add(obj.SizeOfOldEvidence, GetSizeOfFile(evi.EVI_PATH));
                }
                // Get Uploaded VNMaterialTraceability
                obj.OldVNMaterialTraceability = _INCRManagementService.GetStringUploadedVNMaterialTraceability(ncrnum);
                ViewBag.SizeLength = obj.SizeOfOldEvidence.Sum();
                return View(obj);
            }
            else
            {
                return View(new NCRManagementViewModel());
            }
        }

        private long GetSizeOfFile(string eVI_PATH)
        {
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"] + eVI_PATH);
            FileInfo f = new FileInfo(relativePath);
            return f.Length;
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangeStatusToWaitForConfirm(string ncrnum)
        {
            try
            {
                bool chk = _INCRManagementService.UpdateStatusNCR(ncrnum, StatusInDB.WaitingForDisposition,"");
                if (chk)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult SaveListNewDefect(List<NCR_DET> model, string ncrnum)
        {
            if (/*model.Count == 0 || */model == null)
            {
                return Json(new { success = true });

            }
            else
            {
                try
                {
                    //   _INCRManagementService.DeleteAllDefectByNcrNum(ncrnum);
                    //   bool chk = _INCRManagementService.SaveNewDefect(model);
                    bool chk = _INCRManagementService.UpdateNCRDET(ncrnum, model);
                    if (chk)
                    {
                        _INCRManagementService.UpdateStatusNCR(model.ElementAt(0).NCR_NUM, StatusInDB.Created,"");
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }

        }
        [HttpPost]
        public ActionResult EditNcrProcess(NCRManagementViewModel data)
        {
            var _log = new LogWriter("EditNcrProcess");
            if (/*model.Count == 0 || */data == null)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }
            else
            {
                List<NCR_EVI> nCR_EVIs = new List<NCR_EVI>();
                List<NCR_DET> nCR_DETs = new List<NCR_DET>();
                VNMaterialTraceability vNMaterialTraceability = new VNMaterialTraceability();
                try
                {
                    if (data.VNMaterialTraceability != null)
                    {
                        _log.LogWrite("EditNcrProcess - :427 : SaveFile VNMaterialTraceability");
                        string File_Upload_Url = SaveFile(data.VNMaterialTraceability);
                        vNMaterialTraceability.Id = Guid.NewGuid().ToString();
                        vNMaterialTraceability.IsPrint = true;
                        vNMaterialTraceability.NCRNUM = data.NCR_NUM.Trim();
                        vNMaterialTraceability.VNMaterialTraceability1 = File_Upload_Url;
                        //logelse.LogWrite("WriteNCRForIQCController - :171 - VNMaterialTraceability: " + Environment.NewLine + JsonConvert.SerializeObject(VNMaterialTraceability));
                    }

                    if (data.ModelEvidence.Count > 0)
                    {
                        _log.LogWrite("EditNcrProcess - :438 : model.ModelEvidence: " + data.ModelEvidence.Count);
                        foreach (var item in data.ModelEvidence)
                        {
                            //logelse.LogWrite("WriteNCRForIQCController - :179 : SaveFile VNMaterialTraceability " + Environment.NewLine + JsonConvert.SerializeObject(item));
                            string File_Upload_Url = SaveFile(item.EvidenceFile);
                            NCR_EVI ncrevi = new NCR_EVI
                            {
                                EVI_PATH = File_Upload_Url,
                                NCR_NUM = data.NCR_NUM.Trim(),
                                SEC = "PROCESS",
                                IsPrint = true
                            };
                            nCR_EVIs.Add(ncrevi);
                            //_iCCNService.CreateNCR_EVI(ncrevi);
                        }
                    }

                    if (data.ListNCR_DET != null)
                    {
                        foreach (var item in data.ListNCR_DET)
                        {
                            string defect = String.Join("; ", item.DEFECT.ToArray());
                            string desc = String.Join("; ", item.NC_DESC.ToArray());
                            NCR_DET ncrdet = new NCR_DET
                            {
                                ITEM = item.ITEM.Trim(),
                                QTY = item.QTY,
                                SEC = NCRType.PROCESS,
                                REMARK = item.REMARK,
                                NCR_NUM = data.NCR_NUM.Trim(),
                                DEFECT = defect,
                                NC_DESC = desc
                            };
                            if (ncrdet.DEFECT != null && ncrdet.NC_DESC != null)
                            {
                                nCR_DETs.Add(ncrdet);
                            }

                        }
                    }

                    var res = _INCRManagementService.SaveEditNonComformity(nCR_DETs, data.NCR_NUM.Trim(), nCR_EVIs, vNMaterialTraceability, data.EVIID, NCRType.PROCESS);
                    return Json(new { res.success, res.message });
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    _log.LogWrite(ex.InnerException.Message);
                    return Json(new { success = false, message = "Exception- WriteNcrForProcess " + ex.InnerException.Message  });
                }
            }

        }
        [HttpPost]
        public ActionResult GetListLOTByWOAndPartnum(string wo, string partnum,string CCN)
        {
            try
            {
                List<string> lst = _INCRManagementService.GetListLOTByWOAndPartNum(wo, partnum);
                return Json(new { success = true, data = lst });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult GetListLOTBySerialAndPartnum(string serial, string partnum)
        {
            LogWriter log = new LogWriter("Start write log get list lot by sirial");
            try
            {
                List<string> lst = _INCRManagementService.GetListSerialByWOAndPartNum(serial, partnum);
                return Json(new { success = true, data = lst });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult GetListPOByLOT(string lot,string pn,string CCN)
        {
            LogWriter log = new LogWriter("Start write log list po by lot");
          if(lot == "null")
            {
                lot = "";
            }
            try
            {
             // if(lot == null) { }
                List<string> lst = _INCRManagementService.GetListPOByLOT(lot.Trim(),pn,CCN);
                return Json(new { success = true, data = lst });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
        public ActionResult GetLOTbyPN(string pn,string CCN)
        {
            LogWriter log = new LogWriter("GetLOTbyPN - Start write log list po by lot");
            try
            {
              var lot = _INCRManagementService.GetListLOTByPN(pn,CCN);
                return Json(new { success = true, data = lot });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }

        public ActionResult GetSamplingSize(double lotsize)
        {
            float res = 0;
            res = _INCRManagementService.GetSamplingSize(lotsize);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //tuanlua29-01-2019

        [HttpPost]
        public ActionResult GetDescriptionbyPartNum(string partnum,string ccn)
        {
            LogWriter log = new LogWriter("Start write log get list lot by sirial");
            try
            {
                var lst = _INCRManagementService.GetDescriptionbyPartNum( partnum,ccn);
                return Json(new { success = true, result = lst });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
    }
}
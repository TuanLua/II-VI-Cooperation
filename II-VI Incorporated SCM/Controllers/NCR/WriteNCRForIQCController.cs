using System.Collections.Generic;
using System.Web.Mvc;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Web;
using System.IO;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Microsoft.AspNet.Identity;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class WriteNCRForIQCController : Controller
    {
        private readonly IReciverService _IReciverService;
        private readonly ICCNService _iCCNService;
        private readonly IUserService _IUserService;
        private readonly INCRManagementService _INCRManagementService;
        private readonly IEmailService _emailService;

        public WriteNCRForIQCController(IReciverService IReciverService, ICCNService iCCNService, IUserService IUserService, INCRManagementService INCRManagementService, IEmailService emailService)
        {
            _IReciverService = IReciverService;
            _iCCNService = iCCNService;
            _IUserService = IUserService;
            _INCRManagementService = INCRManagementService;
            _emailService = emailService;
        }

        // GET: WriteNCRForIQC0


        public JsonResult GetdropdownDecriptT()
        {
            var list = _IReciverService.GetDropdowndecript();
            return Json(list);
        }

        public JsonResult GetdropdownDeCript(List<string> id,string ccn)
        {
            var listnc = _IReciverService.GetDropdownlistDecript1(id,ccn);
            return Json(listnc);
        }

        public JsonResult GetDropdownlistIQC(List<string> id)
        {
            var listnc = _IReciverService.GetDropdownlistIQC(id);
            return Json(listnc);
        }

        [Authorize(Roles = "CreateNCR")]
        public ActionResult CreateNCR(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.ID_CCN = id;
                ViewBag.IsOPE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE);
                List<SelectList> List = new List<SelectList>();
                SelectList ListQTYASS = new SelectList(_IUserService.GetListUserQTYASS("QUALITY ASSURANCE"), "Id", "FullName");
                SelectList ListENGINEERING = new SelectList(_IUserService.GetListUserQTYASS("ENGINEERING"), "Id", "FullName");
                SelectList ListAFGASSYE = new SelectList(_IUserService.GetListUserQTYASS("AFG/ASSYE"), "Id", "FullName");
                SelectList ListPURCHASING = new SelectList(_IUserService.GetListUserQTYASS("PURCHASING"), "Id", "FullName");
                List.Add(ListQTYASS);
                List.Add(ListENGINEERING);
                List.Add(ListAFGASSYE);
                List.Add(ListPURCHASING);
                ViewBag.List = List;
                ViewBag.ListRole = new SelectList(_IUserService.GetSelectListAllRole(), "Id", "Name");
                ViewBag.ENGINEERINGRoleId = _IUserService.GetENGINEERINGRoleId();
                ViewBag.VNMaterialTraceabilityLink = ConfigurationManager.AppSettings["VNMaterialTraceabilityLink"];
                ViewBag.CHAIRMANRoleId = _IUserService.GetCHAIRMANRoleId();
                // ViewBag.Listdefect = _INCRManagementService.GetListInresultIqc(receiver);
                return View();
            }
            return RedirectToAction("index", "NCR");

        }

        [HttpPost]
        public JsonResult SaveNCRForIQC(NCRInIQCViewModel model)
        {
            var log = new LogWriter("WriteNCRForIQCController - SaveNCRForIQC : 85");
            //log.LogWrite(JsonConvert.SerializeObject(model));
            Random rnd = new Random();
            DateTime? dateSample;
            dateSample = null;
            double sumllns = 0;
            log.LogWrite("WriteNCRForIQCController - :90 - begin CheckDubplicateNCRCreate");
            var ncrcheck = _INCRManagementService.CheckDubplicateNCRCreate(model.RECEIVER1);
            log.LogWrite("WriteNCRForIQCController - :92 - end CheckDubplicateNCRCreate : " + ncrcheck);
            if (ncrcheck)
            {
                return Json(new { success = false, message = "NCR have existed into this receiver: " + model.RECEIVER1 });
            }
            else
            {
                var logelse = new LogWriter("WriteNCRForIQCController - :101 ---------------Log Else------------------");
                var iduser = User.Identity.GetUserId();
                string ncrnumauto = _INCRManagementService.GetAutoNCRNUM("I");
                logelse.LogWrite("WriteNCRForIQCController - :90 - end CheckDubplicateNCRCreate" + ncrnumauto);

                var ReceicerResult = _IReciverService.GetListReciver(model.RECEIVER1, model.MI_PART_NO1,model.CCN1);
                logelse.LogWrite("WriteNCRForIQCController - :106 - end GetListReciver");
                //logelse.LogWrite(JsonConvert.SerializeObject(ReceicerResult));

                var listpartial = _IReciverService.GetListPartial(model.RECEIVER1);
                logelse.LogWrite("WriteNCRForIQCController - :109 - end GetListPartial");
                //logelse.LogWrite(JsonConvert.SerializeObject(listpartial));

                foreach (var item in listpartial)
                {
                    sumllns = Convert.ToDouble(item.PartialIns + sumllns);
                }
                var isSubmitted = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE)
                                  & _IUserService.ISENGINEERING(model.UserApprove);
                logelse.LogWrite("WriteNCRForIQCController - :120 - isSubmitted: " + isSubmitted);

                try
                {
                    logelse.LogWrite("WriteNCRForIQCController - :125 - Begin : ");
                    List<NCR_EVI> nCR_EVIs = new List<NCR_EVI>();
                    List<NCR_DET> nCR_DETs = new List<NCR_DET>();
                    List<APPROVAL> aPPROVALs = new List<APPROVAL>();
                    List<TaskManagementCreateModel> taskManagementCreateModels = new List<TaskManagementCreateModel>();
                    VNMaterialTraceability vNMaterialTraceability = new VNMaterialTraceability();
                    NCR_HDR ncr_hdr = new NCR_HDR
                    {
                        AQL = model.AQL_VISUAL1.Trim(),
                        SAMPLE_INSP = model.AQL_VISUAL1 == AQLText.AQLinDB ? true : false,
                        PERCENT_INSP = model.AQL_VISUAL1 == AQLText.AQL100 ? true : false,
                        LEVEL = ReceicerResult.SKIP_LEVEL,
                        LOT = model.LOT1.Trim(),
                        MI_PART_NO = model.MI_PART_NO1.Trim(),
                        CCN = model.CCN1.Trim(),
                        PO_NUM = model.PO_NUM1.Trim(),
                        RECEIVER = model.RECEIVER1.Trim(),
                        REC_QTY = model.REC_QTY1,
                        REJ_QTY = model.REJ_QTY1,
                        INS_QTY = sumllns,
                        SEC = NCRType.IQC,
                        STATUS = isSubmitted ? StatusInDB.Submitted : StatusInDB.Created,
                        DATESUBMIT = isSubmitted ? DateTime.Now : dateSample,
                        USERSUBMIT = isSubmitted ? iduser : null,
                        TYPE_NCR = NCRType.IQC,
                        NCR_NUM = ncrnumauto.Trim(),
                        INS_DATE = DateTime.Now,
                        INSPECTOR = User.Identity.GetUserId(),
                        MODEL_NO = ReceicerResult.MODEL_NO,
                        VENDOR = ReceicerResult.VENDOR,
                        VEN_NAME = ReceicerResult.VEN_NAME,
                        ITEM_DESC = ReceicerResult.ITEM_DESC,
                        VEN_ADD = ReceicerResult.VEN_ADD,
                        DRAW_REV = ReceicerResult.DRAW_REV,
                        ZIP_CODE = ReceicerResult.ZIP,
                        FIRST_ARTICLE = model.FIRST_ARTICLE1,
                        CITY = ReceicerResult.CTRY,
                        STATE = ReceicerResult.STATE,
                        DEFECTIVE = model.defective,
                        Comment = model.Comment
                    };
                    //logelse.LogWrite("WriteNCRForIQCController - :160 : " + Environment.NewLine + JsonConvert.SerializeObject(ncr_hdr));
                    if (model.VNMaterialTraceability != null)
                    {
                        logelse.LogWrite("WriteNCRForIQCController - :163 : SaveFile VNMaterialTraceability");
                        string File_Upload_Url = SaveFile(model.VNMaterialTraceability);
                        vNMaterialTraceability.Id = Guid.NewGuid().ToString();
                        vNMaterialTraceability.IsPrint = true;
                        vNMaterialTraceability.NCRNUM = ncrnumauto.Trim();
                        vNMaterialTraceability.VNMaterialTraceability1 = File_Upload_Url;
                       // SizeOfFile += model.VNMaterialTraceability.ContentLength;
                        //logelse.LogWrite("WriteNCRForIQCController - :171 - VNMaterialTraceability: " + Environment.NewLine + JsonConvert.SerializeObject(VNMaterialTraceability));
                    }

                    if (model.ModelEvidence != null || model.ModelEvidence.Count > 0)
                    {
                        logelse.LogWrite("WriteNCRForIQCController - :176 : model.ModelEvidence: " + model.ModelEvidence.Count);
                        foreach (var item in model.ModelEvidence)
                        {
                            //logelse.LogWrite("WriteNCRForIQCController - :179 : SaveFile VNMaterialTraceability " + Environment.NewLine + JsonConvert.SerializeObject(item));
                            if(item != null)
                            {
                                string File_Upload_Url = SaveFile(item.EvidenceFile);
                               // SizeOfFile += item.EvidenceFile.ContentLength;
                                NCR_EVI ncrevi = new NCR_EVI
                                {
                                    EVI_PATH = File_Upload_Url,
                                    NCR_NUM = ncr_hdr.NCR_NUM.Trim(),
                                    SEC = ncr_hdr.SEC.Trim(),
                                    IsPrint = true
                                };
                                nCR_EVIs.Add(ncrevi);
                            }
                            //_iCCNService.CreateNCR_EVI(ncrevi);
                        }
                    }

                    if (model.nonComformity != null)
                    {
                        logelse.LogWrite("WriteNCRForIQCController - :195 : model.nonComformity: " + model.ModelEvidence.Count);
                        foreach (var item in model.nonComformity)
                        {
                            //logelse.LogWrite("WriteNCRForIQCController - :198 : model.nonComformity - item: " + Environment.NewLine + JsonConvert.SerializeObject(item));
                            string defect = "", desc = "";

                            if (item.DEFECT != null)
                            {
                                foreach (var b in item.DEFECT)
                                {
                                    List<string> list = new List<string>();
                                    var tmp = b.Split(',');
                                    defect = String.Join("; ", tmp.ToArray());
                                }


                            }
                            if (item.NC_DESC != null)
                            {
                                foreach (var a in item.NC_DESC)
                                {
                                    List<string> list = new List<string>();
                                    var temp = a.Split(',');
                                    desc = String.Join("; ", temp.ToArray());
                                }
                                //desc  = String.Join("; ", item.NC_DESC.ToArray());
                            }
                            logelse.LogWrite("WriteNCRForIQCController - :222");
                            NCR_DET ncrdet = new NCR_DET
                            {
                                ITEM = item.ITEM.Trim(),
                                QTY = item.QTY,
                                SEC = NCRType.IQC,
                                REMARK = item.REMARK,
                                NCR_NUM = ncr_hdr.NCR_NUM.Trim(),
                                DEFECT = defect,
                                NC_DESC = desc
                            };
                            nCR_DETs.Add(ncrdet);
                            //_iCCNService.CreateNCR_DET(ncrdet);
                        }
                        //logelse.LogWrite("WriteNCRForIQCController - :236 - nCR_DETs: " + Environment.NewLine + JsonConvert.SerializeObject(nCR_DETs));
                    }

                    logelse.LogWrite("WriteNCRForIQCController - :239 - CheckReceiverForIQC");
                    if (!_iCCNService.CheckReceiverForIQC(ncr_hdr)) return Json(new { success = false, message = "Check Receiver" });

                    //_iCCNService.CreateNCR_HDR(ncr_hdr);

                    if (isSubmitted)
                    {
                        #region QTS change code add user approve
                        logelse.LogWrite("WriteNCRForIQCController - :247: Add Approver");
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
                            logelse.LogWrite($@"WriteNCRForIQCController - :262: ISENGINEERING('{item.RoleId.Trim()}')");
                            if (_IUserService.ISENGINEERING(item.RoleId.Trim()))
                            {
                                TaskManagementCreateModel createTaskNCRModel = new TaskManagementCreateModel();

                               // createTaskNCRModel.TaskList = new TASKLIST();
                               // createTaskNCRModel.TaskList.Topic = ncrnumauto;
                               // createTaskNCRModel.TaskList.TYPE = "NCR";
                              //  createTaskNCRModel.TaskList.WRITEDATE = DateTime.Now;
                             //   createTaskNCRModel.TaskList.WRITTENBY = User.Identity.GetUserId();

                              //  createTaskNCRModel.TaskDetail = new TaskDetailViewModel();
                              //  createTaskNCRModel.TaskDetail.TASKNAME = "Waiting Disposition";
                              //  createTaskNCRModel.TaskDetail.OWNER = User.Identity.GetUserId();
                             //   createTaskNCRModel.TaskDetail.ASSIGNEE = item.UserId.Trim();
                              //  createTaskNCRModel.TaskDetail.STATUS = "New";
                             //   taskManagementCreateModels.Add(createTaskNCRModel);
                                //_INCRManagementService.CreateTaskManNCR(createTaskNCRModel);
                            }
                            #endregion
                        }
                        #endregion
                    }

                    //if(model.RECEIVER1 != null && model.RECEIVER1 != "")
                    //{
                    //    _iCCNService.UpdateReciever_Defect(model.RECEIVER1, ncrnumauto);
                    //}

                    var res = _INCRManagementService.CreateNCRForIQC(ncr_hdr, nCR_DETs, nCR_EVIs, aPPROVALs, taskManagementCreateModels, vNMaterialTraceability, model.RECEIVER1.Trim());
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
                            }
                        }
                        else
                        {
                            var IsOPE =  _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE);
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
                    return Json(new { success = false, message = "Error. Exception" });
                }
            }
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute + "-" + date.Second + "-" + date.Millisecond;
            virtualPath = relativePath + returnPath;
            //  string FolderPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);
            //  string FolderPath = virtualPath;
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
        [HttpPost]
        public JsonResult GetDetailNCRForIqcSearch(string reciver, string partnum, string po, string lot,string CCN)
        {
            if (reciver.HasValue())
            {
                var obj = _IReciverService.GetListReciver(reciver, partnum,CCN);
                var AQLCheck = "";
                var AQLMS = "";
                double SPL = 0;
                double SPM = 0;
                double sumllns = 0;
                double sumrej = 0;
                double QTY = 0;
                double DEFECT = 0;
                var cs = _IReciverService.GetCS(reciver);
                var change = _IReciverService.GetChangedSpl(reciver);
                var listpartial = _IReciverService.GetListPartial(reciver);
                var defectqty = _IReciverService.GetlistDefectsQty(reciver);
                foreach (var item in listpartial)
                {
                    sumllns = Convert.ToDouble(item.PartialIns + sumllns);
                    sumrej = Convert.ToDouble(item.PartialRej + sumrej);
                }
                foreach (var tmp in defectqty)
                {
                    DEFECT = tmp.NC_Qty + DEFECT;

                }
                if (change != null && cs!=null)
                {
                    AQLCheck = change.AQL_VISUAL;
                    AQLMS = change.AQL_MEASURE;
                    SPL = change.SAMPLING_VISUAL;
                    SPM = change.SAMPLING_MEASURE;
                    QTY = cs.QTY;
                }
                if (cs == null )
                {
                    QTY = 0;
                }
                else if(cs != null && change == null)
                {
                    AQLCheck = cs.AQL_VISUAL;
                    AQLMS = cs.AQL_MEASURE;
                    SPL = cs.SAMPLING_VISUAL;
                    SPM = cs.SAMPLING_MEASURE;
                    QTY = cs.QTY;
                }
                if (obj != null)
                {
                    var nc_qty = obj.NC_Qty;
                    var ncr_num = obj.NCR_Num;
                    if (nc_qty > 0 && ncr_num == null)
                    {
                        ReciverViewmodel model = new ReciverViewmodel();
                        model.RECEIVER1 = obj.RECEIVER1;
                        model.LOT = obj.LOT;
                        model.ITEM = obj.ITEM;
                        model.PO_NUM = obj.PO_NUM;
                        model.AQL_MEASURE = AQLMS;
                        model.AQL_VISUAL = AQLCheck;
                        model.REJ_QTY = sumrej;
                        model.INS_QTY = sumllns;
                        model.REC_QTY = QTY;
                        model.SAMPLING_MEASURE = SPL;
                        model.SAMPLING_VISUAL = SPM;
                        model.VEN_NAME = obj.VEN_NAME;
                        model.VENDOR = obj.VENDOR;
                        model.ITEM_DESC = obj.ITEM_DESC;
                        //model.AQL = obj.AQL;
                        model.DRAW_REV = obj.DRAW_REV;
                        model.defective = DEFECT;
                        //    model.Listdefect = _IReciverService.GetInresult(reciver);
                        model.Listdefect = _INCRManagementService.GetListInresultIqc(reciver);
                        // model.Listdefect = _INCRManagementService.GetListInresultIqc(reciver);
                        return Json(new { success = true, model });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Unable to create NCR with your inputting information!" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please check your inputting information! " + reciver +" or " + partnum });
                }
            }
            else
            {
                var obj = _IReciverService.GetListReciverByPO(po, partnum, lot,CCN);

                if (obj == null)
                {
                    return Json(new { success = false, message = "Please check your inputting information!" });
                }
                var AQLCheck = "";
                var AQLMS = "";
                double SPL = 0;
                double SPM = 0;
                double sumllns = 0;
                double sumrej = 0;
                double QTY = 0;
                double DEFECT = 0;
                var cs = _IReciverService.GetCS(obj.RECEIVER1);
                var change = _IReciverService.GetChangedSpl(obj.RECEIVER1);
                var listpartial = _IReciverService.GetListPartial(obj.RECEIVER1);
                var defectqty = _IReciverService.GetlistDefectsQty(obj.RECEIVER1);
                foreach (var item in listpartial)
                {
                    sumllns = Convert.ToDouble(item.PartialIns + sumllns);
                    sumrej = Convert.ToDouble(item.PartialRej + sumrej);
                }
                foreach (var tmp in defectqty)
                {
                    DEFECT = tmp.NC_Qty + DEFECT;

                }
                if (change != null && cs!=null)
                {
                    AQLCheck = change.AQL_VISUAL;
                    AQLMS = change.AQL_MEASURE;
                    SPL = change.SAMPLING_VISUAL;
                    SPM = change.SAMPLING_MEASURE;
                    QTY = cs.QTY;
                }
                if (cs == null)
                {
                    QTY = 0;
                }
                else if (cs != null && change == null)
                {
                    AQLCheck = cs.AQL_VISUAL;
                    AQLMS = cs.AQL_MEASURE;
                    SPL = cs.SAMPLING_VISUAL;
                    SPM = cs.SAMPLING_MEASURE;
                    QTY = cs.QTY;
                }
                if (obj != null)
                {
                    var nc_qty = obj.NC_Qty;
                    var ncr_num = obj.NCR_Num;
                    if (nc_qty > 0 && ncr_num == null)
                    {
                        ReciverViewmodel model = new ReciverViewmodel();
                        model.RECEIVER1 = obj.RECEIVER1;
                        model.LOT = obj.LOT;
                        model.ITEM = obj.ITEM;
                        model.PO_NUM = obj.PO_NUM;
                        model.AQL_MEASURE = AQLMS;
                        model.AQL_VISUAL = AQLCheck;
                        model.REJ_QTY = sumrej;
                        model.INS_QTY = sumllns;
                        model.REC_QTY = QTY;
                        model.SAMPLING_MEASURE = SPL;
                        model.SAMPLING_VISUAL = SPM;
                        model.VEN_NAME = obj.VEN_NAME;
                        model.VENDOR = obj.VENDOR;
                        model.ITEM_DESC = obj.ITEM_DESC;
                        //model.AQL = obj.AQL;
                        model.DRAW_REV = obj.DRAW_REV;
                        model.defective = DEFECT;
                        model.Listdefect = _INCRManagementService.GetListInresultIqc(obj.RECEIVER1);
                        return Json(new { success = true, model });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Unable to create NCR with your inputting information!"
                        });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please check your inputting information!" });
                }
            }
        }

        public ActionResult ChooseUserSubmit(string ncrnumber, string userquality, string userengineer, string stt,
            string userafg, string userpurchange, string comment)
        {
            try
            {
                APPROVAL model = new APPROVAL();
                model.ENGIEERING = userengineer;
                model.PURCHASING = string.IsNullOrWhiteSpace(userpurchange) ? null : userpurchange;
                model.QUALITY = userquality;
                model.MFG = string.IsNullOrWhiteSpace(userafg) ? null : userafg;
                model.NCR_NUMBER = ncrnumber;
                var iduser = (User.Identity.GetUserId());
                var chek = _INCRManagementService.AddUserApproval(model);
                if (!chek)
                {
                    return Json(new { result = false });
                }

                chek = _INCRManagementService.UpdateStatusNCR(ncrnumber, StatusInDB.Submitted, iduser);

                //Add TaskList TaskDetail. By: Sil. Date: 06/27/2018
                //TaskManagementCreateModel createTaskNCRModel = new TaskManagementCreateModel();

                //createTaskNCRModel.TaskList = new TASKLIST();
                //createTaskNCRModel.TaskList.Topic = ncrnumber;
                //createTaskNCRModel.TaskList.TYPE = "NCR";
                //createTaskNCRModel.TaskList.WRITEDATE = DateTime.Now;
                //createTaskNCRModel.TaskList.WRITTENBY = User.Identity.GetUserId();

                //createTaskNCRModel.TaskDetail = new TaskDetailViewModel();
                //createTaskNCRModel.TaskDetail.TASKNAME = "Waiting For Disposition";
                //createTaskNCRModel.TaskDetail.OWNER = User.Identity.GetUserId();
                //createTaskNCRModel.TaskDetail.ASSIGNEE = userengineer;
                //createTaskNCRModel.TaskDetail.STATUS = "New";
                //_INCRManagementService.CreateTaskManNCR(createTaskNCRModel);
                //if (!string.IsNullOrEmpty(comment))
                //{
                //    var lstUser = _IUserService.GetAllUserByRole("ApproveNCR");
                //    foreach (var user in lstUser)
                //    {
                //        _emailService.SendEmailSubmitWithoutMeeting(user.Email, $"NCR {ncrnumber} had submited! <br/> {comment}");
                //    }

                //}
                if (!chek)
                {
                    return Json(new { result = false });
                }

                return Json(new { result = true });
            }
            catch(Exception ex)
            {
                new LogWriter("ChooseUserSubmit").LogWrite(ex.ToString());
                return Json(new { result = false });
            }
        }
        public ActionResult ChooseUserSubmitConfim(string ncrnumber, string userquality, string userengineer,
            string userafg, string userpurchange)
        {
            var chek = _INCRManagementService.UpdateStatusNCR(ncrnumber, StatusInDB.WaitingForDisposition,"");

            if (!chek)
            {
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        public ActionResult ChooseUserSubmitVoid(string ncrnumber, string userquality, string userengineer,
            string userafg, string userpurchange)
        {
            var chek = _INCRManagementService.UpdateStatusNCR(ncrnumber, StatusInDB.Void, User.Identity.GetUserId());

            if (!chek)
            {
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }
        public ActionResult ChooseUserSubmitDispo(string ncrnumber, string userquality, string userengineer,
            string userafg, string userpurchange)
        {
            var chek = _INCRManagementService.UpdateStatusNCR(ncrnumber, StatusInDB.DispositionApproved,"");

            if (!chek)
            {
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public ActionResult GetDataInputIQCInProcessByPONumAndPartNum(string po, string lot ,string pn,string CCN)
        {
            var _log = new LogWriter($@"WriteNCRForIQCController : 629 - GetDataInputIQCInProcessByPONumAndPartNum ( {po}, {lot}, {pn})");
            try
            {
                _log.LogWrite("_INCRManagementService.GetDataInputIQCInProcessByPONumAndLOT");
                var result = _INCRManagementService.GetDataInputIQCInProcessByPONumAndLOT(po, lot.Trim(),pn,CCN);
                return PartialView("~/Views/WriteNCRForProcess/_TablesearchLOTPO.cshtml", result);
            }
            catch (Exception ex)
            {
                _log.LogWrite("WriteNCRForIQCController - GetDataInputIQCInProcessByPONumAndPartNum: " + Environment.NewLine + ex.ToString());
                return PartialView("~/Views/WriteNCRForProcess/_TablesearchLOTPO.cshtml", new List<NcrSearchViewModelProcess>());
            }
        }

        [HttpPost]
        public ActionResult GetDataInputIQCInProcessByLOTAndPartNum(string lot, string partnum,string CCN)
        {
            try
            {
                var result = _INCRManagementService.GetDataInputIQCInProcessByLOTAndPartNum(lot, partnum,CCN);
                //if(result.)
                return PartialView("~/Views/WriteNCRForProcess/_TableSearchByPOPartial.cshtml", result);
            }
            catch (Exception)
            {
                return PartialView("~/Views/WriteNCRForProcess/_TableSearchByPOPartial.cshtml", new List<NcrSearchViewModelProcess>());
            }
        }
        //[HttpPost]
        //public JsonResult SentMailRequired(string userquality, string userengineer, string userafg, string userpurchange,string ncrnumber)
        //{

        //    bool a = _INCRManagementService.SentMailRemind(userquality, userengineer, userafg, userpurchange, ncrnumber);
        //    return Json(new { success = true });
        //}

        public JsonResult Getnamebyid(string id)
        {
            var name = _IUserService.GetNameById(id);
            return Json(name);
        }

        [HttpPost]
        public ActionResult SaveEdiNCR(NCRManagementViewModel data)
        {
            var _log = new LogWriter("SaveEdiNCR");
            if (data == null)
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
                        _log.LogWrite("SaveEdiNCR - :427 : SaveFile VNMaterialTraceability");
                        string File_Upload_Url = SaveFile(data.VNMaterialTraceability);
                        vNMaterialTraceability.Id = Guid.NewGuid().ToString();
                        vNMaterialTraceability.IsPrint = true;
                        vNMaterialTraceability.NCRNUM = data.NCR_NUM.Trim();
                        vNMaterialTraceability.VNMaterialTraceability1 = File_Upload_Url;
                        //logelse.LogWrite("WriteNCRForIQCController - :171 - VNMaterialTraceability: " + Environment.NewLine + JsonConvert.SerializeObject(VNMaterialTraceability));
                    }

                    if (data.ModelEvidence.Count > 0)
                    {
                        _log.LogWrite("SaveEdiNCR - :438 : model.ModelEvidence: " + data.ModelEvidence.Count);
                        foreach (var item in data.ModelEvidence)
                        {
                            //logelse.LogWrite("WriteNCRForIQCController - :179 : SaveFile VNMaterialTraceability " + Environment.NewLine + JsonConvert.SerializeObject(item));
                            string File_Upload_Url = SaveFile(item.EvidenceFile);
                            NCR_EVI ncrevi = new NCR_EVI
                            {
                                EVI_PATH = File_Upload_Url,
                                NCR_NUM = data.NCR_NUM.Trim(),
                                SEC = "IQC",
                                IsPrint = true
                            };
                            nCR_EVIs.Add(ncrevi);
                            //_iCCNService.CreateNCR_EVI(ncrevi);
                        }
                    }
                    if (data.ListNCR_DET != null)
                    {
                        _log.LogWrite("SaveEdiNCR - :195 : model.nonComformity: " + data.ModelEvidence.Count);
                        foreach (var item in data.ListNCR_DET)
                        {
                            //logelse.LogWrite("WriteNCRForIQCController - :198 : model.nonComformity - item: " + Environment.NewLine + JsonConvert.SerializeObject(item));
                            string defect = "", desc = "";

                            if (item.DEFECT != null)
                            {
                                foreach (var b in item.DEFECT)
                                {
                                    List<string> list = new List<string>();
                                    var tmp = b.Split(',');
                                    defect = String.Join("; ", tmp.ToArray());
                                }


                            }
                            if (item.NC_DESC != null)
                            {
                                foreach (var a in item.NC_DESC)
                                {
                                    List<string> list = new List<string>();
                                    var temp = a.Split(',');
                                    desc = String.Join("; ", temp.ToArray());
                                }
                                //desc  = String.Join("; ", item.NC_DESC.ToArray());
                            }
                            _log.LogWrite("SaveEdiNCR - :222");
                            NCR_DET ncrdet = new NCR_DET
                            {
                                ITEM = item.ITEM.Trim(),
                                QTY = item.QTY,
                                SEC = NCRType.IQC,
                                REMARK = item.REMARK,
                                NCR_NUM = data.NCR_NUM.Trim(),
                                DEFECT = defect,
                                NC_DESC = desc
                            };
                            nCR_DETs.Add(ncrdet);
                            //_iCCNService.CreateNCR_DET(ncrdet);
                        }
                        
                    }

                    var res = _INCRManagementService.SaveEditNonComformity(nCR_DETs, data.NCR_NUM, nCR_EVIs, vNMaterialTraceability, data.EVIID, NCRType.IQC);
                    return Json(new { res.success, res.message });
                }
                catch(Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    return Json(new { success = false, message = "Exception- WriteNcrForProcess :" });
                }
            }
        }
    }
}
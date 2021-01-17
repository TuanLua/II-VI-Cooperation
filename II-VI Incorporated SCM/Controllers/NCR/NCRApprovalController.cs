using EvoPdf;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class NCRApprovalController : Controller
    {
        private readonly INCRManagementService _INCRManagementService;
        private readonly IReciverService _IReciverService;
        private readonly IUserService _IUserService;
        private readonly ITaskManagementService _iTaskManagementService;
        private readonly IEmailService _emailService;
        private readonly LogWriter log = new LogWriter("");

        public NCRApprovalController(INCRManagementService INCRManagementService, IReciverService IReciverService, IUserService IUserService, ITaskManagementService iTaskManagementService, IEmailService emailService)
        {
            _INCRManagementService = INCRManagementService;
            _IReciverService = IReciverService;
            _IUserService = IUserService;
            _iTaskManagementService = iTaskManagementService;
            _emailService = emailService;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewApproval(string NCR_NUM, string Status, bool Pre = false)
        {
            if (string.IsNullOrEmpty(NCR_NUM))
            {
                return RedirectToAction("Index");
            }

            ViewBag.STT = Status;
            ViewBag.IsOPE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE);
            ViewBag.IsMRBTeam = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.MRBTeam);
            ViewBag.IsWHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            ViewBag.IsSQE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.SQE);
            ViewBag.IsBuyer = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Buyer);
            ViewBag.IsChairm = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            //ViewBag.ListMRBTeam = new SelectList(_IUserService.GetListUserByGroupName(UserGroup.MRBTeam), "Id", "FullName");
            ViewBag.ListRole = new SelectList(_IUserService.GetSelectListAllRole(), "Id", "Name");
            ViewBag.ListRole2 = _IUserService.GetSelectListAllRole().Select(x => new SelectListItem
            {
                Value = x.Id.Trim(),
                Text = (x.Name.Trim())
            }).ToList();
            ViewBag.ListUser = _IUserService.GetAllUserWithRole();
            ViewBag.ENGINEERINGRoleId = _IUserService.GetENGINEERINGRoleId();
            ViewBag.MRBRoleId = _IUserService.GetMRBRoleId();
            ViewBag.CHAIRMANRoleId = _IUserService.GetCHAIRMANRoleId();
            ViewBag.VNMaterialTraceabilityLink = ConfigurationManager.AppSettings["VNMaterialTraceabilityLink"];

            ViewBag.isOPEOwner = _INCRManagementService.GetOPEOwner(User.Identity.GetUserId(), NCR_NUM);

            NCRManagementViewModel obj = _INCRManagementService.GetCreateNCR(NCR_NUM);

             obj.ListRespon = _INCRManagementService.getListRESPON();
             obj.ListDispo = _INCRManagementService.getListDispo();
            //if (obj.PERCENT_INSP == true )
            //{
            //    obj.ListAddition = _INCRManagementService.getListAddition();
            //    obj.ListAddition.RemoveAt(obj.ListAddition.Count - 2);
            //   // obj.ListRespon = _INCRManagementService.getListRESPON();
            //   // obj.ListRespon.RemoveAt(obj.ListAddition.Count - 1);
            //  //  obj.ListDispo = _INCRManagementService.getListDispo();
            //   // obj.ListDispo.RemoveAt(obj.ListAddition.Count - 1);
            //}
            //else
            //{
            //    obj.ListAddition = _INCRManagementService.getListAddition();
            //   // obj.ListRespon = _INCRManagementService.getListRESPON();
            //   // obj.ListDispo = _INCRManagementService.getListDispo();
            //}

            //Thi.nguyen 10.10.2020: Add remove "Return to Vendor" when NCR is floorNCR 
            //and move code "obj.ListAddition = _INCRManagementService.getListAddition();" on the top
            obj.ListAddition = _INCRManagementService.getListAddition();
            if (obj.PERCENT_INSP == true)
                obj.ListAddition.RemoveAt(obj.ListAddition.Count - 1);

            if (_INCRManagementService.CheckFloorNCR(obj.NCR_NUM))
            {
                obj.ListAddition.RemoveAll(x => x.NAME.Contains("RETURN"));
                obj.ListDispo.RemoveAll(x => x.NAME.Contains("RETURN"));
                obj.ListRespon.RemoveAll(x => x.NAME.Contains("VENDOR"));
            }

            ViewBag.Fullname = _IUserService.GetNameById(User.Identity.GetUserId());
            ViewBag.ShowBtnChangeSubmitItem = _IUserService.CheckIsApprover(User.Identity.GetUserId(), NCR_NUM.Trim());
            ViewBag.SCRAPCategory = new SelectList(_INCRManagementService.GetSCRAPCategory(), "Category", "Category");


            if (obj != null)
            {
                string strCAREQUESTNO = "", CAREQUESTNO_template = @"; <a target='_blank' id='{0}' href='{1}'>{2}</a>";
                string[] CAREQUESTNO = _INCRManagementService.GetCARequestNO(NCR_NUM);
                foreach (string item in CAREQUESTNO)
                {
                    strCAREQUESTNO += string.Format(CAREQUESTNO_template, item, Url.Action("ViewSCAR", "SCAR", new { scarid = item }), item);
                }
                obj.ISSUED_REQUEST_NO = strCAREQUESTNO.Length > 1 ? strCAREQUESTNO.Remove(0, 1) : "";
                ViewBag.IsMegerNCR = _INCRManagementService.IsMegerNCR(obj.MI_PART_NO, obj.LOT, obj.CCN);
                //Get Uploaded Eividence
                obj.OldEvidence = _INCRManagementService.GetUploadedEvidence(NCR_NUM);
                // Get Uploaded VNMaterialTraceability
                obj.OldVNMaterialTraceability = _INCRManagementService.GetStringUploadedVNMaterialTraceability(NCR_NUM);

                ViewBag.Status = obj.STATUS.Trim();
                ViewBag.SubmitConfirm = Pre;
                ViewBag.allowSCRAP = _INCRManagementService.CheckScrap(NCR_NUM);
                ViewBag.SCRAPMONEY = ConfigurationManager.AppSettings["SCRAPMONEY"];
                ViewBag.RootPath = Url.Content(ConfigurationManager.AppSettings["uploadPath"]);
                VNMaterialTraceability VNMP = _INCRManagementService.GetVNMaterialTraceabilityByID(obj.OldVNMaterialTraceability);
                ViewBag.ViewVNMaterialTraceability = ViewBag.RootPath + (VNMP != null ? VNMP.VNMaterialTraceability1 : "");

                //obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);

                obj.NCRDETs = _INCRManagementService.GetInresultProcessString(NCR_NUM);

                obj.NCRDISs = _INCRManagementService.GetNCRDISs(NCR_NUM);

                obj.DISPOSITION = _INCRManagementService.getOrderDisposition(NCR_NUM);

                ViewBag.HasVendor = false;
                double detqty = 0;
                foreach (NCR_DETViewModel item in obj.NCRDETs)
                {
                    detqty += item.QTY;
                    if (item.RESPONSE == CONFIRMITY_RESPON.ID_VENDOR)
                    {
                        ViewBag.HasVendor = true;
                        break;
                    }
                }
                ViewBag.ListAllUsers = _IUserService.GetAllUser().ToList();
                obj.ListUSerAppr = _INCRManagementService.GetApproverOfNCRForConfirm(NCR_NUM);

                ViewBag.SEC = obj.SEC;
                ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(NCR_NUM.Trim(),"NCR");
                ViewBag.DETQTY = detqty;
                return View(obj);
            }
            return RedirectToAction("Index", "NCR", null);
        }
        [HttpGet]
        public ActionResult ViewApprovalHistory(string NCR_NUM, string CRNo, bool Pre = false)
        {
            ViewBag.IsOPE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE);
            ViewBag.IsMRBTeam = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.MRBTeam);
            ViewBag.IsWHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            ViewBag.IsSQE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.SQE);
            ViewBag.IsBuyer = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Buyer);
            ViewBag.IsChairm = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            //ViewBag.ListMRBTeam = new SelectList(_IUserService.GetListUserByGroupName(UserGroup.MRBTeam), "Id", "FullName");
            ViewBag.ListRole = new SelectList(_IUserService.GetSelectListAllRole(), "Id", "Name");
            ViewBag.ListRole2 = _IUserService.GetSelectListAllRole().Select(x => new SelectListItem
            {
                Value = x.Id.Trim(),
                Text = (x.Name.Trim())
            }).ToList();
            ViewBag.ListUser = _IUserService.GetAllUserWithRole();
            ViewBag.ENGINEERINGRoleId = _IUserService.GetENGINEERINGRoleId();
            ViewBag.CHAIRMANRoleId = _IUserService.GetCHAIRMANRoleId();
            ViewBag.VNMaterialTraceabilityLink = ConfigurationManager.AppSettings["VNMaterialTraceabilityLink"];

            ViewBag.isOPEOwner = _INCRManagementService.GetOPEOwner(User.Identity.GetUserId(), NCR_NUM);

            NCRManagementViewModel obj = _INCRManagementService.GetNCRHistory(NCR_NUM, CRNo);
            obj.ListRespon = _INCRManagementService.getListRESPON();
            obj.ListDispo = _INCRManagementService.getListDispo();

            //if (obj.PERCENT_INSP == true)
            //{
            //    obj.ListAddition = _INCRManagementService.getListAddition();
            //    obj.ListAddition.RemoveAt(obj.ListAddition.Count - 1);

            //}
            //else
            //{
            //    obj.ListAddition = _INCRManagementService.getListAddition();

            //}
            
            //Thi.nguyen 10.10.2020: Add remove "Return to Vendor" when NCR is floorNCR 
            //and move code "obj.ListAddition = _INCRManagementService.getListAddition();" on the top
            obj.ListAddition = _INCRManagementService.getListAddition();
            if (obj.PERCENT_INSP == true)
                obj.ListAddition.RemoveAt(obj.ListAddition.Count - 1);

            if (!_INCRManagementService.CheckFloorNCR(obj.NCR_NUM))
            {
                obj.ListAddition.RemoveAll(x => x.NAME.Contains("RETURN"));
                obj.ListDispo.RemoveAll(x => x.NAME.Contains("RETURN"));
                obj.ListRespon.RemoveAll(x => x.NAME.Contains("VENDOR"));
            }      
            ViewBag.Fullname = _IUserService.GetNameById(User.Identity.GetUserId());
            ViewBag.ShowBtnChangeSubmitItem = _IUserService.CheckIsApprover(User.Identity.GetUserId(), NCR_NUM.Trim());
            ViewBag.SCRAPCategory = new SelectList(_INCRManagementService.GetSCRAPCategory(), "Category", "Category");


            if (obj != null)
            {
                string strCAREQUESTNO = "", CAREQUESTNO_template = @"; <a target='_blank' id='{0}' href='{1}'>{2}</a>";
                string[] CAREQUESTNO = _INCRManagementService.GetCARequestNO(NCR_NUM);
                foreach (string item in CAREQUESTNO)
                {
                    strCAREQUESTNO += string.Format(CAREQUESTNO_template, item, Url.Action("ViewSCAR", "SCAR", new { scarid = item }), item);
                }
                obj.ISSUED_REQUEST_NO = strCAREQUESTNO.Length > 1 ? strCAREQUESTNO.Remove(0, 1) : "";
                ViewBag.IsMegerNCR = _INCRManagementService.IsMegerNCR(obj.MI_PART_NO, obj.LOT, obj.CCN);
                //Get Uploaded Eividence
                obj.OldEvidence = _INCRManagementService.GetUploadedEvidence(NCR_NUM);
                // Get Uploaded VNMaterialTraceability
                obj.OldVNMaterialTraceability = _INCRManagementService.GetStringUploadedVNMaterialTraceability(NCR_NUM);

                ViewBag.Status = obj.STATUS.Trim();
                ViewBag.SubmitConfirm = Pre;
                ViewBag.allowSCRAP = _INCRManagementService.CheckScrap(NCR_NUM);
                ViewBag.SCRAPMONEY = ConfigurationManager.AppSettings["SCRAPMONEY"];
                ViewBag.RootPath = Url.Content(ConfigurationManager.AppSettings["uploadPath"]);
                VNMaterialTraceability VNMP = _INCRManagementService.GetVNMaterialTraceabilityByID(obj.OldVNMaterialTraceability);
                ViewBag.ViewVNMaterialTraceability = ViewBag.RootPath + (VNMP != null ? VNMP.VNMaterialTraceability1 : "");

                //obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);

                obj.NCRDETs = _INCRManagementService.GetInresultProcessStringHistory(NCR_NUM,CRNo);

                obj.NCRDISs = _INCRManagementService.GetNCRDISsHistory(NCR_NUM,CRNo);

                obj.DISPOSITION = _INCRManagementService.getOrderDisposition(NCR_NUM);

                ViewBag.HasVendor = false;
                double detqty = 0;
                foreach (NCR_DETViewModel item in obj.NCRDETs)
                {
                    detqty += item.QTY;
                    if (item.RESPONSE == CONFIRMITY_RESPON.ID_VENDOR)
                    {
                        ViewBag.HasVendor = true;
                        break;
                    }
                }
                ViewBag.ListAllUsers = _IUserService.GetAllUser().ToList();
                obj.ListUSerAppr = _INCRManagementService.GetApproverOfNCRForConfirmHistory(NCR_NUM,CRNo);

                ViewBag.SEC = obj.SEC;
                ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(NCR_NUM.Trim());
                ViewBag.DETQTY = detqty;
                return View(obj);
            }
            return View();
        }
        public ActionResult GetListDefectProcess(string NCR_NUM,string CCN)
        {
            try
            {
                List<NCR_DETViewModel> result = _INCRManagementService.GetInresultProcess(NCR_NUM,CCN);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { result = new List<NCR_DETViewModel>(), JsonRequestBehavior.AllowGet });
            }

        }
        [HttpPost]
        public ActionResult GetListDefect(string receiver)
        {
            try
            {
                List<INS_RESULT_DEFECTViewModel> result = _IReciverService.GetInresult(receiver);
                return Json(new { result = result, JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { result = new List<INS_RESULT_DEFECTViewModel>(), JsonRequestBehavior.AllowGet });
            }

        }

        public JsonResult GetDropdownlistDesForPro(List<string> id,string CCN)
        {
            List<DescriptionModel> listnc = _IReciverService.GetDropdownlistDesForPro(id,CCN);
            return Json(listnc);
        }

        public JsonResult GetDropdownlistDecriptByIdDefect(List<string> id,string CCN)
        {
            List<DescriptionModel> listnc = _IReciverService.GetDropdownlistDecriptByIdDefect(id,CCN);
            return Json(listnc, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EditNCRForIQC(string NCR_NUM)
        {
            NCRManagementViewModel obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
            ViewBag.VNMaterialTraceabilityLink = ConfigurationManager.AppSettings["VNMaterialTraceabilityLink"];
            if (obj != null)
            {
                //obj.Listdefectprocess = _INCRManagementService.GetInresultProcessString(NCR_NUM);
                //foreach (var item in obj.Listdefectprocess)
                //{
                //    defective = Convert.ToDouble(item.QTY + defective);
                //}
                //obj.defect = defective;

                obj.NCRDETs = _INCRManagementService.GetInresultProcessString(NCR_NUM);

                //Get Uploaded Eividence
                obj.OldEvidence = _INCRManagementService.GetUploadedEvidence(NCR_NUM);
                for (int i = 0; i < obj.OldEvidence.Count; i++)
                {
                    NCR_EVI evi = obj.OldEvidence[i];
                    obj.SizeOfOldEvidence = UtilitiesService.Add(obj.SizeOfOldEvidence, GetSizeOfFile(evi.EVI_PATH));
                }
                // Get Uploaded VNMaterialTraceability
                obj.OldVNMaterialTraceability = _INCRManagementService.GetStringUploadedVNMaterialTraceability(NCR_NUM);
                ViewBag.SizeLength = obj.SizeOfOldEvidence.Sum();

                return View(obj);
            }
            else
            {
                return View(new NCRManagementViewModel());
            }
        }
        [HttpPost]
        public ActionResult EditNCRForIQC(NCRManagementViewModel model)
        {
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditNCRForIQCRemark(List<string> Remark)
        {
            return Json(new { success = true });
        }

        public ActionResult ApprovalNcPart()
        {
            return View();
        }
        public ActionResult ApprovalClose()
        {
            return View();
        }
        public ActionResult NCRApprovalList([DataSourceRequest]DataSourceRequest request)
        {
            string id = User.Identity.GetUserId();
            List<NCR_HDR> list = _INCRManagementService.GetListNCRApproval(id);
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult PrintNCR(string NCR_NUM, List<NCR_EVI> OldEvidence, string OldVNMaterialTraceabilityID = "", bool OldVNMaterialTraceabilityIDIsPrint = false)
        {
            NCRManagementViewModel obj = PrintNCRTEMP(NCR_NUM, OldEvidence, OldVNMaterialTraceabilityID, OldVNMaterialTraceabilityIDIsPrint);
            if (obj != null)
            {
                //return View(obj);
                return File($"{Server.MapPath(ConfigurationManager.AppSettings["uploadPath"])}MERGE_EVIDENT/Merge_{NCR_NUM}_Evident.pdf", "application/pdf");
            }
            return View(obj);
        }

        public ActionResult CreateNCREvident(string NCR_NUM)
        {
            if (string.IsNullOrEmpty(NCR_NUM))
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            List<string> listNCRnum = new List<string>
            {
                NCR_NUM
            };
            List<NCR_EVI> evis = _INCRManagementService.GetEVIByNCRNUM(listNCRnum);

            NCRManagementViewModel obj = PrintNCRTEMP(NCR_NUM, null, "", false);
            if (obj != null)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        private NCRManagementViewModel PrintNCRTEMP(string NCR_NUM, List<NCR_EVI> OldEvidence, string OldVNMaterialTraceabilityID, bool OldVNMaterialTraceabilityIDIsPrint)
        {
            ApprovalDetViewmodel appr_model = _INCRManagementService.GetListUserApprovalByNcrNum(NCR_NUM);
            NCRManagementViewModel obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
            obj.ListAdditional = _INCRManagementService.GetAdditional(NCR_NUM);
            obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM,obj.CCN);
            obj.NCRDETs = _INCRManagementService.GetInresultProcessString(NCR_NUM);
            obj.ListNC_Group = _INCRManagementService.GetListNC_GRP_DESC(obj.CCN);
            obj.ListRespon = _INCRManagementService.getListRESPON();
            obj.ListDispo = _INCRManagementService.getListDispo_4_PrintNCR();//_INCRManagementService.getListDispo();

            if (obj != null)
            {
                ViewBag.ListAllUsers = _IUserService.GetAllUser().ToList();
                obj.ListUSerAppr = new List<UserApproval>();

                obj.OldEvidence = _INCRManagementService.GetUploadedEvidence(NCR_NUM);
                // Get Uploaded VNMaterialTraceability
                obj.OldVNMaterialTraceability = _INCRManagementService.GetStringUploadedVNMaterialTraceability(NCR_NUM);

                ViewBag.Status = obj.STATUS.Trim();

                //obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);

                obj.NCRDETs = _INCRManagementService.GetInresultProcessString(NCR_NUM);

                obj.NCRDISs = _INCRManagementService.GetNCRDISs(NCR_NUM);

                obj.DISPOSITION = _INCRManagementService.getOrderDisposition(NCR_NUM);
                obj.ListUSerAppr = _INCRManagementService.GetApproverOfNCRForConfirm(NCR_NUM);
                if (obj.ListUSerAppr.Count % 2 != 0)
                {
                    int num = obj.ListUSerAppr.Count % 2;
                    for (int i = 0; i < num; i++)
                    {
                        obj.ListUSerAppr.Add(new UserApproval
                        {
                            DateAppr = "",
                            FullName = "",
                            Id = "",
                            IdUser = "",
                            IsAppr = false,
                            RoleId = "",
                            RoleName = "",
                            Signature = ""
                        });
                    }
                }
                obj.REV = ConfigurationManager.AppSettings["REV"];
                obj.FORM = ConfigurationManager.AppSettings["FORM"];
                //if (obj.STATUS.Trim() != StatusInDB.Void && obj.STATUS.Trim() != StatusInDB.DispositionApproved)
                //{
                //    obj.REV = ConfigurationManager.AppSettings["REV"];
                //    obj.FORM = ConfigurationManager.AppSettings["FORM"];
                //    obj.URL = _INCRManagementService.getUrlNcr(NCR_NUM);
                //    PrintNCREnvident(NCR_NUM, obj);
                //}
                PrintNCREnvident(NCR_NUM, obj, OldEvidence, OldVNMaterialTraceabilityID, OldVNMaterialTraceabilityIDIsPrint);
                return (obj);
            }
            return null;
        }

        //[HttpPost]
        //public JsonResult SaveUserAprroval(string IdUser, string ncrnum, string password)
        //{
        //    var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    var isValid = false;
        //    var _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    try
        //    {
        //        var iduser = (User.Identity.GetUserId());
        //        if (User.Identity.GetUserId() == IdUser)
        //        {
        //            var Approval = _INCRManagementService.UpdateUserAprrovalDate(ncrnum.Trim(), IdUser);
        //            return Json(new { Approval });

        //        }
        //        else
        //        {
        //            var ApprovalUser = UserManager.FindById(IdUser);
        //            isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
        //            if (isValid == true)
        //            {
        //                var Approval = _INCRManagementService.UpdateUserAprrovalDate(ncrnum.Trim(), IdUser);
        //                return Json(new { Approval });
        //            }
        //            else
        //            {
        //                return Json(new { message = "Password incorect !" });
        //            }
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = false, message = "Please contact to admin! " });
        //    }

        //}

        public ActionResult SaveDispo(List<NCR_DIS> lstDis)
        {
            if (lstDis != null && lstDis.Count > 0)
            {
                bool chk2 = _INCRManagementService.AddListNCRDIS1(lstDis);

                return Json(new { success = chk2 });
            }
            else
            {
                return Json(new { success = false });
            }

            //return Json(new { success = true });
        }
        public ActionResult ChangestatusAprroval(string ncrnumber, string userquality, bool final, string userengineer,
            string userafg, string userpurchange)
        {
            _INCRManagementService.updatedateapprovalNCR(ncrnumber, final);
            bool chek = _INCRManagementService.UpdateStatusNCR(ncrnumber, StatusInDB.DispositionApproved, "");

            if (!chek)
            {
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public JsonResult UpdateUser(string id, string userId)
        {
            Result res = new Result();
            LogWriter _log = new LogWriter("NCRApproval - UpdateUser");
            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(userId))
            {
                res = _INCRManagementService.UpdateUserApproval(id, userId);
                if (!res.success)
                {
                    return Json(new { res.success, res.message, res.obj });
                }
                if (userId != "")
                {
                    try
                    {
                        _INCRManagementService.SentEmailSubmitEdit(userId);
                    }
                    catch (Exception ex)
                    {
                        _log.LogWrite(ex.ToString());
                        return Json(new { success = false, message = $@"Update Approver successful | Sent Email submit edit  unsuccessful!", res.obj });
                    }
                }
                return Json(new { res.success, res.message, res.obj });
            }
            return Json(new { success = false, message = "Not Exist Userid" });
        }
        public FileContentResult DownloadFile(int fileId, string type, string filepath)
        {
            string filePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Trim().ToLower().Equals("orderdisposition"))
                {
                    //DISPOSITION
                    string filePathFull = (filePath + filepath);
                    byte[] file = GetMediaFileContent(filePathFull);
                    return File(file, MimeMapping.GetMimeMapping(filepath), filepath);
                }
            }

            if (fileId != -1)
            {
                NCR_EVI sf = _INCRManagementService.GetFileWithFileID(fileId);
                if (sf != null)
                {
                    string filePathFull = (filePath + sf.EVI_PATH);
                    byte[] file = GetMediaFileContent(filePathFull);
                    return File(file, MimeMapping.GetMimeMapping(sf.EVI_PATH), sf.EVI_PATH);
                }
            }
            else
            {
                return null;
            }
            return null;
        }
        public FileContentResult DownloadFileNCREvident(string NCR_NUM)
        {
            string RootPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]),
                FileMergePath = $"{RootPath}MERGE_EVIDENT/Merge_{NCR_NUM}_Evident.pdf"; ;
            int Idfile = _INCRManagementService.getIDfile(NCR_NUM);
            if (!string.IsNullOrEmpty(NCR_NUM))
            {
                NCR_EVI sf = _INCRManagementService.GetFileWithFileID(Idfile);
                if (sf != null)
                {
                    byte[] file = GetMediaFileContent(FileMergePath);
                    return File(file, MimeMapping.GetMimeMapping(FileMergePath), $"Merge_{NCR_NUM}_Evident.pdf");
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        public static byte[] GetMediaFileContent(string filename)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(
                        filename,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read))
            {
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                return fileData;
            }
        }

        private static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
            {
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            }
            else
            {
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);
            }

            if (viewEngineResult == null)
            {
                throw new System.IO.FileNotFoundException("View cannot be found.");
            }

            // get the view and attach the model to view data
            IView view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (StringWriter sw = new System.IO.StringWriter())
            {
                ViewContext ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public void PrintNCREnvident(string NCR_NUM, NCRManagementViewModel obj, List<NCR_EVI> OldEvidence, string OldVNMaterialTraceabilityID, bool OldVNMaterialTraceabilityIDIsPrint)
        {
            string html = RenderViewToString(ControllerContext, "~/views/NCRApproval/PrintNCR.cshtml", obj, true);
            string FileNameTemp = Guid.NewGuid() + "",
                   RootPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]),
                   FilePathTemp = $"{RootPath}TEMP/{FileNameTemp}.pdf",
                   FileMergePath = $"{RootPath}MERGE_EVIDENT/Merge_{NCR_NUM}_Evident.pdf";

            if (!Directory.Exists($"{RootPath}TEMP"))
            {
                Directory.CreateDirectory($"{RootPath}TEMP");
            }
            if (!Directory.Exists($"{RootPath}MERGE_EVIDENT"))
            {
                Directory.CreateDirectory($"{RootPath}MERGE_EVIDENT");
            }

            #region Create a PDF from an existing HTML using IronPDF
            EvoPdf.HtmlToPdfConverter htmlToPdfConverter = new EvoPdf.HtmlToPdfConverter
            {
                LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw=="
            };
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = new EvoPdf.PdfPageSize();
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = EvoPdf.PdfPageOrientation.Portrait;
            htmlToPdfConverter.ConvertHtmlToFile(html,
                Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority
                , FilePathTemp);

            EvoPdf.Document MergePdf = new EvoPdf.Document
            {
                LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw=="
            };

            Document firstDoc = new EvoPdf.Document(FilePathTemp);
            MergePdf.AppendDocument(firstDoc);

            Document ividentDoc = new EvoPdf.Document();

            // Print VN Material Traceability
            if (OldVNMaterialTraceabilityIDIsPrint)
            {
                string VNMaterialTraceabilityPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"] + _INCRManagementService.GetUploadedVNMaterialTraceability(NCR_NUM).VNMaterialTraceability1);
                if (System.IO.File.Exists(VNMaterialTraceabilityPath))
                {
                    Document VNMaterialTraceability = new EvoPdf.Document(VNMaterialTraceabilityPath);
                    MergePdf.AppendDocument(VNMaterialTraceability);
                }
            }

            if (OldEvidence != null)
            {
                List<string> arEVIID = OldEvidence.Where(x => x.IsPrint != null & x.IsPrint == true).Select(x => x.EVI_ID.ToString()).ToList();
                List<NCR_EVI> EVIs = _INCRManagementService.GetEVIByIds(arEVIID);
                for (int i = 0; i < EVIs.Count; i++)
                {
                    NCR_EVI item = EVIs[i];
                    string EviPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"] + item.EVI_PATH);
                    if (System.IO.File.Exists(EviPath))
                    {
                        if (UtilitiesService.HasImageExtension(EviPath))
                        {
                            EvoPdf.AddElementResult addElementResult = null;
                            // The position on X anf Y axes where to add the next element
                            float yLocation = 10;
                            float xLocation = 10;

                            // Create a PDF page in PDF document
                            Margins margins = new Margins { Top = 20, Bottom = 20, Left = 20, Right = 20 };
                            PdfPage pdfPage = MergePdf.AddPage(margins);
                            // Add section title
                            PdfFont titleFont = MergePdf.AddFont(new Font("Times New Roman", 12, FontStyle.Bold, GraphicsUnit.Point));
                            TextElement titleTextElement = new TextElement(xLocation, yLocation, "", titleFont)
                            {
                                ForeColor = Color.Black
                            };
                            addElementResult = pdfPage.AddElement(titleTextElement);
                            yLocation = addElementResult.EndPageBounds.Bottom + 10;
                            pdfPage = addElementResult.EndPdfPage;

                            float titlesYLocation = yLocation;
                            ImageElement unscaledImageElement = new ImageElement(5, 5, EviPath);
                            addElementResult = pdfPage.AddElement(unscaledImageElement);
                            RectangleF scaledDownImageRectangle = new RectangleF(addElementResult.EndPageBounds.Right + 30, addElementResult.EndPageBounds.Y,
                            addElementResult.EndPageBounds.Width, addElementResult.EndPageBounds.Height);
                        }
                        else
                        {
                            Document docEVI = new EvoPdf.Document(EviPath);
                            MergePdf.AppendDocument(docEVI);
                        }
                    }
                }
            }


            MergePdf.Save(FileMergePath);
            firstDoc.Close();
            ividentDoc.Close();
            System.IO.File.Delete(FilePathTemp);
            #endregion
        }

        [HttpPost]
        public ActionResult DeleteAddIns(string ncrnum, string item)
        {
            if (string.IsNullOrEmpty(ncrnum) || string.IsNullOrEmpty(item))
            {
                return Json(new { success = false });
            }

            return Json(new { success = _INCRManagementService.DeleteAddIns(ncrnum, item), message = "Something when wrong, Addins not exist or Maybe approve not done " });
        }
        [HttpPost]
        public ActionResult EditAddIns(string ncrnum, string item, double qty, string remark)
        {
            if (string.IsNullOrEmpty(ncrnum) || string.IsNullOrEmpty(item) || qty <= 0)
            {
                return Json(new { success = false });
            }

            return Json(new { success = _INCRManagementService.EditAddIns(ncrnum, item, qty, remark), message = "Check Qty or Maybe approve not done" });
        }
        [HttpPost]
        ActionResult Addnew(HttpPostedFileBase data)
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewADDIns(string NCRNUM, string Item, double QTY, string ADDINS, string INSP,
            string OrDISPO, string OrFile, string OrMessage, DISPOSITIONViewModel order,string Remark = "" )
        {
            if (string.IsNullOrEmpty(NCRNUM) || string.IsNullOrEmpty(Item) || string.IsNullOrEmpty(INSP) || string.IsNullOrEmpty(ADDINS) || QTY == 0)
            {
                return Json(new { success = false, message = "Addins not valid !!" });
            }
            DateTime date = DateTime.Now;
            string returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute;
            OrderDisposition orderDisposition = null;
            if (!string.IsNullOrEmpty(OrDISPO) & OrFile != null)
            {
                orderDisposition = new OrderDisposition
                {
                    COST = OrMessage,
                    FileName = returnPath + "/" + OrFile,
                    IsActive = true,
                    ITEM = Item,
                    NCR_NUMBER = NCRNUM,
                    Type = "DIS",
                    TypeOfDisposition = OrDISPO
                };
            }

            Result res = _INCRManagementService.AddinsAddIns(NCRNUM, Item, QTY, ADDINS, Remark, INSP, orderDisposition);
            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["c"];
                string role = EmailCreateConfig.Split('|')[0];
                string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);

                string[] arrRoleName = EmailCreateConfig.Split('|')[1].Split(';').ToArray();
                List<AspNetUser> Users = _IUserService.GetUsersByRoleName(arrRoleName);
                string MailTemplate = EmailCreateConfig.Split('|')[2];

                if (role == "ALL")
                {
                    foreach (AspNetUser mail in Users)
                    {
                        _emailService.SendEmailCreateNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: "");
                    }
                }
                else if (role == "NCR")
                {

                    string[] arrApproverId = _IUserService.GetApproverByNCRNUM(NCRNUM, null).Select(x => x.Id).ToArray();
                    List<AspNetUser> MailOfUser = Users.Where(x => arrApproverId.Contains(x.Id)).ToList();
                    foreach (AspNetUser mail in MailOfUser)
                    {
                        _emailService.SendEmailCreateNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: "");
                    }
                }
            }
            return Json(new { success = res.success, message = res.obj });
        }

        public ActionResult CheckApproal(string ncrnum)
        {
            CApproval Approval = _INCRManagementService.CheckApproval(ncrnum);
            return Json(new { Approval }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CloseNCR(string NCR_NUM)
        {
            bool res = _iTaskManagementService.ExistsTask(NCR_NUM);
            if (!res)
            {
                _INCRManagementService.CloseNCR(NCR_NUM, User.Identity.GetUserId());
            }
            return Json(new { res }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserByRole(string role, string ncrnum)
        {
            IEnumerable<AspNetUser> lstUser = _IUserService.GetUserByRoleGroupId(role.Trim(), ncrnum);
            SelectList selectUser = new SelectList(lstUser, "Id", "FullName");
            var selectUserNew = from lst in lstUser
                                select new
                                {
                                    label = lst.FullName,
                                    title = lst.FullName,
                                    value = lst.Id
                                };
            return Json(selectUserNew, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserByRoleWaittingdispo(string role, string ncrnum)
        {
           List<GetUserByRoleGroupId_Result> lstUser = _IUserService.GetUserByRoleGroupIdEng(role.Trim(), ncrnum);
            SelectList selectUser = new SelectList(lstUser, "Id", "FullName");
            var selectUserNew = from lst in lstUser
                                select new
                                {
                                    label = lst.FullName,
                                    title = lst.FullName,
                                    value = lst.ID
                                };
            return Json(selectUserNew, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult PrintMRBTAG(string NCR_NUM)
        public FileContentResult PrintMRBTAG(string NCR_NUM)
        {
            NCR_HDR Data = _INCRManagementService.GetNCR_HDR(NCR_NUM.Trim());
            ViewBag.NCR_DET_DESC = _INCRManagementService.GetNCR_DET_DESC(NCR_NUM.Trim());
            ViewBag.EMP = _INCRManagementService.GetFullUserNameById(Data.INSPECTOR);
            string RootPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]),
                MRBTagPath = $"{RootPath}MRBTAG/{Data.NCR_NUM}.pdf";
            //if (!Data.STATUS.Trim().Equals(StatusInDB.DispositionApproved))
            //{
            string html = RenderViewToString(ControllerContext, "~/views/NCRApproval/ViewPrintMRBTAG.cshtml", Data, true);
            if (!Directory.Exists($"{RootPath}MRBTAG"))
            {
                Directory.CreateDirectory($"{RootPath}MRBTAG");
            }
            #region Create a PDF from an existing HTML using IronPDF
            EvoPdf.HtmlToPdfConverter htmlConverter = new EvoPdf.HtmlToPdfConverter
            {
                LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw=="
            };
            htmlConverter.PdfDocumentOptions.AutoSizePdfPage = false;
            htmlConverter.PdfDocumentOptions.SinglePage = true;
            htmlConverter.PdfDocumentOptions.StretchToFit = true;
            htmlConverter.PdfDocumentOptions.PdfPageSize = new EvoPdf.PdfPageSize { Width = 330, Height = 220 };
            htmlConverter.PdfDocumentOptions.TopMargin = 9;
            htmlConverter.PdfDocumentOptions.LeftMargin = 15;
            htmlConverter.PdfDocumentOptions.BottomMargin = 9;
            htmlConverter.PdfDocumentOptions.RightMargin = 15;

            htmlConverter.PdfDocumentOptions.TransparencyEnabled = true;

            htmlConverter.ConvertHtmlToFile(html,
                Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority, MRBTagPath);
            #endregion
            // }

            //ViewBag.EMP = _INCRManagementService.GetFullUserNameById(Data.INSPECTOR);

            byte[] file = GetMediaFileContent(MRBTagPath);
            return File(file, MimeMapping.GetMimeMapping(MRBTagPath), $"{Data.NCR_NUM}.pdf");
            //return View("~/views/NCRApproval/ViewPrintMRBTAG.cshtml");
        }

        public FileContentResult DownloadVNMaterialTraceability(string id)
        {
            string RootPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            VNMaterialTraceability VNMaterialTraceability = _INCRManagementService.GetVNMaterialTraceabilityByID(id);
            byte[] file = GetMediaFileContent(RootPath + VNMaterialTraceability.VNMaterialTraceability1);
            return File(file, MimeMapping.GetMimeMapping(RootPath + VNMaterialTraceability.VNMaterialTraceability1), $"VNMaterialTraceability_{VNMaterialTraceability.NCRNUM}.pdf");
        }

        public ActionResult SubmitSCRAP(string NCR_NUM, string Comment,string WHMRBid)
        {
            Result res = _INCRManagementService.SubmitScrap(NCR_NUM, User.Identity.GetUserId(), Comment,WHMRBid);
            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitNCR(NCRManagementViewModel data)
        {
            LogWriter logelse = new LogWriter("SubmitNCR");
            if (data == null)
            {
                return Json(new { success = false, message = "Input infor not valid" });
            }
            if (data.NCRDETs == null)
            {
                return Json(new { success = false, message = "Non-Comformity is not exist, please go to edit NCR to add Non-Comformity" });
            }

            List<APPROVAL> aPPROVALs = new List<APPROVAL>();
            List<TaskManagementCreateModel> taskManagementCreateModels = new List<TaskManagementCreateModel>();
            #region QTS change code add user approve
            logelse.LogWrite("NCRApproval - :247: Add Approver");
            foreach (UserApproveViewModel item in data.UserApprove)
            {
                APPROVAL UserApprove = new APPROVAL
                {
                    UserId = item.UserId.Trim(),
                    RoleId = item.RoleId.Trim(),
                    CreateDate = DateTime.Now,
                    isActive = true,
                    NCR_NUMBER = data.NCR_NUM
                };
                aPPROVALs.Add(UserApprove);
            }
            #endregion

            Result res = _INCRManagementService.NormalSubmitNCR(aPPROVALs, taskManagementCreateModels, data.NCR_NUM, User.Identity.GetUserId(), data.Comment);
            //if (res.success)
            //{
            //    string EmailCreateConfig = ConfigurationManager.AppSettings["b"];
            //    string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
            //    string role = EmailCreateConfig.Split('|')[0];

            //    var arrRoleName = EmailCreateConfig.Split('|')[1].Split(';').ToArray();
            //    List<AspNetUser> Users = _IUserService.GetUsersByRoleName(arrRoleName);
            //    string MailTemplate = EmailCreateConfig.Split('|')[2];
            //    string linkNCR = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = data.NCR_NUM.Trim() }, Request.Url.Scheme);

            //    _emailService.SendEmailToConfirmNCR(data.NCR_NUM.Trim(), linkNCR, path, MailTemplate);


            //}

            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SubmitFloorNCR(NCRManagementViewModel data)
        {
            LogWriter logelse = new LogWriter("SubmitNCR");
            if (data == null)
            {
                return Json(new { success = false, message = "Input infor not valid" });
            }
            if (data.NCRDETs == null)
            {
                return Json(new { success = false, message = "Non-Comformity is not exist, please go to edit NCR to add Non-Comformity" });
            }

            List<APPROVAL> aPPROVALs = new List<APPROVAL>();
            List<TaskManagementCreateModel> taskManagementCreateModels = new List<TaskManagementCreateModel>();
            #region QTS change code add user approve
            logelse.LogWrite("NCRApproval - :247: Add Approver");
            foreach (UserApproveViewModel item in data.UserApprove)
            {
                APPROVAL UserApprove = new APPROVAL
                {
                    UserId = item.UserId.Trim(),
                    RoleId = item.RoleId.Trim(),
                    CreateDate = DateTime.Now,
                    isActive = true,
                    NCR_NUMBER = data.NCR_NUM
                };
                aPPROVALs.Add(UserApprove);
            }
            #endregion

            Result res = _INCRManagementService.FloorSubmitNCR(aPPROVALs, taskManagementCreateModels, data.NCR_NUM, User.Identity.GetUserId(), data.Comment,data.MRB_LOC);
            if(res.success)
            {
                
                res = _INCRManagementService.SubmitConfirmNCR(data.NCR_NUM, aPPROVALs, data.RoleIDs, User.Identity.GetUserId());
            }
            
            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitConfirmNCR(NCRManagementViewModel data)
        {
            LogWriter _log = new LogWriter("NCRApproval - SubmitConfirmNCR");
            if (data == null)
            {
                return Json(new { success = false, message = "Input infor not valid" });
            } 
            List<APPROVAL> aPPROVALs = new List<APPROVAL>();
            //if (data.UserApprove != null)
            //    foreach (var item in data.UserApprove)
            //    {
            //        aPPROVALs.Add(new APPROVAL
            //        {
            //            CreateDate = DateTime.Now,
            //            isActive = true,
            //            NCR_NUMBER = data.NCR_NUM.Trim(),
            //            RoleId = item.RoleId,
            //            UserId = item.UserId
            //        });
            //    }

            Result result = _INCRManagementService.SubmitConfirmNCR(data.NCR_NUM, aPPROVALs, data.RoleIDs, User.Identity.GetUserId());
            ///TODO: Send email to ENG
            if (result.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["b"];
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                string linkNCR = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = data.NCR_NUM.Trim() }, Request.Url.Scheme);
                string test = EmailCreateConfig.Split('|')[2];

                Result resemail = _emailService.SendEmailFromConfirmNCR(data.NCR_NUM.Trim(), path, EmailCreateConfig.Split('|')[2], linkNCR);
            }

            return Json(result.success, result.message);
        }

        public ActionResult SubmitDispositionNCR(NCRManagementViewModel data)
        {
            LogWriter _log = new LogWriter("NCRApproval - SubmitDispositionNCR");
            string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = data.NCR_NUM.Trim() }, Request.Url.Scheme);
            string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
            if (data == null)
            {
                return Json(new { success = false, message = "Input infor not valid" });
            }

            List<OrderDisposition> OrderDispositions = new List<OrderDisposition>();

            foreach (DISPOSITIONViewModel disp in data.DISPOSITION)
            {

                string Dispo = disp.Disposition == "REWORK" ? "REWORK" :
                    disp.Disposition == "VEN MI" ? "VEN MI" :
                     disp.Disposition == "SORT100" ? "SORT100" :
                       disp.Disposition == "ACCEPTED" ? "ACCEPTED" :
                       disp.Disposition == "OTHER" ? "OTHER" :
                    disp.Disposition == "VEN EXP" ? "VEN EXP" :
                     disp.Disposition == "SALVAGE" ? "SALVAGE" :
                    disp.Disposition == "SCRAP" ? "SCRAP" : 
                    disp.Disposition == "USEASIS" ? "USEASIS":
                    disp.Disposition == "SCRAP AND DESTROY" ? "SCRAP AND DESTROY" : "";
                if (!string.IsNullOrEmpty(Dispo))
                {
                    OrderDispositions.Add(new OrderDisposition
                    {
                        COST = disp.Message,
                        FileName = SaveFile(disp.FileAttach),
                        IsActive = true,
                        ITEM = disp.Item,
                        NCR_NUMBER = data.NCR_NUM,
                        Type = !string.IsNullOrEmpty(disp.Type) ? disp.Type : "DET",
                        TypeOfDisposition = Dispo
                    });
                }
            }
            List<OrderDisposition> OrderDispositionsADDIN = new List<OrderDisposition>();
            foreach (DISPOSITIONViewModel disp in data.ADDIN)
            {

                string Dispo = disp.Disposition == "REWORK" ? "REWORK" :
                    disp.Disposition == "VEN MI" ? "VEN MI" :
                     disp.Disposition == "SORT100" ? "SORT100" :
                       disp.Disposition == "ACCEPTED" ? "ACCEPTED" :
                       disp.Disposition == "OTHER" ? "OTHER" :
                    disp.Disposition == "VEN EXP" ? "VEN EXP" :
                     disp.Disposition == "SALVAGE" ? "SALVAGE" :
                    disp.Disposition == "SCRAP" ? "SCRAP" :
                    disp.Disposition == "USEASIS" ? "USEASIS" :
                    disp.Disposition == "SCRAP AND DESTROY" ? "SCRAP AND DESTROY" : "";
                if (!string.IsNullOrEmpty(Dispo))
                {
                    OrderDispositionsADDIN.Add(new OrderDisposition
                    {
                        COST = disp.Message,
                        FileName = SaveFile(disp.FileAttach),
                        IsActive = true,
                        ITEM = disp.Item,
                        NCR_NUMBER = data.NCR_NUM,
                        Type = !string.IsNullOrEmpty(disp.Type) ? disp.Type : "DIS",
                        TypeOfDisposition = Dispo
                    });
                }
            }

            Result res = _INCRManagementService.SubmitDispositionNCR(data, User.Identity.GetUserId(), OrderDispositions, OrderDispositionsADDIN);
            if (!res.success)
            {
                return Json(new { success = false, message = res.message });
            }

            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["c"];
                string role = EmailCreateConfig.Split('|')[0];

                string[] arrRoleName = EmailCreateConfig.Split('|')[1].Split(';').ToArray();
                List<AspNetUser> Users = _IUserService.GetUsersByRoleName(arrRoleName);
                string MailTemplate = EmailCreateConfig.Split('|')[2];

                if (role == "ALL")
                {
                    foreach (AspNetUser mail in Users)
                    {
                        _emailService.SendEmailCreateNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: data.NCR_NUM.Trim(), linkNCR: urlsent, comment: "");
                    }
                }
                else if (role == "NCR")
                {
                    string idCh = _IUserService.GetCHAIRMANRoleId();
                    string[] arrApproverId = _IUserService.GetApproverByNCRNUM(data.NCR_NUM, new string[] { idCh }).Select(x => x.Id).ToArray();
                    List<AspNetUser> MailOfUser = Users.Where(x => arrApproverId.Contains(x.Id)).ToList();
                    foreach (AspNetUser mail in MailOfUser)
                    {
                        _emailService.SendEmailCreateNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: data.NCR_NUM.Trim(), linkNCR: urlsent, comment: "");
                    }
                }


            }

            //var arrRESPONSE = data.NCRDETs.Select(x => x.RESPONSE).ToArray();
            //if (arrRESPONSE.Contains(CONFIRMITY_RESPON.ID_VENDOR))
            //{
            //    _emailService.SentMailRemindSQE(data.NCR_NUM.Trim(), "", path, urlsent, data.Comment);
            //}
            //_emailService.SentMailRemind(data.NCR_NUM, "",  path, urlsent, data.Comment);

            return Json(new { success = true, message = "" });
        }

        [HttpPost]
        public ActionResult Approve(string Id, string NCRNUM, string ApproverId, string password)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return Json(new { success = false, message = "Approver not valid" });
            }

            if (string.IsNullOrEmpty(NCRNUM))
            {
                return Json(new { success = false, message = "NCR NUMBER not valid" });
            }

            if (string.IsNullOrEmpty(ApproverId))
            {
                return Json(new { success = false, message = "Approver not valid" });
            }

            string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
            Result res = new Result();
            Result resApp = new Result();
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            bool isValid = false;
            ApplicationSignInManager _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            bool isChairMain = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            bool checkSignature = _INCRManagementService.CheckSignature(ApproverId);
           // bool checksignaturUserlogin = _INCRManagementService.CheckSignature(User.Identity.GetUserId());
            if (User.Identity.GetUserId() == ApproverId)
            {
                if (checkSignature == true)
                {
                    #region Define parameter
                    string EmailCreateConfig = ConfigurationManager.AppSettings["e"];
                    string role = EmailCreateConfig.Split('|')[0];

                    string[] arrRoleName = EmailCreateConfig.Split('|')[1].Split(';').ToArray();
                    string MailTemplate = EmailCreateConfig.Split('|')[2];

                    string linkNCR = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM }, Request.Url.Scheme);
                    #endregion

                    #region User Approve 
                    res = _INCRManagementService.UpdateUserAprrovalDate(Id, NCRNUM.Trim(), ApproverId, "");
                    //Send Email To chairman approve
                    if (res.obj.ToString() == "EmailChairman")
                    {
                        List<AspNetUser> Chairmans = _IUserService.GetChairmanOfNCR(NCRNUM);
                        _emailService.SendEmailDispoitionToChairman(NCRNUM, linkNCR, Chairmans, path);
                    }
                    #endregion

                    
                    //Thi.Nguyen: 3-Oct-2020: add condition for Floor NCR
                    #region Add task and send email when ncr(not FloorNCR) approval
                    if (!_INCRManagementService.CheckFloorNCR(NCRNUM))
                    {
                        //Add task when NCR approval
                        resApp = _iTaskManagementService.AddTaskNCRDispositionApproval(NCRNUM, Partial: res.obj.ToString());
                        //Send Email Add Task

                        if (resApp.obj.ToString() == StatusInDB.DispositionApproved | res.obj.ToString() == "ApprovePartial")
                        {
                            _emailService.SendEmailAutoAssignTask(mailTemplate: linkNCR, mailPath: path, NCRNUM: NCRNUM);
                        }
                    }
                    #endregion

                    #region Send Email when NCR APPROVAL
                    NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM.Trim());
                    if (NCR.STATUS.Trim() == StatusInDB.DispositionApproved | res.obj.ToString() == "ApprovePartial")
                    {
                        List<AspNetUser> Users = _IUserService.GetUserOfNCRApproval(NCRNUM);

                        foreach (AspNetUser mail in Users)
                        {
                                _emailService.SendEmailDispositionApproval(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: NCRNUM, linkNCR: linkNCR, comment: NCR.Comment);
                        }

                        #region Send template 4 if Corrective action is required and response is vendor for B: Quality
                        _emailService.SendEmailDispositionApproval(linkNCR, path, NCR.NCR_NUM);
                        #endregion
                    }
                    #endregion

                    return Json(new { success = res.success, res.message });
                }
                else
                {
                    return Json(new { success = false, message = "You don't have a signature, Please add your signature or contact IT team !" });
                }
            }

            else
            {
                if (checkSignature == true)
                {

                    if (string.IsNullOrEmpty(password))
                    {
                        return Json(new { success = false, message = "Approver's password not valid" });
                    }
                    Models.Account.ApplicationUser ApprovalUser = UserManager.FindById(ApproverId);
                    isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
                
                    if (isValid == true)
                    {
                        #region Define parameter
                        string EmailCreateConfig = ConfigurationManager.AppSettings["e"];
                        string role = EmailCreateConfig.Split('|')[0];

                        string[] arrRoleName = EmailCreateConfig.Split('|')[1].Split(';').ToArray();
                        string MailTemplate = EmailCreateConfig.Split('|')[2];

                        string linkNCR = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM }, Request.Url.Scheme);
                        #endregion

                        #region User Approve 
                        res = _INCRManagementService.UpdateUserAprrovalDate(Id, NCRNUM.Trim(), ApproverId, "");
                        //Send Email To chairman approve
                        if (res.obj.ToString() == "EmailChairman")
                        {
                            List<AspNetUser> Chairmans = _IUserService.GetChairmanOfNCR(NCRNUM);
                            _emailService.SendEmailDispoitionToChairman(NCRNUM, linkNCR, Chairmans, path);
                        }
                        #endregion
                        //Thi.Nguyen: 3-Oct-2020: add condition for Floor NCR
                        #region Add task and send email when ncr(not FloorNCR) approval
                        if (!_INCRManagementService.CheckFloorNCR(NCRNUM))
                        {
                            //Add task when NCR approval
                            resApp = _iTaskManagementService.AddTaskNCRDispositionApproval(NCRNUM, Partial: res.obj.ToString());
                            //Send Email Add Task
                            if (resApp.obj.ToString() == StatusInDB.DispositionApproved | res.obj.ToString() == "ApprovePartial")
                            {
                                _emailService.SendEmailAutoAssignTask(mailTemplate: linkNCR, mailPath: path, NCRNUM: NCRNUM);
                            }
                        }
                        #endregion

                        #region Send Email when NCR APPROVAL
                        NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM.Trim());
                        if (NCR.STATUS.Trim() == StatusInDB.DispositionApproved | res.obj.ToString() == "ApprovePartial")
                        {
                            List<AspNetUser> Users = _IUserService.GetUserOfNCRApproval(NCRNUM);
                            foreach (AspNetUser mail in Users)
                            {
                                _emailService.SendEmailDispositionApproval(mailTemplate: MailTemplate, mailPath: path, RecipientName: mail.FullName, email: mail.Email, NCRnumber: NCRNUM, linkNCR: linkNCR, comment: NCR.Comment);
                            }

                            #region Send template 4 if Corrective action is required and response is vendor for B: Quality
                            _emailService.SendEmailDispositionApproval(linkNCR, path, NCR.NCR_NUM);
                            #endregion
                        }
                        #endregion


                        return Json(new { success = res.success, res.message });
                    }

                    else
                    {
                        return Json(new { success = false, message = "Password incorect !" });
                    }
                }

                else
                {
                    return Json(new { success = false, message = "You don't have a signature, Please add your signature or contact IT team !" });
                }
            }
        }
        public ActionResult RejectNCRWH(string NCRNUM, string reason)
        {
            string IDuser = User.Identity.GetUserId();
            string name = _IUserService.GetNameById(IDuser);
            Result res = new Result();

            res = _INCRManagementService.RejectNCRWH(NCRNUM, reason, IDuser);
            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["rejectNCR"];
                string role = EmailCreateConfig.Split('|')[0];
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                string MailTemplate = EmailCreateConfig.Split('|')[2];
                NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                if (role == "ALL")
                {

                }
                else if (role == "NCR")
                {
                    List<AspNetUser> ENGs = _IUserService.GetENGOfNCR(NCR.NCR_NUM);
                    AspNetUser OPE = _IUserService.GetSubmiterNCR(NCR.NCR_NUM);
                    //     var Approvers = _IUserService.GetApproverByNCRNUM(NCR.NCR_NUM, new string[] { _IUserService.GetENGINEERINGRoleId() });
                    _emailService.SendEmailRejectNCR(mailTemplate: MailTemplate, mailPath: path, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: NCR.Comment, reason: reason,user:name, ENGs, OPE, new List<AspNetUser>());
                }
            }
            return Json(new { success = res.success, message = res.message });
        }

        public ActionResult RejectNCREng(string Id, string NCRNUM, string ApproverId, string reason, string password)
        {
            if (string.IsNullOrEmpty(Id) | string.IsNullOrEmpty(NCRNUM) | string.IsNullOrEmpty(ApproverId) | string.IsNullOrEmpty(reason))
            {
                return Json(new { success = false, message = "Data request is not valid" });
            }
            string name = _IUserService.GetNameById(User.Identity.GetUserId());
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            bool isValid = false;
            ApplicationSignInManager _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            Result res = new Result();

            if (User.Identity.GetUserId() == ApproverId)
            {
                res = _INCRManagementService.RejectNCREng(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
            }
            else
            {
                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Approver's password not valid" });
                }

                Models.Account.ApplicationUser ApprovalUser = UserManager.FindById(ApproverId);
                isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
                if (isValid == true)
                {
                    res = _INCRManagementService.RejectNCREng(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
                }
                else
                {
                    return Json(new { success = false, message = "Password incorect !" });
                }
            }
            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["rejectNCR"];
                string role = EmailCreateConfig.Split('|')[0];
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                string MailTemplate = EmailCreateConfig.Split('|')[2];
                NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                if (role == "ALL")
                {

                }
                else if (role == "NCR")
                {
                    List<AspNetUser> ENGs = _IUserService.GetENGOfNCR(NCR.NCR_NUM);
                    AspNetUser OPE = _IUserService.GetSubmiterNCR(NCR.NCR_NUM);
                    List<AspNetUser> Approvers = _IUserService.GetApproverByNCRNUM(NCR.NCR_NUM, new string[] { _IUserService.GetENGINEERINGRoleId() });
                    _emailService.SendEmailRejectNCR(mailTemplate: MailTemplate, mailPath: path, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: NCR.Comment, reason: reason,user:name, ENGs, OPE, Approvers);
                }
            }
            return Json(new { success = res.success, message = res.message });
        }
        public ActionResult RejectNCR(string Id, string NCRNUM, string ApproverId, string reason, string password)
        {
            if (string.IsNullOrEmpty(Id) | string.IsNullOrEmpty(NCRNUM) | string.IsNullOrEmpty(ApproverId) | string.IsNullOrEmpty(reason))
            {
                return Json(new { success = false, message = "Data request is not valid" });
            }
            string name = _IUserService.GetNameById(User.Identity.GetUserId());
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            bool isValid = false;
            ApplicationSignInManager _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            Result res = new Result();

            if (User.Identity.GetUserId() == ApproverId)
            {
                res = _INCRManagementService.RejectNCR(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
            }
            else
            {
                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Approver's password not valid" });
                }

                Models.Account.ApplicationUser ApprovalUser = UserManager.FindById(ApproverId);
                isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
                if (isValid == true)
                {
                    res = _INCRManagementService.RejectNCR(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
                }
                else
                {
                    return Json(new { success = false, message = "Password incorect !" });
                }
            }
            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["rejectNCR"];
                string role = EmailCreateConfig.Split('|')[0];
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                string MailTemplate = EmailCreateConfig.Split('|')[2];
                NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                if (role == "ALL")
                {

                }
                else if (role == "NCR")
                {
                    List<AspNetUser> ENGs = _IUserService.GetENGOfNCR(NCR.NCR_NUM);
                    AspNetUser OPE = _IUserService.GetSubmiterNCR(NCR.NCR_NUM);
                    List<AspNetUser> Approvers = _IUserService.GetApproverByNCRNUM(NCR.NCR_NUM, new string[] { _IUserService.GetENGINEERINGRoleId() });
                    _emailService.SendEmailRejectNCR(mailTemplate: MailTemplate, mailPath: path, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: NCR.Comment, reason: reason,user: name, ENGs, OPE, Approvers);
                }
            }
            return Json(new { success = res.success, message = res.message });
        }

        [HttpPost]
        public ActionResult AssignNCR(string Id, string NCRNUM, string ApproverId, string reason, string password, string oldApproverId)
        {
            if (string.IsNullOrEmpty(oldApproverId) | string.IsNullOrEmpty(Id) | string.IsNullOrEmpty(NCRNUM) | string.IsNullOrEmpty(ApproverId) | string.IsNullOrEmpty(reason))
            {
                return Json(new { success = false, message = "Data request is not valid" });
            }
            bool IsChairm = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            bool WHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            string name = _IUserService.GetNameById(User.Identity.GetUserId());
            if (IsChairm || WHMRB)
            {
                Result result = _INCRManagementService.AssignNCR(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
                if (result.success)
                {
                    string EmailCreateConfig = ConfigurationManager.AppSettings["reAssignNCR"];
                    string role = EmailCreateConfig.Split('|')[0];
                    string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                    string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                    string MailTemplate = EmailCreateConfig.Split('|')[2];
                    NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                    string[] arrApproverId = _IUserService.GetApproverByNCRNUM(NCRNUM, null).Select(x => x.Id).ToArray();
                    AspNetUser MailOfUser = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(ApproverId));

                    _emailService.SendEmailReAssignNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: MailOfUser.FullName, email: MailOfUser.Email, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: reason, reason: reason,user: name);

                }
                return Json(new { success = result.success, message = result.message });
            }
            else
            {


            Result res = new Result();

            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            bool isValid = false;
            ApplicationSignInManager _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

            if (User.Identity.GetUserId() == oldApproverId)
            {
                res = _INCRManagementService.AssignNCR(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
            }
            else
            {
              
                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Approver's password not valid" });
                }

                Models.Account.ApplicationUser ApprovalUser = UserManager.FindById(oldApproverId);
                isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
                if (isValid == true)
                {
                    res = _INCRManagementService.AssignNCR(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId());
                }
                else
                {
                    return Json(new { success = false, message = "Password incorect !" });
                }
            }
            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["reAssignNCR"];
                string role = EmailCreateConfig.Split('|')[0];
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                string MailTemplate = EmailCreateConfig.Split('|')[2];
                NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                string[] arrApproverId = _IUserService.GetApproverByNCRNUM(NCRNUM, null).Select(x => x.Id).ToArray();
                AspNetUser MailOfUser = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(ApproverId));

                _emailService.SendEmailReAssignNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: MailOfUser.FullName, email: MailOfUser.Email, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: reason, reason: reason,user: name);

            }
            return Json(new { success = res.success, message = res.message });
            }
        }
        [HttpPost]
        public ActionResult AssignNCREng(string Id, string NCRNUM, string ApproverId, string reason, string password, string oldApproverId,string roleid)
        {
            if (string.IsNullOrEmpty(oldApproverId) | string.IsNullOrEmpty(Id) | string.IsNullOrEmpty(NCRNUM) | string.IsNullOrEmpty(ApproverId) | string.IsNullOrEmpty(reason))
            {
                return Json(new { success = false, message = "Data request is not valid" });
            }
            bool IsChairm = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            bool WHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            string name = _IUserService.GetNameById(User.Identity.GetUserId());
            if (IsChairm || WHMRB)
            {
                Result result = _INCRManagementService.AssignNCREng(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId(),roleid);
                if (result.success)
                {
                    string EmailCreateConfig = ConfigurationManager.AppSettings["reAssignNCREng"];
                    string role = EmailCreateConfig.Split('|')[0];
                    string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                    string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                    string MailTemplate = EmailCreateConfig.Split('|')[2];
                    NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                    string[] arrApproverId = _IUserService.GetApproverByNCRNUM(NCRNUM, null).Select(x => x.Id).ToArray();
                    AspNetUser MailOfUser = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(ApproverId));

                    _emailService.SendEmailReAssignNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: MailOfUser.FullName, email: MailOfUser.Email, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: reason, reason: reason,user:name);

                }
                return Json(new { success = result.success, message = result.message });
            }
            else
            {

            Result res = new Result();

            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            bool isValid = false;
            ApplicationSignInManager _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

            if (User.Identity.GetUserId() == oldApproverId)
            {
                res = _INCRManagementService.AssignNCREng(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId(),roleid);
            }
            else
            {
                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Approver's password not valid" });
                }

                Models.Account.ApplicationUser ApprovalUser = UserManager.FindById(oldApproverId);
                isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
                if (isValid == true)
                {
                    res = _INCRManagementService.AssignNCREng(Id, NCRNUM, ApproverId, reason, UserIdLogin: User.Identity.GetUserId(),roleid);
                }
                else
                {
                    return Json(new { success = false, message = "Password incorect !" });
                }
            }
            if (res.success)
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["reAssignNCREng"];
                string role = EmailCreateConfig.Split('|')[0];
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = NCRNUM.Trim() }, Request.Url.Scheme);

                string MailTemplate = EmailCreateConfig.Split('|')[2];
                NCR_HDR NCR = _INCRManagementService.GetNCR_HDR(NCRNUM);
                string[] arrApproverId = _IUserService.GetApproverByNCRNUM(NCRNUM, null).Select(x => x.Id).ToArray();
                AspNetUser MailOfUser = _IUserService.GetAllUser().FirstOrDefault(x => x.Id.Equals(ApproverId));

                _emailService.SendEmailReAssignNCR(mailTemplate: MailTemplate, mailPath: path, RecipientName: MailOfUser.FullName, email: MailOfUser.Email, NCRnumber: NCRNUM.Trim(), linkNCR: urlsent, comment: reason, reason: reason,user:name);

            }
            return Json(new { success = res.success, message = res.message });
            }
        }
        public string SaveFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }

            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute + "-" + date.Second + "-" + date.Millisecond;
            virtualPath = relativePath + returnPath;
            //  string FolderPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);
            //  string FolderPath = virtualPath;
            if (!Directory.Exists(virtualPath))
            {
                Directory.CreateDirectory(virtualPath);
            }

            string FileName = file.FileName;
            if (Request.Browser.Browser.Contains("InternetExplorer") || Request.Browser.Browser.Contains("IE"))
            {
                FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
            }
            file.SaveAs(Path.Combine(virtualPath, FileName));
            return returnPath + "/" + FileName;
        }
        public string SaveFileADDIN(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }

            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute;
            virtualPath = relativePath + returnPath;
            //  string FolderPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);
            //  string FolderPath = virtualPath;
            if (!Directory.Exists(virtualPath))
            {
                Directory.CreateDirectory(virtualPath);
            }

            string FileName = file.FileName;
            if (Request.Browser.Browser.Contains("InternetExplorer") || Request.Browser.Browser.Contains("IE"))
            {
                FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
            }
            file.SaveAs(Path.Combine(virtualPath, FileName));
            return returnPath + "/" + FileName;
        }

        [HttpPost]
        public string SaveFileReview(HttpPostedFileBase ReviewFile)
        {
            if (ReviewFile == null)
            {
                return "";
            }

            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = Guid.NewGuid().ToString();
            virtualPath = $"{relativePath}TEMP/{ returnPath}";
            //  string FolderPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);
            //  string FolderPath = virtualPath;
            if (!Directory.Exists(virtualPath))
            {
                Directory.CreateDirectory(virtualPath);
            }

            string FileName = ReviewFile.FileName;
            if (Request.Browser.Browser.Contains("InternetExplorer") || Request.Browser.Browser.Contains("IE"))
            {
                FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
            }
            ReviewFile.SaveAs(Path.Combine(virtualPath, FileName));
            string protocol = Request.Url.Scheme;
            string filepath = Url.Content($"{ConfigurationManager.AppSettings["uploadPath"]}TEMP/{returnPath}/{FileName}");
            return filepath;
        }

        [HttpPost]
        public ActionResult MegerNCR(string NCRNUM)
        {
            if (string.IsNullOrEmpty(NCRNUM))
            {
                return Json(new { success = false, message = $"NCR {NCRNUM} is not exist !" });
            }

            #region Allow Meger NCR
            Result res = _INCRManagementService.MergeNCR(NCRNUM);
            return Json(new { res.success, res.message });
            #endregion

            // Deny Meger NCR
            //return Json(new { success = false, message = "Deny Meger NCR" });

        }

        private long GetSizeOfFile(string eVI_PATH)
        {
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"] + eVI_PATH);
            FileInfo f = new FileInfo(relativePath);
            return f.Length;
        }

    }
}
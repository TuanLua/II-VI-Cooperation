using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using II_VI_Incorporated_SCM.Services;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.IO;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class NCRApprovalController : Controller
    {
        private readonly INCRManagementService _INCRManagementService;
        private readonly IReciverService _IReciverService;
        private readonly IUserService _IUserService;
        private readonly ITaskManagementService _iTaskManagementService;
        LogWriter log = new LogWriter("");

        public NCRApprovalController(INCRManagementService INCRManagementService, IReciverService IReciverService, IUserService IUserService, ITaskManagementService iTaskManagementService)
        {
            _INCRManagementService = INCRManagementService;
            _IReciverService = IReciverService;
            _IUserService = IUserService;
            _iTaskManagementService = iTaskManagementService;
        }

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set { _signInManager = value; }
        //}


        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewApproval(string NCR_NUM, string Status)
        {
            if (string.IsNullOrEmpty(NCR_NUM)) return RedirectToAction("Index");
            double defect = 0;
            ViewBag.STT = Status;
            ViewBag.IsOPE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.OPE);
            ViewBag.IsMRBTeam = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.MRBTeam);
            ViewBag.IsWHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            ViewBag.IsSQE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.SQE);
            ViewBag.IsBuyer = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Buyer);
            ViewBag.IsChairm = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            //ViewBag.ListMRBTeam = new SelectList(_IUserService.GetListUserByGroupName(UserGroup.MRBTeam), "Id", "FullName");
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
            ViewBag.isOPEOwner = _INCRManagementService.GetOPEOwner(User.Identity.GetUserId(), NCR_NUM);
            //ViewBag.List.ListQTYASS = new SelectList(_IUserService.GetListUserQTYASS("QUALITY ASSURANCE"), "Id", "FullName");
            //ViewBag.List.ListENGINEERING = new SelectList(_IUserService.GetListUserQTYASS("ENGINEERING"), "Id", "FullName");
            //ViewBag.List.ListAFGASSYE = new SelectList(_IUserService.GetListUserQTYASS("AFG/ASSYE"), "Id", "FullName");
            //ViewBag.List.ListPURCHASING = new SelectList(_IUserService.GetListUserQTYASS("PURCHASING"), "Id", "FullName");
            var obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
            obj.URL = _INCRManagementService.getUrlNcr(NCR_NUM);
            obj.Idfile = _INCRManagementService.getIDfile(NCR_NUM);
            //var url = obj.URL;
            //if (url != null)
            //{
            //    url = url.Replace("~", "..");
            //}
            //obj.URL = url;
            obj.ListRespon = _INCRManagementService.getListRESPON();
            obj.ListDispo = _INCRManagementService.getListDispo();
            obj.ListAddition = _INCRManagementService.getListAddition();
            var appr_model = _INCRManagementService.GetListUserApprovalByNcrNum(NCR_NUM);
            if (obj != null)
            {
                ViewBag.Status = obj.STATUS.Trim();
                obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);

                obj.ListdefectprocessString = _INCRManagementService.GetInresultProcessString(NCR_NUM);

                obj.ListAdditional = _INCRManagementService.GetAdditional(NCR_NUM);
                int count = 1;
                foreach (var item in obj.Listdefectprocess)
                {
                    defect = Convert.ToDouble(item.QTY + defect);
                    foreach (var tmp in obj.ListAdditional)
                    {
                        if (tmp.ITEM.Trim() == item.ITEM.Trim())
                        {
                            tmp.NO = count;
                        }
                    }
                    count++;
                }
                ViewBag.HasVendor = false;
                foreach (var item in obj.ListdefectprocessString)
                {
                    if (item.RESPONSE == CONFIRMITY_RESPON.ID_VENDOR)
                    {
                        ViewBag.HasVendor = true;
                        break;
                    }
                }
                ViewBag.ListAllUsers = _IUserService.GetAllUser().ToList();
                obj.ListUSerAppr = new List<UserApproval>();
                if (appr_model != null)
                {
                    var str_user = _IUserService.GetNameById(appr_model.QUALITY);
                    UserApproval tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.IsAppr = appr_model.QUALITY_COMFIRM;
                    tmp.DateAppr = appr_model.QUALITY_DATE.HasValue ? appr_model.QUALITY_DATE.Value.ToString("MM/dd/yyyy") : "";
                    tmp.IdUser = appr_model.QUALITY;
                    obj.ListUSerAppr.Add(tmp);

                    str_user = _IUserService.GetNameById(appr_model.ENGIEERING);
                    tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.IsAppr = appr_model.ENGIEERING_CONFIRM;
                    tmp.DateAppr = appr_model.ENGIEERING_DATE.HasValue ? appr_model.ENGIEERING_DATE.Value.ToString("MM/dd/yyyy") : "";
                    tmp.IdUser = appr_model.ENGIEERING;
                    obj.ListUSerAppr.Add(tmp);

                    str_user = _IUserService.GetNameById(appr_model.MFG);
                    tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.IsAppr = appr_model.MFG_CONFIRM;
                    tmp.DateAppr = appr_model.MFG_DATE.HasValue ? appr_model.MFG_DATE.Value.ToString("MM/dd/yyyy") : "";
                    tmp.IdUser = appr_model.MFG;
                    obj.ListUSerAppr.Add(tmp);

                    str_user = _IUserService.GetNameById(appr_model.PURCHASING);
                    tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.IsAppr = appr_model.PURCHASING_CONFIRM;
                    tmp.DateAppr = appr_model.PURCHASING_DATE.HasValue ? appr_model.PURCHASING_DATE.Value.ToString("MM/dd/yyyy") : "";
                    tmp.IdUser = appr_model.PURCHASING;
                    obj.ListUSerAppr.Add(tmp);
                }
                if (obj.ListUSerAppr.Count < 4)
                {
                    int lenght = obj.ListUSerAppr.Count;
                    for (int i = 0; i < 4 - lenght; i++)
                    {
                        obj.ListUSerAppr.Add(new UserApproval());
                    }
                }
                obj.defect = defect;
                ViewBag.SEC = obj.SEC;
                return View(obj);
            }
            return RedirectToAction("Index", "NCR", null);
        }
        //[HttpPost]
        //public ActionResult ViewApproval(NCRManagementViewModel model)
        //{
        //    try
        //    {
        //        _INCRManagementService.UpdateNCRForIQC(model);
        //        return Json(new { success = true });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = false });
        //    }
        //}
        //[HttpGet]
        //public ActionResult CreateApprovalProcess(string NCR_NUM)
        //{
        //    var obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
        //    if (obj != null)
        //    {
        //        obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);
        //        return View(obj);
        //    }
        //    else
        //        return View(new NCRManagementViewModel());
        //}
        //[HttpPost]
        //public ActionResult CreateApprovalProcess(NCRManagementViewModel model)
        //{
        //    try
        //    {
        //        // _INCRManagementService.UpdateNCRForProcess(model);
        //        return Json(new { success = true });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = false });
        //    }
        //}


        [HttpPost]
        public ActionResult GetListDefectProcess(string NCR_NUM)
        {
            try
            {
                var result = _INCRManagementService.GetInresultProcess(NCR_NUM);
                return Json(new { result = result, JsonRequestBehavior.AllowGet });
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
                var result = _IReciverService.GetInresult(receiver);
                return Json(new { result = result, JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { result = new List<INS_RESULT_DEFECTViewModel>(), JsonRequestBehavior.AllowGet });
            }

        }

        public JsonResult GetDropdownlistDesForPro(List<string> id)
        {
            var listnc = _IReciverService.GetDropdownlistDesForPro(id);
            return Json(listnc);
        }

        public JsonResult GetDropdownlistDecriptByIdDefect(List<string> id)
        {
            var listnc = _IReciverService.GetDropdownlistDecriptByIdDefect(id);
            return Json(listnc);
        }
        [HttpGet]
        public ActionResult EditNCRForIQC(string NCR_NUM)
        {
            double defective = 0;
            ViewBag.ListMRBTeam = new SelectList(_IUserService.GetListUserByGroupName(UserGroup.MRBTeam), "Id", "FullName");
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
            var obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
            if (obj != null)
            {
                obj.Listdefectprocess = _INCRManagementService.GetInresultProcessString(NCR_NUM);
                foreach (var item in obj.Listdefectprocess)
                {
                    defective = Convert.ToDouble(item.QTY + defective);
                }
                obj.defect = defective;
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
            var list = _INCRManagementService.GetListNCRApproval();
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult PrintNCR(string NCR_NUM)
        {
            // NCR_NUM = "ABCd";
            var obj = PrintNCRTEMP(NCR_NUM);
            if (obj != null)
            {
                //return View(obj);
                return File($"{ConfigurationManager.AppSettings["uploadPath"]}MERGE_EVIDENT/Merge_{NCR_NUM}_Evident.pdf", "application/pdf");
            }
                return View(new NCRManagementViewModel());
        }

        public ActionResult CreateNCREvident(string NCR_NUM)
        {
            var obj = PrintNCRTEMP(NCR_NUM);
            if (obj != null) return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        private NCRManagementViewModel PrintNCRTEMP(string NCR_NUM)
        {
            double defect = 0;
            var appr_model = _INCRManagementService.GetListUserApprovalByNcrNum(NCR_NUM);
            var obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
            obj.ListAdditional = _INCRManagementService.GetAdditional(NCR_NUM);
            obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);
            obj.ListdefectprocessString = _INCRManagementService.GetInresultProcessString(NCR_NUM);
            obj.ListNC_Group = _INCRManagementService.GetListNC_GRP_DESC();
            obj.ListRespon = _INCRManagementService.getListRESPON();
            obj.ListDispo = _INCRManagementService.getListDispo();
            foreach (var item in obj.ListdefectprocessString)
            {
                defect = Convert.ToDouble(item.QTY + defect);
            }
            if (obj != null)
            {
                ViewBag.ListAllUsers = _IUserService.GetAllUser().ToList();
                obj.ListUSerAppr = new List<UserApproval>();
                if (appr_model != null)
                {
                    var str_user = _IUserService.GetNameById(appr_model.QUALITY);
                    var str_sign = _IUserService.GetSignatureById(appr_model.QUALITY);
                    
                    UserApproval tmp = new UserApproval();
                    tmp.IdUser = appr_model.QUALITY;
                    tmp.FullName = str_user;
                    tmp.Signature = str_sign;
                    tmp.IsAppr = appr_model.QUALITY_COMFIRM;
                    tmp.DateAppr = appr_model.QUALITY_DATE.HasValue ? appr_model.QUALITY_DATE.Value.ToString("MM/dd/yyyy") : "";
                    obj.ListUSerAppr.Add(tmp);

                    str_user = _IUserService.GetNameById(appr_model.ENGIEERING);
                    str_sign = _IUserService.GetSignatureById(appr_model.ENGIEERING);
                    
                    tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.Signature = str_sign;
                    tmp.IdUser = appr_model.ENGIEERING;
                    tmp.IsAppr = appr_model.ENGIEERING_CONFIRM;
                    tmp.DateAppr = appr_model.ENGIEERING_DATE.HasValue ? appr_model.ENGIEERING_DATE.Value.ToString("MM/dd/yyyy") : "";

                    obj.ListUSerAppr.Add(tmp);
                    str_user = _IUserService.GetNameById(appr_model.MFG);
                    str_sign = _IUserService.GetSignatureById(appr_model.MFG);
                    
                    tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.Signature = str_sign;
                    tmp.IdUser = appr_model.MFG;
                    tmp.IsAppr = appr_model.MFG_CONFIRM;
                    tmp.DateAppr = appr_model.MFG_DATE.HasValue ? appr_model.MFG_DATE.Value.ToString("MM/dd/yyyy") : "";

                    obj.ListUSerAppr.Add(tmp);

                    str_user = _IUserService.GetNameById(appr_model.PURCHASING);
                    str_sign = _IUserService.GetSignatureById(appr_model.PURCHASING);
                    
                    tmp = new UserApproval();
                    tmp.FullName = str_user;
                    tmp.Signature = str_sign;
                    tmp.IdUser = appr_model.PURCHASING;
                    tmp.IsAppr = appr_model.PURCHASING_CONFIRM;
                    tmp.DateAppr = appr_model.PURCHASING_DATE.HasValue ? appr_model.PURCHASING_DATE.Value.ToString("MM/dd/yyyy") : "";

                    obj.ListUSerAppr.Add(tmp);
                }
                if (obj.ListUSerAppr.Count < 4)
                {
                    int lenght = obj.ListUSerAppr.Count;
                    for (int i = 0; i < 4 - lenght; i++)
                    {
                        obj.ListUSerAppr.Add(new UserApproval());
                    }
                }
                obj.defect = defect;

                if (obj.STATUS.Trim() != StatusInDB.Void)
                {
                    obj.URL = _INCRManagementService.getUrlNcr(NCR_NUM);
                    PrintNCREnvident(NCR_NUM, obj);
                }
                return (obj);
            }
            return null;
        }

        [HttpPost]
        public JsonResult SaveUserAprroval(string IdUser, string ncrnum, string password)
        {
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var isValid = false;
            var _signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            try
            {
                var iduser = (User.Identity.GetUserId());
                if (User.Identity.GetUserId() == IdUser)
                {
                    var Approval = _INCRManagementService.UpdateUserAprrovalDate(ncrnum.Trim(), IdUser);
                    return Json(new { Approval });

                }
                else
                {
                    var ApprovalUser = UserManager.FindById(IdUser);
                    isValid = _signInManager.UserManager.CheckPassword(ApprovalUser, password);
                    if (isValid == true)
                    {
                        var Approval = _INCRManagementService.UpdateUserAprrovalDate(ncrnum.Trim(), IdUser);
                        return Json(new { Approval });
                    }
                    else
                    {
                        return Json(new { message = "Password incorect !" });
                    }
                }

            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Please contact to admin! " });
            }

        }
        public ActionResult SaveDisposition(DispositionModel model, List<REWORK_COST> lstRework, List<NCR_DIS> lstDis, List<SCRAP_REASON> lstScrap, string qlity, string eng,string afg,string pur,string url)
        {
            string urlsent = Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = model.NCR_NUM },protocol: Request.Url.Scheme);
            bool chk = _INCRManagementService.SaveNCRHDRByDispositionModel(model);
            bool chk1 = true;
            bool chk2 = true;
            bool chk3 = true;
            var iduser = (User.Identity.GetUserId());
            if (lstRework != null && lstRework.Count > 0)
            {
                chk1 = _INCRManagementService.AddListREWORK(lstRework);
            }
            if (lstDis != null && lstDis.Count > 0)
            {
                chk2 = _INCRManagementService.AddListNCRDIS(lstDis);
            }
            if (lstScrap != null && lstScrap.Count > 0)
            {
                chk3 = _INCRManagementService.AddListSCRAP(lstScrap);
            }

            try
            {
                if (chk1 && chk2 && chk3 && chk)
                {

                    _INCRManagementService.UpdateStatusNCR(model.NCR_NUM, StatusInDB.WaitingForDispositionApproval, iduser);
                    //get respone .
                  //  for (var i = 0; i < model.lstResDis.Count; i++)
                //    {

                        //get respone .
                        for (var i = 0; i <= model.lstResDis.Count; i++)
                        {
                            if (model.lstResDis[i].respon == CONFIRMITY_RESPON.ID_VENDOR)
                            {
                                _INCRManagementService.SentMailRemindSQE(model.NCR_NUM, urlsent);
                                break;
                            }
                        }
                        _INCRManagementService.SentMailRemind(qlity, eng, afg, pur, model.NCR_NUM, urlsent);
                   // }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }

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
            var chek = _INCRManagementService.UpdateStatusNCR(ncrnumber, StatusInDB.DispositionApproved,"");

            if (!chek)
            {
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public JsonResult UpdateUser(string id, string userId, string ncrNum)
        {
            if (id != "")
            {
                _INCRManagementService.UpdateUserApproval(id, userId, ncrNum);
                if (userId != "")
                {
                    _INCRManagementService.SentEmailSubmitEdit(userId);
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public FileContentResult DownloadFile(int fileId)
        {
            string filePath = ConfigurationManager.AppSettings["uploadPath"];
            if (fileId != -1)
            {
                var sf = _INCRManagementService.GetFileWithFileID(fileId);
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
            string RootPath = ConfigurationManager.AppSettings["uploadPath"],
                FileMergePath = $"{RootPath}MERGE_EVIDENT/Merge_{NCR_NUM}_Evident.pdf"; ;
            var Idfile = _INCRManagementService.getIDfile(NCR_NUM);
            if (!string.IsNullOrEmpty(NCR_NUM))
            {
                var sf = _INCRManagementService.GetFileWithFileID(Idfile);
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

        static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial) viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null) throw new System.IO.FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new System.IO.StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public void PrintNCREnvident(string NCR_NUM, NCRManagementViewModel obj)
        {
            string html = RenderViewToString(ControllerContext, "~/views/NCRApproval/PrintNCR.cshtml", obj, true);
            string FileNameTemp = Guid.NewGuid() + "",
                   RootPath = ConfigurationManager.AppSettings["uploadPath"],
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
            EvoPdf.HtmlToPdfConverter htmlToPdfConverter = new EvoPdf.HtmlToPdfConverter();
            htmlToPdfConverter.LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw==";
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = new EvoPdf.PdfPageSize();
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = EvoPdf.PdfPageOrientation.Portrait;
            htmlToPdfConverter.ConvertHtmlToFile(html,
                Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority
                , FilePathTemp);

            EvoPdf.Document MergePdf = new EvoPdf.Document();
            MergePdf.LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw==";

            var firstDoc = new EvoPdf.Document(FilePathTemp);
            MergePdf.AppendDocument(firstDoc);
            var ividentDoc = new EvoPdf.Document();
            if (System.IO.File.Exists($"{RootPath}{obj.URL.Replace("/", "\\")}"))
            {
                ividentDoc = new EvoPdf.Document($"{RootPath}{obj.URL.Replace("/", "\\")}");
                MergePdf.AppendDocument(ividentDoc);
            }

            MergePdf.Save(FileMergePath);
            firstDoc.Close();
            ividentDoc.Close();
            System.IO.File.Delete(FilePathTemp);
            #endregion
        }

        [HttpPost]
        public ActionResult DeleteAddIns(string ncrnum, string item, string addins)
        {
            if (string.IsNullOrEmpty(ncrnum) || string.IsNullOrEmpty(item) || string.IsNullOrEmpty(addins)) return Json(new { success = false });
            return Json(new { success = _INCRManagementService.DeleteAddIns(ncrnum, item, addins) });
        }
        [HttpPost]
        public ActionResult EditAddIns(string ncrnum, string item, string addins, double qty, string remark)
        {
            if (string.IsNullOrEmpty(ncrnum) || string.IsNullOrEmpty(item) || string.IsNullOrEmpty(addins) || qty <= 0) return Json(new { success = false });
            return Json(new { success = _INCRManagementService.EditAddIns(ncrnum, item, addins, qty, remark) });
        }
        public ActionResult CheckApproal(string ncrnum)
        {
            CApproval Approval = _INCRManagementService.CheckApproval(ncrnum);
            return Json(new { Approval }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CloseNCR(string NCR_NUM)
        {
            var res = _iTaskManagementService.ExistsTask(NCR_NUM);
            if (!res)
            {
                _INCRManagementService.CloseNCR(NCR_NUM);
             
            }
            return Json(new { res }, JsonRequestBehavior.AllowGet);
        }
    }
}
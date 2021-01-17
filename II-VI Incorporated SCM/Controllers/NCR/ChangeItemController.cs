using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    public class ChangeItemController : Controller
    {
        private readonly ICCNService _ICCNService;
        private readonly INCRManagementService _INCRManagementService;
        private readonly IChangeItemService _IChangeItemService;
        private readonly IEmailService _IEmailService;
        private readonly IUserService _IUserService;
        public ChangeItemController(ICCNService iCCNService, INCRManagementService INCRManagementService, IChangeItemService iChangeItemService, IEmailService iEmailService, IUserService IUserService)
        {
            _ICCNService = iCCNService;
            _INCRManagementService = INCRManagementService;
            _IChangeItemService = iChangeItemService;
            _IEmailService = iEmailService;
            _IUserService = IUserService;
        }
        // GET: ChangeItem
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NCRChangeItem([DataSourceRequest]DataSourceRequest request)
        {
            List<ChangeItemViewmodel> list = _IChangeItemService.Getlistchangeitem();
            return Json(list.ToDataSourceResult(request));
        }
        public ActionResult NCRChangeItemByNCR([DataSourceRequest]DataSourceRequest request,string NCRNUM)
        {
            List<ChangeItemViewmodel> list = _IChangeItemService.Getlistchangeitembyncrnum(NCRNUM);
            return Json(list.ToDataSourceResult(request));
        }
        
        public ActionResult ChangeitemView()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewChangeItem(string CR_NUMBER)
        {
            ViewBag.IsWHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            ViewBag.IsChairm = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.CHAIRMAN);
            ChangeItemViewmodel crinfo = _IChangeItemService.getinfoCR(CR_NUMBER);
            string path = Url.Content(ConfigurationManager.AppSettings["uploadPath"]);
            crinfo.idfile = _IChangeItemService.getIDfile(CR_NUMBER);
            string url = crinfo.Linkactack;
            if (url != null)
            {
                url = path + url;
            }
            crinfo.Linkactack = url;
            return View(crinfo);
        }
        [HttpPost]
        public JsonResult SaveChangeItem(ChangeItemViewmodel model)
        {
            //var isDispoTungPhan = _IChangeItemService.CheckDispoSubmitChange(model.REF_NUM);
            //if(isDispoTungPhan.Count >0 )
            //{
            //    return Json(new { success = true, message = $@"NCR {model.REF_NUM} Disposition partial!!!" });
            //}
            var ischangeItem = _IChangeItemService.Checksubmitchangeitem(model.REF_NUM);
            if (ischangeItem)
            {
                return Json(new { success = true, message = $@"NCR {model.REF_NUM} A Change Item is waiting for resolving. Cannot submit the new one!" });
            }
            string ncrnumauto = _IChangeItemService.GetAutoCRNUM();
            string File_Upload_Url = "";

            if (model.Attacment != null)
            {
                File_Upload_Url = SaveFile(model.Attacment);
                            NCR_EVI crevi = new NCR_EVI
                            {
                                EVI_PATH = File_Upload_Url,
                                SEC ="CRNO",
                                NCR_NUM = ncrnumauto,
                            };
                _IChangeItemService.savefileEvident(crevi);
                        //_iCCNService.CreateNCR_EVI(ncrevi);
            }
            ChangeItem cr_hdr = new ChangeItem
            {
                CRID = ncrnumauto,
                Brief = model.Brief,
                UserSubmit = User.Identity.GetUserId(),
                Comment = model.Comments,
                CRStatus = "Created",
                CRType = CRTYPE.NCR,
                CRPriority = model.Priority,
                DateSubmit = DateTime.Now,
                DueDate = model.DateRequired,
                RefNumber = model.REF_NUM,
                Reason = model.Reason,
                OtherArtifactsImpacted = model.OtherAtifact,
                Attachment = File_Upload_Url,

            };
            bool save = _IChangeItemService.savechangeItem(cr_hdr);
            string email = _IChangeItemService.getinforChairmain(model.REF_NUM);
            string name = _IChangeItemService.getnameChairmain(model.REF_NUM);
            string link = Url.Action("ViewChangeItem", "ChangeItem", new { CR_NUMBER = ncrnumauto }, Request.Url.Scheme);
            if (save == true)
            {
                try
                {
                    string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                    _IEmailService.SendEmailSubmitChangeItemToChairman(path, "", name, email, ncrnumauto, link);
                }
                catch (Exception)
                {
                    return Json(new { success = true, message = $@"Create CR {ncrnumauto} success! | Send email to chairnman fail!!!" });
                }
            }
            return Json(new { success = true, message = $@"Create CR {ncrnumauto} success!" });
        }
        public string SaveFile(HttpPostedFileBase file)
        {
            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute + "-" + date.Second + "-" + date.Millisecond;
            virtualPath = relativePath + returnPath;
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
        public JsonResult RejectChangeSubmitItem(string comment, string crno, string ncnum)
        {
            string idchairmain = User.Identity.GetUserId();
            //  var emailuser = string.Join(";", _IChangeItemService.getinforuserapproval(crno));
            string emailsubmit = _IChangeItemService.getemailusersubmit(crno);
            string name = _IChangeItemService.getnameusersubmit(crno);
            string link = Url.Action("ViewChangeItem", "ChangeItem", new { CR_NUMBER = crno.Trim() }, Request.Url.Scheme);
            string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
            _IEmailService.SendEmailRejectChangeItem(path, "", name, emailsubmit, crno, link, comment);
            _IChangeItemService.updatestatusreject(crno, "Reject", comment, idchairmain);
            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult ApprovalChangeSubmitItem(string crno, string ncnum)
        {
            string id = User.Identity.GetUserId();
            List<AspNetUser> Users = _IChangeItemService.getinforMRBWH(ncnum);
            string link = Url.Action("ViewChangeItem", "ChangeItem", new { CR_NUMBER = crno.Trim() }, Request.Url.Scheme);
            string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
            foreach (var mail in Users)
            {
                _IEmailService.SendEmailApprovalChangeItem(path, "", mail.FullName, mail.Email, crno, link);
            }
            _IChangeItemService.updatestatus(crno, "Approve", id);
            return Json(new { success = true });
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
        [HttpPost]
        public JsonResult AcknowlegedChangeSubmitItem(string crno, string ncnum, string status)
        {
            LogWriter log = new LogWriter("");
            try
            {
                ChangeItemViewmodel obj = _IChangeItemService.getinfoCR(crno);
                string html = RenderViewToString(ControllerContext, "~/views/ChangeItem/ViewMailTemplate.cshtml", obj, true);
                string emailsubmit = _IChangeItemService.getemailusersubmit(crno);
                string nameusesubmit = _IChangeItemService.getnameusersubmit(crno);
                List<AspNetUser> Users = _IChangeItemService.getfulluserapproval(ncnum);
                string link = Url.Action("ViewChangeItem", "ChangeItem", new { CR_NUMBER = crno.Trim() }, Request.Url.Scheme);
                string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
                // _IEmailService.SendEmailAcknowapprovalChangeItem(path, "", "", emailuser, crno, link, html);
                foreach (var mail in Users)
                {
                    _IEmailService.SendEmailAcknowapprovalChangeItem(path, "", mail.FullName, mail.Email, crno, link, html);
                }
                _IEmailService.SendEmailAcknowapprovalChangeItem(path, "", nameusesubmit, emailsubmit, crno, link, html);
            }
           catch(Exception ex)
            {
                log.LogWrite(ex.ToString());
            }
            Result result = _IChangeItemService.updateallchangeitem(crno, ncnum);
            ///Send email for approver
            if (result.success == true)
            {
            string idWH = User.Identity.GetUserId();

                _IChangeItemService.updatestatusacknow(crno, "Acknowledged", idWH);
                return Json(new { success = true });
            }
            return Json(new { success = false, message=result });

        }
        public FileContentResult DownloadFile(int fileId)
        {
            string filePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using II_VI_Incorporated_SCM.Models.RTV;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class NCRRTVController : Controller
    {
        private readonly INCRManagementService _INCRManagementService;
        private readonly ICCNService _iCCNService;
        ////// GET: NCR_RTV

        public NCRRTVController(INCRManagementService INCRManagementService, ICCNService iCCNService)
        {
            _INCRManagementService = INCRManagementService;
            _iCCNService = iCCNService;
        }

        public ActionResult Index()
        {
            ViewBag.ListInspector = _INCRManagementService.GetUser();
            return View();

        }

        public ActionResult NCR_RTVList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _INCRManagementService.GetListNCR_RTV();
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult ViewNCRRTV(string NCR_NUM, string Status)
        {
            var check = _INCRManagementService.checkexistsRTVProcess(NCR_NUM);

            var stt = _INCRManagementService.getRTVstatus(NCR_NUM);

            return Json(new { jcheck = check, jstt = stt }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewRTV(string NCR_NUM)
        {
            double defect = 0;
            ViewBag.NCR_NUM = NCR_NUM;

            var list = _INCRManagementService.GetInresultProcessString(NCR_NUM);
            foreach (var item in list)
            {
                defect = Convert.ToDouble(item.QTY + defect);
            }
            ViewBag.defect = defect.ToString();
            return View();
        }
        public string SaveFile(HttpPostedFileBase file)
        {
            DateTime date = DateTime.Now;
            string relativePath = ConfigurationManager.AppSettings["uploadPathRTV"];
            string virtualPath = relativePath + date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute + "-" + date.Second + "-" + date.Millisecond;
            string FolderPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            string FileName = file.FileName;
            if (Request.Browser.Browser.Contains("InternetExplorer") || Request.Browser.Browser.Contains("IE"))
            {
                FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
            }
            file.SaveAs(Path.Combine(FolderPath, FileName));
            return virtualPath + "/" + FileName;
        }
        public ActionResult ProcessRTV(string NCR_NUM)
        {
            ViewBag.NCR_NUM = NCR_NUM;
            var obj = _INCRManagementService.getRTV(NCR_NUM);
            //if(obj.TypeRTV== II_VI_Incorporated_SCM.Services.TypeRTV.SHIPPED)
            var qty = Convert.ToInt32(obj.Qty);
            var url = obj.CreditFile;
            if (url != null)
            {
                url = url.Replace("~", "..");
            }
            obj.CreditFile = url;
            obj.Qty = qty;
            double defect = 0;
            var list = _INCRManagementService.GetInresultProcessString(NCR_NUM);
            foreach (var item in list)
            {
                defect = Convert.ToDouble(item.QTY + defect);
            }
            ViewBag.defect = defect.ToString();

            return View(obj);
        }

        public ActionResult CloseRTV(string NCR_NUM)
        {
            var obj = _INCRManagementService.getRTV(NCR_NUM);
            var qty = Convert.ToInt32(obj.Qty);
            obj.Qty = qty;
            double defect = 0;

            var list = _INCRManagementService.GetInresultProcessString(NCR_NUM);
            foreach (var item in list)
            {
                defect = Convert.ToDouble(item.QTY + defect);
            }
            ViewBag.defect = defect.ToString();
            return View(obj);
        }
        [HttpPost]
        public JsonResult addRTVProcess(RTVProccessViewModel rtv)
        {
            //string File_Upload_Url = SaveFile(model.File_Upload);
            string File_Upload_Url = "";
            string File_Upload_Url1 = "";
            var fileup1 = rtv.File_Upload1;
            var fileup = rtv.File_Upload;

            if (fileup != null)
            {
                //var file = _INCRManagementService.GetFile(rtv.NCR_NUMBER);
                //System.IO.File.Delete(Server.MapPath(file));
                File_Upload_Url = SaveFile(fileup);
                rtv.CreditFile = File_Upload_Url;
            }
            if (fileup1 != null)
            {
                //var file = _INCRManagementService.GetFile(rtv.NCR_NUMBER);
                //System.IO.File.Delete(Server.MapPath(file));
                File_Upload_Url1 = SaveFile(fileup1);
                rtv.CreditFile = File_Upload_Url1;
            }
            var insert = _INCRManagementService.SaveRTVProcess(rtv);
            return Json(new { success = insert });
        }

        public ActionResult UpdateRTV(RTVProccessViewModel rtv)
        {
            string File_Upload_Url = "";
            string File_Upload_Url1 = "";
            var fileup1 = rtv.File_Upload1;
            var fileup = rtv.File_Upload;
            if (fileup != null)
            {
                File_Upload_Url = SaveFile(fileup);

                rtv.CreditFile = File_Upload_Url;
            }
            if (fileup1 != null)
            {
                File_Upload_Url1 = SaveFile(fileup1);
                rtv.CreditFile = File_Upload_Url1;
            }
            var update = _INCRManagementService.UpdateRTVProcess(rtv);
            return Json(new { success = update });
        }
        [HttpPost]
        public ActionResult CompleteRTV(string NCR_NUM)
        {
            var check = _INCRManagementService.updatestatus(NCR_NUM);
            return Json(check, JsonRequestBehavior.AllowGet);
        }
    }
}
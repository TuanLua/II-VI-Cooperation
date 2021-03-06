using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.SOReview;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.SOReview
{
    public class SOReviewController : Controller
    {
        private readonly ISoReviewService _iSoReviewService;
        private readonly IUserService _IUserService;
        private readonly ITaskManagementService _iTaskManagementService;
        public SOReviewController(ISoReviewService iSoReviewService, IUserService IUserService, ITaskManagementService iTaskManagementService)
        {
            _iSoReviewService = iSoReviewService;
            _IUserService = IUserService;
            _iTaskManagementService = iTaskManagementService;
        }

        #region SoReview
        public ActionResult ReleaseforReview()
        {

            return View();
        }

        public ActionResult SOReviewList()
        {
            return View();
        }

        public ActionResult SoReViewListRead([DataSourceRequest] DataSourceRequest request)
        {
            List<sp_SOR_GetSoReview_Result> list = _iSoReviewService.GetListSoReview();
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult SOReviewDetail(string SoNo, string Date, string status,string planshipdate)
        {
            Date = Date.Substring(0, Date.IndexOf(" GMT"));
            DateTime dt;
            DateTime.TryParseExact(Date, "ddd MMM d yyyy hh:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
            var data = _iSoReviewService.GetSoReviewDetail(SoNo, dt, status);
            //set default item high qty
            int IDItemHigh = 1;
            if (data.Count > 0)
            {
                data.FirstOrDefault().OldEvidence = _iSoReviewService.GetListFileItem(IDItemHigh);
            }
            else
            {
                data.FirstOrDefault().OldEvidence = new List<tbl_SOR_Attached_ForItemReview>();
            }
            ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(SoNo, "SoReview");
            ViewBag.IsDaprt = _iSoReviewService.GetDepart(User.Identity.GetUserId());
            ViewBag.SoNo = SoNo;
            ViewBag.Date = dt.ToString("dd-MMM-yyyy");
            ViewBag.Status = status;
            ViewBag.planshipdate = planshipdate;
            return View(data);
        }
        [HttpPost]
        public JsonResult AddTaskForItemReview(string SoNo, string Date, string itemreview, string assignee)
        {
            Result res = _iSoReviewService.AddTaskForItemReview(SoNo, Date, itemreview, User.Identity.GetUserId(), assignee);
            return Json(new { res.success, message = res.message, obj = res.obj });
        }
        public JsonResult ReadTaksMantSoReview([DataSourceRequest] DataSourceRequest request, string taskNo)
        {
            return Json(_iTaskManagementService.GetListTaskMantSoreviewByID(taskNo).ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult UpdateSoReview(string id, string reviewresult, string comment,string islock)
        {
            int ID = Convert.ToInt32(id);
           
            SoReviewDetail data = new SoReviewDetail();
            data.Comment = comment;
            data.ReviewResult = reviewresult;
            data.IsLock = islock;
            Result res = _iSoReviewService.UpdateDataSoReview(data, ID);
            return Json(new { res.success, message = res.message, obj = res.obj });
        }
        [HttpPost]
        public JsonResult UpdateSoReviewFinish(string sono, string planshipdate, string isdefine,string Date)
        {
            SoReviewDetail data = new SoReviewDetail();
            if (isdefine == "true")
            {
                planshipdate = "TBD";
            }
            data.SONO = sono;
            data.PlanShipDate = planshipdate;
            data.DateDownLoad = DateTime.Parse(Date);
            Result res = _iSoReviewService.UpdateSoReviewFinish(data);
            return Json(new { res.success, message = "Finish sucess!", obj = res.obj });
        }
        [HttpPost]
        public JsonResult SaveFileUpLoadDocument()
        {
            int ID = 0;
            SoReviewDetail data = new SoReviewDetail();
            Result res = _iSoReviewService.UpdateDataSoReview(data, ID);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }

        public FileContentResult DownloadFile(int fileId, string type, string filepath)
        {
            string filePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);

            if (fileId != -1)
            {
                tbl_SOR_Attached_ForItemReview sf = _iSoReviewService.GetFileWithFileID(fileId);
                if (sf != null)
                {
                    string filePathFull = (filePath + sf.Attached_File);
                    byte[] file = GetMediaFileContent(filePathFull);
                    return File(file, MimeMapping.GetMimeMapping(sf.Attached_File), sf.Attached_File);
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
        [HttpPost]
        public JsonResult GetListUser()
        {
            var result = _iSoReviewService.GetDropdownlistUser();
            return Json(result);
        }

        public JsonResult ReViewReSult()
        {
            var result = _iSoReviewService.GetReviewResult();
            return Json(result);
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

        public string SaveFileADDINItem(HttpPostedFileBase file)
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
        public ActionResult AddFileforItemReview(string SO_NO, string Date,  string File, string ID )
        {
            DateTime date = DateTime.Now;
            string returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute;
            int id = int.Parse(ID);
            if (File != null)
            {
                var  datafiles = new tbl_SOR_Attached_ForItemReview
                {
                   SO_NO = SO_NO,
                    Attached_File = returnPath + "/" + File,
                   Download_Date = date,
                   Item_Idx = id
                };
                Result res = _iSoReviewService.SaveFileAttachedItemReview(datafiles);
                return Json(new { success = res.success, message = res.obj });
            }
            return Json(new { success = false, message = "Save file not sucess!" });
        }


        [HttpPost]
        public ActionResult DeleteFileReview(string id)
        {
            Result res = _iSoReviewService.DeleteDataFileofItemReview(id);
            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region MaterData

        #region PIC ReView

        public ActionResult PICReviewList()
        {
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planer);
            var lstPIC = _iSoReviewService.GetListPIC();
            return View(lstPIC);
        }

        [HttpPost]
        public JsonResult SavePICReview(string dept, string pic)
        {
            PICReviewmodel data = new PICReviewmodel();
            data.Dept = dept;
            data.Pic = pic;
            Result res = _iSoReviewService.SaveDataPICReview(data);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }

        [HttpPost]
        public JsonResult UpdatePICReview(string id, string dept, string pic)
        {
            int ID = Convert.ToInt32(id);
            PICReviewmodel data = new PICReviewmodel();
            data.Dept = dept;
            data.Pic = pic;
            Result res = _iSoReviewService.UpdateDataPICReview(data, ID);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }


        [HttpPost]
        public ActionResult DeletePICReview(string id)
        {
            Result res = _iSoReviewService.DeleteDataPICReview(id);
            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Item Review
        public ActionResult ItemReviewList()
        {
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planer);
            var lstPIC = _iSoReviewService.GetListItem();
            return View(lstPIC);
        }

        [HttpPost]
        public JsonResult SaveItemReview(string dept, string pic, string isdefault)
        {
            ItemReviewmodel data = new ItemReviewmodel();
            data.Dept = dept;
            data.ItemReview = pic;
            data.Isdefault = isdefault;
            Result res = _iSoReviewService.SaveDataItemReview(data);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }

        [HttpPost]
        public JsonResult UpdateItemReview(string id, string dept, string pic, string isdefault)
        {

            int ID = Convert.ToInt32(id);
            ItemReviewmodel data = new ItemReviewmodel();
            data.Dept = dept;
            data.ItemReview = pic;
            data.Isdefault = isdefault;
            Result res = _iSoReviewService.UpdateDataItemReview(data, ID);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }


        [HttpPost]
        public ActionResult DeleteItemReview(string id)
        {
            Result res = _iSoReviewService.DeleteDataItemReview(id);
            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Family

        public ActionResult FamilyReviewList()
        {
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planer);
            var lstPIC = _iSoReviewService.GetListFamily();
            return View(lstPIC);
        }

        [HttpPost]
        public JsonResult SaveFamilyReview(string dept, double pic)
        {
            FamilyReviewmodel data = new FamilyReviewmodel();
            data.Family = dept;
            data.MaxQty = pic;
            // data.Isdefault = isdefault;
            Result res = _iSoReviewService.SaveDataFamilyReview(data);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }

        [HttpPost]
        public JsonResult UpdateFamilyReview(string id, string dept, double pic)
        {
            int ID = Convert.ToInt32(id);
            FamilyReviewmodel data = new FamilyReviewmodel();
            data.Family = dept;
            data.MaxQty = pic;
            Result res = _iSoReviewService.UpdateDataFamilyReview(data, ID);
            return Json(new { res.success, message = "Create sucess!", obj = res.obj });
        }


        [HttpPost]
        public ActionResult DeleteFamilyReview(string id)
        {
            Result res = _iSoReviewService.DeleteDataItemReview(id);
            return Json(new { res.success, res.message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        #region Report

        public ActionResult ListSoWithDetailReview()
        {
           ViewBag.Part = _iSoReviewService.GetdropdownPart();
            ViewBag.SoNo = _iSoReviewService.GetdropdownSoReview();
            return View();
        }

        public ActionResult RiskShipByValue()
        {

            return View();
        }

        public ActionResult OTDByLine()
        {
            return View();
        }
        #endregion


    }
}
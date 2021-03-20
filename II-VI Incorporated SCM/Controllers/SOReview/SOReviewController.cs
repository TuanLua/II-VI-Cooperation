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
        public ActionResult SoReViewReaLeaseRead([DataSourceRequest] DataSourceRequest request)
        {
            List<sp_SOR_GetSoOpen_Result1> list = _iSoReviewService.GetListReleaseSoReview();
            return Json(list.ToDataSourceResult(request));
        }
        public JsonResult ReleaseSo()
        {
            bool res = _iSoReviewService.RealeaseSo();
            return Json(new { res, message = "Release succes!"});
        }

        public ActionResult SOReviewList()
        {
            return View();
        }

        public ActionResult SoReViewListRead([DataSourceRequest] DataSourceRequest request)
        {
            List<sp_SOR_GetSoReview_Result1> list = _iSoReviewService.GetListSoReview();
            return Json(list.ToDataSourceResult(request));
        }

        
             [HttpPost]
        public JsonResult LockSoReview(string SoNo,string item,string Date,string islock)
        {
            DateTime   date=   DateTime.Parse(Date);
            Result res = _iSoReviewService.LockSoReview(SoNo,item, date, islock);
            return Json(new { res.success, message = res.message, obj = res.obj });
        }
        public ActionResult SOReviewDetail(string SoNo, string Date, string status,string planshipdate,string item)
        {
            Date = Date.Substring(0, Date.IndexOf(" GMT"));
            DateTime dt;
            DateTime.TryParseExact(Date, "ddd MMM d yyyy hh:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
            var isLock = _iSoReviewService.GetIsLockBySo(SoNo, dt, item);
            var data = _iSoReviewService.GetSoReviewDetail(SoNo, dt, status,item.Trim());
            List<tbl_SOR_Attached_ForItemReview> lstFile = new List<tbl_SOR_Attached_ForItemReview>();
            //set default item high qty
            if (data.Count > 0)
            {
                foreach (var item1 in data)
                {
                    if(item1.DeptReview == "HIGH_VOLUME")
                    {
                        lstFile = _iSoReviewService.GetListFileItem(SoNo, dt, item1.ID, item);
                    }
                  
                }
                data.FirstOrDefault().OldEvidence = lstFile;
            }
            else
            {
                SoReviewDetail example = new SoReviewDetail();
                data.Add(example);
                data.FirstOrDefault().OldEvidence = new List<tbl_SOR_Attached_ForItemReview>();
            }
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planner);
            ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(SoNo, "SoReview");
            ViewBag.IsDaprt = _iSoReviewService.GetDepart(User.Identity.GetUserId());
            ViewBag.SoNo = SoNo;
            ViewBag.Date = dt.ToString("dd-MMM-yyyy");
            ViewBag.Status = status;
            ViewBag.IsLock = isLock;
            ViewBag.Item = item;
            if (planshipdate != "null" || planshipdate!= "TBD")
            {
                var dates = DateTime.Parse(planshipdate);
                ViewBag.planshipdate = dates.ToString("dd-MMM-yyyy"); ;
            }
            else
            {
                ViewBag.planshipdate = planshipdate;
            }

            return View(data);
        }
        [HttpPost]
        public JsonResult AddTaskForItemReview(string SoNo, string Date, string itemreview, string assignee,string item,string taskname)
        {
            Result res = _iSoReviewService.AddTaskForItemReview(SoNo, Date, itemreview, User.Identity.GetUserId(), assignee,item, taskname);
            return Json(new { res.success, message = res.message, obj = res.obj });
        }
        public JsonResult ReadTaksMantSoReview([DataSourceRequest] DataSourceRequest request, string taskNo)
        {
            return Json(_iTaskManagementService.GetListTaskMantSoreviewByID(taskNo).ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult UpdateSoReview(string id, string reviewresult, string comment,string islock,string item)
        {
            int ID = Convert.ToInt32(id);
           
            SoReviewDetail data = new SoReviewDetail();
            data.Comment = comment;
            data.ReviewResult = reviewresult;
            data.IsLock = islock;
            data.Item = item;
            Result res = _iSoReviewService.UpdateDataSoReview(data, ID);
            return Json(new { res.success, message = res.message, obj = res.obj });
        }
        [HttpPost]
        public JsonResult UpdateSoReviewFinish(string sono, string planshipdate, string isdefine,string Date,string item)
        {
            SoReviewDetail data = new SoReviewDetail();
            if (isdefine == "true")
            {
                planshipdate = "TBD";
            }
            data.SONO = sono;
            data.PlanShipDate = planshipdate;
            data.DateDownLoad = DateTime.Parse(Date);
            data.Item = item;
            Result res = _iSoReviewService.UpdateSoReviewFinish(data);
            return Json(new { res.success, message = res.message, obj = res.obj });
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
        public ActionResult AddFileforItemReview(string SO_NO, string Date,  string File, string ID,string item )
        {
            DateTime date = DateTime.Now;
            string returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute;
            int id = int.Parse(ID);
            Date = Date.Substring(0, Date.IndexOf(" GMT"));
            DateTime dt;
            DateTime.TryParseExact(Date, "ddd MMM d yyyy hh:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
            if (File != null)
            {
                var  datafiles = new tbl_SOR_Attached_ForItemReview
                {
                   SO_NO = SO_NO,
                    Attached_File = returnPath + "/" + File,
                   Download_Date = dt,
                   Item_Idx = id,
                   LINE = item
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
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planner);
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
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planner);
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
            ViewBag.IsPlanner = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Planner);
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
            return View();
        }
        public ActionResult SoReViewHistoryRead([DataSourceRequest] DataSourceRequest request)
        {
            List<sp_SOR_GetSoReviewHist_Result1> list = _iSoReviewService.GetListSoReviewHistory();
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult RiskShipByValue()
        {
            var data = _iSoReviewService.SOR_RiskShip_Report1_Result();

            return View(data);
        }

        public ActionResult OTDByLine()
        {
            var data = _iSoReviewService.SOR_OTDFailByLine_Report();
            return View(data);
        }
        #endregion


    }
}
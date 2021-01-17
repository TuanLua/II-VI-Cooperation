using II_VI_Incorporated_SCM.Models.ESuggestion;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Services;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace II_VI_Incorporated_SCM.Controllers.ESuggestion
{
    public class ESuggestionController : Controller
    {
        private readonly IESuggestionService _iESuggestionService;
        private readonly ITaskManagementService _iTaskManagementService;
        private readonly IUserService _IUserService;
        public ESuggestionController(IESuggestionService iESuggestionService, IUserService IUserService, ITaskManagementService iTaskManagementService)
        {
            _iESuggestionService = iESuggestionService;
            _iTaskManagementService = iTaskManagementService;
            _IUserService = IUserService;
        }
        // GET: E_Suggestion
        public ActionResult Index()
        {
            //ViewBag.ListUser = _iESuggestionService.GetDropdownlistUser();
            ViewBag.ListUser = _iESuggestionService.GetDropdownlistMember();
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ListUser = _iESuggestionService.GetDropdownlistUser();
            return View();
        }
        public JsonResult ReadDataE_Suggestion([DataSourceRequest] DataSourceRequest request)
        {
            var lstESuggestion = _iESuggestionService.GetListIndex();
            return Json(lstESuggestion.ToDataSourceResult(request));
        }
        [HttpGet]
        public ActionResult SearchSimilar(string SuggestionID)
        {
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            ViewBag.Idea = _iESuggestionService.getIdeabyid(SuggestionID);
            ViewBag.IDSuggestion = SuggestionID;
            ViewBag.Step = _iESuggestionService.getStepEsugestion(SuggestionID);
            return View();
        }
        public JsonResult ReadDataE_SuggestionSimilar([DataSourceRequest] DataSourceRequest request, string title, string ideal)
        {
            var lstESuggestion = _iESuggestionService.GetListSearch(title, ideal);
            return Json(lstESuggestion.ToDataSourceResult(request));
        }
        public JsonResult ReadDataE_SuggestionManagement([DataSourceRequest] DataSourceRequest request, string step, string id)
        {
            List<sp_Inv_GetStepInfor_Result> lstESuggestion = _iESuggestionService.GetListManagement(step, id);
            if (step == "Step1")
            {
                if (lstESuggestion[lstESuggestion.Count - 2].item_value != null)
                {
                    string[] arrListFie = lstESuggestion[lstESuggestion.Count - 2].item_value.Split('/');
                    if (arrListFie.Length > 0) lstESuggestion[lstESuggestion.Count - 2].item_value = "";
                    for (int i = 0; i < arrListFie.Length; i++)
                    {

                        lstESuggestion[lstESuggestion.Count - 2].item_value += $"<br/> <a target='_blank' href='{Url.Action("DownloadFile", "ESuggestion", new { filename = arrListFie[i] })}' >{arrListFie[i]}</a>"; ;
                    }
                }
                if (lstESuggestion[lstESuggestion.Count - 1].item_value != null)
                {
                    string[] arrListFie1 = lstESuggestion[lstESuggestion.Count - 1].item_value.Split('/');
                    if (arrListFie1.Length > 0) lstESuggestion[lstESuggestion.Count - 1].item_value = "";
                    for (int i = 0; i < arrListFie1.Length; i++)
                    {
                        lstESuggestion[lstESuggestion.Count - 1].item_value += $"<br/> <a target='_blank' href='{Url.Action("DownloadFile", "ESuggestion", new { filename = arrListFie1[i] })}' >{arrListFie1[i]}</a>";
                    }
                }
            }
            return Json(lstESuggestion.ToDataSourceResult(request));
        }
        public ActionResult ProcessingSuggestion(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            ViewBag.ListImple = _iESuggestionService.GetDropdownlistImple();
            ViewBag.ListBoard = _iESuggestionService.GetDropdownlistUserBoard();
            ViewBag.ListCoach = _iESuggestionService.GetDropdownlistUserCoacher();
            ViewBag.ListSponsor = _iESuggestionService.GetDropdownlistUserSponsor();
            return View();
        }
        [HttpGet]
        public ActionResult SponsorSuggestion(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            ViewBag.ListUser = _iESuggestionService.GetDropdownlistUser();
            return View();
        }
        public ActionResult EditSponsor(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            ViewBag.ListUser = _iESuggestionService.GetDropdownlistUser();
            var model = _iESuggestionService.getSponsor(SuggestionID);
            return View(model);
        }
        public ActionResult BoardDirectorSuggestion(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            return View();
        }
        public ActionResult EditBoardDirectorSuggestion(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            var model = _iESuggestionService.getBoardirector(SuggestionID);
            return View(model);
        }
        public ActionResult EditProcessing(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.ListImple = _iESuggestionService.GetDropdownlistImple();
            ViewBag.ListBoard = _iESuggestionService.GetDropdownlistUserBoard();
            ViewBag.ListCoach = _iESuggestionService.GetDropdownlistUserCoacher();
            ViewBag.ListSponsor = _iESuggestionService.GetDropdownlistUserSponsor();
            var model = _iESuggestionService.getprocess(SuggestionID);
            model.Sug_title = _iESuggestionService.gettitlebyid(SuggestionID);

            return View(model);
        }
        public JsonResult getCIApprove(string SuggestionID)
        {
            var result = _iESuggestionService.getprocess(SuggestionID);
            return Json(result);
        }
        public ActionResult LeaderActionSuggestion()
        {
            return View();
        }
        public ActionResult Management(string SuggestionID)
        {
            ViewBag.ID = SuggestionID;
            ViewBag.Step = _iESuggestionService.getStepEsugestion(SuggestionID);
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            return View();
        }
        //[HttpPost]
        //public ActionResult GetsuggestionReport()
        //{

        //}
        [HttpPost]
        public ActionResult SaveESuggestion(E_SuggestionCreateViewmodel model, FormCollection filesave)
        {
            HttpFileCollectionBase savefiles = Request.Files;

            //var e = savefiles[0].FileName;
            for (int i = 0; i < savefiles.Count; i++)
            {
                var item = savefiles[i];
                DateTime date = DateTime.Now;
                string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
                //string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                //                     date.Minute;
                string virtualPath, returnPath = "eSuggestion";
                virtualPath = relativePath + returnPath;
                //  string FolderPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);
                //  string FolderPath = virtualPath;
                if (!Directory.Exists(virtualPath))
                    Directory.CreateDirectory(virtualPath);
                string FileName = item.FileName;
                if (FileName != "")
                {
                    if (Request.Browser.Browser.Contains("InternetExplorer") || Request.Browser.Browser.Contains("IE"))
                    {
                        FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
                    }
                    item.SaveAs(Path.Combine(virtualPath, FileName));
                }
            }

            //model.rqtor_name = _IUserService.GetNameById(model.Requestor);
            model.Submitter = User.Identity.GetUserId();
            bool result = _iESuggestionService.SaveEsuggesttion(model);
            return RedirectToAction("index", "ESuggestion");
        }
        [HttpPost]
        public ActionResult SaveCostSaving(CostSavingmodel model)
        {
            model.User_Input = _IUserService.GetNameById(User.Identity.GetUserId());
            bool result = _iESuggestionService.SavaCostSaving(model);
            return RedirectToAction("index", "ESuggestion"); ;
        }
        [HttpPost]
        public ActionResult SaveSponsorESuggestion(ESuggestinSponsorViewModel model)
        {
            string userName = _IUserService.GetNameById(User.Identity.GetUserId());
            bool result = _iESuggestionService.SaveBoardSponsor(model, userName);
            return RedirectToAction("index", "ESuggestion");
        }
        [HttpPost]
        public ActionResult SaveBoardESuggestion(BoardirectorViewmodel model)
        {
            string userName = _IUserService.GetNameById(User.Identity.GetUserId());
            bool result = _iESuggestionService.SaveBoardDirector(model, userName);
            return RedirectToAction("index", "ESuggestion");
        }
        [HttpPost]
        public ActionResult SaveProcessing(ProcessingViewModel model)
        {
            string userName = _IUserService.GetNameById(User.Identity.GetUserId());
            bool result = _iESuggestionService.SaveProcessing(model, userName);
            return RedirectToAction("index", "ESuggestion");
        }
        [HttpPost]
        public ActionResult SaveLeader(LeaderViewmodel model)
        {
            bool result = _iESuggestionService.SaveLeader(model);
            return RedirectToAction("index", "ESuggestion");
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            DateTime date = DateTime.Now;
            string relativePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            string virtualPath, returnPath = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" +
                                 date.Minute;
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
        public JsonResult UpdateStatusEsuggestion(string id, string status,string comment)
        {
            var result = _iESuggestionService.UpdateStatusSimalar(id, status,comment);
            return Json(true);
        }
        [HttpPost]
        public JsonResult getvaluesetselected(string ID)
        {
            var list = _iESuggestionService.selected(ID);
            return Json(new { result = list });
        }
        [HttpPost]
        public JsonResult getvaluesetselectedBoard(string ID)
        {
            var list = _iESuggestionService.getuserboard(ID);
            return Json(new { result = list });
        }
        [HttpPost]
        public JsonResult getSponsor(string Sug_ID)
        {
            var list = _iESuggestionService.getSponsor(Sug_ID);
            return Json(new { result = list });
        }
        [HttpPost]
        public JsonResult getCostSaving(string Sug_ID)
        {
            var list = _iESuggestionService.getCostSaving(Sug_ID);
            return Json(new { result = list });
        }
        [HttpPost]
        public JsonResult getRoleByUserID(string UserID)
        {
            var result = _iESuggestionService.GetRoleByUserID(UserID);
            return Json(result);
        }
        [HttpPost]
        public JsonResult GetDeptByUserID(string UserID)
        {
            var result = _iESuggestionService.GetDeptByUserID(UserID);
            return Json(result);
        }
        [HttpPost]
        public JsonResult GetRequestorInfo(string UserID)
        {
            var result = _iESuggestionService.GetRequestorInfo(UserID);
            return Json(result);
        }
        public ActionResult CostSaving(string SuggestionID)
        {
            ViewBag.titi = _iESuggestionService.gettitlebyid(SuggestionID);
            ViewBag.Idea = _iESuggestionService.getIdeabyid(SuggestionID);
            ViewBag.IDSuggestion = SuggestionID;
            return View();
        }
        public ActionResult SuggestionReport()
        {
            ViewBag.ListDept = _iESuggestionService.GetDeptByUserID();
            return View();
        }
        public ActionResult Role()
        {
            return View();
        }
        public FileContentResult DownloadFile(string filename)
        {
            if (filename != "")
            {
                string filePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
                string filePathFull = (filePath + "eSuggestion\\" + filename);
                byte[] file = GetMediaFileContent(filePathFull);
                return File(file, MimeMapping.GetMimeMapping(filePathFull), filename);
            }
            else
            {
                return null;
            }
        }
        //public JsonResult ReadEsuggestionData([DataSourceRequest] DataSourceRequest request, DateTime dtFrom, DateTime dtTo, string DeptList, string Imp_Method)
        //{
        //    return Json(_iESuggestionService.ReadEsuggestionData(dtFrom, dtTo, DeptList, Imp_Method).ToDataSourceResult(request));
        //}
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
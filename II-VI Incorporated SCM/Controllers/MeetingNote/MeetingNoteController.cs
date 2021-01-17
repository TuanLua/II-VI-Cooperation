using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.MeetingNote;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.MeetingNote
{
    public class MeetingNoteController : Controller
    {
        private readonly IMeetingNoteService _iMeetingNoteService;
        private readonly ITaskManagementService _iTaskManagementService;

        public MeetingNoteController(IMeetingNoteService iMeetingNoteService, ITaskManagementService iTaskManagementService)
        {
            _iMeetingNoteService = iMeetingNoteService;
            _iTaskManagementService = iTaskManagementService;
        }
        // GET: MeetingNote
        public ActionResult Index()
        {
            ViewBag.ListUser = _iMeetingNoteService.GetDropdownlistUser();
         
            return View();
        }
        public ActionResult MeetingNoteManagement()
        {
            return View();
        }
        public ActionResult getlistMeetingNote([DataSourceRequest]DataSourceRequest request)
        {
            var list = _iMeetingNoteService.getlistdata();
            // _INCRManagementService.updateAgeNCRinListNCR(list);
            return Json(list.ToDataSourceResult(request));
        }
        public JsonResult getvaluesetselected(string meetingnum)
        {
            var list = _iMeetingNoteService.selected(meetingnum);
            return Json(new { result = list });
        }

        public ActionResult ViewMeetingNote(string MeetingNum)
        {
            string iduser = User.Identity.GetUserId();
            ViewBag.TaskList= _iTaskManagementService.GetTaskListByTaskNO(MeetingNum.Trim());
            ViewBag.UserCreate = _iMeetingNoteService.CheckOnwerCreate(iduser);
            MeetingNoteViewmodel meeting = new MeetingNoteViewmodel();
            ViewBag.ListUser = _iMeetingNoteService.GetDropdownlistUser();
            meeting = _iMeetingNoteService.getMeetingbyNUm(MeetingNum);
            ViewBag.Name = meeting.CREATED_BY;
            ViewBag.Num = meeting.MINUTES_NUM;
            ViewBag.Status = meeting.STATUS;
            ViewBag.DateCreate = meeting.CREATED_DATE;
            ViewBag.DateMeeting = meeting.MEETING_DATE;
            meeting.OldEvidence = _iMeetingNoteService.GetUploadedEvidence(meeting.MINUTES_NUM);
            for (int i = 0; i < meeting.OldEvidence.Count; i++)
            {
                MEETING_ATT evi = meeting.OldEvidence[i];
              //  meeting.SizeOfOldEvidence = UtilitiesService.Add(meeting.SizeOfOldEvidence, GetSizeOfFile(evi.EVI_PATH));
            }
            return View(meeting);
        }
        public FileContentResult DownloadFile(string fileId, string filename)
        {
            string filePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
                MEETING_ATT sf = _iMeetingNoteService.GetFileWithFileMeetingNum(fileId, filename);
                if (sf != null)
                {
                    string filePathFull = (filePath + sf.ATT_PATH);
                    byte[] file = GetMediaFileContent(filePathFull);
                    return File(file, MimeMapping.GetMimeMapping(sf.ATT_PATH), sf.ATT_PATH);
                }
            else
            {
                return null;
            }
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

        [HttpPost]
        public ActionResult SaveMeetingNote(MeetingNoteViewmodel meetingNoteModel)
        {
            string iduser = User.Identity.GetUserId();
            List<MEETING_ATT> nCR_EVIs = new List<MEETING_ATT>();
            string meetingnumauto = _iMeetingNoteService.GetAutoMeetingNUM();
            if (meetingNoteModel.ModelEvidence != null)
            {
                foreach (var item in meetingNoteModel.ModelEvidence)
                {
                    if (item != null)
                    {
                        string File_Upload_Url = SaveFile(item.EvidenceFile);
                        MEETING_ATT meetingatt = new MEETING_ATT
                        {
                            ATT_PATH = File_Upload_Url,
                            MINUTES_NUM = meetingnumauto,
                            FILE_NAME = item.EvidenceFile.FileName,
                        };
                        nCR_EVIs.Add(meetingatt);
                    }
                }
            }
            var result = _iMeetingNoteService.SaveMeetingNote(meetingNoteModel, iduser, nCR_EVIs, meetingnumauto);
            return RedirectToAction("MeetingNoteManagement", "MeetingNote");
        }
        [HttpPost]
        public ActionResult PublicMeetingNote(string MeetingNum)
        {
            var result = _iMeetingNoteService.UpdateStatusMeeting(MeetingNum);
            //sent mail attendant

            return Json(true);
        }
        [HttpPost]
        public ActionResult EditMeetingNote(MeetingNoteViewmodel meetingNoteModel)
        {
            string iduser = User.Identity.GetUserId();
            List<MEETING_ATT> nCR_EVIs = new List<MEETING_ATT>();
            if (meetingNoteModel.ModelEvidence != null)
            {
                foreach (var item in meetingNoteModel.ModelEvidence)
                {
                    if (item != null)
                    {
                        string File_Upload_Url = SaveFile(item.EvidenceFile);
                        MEETING_ATT meetingatt = new MEETING_ATT
                        {
                            ATT_PATH = File_Upload_Url,
                            MINUTES_NUM = meetingNoteModel.MINUTES_NUM,
                            FILE_NAME = item.EvidenceFile.FileName,
                        };
                        nCR_EVIs.Add(meetingatt);
                    }
                }
            }
            var result = _iMeetingNoteService.EDITMeetingNote(meetingNoteModel, iduser, nCR_EVIs, meetingNoteModel.MINUTES_NUM);
            return RedirectToAction("MeetingNoteManagement", "MeetingNote");
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
    }
}
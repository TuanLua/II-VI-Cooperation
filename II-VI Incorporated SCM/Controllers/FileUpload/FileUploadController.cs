using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.FileUpload
{
    public class FileUploadController : Controller
    {
        private readonly ITaskManagementService _iTaskManagementService;
        // GET: FileUpload
        private string filePath = ConfigurationManager.AppSettings["UploadFile"];
        public FileUploadController(ITaskManagementService iTaskManagementService)
        {
            _iTaskManagementService = iTaskManagementService;
        }

        // GET: FileUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadFiles(IEnumerable<HttpPostedFileBase> lstfile/*, TASKDOCUMENT taskDoc*/)
        {
            if (lstfile.Count() > 0)
            {
                foreach (var file in lstfile)
                {
                    string filePath = ConfigurationManager.AppSettings["UploadFile"];
                    Guid guiId = Guid.NewGuid();
                    string fileName = guiId + System.IO.Path.GetExtension(file.FileName);
                    try
                    {
                        //bool rs = false;
                        //ConsultingFile consulFile = new ConsultingFile();
                        //consulFile.OriginalFileName = file.FileName;
                        //consulFile.ConsultingId = consultingId;
                        //consulFile.FileSize = file.ContentLength;
                        //consulFile.FilePath = fileName;
                        //consulFile.isDelete = false;
                        //consulFile.UserIDAttachedFile = userAF;
                        //rs = _consultingService.CreateConsultingFile(consulFile);

                        //string _FileName = Path.GetFileName(file.FileName);
                        //string _path = Path.Combine(Server.MapPath(filePath), fileName);
                        //file.SaveAs(_path);
                        return Json(new { success = true });

                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false });
                    }
                }

            }
            return Json(new { success = false });
        }

        public FileContentResult DownloadFile(int fileId)
        {
            string filePath = ConfigurationManager.AppSettings["UploadFile"];
            if (fileId != -1)
            {
                var sf = _iTaskManagementService.GetTaskDocFileWithFileID(fileId);
                if (sf != null)
                {
                    string filePathFull = Server.MapPath(filePath + "/" + sf.FILEPATH);
                    byte[] file = GetMediaFileContent(filePathFull);
                    return File(file, MimeMapping.GetMimeMapping(sf.FILENAME), sf.FILENAME);
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        #region Function
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
        #endregion
    }
}
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.SCAR;
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

namespace II_VI_Incorporated_SCM.Controllers.SCAR
{
    [Authorize(Roles = "SQE")]
    public class SCARController : Controller
    {
        private readonly ISCARService _iSCARService;
        private readonly ITaskManagementService _iTaskManagementService;

        public SCARController(ISCARService iSCARService, ITaskManagementService iTaskManagementService)
        {
            _iSCARService = iSCARService;
            _iTaskManagementService = iTaskManagementService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getseletedNCR(string NCR_NUM, string defect)
        {
            List<SelectListItem> list = _iSCARService.GetListNCRNumber(NCR_NUM, defect);
            string selected = "";
            if (list.Count > 1)
            {
                foreach (SelectListItem item in list)
                {
                    if (item.Selected == true)
                    {
                        selected = item.Value;
                    }
                }
            }
            ViewBag.LstNCRNum = selected;
            return Json(new { result = ViewBag.LstNCRNum });
        }

        [HttpPost]
        public ActionResult getdropdownncr(string NCR_NUM, string defect)
        {
            List<SelectListItem> list = _iSCARService.GetListDefect(NCR_NUM);
            ViewBag.ListNC = list;
            ViewBag.LstNCRNum = _iSCARService.GetListNCRNumber(NCR_NUM, defect);
            return Json(new { result = ViewBag.LstNCRNum });
        }

        [HttpGet]
        public ActionResult CreateSCARForNCR(string NCR_NUM, string defect)
        {
            List<SelectListItem> list = _iSCARService.GetListDefect(NCR_NUM);
            ViewBag.ListNC = list;
            if (defect != null)
            {
                List<SelectListItem> lstncrdefcet = _iSCARService.GetListNCRNumber(NCR_NUM, defect);
                ViewBag.LstNCRNum = lstncrdefcet;
                return View();
            }
            else if (defect == null && list.Count > 0)
            {
                List<SelectListItem> lstncr = _iSCARService.GetListNCRNumber(NCR_NUM, list[0].Value);
                ViewBag.LstNCRNum = lstncr;
                VendorViewModel data = new VendorViewModel();
                data = _iSCARService.GetVendorInfomation(NCR_NUM);
                _iSCARService.GetInfoByNCRNUM(data, NCR_NUM);
                data.NCR_NUMBER = NCR_NUM;
                ViewBag.SQE = _iSCARService.GetNameRoleSQE(User.Identity.GetUserId()) == true ? _iSCARService.GetFullNameByUserId(User.Identity.GetUserId()) : "";
                ViewBag.Category = _iSCARService.GetDropdownCategory();
                return View(data);
            }
            else
            {
                return RedirectToAction("NoSCARView", "SCAR");
            }

        }

        public ActionResult NoSCARView()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateSCARForSupplier(string NCR_NUM)
        {
            ViewBag.ListSupplier = _iSCARService.GetListSuppliers();
            ViewBag.Category = _iSCARService.GetDropdownCategory();
            ViewBag.Buyer = _iSCARService.GetNameRoleBuyer(User.Identity.GetUserId()) == true ? _iSCARService.GetFullNameByUserId(User.Identity.GetUserId()) : "";
            ViewBag.SQE = _iSCARService.GetNameRoleSQE(User.Identity.GetUserId()) == true ? _iSCARService.GetFullNameByUserId(User.Identity.GetUserId()) : "";
            return View();
        }

        [HttpGet]
        public ActionResult SentSCAR(string SCARID, string VERSION)
        {
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);
            ViewBag.NCRPdf = "";
            #region Get file NCR pdf
            if (!string.IsNullOrEmpty(SCARInfoData.NCR_NUMBER))
            {
                string FileMergePath = Url.Content((ConfigurationManager.AppSettings["uploadPath"] + "/MERGE_EVIDENT/Merge_{0}_Evident.pdf"));
                string[] lstncrnum = SCARInfoData.NCR_NUMBER.Split(',');
                string NCRFILEs = "";
                int lengthNCR = lstncrnum.Length;
                foreach (string item in lstncrnum)
                {
                    ViewBag.NCRPdf += $" <a target='_blank' class= 'a-download-evidence' ncrpath='{string.Format(FileMergePath, item)}' ncr='{item.Trim()}' href='{string.Format(FileMergePath, item)}'><i class='fa fa-file-pdf-o'> NCR_{item}_EVIDENT.pdf</i></a> &nbsp"; // Get file pdf if exists
                    NCRFILEs += $"{ConfigurationManager.AppSettings["SendMegerNcr"]}/Merge_{item}_Evident.pdf;";
                }
                ViewBag.NCRFILEs = NCRFILEs.Remove(NCRFILEs.Length - 1);
            }
            #endregion
            ViewBag.NCCodeValue = _iSCARService.GetdefectScar(SCARID);
            ViewBag.Category = _iSCARService.Getcategory(SCARID);
            return View(SCARInfoData);
        }

        [HttpGet]
        public ActionResult AcceptSCAR(string SCARID, int VERSION)
        {
            //var SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            //var data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            //ViewBag.SupplierInfo = data;
            //ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);
            //return View(SCARInfoData);

            SupplierCompletedViewModel dataModel = new SupplierCompletedViewModel();
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            dataModel.SCARInfo = SCARInfoData;
            ViewBag.NCCodeValue = _iSCARService.GetdefectScar(SCARID);
            dataModel.D0 = _iSCARService.GetDataD0(idSCAR);
            dataModel.D1 = _iSCARService.GetDataD1(idSCAR);
            dataModel.D2 = _iSCARService.GetDataD2(idSCAR);
            dataModel.D3 = _iSCARService.GetDataD3(idSCAR);
            dataModel.D4 = _iSCARService.GetDataD4(idSCAR);
            dataModel.D5 = _iSCARService.GetDataD5(idSCAR);
            dataModel.D6 = _iSCARService.GetDataD6(idSCAR);
            dataModel.D7 = _iSCARService.GetDataD7(idSCAR);
            dataModel.D8 = _iSCARService.GetDataD8(idSCAR);
            int check6D = 0;
            if (dataModel.D1.ID != 0)
            {
                check6D++;
            }

            if (dataModel.D2.ID != 0)
            {
                check6D++;
            }

            if (dataModel.D3.Count > 0)
            {
                check6D++;
            }

            if (dataModel.D4.ID != 0)
            {
                check6D++;
            }

            if (dataModel.D5.Count > 0)
            {
                check6D++;
            }

            if (dataModel.D6.Count > 0)
            {
                check6D++;
            }

            ViewBag.Check6D = check6D >= 6;
            ViewBag.Status = _iSCARService.Status(SCARID);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);
            ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(SCARID);
            ViewBag.Category = _iSCARService.Getcategory(SCARID);
            //if (!string.IsNullOrEmpty(SCARInfoData.EDIVENCE_D8))
            //{
            //    ViewBag.FileD8 = SCARInfoData.EDIVENCE_D8;
            //}
            //else
            //{
            //    ViewBag.FileD8 = string.Empty;
            //}
            return View(dataModel);
        }

        [HttpGet]
        public ActionResult EditSCAR(string SCARID)
        {
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            return View(SCARInfoData);
        }

        [HttpGet]
        public ActionResult SupplierCompletedSCAR(string SCARID)
        {
            SupplierCompletedViewModel dataModel = new SupplierCompletedViewModel();
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            dataModel.SCARInfo = SCARInfoData;
            dataModel.D0 = _iSCARService.GetDataD0(idSCAR);
            dataModel.D1 = _iSCARService.GetDataD1(idSCAR);
            dataModel.D2 = _iSCARService.GetDataD2(idSCAR);
            dataModel.D3 = _iSCARService.GetDataD3(idSCAR);
            dataModel.D4 = _iSCARService.GetDataD4(idSCAR);
            dataModel.D5 = _iSCARService.GetDataD5(idSCAR);
            dataModel.D6 = _iSCARService.GetDataD6(idSCAR);
            dataModel.D7 = _iSCARService.GetDataD7(idSCAR);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);
            return View(dataModel);
        }

        [HttpGet]
        public ActionResult CompletedAllSCAR(string SCARID)
        {
            SupplierCompletedViewModel dataModel = new SupplierCompletedViewModel();
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            dataModel.SCARInfo = SCARInfoData;
            dataModel.D0 = _iSCARService.GetDataD0(idSCAR);
            dataModel.D1 = _iSCARService.GetDataD1(idSCAR);
            dataModel.D2 = _iSCARService.GetDataD2(idSCAR);
            dataModel.D3 = _iSCARService.GetDataD3(idSCAR);
            dataModel.D4 = _iSCARService.GetDataD4(idSCAR);
            dataModel.D5 = _iSCARService.GetDataD5(idSCAR);
            dataModel.D6 = _iSCARService.GetDataD6(idSCAR);
            dataModel.D7 = _iSCARService.GetDataD7(idSCAR);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);
            ViewBag.NCCodeValue = _iSCARService.GetdefectScar(SCARID);
            ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(SCARID);
            #region Get file NCR pdf
            if (!string.IsNullOrEmpty(SCARInfoData.EDIVENCE_D8))
            {
                ViewBag.FileD8 = SCARInfoData.EDIVENCE_D8;
            }
            else
            {
                ViewBag.FileD8 = string.Empty;
            }
            #endregion
            return View(dataModel);
        }

        [HttpGet]
        public ActionResult ClosedSCAR(string SCARID)
        {
            SupplierCompletedViewModel dataModel = new SupplierCompletedViewModel();
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            dataModel.SCARInfo = SCARInfoData;
            dataModel.D0 = _iSCARService.GetDataD0(idSCAR);
            dataModel.D1 = _iSCARService.GetDataD1(idSCAR);
            dataModel.D2 = _iSCARService.GetDataD2(idSCAR);
            dataModel.D3 = _iSCARService.GetDataD3(idSCAR);
            dataModel.D4 = _iSCARService.GetDataD4(idSCAR);
            dataModel.D5 = _iSCARService.GetDataD5(idSCAR);
            dataModel.D6 = _iSCARService.GetDataD6(idSCAR);
            dataModel.D7 = _iSCARService.GetDataD7(idSCAR);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);
            ViewBag.NCCodeValue = _iSCARService.GetdefectScar(SCARID);
            ViewBag.Category = _iSCARService.Getcategory(SCARID);
            #region Get file NCR pdf
            if (!string.IsNullOrEmpty(SCARInfoData.EDIVENCE_D8))
            {
                ViewBag.FileD8 = SCARInfoData.EDIVENCE_D8;
            }
            else
            {
                ViewBag.FileD8 = string.Empty;
            }
            #endregion
            return View(dataModel);
        }

        [HttpGet]
        public ActionResult VoidSCAR(string SCARID)
        {
            SupplierCompletedViewModel dataModel = new SupplierCompletedViewModel();
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            SCARInfoViewModel SCARInfoData = _iSCARService.GetSCARInfoBySCARID(SCARID);
            dataModel.SCARInfo = SCARInfoData;
            dataModel.D0 = _iSCARService.GetDataD0(idSCAR);
            dataModel.D1 = _iSCARService.GetDataD1(idSCAR);
            dataModel.D2 = _iSCARService.GetDataD2(idSCAR);
            dataModel.D3 = _iSCARService.GetDataD3(idSCAR);
            dataModel.D4 = _iSCARService.GetDataD4(idSCAR);
            dataModel.D5 = _iSCARService.GetDataD5(idSCAR);
            dataModel.D6 = _iSCARService.GetDataD6(idSCAR);
            dataModel.D7 = _iSCARService.GetDataD7(idSCAR);
            VendorViewModel data = _iSCARService.GetSupplierById(SCARInfoData.VENDOR);
            ViewBag.SupplierInfo = data;
            ViewBag.ListSuggestEmail = _iSCARService.ListSuggestEmail(SCARID);

            #region Get file NCR pdf
            if (!string.IsNullOrEmpty(SCARInfoData.EDIVENCE_D8))
            {
                ViewBag.FileD8 = SCARInfoData.EDIVENCE_D8;
            }
            else
            {
                ViewBag.FileD8 = string.Empty;
            }
            #endregion
            return View(dataModel);
        }

        /// <summary>
        /// DownloadFileD8
        /// By: Sil
        /// Date: 06/29/2018
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileContentResult DownloadFileD8(string fileName, string SCARID)
        {
            string filePath = ConfigurationManager.AppSettings["acceptedSCAR"];
            string filePath1 = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "\\";
            if (!string.IsNullOrEmpty(fileName))
            {
                string filePathFull = Server.MapPath(filePath1 + "/" + fileName);
                byte[] file = GetMediaFileContent(filePathFull);
                return File(file, MimeMapping.GetMimeMapping(filePathFull), fileName);
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

        public JsonResult GetListDefect(string NCR_NUM)
        {
            List<SelectListItem> list = _iSCARService.GetListDefect(NCR_NUM);
            return Json(list);
        }

        public JsonResult GetSupplierById(string id)
        {
            if (id != "")
            {
                VendorViewModel data = _iSCARService.GetSupplierById(id);
                return Json(new { success = true, data = data });
            }
            return Json(new { success = false });
        }

        public JsonResult GetListSCAR([DataSourceRequest] DataSourceRequest request)
        {
            List<SCARInfoViewModel> list = _iSCARService.GetListSCAR();
            if (list != null)
            {
                foreach (SCARInfoViewModel item in list)
                {
                    item.NCR_NUMBER = $"<a target='_blank' href='{Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = item.NCR_NUMBER })}' >{item.NCR_NUMBER}</a>"; ;
                    foreach (string ncrnum in item.LstNCRNum)
                    {
                        string template = $"<br/> <a target='_blank' href='{Url.Action("ViewApproval", "NCRApproval", new { NCR_NUM = ncrnum })}' >{ncrnum}</a>";
                        item.NCR_NUMBER += template;
                    }
                }
            }
            return Json(list.ToDataSourceResult(request));
        }

        public JsonResult SaveSCARInfo(SCARInfoViewModel model)
        {
            if (model != null)
            {

                SCARINFO data = new SCARINFO
                {
                    BUYER = model.BUYER,
                    DATEPROBLEM = model.DATEPROBLEM,
                    DATERESPOND = model.DATERESPOND,
                    ID = model.ID,
                    ITEM = model.ITEM,
                    //   NCR_NUMBER = model.NCR_NUMBER,
                    NON_QTY = model.NON_QTY,
                    PO_NUMBER = model.PO_NUMBER,
                    PROBLEM = model.PROBLEM,
                    QUALITY = model.QUALITY,
                    RMA = model.RMA,
                    STATUS = "Created",
                    VENDOR = model.VENDOR,
                    VERSION = 0,
                    VN_NCR = model.VN_NCR,
                    VN_SCAR = model.VN_SCAR,
                    WRITTENBY = User.Identity.GetUserId(),
                    WRITTENDATE = DateTime.Now,
                    SCAR_ID = model.SCAR_ID,
                    LOT = model.LOT,
                    MI_PART_NO = model.MI_PART_NO,
                    DEFECTCODE = model.DEFECT,
                    RECURING_PROBLEM = model.RECURING_PROBLEM,
                    CATEGORY = model.CATEGORY
                };
                LogWriter log = new LogWriter("SaveSCARInfo");
                try
                {
                    // _iSCARService.SaveSCARInfo(data);
                    _iSCARService.SaveSCARnNCR(data, model.LstNCRNum);
                    string SCARID = data.ID.ToString().Length == 1 ? "VN00" + data.ID : (data.ID.ToString().Length == 2 ? "VN0" + data.ID : "VN" + data.ID);
                    log.LogWrite(SCARID);
                    string path = CopyFileSCAR(SCARID, data.VERSION);
                    log.LogWrite(path);
                    if (path != "")
                    {
                        string dest = Server.MapPath(ConfigurationManager.AppSettings["pathSCAR"]) + path;
                        log.LogWrite(dest);
                        ModifySCARFile(data, dest,model.LstNCRNum);
                        return Json(new { success = true, path = path, SCARID = SCARID });
                    }
                    else
                    {
                        return Json(new { success = false, alert = "Can't load SCAR file" });
                    }
                }
                catch (Exception ex)
                {
                    log.LogWrite(ex.ToString());
                    return Json(new { success = false, alert = "Save SCAR failed" });
                }
            }
            return Json(new { success = false, alert = "Check data input" });
        }

        public string CopyFileSCAR(string SCARID, int ver)
        {
            string path = "";
            LogWriter log = new LogWriter("Start write log file");
            try
            {
                string source = Server.MapPath(ConfigurationManager.AppSettings["sampleSCAR"]);
                log.LogWrite(source);
                string dest = Server.MapPath(ConfigurationManager.AppSettings["pathSCAR"]) + "SCAR_" + SCARID + "_" + ver + ".xlsx";
                log.LogWrite(dest);
                if (System.IO.File.Exists(source))
                {
                    System.IO.File.Copy(source, dest, true);
                    path = "SCAR_" + SCARID + "_" + ver + ".xlsx";
                    log.LogWrite(path);
                }
            }
            catch (Exception ex)
            {
                new LogWriter("CopyFileSCAR").LogWrite(ex.ToString());
            }
            return path;
        }

        //private static Microsoft.Office.Interop.Excel.Workbook mWorkBook;
        //private static Microsoft.Office.Interop.Excel.Sheets mWorkSheets;
        //private static Microsoft.Office.Interop.Excel.Worksheet mWSheet1;
        //private static Microsoft.Office.Interop.Excel.Application oXL;

        public void ModifySCARFile(SCARINFO model, string path, List<string> lstNCRnum)
        {
            LogWriter log = new LogWriter("ModifySCARFile");
            string NCRNum = "";
            if(lstNCRnum != null)
            {
                foreach (var item in lstNCRnum)
                {
                    NCRNum = item + ";";
                }
            }
            log.LogWrite("path: " + path);
            try
            {
                VendorViewModel data = _iSCARService.GetSupplierById(model.VENDOR);
                ExcelSupport.UpdateStringCell(path, data.VEN_NAM, 6, "C");
                ExcelSupport.UpdateCell(path, model.ID.ToString().Length == 1 ? "VN00" + model.ID : (model.ID.ToString().Length == 2 ? "VN0" + model.ID : "VN" + model.ID), 6, "G");
                ExcelSupport.UpdateStringCell(path, data.ADDRESS, 7, "C");
                ExcelSupport.UpdateCell(path, model.NON_QTY.ToString(), 7, "G");
                ExcelSupport.UpdateStringCell(path, model.BUYER, 7, "J");
                ExcelSupport.UpdateStringCell(path, data.CONTACT, 11, "C");
                ExcelSupport.UpdateCell(path, NCRNum, 9, "H");
                ExcelSupport.UpdateCell(path, model.QUALITY, 8, "J");
                ExcelSupport.UpdateCell(path, model.RMA, 11, "G");
                ExcelSupport.UpdateStringCell(path, data.EMAIL, 12, "C");
                ExcelSupport.UpdateStringCell(path, data.TEL, 13, "C");
                ExcelSupport.UpdateStringCell(path, data.FAX, 14, "C");
                ExcelSupport.UpdateCell(path, model.VN_SCAR, 12, "J");
                ExcelSupport.UpdateCell(path, model.VN_NCR, 14, "J");
                ExcelSupport.UpdateCell(path, model.PO_NUMBER, 15, "D");
                ExcelSupport.UpdateCell(path, model.MI_PART_NO, 16, "D");
                string dateProblem = model.DATEPROBLEM.ToString("MM-dd-yyyy");
                ExcelSupport.UpdateStringCell(path, dateProblem, 17, "J");
                ExcelSupport.UpdateStringCell(path, model.RECURING_PROBLEM, 18, "c");
                ExcelSupport.UpdateStringCell(path, model.PROBLEM, 19, "B");
                string dateRespone = model.DATERESPOND.ToString("MM-dd-yyyy");
                ExcelSupport.UpdateStringCell(path, dateRespone, 26, "E");

            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
            }
            //oXL = new Microsoft.Office.Interop.Excel.Application();

            //mWorkBook = oXL.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //mWorkSheets = mWorkBook.Worksheets;

            //mWSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item("SCAR");
            //Microsoft.Office.Interop.Excel.Range range = mWSheet1.UsedRange;

            //var data = _iSCARService.GetSupplierById(model.VENDOR);

            //mWSheet1.Cells[6, 3] = data.VEN_NAM;
            //mWSheet1.Cells[6, 7] = model.ID.ToString().Length == 1 ? "VN00" + model.ID : (model.ID.ToString().Length == 2 ? "VN0" + model.ID : "VN" + model.ID);
            //mWSheet1.Cells[7, 3] = data.ADDRESS;
            //mWSheet1.Cells[7, 8] = model.NON_QTY;
            //mWSheet1.Cells[7, 10] = model.BUYER;
            //mWSheet1.Cells[11, 3] = data.CONTACT;
            //mWSheet1.Cells[9, 8] = model.NCR_NUMBER;
            //mWSheet1.Cells[8, 10] = model.QUALITY;
            //mWSheet1.Cells[7, 3] = data.ADDRESS;
            //mWSheet1.Cells[11, 8] = model.RMA;
            //mWSheet1.Cells[12, 3] = data.EMAIL;
            //mWSheet1.Cells[13, 3] = data.TEL;
            //mWSheet1.Cells[14, 3] = data.FAX;
            //mWSheet1.Cells[12, 10] = model.VN_SCAR;
            //mWSheet1.Cells[14, 10] = model.VN_NCR;
            //mWSheet1.Cells[15, 4] = model.PO_NUMBER;
            //mWSheet1.Cells[16, 4] = model.MI_PART_NO;
            //mWSheet1.Cells[17, 10] = String.Format("{0:dd/MM/yyyy}", model.DATEPROBLEM.ToString());
            //mWSheet1.Cells[19, 2] = model.PROBLEM;
            //mWSheet1.Cells[26, 5] = String.Format("{0:dd/MM/yyyy}", model.DATERESPOND.ToString());

            //mWorkBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);

            //mWorkBook.Close();
            //mWSheet1 = null;

            //mWorkBook = null;

            //oXL.Quit();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }

        public JsonResult UpdateStatusSCAR(string status, string SCARID)
        {
            try
            {
                _iSCARService.UpdateStatusSCAR(status, SCARID);
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        public JsonResult SentEmail(SentMailViewModel model)
        {
            string pathscar = ConfigurationManager.AppSettings["SendScarExcel"] + model.FILESCAR;
            string pathMegerNCR = $"{model.NCRFILE}";
            string path = $"{pathscar}";
            if (!string.IsNullOrEmpty(pathMegerNCR))
            {
                path = $"{pathscar};{pathMegerNCR}";
            }
            bool result = _iSCARService.SentEmail(model, path);
            return Json(new { success = result });
        }

        [HttpPost]
        public JsonResult SentMailRemind(SentMailViewModel model)
        {
            string path = Server.MapPath(ConfigurationManager.AppSettings["pathSCAR"]) + model.FILESCAR;
            bool result = _iSCARService.SentMailRemind(model, path);
            return Json(new { success = result });
        }

        public JsonResult SearchPositon(string ListD, string SCARID, HttpPostedFileBase FILE)
        {
            string relativePath = ConfigurationManager.AppSettings["acceptedSCAR"];
            string FolderPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
            string result = "";
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            string path = Path.Combine(FolderPath, "SCAR_" + SCARID + ".xlsm");
            FILE.SaveAs(Path.Combine(FolderPath, "SCAR_" + SCARID + ".xlsm"));

            List<string> listD = ListD.Split(',').Distinct().ToList();

            int idSCAR = Convert.ToInt32(SCARID.Substring(2));

            // Delete data D0-D7 from database
            
            //if (!_iSCARService.DeleteData7D(idSCAR))
            //{
            //    // Delete D0-D7 fails
            //    return Json(new { success = "Refesh Data UnSuccessful" });
            //}
            result = GetCellValueD(listD, path, idSCAR);

            return Json(new { success = result });
        }

        public string GetCellValueD(List<string> listD, string path, int SCARID)
        {
            //Microsoft.Office.Interop.Excel.Workbook oWB;
            //Microsoft.Office.Interop.Excel.Worksheet oSheet;
            string alert = "";
            SpreadsheetDocument spreadSheet = null;
            try
            {
                spreadSheet = SpreadsheetDocument.Open(path, true);
                spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
                WorksheetPart worksheetPart = ExcelSupport.GetWorksheetPartByName(spreadSheet, "SCAR");
                WorkbookPart workbookPart = spreadSheet.WorkbookPart;
                //WorksheetPart worksheetPart = ExcelSupport.GetWorksheetPartByName(spreadSheet, "SCAR");
                List<string> CheckFull7D = new List<string>(); ;

                if (worksheetPart != null)
                {
                    foreach (string item in listD)
                    {
                        if (item == "D0")
                        {
                            #region get data D0 from excel
                            SCAR_RESULT_D0 dataD0 = new SCAR_RESULT_D0();
                            int position = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D0);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, 2));
                                }
                                catch { }
                            }

                            if (position != 0)
                            {
                                Cell d0ContentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", position + 1);
                                Cell d0DateCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", position);

                                string dateStr = d0DateCell.InnerText;
                                string content = ExcelSupport.GetValueCell(workbookPart, d0ContentCell);

                                string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                                string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                                if (!Directory.Exists(pathPDF))
                                {
                                    Directory.CreateDirectory(pathPDF);
                                }
                                string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D0", pathPDF, serverPath);
                                //if (evi == "")
                                //{
                                //    alert = "Please add edivence for D0";
                                //    break;
                                //}

                                //if (content != "" && dateStr != "")
                                //{
                                /////
                                if (d0DateCell.InnerText != "")
                                {
                                    DateTime date = DateTime.FromOADate(Convert.ToDouble(d0DateCell.InnerText));
                                    dataD0.DATE_D0 = date;
                                }
                                if (!string.IsNullOrEmpty(content))
                                {
                                    DateTime date = DateTime.FromOADate(Convert.ToDouble(d0DateCell.InnerText));
                                    dataD0.DATE_D0 = date;

                                    dataD0.CONTENT = content;
                                    dataD0.DATE_CREATE = DateTime.Now;
                                    dataD0.SCAR_ID = SCARID;
                                    dataD0.WRITERBY = User.Identity.GetUserId();
                                    dataD0.EDIVENCE = evi;
                                    _iSCARService.SaveD0(dataD0);
                                    _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                    // CheckFull7D.Add("D0");
                                }
                                //}
                                //else
                                //{
                                //    alert = "Please add content for D0";
                                //    break;
                                //}
                            }
                            #endregion
                        }
                        if (item == "D1")
                        {
                            #region get data D1 from excel
                            SCAR_RESULT_D1 dataD1 = new SCAR_RESULT_D1();
                            int position = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D1);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, 2));
                                }
                                catch { }
                            }
                            if (position != 0)
                            {
                                Cell d1DateCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", position);
                                Cell d1leaderCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", position + 2);
                                Cell d1championCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "D", position + 2);
                                Cell d1memberCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "F", position + 2);
                                Cell d1externalMemberCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "H", position + 2);

                                string dateStr = d1DateCell.InnerText;
                                string leader = ExcelSupport.GetValueCell(workbookPart, d1leaderCell);
                                string champion = ExcelSupport.GetValueCell(workbookPart, d1championCell);
                                string member = ExcelSupport.GetValueCell(workbookPart, d1memberCell);
                                string externalMember = ExcelSupport.GetValueCell(workbookPart, d1externalMemberCell);

                                string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                                string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                                if (!Directory.Exists(pathPDF))
                                {
                                    Directory.CreateDirectory(pathPDF);
                                }
                                string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D1", pathPDF, serverPath);
                                if (!string.IsNullOrEmpty(leader) && d1DateCell.InnerText != "")//1 va 5 bat buoc
                                {
                                    DateTime date = DateTime.FromOADate(Convert.ToDouble(d1DateCell.InnerText));
                                    dataD1.DATE_D1 = date;

                                    dataD1.CHAMPION = champion;
                                    dataD1.DATE_CREATE = DateTime.Now;
                                    dataD1.SCAR_ID = SCARID;
                                    dataD1.WRITERBY = User.Identity.GetUserId();
                                    dataD1.EXTERNAL_MEMBER = externalMember;
                                    dataD1.LEADER = leader;
                                    dataD1.MEMBER = member;
                                    dataD1.EDIVENCE = evi;
                                    _iSCARService.SaveD1(dataD1);
                                    CheckFull7D.Add("D1");
                                    _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                }
                            }
                            #endregion
                        }

                        if (item == "D2")
                        {
                            #region get data D2 from excel
                            SCAR_RESULT_D2 dataD2 = new SCAR_RESULT_D2();
                            int position = 0;

                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D2);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, 2));
                                }
                                catch { }
                            }

                            if (position != 0)
                            {
                                Cell d2DateCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", position);
                                Cell d2ContentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", position + 1);

                                string content = ExcelSupport.GetValueCell(workbookPart, d2ContentCell);
                                string dateStr = d2DateCell.InnerText;

                                string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                                string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                                if (!Directory.Exists(pathPDF))
                                {
                                    Directory.CreateDirectory(pathPDF);
                                }
                                string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D2", pathPDF, serverPath);
                                //if (evi == "")
                                //{
                                //    alert = "Please add edivence for D2";
                                //    break;
                                //}

                                //if (content != "" && dateStr != "")
                                //{
                                //if (d2DateCell.InnerText != "")
                                //{
                                //    DateTime date = DateTime.FromOADate(Convert.ToDouble(d2DateCell.InnerText));
                                //    dataD2.DATE_D2 = date;
                                //}
                                if (!string.IsNullOrEmpty(content) & d2DateCell.InnerText != "")
                                {
                                    DateTime date = DateTime.FromOADate(Convert.ToDouble(d2DateCell.InnerText));
                                    dataD2.DATE_D2 = date;

                                    dataD2.CONTENT = content;
                                    dataD2.DATE_CREATE = DateTime.Now;
                                    dataD2.SCAR_ID = SCARID;
                                    dataD2.WRITERBY = User.Identity.GetUserId();
                                    dataD2.EDIVENCE = evi;
                                    _iSCARService.SaveD2(dataD2);
                                    CheckFull7D.Add("D2");
                                    _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                }
                                //}
                                //else
                                //{
                                //    alert = "Please add content for D2";
                                //    break;
                                //}
                            }
                            #endregion
                        }
                        if (item == "D3")
                        {
                            #region get data D3 from excel
                            SCAR_RESULT_D3 dataD3 = new SCAR_RESULT_D3();
                            int position = 0;
                            int end = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D3);
                            string cellReferenceEnd = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D4);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, cellReference.Length - 1));
                                    end = Convert.ToInt32(cellReferenceEnd.Substring(1, cellReferenceEnd.Length - 1));
                                }
                                catch { }
                            }

                            string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                            string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                            if (!Directory.Exists(pathPDF))
                            {
                                Directory.CreateDirectory(pathPDF);
                            }
                            string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D3", pathPDF, serverPath);
                            //if (evi == "")
                            //{
                            //    alert = "Please add edivence for D3";
                            //    break;
                            //}
                            _iSCARService.RemoveD3(SCARID);
                            if (position != 0 && end != 0)
                            {
                                int dem = 1;
                                for (int i = position + 1; i < end - 1; i++)
                                {
                                    Cell d3whoCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", i);
                                    Cell d3whenCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "K", i);
                                    Cell d3contentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", i);

                                    string content = ExcelSupport.GetValueCell(workbookPart, d3contentCell);
                                    string who = ExcelSupport.GetValueCell(workbookPart, d3whoCell);
                                    string whenStr = d3whenCell.InnerText;

                                    //if (content != "" && whenStr != "" && who != "")
                                    //{
                                    //if (d3whenCell.InnerText != "")
                                    //{
                                    //    DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                    //    dataD3.WHEN_IMPLEMENT = when;
                                    //}
                                    //else
                                    //{
                                    //    dataD3.WHEN_IMPLEMENT = null;
                                    //}

                                    if (!string.IsNullOrEmpty(who) && !string.IsNullOrEmpty(content) & d3whenCell.InnerText != "")
                                    {
                                        DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                        dataD3.WHEN_IMPLEMENT = when;

                                        dataD3.ITEM = dem;

                                        dataD3.WHO_IMPLEMENT = who;
                                        dataD3.CONTENT = content;
                                        dataD3.DATE_D3 = DateTime.Now;
                                        dataD3.DATE_CREATE = DateTime.Now;
                                        dataD3.SCAR_ID = SCARID;
                                        dataD3.WRITERBY = User.Identity.GetUserId();
                                        dataD3.EDIVENCE = evi;
                                        _iSCARService.SaveD3(dataD3);
                                        CheckFull7D.Add("D3");
                                        _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                    }
                                    dem++;
                                    //}
                                }
                            }
                            #endregion
                        }
                        if (item == "D4")
                        {
                            #region get data D4 from excel
                            SCAR_RESULT_D4 dataD4 = new SCAR_RESULT_D4();
                            int position = 0;

                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D4);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, cellReference.Length - 1));
                                }
                                catch { }
                            }
                            if (position != 0)
                            {
                                Cell d4DateCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", position);
                                Cell d4ContentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", position + 1);

                                string content = ExcelSupport.GetValueCell(workbookPart, d4ContentCell);
                                string dateStr = d4DateCell.InnerText;

                                string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                                string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                                if (!Directory.Exists(pathPDF))
                                {
                                    Directory.CreateDirectory(pathPDF);
                                }
                                string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D4", pathPDF, serverPath);
                                //if (evi == "")
                                //{
                                //    alert = "Please add edivence for D4";
                                //    break;
                                //}

                                //if (content != "" && dateStr != "")
                                //{
                                //if (d4DateCell.InnerText != "")
                                //{
                                //    DateTime date = DateTime.FromOADate(Convert.ToDouble(d4DateCell.InnerText));
                                //    dataD4.DATE_D4 = date;
                                //}
                                if (!string.IsNullOrEmpty(content) & d4DateCell.InnerText != "")
                                {
                                    DateTime date = DateTime.FromOADate(Convert.ToDouble(d4DateCell.InnerText));
                                    dataD4.DATE_D4 = date;
                                    dataD4.EDIVENCE = evi;
                                    dataD4.CONTENT = content;
                                    dataD4.DATE_CREATE = DateTime.Now;
                                    dataD4.SCAR_ID = SCARID;
                                    dataD4.WRITERBY = User.Identity.GetUserId();
                                    _iSCARService.SaveD4(dataD4);
                                    CheckFull7D.Add("D4");
                                    _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                }
                                //}
                                //else
                                //{
                                //    alert = "Please add content for D4";
                                //    break;
                                //}
                            }
                            #endregion
                        }
                        if (item == "D5")
                        {
                            #region Get data D5 from excel
                            SCAR_RESULT_D5 dataD5 = new SCAR_RESULT_D5();
                            int position = 0;
                            int end = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D5);
                            string cellReferenceEnd = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D6);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, cellReference.Length - 1));
                                    end = Convert.ToInt32(cellReferenceEnd.Substring(1, cellReferenceEnd.Length - 1));
                                }
                                catch { }
                            }

                            string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                            string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                            if (!Directory.Exists(pathPDF))
                            {
                                Directory.CreateDirectory(pathPDF);
                            }
                            string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D5", pathPDF, serverPath);
                            //if (evi == "")
                            //{
                            //    alert = "Please add edivence for D5";
                            //    break;
                            //}
                            _iSCARService.RemoveD5(SCARID);
                            if (position != 0 && end != 0)
                            {
                                int dem = 1;
                                for (int i = position + 1; i < end - 1; i++)
                                {
                                    Cell d3whoCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", i);
                                    Cell d3whenCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "K", i);
                                    Cell d3contentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", i);

                                    string content = ExcelSupport.GetValueCell(workbookPart, d3contentCell);
                                    string who = ExcelSupport.GetValueCell(workbookPart, d3whoCell);
                                    string whenStr = d3whenCell.InnerText;

                                    //if (content != "" && whenStr != "" && who != "")
                                    //{
                                    //if (d3whenCell.InnerText != "")
                                    //{
                                    //    DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                    //    dataD5.WHEN_IMPLEMENT = when;
                                    //}
                                    //else
                                    //{
                                    //    dataD5.WHEN_IMPLEMENT = null;
                                    //}

                                    if (!string.IsNullOrEmpty(who) && !string.IsNullOrEmpty(content) & d3whenCell.InnerText != "")
                                    {
                                        DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                        dataD5.WHEN_IMPLEMENT = when;

                                        dataD5.ITEM = dem;
                                        dataD5.WHO_IMPLEMENT = who;
                                        dataD5.CONTENT = content;
                                        dataD5.DATE_CREATE = DateTime.Now;
                                        dataD5.SCAR_ID = SCARID;
                                        dataD5.WRITERBY = User.Identity.GetUserId();
                                        dataD5.EDIVENCE = evi;
                                        _iSCARService.SaveD5(dataD5);
                                        CheckFull7D.Add("D5");
                                        _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                    }
                                    dem++;
                                    //}
                                }
                            }
                            #endregion
                        }
                        if (item == "D6")
                        {
                            #region Get data D6 from excel
                            SCAR_RESULT_D6 dataD6 = new SCAR_RESULT_D6();
                            int position = 0;
                            int end = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D6);
                            string cellReferenceEnd = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D7);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, cellReference.Length - 1));
                                    end = Convert.ToInt32(cellReferenceEnd.Substring(1, cellReferenceEnd.Length - 1));
                                }
                                catch { }
                            }

                            string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                            string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                            if (!Directory.Exists(pathPDF))
                            {
                                Directory.CreateDirectory(pathPDF);
                            }
                            string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D6", pathPDF, serverPath);
                            //if (evi == "")
                            //{
                            //    alert = "Please add edivence for D6";
                            //    break;
                            //}
                            
                            _iSCARService.RemoveD6(SCARID);
                            if (position != 0 && end != 0)
                            {
                                int dem = 1;
                                for (int i = position + 2; i < end - 1; i++)
                                {
                                    Cell d3whoCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", i);
                                    Cell d3whenCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "K", i);
                                    Cell d3contentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", i);

                                    string content = ExcelSupport.GetValueCell(workbookPart, d3contentCell);
                                    string who = ExcelSupport.GetValueCell(workbookPart, d3whoCell);
                                    string whenStr = d3whenCell.InnerText;

                                    //if (content != "" && whenStr != "" && who != "")
                                    //{
                                    //if (d3whenCell.InnerText != "")
                                    //{
                                    //    DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                    //    dataD6.WHEN_IMPLEMENT = when;
                                    //}
                                    //else
                                    //{
                                    //    dataD6.WHEN_IMPLEMENT = null;
                                    //}

                                    if (!string.IsNullOrEmpty(who) && !string.IsNullOrEmpty(content) & d3whenCell.InnerText != "")
                                    {
                                        DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                        dataD6.WHEN_IMPLEMENT = when;

                                        dataD6.ITEM = dem;
                                        dataD6.WHO_IMPLEMENT = who;
                                        dataD6.CONTENT = content;
                                        dataD6.DATE_CREATE = DateTime.Now;
                                        dataD6.SCAR_ID = SCARID;
                                        dataD6.WRITERBY = User.Identity.GetUserId();
                                        dataD6.EDIVENCE = evi;
                                        _iSCARService.SaveD6(dataD6);
                                        CheckFull7D.Add("D6");
                                        _iSCARService.UpdateStatusSCAR("Verification",
                                   SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                   );
                                    }
                                    dem++;
                                    //}
                                }
                            }
                            #endregion
                        }
                        if (item == "D7")
                        {
                            #region Get data D7 from excel
                            SCAR_RESULT_D7 dataD7 = new SCAR_RESULT_D7();
                            int position = 0;
                            int end = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D7);
                            string cellReferenceEnd = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D8);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, cellReference.Length - 1));
                                    end = Convert.ToInt32(cellReferenceEnd.Substring(1, cellReferenceEnd.Length - 1));
                                }
                                catch { }
                            }

                            string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                            string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                            if (!Directory.Exists(pathPDF))
                            {
                                Directory.CreateDirectory(pathPDF);
                            }
                            string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D7", pathPDF, serverPath);

                            if (position != 0 && end != 0)
                            {
                                int dem = 1;
                                for (int i = position + 1; i < end - 1; i++)
                                {
                                    Cell d3whoCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "J", i);
                                    Cell d3whenCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "K", i);
                                    Cell d3contentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", i);

                                    string content = ExcelSupport.GetValueCell(workbookPart, d3contentCell);
                                    string who = ExcelSupport.GetValueCell(workbookPart, d3whoCell);
                                    string whenStr = d3whenCell.InnerText;


                                    //if (content != "" && whenStr != "" && who != "")
                                    //{
                                    //if (d3whenCell.InnerText != "")
                                    //{
                                    //    DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                    //    dataD7.WHEN_IMPLEMENT = when;
                                    //}
                                    //else
                                    //{
                                    //    dataD7.WHEN_IMPLEMENT = null;
                                    //}

                                    if (!string.IsNullOrEmpty(who) && !string.IsNullOrEmpty(content) & d3whenCell.InnerText != "")
                                    {
                                        DateTime when = DateTime.FromOADate(Convert.ToDouble(d3whenCell.InnerText));
                                        dataD7.WHEN_IMPLEMENT = when;

                                        dataD7.ITEM = dem;
                                        dataD7.WHO_IMPLEMENT = who;
                                        dataD7.CONTENT = content;
                                        dataD7.DATE_CREATE = DateTime.Now;
                                        dataD7.SCAR_ID = SCARID;
                                        dataD7.WRITERBY = User.Identity.GetUserId();
                                        dataD7.EDIVENCE = evi;
                                        _iSCARService.SaveD7(dataD7);
                                        CheckFull7D.Add("D7");
                                    }
                                    dem++;
                                    //}
                                }
                            }
                            #endregion

                            #region Change Status to "Supplier Completed" when D1-D7 done: manual
                            if (new HashSet<string>(CheckFull7D).Count >= 7)
                            {
                                _iSCARService.UpdateStatusSCAR("Verification",
                                    SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                                    );
                            }
                            #endregion
                        }
                        if (item == "D8")
                        {
                            #region Get data D8 from excel
                            int position = 0;
                            int end = 0;
                            string cellReference = ExcelSupport.GetCellReference(workbookPart, FieldRespone.D8);
                            string cellReferenceEnd = ExcelSupport.GetCellReference(workbookPart, FieldRespone.End);
                            if (cellReference != null)
                            {
                                try
                                {
                                    position = Convert.ToInt32(cellReference.Substring(1, cellReference.Length - 1));
                                    end = Convert.ToInt32(cellReferenceEnd.Substring(1, cellReferenceEnd.Length - 1));
                                }
                                catch { }
                            }

                            string pathPDF = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["pathEviSCAR"]) + SCARID + "\\";
                            string serverPath = ConfigurationManager.AppSettings["pathEviSCAR"] + SCARID + "/";
                            if (!Directory.Exists(pathPDF))
                            {
                                Directory.CreateDirectory(pathPDF);
                            }
                            string evi = ExcelSupport.GetPicWorkSheet(workbookPart, "D8", pathPDF, serverPath);
                            if (position != 0 && end != 0)
                            {
                                    Cell d4ContentCell = ExcelSupport.GetCell(worksheetPart.Worksheet, "B", position + 1);
                                    string content = ExcelSupport.GetValueCell(workbookPart, d4ContentCell);
                                    if (!string.IsNullOrEmpty(content) )
                                    {
                                         var _db = new IIVILocalDB();
                                        var data = _db.SCARINFOes.Where(x => x.ID == SCARID).FirstOrDefault();
                                        data.CONTENT = content;
                                        data.EDIVENCE_D8 = evi;
                                        data.DATE_D8 = DateTime.Now;
                                        _db.SaveChanges();
                                        CheckFull7D.Add("D8");
                                }
                            }
                            #endregion
                            //if (new HashSet<string>(CheckFull7D).Count >= 8)
                            //{
                            //    _iSCARService.UpdateStatusSCAR("Completed All",
                            //        SCARID.ToString().Length == 1 ? "VN00" + SCARID : (SCARID.ToString().Length == 2 ? "VN0" + SCARID : "VN" + SCARID)
                            //        );
                            //}
                        
                        }
                        
                    }
                }

                #region
                //object missing = System.Reflection.Missing.Value;
                //oXL.Visible = true;
                //oXL.DisplayAlerts = false;
                //oWB = oXL.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //mWorkSheets = oWB.Worksheets;
                //oSheet = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item("SCAR");

                //foreach (var item in listD)
                //{
                //    int position = 0;
                //    if (item == "D0")
                //    {
                //        #region
                //        SCAR_RESULT_D0 data = new SCAR_RESULT_D0();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D0, oSheet);
                //        if (oRng != null)
                //        {
                //            position = oRng.Row;
                //            if ((oSheet.Cells[position + 1, 2] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //            {
                //                var D0Content = (string)(oSheet.Cells[position + 1, 2] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var DATED0 = (DateTime)(oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value;

                //                data.CONTENT = D0Content;
                //                data.DATE_D0 = DATED0;
                //                data.DATE_CREATE = DateTime.Now;
                //                data.SCAR_ID = SCARID;
                //                data.WRITERBY = User.Identity.GetUserId();
                //                _iSCARService.SaveD0(data);
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D1")
                //    {
                //        #region
                //        SCAR_RESULT_D1 data = new SCAR_RESULT_D1();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D1, oSheet);
                //        if (oRng != null)
                //        {
                //            position = oRng.Row;
                //            if ((oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position + 2, 2] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position + 2, 4] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position + 2, 6] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position + 2, 8] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //            {
                //                var D1Date = (DateTime)(oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var leader = (string)(oSheet.Cells[position + 2, 2] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var champion = (string)(oSheet.Cells[position + 2, 4] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var member = (string)(oSheet.Cells[position + 2, 6] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var externalMember = (string)(oSheet.Cells[position + 2, 8] as Microsoft.Office.Interop.Excel.Range).Value;

                //                data.CHAMPION = champion;
                //                data.DATE_D1 = D1Date;
                //                data.DATE_CREATE = DateTime.Now;
                //                data.SCAR_ID = SCARID;
                //                data.WRITERBY = User.Identity.GetUserId();
                //                data.EXTERNAL_MEMBER = externalMember;
                //                data.LEADER = leader;
                //                data.MEMBER = member;
                //                _iSCARService.SaveD1(data);
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D2")
                //    {
                //        #region
                //        SCAR_RESULT_D2 data = new SCAR_RESULT_D2();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D2, oSheet);
                //        if (oRng != null)
                //        {
                //            position = oRng.Row;
                //            if ((oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position + 1, 2] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //            {
                //                var D2Date = (DateTime)(oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var content = (string)(oSheet.Cells[position + 1, 2] as Microsoft.Office.Interop.Excel.Range).Value;

                //                data.CONTENT = content;
                //                data.DATE_D2 = D2Date;
                //                data.DATE_CREATE = DateTime.Now;
                //                data.SCAR_ID = SCARID;
                //                data.WRITERBY = User.Identity.GetUserId();
                //                _iSCARService.SaveD2(data);
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D3")
                //    {
                //        #region
                //        SCAR_RESULT_D3 data = new SCAR_RESULT_D3();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D3, oSheet);
                //        Microsoft.Office.Interop.Excel.Range eRng = GetSpecifiedRange(FieldRespone.D4, oSheet);
                //        if (oRng != null && eRng != null)
                //        {
                //            position = oRng.Row;
                //            int endPos = eRng.Row;
                //            int dem = 1;
                //            for (int i = position + 1; i < endPos - 1; i++)
                //            {
                //                if ((oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //                {
                //                    var who = (string)(oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var when = (DateTime)(oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var content = (string)(oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value;

                //                    data.ITEM = dem;
                //                    data.WHEN_IMPLEMENT = when;
                //                    data.WHO_IMPLEMENT = who;
                //                    data.CONTENT = content;
                //                    data.DATE_D3 = DateTime.Now;
                //                    data.DATE_CREATE = DateTime.Now;
                //                    data.SCAR_ID = SCARID;
                //                    data.WRITERBY = User.Identity.GetUserId();
                //                    _iSCARService.SaveD3(data);
                //                    dem++;
                //                }
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D4")
                //    {
                //        #region
                //        SCAR_RESULT_D4 data = new SCAR_RESULT_D4();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D4, oSheet);
                //        if (oRng != null)
                //        {
                //            position = oRng.Row;
                //            if ((oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[position + 1, 2] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //            {
                //                var D4Date = (DateTime)(oSheet.Cells[position, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                var content = (string)(oSheet.Cells[position + 1, 2] as Microsoft.Office.Interop.Excel.Range).Value;

                //                data.CONTENT = content;
                //                data.DATE_D4 = D4Date;
                //                data.DATE_CREATE = DateTime.Now;
                //                data.SCAR_ID = SCARID;
                //                data.WRITERBY = User.Identity.GetUserId();
                //                _iSCARService.SaveD4(data);
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D5")
                //    {
                //        #region
                //        SCAR_RESULT_D5 data = new SCAR_RESULT_D5();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D5, oSheet);
                //        Microsoft.Office.Interop.Excel.Range eRng = GetSpecifiedRange(FieldRespone.D6, oSheet);
                //        if (oRng != null && eRng != null)
                //        {
                //            position = oRng.Row;
                //            int endPos = eRng.Row;
                //            int dem = 1;
                //            for (int i = position + 1; i < endPos - 1; i++)
                //            {
                //                if ((oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //                {
                //                    var who = (string)(oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var when = (DateTime)(oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var content = (string)(oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value;

                //                    data.ITEM = dem;
                //                    data.WHEN_IMPLEMENT = when;
                //                    data.WHO_IMPLEMENT = who;
                //                    data.CONTENT = content;
                //                    data.DATE_CREATE = DateTime.Now;
                //                    data.SCAR_ID = SCARID;
                //                    data.WRITERBY = User.Identity.GetUserId();
                //                    _iSCARService.SaveD5(data);
                //                    dem++;
                //                }
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D6")
                //    {
                //        #region
                //        SCAR_RESULT_D6 data = new SCAR_RESULT_D6();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D6, oSheet);
                //        Microsoft.Office.Interop.Excel.Range eRng = GetSpecifiedRange(FieldRespone.D7, oSheet);
                //        if (oRng != null && eRng != null)
                //        {
                //            position = oRng.Row;
                //            int endPos = eRng.Row;
                //            int dem = 1;
                //            for (int i = position + 1; i < endPos - 1; i++)
                //            {
                //                if ((oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //                {
                //                    var who = (string)(oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var when = (DateTime)(oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var content = (string)(oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value;

                //                    data.ITEM = dem;
                //                    data.WHEN_IMPLEMENT = when;
                //                    data.WHO_IMPLEMENT = who;
                //                    data.CONTENT = content;
                //                    data.DATE_CREATE = DateTime.Now;
                //                    data.SCAR_ID = SCARID;
                //                    data.WRITERBY = User.Identity.GetUserId();
                //                    _iSCARService.SaveD6(data);
                //                    dem++;
                //                }
                //            }
                //        }
                //        #endregion
                //    }
                //    if (item == "D7")
                //    {
                //        #region
                //        SCAR_RESULT_D7 data = new SCAR_RESULT_D7();
                //        Microsoft.Office.Interop.Excel.Range oRng = GetSpecifiedRange(FieldRespone.D7, oSheet);
                //        Microsoft.Office.Interop.Excel.Range eRng = GetSpecifiedRange(FieldRespone.D8, oSheet);
                //        if (oRng != null && eRng != null)
                //        {
                //            position = oRng.Row;
                //            int endPos = eRng.Row;
                //            int dem = 1;
                //            for (int i = position + 1; i < endPos - 1; i++)
                //            {
                //                if ((oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value != null &&
                //                (oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value != null)
                //                {
                //                    var who = (string)(oSheet.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var when = (DateTime)(oSheet.Cells[i, 11] as Microsoft.Office.Interop.Excel.Range).Value;
                //                    var content = (string)(oSheet.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value;

                //                    data.ITEM = dem;
                //                    data.WHEN_IMPLEMENT = when;
                //                    data.WHO_IMPLEMENT = who;
                //                    data.CONTENT = content;
                //                    data.DATE_CREATE = DateTime.Now;
                //                    data.SCAR_ID = SCARID;
                //                    data.WRITERBY = User.Identity.GetUserId();
                //                    _iSCARService.SaveD7(data);
                //                    dem++;
                //                }
                //            }
                //        }
                //        #endregion
                //    }
                //}

                //oWB.Close();
                //oXL.Quit();
                #endregion
                return alert;
            }
            catch (Exception ex)
            {
                new LogWriter("GetCellValueD").LogWrite(ex.ToString());
                return "There was an error reading the file, please check your file!";
            }
            finally
            {
                spreadSheet?.Dispose();
            }
        }

        //public Microsoft.Office.Interop.Excel.Range GetSpecifiedRange(string matchStr, Microsoft.Office.Interop.Excel.Worksheet objWs)
        //{
        //    object missing = System.Reflection.Missing.Value;
        //    Microsoft.Office.Interop.Excel.Range currentFind = null;
        //    currentFind = objWs.get_Range("A1", "AM100").Find(matchStr, missing,
        //                   Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
        //                   Microsoft.Office.Interop.Excel.XlLookAt.xlPart,
        //                   Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
        //                   Microsoft.Office.Interop.Excel.XlSearchDirection.xlNext, false, missing, missing);
        //    return currentFind;

        //}

        public JsonResult CheckAcceptedStatus(string SCARID)
        {
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            bool status = _iSCARService.CheckAcceptedStatus(idSCAR);
            if (status == true)
            {
                try
                {
                    _iSCARService.UpdateStatusSCAR("Verification", SCARID);
                    return Json(new { success = true });
                }
                catch
                {
                    return Json(new { success = false, alert = "Update status failded" });
                }
            }
            return Json(new { success = false, alert = "" });
        }

        public JsonResult LoadCheckBoxD(string SCARID)
        {
            int idSCAR = Convert.ToInt32(SCARID.Substring(2));
            List<bool> result = _iSCARService.LoadCheckBoxD(idSCAR);
            return Json(new { success = result });
        }

        public JsonResult SaveReasonReject(string SCARID, string Reason)
        {
            SCAR_INFO_BACKUP data = _iSCARService.GetReasonReject(SCARID, Reason);
            data.WRITERREJECT = User.Identity.GetUserId();
            try
            {
                _iSCARService.SaveReasonReject(data);
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        public JsonResult SaveEditSCAR(EditSCARViewModel model)
        {
            try
            {
                SCARINFO data = _iSCARService.SaveEditSCAR(model);
                List<string> lstnum = _iSCARService.getNCRnumbyScar(model.SCAR_ID);
                string SCARID = data.ID.ToString().Length == 1 ? "VN00" + data.ID : (data.ID.ToString().Length == 2 ? "VN0" + data.ID : "VN" + data.ID);
                string path = CopyFileSCAR(SCARID, data.VERSION);

                if (path != "")
                {
                    string dest = Server.MapPath(ConfigurationManager.AppSettings["pathSCAR"]) + path;
                    ModifySCARFile(data, dest, lstnum);
                    return Json(new { success = true, path = path });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch
            {
                return Json(new { success = false });
            }
        }
              [HttpPost]
        public JsonResult EditDataD8(string SUPPLIER_REPRESENTATIVE, DateTime DATE_D8, string ACKNOWLEDGEMENT, string SCAR_ID)
        {
            if (SCAR_ID != "" || SCAR_ID != null)
            {
                bool result = _iSCARService.SaveDataD8Popup(SUPPLIER_REPRESENTATIVE, DATE_D8, ACKNOWLEDGEMENT, SCAR_ID);
                return Json(new { success = result });
            }
            return Json(new { success = false });
        }
        public JsonResult SaveFileD8(HttpPostedFileBase file,string ID,string content)
        {
            bool result = _iSCARService.SaveDataD8Edit(ID,file,content);
            return Json(new { success = result });
        }
        public JsonResult SaveDataD8(D8ViewModel model)
        {
            if (model.SCAR_ID != "" || model.SCAR_ID != null)
            {
                bool result = _iSCARService.SaveDataD8(model);
                return Json(new { success = result });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult SaveDataD8Popup(string SUPPLIER_REPRESENTATIVE,DateTime DATE_D8,string ACKNOWLEDGEMENT,string SCAR_ID)
        {
            if (SCAR_ID != "" || SCAR_ID != null)
            {
                bool result = _iSCARService.SaveDataD8Popup(SUPPLIER_REPRESENTATIVE, DATE_D8, ACKNOWLEDGEMENT, SCAR_ID);
                return Json(new { success = result });
            }
            return Json(new { success = false });
        }
        public FileContentResult DownloadFile(string file)
        {
            string filePath = Server.MapPath($"{ConfigurationManager.AppSettings["pathSCAR"]}{file}");
            if (System.IO.File.Exists(filePath))
            {
                byte[] filebyte = GetMediaFileContent(filePath);
                return File(filebyte, MimeMapping.GetMimeMapping(file), file);
            }
            else
            {
                return null;
            }
        }

        public ActionResult ViewSCAR(string scarid)
        {
            SCARInfoViewModel SCAR = _iSCARService.GetSCARInfoBySCARID(scarid);
            if (SCAR == null)
            {
                return RedirectToAction("Index");
            }

            switch (SCAR.STATUS.Trim())
            {
                case "Created": return Redirect(Url.Action("SentSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION }));
                case "Sent to Supplier": return Redirect(Url.Action("AcceptSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION }));
                case "Accepted by Supplier": return Redirect(Url.Action("AcceptSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION }));
                case "Verification": return Redirect(Url.Action("AcceptSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION, STATUS = SCAR.STATUS }));
              //  case "Supplier Completed": return Redirect(Url.Action("AcceptSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION, STATUS = SCAR.STATUS }));
              //  case "Completed All": return Redirect(Url.Action("CompletedAllSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION }));
                case "Voided": return Redirect(Url.Action("VoidSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION }));
                case "Closed": return Redirect(Url.Action("ClosedSCAR", "SCAR", new { SCARID = scarid, VERSION = SCAR.VERSION }));
                default: return RedirectToAction("Index");
            }
        }

        public ActionResult NCRWaitingforSCAR()
        {
            return View();
        }
        public JsonResult GetListwaitingSCAR([DataSourceRequest] DataSourceRequest request)
        {
            var list = _iSCARService.GetListwaittingSCAR();
                  return Json(list.ToDataSourceResult(request));
        }
    }
}
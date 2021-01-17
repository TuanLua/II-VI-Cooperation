using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Models.NCRReport;
using II_VI_Incorporated_SCM.Models.Templates;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Models.REPORT
{
    public class NCRReportController : Controller
    {
        // GET: NCRReport
        private readonly IReportNcrService _IReportNcrService;
        private readonly IUserService _userService;
        public NCRReportController(IReportNcrService IReportNcrService, IUserService userService)
        {
            _IReportNcrService = IReportNcrService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            ViewBag.DateList = _IReportNcrService.GetdropdownYear();
            return View();
        }
        public JsonResult GetNCRDispositionDay([DataSourceRequest]DataSourceRequest request, string year)
        {
            //   year = "2019";
            var list = _IReportNcrService.GetListNCRDispositionDay(year);

            return Json(list.ToDataSourceResult(request));
        }

        //public ActionResult EditingNCRDispositionDay([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<NCRDispositionDay> ListDay)
        //{
        //    var results = new List<NCRDispositionDay>();

        //    if (ListDay != null && ModelState.IsValid)
        //    {
        //        foreach (var ncrday in ListDay)
        //        {
        //            _IReportNcrService.Update(ncrday);
        //        }
        //    }

        //    return Json(results.ToDataSourceResult(request, ModelState));
        //}
        public JsonResult SaveNCRDispositionDay(string ListDay)
        {
            var obj = JsonConvert.DeserializeObject<List<NCRDispositionDay>>(ListDay);
            var results = new List<NCRDispositionDay>();

            if (ListDay != null && ModelState.IsValid)
            {
                foreach (var ncrday in obj)
                {
                    _IReportNcrService.Update(ncrday);
                }
            }

            return Json(true);
        }

        public ActionResult UseAsIs()
        {
            return View();
        }

        /// <summary>
        /// Fill Data To GridKendo TaskNCR
        /// By: Sil
        /// Date: 2018/05/28
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult ReadLotUseAsIs([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_IReportNcrService.GetListUseAsIs().ToDataSourceResult(request));
        }
        //moi them sau ban revert:
        public ActionResult Pareto()
        {
            return View();
        }
        public JsonResult ReadParetoGridforPartNum([DataSourceRequest] DataSourceRequest request, DateTime datefrom, DateTime dateto, string partnum)
        {
            return Json(_IReportNcrService.GetListPorentobyPartNum(datefrom, dateto, partnum).ToDataSourceResult(request));
        }
        public JsonResult ReadParetoPartNum(DateTime datefrom, DateTime dateto, string partnum)
        {
            return Json(_IReportNcrService.GetListPorentobyPartNum(datefrom, dateto, partnum));
        }
        public JsonResult ReadParetoGridRawdatabypartnum([DataSourceRequest] DataSourceRequest request, DateTime datefrom, DateTime dateto, string partnum)
        {
            return Json(_IReportNcrService.GetListPorentoRawdata(datefrom, dateto, partnum).ToDataSourceResult(request));
        }
        public ActionResult ParetobyVendor()
        {
            ViewBag.VendorList = _IReportNcrService.GetdropdownVendors();
            return View();
        }
        public JsonResult ReadParetoGridforVendor([DataSourceRequest] DataSourceRequest request, DateTime datefrom, DateTime dateto, string vendor)
        {
            return Json(_IReportNcrService.GetListPorentobyVendor(datefrom, dateto, vendor).ToDataSourceResult(request));
        }
        public JsonResult ReadParetoVendor(DateTime datefrom, DateTime dateto, string vendor)
        {
            return Json(_IReportNcrService.GetListPorentobyVendor(datefrom, dateto, vendor));
        }
        public JsonResult ReadParetoGridRawdatabyvendor([DataSourceRequest] DataSourceRequest request, DateTime datefrom, DateTime dateto, string vendor)
        {
            return Json(_IReportNcrService.GetListPorentoRawdatabyvendor(datefrom, dateto, vendor).ToDataSourceResult(request));
        }
        public ActionResult EscappingPPM()
        {
            ViewBag.User = User.Identity.GetUserId();
            return View();
        }
        [HttpPost]
        public JsonResult GetValueFile(HttpPostedFileBase FILE, bool check)
        {
            string relativePath = ConfigurationManager.AppSettings["FileReport"];
            string FolderPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var namefile = Guid.NewGuid().ToString();
            string path = Path.Combine(FolderPath, "EEP_" + namefile + ".xlsx");
            FILE.SaveAs(Path.Combine(FolderPath, "EEP_" + namefile + ".xlsx"));
            FILE.InputStream.Close();
            FILE.InputStream.Dispose();
            return Json(new { success = GetCellValue(path, check) });
        }
        SpreadsheetDocument spreadSheet = null;
        public int GetCellValue(string path, bool check)
        {
            var list = new List<ESCAPING_PPM>();
            spreadSheet = SpreadsheetDocument.Open(path, true);
            spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
            WorkbookPart workbookPart = spreadSheet.WorkbookPart;
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First();
            if (sheet != null)
            {
                string ncrnum = "";
                DateTime date = DateTime.Now;
                string analys = "";
                int qty = 0;
                DateTime period = DateTime.Now;
                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

                SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                for (int i = 5; i <= 300000; i++)
                {
                    try
                    {
                        Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == i).First();

                        if (row != null)
                        {
                            foreach (Cell c in row.Elements<Cell>())
                            {

                                string Col = c.CellReference.ToString().Substring(0, 1);
                                if (c.DataType != null)
                                {
                                    int index = int.Parse(c.CellValue.InnerText);
                                    string cellText = stringTable.SharedStringTable.ElementAt(index).InnerText;

                                    if (Col == "A")
                                    {
                                        ncrnum = cellText;
                                    }
                                    if (Col == "C")
                                    {
                                        analys = cellText;
                                    }
                                }
                                else
                                {
                                    if (Col == "B")
                                    {
                                        date = DateTime.FromOADate(Convert.ToDouble(c.InnerText));
                                    }
                                    if (Col == "D")
                                    {
                                        try
                                        {
                                            var QTYTEXT = (c.InnerText);
                                            var qt = Convert.ToDecimal(QTYTEXT);
                                            decimal qty1 = Math.Round(qt, 0);
                                            qty = Convert.ToInt32(qty1);
                                        }
                                        catch (Exception ex)
                                        {
                                            break;
                                        }
                                    }
                                    if (Col == "E")
                                    {
                                        period = DateTime.FromOADate(Convert.ToDouble(c.InnerText));
                                    }

                                }
                            }
                        }
                        list.Add(new ESCAPING_PPM()
                        {
                            NCR_NUM = ncrnum,
                            DATE = date,
                            ANALYST = analys,
                            QTY = qty,
                            PERIOD = period,
                            DATEIMPORT = DateTime.Now,
                            USERIMPORT = User.Identity.GetUserId()
                        });
                    }
                    catch(Exception ex)
                    {
                        break;
                    }
                }

            }
            var istrung = _IReportNcrService.checkidtrung(list);

            if (istrung == true && check) return Convert.ToInt32(_IReportNcrService.UpdateEscappingPPM(list));
            if (istrung == true && !check) return 2;

            return Convert.ToInt32(_IReportNcrService.SaveData(list));
        }
        public JsonResult ReadEscapPPMToProduction([DataSourceRequest] DataSourceRequest request, string yearselect)
        {
            return Json(_IReportNcrService.GetdataEEPtoproduction(yearselect).ToDataSourceResult(request));
        }
        public JsonResult ReadEscapPPMToComponent([DataSourceRequest] DataSourceRequest request, string yearselect)
        {

            return Json(_IReportNcrService.GetdataEEPtocomponent(yearselect).ToDataSourceResult(request));
        }
        public JsonResult SaveEEPToProduction(string ListDay)
        {
            var obj = JsonConvert.DeserializeObject<List<EEP_REPORT>>(ListDay);
            var results = new List<EEP_REPORT>();

            if (ListDay != null && ModelState.IsValid)
            {
                foreach (var ncrday in obj)
                {
                    _IReportNcrService.UpdateEEP(ncrday);
                }
            }

            return Json(true);
        }
        public JsonResult ReadEscapPPMToSystem([DataSourceRequest] DataSourceRequest request, string yearselect)
        {
            return Json(_IReportNcrService.GetdataEEPtoSystem(yearselect).ToDataSourceResult(request));
        }
        public ActionResult SupplierforPPM()
        {
            ViewBag.DateList = _IReportNcrService.GetdropdownYear();
            ViewBag.CCNList = _IReportNcrService.GetdropdownCCN();
            return View();
        }
     //   public ActionResult ReadSupplierforPPM(string part, string CCN, DateTime dateSta, DateTime dateDue)
       // {
         //   var list = _IReportNcrService.GetdataSupplierPPMTest(part, CCN, dateSta, dateDue);
           // return View(list);
        //}
       public JsonResult ReadSupplierforPPM([DataSourceRequest] DataSourceRequest request, string part, string CCN, DateTime dateSta, DateTime dateDue)
        {
           // int year = Int32.Parse(yearselect);
           return Json(_IReportNcrService.GetdataSupplierPPMTest(part, CCN,dateSta,dateDue).ToDataSourceResult(request));
        }
        //public JsonResult ReadSupplierforPPMTest([DataSourceRequest] DataSourceRequest request, string yearselect, string CCN)
        //{
        //    int year = Int32.Parse(yearselect);
        //    return Json(_IReportNcrService.GetdataSupplierPPMTest(year, CCN).ToDataSourceResult(request));
        //}
        public JsonResult SaveDataSupplierPPM(string ListDay)
        {
            var obj = JsonConvert.DeserializeObject<List<SUPPLIER_PPM_RP>>(ListDay);
            var results = new List<SUPPLIER_PPM_RP>();

            if (ListDay != null && ModelState.IsValid)
            {
                foreach (var ncrday in obj)
                {
                    _IReportNcrService.UpdateSupplierPPM(ncrday);
                }
            }

            return Json(true);
        }
        public ActionResult ViewSupplierStrategy(string CCN, string FY)
        {
            ViewBag.VendorList = _IReportNcrService.GetdropdownVendors();
            ViewBag.DateList = _IReportNcrService.GetdropdownYear();
            ViewBag.CCNList = _IReportNcrService.GetdropdownCCN();

            if (!string.IsNullOrEmpty(CCN) & !string.IsNullOrEmpty(FY))
            {
                ViewBag.DateList = _IReportNcrService.GetdropdownYear(FY.Trim());
                ViewBag.VendorList = _IReportNcrService.GetdropdownVendorsSelected(FY.Trim(), CCN.Trim());
                ViewBag.CCNList = _IReportNcrService.GetdropdownCCN(CCN.Trim());
            }
            return View();
        }
        public JsonResult StrategyList([DataSourceRequest] DataSourceRequest request, string CCN, string FY)
        {
            return Json(_IReportNcrService.StrategyList(FY, CCN).ToDataSourceResult(request));
        }
        public ActionResult SelectSupplierStrategy(string CCN, string FY)
        {
            ViewBag.VendorList = _IReportNcrService.GetdropdownVendors();
            ViewBag.DateList = _IReportNcrService.GetdropdownYear();
            ViewBag.CCNList = _IReportNcrService.GetdropdownCCN();

            if (!string.IsNullOrEmpty(CCN) & !string.IsNullOrEmpty(FY))
            {
                ViewBag.DateList = _IReportNcrService.GetdropdownYear(FY.Trim());
                ViewBag.VendorList = _IReportNcrService.GetdropdownVendorsSelected(FY.Trim(), CCN.Trim());
                ViewBag.CCNList = _IReportNcrService.GetdropdownCCN(CCN.Trim());
            }
            return View();
        }
        public ActionResult SaveSupplierStrategy(string CCN, string FY, string vendor)
        {
            List<string> listvendor = JsonConvert.DeserializeObject<List<string>>(vendor);
            List<SUPPLIER_PPM> listsupplier = new List<SUPPLIER_PPM>();

            foreach (var item in listvendor)
            {
                string[] arrayvendor = item.Split(',');
                string vendorvalue = arrayvendor[0];
                string purloc = arrayvendor[1];
                listsupplier.Add(new SUPPLIER_PPM
                {
                    CCN = CCN.Trim(),
                    FY = int.Parse(FY),
                    VENDOR = vendorvalue.Trim(),
                    PUR_LOC = purloc.Trim()
                });
            }
            var res = _IReportNcrService.InsertAndDelete(CCN, int.Parse(FY), listsupplier);
            return RedirectToAction("SelectSupplierStrategy", "NCRReport", new { CCN, FY });
        }

        public JsonResult ReadSupplier([DataSourceRequest] DataSourceRequest request, string yearselect, string CCN)
        {
            int year = int.Parse(yearselect) - 1;

            return Json(_IReportNcrService.GetDataReadSupplier(year, CCN).ToDataSourceResult(request));
        }
        public JsonResult ReadSupplierNone([DataSourceRequest] DataSourceRequest request, string yearselect, string CCN)
        {
            int year = int.Parse(yearselect) - 1;
            return Json(_IReportNcrService.GetDataReadSupplierNone(year, CCN).ToDataSourceResult(request));
        }
        public ActionResult Report4Panel()
        {
            ViewBag.VendorList = _IReportNcrService.GetdropdownVendors();
            ViewBag.DateList = _IReportNcrService.GetdropdownYear();
            return View();
        }
        public ActionResult Report4Panelby1Supplier([DataSourceRequest] DataSourceRequest request, DateTime yearselect, string id)
        {
            int FY = yearselect.Year;
            int Month = yearselect.Month;
            var lst = _IReportNcrService.GetDataReadSupplierOne(FY, id, Month);
            // var lstline = _IReportNcrService.GetLine(lst);
            return Json(lst.ToDataSourceResult(request));
        }
        public JsonResult SaveReport4Panelby1Supplier(string ListDay)
        {
            var obj = JsonConvert.DeserializeObject<List<ONESUPPLLIER_PPMRB>>(ListDay);
            var results = new List<ONESUPPLLIER_PPMRB>();

            if (ListDay != null && ModelState.IsValid)
            {
                foreach (var ncrday in obj)
                {
                    _IReportNcrService.UpdateSupplierPPMforOne(ncrday);
                }
            }
            return Json(true);
        }
        public ActionResult TopNCbyMonth(DateTime month, int qty, string id)
        {
            var lst = _IReportNcrService.getLstTopQTybmonthqts(month, qty, id);
            // var lstline = _IReportNcrService.GetLine(lst);
            return Json(lst);
        }
        public ActionResult TopNCbyyear(DateTime year, int qty, string id)
        {
            int FY = year.Year;
            var lst = _IReportNcrService.getLstTopQTybyYear(FY, qty, id);
            return Json(lst);
        }
        public JsonResult GetValueFile4panel(HttpPostedFileBase FILE, bool check, DateTime date)
        {
            string relativePath = ConfigurationManager.AppSettings["FileReport"];
            string FolderPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var namefile = Guid.NewGuid().ToString();
            string path = Path.Combine(FolderPath, "4Panel_" + namefile + ".xlsx");
            FILE.SaveAs(Path.Combine(FolderPath, "4Panel_" + namefile + ".xlsx"));
            FILE.InputStream.Close();
            FILE.InputStream.Dispose();
            return Json(new { success = GetCellValue4panel(path, check, date) });
        }
        /// <summary>
        /// 2: duplicate
        /// 1: save success
        /// 0: save unsuccess,
        /// </summary>
        public int GetCellValue4panel(string path, bool check, DateTime date)
        {
            var list = new List<PANEL_EXCEL>();
            spreadSheet = SpreadsheetDocument.Open(path, true);
            spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
            WorkbookPart workbookPart = spreadSheet.WorkbookPart;
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First();
            if (sheet != null)
            {
                string vendor = "";
                int Early = 0;
                int Late = 0;
                double Ontime = 0;
                double Total = 0;
                int Taget = 0;
                int Actual = 0;
                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

                SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                for (int i = 7; i <= 30000; i++)
                {
                    try
                    {
                        Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == i).First();

                        if (row != null)
                        {
                            foreach (Cell c in row.Elements<Cell>())
                            {

                                string Col = c.CellReference.ToString().Substring(0, 1);
                                if (c.DataType != null)
                                {
                                    int index = int.Parse(c.CellValue.InnerText);
                                    string cellText = stringTable.SharedStringTable.ElementAt(index).InnerText;
                                    if (Col == "A")
                                    {
                                        vendor = cellText;
                                    }
                                    //if (Col == "B")
                                    //{
                                    //    Early = Convert.ToInt32(cellText);
                                    //}
                                    //if (Col == "C")
                                    //{

                                    //    Late = Convert.ToInt32(cellText);
                                    //}
                                    //if (Col == "D")
                                    //{
                                    //    Ontime = Convert.ToInt32(cellText);
                                    //}
                                    //if (Col == "E")
                                    //{
                                    //    Total = (Convert.ToInt32(cellText));
                                    //}
                                    //if (Col == "F")
                                    //{
                                    //    Taget = (Convert.ToInt32(cellText));
                                    //}
                                    //if (Col == "G")
                                    //{
                                    //    Actual = (Convert.ToInt32(cellText));
                                    //}

                                }
                                else
                                {
                                    //if (Col == "A")
                                    //    {
                                    //        vendor = c.InnerText;
                                    //    }
                                    if (Col == "B")
                                    {
                                        Early = Convert.ToInt32(c.InnerText);
                                    }
                                    if (Col == "C")
                                    {

                                        Late = Convert.ToInt32(c.InnerText);
                                    }
                                    if (Col == "D")
                                    {

                                        Ontime = Math.Round(Convert.ToDouble(c.InnerText), 1);
                                    }
                                    if (Col == "E")
                                    {
                                        Total = Math.Round(Convert.ToDouble(c.InnerText), 1);
                                    }
                                    if (Col == "F")
                                    {
                                        Taget = (Convert.ToInt32(c.InnerText));
                                    }
                                    if (Col == "G")
                                    {
                                        Actual = (Convert.ToInt32(c.InnerText));
                                    }

                                }
                            }
                        }
                        list.Add(new PANEL_EXCEL()
                        {
                            VENDOR = vendor,
                            EARLY = Early,
                            LATE = Late,
                            ONTIME = Ontime,
                            TOTAL = Total,
                            TARGET = Taget,
                            ACTUAL = Actual,
                            Month = date
                        });
                    }
                    catch
                    {
                        break;
                    }
                }

            }
            var istrung = _IReportNcrService.checkidtrung4panel(list);

            if (istrung == true && check) return Convert.ToInt32(_IReportNcrService.Update4Panel(list));
            if (istrung == true && !check) return 2;
            return Convert.ToInt32(_IReportNcrService.SaveDataTo4Panel(list));
        }

        public JsonResult SaveReport4PanelOTD(string ListDay)
        {
            var obj = JsonConvert.DeserializeObject<List<PANEL_RP>>(ListDay);
            var results = new List<PANEL_RP>();

            if (ListDay != null && ModelState.IsValid)
            {
                foreach (var ncrday in obj)
                {
                    _IReportNcrService.SaveDataRAWORUPDATE(ncrday);
                }
            }
            return Json(true);
        }
        public JsonResult ReadOTD([DataSourceRequest] DataSourceRequest request, DateTime yearselect, string vendor)
        {
            return Json(_IReportNcrService.getDatarawOTD(yearselect, vendor).ToDataSourceResult(request));
        }
        public JsonResult ReadImproveStracking([DataSourceRequest] DataSourceRequest request, DateTime yearselect, string vendor)
        {
            int FY = yearselect.Year;
            int month = yearselect.Month;
            return Json(_IReportNcrService.getlistimprovestracking(FY, month, vendor.Trim()).ToDataSourceResult(request));
        }
        public JsonResult ReadSCARProblem([DataSourceRequest] DataSourceRequest request, DateTime yearselect, string vendor)
        {
            // int FY = yearselect.Year;
            return Json(_IReportNcrService.getLstScarProblem(yearselect, vendor.Trim()).ToDataSourceResult(request));
        }
        [HttpPost]
        public ActionResult Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        public JsonResult ReadDataRawViewPPM([DataSourceRequest] DataSourceRequest request, DateTime yearselect, string vendor)
        {
            int FY = yearselect.Year;
            int month = yearselect.Month;
            return Json(_IReportNcrService.GetDataRawRejQty4panelPPM(FY, month, vendor.Trim()).ToDataSourceResult(request));
        }
        public ActionResult ImportOTD()
        {
            return View();
        }

        public ActionResult Export(DateTime yearselect, string vendor, int mqty, int yqty)
        {
            var log = new LogWriter("NCRReport-Export");
            string ex4panelPath = Server.MapPath(ConfigurationManager.AppSettings["4PanelExcelPath"]),
                   filename = Guid.NewGuid().ToString(),
                   tempPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]),
                   newPath = $"{tempPath}TEMP\\{filename}.xlsx";
            log.LogWrite($"New temp file: {newPath}");
            var user = _userService.GetNameById(User.Identity.GetUserId());
            FourPanel fourPanel = new FourPanel(Server.MapPath(ConfigurationManager.AppSettings["4PanelTempatePath"]));
            try
            {
                log.LogWrite($"CopyFile: {newPath}");
                CopyFile(ex4panelPath, newPath);

                #region Change value
                ExcelSupport.UpdateCell(newPath, user, fourPanel.FullNameIndex, fourPanel.FullNameCol, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(newPath, string.Format(fourPanel.VendorText, vendor), fourPanel.VendorIndex, fourPanel.VendorCol, "RP", CellValues.Date);
                #endregion

                #region PPM
                int FY = yearselect.Year;
                int Month = yearselect.Month;
                log.LogWrite($"gridOneSupplier: {FY} - {Month}");

                var lstgridOneSupplier = _IReportNcrService.GetDataReadSupplierOne(FY, vendor, Month);
                ModifygridOneSupplier(lstgridOneSupplier, newPath, yearselect, fourPanel);

                #endregion

                #region OTD
                 var OTD = _IReportNcrService.getDatarawOTD(yearselect, vendor);
                ModifyOTD(OTD, path:newPath, dateTime: yearselect, fourPanel);
                #endregion

                #region Top NC By Month
                var tbm = _IReportNcrService.getLstTopQTybmonthqts(yearselect, mqty, vendor);
                TopByMonth(path: newPath, dateTime: yearselect, data: tbm, vendor);
                //new ReportWithChart().CreateExcelDoc(newPath);
                #endregion

                #region Top NC By Year
                var tby = _IReportNcrService.getLstTopQTybyYear(FY, yqty, vendor);
                TopByYear(path: newPath, dateTime: yearselect, data: tby, vendor);
                #endregion

                #region Improvement Tracking
                var imp = _IReportNcrService.getlistimprovestracking(FY, yearselect.Month, vendor.Trim());
                ImprovementTracking(path: newPath, data: imp, yearselect, fourPanel);
                #endregion

                #region SCAR PROBLEM
                var scarpdata = _IReportNcrService.getLstScarProblem(yearselect, vendor.Trim());
                SCARPROBLEM(path: newPath, fourPanel, yearselect, vendor.Trim(), data: scarpdata);
                #endregion

                return File(fileName: newPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                log.LogWrite(!string.IsNullOrEmpty(ex.Message) ? ex.Message : "");
            }
            return null;
        }

        private void SCARPROBLEM(string path, FourPanel fourPanel, DateTime yearselect, string v, List<Scarviewmodel> data)
        {
            #region data emp
            //data.Add(new Scarviewmodel {
            //    No = "1",
            //    Owner = "Tester",
            //    ActualDate = DateTime.Now,
            //    Corective = "Error 1",
            //    Status = "New",
            //    TargetDate = DateTime.Now.AddDays(1)
            //});
            #endregion
            var scar = fourPanel.SCARProblem;
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                ExcelSupport.UpdateCell(path, item.No, scar.Begin + 1 + i, scar.Ref.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.Corective, scar.Begin + 1 + i, scar.CAD.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.TargetDate.ToString("MMM-yy"), scar.Begin + 1 + i, scar.TargetDate.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.ActualDate.ToString("MMM-yy"), scar.Begin + 1 + i, scar.ActualDate.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.Status, scar.Begin + 1 + i, scar.Status.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.Owner, scar.Begin + 1 + i, scar.Owner.Begin, "RP", CellValues.Date);

            }
        }

        private void ImprovementTracking(string path, List<ImprovestrackingViewmodel> data, DateTime dateTime, FourPanel fourPanel)
        {
            var IMP = fourPanel.ImprovementTracking;
            DateTime date = DateTime.Parse(string.Format("07/01/{0}", dateTime.Year));
            ExcelSupport.UpdateCell(path, date.ToString("MMM-yy"), IMP.Begin, IMP.JUL.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.AUG.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.SEP.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.OCT.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.NOV.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.DEC.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.JAN.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.FEB.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.MAR.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.APR.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.MAY.Begin, "RP", CellValues.Date);
            ExcelSupport.UpdateCell(path, date.AddMonths(1).ToString("MMM-yy"), IMP.Begin, IMP.JUN.Begin, "RP", CellValues.Date);

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                ExcelSupport.UpdateCell(path, item.No, IMP.Begin+ 1 + i, IMP.Ref.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, !string.IsNullOrEmpty(item.NC_NAME) ? item.NC_NAME.ToString(): "", IMP.Begin+ 1, IMP.Desc.Begin, "RP", CellValues.Date);

                ExcelSupport.UpdateCell(path, item.JUL.ToString(), IMP.Begin+ 1 + i, IMP.JUL.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.AUG.ToString(), IMP.Begin+ 1 + i, IMP.AUG.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.SEP.ToString(), IMP.Begin+ 1 + i, IMP.SEP.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.OCT.ToString(), IMP.Begin+ 1 + i, IMP.OCT.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.NOV.ToString(), IMP.Begin+ 1 + i, IMP.NOV.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.DEC.ToString(), IMP.Begin+ 1 + i, IMP.DEC.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.JAN.ToString(), IMP.Begin+ 1 + i, IMP.JAN.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.FEB.ToString(), IMP.Begin+ 1 + i, IMP.FEB.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.MAR.ToString(), IMP.Begin+ 1 + i, IMP.MAR.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.APR.ToString(), IMP.Begin+ 1 + i, IMP.APR.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.MAY.ToString(), IMP.Begin+ 1 + i, IMP.MAY.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.JUN.ToString(), IMP.Begin+ 1 + i, IMP.JUN.Begin, "RP", CellValues.Date);
                ExcelSupport.UpdateCell(path, item.SUMYTD.ToString(), IMP.Begin+ 1 + i, IMP.YTD.Begin, "RP", CellValues.Date);
            }
        }

        private void TopByYear(string path, DateTime dateTime, List<topmonthViewmodel> data, string vendor)
        {
           // int month = dateTime.Month;
           // int year = month >= 1 & month <= 6 ? dateTime.Year + 1 : dateTime.Year;
           // ExcelSupport.UpdateCell(path, $"{month}-01-{year}", 1, "AG", "NC by PN", CellValues.Date);
            int i = 2;
            foreach (var item in data)
            {
                ExcelSupport.UpdateCell(path, item.PartNum, i, "P", "NC by PN", CellValues.String);
                ExcelSupport.UpdateCell(path, item.Value.ToString(), i, "Q", "NC by PN", CellValues.Number);
                i++;
            }
        }

        private void TopByMonth(string path, DateTime dateTime, List<topmonthViewmodel> data, string vendor)
        {
            int month = dateTime.Month;
            int year = month >= 1 & month <= 6 ? dateTime.Year + 1 : dateTime.Year;
            ExcelSupport.UpdateCell(path, $"{month}-01-{year}", 1, "AG", "NC by PN", CellValues.Date);
            int i = 2;
            foreach (var item in data)
            {
                ExcelSupport.UpdateCell(path, item.PartNum, i, "AF", "NC by PN", CellValues.String);
                ExcelSupport.UpdateCell(path, item.Value.ToString(), i, "AG", "NC by PN", CellValues.Number);
                i++;
            }
        }

        private void ModifyOTD(List<PANEL_RP> oTD, string path, DateTime dateTime, FourPanel fourPanel)
        {

            var Early = oTD.FirstOrDefault(x => x.TYPE.Equals("Early")); //B
            var Late = oTD.FirstOrDefault(x => x.TYPE.Equals("Late")); //C
            var OnTime = oTD.FirstOrDefault(x => x.TYPE.Equals("OnTime"));//D
            var Total = oTD.FirstOrDefault(x => x.TYPE.Equals("Total"));//E
            var TarGet = oTD.FirstOrDefault(x => x.TYPE.Equals("TarGet"));//F
            var Actual = oTD.FirstOrDefault(x => x.TYPE.Equals("Actual"));//G

            //int Index = cell.IndexOfAny("0123456789".ToCharArray());
            //int fourPanel.OTD.Begin = int.Parse(cell.Substring(Index, cell.Length - 1));
            //var rName = cell.Substring(0, Index);

            for (int i = 0; i < 12; i++)
            {
                
                int month = i + 7 > 12 ? i + 7 - 12 : i + 7;
                int year = month < 7 ? dateTime.Year + 1 : dateTime.Year;
                DateTime date = DateTime.Parse($"{month}/1/{year}");
                ExcelSupport.UpdateCell(path, date.ToString("MMM-yy"), fourPanel.OTD.Begin + i, fourPanel.OTD.Month.Begin, "RP", CellValues.Date);

                switch (month)
                {
                    case 7:
                        {
                            ExcelSupport.UpdateCell(path, Early.JUL + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.JUL + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.JUL + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.JUL + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.JUL/100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.JUL / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 8:
                        {
                            ExcelSupport.UpdateCell(path, Early.AUG + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.AUG + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.AUG + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.AUG + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.AUG / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.AUG / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 9:
                        {
                            ExcelSupport.UpdateCell(path, Early.SEP + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.SEP + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.SEP + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.SEP + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.SEP / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.SEP / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 10:
                        {
                            ExcelSupport.UpdateCell(path, Early.OTC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.OTC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.OTC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.OTC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.OTC / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.OTC / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 11:
                        {
                            ExcelSupport.UpdateCell(path, Early.NOV + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.NOV + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.NOV + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.NOV + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.NOV / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.NOV / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 12:
                        {
                            ExcelSupport.UpdateCell(path, Early.DEC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.DEC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.DEC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.DEC + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.DEC / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.DEC / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 1:
                        {
                            ExcelSupport.UpdateCell(path, Early.JAN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.JAN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.JAN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.JAN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.JAN / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.JAN / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 2:
                        {
                            ExcelSupport.UpdateCell(path, Early.FEB + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.FEB + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.FEB + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.FEB + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.FEB / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.FEB / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 3:
                        {
                            ExcelSupport.UpdateCell(path, Early.MAR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.MAR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.MAR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.MAR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.MAR / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.MAR / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 4:
                        {
                            ExcelSupport.UpdateCell(path, Early.APR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.APR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.APR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.APR + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.APR / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.APR / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 5:
                        {
                            ExcelSupport.UpdateCell(path, Early.MAY + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.MAY + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.MAY + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.MAY + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.MAY / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.MAY / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 6:
                        {
                            ExcelSupport.UpdateCell(path, Early.JUN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Early.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Late.JUN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Late.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, OnTime.JUN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.OnTime.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Total.JUN + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Total.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TarGet.JUN / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, Actual.JUN / 100 + "", fourPanel.OTD.Begin + i, fourPanel.OTD.Actual.Begin, "RP", CellValues.Number);
                        }
                        break;
                }

            }
        }

        private void ModifygridOneSupplier(List<OneSupplierforPPM> lst, string path, DateTime dateTime, FourPanel fourPanel)
        {
            //DateTime.Parse(text).ToOADate().ToString()
            //ExcelSupport.UpdateCell(path, dateTime.ToString("MM/dd/yyy"), 54, "A", "RP", CellValues.Date);
            
            //var PPMYTD = lst.FirstOrDefault(x => x.TYPE.Equals("PPM YTD"));

            var REC = lst.FirstOrDefault(x => x.TYPE.Equals("REC")); //B
            var REJEC = lst.FirstOrDefault(x => x.TYPE.Equals("REJEC")); //C
            var TARGET = lst.FirstOrDefault(x => x.TYPE.Equals("TARGET"));//D
            var PPMYTD = lst.FirstOrDefault(x => x.TYPE.Equals("PPM YTD"));//D
            for (int i = 0; i < 12; i++)
            {
                int month = i + 7 > 12 ? i + 7 - 12 : i + 7;
                int year = month < 7 ? dateTime.Year + 1 : dateTime.Year;
                DateTime date = DateTime.Parse($"{month}/1/{year}");
                ExcelSupport.UpdateCell(path, date.ToString("MMM-yy"), fourPanel.PPM.Begin + i, fourPanel.PPM.Month.Begin, "RP", CellValues.Date);
                switch (month)
                {
                    case 7:
                        {
                            ExcelSupport.UpdateCell(path, REC.JUL + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.JUL + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.JUL + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.JUL + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 8:
                        {
                            ExcelSupport.UpdateCell(path, REC.AUG + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.AUG + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.AUG + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.AUG + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 9:
                        {
                            ExcelSupport.UpdateCell(path, REC.SEP + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.SEP + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.SEP + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.SEP + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 10:
                        {
                            ExcelSupport.UpdateCell(path, REC.OCT + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.OCT + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.OCT + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.OCT + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 11:
                        {
                            ExcelSupport.UpdateCell(path, REC.NOV + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.NOV + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.NOV + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.NOV + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 12:
                        {
                            ExcelSupport.UpdateCell(path, REC.DEC + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.DEC + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.DEC + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.DEC + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 1:
                        {
                            ExcelSupport.UpdateCell(path, REC.JAN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.JAN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.JAN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.JAN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 2:
                        {
                            ExcelSupport.UpdateCell(path, REC.FEB + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.FEB + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.FEB + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.FEB + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 3:
                        {
                            ExcelSupport.UpdateCell(path, REC.MAR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.MAR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.MAR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.MAR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 4:
                        {
                            ExcelSupport.UpdateCell(path, REC.APR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.APR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.APR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.APR + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 5:
                        {
                            ExcelSupport.UpdateCell(path, REC.MAY + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.MAY + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.MAY + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.MAY + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                    case 6:
                        {
                            ExcelSupport.UpdateCell(path, REC.JUN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RecQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, REJEC.JUN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.RejQty.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, TARGET.JUN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.Target.Begin, "RP", CellValues.Number);
                            ExcelSupport.UpdateCell(path, PPMYTD.JUN + "", fourPanel.PPM.Begin + i, fourPanel.PPM.PPMYTD.Begin, "RP", CellValues.Number);
                        }
                        break;
                }
            }

        }

        public void CopyFile(string oldPath, string newPath, int ver = 0)
        {
            try
            {
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Copy(oldPath, newPath, true);
                }
            }
            catch (Exception ex)
            {
                new LogWriter("NCRReport-CopyFile").LogWrite(ex.ToString());
            }
        }
        public ActionResult GetpartbyDate(string dateSta, string dateDue)
        {
            LogWriter log = new LogWriter("GetpartbyDate - Start write log Getsupplierbytype");
            try
            {
                var part = _IReportNcrService.GetPartbyDate(dateSta, dateDue);
                return Json(new { success = true, data = part });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
        public ActionResult Getsupplierbytype(string type,string dateSta, string dateDue)
        {
            LogWriter log = new LogWriter("Getsupplierbytype - Start write log Getsupplierbytype");
            try
            {
                var part = _IReportNcrService.Getsupplierbytype(type,dateSta, dateDue);
                return Json(new { success = true, data = part });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    }
}
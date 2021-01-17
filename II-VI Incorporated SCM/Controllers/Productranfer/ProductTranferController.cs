using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.Producttranfer;
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
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;

namespace II_VI_Incorporated_SCM.Controllers.Productranfer
{
    public class ProductTranferController : Controller
    {
        private readonly IProductTranferService _IProductTranferService;
        private readonly ITaskManagementService _iTaskManagementService;
        private readonly IUserService _IUserService;
        public ProductTranferController(ICCNService iCCNService, ITaskManagementService iTaskManagementService, IProductTranferService _iProductTranferService, IReciverService IReciverService, INCRManagementService INCRManagementService, IUserService IUserService, IEmailService emailService)
        {

            _IProductTranferService = _iProductTranferService;
            _IUserService = IUserService;
            _iTaskManagementService = iTaskManagementService;

        }

        // GET: ProductTranfer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListProductTranfer([DataSourceRequest]DataSourceRequest request)
        {
            List<sp_PT_GetProTransfer_List_Result> list = _IProductTranferService.getlistdata();
            // _INCRManagementService.updateAgeNCRinListNCR(list);
            return Json(list.ToDataSourceResult(request));
        }
        public string GetRoleByID()
        {
            string iduser = User.Identity.GetUserId();
            return JsonConvert.SerializeObject(_IProductTranferService.getrole(iduser));
        }
        public ActionResult CreateProductranfer(string PartNum)
       {
            string iduser = User.Identity.GetUserId();
            ViewBag.PartNum = PartNum;
            ViewBag.Roled = JsonConvert.SerializeObject(_IProductTranferService.getrole(iduser));
            ViewBag.IsInitiator = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.Initiator);
            
            ViewBag.PE = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.PE);
            ViewBag.OwnerCheck = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.VNOwner);


            ViewBag.UserCreateInitiator = _IProductTranferService.IsUsercreate(iduser,PartNum);
            ViewBag.UserUpdate = _IProductTranferService.IsUserUpdate(iduser, PartNum);
            ViewBag.UserOner = _IProductTranferService.IsUserOwner(iduser, PartNum);
              ViewBag.CheckEdit = _IProductTranferService.GetStepstautusedit(PartNum);
            ViewBag.Step1 = _IProductTranferService.GetStep1stautussave(PartNum);
            ViewBag.Step2 = _IProductTranferService.GetStep2stautussave(PartNum);
            ViewBag.Step3 = _IProductTranferService.GetStep3stautussave(PartNum);
            ViewBag.Step4 = _IProductTranferService.GetStep4stautussave(PartNum);
            ProductViewModel data = _IProductTranferService.getproduct(PartNum);
            data.Checklist = _IProductTranferService.getchecklist(PartNum);
            ViewBag.Build = _IProductTranferService.GetdropdownBuild();
            ViewBag.Section = _IProductTranferService.GetdropdownSection();
            ViewBag.WC = _IProductTranferService.GetdropdownWork();
            ViewBag.Owner = _IProductTranferService.GetListUserRoleOwner("VN Owner");
          //  data.Listpart = _IProductTranferService.getchecklist(PartNum,iduser);
            ViewBag.CreateFlag = string.IsNullOrEmpty(PartNum);
            ViewBag.UpdateFlag = _IProductTranferService.IsUpdate(PartNum);
            ViewBag.ListTask = _IProductTranferService.GetListTaskProduct();
            return View(data);
        }
        public ActionResult Initailinfo()
        {
            return PartialView();
        }
        public ActionResult Updateinfo()
        {
            return PartialView();
        }
        public ActionResult EditProductranfer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveInitailinfo(ProductViewModel model)
        {
            try
            {
                string iduser = User.Identity.GetUserId();
                string email = _IProductTranferService.getemailbyid(iduser);
                tbl_PT_Infor data = new tbl_PT_Infor
                {
                    Part_Num = model.Part_Num,
                    Initial_Note = model.Initial_Note,
                    Plan_Type = model.Plan_Type,
                    Description = model.Description,
                    Date = DateTime.Now,
                    Initail_User = iduser,
                    //   Role = "Initiator",
                    Status = "In Process"
                };
                if(model.type == "Edit")
                {
                    List<tbl_PT_Infor> listpart = _IProductTranferService.getTransferbyuser(data.Part_Num, data.Initail_User);
                    Result res = _IProductTranferService.SaveEditProductstranfer(data, model.edit);
                    if (res.message == "submit")
                    {
                        foreach (var item in listpart)
                        {
                            if (item.Plan_Type == "Buy Part")
                            {
                                _iTaskManagementService.AddTaskProductTranfer_BuyPart(item.Part_Num, iduser);
                            }
                            

                        }
                        var sentmail = _IProductTranferService.sentmailsubmitInitial(iduser, email);
                    }
                    return Json(new { res.success, message = "submit", obj = res.obj });
                }
                else {
                    List<tbl_PT_Infor> listpart = _IProductTranferService.getTransferbyuser(data.Part_Num, data.Initail_User);
                    Result res = _IProductTranferService.SaveProductstranfer(data, model.type);
                    if (res.message == "submit")
                    {
                        
                            
                            foreach (var item in listpart)
                            {
                                if (item.Plan_Type == "Buy Part")
                                {
                                    _iTaskManagementService.AddTaskProductTranfer_BuyPart(item.Part_Num, iduser);
                                }
                                
                            }
                            var sentmail = _IProductTranferService.sentmailsubmitInitial(iduser, email);

                    }
                    return Json(new { res.success, message = res.message, obj = res.obj });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }
        }
        [HttpPost]
        public ActionResult SaveUpdate(ProductViewModel model)
        {
            try
            {
                string iduser = User.Identity.GetUserId();
                tbl_PT_Infor data = new tbl_PT_Infor
                {
                    Part_Num = model.Part_Num,
                    Build_Loc = model.Build_Loc,
                    Plan_Yield = model.Plan_Yield,
                    Wc = model.Wc,
                    Setion = model.Setion,
                    Vn_Owner = model.Vn_Owner,
                    PE_User = iduser,
                    PE_Note = model.PE_Note,
                };
                Result res = _IProductTranferService.UpdateProductstranfer(data, model.type);

                if (res.success && model.type == "Submit")
                {
                    string email = _IProductTranferService.getemailbyid(model.Vn_Owner);
                    //sent mail owner 
                    var sentmail = _IProductTranferService.sentmailsubmitvnower(model.Part_Num, email);
                    //auto create Task
                    //but no need for Tooling(thi.nguyen 11-Jan-2021)
                    if (model.Setion!="Tooling")
                    {
                        var resApp = _iTaskManagementService.AddTaskProductTranfer(model.Part_Num, iduser, model.Vn_Owner);
                        //sent mail user dept
                        var mail = _IProductTranferService.sentmailsubmitchecktask(model.Part_Num);
                    }
                    return Json(new { res.success, message = "submit", obj = res.obj });
                }
                else
                {
                    return Json(new { res.success, message = res.message, obj = res.obj });

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }
        }
        public ActionResult TaskProductView(string PartNum)
        {
            ViewBag.partnum = PartNum;
            return PartialView();
        }
        public JsonResult ReadTaksMantProduct([DataSourceRequest] DataSourceRequest request, string partnum)
        {
            return Json(_IProductTranferService.GetListTaskProduct(partnum).ToDataSourceResult(request));
        }
        [HttpGet]
        public ActionResult CheckSheet(string PartNum)
        {
            ProductViewModel data = new ProductViewModel
            {
                Checklist = _IProductTranferService.getchecklist(PartNum)
            };
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult SaveCheckSheet(List<checklistview> listresult, string partnum)
        {
            string iduser = User.Identity.GetUserId();
            var res = _IProductTranferService.SaveChecklist(listresult, partnum,iduser);
            return Json(new { res.success, message = res.message, obj = res.obj });
        }
        public ActionResult CreateChecksheet(string PartNum)
        {
            ViewBag.PartTT = PartNum;
            return View();
        }
        public ActionResult CopyChecksheet(string PartNum)
        {
            ViewBag.PartTT = PartNum;
            ViewBag.PartNumCopy = _IProductTranferService.GetdropdownPart();
            return View();
        }
        [HttpPost]
        public ActionResult GetchecksheetbyPart([DataSourceRequest]DataSourceRequest request, string part)
        {
            List<ProductViewModel> list = _IProductTranferService.getlistdatabypart(part);
            return Json(list.ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult SaveCopyChechSheetbypart(string Part, string PartCurrent)
        {
            try
            {
                string iduser = User.Identity.GetUserId();
                Result res = _IProductTranferService.SaveCopyCheckSheet(Part, PartCurrent,iduser);
                return Json(new { res.success, message = res.message, obj = res.obj });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }
        }
      [HttpPost]
        public ActionResult DeleteDestroy(string part,string Item_Desc,byte Index)
        {
            try
            {
                Result res = _IProductTranferService.DeletedItemCheckList(part,Item_Desc,Index);
                return Json(new { res.success, message = res.message, obj = res.obj });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }

        }
        [HttpPost]
        public ActionResult SaveUpdateCheckSheetConlusion(ProductViewModel model)
        {
            try
            {
                string File_Upload_Url ="";
                string iduser = User.Identity.GetUserId();
                if (model.File != null)
                {
                 File_Upload_Url = SaveFile(model.File);
                }
                tbl_PT_Infor data = new tbl_PT_Infor
                {
                    Part_Num = model.Part_Num,
                    GM = model.GM,
                    Conclution = model.Conlusion,
                    Yield = model.Yield,
                    FileConclusion = File_Upload_Url
                };
                Result res = _IProductTranferService.UpdateConlusionProductstranfer(data, model.type, iduser);
                return Json(new { res.success, message = res.message, obj = res.obj });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }
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
        public FileContentResult DownloadFile(string fileId, string filename)
        {
            string filePath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]);
            tbl_PT_Infor sf = _IProductTranferService.GetFileWithFile(fileId, filename);
            if (sf != null)
            {
                string filePathFull = (filePath + sf.FileConclusion);
                byte[] file = GetMediaFileContent(filePathFull);
                return File(file, MimeMapping.GetMimeMapping(sf.FileConclusion), sf.FileConclusion);
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
        public JsonResult SaveListTask(ListTaskModel listTask)
        {
            Result res = new Result();
            return Json(new { success = true, message = res.message });
        }
        public JsonResult SaveAddCheckList(string item, string required, string remark, string partnum, string type)
        {
            Result res = _IProductTranferService.SaveItemCheckSheet(item, required,remark,partnum, type);
            return Json(new { success = true, message = res.message});

        }
        public JsonResult Editchecklistingrid(string part, string Item_Desc, byte Index)
        {
            Result res = _IProductTranferService.EditItemCheckList(part, Item_Desc, Index);
            return Json(new { success = true, message = res.message });

        }
        [HttpPost]
        public ActionResult ListPartByUserCreate([DataSourceRequest]DataSourceRequest request)
        {
            string iduser = User.Identity.GetUserId();
            List<ProductViewModel> list = _IProductTranferService.getlistdatabyuser(iduser);
            // _INCRManagementService.updateAgeNCRinListNCR(list);
            return Json(list.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Update([DataSourceRequest] DataSourceRequest request, ProductViewModel product)
        {
            if (product != null && ModelState.IsValid)
            {
                //productService.Update(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }
[HttpPost]
        public JsonResult GetValueFile(HttpPostedFileBase File)
        {
            string relativePath = ConfigurationManager.AppSettings["uploadPath"];
            string FolderPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var namefile = Guid.NewGuid().ToString();
            string path = Path.Combine(FolderPath, "Product_" + namefile + ".xlsx");
            File.SaveAs(Path.Combine(FolderPath, "Product_" + namefile + ".xlsx"));
            File.InputStream.Close();
            File.InputStream.Dispose();
            return Json(new { success = GetCellValue(path, true) });
        }
        SpreadsheetDocument spreadSheet = null;
        public int GetCellValue(string path, bool check)
        {
            //var list = new List<tbl_PT_Infor>();
            //spreadSheet = SpreadsheetDocument.Open(path, true);
            //spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
            //WorkbookPart workbookPart = spreadSheet.WorkbookPart;
            //Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First();
            //if (sheet != null)
            //{
            //    string partnum = "";
            //    string type  = "";
            //    string Description = "";
            //  //  int qty = 0;
            //    DateTime period = DateTime.Now;
            //    WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

            //    SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            //    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            //    for (int i = 18; i <= 300000; i++)
            //    {
            //        bool isAdd = false;
            //        try
            //        {
            //            Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == i).First();

            //            if (row != null)
            //            {
            //                foreach (Cell c in row.Elements<Cell>())
            //                {
            //                    string Col = c.CellReference.ToString().Substring(0, 1);
            //                    if (c.DataType != null && c.CellValue != null)
            //                    {
            //                        isAdd = true;
            //                        int index = 0;
            //                        if (!string.IsNullOrEmpty(c.CellValue.InnerText))
            //                        {
            //                            index  = int.Parse(c.CellValue.InnerText);
            //                        }
            //                        string cellText = stringTable.SharedStringTable.ElementAt(index).InnerText;

            //                        if (Col == "A")
            //                        {
            //                            partnum = cellText;
            //                        }
            //                        if (Col == "B")
            //                        {
            //                            Description = cellText;
            //                        }
            //                        if (Col == "G" && cellText == "Y")
            //                        {
            //                                type = "Make Part";
            //                        }
            //                        if (Col == "H" && cellText == "Y")
            //                        {
            //                                type = "Buy Part";
            //                        }

            //                    }
            //                }
            //            }
            //            if(isAdd)
            //            list.Add(new tbl_PT_Infor()
            //            {
            //                Part_Num = partnum,
            //                Description = Description,
            //                Plan_Type = type,
            //                Initial_Note = "",
            //                Date = DateTime.Now,
            //                Status = "In Process",
            //                Initail_User = User.Identity.GetUserId()
            //            });
            //        }
            //        catch(Exception ex)
            //        {
            //            break;
            //        }
            //    }
            //}
            //var a = list;
            //var issave = _IProductTranferService.SaveData(a);
            //if (issave)
            //{
            //    return 1;
            //}
            //else
            //{
            //    return 2;
            //}

                var list = new List<tbl_PT_Infor>();
                spreadSheet = SpreadsheetDocument.Open(path, true);
                spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
                WorkbookPart workbookPart = spreadSheet.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First();
                if (sheet != null)
                {
                    string partnum = "";
                    string type = "";
                    string Description = "";
                    //  int qty = 0;
                    DateTime period = DateTime.Now;
                    WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

                    SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    for (int i = 18; i <= 300000; i++)
                    {
                        bool isAdd = false;
                        try
                        {
                            Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == i).First();


                            if (row != null)
                            {
                                foreach (Cell c in row.Elements<Cell>())
                                {
                                    string Col = c.CellReference.ToString().Substring(0, 1);
                                    if (c.DataType != null && c.CellValue != null)
                                    {
                                        isAdd = true;
                                        int index = 0;
                                        if (!string.IsNullOrEmpty(c.CellValue.InnerText))
                                        {
                                            index = int.Parse(c.CellValue.InnerText);
                                        }
                                        string cellText = stringTable.SharedStringTable.ElementAt(index).InnerText;

                                        if (Col == "A")
                                        {
                                            partnum = cellText;
                                        }
                                        if (Col == "B")
                                        {
                                            Description = cellText;
                                        }
                                        if (Col == "G" && cellText.ToUpper() == "Y")
                                        {
                                            type = "Make Part";
                                        }
                                        if (Col == "H" && cellText.ToUpper() == "Y")
                                        {
                                            type = "Buy Part";
                                        }

                                    }
                                }
                            }
                            if (isAdd)
                            {
                                list.Add(new tbl_PT_Infor()
                                {
                                    Part_Num = partnum,
                                    Description = Description,
                                    Plan_Type = type,
                                    Initial_Note = "",
                                    Date = DateTime.Now,
                                    Status = "In Process",
                                    Initail_User = User.Identity.GetUserId()
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    var a = list;
                    var issave = _IProductTranferService.SaveData(a);
                    if (issave)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 2;
                }
            }
        
    }
}
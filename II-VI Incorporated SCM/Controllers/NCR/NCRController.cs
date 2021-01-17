using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using EvoPdf;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Services;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class NCRController : Controller
    {
        private readonly ICCNService _ICCNService;
        private readonly INCRManagementService _INCRManagementService;
        private readonly IUserService _IUserService;
        public NCRController(ICCNService iCCNService, INCRManagementService INCRManagementService, IUserService IUserService)
        {
            _ICCNService = iCCNService;
            _INCRManagementService = INCRManagementService;
            _IUserService = IUserService;
        }
        public ActionResult Index()
        {
            ViewBag.VendorList = _INCRManagementService.GetdropdownVendors();
            ViewBag.ListInspector = _INCRManagementService.GetUser();
            ViewBag.IsWHMRB = _IUserService.CheckGroupRoleForUser(User.Identity.GetUserId(), UserGroup.WHMRB);
            return View();
        }
        public ActionResult NCRAging()
        {
            return View();
        }

        // get list ncrage where status == committed
        public ActionResult NCRCommitted([DataSourceRequest]DataSourceRequest request)
        {
            var list = _INCRManagementService.GetListNCRAge();
           // _INCRManagementService.updateAgeNCRinListNCR(list);
            return Json(list.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult NCRList([DataSourceRequest]DataSourceRequest request, string vendor)
        {
            var list = _INCRManagementService.GetListNCRByVendor(vendor);
            return Json(list.ToDataSourceResult(request));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        
        [HttpPost]
        public JsonResult ListNCR([DataSourceRequest]DataSourceRequest request)
        {
            List<NCRManagementViewModel> list = _INCRManagementService.GetListNCR();
            return new JsonResult()
            {
                Data = list.ToDataSourceResult(request),
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            // return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult IsExistReceiver(string receiver)
        {
            try
            {
                bool result = _INCRManagementService.IsExistReceiver(receiver);
                if (result)
                {
                    return Json(new { result = true, message = "" });
                }
                else
                {
                    return Json(new { result = false, message = "Receiver not Existed" });
                }
            }
            catch
            {
                return Json(new { result = false, message = "Please contact to Administrator!" });
            }
        }
        [HttpPost]
        public ActionResult IsnotExistVendor(string vendor)
        {
            try
            {
                bool result = _INCRManagementService.IsExistVendor(vendor);
                if (result)
                {
                    return Json(new { result = true, message = "" });
                }
                else
                {
                    return Json(new { result = false, message = "Vendor not Existed" });
                }
            }
            catch
            {
                return Json(new { result = false, message = "Please contact to Administrator!" });
            }
        }

        public ActionResult Print()
        {
            ViewBag.VendorList = _INCRManagementService.GetdropdownVendors();
            //List<DropdownlistViewModelPrint> ncrnum = _INCRManagementService.GetdropdownNCRnum(vendor);
            //SelectList ncrnumList = new SelectList(ncrnum, "NCR_NUM", "NCR_NUM");
            DropdownlistViewModelPrint model = new DropdownlistViewModelPrint();
            return View(model);
        }

        public ActionResult GetVendorList(string q)
        {
            var list = new List<SelectListItem>();

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list = list.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DropdownNCRNum(string vendor)
        {
            try
            {
                List<DropdownlistViewModelPrint> listncrnum = new List<DropdownlistViewModelPrint>();
                listncrnum = _INCRManagementService.GetdropdownNCRnum(vendor);
                SelectList listList = new SelectList(listncrnum, "NCR_NUM", "NCR_NUM");
                return Json(new { success = true, list = listList });
            }
            catch
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        public JsonResult DropdownNCRNumPartNum(string vendor)
        {
            try
            {
                List<DropdownlistViewModelPrint> listncrnum = new List<DropdownlistViewModelPrint>();
                listncrnum = _INCRManagementService.GetdropdownNCRnum(vendor.Trim());
                SelectList listList = new SelectList(listncrnum, "MI_PART_NO", "MI_PART_NO");
                return Json(new { success = true, list = listList });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        public ActionResult PrintMutilpleNCR(string StringListncrnum, string PrintedEvidences)
        {
            // StringListncrnum = "I000010D,I000010C";
            List<string> listncrnum = JsonConvert.DeserializeObject<List<string>>(StringListncrnum); // Parse Json StringListncrnum to List<string>
            List<PrintedEvidence> printedEvidences = JsonConvert.DeserializeObject<List<PrintedEvidence>>(PrintedEvidences);
            List<NCRManagementViewModel> listResult = new List<NCRManagementViewModel>();

            string html = "";
            string FileNameTemp = Guid.NewGuid() + "",
                    FolderTemp = Guid.NewGuid() + "",
                       RootPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"]),
                       TEMPPath = $"{RootPath}TEMP/{FolderTemp}/",
                       FilePathTemp = $"{RootPath}TEMP/{FolderTemp}/{FileNameTemp}.pdf",
                       FileMergePath = $"{RootPath}MERGE_EVIDENT/Merge_{String.Join("_", listncrnum.ToArray())}_Evident.pdf";

            if (!Directory.Exists($"{RootPath}TEMP/{FolderTemp}"))
            {
                Directory.CreateDirectory($"{RootPath}TEMP/{FolderTemp}");
            }
            if (!Directory.Exists($"{RootPath}MERGE_EVIDENT"))
            {
                Directory.CreateDirectory($"{RootPath}MERGE_EVIDENT");
            }
  
            EvoPdf.Document MergePdf = new EvoPdf.Document();
            MergePdf.LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw==";
            EvoPdf.Document MergeEvi = new EvoPdf.Document();
            MergeEvi.LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw==";

            #region Get All NCR from StringListncrnum
            foreach (var NCR_NUM in listncrnum)
            {
                //var appr_model = _INCRManagementService.GetListUserApprovalByNcrNum(NCR_NUM);
                var obj = _INCRManagementService.GetCreateNCR(NCR_NUM);
                obj.ListAdditional = _INCRManagementService.GetAdditional(NCR_NUM);
                obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM,obj.CCN);
                obj.NCRDETs = _INCRManagementService.GetInresultProcessString(NCR_NUM);
                obj.ListNC_Group = _INCRManagementService.GetListNC_GRP_DESC(obj.CCN);
                obj.ListRespon = _INCRManagementService.getListRESPON();
                obj.ListDispo = _INCRManagementService.getListDispo();

                if (obj != null)
                {
                    //ViewBag.ListAllUsers = _IUserService.GetAllUser().ToList();
                    obj.ListUSerAppr = new List<UserApproval>();

                    //obj.OldEvidence = _INCRManagementService.GetUploadedEvidence(NCR_NUM);
                    // Get Uploaded VNMaterialTraceability
                    //obj.OldVNMaterialTraceability = _INCRManagementService.GetStringUploadedVNMaterialTraceability(NCR_NUM);

                    ViewBag.Status = obj.STATUS.Trim();

                    //obj.Listdefectprocess = _INCRManagementService.GetInresultProcess(NCR_NUM);

                    obj.NCRDETs = _INCRManagementService.GetInresultProcessString(NCR_NUM);

                    //obj.NCRDISs = _INCRManagementService.GetNCRDISs(NCR_NUM);

                    //obj.DISPOSITION = _INCRManagementService.getOrderDisposition(NCR_NUM);
                    obj.ListUSerAppr = _INCRManagementService.GetApproverOfNCRForConfirm(NCR_NUM);
                    if (obj.ListUSerAppr.Count % 2 != 0)
                    {
                        int num = obj.ListUSerAppr.Count % 2;
                        for (int i = 0; i < num; i++)
                        {
                            obj.ListUSerAppr.Add(new UserApproval
                            {
                                DateAppr = "",
                                FullName = "",
                                Id = "",
                                IdUser = "",
                                IsAppr = false,
                                RoleId = "",
                                RoleName = "",
                                Signature = ""
                            });
                        }
                    }
                    obj.REV = ConfigurationManager.AppSettings["REV"];
                    obj.FORM = ConfigurationManager.AppSettings["FORM"];
                    //if (obj.STATUS.Trim() != StatusInDB.Void && obj.STATUS.Trim() != StatusInDB.DispositionApproved)
                    //{
                    //    obj.REV = ConfigurationManager.AppSettings["REV"];
                    //    obj.FORM = ConfigurationManager.AppSettings["FORM"];
                    //    obj.URL = _INCRManagementService.getUrlNcr(NCR_NUM);
                    //    PrintNCREnvident(NCR_NUM, obj);
                    //}
                }
                listResult.Add(obj);
            }
            #endregion

            ViewBag.Writeln = _IUserService.GetNameById(User.Identity.GetUserId());
            #region Create a PDF from an existing HTML using IronPDF
            html = RenderViewToString(ControllerContext, "~/views/ncr/PrintMutilpleNCR.cshtml", listResult, true);
            EvoPdf.HtmlToPdfConverter htmlToPdfConverter = new EvoPdf.HtmlToPdfConverter();
            htmlToPdfConverter.LicenseKey = "VNrK28jI28/bzNXL28jK1crJ1cLCwsLbyw==";
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = new EvoPdf.PdfPageSize();
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = EvoPdf.PdfPageOrientation.Portrait;
            htmlToPdfConverter.ConvertHtmlToFile(html,
                Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority
                , FilePathTemp);
            // Merge multiple file PDF
            
            var headerDoc = new EvoPdf.Document(FilePathTemp);
            MergePdf.AppendDocument(headerDoc);



            #region Improve SEP
            var EVIs = _INCRManagementService.GetEVIByIds(printedEvidences.Select(x => x.EVIID).ToList());
            foreach (var item in listResult)
            {
                string htmlNCR = RenderViewToString(ControllerContext, "~/views/NCRApproval/PrintNCR.cshtml", item, true);
                htmlToPdfConverter.ConvertHtmlToFile(htmlNCR,
                Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority
                , $@"{TEMPPath}{item.NCR_NUM.Trim()}_Temp.pdf");
                var pdfNCRTemp = new EvoPdf.Document($@"{TEMPPath}{item.NCR_NUM.Trim()}_Temp.pdf");
                MergePdf.AppendDocument(pdfNCRTemp);

                //Select evidence of ncr
                var LstEVI = EVIs.Where(x => x.NCR_NUM.Trim().Equals(item.NCR_NUM.Trim())).ToList();
                if (LstEVI.Count > 0)
                {
                    foreach (var EVI in LstEVI)
                    {
                        string EviPath = Server.MapPath(ConfigurationManager.AppSettings["uploadPath"] + EVI.EVI_PATH);
                        if (System.IO.File.Exists(EviPath))
                        {
                            if (UtilitiesService.HasImageExtension(EviPath))
                            {
                                EvoPdf.AddElementResult addElementResult = null;
                                // The position on X anf Y axes where to add the next element
                                float yLocation = 10;
                                float xLocation = 10;

                                // Create a PDF page in PDF document
                                Margins margins = new Margins { Top = 20, Bottom = 20, Left = 20, Right = 20 };
                                EvoPdf.PdfPage pdfPage = MergePdf.AddPage(margins);
                                // Add section title
                                EvoPdf.PdfFont titleFont = MergePdf.AddFont(new System.Drawing.Font("Times New Roman", 12, FontStyle.Bold, GraphicsUnit.Point));
                                TextElement titleTextElement = new TextElement(xLocation, yLocation, "", titleFont);
                                titleTextElement.ForeColor = Color.Black;
                                addElementResult = pdfPage.AddElement(titleTextElement);
                                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                                pdfPage = addElementResult.EndPdfPage;

                                float titlesYLocation = yLocation;
                                ImageElement unscaledImageElement = new ImageElement(5, 5, EviPath);
                                addElementResult = pdfPage.AddElement(unscaledImageElement);
                                RectangleF scaledDownImageRectangle = new RectangleF(addElementResult.EndPageBounds.Right + 30, addElementResult.EndPageBounds.Y,
                                addElementResult.EndPageBounds.Width, addElementResult.EndPageBounds.Height);
                            }
                            else
                            {
                                var docEVI = new EvoPdf.Document(EviPath);
                                MergePdf.AppendDocument(docEVI);
                            }
                        }
                    }
                }
            }
            #endregion


            #endregion
            // Merge pdf
            MergePdf.Save(FileMergePath);
            headerDoc.Close();
            //System.IO.Directory.Delete($"{RootPath}TEMP/{FolderTemp}", true);
            return File(FileMergePath, "application/pdf");
            //return View(listResult);
        }

        static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial) viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null) throw new System.IO.FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new System.IO.StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public ActionResult GetListEvidenceByNCR(string StringListncrnum)
        {
            List<string> listncrnum = JsonConvert.DeserializeObject<List<string>>(StringListncrnum); // Parse Json StringListncrnum to List<string>
            var LstEVI = _INCRManagementService.GetEVIByNCRNUM(listncrnum);
            var data = new List<PrintedEvidence>();
            if (LstEVI.Count > 0)
            {
                foreach (var EVI in LstEVI)
                {
                    data.Add(new PrintedEvidence {
                        EVIID = EVI.EVI_ID.ToString(),
                        FileName = EVI.EVI_PATH.Split('/')[1],
                        NCRNUM = EVI.NCR_NUM.Trim()
                    });
                }
            }
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }
        //tuanlua add.16/01 
        public ActionResult Waitingdisposition()
        {
            return View();
        }
            public ActionResult GetListWaitingDisposition([DataSourceRequest]DataSourceRequest request)
        {
            var id = User.Identity.GetUserId();
            var listdispo = _INCRManagementService.GetListNCRWaitingDisposition(id);
            return Json(listdispo.ToDataSourceResult(request));
        }
        //tuan lua 08/03
        public ActionResult NCRSCRAP()
        {
            return View();
        }
        public ActionResult NCRListSCRAP([DataSourceRequest]DataSourceRequest request)
        {
            var list = _INCRManagementService.GetListNCRSCRAP();
            return Json(list.ToDataSourceResult(request));
        }
        //tuan lua add 25-03-2019
        public ActionResult NCRSubmit()
        {
            return View();
        }
        public ActionResult NCRListsubmit([DataSourceRequest]DataSourceRequest request)
    {
            string id = User.Identity.GetUserId();
            var list = _INCRManagementService.GetListNCRSubmitbyUser(id);
            return Json(list.ToDataSourceResult(request));
        }
        //tuan lua add 01/04/2019

        public ActionResult NCRWaitingYourApproval()
        {
            return View();
        }
        public ActionResult getListNCRWaitingYourApproval([DataSourceRequest]DataSourceRequest request)
        {
            var list = _INCRManagementService.GetListNCRwaitingyourapproval();
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult ListUser()
        {
            return View();
        }

        public ActionResult getlistUser([DataSourceRequest]DataSourceRequest request)
        {
            var list = _IUserService.getlistUser();
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult ListDispositionpartial()
        {
            return View();
        }

        public ActionResult getListDispositionpartial([DataSourceRequest]DataSourceRequest request)
        {
            string id = User.Identity.GetUserId();
            var list = _INCRManagementService.getListDispositionpartial(id);
            return Json(list.ToDataSourceResult(request));
        }
        public JsonResult CheckHistory(string NCRNUM)
        {
            var chek = _INCRManagementService.CheckhistoryNCR(NCRNUM);   
            if (chek == true)
            {
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }
    }
}
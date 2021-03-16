using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using II_VI_Incorporated_SCM.Extensions;
using II_VI_Incorporated_SCM.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace II_VI_Incorporated_SCM.Services
{
    public class UtilitiesService
    {
        public static string[] RemoveDuplicates(string[] s)
        {
            HashSet<string> set = new HashSet<string>(s);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }

        public static bool HasImageExtension(string source)
        {
            List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
            //return (source.EndsWith(".png") || source.EndsWith(".jpg"));
            return ImageExtensions.Contains(Path.GetExtension(source).ToUpperInvariant());
        }

        public static long[] Add(long[] array, long newValue)
        {
            int newLength = array.Length + 1;

            long[] result = new long[newLength];

            for (int i = 0; i < array.Length; i++)
                result[i] = array[i];

            result[newLength - 1] = newValue;

            return result;
        }

        public static long[] RemoveAt(long[] array, int index)
        {
            int newLength = array.Length - 1;

            if (newLength < 1)
            {
                return array;//probably want to do some better logic for removing the last element
            }

            //this would also be a good time to check for "index out of bounds" and throw an exception or handle some other way

            long[] result = new long[newLength];
            int newCounter = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (i == index)//it is assumed at this point i will match index once only
                {
                    continue;
                }
                result[newCounter] = array[i];
                newCounter++;
            }

            return result;
        }
    }
    public class CApproval
    {
        public bool QUALITY { get; set; }
        public bool ENGIEERING { get; set; }
        public bool MFG { get; set; }
        public bool PURCHASING { get; set; }
        public string QUALITYDATE { get; set; }
        public string ENGIEERINGDATE { get; set; }
        public string MFGDATE { get; set; }
        public string PURCHASINGDATE { get; set; }
        public bool IsAllApproval { get; set; }
        public bool IsApproval { get; set; }

        public void SetCheck(bool check)
        {
            QUALITY = ENGIEERING = MFG = PURCHASING = IsAllApproval = IsApproval = check;
        }
        public void CheckedAll()
        {
            QUALITY = ENGIEERING = MFG = PURCHASING = IsAllApproval= IsApproval= true;
        }

       
        internal void CheckedAll(NCR_DET nCR_DET, APPROVAL aPPROVAL)
        {
            //QUALITY = ENGIEERING = MFG = PURCHASING = IsAllApproval = true;
            QUALITY = nCR_DET.QUALITY != null ;
            ENGIEERING = nCR_DET.ENGIEERING != null;
            MFG = aPPROVAL.MFG != null ?  nCR_DET.MFG != null : true;
            PURCHASING = aPPROVAL.PURCHASING != null ? nCR_DET.PURCHASING != null : true;

            IsAllApproval = QUALITY & ENGIEERING & MFG & PURCHASING;

            QUALITYDATE = nCR_DET.QUALITY != null ? nCR_DET.QUALITY.Value.GetDateTimeFormat() : "";
            ENGIEERINGDATE = nCR_DET.ENGIEERING != null ? nCR_DET.ENGIEERING.Value.GetDateTimeFormat() : "";
            MFGDATE = nCR_DET.MFG != null ? nCR_DET.MFG.Value.GetDateTimeFormat() : "";
            PURCHASINGDATE = nCR_DET.PURCHASING != null ? nCR_DET.PURCHASING.Value.GetDateTimeFormat() : "";
        }
        internal void SetDate(NCR_DIS nCR_DIS)
        {
            QUALITYDATE = nCR_DIS.QUALITY != null ? nCR_DIS.QUALITY.Value.GetDateTimeFormat() : "";
            ENGIEERINGDATE = nCR_DIS.ENGIEERING != null ? nCR_DIS.ENGIEERING.Value.GetDateTimeFormat() : "";
            MFGDATE = nCR_DIS.MFG != null ? nCR_DIS.MFG.Value.GetDateTimeFormat() : "";
            PURCHASINGDATE = nCR_DIS.PURCHASING != null ? nCR_DIS.PURCHASING.Value.GetDateTimeFormat() : "";
        }
    }
    public static class UserGroup
    {
        public static string MRBTeam = "MRB";
        public static string OPE = "OPE";
        public static string WHMRB = "WHMRB";
        public static string SQE = "SQE";
        public static string Buyer = "Buyer";
        public static string PURCHASING = "PURCHASING";
        public static string CHAIRMAN = "MRB Chairman";
        public static string ENGINEERING = "ENGINEERING";
        public static string QUALITYASSURANCE = "QUALITY ASSURANCE";
        public static string VNOwner = "VN Owner";
        public static string Initiator = "Initiator";
        public static string PE = "PE";
        public static string Planner = "Planner";
    }

    public static class Status
    {
        //public static string Created = "Created";
        //public static string Submitted = "Submitted";
        //public static string WaitingWH = "WH MRB confirmed Received NC Parts";
        //public static string WaitingDisposition = "Waiting for Disposition Approval";
        //public static string DispositionApproved = "Disposition Approved";
        //public static string Close = "Close";
        public static string Void = "Voided";

    }
    public static class StatusInDB
    {
        public static string Created = "a";
        public static string Submitted = "b";
        public static string WaitingForDisposition = "c";
        public static string WaitingForDispositionApproval = "d";
        public static string DispositionApproved = "e";
        public static string Close = "f";
        public static string Void = "g";
        public static string Complete = "Complete";
    }

    public static class StatusRTV
    {
        public static string New = "New";
        public static string Process = "Process";
        public static string Close = "Close";
    }

    public static class NCRType
    {
        public static string IQC = "IQC";
        public static string PROCESS = "PROCESS";
    }
    public static class CRTYPE
    {
        public static string NCR = "NCR";
        public static string SCAR = "SCAR";
    }
    public static class AQLText
    {
        public static string AQL = "0.65";
        public static string LEVEL = "C=0";
        public static string AQLinDB = "C=0";
        public static string AQL100 = "100%";
    }

    public static class CONFIRMITY_RESPON
    {
        //public static string VENDOR = "VENDOR";
        //public static string PURCHASING = "PURCHASING";
        //public static string PRODUCTION = "PRODUCTION";
        //public static string TEST = "TEST";
        //public static string ENGINEERING = "ENGINEERING";
        //public static string MARKETING = "MARKETING";
        //public static string SHIPPING = "SHIPPING";
        //public static string DESCRIBE = "DESCRIBE";

        public static string ID_VENDOR = "A";
        public static string ID_PURCHASING = "B";
        public static string ID_PRODUCTION = "C";
        public static string ID_TEST = "D";
        public static string ID_ENGINEERING = "E";
        public static string ID_MARKETING = "F";
        public static string ID_SHIPPING = "G";
        public static string ID_DESCRIBE = "H";
    }

    public static class CONFIRMITY_DISPN
    {
        //public static string VENEXP = "RETURN TO VENDOR(VEN. EXP)";
        //public static string VENMI = "RETURN TO VENDOR (MI EXP)";
        //public static string REWORK = "REWORK";
        //public static string SCRAP = "SCRAP";
        //public static string USEASIS = "USE AS IS";
        //public static string SALVAGE = "SALVAGE";
        //public static string DESCRIBE = "DESCRIBE";

        public static string ID_VENEXP = "A";
        public static string ID_VENREP = "A1";
        public static string ID_VENCRE = "A2";
        public static string ID_VENMI = "B";
        public static string ID_REWORK = "C";
        public static string ID_SCRAP = "D";
        public static string ID_USEASIS = "E";
        public static string ID_SALVAGE = "F";
        public static string ID_DESCRIBE = "G";
        public static string ID_TBD = "H";
    }
    public static class CONFIRMITY_DISPNP
    {
        //public static string VENEXP = "RETURN TO VENDOR (VEN, EXP)";
        //public static string VENMI = "RETURN TO VENDOR (VEN, MI)";
        //public static string REWORK = "REWORK";
        //public static string SCRAP = "SCRAP";
        //public static string USEASIS = "USE AS IS";
        //public static string SALVAGE = "SALVAGE";
        //public static string DESCRIBE = "DESCRIBE";

        public static string ID_VENEXP = "A";
        public static string ID_VENMI = "B";
        public static string ID_REWORK = "C";
        public static string ID_SCRAP = "D";
        public static string ID_USEASIS = "E";
        public static string ID_SALVAGE = "F";
        public static string ID_DESCRIBE = "G";
    }
    public static class REPORT
    {
        public static string Average = "Average";
        public static string Target = "Target";
        public static string Thresh = "Thresh old";
    }

    public static class ADDINS
    {
        public static string ID_VENEXP = "A";
        public static string ID_VENMI = "B";
        public static string ID_REWORK = "C";
        public static string ID_SCRAP = "D";
        public static string ID_ACCEPTED = "E";
        public static string ID_SALVAGE = "F";
        public static string ID_SORT100 = "G";

        public static string ID_OTHER = "H";
        public static string ID_USEASIS = "I";
    }


    public static class StaticText
    {
        public static string INS_PLAN = "Inspector Plan";
        public static string IIVIVietnam = "26";
        //public static string ADDRESS = " 36 VSIP đường số 4, KCN Việt Nam Singapore I, TA,BD";
        //public static string STATE = "26";
        //public static string ZIP = "26000";
        //public static string CTRY = "Singapore";
        //public static string NAME = "Công ty TNHH II-VI VIỆT NAM";
    }

    public static class Print
    {
        public static int count = 9;
    }
    public static class PrintCCN
    {
        public static int count = 7;
    }
    public static class TypeRTV
    {
        public static string RTVREP = "RTVREP";
        public static string SCRREP = "SCRREP";
        public static string RTVSCN = "RTVSCN";
        public static string SCRCRN = "SCRCRN";
        public static string SHIPPED = "SHIPPED";
    }

    public static class FieldRespone
    {
        public static string D0 = "D0 - PLAN";
        public static string D1 = "D1 - USE TEAM APPROACH";
        public static string D2 = "D2 - DESCRIBE THE PROBLEM";
        public static string D3 = "D3 - IMPLEMENT CONTAINMENT PLANS (SHORT TERM) - DESCRIBE";
        public static string D4 = "D4 - DESCRIBE THE ROOT CAUSE";
        public static string D5 = "D5 - IMPLEMENT PERMANENT CORRECTIVE ACTIONS - DESCRIBE";
        public static string D6 = "D6 - VALIDATION OF CORRECTIVE ACTION (Please attach copies of audits, meeting ";
        public static string D7 = "D7 - VERIFY EFFECTIVENESS - DESCRIBE";
        //public static string D8 = "D8 - Verification";
        public static string D8 = "D8 - FOLLOW UP BY II-VI MARLOW";
        public static string End = "Supplier Representative:";
    }

    public static class TaskManagement
    {
        public static string NCR = "NCR";
        public static string SCAR = "SCAR";
        public static string Supplier = "Supplier";
    }

    public static class TaskManagementStatus
    {
        public static string New = "Created";
        public static string Approve = "Approve";
        //public static string InComplete = "InComplete";
        public static string Complete = "Completed";
        public static string Reject = "Reject";
        public static string Reopen = "Reopen";
        public static string Cancel = "Cancel";
        public static string Hold = "Hold";
        //public static string Delete = "Delete";
    }

    public static class TaskManagementPriority
    {
        public static string Low = "Low";
        public static string Normal = "Normal";
        public static string High = "High";
        public static string Urgent = "Urgent";
        public static string Immediate = "Immediate";
    }

    public static class TaskManagementType
    {
        public static string Document = "Document";
    }

    public static class SCARStatus
    {
        public static string Created = "Created";
        public static string Sent = "Sent";
        public static string Accepted = "Accepted";
        public static string AcceptedRemind = "Accepted(Remind)";
        public static string SupplierCompleted = "Supplier Completed";
        public static string CompletedAll = "Completed All";
        public static string Voided = "Voided";
        public static string Closed = "Closed";
    }

    //public static class SCARStatus
    //{
    //    public static string Created = "Created";
    //    public static string Sent = "Sent to Supplier";
    //    public static string Accepted = "Accepted by Supplier";
    //    public static string AcceptedRemind = "Verification";
    //    public static string SupplierCompleted = "Verification";
    //    public static string CompletedAll = "Verification";
    //    public static string Voided = "Voided";
    //    public static string Closed = "Closed";
    //}

    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            //m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                string FolderPath = System.Web.HttpContext.Current.Server.MapPath("~\\II_VI_Log");
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);
                using (StreamWriter w = File.AppendText(FolderPath + "\\log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception Ex)
            {
                LogWrite("Error Log: " + Environment.NewLine +  Ex.ToString());
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception)
            {
            }
            finally
            {
                txtWriter.Close();
                txtWriter.Dispose();
            }
        }
    }
    public class ExcelSupport
    {
        public static void UpdateCell(string docName, string text,
            int rowIndex, string columnName, string sheetName = "SCAR", CellValues cellValues = CellValues.Number)
        {
            // Open the document for editing.
            SpreadsheetDocument spreadSheet = null;
            try
            {
                spreadSheet = SpreadsheetDocument.Open(docName, true);
                //spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
                WorksheetPart worksheetPart =
                      GetWorksheetPartByName(spreadSheet, sheetName);

                if (worksheetPart != null)
                {
                    Cell cell = GetCell(worksheetPart.Worksheet,
                                             columnName, rowIndex);

                    cell.CellValue = new CellValue(text);
                    cell.DataType = cellValues;

                    worksheetPart.Worksheet.Save();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                spreadSheet?.Dispose();
            }
        }

        public static void UpdateStringCell(string docName, string text,
            int rowIndex, string columnName, string sheetName = "SCAR")
        {
            // Open the document for editing.
            SpreadsheetDocument spreadSheet = null;
            try
            {
                spreadSheet = SpreadsheetDocument.Open(docName, true);
                //spreadSheet.WorkbookPart.DeletePart(spreadSheet.WorkbookPart.VbaProjectPart);
                WorksheetPart worksheetPart =
                      GetWorksheetPartByName(spreadSheet, sheetName);

                if (worksheetPart != null)
                {
                    Cell cell = GetCell(worksheetPart.Worksheet,
                                             columnName, rowIndex);

                    cell.CellValue = new CellValue(text);
                    cell.DataType = CellValues.String;

                    //Save the worksheet.
                   worksheetPart.Worksheet.Save();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                spreadSheet?.Dispose();
            }
        }

        public static WorksheetPart
             GetWorksheetPartByName(SpreadsheetDocument document,
             string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                // The specified worksheet does not exist.

                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        public static Cell GetCell(Worksheet worksheet,
                  string columnName, int rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
                return null;

            return row.Elements<Cell>().Where(c => string.Compare
                   (c.CellReference.Value, columnName +
                   rowIndex, true) == 0).First();
        }


        // Given a worksheet and a row index, return the row.
        public static Row GetRow(Worksheet worksheet, int rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }

        public static string GetCellReference(WorkbookPart workbookPart, string textToFind)
        {
            //get the correct sheet
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == "SCAR").First();
            if (sheet != null)
            {
                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

                SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                for (int i = 25; i <= 150; i++)
                {
                    Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == i).First();

                    if (row != null)
                    {
                        foreach (Cell c in row.Elements<Cell>())
                        {
                            string cellText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    int index = int.Parse(c.CellValue.InnerText);
                                    cellText = stringTable.SharedStringTable.ElementAt(index).InnerText;
                                }
                                else
                                {
                                    cellText = c.CellValue.InnerText;
                                }
                                if (cellText == textToFind)
                                {
                                    return c.CellReference;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string GetValueCell(WorkbookPart workbookPart, Cell c)
        {
            string cellText = "";
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == "SCAR").First();
            if (sheet != null)
            {
                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;
                SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (c.DataType != null)
                {
                    if (c.DataType == CellValues.SharedString)
                    {
                        int index = int.Parse(c.CellValue.InnerText);
                        cellText = stringTable.SharedStringTable.ElementAt(index).InnerText;
                    }
                    else
                    {
                        cellText = c.CellValue.InnerText;
                    }
                }
            }
            return cellText;
        }

        public static string GetPicWorkSheet(WorkbookPart wbPart, string sheetName, string path, string serverPath)
        {
            string pdfPath = "";
            try
            {
                Sheet sheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).First();
                WorksheetPart worksheetPart = wbPart.GetPartById(sheet.Id) as WorksheetPart;
                var workSheet = wbPart.WorksheetParts.FirstOrDefault();
                List<string> imageList = new List<string>();
                int dem = 1;
                foreach (ImagePart i in worksheetPart.DrawingsPart.GetPartsOfType<ImagePart>())
                {
                    ImagePart imagePart = i;
                    string imageFileName = string.Empty;
                    using (System.Drawing.Image toSaveImage = Bitmap.FromStream(imagePart.GetStream()))
                    {
                        imageFileName = path + sheetName + "_" + dem + ".png";
                        try
                        {
                            toSaveImage.Save(imageFileName, ImageFormat.Png);
                            imageList.Add(imageFileName);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            toSaveImage.Dispose();
                        }
                    }
                    dem++;
                }
                pdfPath = InsertImageIntoPDF(path, sheetName, serverPath, imageList);
                return pdfPath;
            }
            catch(Exception e)
            {
                return "";
            }
        }

        public static string InsertImageIntoPDF(string path, string sheetName, string serverPath, List<string> imageList)
        {
            string result = "";
            Document doc = new Document(PageSize.A3, 10f, 10f, 100f, 0f);
            string pdfFilePath = path;
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + sheetName + ".pdf", FileMode.Create));
            doc.Open();
            try
            {
                Paragraph paragraph = new Paragraph();
                foreach(var item in imageList)
                {
                    string imageURL = item;
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                    var height = jpg.Height;
                    var width = jpg.Width;
                    jpg.Alignment = Element.ALIGN_LEFT;
                    jpg.ScaleToFit(800, 800 * height / width);
                    doc.Add(paragraph);
                    doc.Add(jpg);
                }
                
                result = serverPath + sheetName + ".pdf";
            }
            catch (Exception)
            { }
            finally
            {
                doc.Close();
            }

            return result;
        }

        public Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
            };
        }

        //public enum DataType
        //{
        //    YN,
        //    Date,
        //    Text,
        //    DrownDown
        //}

        public static string generateHtml(string type, string name, string id = "", object value = null, string list = null,bool isrequired = false)
        {
            string template = "";
            string val = "";
            List<string> lst = new List<string>();
            id = !string.IsNullOrEmpty(id) ? id : name;
            switch (type)
            {
                case "Date":
                    val = value != null ? value.ToString() : "";
                    template = $"<input c-required='{isrequired}' type='text' name='{name}' id='{id}' value='{val}'  class='form-control date' />"; break;
                case "DrownDown":
                    string option = "";
                    val = value != null ? value.ToString() : "";
                    if (list != null)
                    {
                        lst = list.Split('/').ToList();
                        foreach (var item in lst)
                        {

                            var selected = val.ToString().ToUpper() == item.ToUpper() ? "selected" : "";
                            option += $" <option value='{item}' {selected}> {item} </option> ";
                        }
                    }
                    template = $@" <select class='form-control' c-required='{isrequired}'>  <option value=''> Please select</option> {option} </select> ";
                    break;
                case "Text":
                    val = value != null ? value.ToString() : "";
                    template = $"<input type='text' name='{name}' id='{id}' value='{val}' class='form-control' c-required='{isrequired}' />"; break;
                case "YN":
                    //thi.nguyen:11-Jan-2021
                    val= value != null ? value.ToString() : "";
                    val = val == "on" || val == "true" ? "checked" : "";
                    //val = value != null ? ( bool.Parse(value.ToString()) == true ?  "checked" : "") : "";
                    template = $"<input c-required='{isrequired}' type='checkbox' name='{name}' id='{id}' class='form-control' {val}  onchange='$(this).prop(\"checked\") ? $(this).val(\"true\") : $(this).val(\"false\")' />"; break;
            }

            return template;
        }
    }

}
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.NCR;
using II_VI_Incorporated_SCM.Models.NCRReport;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Services
{

    public interface IReportNcrService
    {
        List<NCRDispositionDayViewModel> GetListNCRDispositionDay(string selectedyear);
        List<SelectListItem> GetdropdownYear();
        List<SelectListItem> GetdropdownVendors4panel();
        List<SelectListItem> GetdropdownYear(string y);
        IEnumerable<LotUseAsIsViewModel> GetListUseAsIs();
        List<SelectListItem> GetdropdownVendorsToStrategy();
        List<VENDOR> StrategyList(string y, string ccn);
        List<EEPreportviewModel> GetdataEEPtoproduction(string yearselect);
        List<EEPreportviewModel> GetdataEEPtocomponent(string yearselect);
        List<EEPreportviewModel> GetdataEEPtoSystem(string yearselect);
        List<SelectListItem> GetdropdownCCN();
        List<SelectListItem> GetdropdownCCN(string CCN);
        List<SupplierforPPMViewModel> GetdataSupplierPPM(int yearselect, string CCN);
        List<PoretoViewModel> GetListPorentobyPartNum(DateTime datefrom, DateTime dateto, string partnum);
        List<NCR_DETViewModel> GetListPorentoRawdata(DateTime datefrom, DateTime dateto, string partnum);
        List<SelectListItem> GetdropdownVendors();
        List<SelectListItem> GetdropdownVendorsSelected(string y, string ccn);
        List<PoretoViewModel> GetListPorentobyVendor(DateTime datefrom, DateTime dateto, string vendor);
        List<NCR_DETViewModel> GetListPorentoRawdatabyvendor(DateTime datefrom, DateTime dateto, string vendor);
        List<StrateryViewModel> GetDataReadSupplier(int year, string ccn);
        List<StrateryViewModel> GetDataReadSupplierNone(int year, string ccn);
        List<OneSupplierforPPM> GetDataReadSupplierOne(int FY, string id, int month);
        List<topmonthViewmodel> getLstTopQTybyYear(int year, int qty, string id);
        List<topmonthViewmodel> getLstTopQTybmonth(DateTime month, int qty, string id);
        List<PANEL_RP> getDatarawOTD(DateTime month, string vendor);
        List<Scarviewmodel> getLstScarProblem(DateTime year, string vendor);
        List<ImprovestrackingViewmodel> getlistimprovestracking(int year, int month, string vendor);
        List<DataRawViewmodel> GetDataRawRejQty4panelPPM(int year, int month, string id);
        bool checkidtrung4panel(List<PANEL_EXCEL> model);
        bool SaveData(List<ESCAPING_PPM> model);
        bool checkidtrung(List<ESCAPING_PPM> model);
        bool UpdateEscappingPPM(List<ESCAPING_PPM> lstUpdate);
        void UpdateSupplierPPM(SUPPLIER_PPM_RP ncrday);
        bool InsertAndDelete(string CCN, int FY, List<SUPPLIER_PPM> entity);
        void UpdateEEP(EEP_REPORT ncrday);
        void Update(NCRDispositionDay ncrday);
        void UpdateSupplierPPMforOne(ONESUPPLLIER_PPMRB ncrday);
        bool Update4Panel(List<PANEL_EXCEL> lstUpdate);
        bool SaveDataTo4Panel(List<PANEL_EXCEL> model);
        void SaveDataRAWORUPDATE(PANEL_RP ncrday);
        List<topmonthViewmodel> getLstTopQTybmonthqts(DateTime month, int qty, string id);
        //Tuan
        List<string> GetPartbyDate(string dateSta, string dateDue);
        List<string> Getsupplierbytype(string type, string dateSta, string dateDue);
        List<sp_Report_PPMByPart_Result> GetdataSupplierPPMTest(string part, string CCN, DateTime dateSta, DateTime dateDue);
    }
    public class ReportNcrService : IReportNcrService
    {
        private IIVILocalDB _db;
        private static readonly bool UpdateDatabase = false;

        public ReportNcrService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        private List<double> GetFYselectedyear(string year)
        {
            List<double> ListYear = new List<double>
            {
                GetFYselectedItem(year, REPORT.Average),
                GetFYselectedItem(year, REPORT.Target),
                GetFYselectedItem(year, REPORT.Thresh)
            };//list year1
            return ListYear.ToList();
        }
        private double GetFYselectedItem(string year, string type)
        {
            double sumyear = 0;
            NCRDispositionDayViewModel selectaver = _db.NCRDispositionDays.Where(n => n.FY == year && n.TYPE == type).
               Select(n => new NCRDispositionDayViewModel
               {
                   FY = n.FY,
                   TYPE = n.TYPE,
                   JUL = n.JUL,
                   AUG = n.AUG,
                   SEP = n.SEP,
                   OCT = n.OCT,
                   NOV = n.NOV,
                   DEC = n.DEC,
                   JAN = n.JAN,
                   FEB = n.FEB,
                   MAR = n.MAR,
                   APR = n.APR,
                   MAY = n.MAY,
                   JUN = n.JUN
               }).FirstOrDefault();
            if (selectaver != null)
            {
                //tinh tong cac item theo type
                sumyear = Math.Round(((selectaver.JUL + selectaver.AUG + selectaver.SEP + selectaver.OCT + selectaver.NOV + selectaver.DEC + selectaver.JAN + selectaver.FEB + selectaver.MAR + selectaver.APR + selectaver.MAY + selectaver.JUN) / 12), 1);
            }
            return sumyear;
        }
        private double Summonth(int month, int year)
        {
            TimeSpan ts = new TimeSpan();
            var listncr = _db.NCR_HDR.Where(n => n.DATESUBMIT.HasValue && n.DATEDISPO.HasValue && n.DATEDISPO.Value.Month == month && n.DATEDISPO.Value.Year == year).Select(n => new { Diposition = n.DATEDISPO.Value, SubmitDate = n.DATESUBMIT.Value }).ToList();
            double sum = 0;
            if (listncr.Count() > 0)
            {
                for (int i = 0; i < listncr.Count(); i++)
                {
                    ts = listncr[i].Diposition - listncr[i].SubmitDate;
                    sum += (int)ts.TotalDays;
                }
                return Math.Round((sum / listncr.Count()), 1);
            }
            return sum;
        }

        public List<NCRDispositionDayViewModel> GetListNCRDispositionDay(string selectedyear)
        {


            // lấy list dữ liệu từ tháng 6 của năm tryền vào đén t6 năm kế tiếp        
            List<NCRDispositionDayViewModel> ListNCRDispositionDay = _db.NCRDispositionDays.Where(n => n.FY == selectedyear).
                                              Select(n => new NCRDispositionDayViewModel
                                              {
                                                  FY = n.FY,
                                                  TYPE = n.TYPE,
                                                  FY1 = 0,
                                                  FY2 = 0,
                                                  FY3 = 0,
                                                  FYCurrent = 0,
                                                  JUL = n.JUL,
                                                  AUG = n.AUG,
                                                  SEP = n.SEP,
                                                  OCT = n.OCT,
                                                  NOV = n.NOV,
                                                  DEC = n.DEC,
                                                  JAN = n.JAN,
                                                  FEB = n.FEB,
                                                  MAR = n.MAR,
                                                  APR = n.APR,
                                                  MAY = n.MAY,
                                                  JUN = n.JUN
                                              }).ToList();
            if (ListNCRDispositionDay.Count < 1)
            {

                NCRDispositionDayViewModel listdate1 = new NCRDispositionDayViewModel
                {
                    FY = selectedyear,
                    TYPE = "Average",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                ListNCRDispositionDay.Add(listdate1);


                NCRDispositionDayViewModel listdate2 = new NCRDispositionDayViewModel
                {
                    FY = selectedyear,
                    TYPE = "Target",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                ListNCRDispositionDay.Add(listdate2);

                NCRDispositionDayViewModel listdate3 = new NCRDispositionDayViewModel
                {
                    FY = selectedyear,
                    TYPE = "Thresh old",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                ListNCRDispositionDay.Add(listdate3);
            }

            int year = (int.Parse(selectedyear));
            DateTime firstDayOfMonth = new DateTime((year - 1), (7), 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(12).AddDays(-1);

            if ((firstDayOfMonth <= DateTime.Now) && (DateTime.Now <= lastDayOfMonth))
            {

                double Jul = Summonth(07, year - 1);
                if (Jul > 0d)
                {
                    ListNCRDispositionDay[0].JUL = Jul;
                }
                double Aug = Summonth(08, year - 1);
                if (Aug > 0d)
                {
                    ListNCRDispositionDay[0].AUG = Aug;
                }
                double sep = Summonth(09, year - 1);
                if (sep > 0d)
                {
                    ListNCRDispositionDay[0].SEP = sep;
                }
                double oct = Summonth(10, year - 1);
                if (oct > 0d)
                {
                    ListNCRDispositionDay[0].OCT = oct;
                }
                double nov = Summonth(11, year - 1);
                if (nov > 0d)
                {
                    ListNCRDispositionDay[0].NOV = nov;
                }
                double dec = Summonth(12, year - 1);
                if (dec > 0d)
                {
                    ListNCRDispositionDay[0].DEC = dec;
                }
                double jan = Summonth(01, year);
                if (jan > 0d)
                {
                    ListNCRDispositionDay[0].JAN = jan;
                }
                double feb = Summonth(02, year);
                if (feb > 0d)
                {
                    ListNCRDispositionDay[0].FEB = feb;
                }
                double mar = Summonth(03, year);
                if (mar > 0d)
                {
                    ListNCRDispositionDay[0].MAR = mar;
                }
                double apr = Summonth(04, year);
                if (apr > 0d)
                {
                    ListNCRDispositionDay[0].APR = apr;
                }
                double may = Summonth(05, year);
                if (may > 0d)
                {
                    ListNCRDispositionDay[0].MAY = may;
                }
                double jun = Summonth(06, year);
                if (jun > 0d)
                {
                    ListNCRDispositionDay[0].JUN = jun;
                }
            }
            // if (ListNCRDispositionDay.Count > 1)
            // {

            List<double> GetFYselectedyear1 = GetFYselectedyear((year - 1).ToString());
            // 1get Avg GetFYselectedyear1
            if (GetFYselectedyear1[0] > 0 && GetFYselectedyear1[1] > 0 && GetFYselectedyear1[2] > 0)
            {
                ListNCRDispositionDay[0].FY1 = GetFYselectedyear1[0];
                ListNCRDispositionDay[1].FY1 = GetFYselectedyear1[1];
                ListNCRDispositionDay[2].FY1 = GetFYselectedyear1[2];
            }
            //2 nam
            List<double> GetFYselectedyear2 = GetFYselectedyear((year - 2).ToString());
            if (GetFYselectedyear2[0] > 0 && GetFYselectedyear2[1] > 0 && GetFYselectedyear2[2] > 0)
            {
                ListNCRDispositionDay[0].FY2 = GetFYselectedyear2[0];
                ListNCRDispositionDay[1].FY2 = GetFYselectedyear2[1];
                ListNCRDispositionDay[2].FY2 = GetFYselectedyear2[2];
            }
            //year cach 3 nam
            List<double> GetFYselectedyear3 = GetFYselectedyear((year - 3).ToString());
            if (GetFYselectedyear3[0] > 0 && GetFYselectedyear3[1] > 0 && GetFYselectedyear3[2] > 0)
            {
                ListNCRDispositionDay[0].FY3 = GetFYselectedyear3[0];
                ListNCRDispositionDay[1].FY3 = GetFYselectedyear3[1];
                ListNCRDispositionDay[2].FY3 = GetFYselectedyear3[2];
            }
            //get FY now
            List<double> GetFYselectedyearfinal = GetFYselectedyear(year.ToString());
            if (GetFYselectedyearfinal[0] > 0 && GetFYselectedyearfinal[1] > 0 && GetFYselectedyearfinal[2] > 0)
            {
                ListNCRDispositionDay[0].FYCurrent = GetFYselectedyearfinal[0];
                ListNCRDispositionDay[1].FYCurrent = GetFYselectedyearfinal[1];
                ListNCRDispositionDay[2].FYCurrent = GetFYselectedyearfinal[2];
            }
            //  }
            return ListNCRDispositionDay;
        }
        public List<SelectListItem> GetdropdownYear()
        {
            //get list year string
            List<SelectListItem> dateTimeList = new List<SelectListItem>();
            int year = DateTime.Now.Year + 1;
            if (DateTime.Now.Subtract(DateTime.ParseExact("01/07/" + DateTime.Now.Year, "dd/MM/yyyy", CultureInfo.CurrentCulture)).Minutes >= 0)
            {
                year = DateTime.Now.Year + 2;
            }

            for (int i = 0; i < 10; i++)
            {
                year--;
                dateTimeList.Add(new SelectListItem
                {
                    Value = year + "",
                    Text = year + "",
                    Selected = +i == 0
                });
            }
            return dateTimeList.ToList();
        }
        public List<SelectListItem> GetdropdownYear(string y)
        {
            //get list year string
            List<SelectListItem> dateTimeList = new List<SelectListItem>();
            int year = DateTime.Now.Year + 1;
            if (DateTime.Now.Subtract(DateTime.ParseExact("01/07/" + DateTime.Now.Year, "dd/MM/yyyy", CultureInfo.CurrentCulture)).Minutes >= 0)
            {
                year = DateTime.Now.Year + 2;
            }

            for (int i = 0; i < 10; i++)
            {
                year--;
                dateTimeList.Add(new SelectListItem
                {
                    Value = year + "",
                    Text = year + "",
                    Selected = year == int.Parse(y)
                });
            }
            return dateTimeList.ToList();
        }
        public void Update(NCRDispositionDay ncrday)
        {

            if (!UpdateDatabase)
            {

                NCRDispositionDay data = _db.NCRDispositionDays.Where(n => n.FY == ncrday.FY && n.TYPE == ncrday.TYPE).FirstOrDefault();
                if (data != null)
                {
                    data.FY = ncrday.FY;
                    data.TYPE = ncrday.TYPE;
                    data.JUL = ncrday.JUL;
                    data.AUG = ncrday.AUG;
                    data.SEP = ncrday.SEP;
                    data.OCT = ncrday.OCT;
                    data.NOV = ncrday.NOV;
                    data.DEC = ncrday.DEC;
                    data.JAN = ncrday.JAN;
                    data.FEB = ncrday.FEB;
                    data.MAR = ncrday.MAR;
                    data.APR = ncrday.APR;
                    data.MAY = ncrday.MAY;
                    data.JUN = ncrday.JUN;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    _db.NCRDispositionDays.Add(ncrday);
                    _db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// GetListUseAsIs
        /// By: SIL
        /// Date: 01/07/2018
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public IEnumerable<LotUseAsIsViewModel> GetListUseAsIs()
        {
            List<LotUseAsIsViewModel> lsUseAsIs = new List<LotUseAsIsViewModel>();
            //Get LotUseAsIs NCR_DET
            List<NCR_DET> listdet = _db.NCR_DET.ToList();
            foreach (NCR_DET item in listdet)
            {
                if (item.DISPOSITION != null && item.DISPOSITION.Trim().ToUpper() == CONFIRMITY_DISPN.ID_USEASIS)
                {
                    LotUseAsIsViewModel model = new LotUseAsIsViewModel
                    {
                        RECEIVER = GetReceiverByNCRNUM(item.NCR_NUM),
                        DATE = GetINSDATEByNCRNUM(item.NCR_NUM),
                        DATE_APPROVE = item.DATEAPPROVAL,
                        PARTNUMBER = GetPARTNUMBERByNCRNUM(item.NCR_NUM),
                        ITEM_DESC = GetITEMDESCByNCRNUM(item.NCR_NUM),
                        VEN_NAME = GetVENNAMEByNCRNUM(item.NCR_NUM),
                        QTY_DISDET = item.QTY,
                        NCRNUM = item.NCR_NUM,
                        REMARK_DISDET = item.REMARK
                    };
                    lsUseAsIs.Add(model);
                }
            }
            //Get LotUseAsIs NCR_DIS
            List<NCR_DIS> listdis = _db.NCR_DIS.ToList();
            foreach (NCR_DIS item in listdis)
            {
                if (item.ADD_INS != null && item.ADD_INS.Trim().ToUpper() == CONFIRMITY_DISPN.ID_USEASIS)
                {
                    LotUseAsIsViewModel model = new LotUseAsIsViewModel
                    {
                        RECEIVER = GetReceiverByNCRNUM(item.NCR_NUM),
                        DATE = GetINSDATEByNCRNUM(item.NCR_NUM),
                        DATE_APPROVE = item.DATEAPPROVAL,
                        PARTNUMBER = GetPARTNUMBERByNCRNUM(item.NCR_NUM),
                        ITEM_DESC = GetITEMDESCByNCRNUM(item.NCR_NUM),
                        VEN_NAME = GetVENNAMEByNCRNUM(item.NCR_NUM),
                        QTY_DISDET = item.QTY,
                        NCRNUM = item.NCR_NUM,
                        REMARK_DISDET = item.REMARK
                    };
                    lsUseAsIs.Add(model);
                }
            }

            return lsUseAsIs;
        }
        public List<SelectListItem> GetdropdownVendorsSelected(string y, string ccn)
        {
            int fy = int.Parse(y);
            //cho nay lay them purloc
            var ppm = _db.SUPPLIER_PPM.Where(x => x.CCN.Equals(ccn) & x.FY == fy).ToList();
            var vendor = ppm.Select(x => x.VENDOR).ToArray();
            var puloc = ppm.Select(x => x.PUR_LOC).ToArray();
            //  var arrVendor = ppm.Select(x => $"{x.VENDOR}-{x.PUR_LOC}" ).ToArray();
           // var arrselect= _db.VENDORs.Select(x => $"{x.VENDOR1.Trim()}-{x.PUR_LOC.Trim()}").ToArray();
            List<SelectListItem> listvendor = _db.VENDORs
                .Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim() + "," + x.PUR_LOC.Trim(),
                Text = (x.VENDOR1.Trim()) + "-"+(x.PUR_LOC.Trim()) + "-" + (x.VEN_NAM) ,
                Selected = vendor.Contains(x.VENDOR1.Trim()) && puloc.Contains(x.PUR_LOC.Trim())
            }).ToList();
            return listvendor;
        }
        public List<VENDOR> StrategyList(string y, string ccn)
        {
            int fy = DateTime.Now.Year;
            if(y!=null) fy = int.Parse(y);
            if (ccn == null) ccn = "03";
            //cho nay lay them purloc
            var ppm = _db.SUPPLIER_PPM.Where(x => x.CCN.Equals(ccn) & x.FY == fy).ToList();
            var vendor = ppm.Select(x => x.VENDOR).ToArray();
            var puloc = ppm.Select(x => x.PUR_LOC).ToArray();
            List<VENDOR> listvendor = _db.VENDORs.Where(x=> vendor.Contains(x.VENDOR1.Trim()) && puloc.Contains(x.PUR_LOC.Trim())&&x.CCN==ccn).ToList();
            return listvendor;
        }
        public List<SelectListItem> GetdropdownVendors()
        {
            List<SelectListItem> listvendor = _db.VENDORs.Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim() +"," + x.PUR_LOC.Trim(),
                Text = (x.VENDOR1.Trim()) +"-"+(x.PUR_LOC.Trim()) + "-" + (x.VEN_NAM),
            }).ToList();
            return listvendor;
        }
        public List<SelectListItem> GetdropdownVendors4panel()
        {
            List<SelectListItem> listvendor = _db.VENDORs.Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim() + ";" + (x.PUR_LOC),
                Text = (x.VENDOR1.Trim()) + " " + (x.VEN_NAM),
            }).ToList();
            return listvendor;
        }
        public List<SelectListItem> GetdropdownVendorsToStrategy()
        {
            List<SelectListItem> listvendor = _db.VENDORs.Select(x => new SelectListItem
            {
                Value = x.VENDOR1.Trim() + ";" + (x.PUR_LOC),
                Text = (x.VENDOR1.Trim()) + " " + (x.VEN_NAM),
            }).ToList();
            return listvendor;
        }

        #region Function
        /// <summary>
        /// Get Receiver By NCRNUM From NCR_HDR
        /// By: SIL
        /// Date: 01/07/2018
        /// </summary>
        /// <param name="NCRNUM"></param>
        /// <returns></returns>
        public string GetReceiverByNCRNUM(string NCRNUM)
        {
            try
            {
                string receiver = _db.NCR_HDR.Where(m => m.NCR_NUM == NCRNUM).FirstOrDefault().RECEIVER;
                return receiver;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get INS_DATE By NCRNUM From NCR_HDR
        /// By: SIL
        /// Date: 01/07/2018
        /// </summary>
        /// <param name="NCRNUM"></param>
        /// <returns></returns>
        public DateTime GetINSDATEByNCRNUM(string NCRNUM)
        {
            try
            {
                DateTime ins_Date = (DateTime)_db.NCR_HDR.Where(m => m.NCR_NUM == NCRNUM).FirstOrDefault().INS_DATE;
                return ins_Date;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Get PARTNUMBER By NCRNUM From NCR_HDR
        /// By: SIL
        /// Date: 01/07/2018
        /// </summary>
        /// <param name="NCRNUM"></param>
        /// <returns></returns>
        public string GetPARTNUMBERByNCRNUM(string NCRNUM)
        {
            try
            {
                string partNum = _db.NCR_HDR.Where(m => m.NCR_NUM == NCRNUM).FirstOrDefault().MI_PART_NO;
                return partNum;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get ITEMDESC By NCRNUM From NCR_HDR
        /// By: SIL
        /// Date: 01/07/2018
        /// </summary>
        /// <param name="NCRNUM"></param>
        /// <returns></returns>
        public string GetITEMDESCByNCRNUM(string NCRNUM)
        {
            try
            {
                string partNum = _db.NCR_HDR.Where(m => m.NCR_NUM == NCRNUM).FirstOrDefault().ITEM_DESC;
                return partNum;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get VENNAME By NCRNUM From NCR_HDR
        /// By: SIL
        /// Date: 01/07/2018
        /// </summary>
        /// <param name="NCRNUM"></param>
        /// <returns></returns>
        public string GetVENNAMEByNCRNUM(string NCRNUM)
        {
            try
            {
                string partNum = _db.NCR_HDR.Where(m => m.NCR_NUM == NCRNUM).FirstOrDefault().VEN_NAME;
                return partNum;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion

        //pareto by parnum them vao table NCR-HDR cot date aprroval
        #region pareto bu partnum
        public List<PoretoViewModel> GetListPorentobyPartNum(DateTime datefrom, DateTime dateto, string partnum)
        {

            List<PoretoViewModel> listview = new List<PoretoViewModel>();
            //get all part-num
            List<NCR_HDR> ncr_hdr = _db.NCR_HDR.ToList();
            string[] listncr = new string[] { };
            // get by condition
            if (!partnum.Contains("*"))
            {
                ncr_hdr = ncr_hdr.Where(n => n.DateApproval > datefrom && n.DateApproval < dateto && n.MI_PART_NO == partnum).ToList();
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();
            }
            else
            {
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();
            }
            List<NC> listnc = _db.NCs.ToList();
            double TotalAccTEMP = 0;
            double TotalSum = 0;
            //   double  TotalPercent = 0;
            foreach (NC item in listnc)
            {
                string sqlQuery = $" select * from ncr_det where nc_desc in ( '{item.NC_CODE.Trim()}' ) AND NCR_NUM IN ('{string.Join("','", listncr)}')";
                List<NCR_DET> ListDet = _db.Database.SqlQuery<NCR_DET>(sqlQuery).ToList();
              //  List<NCR_DET> ListDet = _db.NCR_DET.Where(x => x.NC_DESC.Contains(item.NC_CODE.Trim()) & listncr.Contains(x.NCR_NUM.Trim())).ToList();
                if (ListDet.Count > 0)
                {
                    double TotalQty = ListDet.Sum(y => y.QTY);
                    PoretoViewModel ListViewPorentoNew = new PoretoViewModel()
                    {

                        Description = (item.NC_CODE + item.NC_DESC).Length > 15 ? (item.NC_CODE + item.NC_DESC).Substring(0, 15) + "..." : (item.NC_CODE + item.NC_DESC),
                        NC_CODEDES = item.NC_CODE,
                        TotalQty = TotalQty,
                        //  TotalAccQty = TotalQty + TotalAccTEMP,
                        //  PercenAccQty= (TotalQty+TotalAccTEMP)/TotalSum
                    };
                    listview.Add(ListViewPorentoNew);
                    //  TotalAccTEMP = ListViewPorentoNew.TotalAccQty;
                    //  TotalSum += ListViewPorentoNew.TotalQty;
                }
            }
            listview = listview.OrderByDescending(x => x.TotalQty).ToList();

            foreach (PoretoViewModel item2 in listview)
            {
                item2.TotalAccQty = item2.TotalQty + TotalAccTEMP;
                TotalAccTEMP = item2.TotalAccQty;
                TotalSum += item2.TotalQty;
            }

            List<PoretoViewModel> list = listview.Select(x => new PoretoViewModel
            {
                PercenAccQty = Math.Round((x.TotalAccQty / TotalSum) * 100, 1),
                Description = x.Description,
                NC_CODEDES = x.NC_CODEDES,
                TotalQty = x.TotalQty,
                TotalAccQty = x.TotalAccQty,
            }).ToList();
            //var lisdet= _db.NCR_DET.Where(d => listncr.Contains(d.NCR_NUM)).ToList();
            //   var lisdet = _db.NCR_DET.Where(x => listnc.Contains(x.NC_DESC)).Sum(y => y.QTY);
            //foreach (var item in listnc)
            //{
            //  var listcode= lisdet.Contains()
            //}
            return list;
        }
        public List<NCR_DETViewModel> GetListPorentoRawdata(DateTime datefrom, DateTime dateto, string partnum)
        {

            List<NCR_DETViewModel> listview = new List<NCR_DETViewModel>();
            List<NCR_HDR> ncr_hdr = _db.NCR_HDR.ToList();
            string[] listncr = new string[] { };
            // get by condition
            if (!partnum.Contains("*"))
            {
                ncr_hdr = ncr_hdr.Where(n => n.DateApproval > datefrom && n.DateApproval < dateto && n.MI_PART_NO == partnum).ToList();
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();

            }
            else
            {
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();
            }
            List<NCR_DET> lisdet = _db.NCR_DET.Where(d => listncr.Contains(d.NCR_NUM)).ToList();
            if (lisdet != null)
            {
                foreach (NCR_DET item in lisdet)
                {
                    List<string> listdefect = new List<string>();
                    List<string> listnc_dest = new List<string>();
                    if (item.DEFECT != null && item.DEFECT != "")
                    {
                        listdefect = cutString(item.DEFECT);
                        //if (listdefect.Count != 1)
                        //{
                        //    listdefect.RemoveAt(listdefect.Count - 1);
                        //}
                    }
                    if (item.NC_DESC != null && item.NC_DESC != "")
                    {
                        listnc_dest = cutString(item.NC_DESC);
                        //if (listnc_dest.Count != 1)
                        //{
                        //    listnc_dest.RemoveAt(listnc_dest.Count - 1);
                        //}
                    }
                    listview.Add(new NCR_DETViewModel()
                    {
                        NCR_NUM = item.NCR_NUM,
                        ITEM = item.ITEM,
                        QTY = item.QTY,
                        NC_DESC = listnc_dest,
                        NC_DESC_STRING = "",
                        DEFECT = listdefect,
                        DEFECT_STRING = "",
                        RESPONSE = item.RESPONSE,
                        // RESPONSENAME = GetResponNameByID(item.RESPONSE),
                        DISPOSITION = item.DISPOSITION,
                        //     DISPOSITIONNAME = GetDispoNameByID(item.DISPOSITION),
                        REMARK = item.REMARK,
                        DATEAPPROVAL = item.DATEAPPROVAL,
                        SEC = item.SEC,
                        PARTNUM = ncr_hdr.FirstOrDefault(x => x.NCR_NUM.Trim() == item.NCR_NUM.Trim()).MI_PART_NO
                    });
                }
            }
            // update string mô tả
            foreach (NCR_DETViewModel item in listview)
            {
                updatedescription(item);
            }
            return listview;
        }
        public void updatedescription(NCR_DETViewModel ncrview)
        {
            string desc = "";
            string defect = "";

            // lấy chuỗi mô tả dựa vào chuỗi mã desc (List<string> NC_DESC)

            foreach (string itemnc in ncrview.NC_DESC)
            {
                NC dete = _db.NCs.Where(o => o.NC_CODE.Trim() == itemnc.Trim()).FirstOrDefault();
                if (dete != null)
                {
                    desc += dete.NC_DESC + "; ";
                }
                else
                {
                    desc += "";
                }
            }

            foreach (string item in ncrview.DEFECT)
            {
                NC_GROUP data = _db.NC_GROUP.Where(m => m.NC_GRP_CODE.Trim() == item.Trim()).FirstOrDefault();
                if (data != null)
                {
                    defect += data.NC_GRP_DESC + "; ";
                }
                else
                {
                    defect += "";
                }
            }


            ncrview.NC_DESC_STRING = desc;
            ncrview.DEFECT_STRING = defect;
        }
        public List<string> cutString(string str)
        {
            List<string> list = new List<string>();
            //if (str.Contains(";"))
            //      {
            string[] temp = str.Split(';');
            list = temp.ToList();
            return list;
            //   }


        }
        #endregion
        #region pareto by vendor
        public List<PoretoViewModel> GetListPorentobyVendor(DateTime datefrom, DateTime dateto, string vendor)
        {

            List<PoretoViewModel> listview = new List<PoretoViewModel>();

            //get all part-num
            List<NCR_HDR> ncr_hdr = _db.NCR_HDR.ToList();
            string[] listncr = new string[] { };
            // get by condition
            if (!string.IsNullOrEmpty(vendor.Trim()))
            {
                ncr_hdr = _db.NCR_HDR.Where(n => n.DateApproval > datefrom && n.DateApproval < dateto && n.VENDOR == vendor).ToList();
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();

            }
            else
            {
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();
            }
            List<NC> listnc = _db.NCs.ToList();
            double TotalAccTEMP = 0;
            double TotalSum = 0;
            // double TotalPercent = 0;
            foreach (NC item in listnc)
            {
                //  list.Where(x => x.ToString().ToLower().Split(',').Where(a => a.Trim() == "a").Any()).ToList();
                string sqlQuery = $" select * from ncr_det where nc_desc in ( '{item.NC_CODE.Trim()}' ) AND NCR_NUM IN ('{string.Join("','", listncr)}')";
                List<NCR_DET> ListDet = _db.Database.SqlQuery<NCR_DET>(sqlQuery).ToList();
                //List<NCR_DET> ListDet = _db.NCR_DET.Where(x => x.NC_DESC.Contains(item.NC_CODE.Trim()) & listncr.Contains(x.NCR_NUM.Trim())).ToList();
                if (ListDet.Count > 0)
                {
                    double TotalQty = ListDet.Sum(y => y.QTY);
                    PoretoViewModel ListViewPorentoNew = new PoretoViewModel()
                    {
                        Description = (item.NC_CODE + item.NC_DESC).Length > 15 ? (item.NC_CODE + item.NC_DESC).Substring(0, 15) + "..." : (item.NC_CODE + item.NC_DESC),
                        NC_CODEDES = item.NC_CODE,
                        TotalQty = TotalQty,
                        //TotalAccQty = TotalQty + TotalAccTEMP,
                        //  PercenAccQty= (TotalQty+TotalAccTEMP)/TotalSum
                    };
                    listview.Add(ListViewPorentoNew);
                    //TotalAccTEMP = ListViewPorentoNew.TotalAccQty;
                    //TotalSum += ListViewPorentoNew.TotalQty;
                }
            }

            listview = listview.OrderByDescending(x => x.TotalQty).ToList();

            foreach (PoretoViewModel item2 in listview)
            {
                item2.TotalAccQty = item2.TotalQty + TotalAccTEMP;
                TotalAccTEMP = item2.TotalAccQty;
                TotalSum += item2.TotalQty;
            }

            List<PoretoViewModel> list = listview.Select(x => new PoretoViewModel
            {
                PercenAccQty = Math.Round((x.TotalAccQty / TotalSum) * 100, 1),
                Description = x.Description,
                NC_CODEDES = x.NC_CODEDES,
                TotalQty = x.TotalQty,
                TotalAccQty = x.TotalAccQty,
            }).ToList();
            //var lisdet= _db.NCR_DET.Where(d => listncr.Contains(d.NCR_NUM)).ToList();
            //   var lisdet = _db.NCR_DET.Where(x => listnc.Contains(x.NC_DESC)).Sum(y => y.QTY);
            //foreach (var item in listnc)
            //{
            //  var listcode= lisdet.Contains()
            //}
            return list;
        }
        public List<NCR_DETViewModel> GetListPorentoRawdatabyvendor(DateTime datefrom, DateTime dateto, string vendor)
        {

            List<NCR_DETViewModel> listview = new List<NCR_DETViewModel>();

            List<NCR_HDR> ncr_hdr = _db.NCR_HDR.ToList();
            string[] listncr = new string[] { };
            // get by condition
            if (!string.IsNullOrEmpty(vendor.Trim()))
            {
                ncr_hdr = _db.NCR_HDR.Where(n => n.DateApproval > datefrom && n.DateApproval < dateto && n.VENDOR == vendor).ToList();
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();

            }
            else
            {
                listncr = ncr_hdr.Select(n => n.NCR_NUM).ToArray();
            }
            List<NCR_DET> lisdet = _db.NCR_DET.Where(d => listncr.Contains(d.NCR_NUM)).ToList();
            if (lisdet != null)
            {
                foreach (NCR_DET item in lisdet)
                {
                    List<string> listdefect = new List<string>();
                    List<string> listnc_dest = new List<string>();
                    if (item.DEFECT != null && item.DEFECT != "")
                    {
                        listdefect = cutString(item.DEFECT);
                        //if (listdefect.Count != 1)
                        //{
                        //    listdefect.RemoveAt(listdefect.Count - 1);
                        //}
                    }
                    if (item.NC_DESC != null && item.NC_DESC != "")
                    {
                        listnc_dest = cutString(item.NC_DESC);
                        //if (listnc_dest.Count != 1)
                        //{
                        //    listnc_dest.RemoveAt(listnc_dest.Count - 1);
                        //}
                    }
                    listview.Add(new NCR_DETViewModel()
                    {
                        NCR_NUM = item.NCR_NUM,
                        ITEM = item.ITEM,
                        QTY = item.QTY,
                        NC_DESC = listnc_dest,
                        NC_DESC_STRING = "",
                        DEFECT = listdefect,
                        DEFECT_STRING = "",
                        RESPONSE = item.RESPONSE,
                        // RESPONSENAME = GetResponNameByID(item.RESPONSE),
                        DISPOSITION = item.DISPOSITION,
                        //     DISPOSITIONNAME = GetDispoNameByID(item.DISPOSITION),
                        REMARK = item.REMARK,
                        DATEAPPROVAL = item.DATEAPPROVAL,
                        SEC = item.SEC
                    });
                }
            }
            // update string mô tả
            foreach (NCR_DETViewModel item in listview)
            {
                updatedescription(item);
            }
            return listview;
        }
        public bool SaveData(List<ESCAPING_PPM> model)
        {
            try
            {
                _db.ESCAPING_PPM.AddRange(model);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                new LogWriter("SaveData").LogWrite(ex.ToString());
                return false;
            }
        }
        public bool checkidtrung(List<ESCAPING_PPM> model)
        {
            if (model.Count > 0)
            {
                int Month = model[0].PERIOD.Value.Month;
                int Year = model[0].PERIOD.Value.Year;
                string ANALYST = model[0].ANALYST;
                ESCAPING_PPM lst = _db.ESCAPING_PPM.Where(x => x.PERIOD.Value.Month == Month && x.PERIOD.Value.Year == Year && x.ANALYST == ANALYST).FirstOrDefault();
                if (lst != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool UpdateEscappingPPM(List<ESCAPING_PPM> lstUpdate)
        {
            using (DbContextTransaction tran = _db.Database.BeginTransaction())
            {
                try
                {
                    int FM = lstUpdate[0].PERIOD.Value.Month;
                    int FY = lstUpdate[0].PERIOD.Value.Year;
                    List<ESCAPING_PPM> lstDelete = _db.ESCAPING_PPM.Where(x => x.PERIOD.Value.Year == FY & x.PERIOD.Value.Month == FM).ToList();
                    _db.ESCAPING_PPM.RemoveRange(lstDelete);
                    _db.ESCAPING_PPM.AddRange(lstUpdate);
                    _db.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    new LogWriter("UpdateEscappingPPM").LogWrite(ex.ToString());
                    return false;
                }
            }
        }
        #endregion
        //EEP
        private int getQtyisue(DateTime date)
        {
            List<ESCAPING_PPM> lst = _db.ESCAPING_PPM.Where(n => n.PERIOD.Value.Month == date.Month & n.PERIOD.Value.Year == date.Year & n.QTY != null).ToList();
            return lst.Count <= 0 ? 0 : (int)lst.Sum(x => x.QTY);
        }
        private int getQtyisuetocomponent(DateTime date)
        {
            string[] a = new string[] { "FCBC", "FCBR", "MFPC", "MFSL", "MFSY" };
            List<ESCAPING_PPM> lst = _db.ESCAPING_PPM.Where(n => n.PERIOD.Value.Month == date.Month & n.PERIOD.Value.Year == date.Year & n.QTY != null & !a.Contains(n.ANALYST)).ToList();
            return lst.Count <= 0 ? 0 : (int)lst.Sum(x => x.QTY);
        }
        private int getQtyisuetosystem(DateTime date)
        {
            string[] a = new string[] { "FCBC", "FCBR", "MFPC", "MFSL", "MFSY" };
            List<ESCAPING_PPM> lst = _db.ESCAPING_PPM.Where(n => n.PERIOD.Value.Month == date.Month & n.PERIOD.Value.Year == date.Year & n.QTY != null & a.Contains(n.ANALYST)).ToList();
            return lst.Count <= 0 ? 0 : (int)lst.Sum(x => x.QTY);
        }
        private int getProQty(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            List<sp_EscapePPMToProduction_Result> lst = _db.sp_EscapePPMToProduction(year, month).ToList();
            return lst.Count() <= 0 ? 0 : (int)lst.Sum(x => x.REJ_QTY);
        }
        private int getProQtytoComponent(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            List<sp_EscapePPMToComponent_Result> lst = _db.sp_EscapePPMToComponent(year, month).ToList();
            return lst.Count() <= 0 ? 0 : (int)lst.Sum(x => x.REJ_QTY);
        }
        private int getProQtytoSysTem(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            List<sp_EscapePPMToSystem_Result> lst = _db.sp_EscapePPMToSystem(year, month).ToList();
            return lst.Count() <= 0 ? 0 : (int)lst.Sum(x => x.REJ_QTY);
        }
        //public List<EEPreportviewModel> GetdataEEPtoproduction(string yearselect)
        //{
        //    int yearNOW = int.Parse(yearselect)-1;
        //    int yearone = yearNOW - 1;
        //    string yearnam1 = yearone.ToString();
        //    int yeartwo = yearNOW - 2;
        //    string yearnam2 = yeartwo.ToString();
        //    int yearthree = yearNOW - 3;
        //    string yearnam3 = yearthree.ToString();
        //    int yearfour = yearNOW - 4;
        //    string yearnam4 = yearfour.ToString();
        //    List<EEPreportviewModel> LstEEP = _db.EEP_REPORT.Where(n => n.FY == yearselect & n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)").
        //                                     Select(n => new EEPreportviewModel
        //                                     {
        //                                         FY = n.FY,
        //                                         TYPE = n.TYPE,
        //                                         FY1 = 0,
        //                                         FY2 = 0,
        //                                         FY3 = 0,
        //                                         FY4 = 0,
        //                                         FYCurrent = 0,
        //                                         JUL = n.JUL,
        //                                         AUG = n.AUG,
        //                                         SEP = n.SEP,
        //                                         OCT = n.OCT,
        //                                         NOV = n.NOV,
        //                                         DEC = n.DEC,
        //                                         JAN = n.JAN,
        //                                         FEB = n.FEB,
        //                                         MAR = n.MAR,
        //                                         APR = n.APR,
        //                                         MAY = n.MAY,
        //                                         JUN = n.JUN
        //                                     }).ToList();
        //    if (LstEEP.Count < 1)
        //    {

        //        EEPreportviewModel listdate1 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Qty issued to Prod. (K)",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate1);

        //        EEPreportviewModel listdate2 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escaping to Prod. Qty",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate2);

        //        EEPreportviewModel listdate3 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escaping PPM to Prod.",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate3);

        //        EEPreportviewModel listdate4 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escapping PPM (Target)",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate4);
        //        //thang 7
        //        string year = "07/" + DateTime.Now.Year;
        //        DateTime date = DateTime.Parse(year);
        //        double Qty07 = getQtyisue(date);
        //        LstEEP[0].JUL = Qty07;
        //        double Proqty07 = getProQty(date);
        //        LstEEP[1].JUL = Proqty07;
        //        double EEPQty = 0;
        //        if (Qty07 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty07 / Qty07) * 1000000), 1);
        //        }
        //        LstEEP[2].JUL = EEPQty;
        //        //thang 8
        //        string year08 = "08/" + DateTime.Now.Year;
        //        DateTime date08 = DateTime.Parse(year08);
        //        double Qty08 = getQtyisue(date08);
        //        LstEEP[0].AUG = Qty08;
        //        double Proqty08 = getProQty(date08);
        //        LstEEP[1].AUG = Proqty08;
        //        if (Qty08 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty08 / Qty08) * 1000000), 1);
        //        }
        //        LstEEP[2].AUG = EEPQty;
        //        //thang 9
        //        string year09 = "09/" + DateTime.Now.Year;
        //        DateTime date09 = DateTime.Parse(year09);
        //        double Qty09 = getQtyisue(date09);
        //        LstEEP[0].SEP = Qty09;
        //        double Proqty09 = getProQty(date09);
        //        LstEEP[1].SEP = Proqty09;
        //        if (Qty09 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty09 / Qty09) * 1000000), 1);
        //        }
        //        LstEEP[2].SEP = EEPQty;
        //        //thang 10
        //        string year10 = "10/" + DateTime.Now.Year;
        //        DateTime date10 = DateTime.Parse(year10);
        //        double Qty10 = getQtyisue(date10);
        //        LstEEP[0].OCT = Qty10;
        //        double Proqty10 = getProQty(date10);
        //        LstEEP[1].OCT = Proqty10;
        //        if (Qty10 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty10 / Qty10) * 1000000), 1);
        //        }
        //        LstEEP[2].OCT = EEPQty;
        //        //thang 11
        //        string year11 = "11/" + DateTime.Now.Year;
        //        DateTime date11 = DateTime.Parse(year11);
        //        double Qty11 = getQtyisue(date11);
        //        LstEEP[0].NOV = Qty11;
        //        double Proqty11 = getProQty(date11);
        //        LstEEP[1].NOV = Proqty11;
        //        if (Qty11 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty11 / Qty11) * 1000000), 1);
        //        }
        //        LstEEP[2].NOV = EEPQty;
        //        //thang 12
        //        string year12 = "12/" + DateTime.Now.Year;
        //        DateTime date12 = DateTime.Parse(year12);
        //        double Qty12 = getQtyisue(date12);
        //        LstEEP[0].DEC = Qty12;
        //        double Proqty12 = getProQty(date12);
        //        LstEEP[1].DEC = Proqty12;
        //        if (Qty12 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty12 / Qty12) * 1000000), 1);
        //        }
        //        LstEEP[2].DEC = EEPQty;
        //        //thang 01
        //        string year01 = "01/" + yearselect;
        //        DateTime date01 = DateTime.Parse(year01);
        //        double Qty01 = getQtyisue(date01);
        //        LstEEP[0].JAN = Qty01;
        //        double Proqty01 = getProQty(date01);
        //        LstEEP[1].JAN = Proqty01;
        //        if (Qty01 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty01 / Qty01) * 1000000), 1);
        //        }
        //        LstEEP[2].JAN = EEPQty;
        //        //thang 02
        //        string year02 = "02/" + yearselect;
        //        DateTime date02 = DateTime.Parse(year02);
        //        double Qty02 = getQtyisue(date02);
        //        LstEEP[0].FEB = Qty02;
        //        double Proqty02 = getProQty(date02);
        //        LstEEP[1].FEB = Proqty02;
        //        if (Qty02 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty02 / Qty02) * 1000000), 1);
        //        }
        //        LstEEP[2].FEB = EEPQty;
        //        //thang 03
        //        string year03 = "03/" + yearselect;
        //        DateTime date03 = DateTime.Parse(year03);
        //        double Qty03 = getQtyisue(date03);
        //        LstEEP[0].MAR = Qty03;
        //        double Proqty03 = getProQty(date03);
        //        LstEEP[1].MAR = Proqty03;
        //        if (Qty03 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty03 / Qty03) * 1000000), 1);
        //        }
        //        LstEEP[2].MAR = EEPQty;
        //        //thang 04
        //        string year04 = "04/" + yearselect;
        //        DateTime date04 = DateTime.Parse(year04);
        //        double Qty04 = getQtyisue(date04);
        //        LstEEP[0].APR = Qty04;
        //        double Proqty04 = getProQty(date04);
        //        LstEEP[1].APR = Proqty04;
        //        if (Qty04 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty04 / Qty04) * 1000000), 1);
        //        }
        //        LstEEP[2].APR = EEPQty;
        //        //thang 05
        //        string year05 = "05/" + yearselect;
        //        DateTime date05 = DateTime.Parse(year05);
        //        double Qty05 = getQtyisue(date05);
        //        LstEEP[0].MAY = Qty05;
        //        double Proqty05 = getProQty(date05);
        //        LstEEP[1].MAY = Proqty05;
        //        if (Qty05 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty05 / Qty05) * 1000000), 1);
        //        }
        //        LstEEP[2].MAY = EEPQty;
        //        //thang 06
        //        string year06 = "06/" + yearselect;
        //        DateTime date06 = DateTime.Parse(year06);
        //        double Qty06 = getQtyisue(date06);
        //        LstEEP[0].JUN = Qty06;
        //        double Proqty06 = getProQty(date06);
        //        LstEEP[1].JUN = Proqty06;
        //        if (Qty06 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty06 / Qty06) * 1000000), 1);
        //        }
        //        LstEEP[2].JUN = EEPQty;

        //    }
        //    //GET LIST yEAR ago
        //    List<EEP_REPORT> lstyear1 = _db.EEP_REPORT.Where(n => n.FY == yearnam1 && n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)").ToList();
        //    //two year ago 
        //    List<EEP_REPORT> lstyear2 = _db.EEP_REPORT.Where(n => n.FY == yearnam2 && n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)").ToList();
        //    //three year
        //    List<EEP_REPORT> lstyear3 = _db.EEP_REPORT.Where(n => n.FY == yearnam3 && n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)").ToList();
        //    List<EEPreportviewModel> list = new List<EEPreportviewModel>();
        //    EEPreportviewModel a1 = LstEEP.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
        //    double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
        //    a1.FYCurrent = qtysum;
        //    EEP_REPORT a11 = lstyear1.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
        //    EEP_REPORT a21 = lstyear2.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
        //    EEP_REPORT a23 = lstyear3.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
        //    double qtyyear1;
        //    double qtyyear2;
        //    double qtyyear3;
        //    if (a11 != null)
        //    {
        //        qtyyear1 = Math.Round((a11.JUL + a11.AUG + a11.SEP + a11.OCT + a11.NOV + a11.DEC + a11.JAN + a11.FEB + a11.MAR + a11.APR + a11.APR + a11.MAY + a11.JUN), 1);
        //    }
        //    else
        //    {
        //        qtyyear1 = 0;
        //    }
        //    if (a21 != null)
        //    {
        //        qtyyear2 = Math.Round((a21.JUL + a21.AUG + a21.SEP + a21.OCT + a21.NOV + a21.DEC + a21.JAN + a21.FEB + a21.MAR + a21.APR + a21.APR + a21.MAY + a21.JUN), 1);
        //    }
        //    else
        //    {
        //        qtyyear2 = 0;
        //    }
        //    if (a23 != null)
        //    {
        //        qtyyear3 = Math.Round((a23.JUL + a23.AUG + a23.SEP + a23.OCT + a23.NOV + a23.DEC + a23.JAN + a23.FEB + a23.MAR + a23.APR + a23.APR + a23.MAY + a23.JUN), 1);
        //    }
        //    else
        //    {
        //        qtyyear3 = 0;
        //    }
        //    a1.FY1 = qtyyear1;
        //    a1.FY2 = qtyyear2;
        //    a1.FY3 = qtyyear3;
        //    list.Add(a1);
        //    EEPreportviewModel a2 = LstEEP.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();

        //    double qtytwo = Math.Round((a2.JUL + a2.AUG + a2.SEP + a2.OCT + a2.NOV + a2.DEC + a2.JAN + a2.FEB + a2.MAR + a2.APR + a2.APR + a2.MAY + a2.JUN), 1);
        //    a2.FYCurrent = qtytwo;
        //    EEP_REPORT a22 = lstyear1.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();
        //    EEP_REPORT a32 = lstyear2.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();
        //    EEP_REPORT a42 = lstyear3.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();
        //    double qtyfyear1;
        //    double qtyfyear2;
        //    double qtyfyear3;
        //    //NAM 1
        //    if (a22 != null)
        //    {
        //        qtyfyear1 = Math.Round((a22.JUL + a22.AUG + a22.SEP + a22.OCT + a22.NOV + a22.DEC + a22.JAN + a22.FEB + a22.MAR + a22.APR + a22.APR + a22.MAY + a22.JUN), 1);
        //    }
        //    else
        //    {
        //        qtyfyear1 = 0;
        //    }
        //    //NAM 2
        //    if (a32 != null)
        //    {
        //        qtyfyear2 = Math.Round((a32.JUL + a32.AUG + a32.SEP + a32.OCT + a32.NOV + a32.DEC + a32.JAN + a32.FEB + a32.MAR + a32.APR + a32.APR + a32.MAY + a32.JUN), 1);
        //    }
        //    else
        //    {
        //        qtyfyear2 = 0;
        //    }
        //    //NAM3
        //    if (a42 != null)
        //    {
        //        qtyfyear3 = Math.Round((a42.JUL + a42.AUG + a42.SEP + a42.OCT + a42.NOV + a42.DEC + a42.JAN + a42.FEB + a42.MAR + a42.APR + a42.APR + a42.MAY + a42.JUN), 1);
        //    }
        //    else
        //    {
        //        qtyfyear3 = 0;
        //    }
        //    a2.FY1 = qtyfyear1;
        //    a2.FY2 = qtyfyear2;
        //    a2.FY3 = qtyfyear3;
        //    list.Add(a2);
        //    EEPreportviewModel a3 = LstEEP.Where(x => x.TYPE == "Escaping PPM to Prod.").FirstOrDefault();
        //    double qtypercent = Math.Round(((qtytwo / qtysum) * 1000000), 1);
        //    a3.FYCurrent = qtypercent;
        //    double qtyFy1;
        //    //NAM1
        //    if (qtyyear1 != 0 && qtyfyear1 != 0)
        //    {
        //        qtyFy1 = Math.Round(((qtyfyear1 / qtyyear1) * 1000000), 1);
        //    }
        //    else
        //    {
        //        qtyFy1 = 0;
        //    }
        //    //NAM 2
        //    double qtyFy2;
        //    if (qtyyear2 != 0 && qtyfyear2 != 0)
        //    {
        //        qtyFy2 = Math.Round(((qtyfyear2 / qtyyear2) * 1000000), 1);
        //    }
        //    else
        //    {
        //        qtyFy2 = 0;
        //    }
        //    //NAM3
        //    double qtyFy3;
        //    if (qtyyear3 != 0 && qtyfyear3 != 0)
        //    {
        //        qtyFy3 = Math.Round(((qtyfyear3 / qtyyear3) * 1000000), 1);
        //    }
        //    else
        //    {
        //        qtyFy3 = 0;
        //    }
        //    a3.FY1 = qtyFy1;
        //    a3.FY2 = qtyFy2;
        //    a3.FY3 = qtyFy3;
        //    list.Add(a3);
        //    EEPreportviewModel a4 = LstEEP.Where(x => x.TYPE == "Escapping PPM (Target)").FirstOrDefault();
        //    a4.FY1 = 0;
        //    a4.FY2 = 0;
        //    a4.FY3 = 0;
        //    list.Add(a4);


        //    return list;
        //}
        //public List<EEPreportviewModel> GetdataEEPtocomponent(string yearselect)
        //{
        //    string[] type = new string[] { "Qty issued to Component Line(K)", "Escaping to Component Line Qty", "Escaping PPM to Component Line", "Escapping to Component PPM (Target)" };
        //    List<EEPreportviewModel> LstEEP = _db.EEP_REPORT.Where(n => n.FY == yearselect & n.TYPE == "Qty issued to Component Line(K)" || n.TYPE == "Escaping to Component Line Qty" || n.TYPE == "Escaping PPM to Component Line" || n.TYPE == "Escapping to Component PPM (Target)").
        //                                     Select(n => new EEPreportviewModel
        //                                     {
        //                                         FY = n.FY,
        //                                         TYPE = n.TYPE,
        //                                         FY1 = 0,
        //                                         FY2 = 0,
        //                                         FY3 = 0,
        //                                         FY4 = 0,
        //                                         FYCurrent = 0,
        //                                         JUL = n.JUL,
        //                                         AUG = n.AUG,
        //                                         SEP = n.SEP,
        //                                         OCT = n.OCT,
        //                                         NOV = n.NOV,
        //                                         DEC = n.DEC,
        //                                         JAN = n.JAN,
        //                                         FEB = n.FEB,
        //                                         MAR = n.MAR,
        //                                         APR = n.APR,
        //                                         MAY = n.MAY,
        //                                         JUN = n.JUN
        //                                     }).ToList();
        //    if (LstEEP.Count < 1)
        //    {

        //        EEPreportviewModel listdate1 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Qty issued to Component Line(K)",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate1);

        //        EEPreportviewModel listdate2 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escaping to Component Line Qty",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate2);

        //        EEPreportviewModel listdate3 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escaping PPM to Component Line",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate3);

        //        EEPreportviewModel listdate4 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escapping to Component PPM (Target)",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate4);
        //        //thang 7
        //        string year07 = "07/" + DateTime.Now.Year;
        //        DateTime date07 = DateTime.Parse(year07);
        //        double Qty07 = getQtyisuetocomponent(date07);
        //        LstEEP[0].JUL = Qty07;
        //        double Proqty07 = getProQtytoComponent(date07);
        //        LstEEP[1].JUL = Proqty07;
        //        double EEPQty = 0;
        //        if (Qty07 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty07 / Qty07) * 1000000), 1);
        //        }
        //        LstEEP[2].JUL = EEPQty;
        //        //thang 8
        //        string year08 = "08/" + DateTime.Now.Year;
        //        DateTime date08 = DateTime.Parse(year08);
        //        double Qty08 = getQtyisuetocomponent(date08);
        //        LstEEP[0].AUG = Qty08;
        //        double Proqty08 = getProQtytoComponent(date08);
        //        LstEEP[1].AUG = Proqty08;
        //        if (Qty08 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty08 / Qty08) * 1000000), 1);
        //        }
        //        LstEEP[2].AUG = EEPQty;
        //        //thang 9
        //        string year09 = "09/" + DateTime.Now.Year;
        //        DateTime date09 = DateTime.Parse(year09);
        //        double Qty09 = getQtyisuetocomponent(date09);
        //        LstEEP[0].SEP = Qty09;
        //        double Proqty09 = getProQtytoComponent(date09);
        //        LstEEP[1].SEP = Proqty09;
        //        if (Qty09 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty09 / Qty09) * 1000000), 1);
        //        }
        //        LstEEP[2].SEP = EEPQty;
        //        //thang 10
        //        string year10 = "10/" + DateTime.Now.Year;
        //        DateTime date10 = DateTime.Parse(year10);
        //        double Qty10 = getQtyisuetocomponent(date10);
        //        LstEEP[0].OCT = Qty10;
        //        double Proqty10 = getProQtytoComponent(date10);
        //        LstEEP[1].OCT = Proqty10;
        //        if (Qty10 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty10 / Qty10) * 1000000), 1);
        //        }
        //        LstEEP[2].OCT = EEPQty;
        //        //thang 11
        //        string year11 = "11/" + DateTime.Now.Year;
        //        DateTime date11 = DateTime.Parse(year11);
        //        double Qty11 = getQtyisuetocomponent(date11);
        //        LstEEP[0].NOV = Qty11;
        //        double Proqty11 = getProQtytoComponent(date11);
        //        LstEEP[1].NOV = Proqty11;
        //        if (Qty11 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty11 / Qty11) * 1000000), 1);
        //        }
        //        LstEEP[2].NOV = EEPQty;
        //        //thang 12
        //        string year12 = "12/" + DateTime.Now.Year;
        //        DateTime date12 = DateTime.Parse(year12);
        //        double Qty12 = getQtyisuetocomponent(date12);
        //        LstEEP[0].DEC = Qty12;
        //        double Proqty12 = getProQtytoComponent(date12);
        //        LstEEP[1].DEC = Proqty12;
        //        if (Qty12 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty12 / Qty12) * 1000000), 1);
        //        }
        //        LstEEP[2].DEC = EEPQty;
        //        //thang 01
        //        string year01 = "01/" + yearselect;
        //        DateTime date01 = DateTime.Parse(year01);
        //        double Qty01 = getQtyisuetocomponent(date01);
        //        LstEEP[0].JAN = Qty01;
        //        double Proqty01 = getProQtytoComponent(date01);
        //        LstEEP[1].JAN = Proqty01;
        //        if (Qty01 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty01 / Qty01) * 1000000), 1);
        //        }
        //        LstEEP[2].JAN = EEPQty;
        //        //thang 02
        //        string year02 = "02/" + yearselect;
        //        DateTime date02 = DateTime.Parse(year02);
        //        double Qty02 = getQtyisuetocomponent(date02);
        //        LstEEP[0].FEB = Qty02;
        //        double Proqty02 = getProQtytoComponent(date02);
        //        LstEEP[1].FEB = Proqty02;
        //        if (Qty02 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty02 / Qty02) * 1000000), 1);
        //        }
        //        LstEEP[2].FEB = EEPQty;
        //        //thang 03
        //        string year03 = "03/" + yearselect;
        //        DateTime date03 = DateTime.Parse(year03);
        //        double Qty03 = getQtyisuetocomponent(date03);
        //        LstEEP[0].MAR = Qty03;
        //        double Proqty03 = getProQtytoComponent(date03);
        //        LstEEP[1].MAR = Proqty03;
        //        if (Qty03 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty03 / Qty03) * 1000000), 1);
        //        }
        //        LstEEP[2].MAR = EEPQty;
        //        //thang 04
        //        string year04 = "04/" + yearselect;
        //        DateTime date04 = DateTime.Parse(year04);
        //        double Qty04 = getQtyisuetocomponent(date04);
        //        LstEEP[0].APR = Qty04;
        //        double Proqty04 = getProQtytoComponent(date04);
        //        LstEEP[1].APR = Proqty04;
        //        if (Qty04 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty04 / Qty04) * 1000000), 1);
        //        }
        //        LstEEP[2].APR = EEPQty;
        //        //thang 05
        //        string year05 = "05/" + yearselect;
        //        DateTime date05 = DateTime.Parse(year05);
        //        double Qty05 = getQtyisuetocomponent(date05);
        //        LstEEP[0].MAY = Qty05;
        //        double Proqty05 = getProQtytoComponent(date05);
        //        LstEEP[1].MAY = Proqty05;
        //        if (Qty05 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty05 / Qty05) * 1000000), 1);
        //        }
        //        LstEEP[2].MAY = EEPQty;
        //        //thang 06
        //        string year06 = "06/" + yearselect;
        //        DateTime date06 = DateTime.Parse(year06);
        //        double Qty06 = getQtyisuetocomponent(date06);
        //        LstEEP[0].JUN = Qty06;
        //        double Proqty06 = getProQtytoComponent(date06);
        //        LstEEP[1].JUN = Proqty06;
        //        if (Qty06 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty06 / Qty06) * 1000000), 1);
        //        }
        //        LstEEP[2].JUN = EEPQty;
        //    }


        //    List<EEPreportviewModel> listRP = new List<EEPreportviewModel>();
        //    EEPreportviewModel a1 = LstEEP.Where(x => x.TYPE == "Qty issued to Component Line(K)").FirstOrDefault();
        //    double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
        //    a1.FYCurrent = qtysum;
        //    listRP.Add(a1);
        //    EEPreportviewModel a2 = LstEEP.Where(x => x.TYPE == "Escaping to Component Line Qty").FirstOrDefault();
        //    double qtytwo = Math.Round((a2.JUL + a2.AUG + a2.SEP + a2.OCT + a2.NOV + a2.DEC + a2.JAN + a2.FEB + a2.MAR + a2.APR + a2.APR + a2.MAY + a2.JUN), 1);
        //    a2.FYCurrent = qtytwo;
        //    listRP.Add(a2);
        //    EEPreportviewModel a3 = LstEEP.Where(x => x.TYPE == "Escaping PPM to Component Line").FirstOrDefault();
        //    double qtypercent = 0;
        //    if (qtytwo != 0 && qtysum != 0)
        //    {
        //        qtypercent = Math.Round(((qtytwo / qtysum) * 1000000), 1);
        //    }
        //    else
        //    {
        //        qtypercent = 0;
        //    }
        //    a3.FYCurrent = qtypercent;
        //    listRP.Add(a3);
        //    EEPreportviewModel a4 = LstEEP.Where(x => x.TYPE == "Escapping to Component PPM (Target)").FirstOrDefault();
        //    listRP.Add(a4);


        //    //fianal
        //    //double qtysum = Math.Round((LstEEP[0].JUL + LstEEP[0].AUG + LstEEP[0].SEP + LstEEP[0].OCT + LstEEP[0].NOV + LstEEP[0].DEC + LstEEP[0].JAN + LstEEP[0].FEB + LstEEP[0].MAR + LstEEP[0].APR + LstEEP[0].APR + LstEEP[0].MAY + LstEEP[0].JUN), 1);
        //    //LstEEP[0].FYCurrent = qtysum;
        //    //double qtytwo = Math.Round((LstEEP[1].JUL + LstEEP[1].AUG + LstEEP[1].SEP + LstEEP[1].OCT + LstEEP[1].NOV + LstEEP[1].DEC + LstEEP[1].JAN + LstEEP[1].FEB + LstEEP[1].MAR + LstEEP[1].APR + LstEEP[1].APR + LstEEP[1].MAY + LstEEP[1].JUN), 1);
        //    //LstEEP[1].FYCurrent = qtytwo;
        //    //LstEEP[2].FYCurrent = Math.Round(((qtytwo/qtysum) * 1000000),1);
        //    return listRP;
        //}
        //public List<EEPreportviewModel> GetdataEEPtoSystem(string yearselect)
        //{
        //    List<EEPreportviewModel> LstEEP = _db.EEP_REPORT.Where(n => n.FY == yearselect & n.TYPE == "Qty issued to System Line(K)" || n.TYPE == "Escaping to System Line Qty" || n.TYPE == "Escaping PPM to System Line" || n.TYPE == "Escapping to System PPM (Target)").
        //                                     Select(n => new EEPreportviewModel
        //                                     {
        //                                         FY = n.FY,
        //                                         TYPE = n.TYPE,
        //                                         FY1 = 0,
        //                                         FY2 = 0,
        //                                         FY3 = 0,
        //                                         FY4 = 0,
        //                                         FYCurrent = 0,
        //                                         JUL = n.JUL,
        //                                         AUG = n.AUG,
        //                                         SEP = n.SEP,
        //                                         OCT = n.OCT,
        //                                         NOV = n.NOV,
        //                                         DEC = n.DEC,
        //                                         JAN = n.JAN,
        //                                         FEB = n.FEB,
        //                                         MAR = n.MAR,
        //                                         APR = n.APR,
        //                                         MAY = n.MAY,
        //                                         JUN = n.JUN
        //                                     }).ToList();
        //    if (LstEEP.Count < 1)
        //    {

        //        EEPreportviewModel listdate1 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Qty issued to System Line(K)",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate1);

        //        EEPreportviewModel listdate2 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escaping to System Line Qty",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate2);

        //        EEPreportviewModel listdate3 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escaping PPM to System Line",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate3);

        //        EEPreportviewModel listdate4 = new EEPreportviewModel
        //        {
        //            FY = yearselect,
        //            TYPE = "Escapping to System PPM (Target)",
        //            FY1 = 0,
        //            FY2 = 0,
        //            FY3 = 0,
        //            FYCurrent = 0,
        //            JUL = 0,
        //            AUG = 0,
        //            SEP = 0,
        //            OCT = 0,
        //            NOV = 0,
        //            DEC = 0,
        //            JAN = 0,
        //            FEB = 0,
        //            MAR = 0,
        //            APR = 0,
        //            MAY = 0,
        //            JUN = 0
        //        };
        //        LstEEP.Add(listdate4);
        //        //thang 7
        //        string year07 = "07/" + DateTime.Now.Year;
        //        DateTime date07 = DateTime.Parse(year07);
        //        double Qty07 = getQtyisuetosystem(date07);
        //        LstEEP[0].JUL = Qty07;
        //        double Proqty07 = getProQtytoSysTem(date07);
        //        LstEEP[1].JUL = Proqty07;
        //        double EEPQty = 0;
        //        if (Qty07 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty07 / Qty07) * 1000000), 1);
        //        }
        //        LstEEP[2].JUL = EEPQty;
        //        //thang 8
        //        string year08 = "08/" + DateTime.Now.Year;
        //        DateTime date08 = DateTime.Parse(year08);
        //        double Qty08 = getQtyisuetosystem(date08);
        //        LstEEP[0].AUG = Qty08;
        //        double Proqty08 = getProQtytoSysTem(date08);
        //        LstEEP[1].AUG = Proqty08;
        //        if (Qty08 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty08 / Qty08) * 1000000), 1);
        //        }
        //        LstEEP[2].AUG = EEPQty;
        //        //thang 9
        //        string year09 = "09/" + DateTime.Now.Year;
        //        DateTime date09 = DateTime.Parse(year09);
        //        double Qty09 = getQtyisuetosystem(date09);
        //        LstEEP[0].SEP = Qty09;
        //        double Proqty09 = getProQtytoSysTem(date09);
        //        LstEEP[1].SEP = Proqty09;
        //        if (Qty09 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty09 / Qty09) * 1000000), 1);
        //        }
        //        LstEEP[2].SEP = EEPQty;
        //        //thang 10
        //        string year10 = "10/" + DateTime.Now.Year;
        //        DateTime date10 = DateTime.Parse(year10);
        //        double Qty10 = getQtyisuetosystem(date10);
        //        LstEEP[0].OCT = Qty10;
        //        double Proqty10 = getProQtytoSysTem(date10);
        //        LstEEP[1].OCT = Proqty10;
        //        if (Qty10 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty10 / Qty10) * 1000000), 1);
        //        }
        //        LstEEP[2].OCT = EEPQty;
        //        //thang 11
        //        string year11 = "11/" + DateTime.Now.Year;
        //        DateTime date11 = DateTime.Parse(year11);
        //        double Qty11 = getQtyisuetosystem(date11);
        //        LstEEP[0].NOV = Qty11;
        //        double Proqty11 = getProQtytoSysTem(date11);
        //        LstEEP[1].NOV = Proqty11;
        //        if (Qty11 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty11 / Qty11) * 1000000), 1);
        //        }
        //        LstEEP[2].NOV = EEPQty;
        //        //thang 12
        //        string year12 = "12/" + DateTime.Now.Year;
        //        DateTime date12 = DateTime.Parse(year12);
        //        double Qty12 = getQtyisuetosystem(date12);
        //        LstEEP[0].DEC = Qty12;
        //        double Proqty12 = getProQtytoSysTem(date12);
        //        LstEEP[1].DEC = Proqty12;
        //        if (Qty12 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty12 / Qty12) * 1000000), 1);
        //        }
        //        LstEEP[2].DEC = EEPQty;
        //        //thang 01
        //        string year01 = "01/" + yearselect;
        //        DateTime date01 = DateTime.Parse(year01);
        //        double Qty01 = getQtyisuetosystem(date01);
        //        LstEEP[0].JAN = Qty01;
        //        double Proqty01 = getProQtytoSysTem(date01);
        //        LstEEP[1].JAN = Proqty01;
        //        if (Qty01 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty01 / Qty01) * 1000000), 1);
        //        }
        //        LstEEP[2].JAN = EEPQty;
        //        //thang 02
        //        string year02 = "02/" + yearselect;
        //        DateTime date02 = DateTime.Parse(year02);
        //        double Qty02 = getQtyisuetosystem(date02);
        //        LstEEP[0].FEB = Qty02;
        //        double Proqty02 = getProQtytoSysTem(date02);
        //        LstEEP[1].FEB = Proqty02;
        //        if (Qty02 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty02 / Qty02) * 1000000), 1);
        //        }
        //        LstEEP[2].FEB = EEPQty;
        //        //thang 03
        //        string year03 = "03/" + yearselect;
        //        DateTime date03 = DateTime.Parse(year03);
        //        double Qty03 = getQtyisuetosystem(date03);
        //        LstEEP[0].MAR = Qty03;
        //        double Proqty03 = getProQtytoSysTem(date03);
        //        LstEEP[1].MAR = Proqty03;
        //        if (Qty03 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty03 / Qty03) * 1000000), 1);
        //        }
        //        LstEEP[2].MAR = EEPQty;
        //        //thang 04
        //        string year04 = "04/" + yearselect;
        //        DateTime date04 = DateTime.Parse(year04);
        //        double Qty04 = getQtyisuetosystem(date04);
        //        LstEEP[0].APR = Qty04;
        //        double Proqty04 = getProQtytoSysTem(date04);
        //        LstEEP[1].APR = Proqty04;
        //        if (Qty04 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty04 / Qty04) * 1000000), 1);
        //        }
        //        LstEEP[2].APR = EEPQty;
        //        //thang 05
        //        string year05 = "05/" + yearselect;
        //        DateTime date05 = DateTime.Parse(year05);
        //        double Qty05 = getQtyisuetosystem(date05);
        //        LstEEP[0].MAY = Qty05;
        //        double Proqty05 = getProQtytoSysTem(date05);
        //        LstEEP[1].MAY = Proqty05;
        //        if (Qty05 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty05 / Qty05) * 1000000), 1);
        //        }
        //        LstEEP[2].MAY = EEPQty;
        //        //thang 06
        //        string year06 = "06/" + yearselect;
        //        DateTime date06 = DateTime.Parse(year06);
        //        double Qty06 = getQtyisuetosystem(date06);
        //        LstEEP[0].JUN = Qty06;
        //        double Proqty06 = getProQtytoSysTem(date06);
        //        LstEEP[1].JUN = Proqty06;
        //        if (Qty06 == 0)
        //        {
        //            EEPQty = 0;
        //        }
        //        else
        //        {
        //            EEPQty = Math.Round(((Proqty06 / Qty06) * 1000000), 1);
        //        }
        //        LstEEP[2].JUN = EEPQty;
        //    }


        //    List<EEPreportviewModel> listRP = new List<EEPreportviewModel>();
        //    EEPreportviewModel a1 = LstEEP.Where(x => x.TYPE == "Qty issued to System Line(K)").FirstOrDefault();
        //    double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
        //    a1.FYCurrent = qtysum;
        //    listRP.Add(a1);
        //    EEPreportviewModel a2 = LstEEP.Where(x => x.TYPE == "Escaping to System Line Qty").FirstOrDefault();
        //    double qtytwo = Math.Round((a2.JUL + a2.AUG + a2.SEP + a2.OCT + a2.NOV + a2.DEC + a2.JAN + a2.FEB + a2.MAR + a2.APR + a2.APR + a2.MAY + a2.JUN), 1);
        //    a2.FYCurrent = qtytwo;
        //    listRP.Add(a2);
        //    EEPreportviewModel a3 = LstEEP.Where(x => x.TYPE == "Escaping PPM to System Line").FirstOrDefault();
        //    double qtypercent = Math.Round(((qtytwo / qtysum) * 1000000), 1);
        //    a3.FYCurrent = qtypercent;
        //    listRP.Add(a3);
        //    EEPreportviewModel a4 = LstEEP.Where(x => x.TYPE == "Escapping to System PPM (Target)").FirstOrDefault();
        //    listRP.Add(a4);


        //    //fianal
        //    //double qtysum = Math.Round((LstEEP[0].JUL + LstEEP[0].AUG + LstEEP[0].SEP + LstEEP[0].OCT + LstEEP[0].NOV + LstEEP[0].DEC + LstEEP[0].JAN + LstEEP[0].FEB + LstEEP[0].MAR + LstEEP[0].APR + LstEEP[0].APR + LstEEP[0].MAY + LstEEP[0].JUN), 1);
        //    //LstEEP[0].FYCurrent = qtysum;
        //    //double qtytwo = Math.Round((LstEEP[1].JUL + LstEEP[1].AUG + LstEEP[1].SEP + LstEEP[1].OCT + LstEEP[1].NOV + LstEEP[1].DEC + LstEEP[1].JAN + LstEEP[1].FEB + LstEEP[1].MAR + LstEEP[1].APR + LstEEP[1].APR + LstEEP[1].MAY + LstEEP[1].JUN), 1);
        //    //LstEEP[1].FYCurrent = qtytwo;
        //    //LstEEP[2].FYCurrent = Math.Round(((qtytwo/qtysum) * 1000000),1);
        //    return listRP;
        //}
        public List<EEPreportviewModel> GetdataEEPtoproduction(string yearselect)
        {
            int yearNOW = int.Parse(yearselect) - 1;
            int yearone = yearNOW;
            string yearnam1 = yearone.ToString();
            int yeartwo = yearNOW - 1;
            string yearnam2 = yeartwo.ToString();
            int yearthree = yearNOW - 2;
            string yearnam3 = yearthree.ToString();
            int yearfour = yearNOW - 3;
            string yearnam4 = yearfour.ToString();
            List<EEPreportviewModel> LstEEP = _db.EEP_REPORT.Where(n => n.FY == yearselect &&( n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)")).
                                             Select(n => new EEPreportviewModel
                                             {
                                                 FY = n.FY,
                                                 TYPE = n.TYPE,
                                                 FY1 = 0,
                                                 FY2 = 0,
                                                 FY3 = 0,
                                                 FY4 = 0,
                                                 FYCurrent = 0,
                                                 JUL = n.JUL,
                                                 AUG = n.AUG,
                                                 SEP = n.SEP,
                                                 OCT = n.OCT,
                                                 NOV = n.NOV,
                                                 DEC = n.DEC,
                                                 JAN = n.JAN,
                                                 FEB = n.FEB,
                                                 MAR = n.MAR,
                                                 APR = n.APR,
                                                 MAY = n.MAY,
                                                 JUN = n.JUN
                                             }).ToList();
            if (LstEEP.Count > 1)
            {
                foreach (var item in LstEEP)
                {
                    if (yearNOW == (int.Parse(yearselect) - 1))
                    {
                        #region "Month 07"
                        string year07 = "07/" + yearNOW;
                        DateTime date07 = DateTime.Parse(year07);
                        double Qty07 = 0;
                        double Proqty07 = 0;
                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.JUL != 0)
                            {
                                Qty07 = item.JUL;
                            }
                            else
                            {
                                Qty07 = getQtyisue(date07);
                            }
                            LstEEP[3].JUL = Qty07;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.JUL != 0)
                            {
                                Proqty07 = item.JUL;

                            }
                            else
                            {
                                Proqty07 = getProQty(date07);
                            }
                            LstEEP[1].JUL = Proqty07;
                        }

                        #endregion

                        #region "Month 08"
                        string year08 = "08/" + yearNOW;
                        DateTime date08 = DateTime.Parse(year08);
                        double Qty08 = 0;
                        double Proqty08 = 0;


                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.AUG != 0)
                            {
                                Qty08 = item.AUG;
                            }
                            else
                            {
                                Qty08 = getQtyisue(date08);
                            }
                            LstEEP[3].AUG = Qty08;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.AUG != 0)
                            {
                                Proqty08 = item.AUG;
                            }
                            else
                            {
                                Proqty08 = getProQty(date08);
                            }
                            LstEEP[1].AUG = Proqty08;
                        }
                        #endregion

                        #region "Month 09"
                        string year09 = "09/" + yearNOW;
                        DateTime date09 = DateTime.Parse(year09);
                        double Qty09 = 0;
                        double Proqty09 = 0;


                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.SEP != 0)
                            {
                                Qty09 = item.SEP;
                            }
                            else
                            {
                                Qty09 = getQtyisue(date09);
                            }
                            LstEEP[3].SEP = Qty09;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.SEP != 0)
                            {
                                Proqty09 = item.SEP;

                            }
                            else
                            {
                                Proqty09 = getProQty(date09);
                            }
                            LstEEP[1].SEP = Proqty09;
                        }
                        #endregion

                        #region "Month 10"
                        string year10 = "10/" + yearNOW;
                        DateTime date10 = DateTime.Parse(year10);
                        double Qty10 = 0;
                        double Proqty10 = 0;


                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.OCT != 0)
                            {
                                Qty10 = item.OCT;
                            }
                            else
                            {
                                Qty10 = getQtyisue(date10);
                            }
                            LstEEP[3].OCT = Qty10;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.OCT != 0)
                            {
                                Proqty10 = item.OCT;

                            }
                            else
                            {
                                Proqty10 = getProQty(date10);
                            }
                            LstEEP[1].OCT = Proqty10;
                        }
                        #endregion

                        #region "Month 11"
                        string year11 = "11/" + yearNOW;
                        DateTime date11 = DateTime.Parse(year11);
                        double Qty11 = 0;
                        double Proqty11 = 0;


                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.NOV != 0)
                            {
                                Qty11 = item.SEP;
                            }
                            else
                            {
                                Qty11 = getQtyisue(date11);
                            }
                            LstEEP[3].NOV = Qty11;

                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.NOV != 0)
                            {
                                Proqty11 = item.NOV;

                            }
                            else
                            {
                                Proqty11 = getProQty(date11);
                            }
                            LstEEP[1].NOV = Proqty11;
                        }
                        #endregion

                        #region "Month 12"
                        string year12 = "12/" + yearNOW;
                        DateTime date12 = DateTime.Parse(year12);
                        double Qty12 = 0;
                        double Proqty12 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.DEC != 0)
                            {
                                Qty12 = item.DEC;
                            }
                            else
                            {
                                Qty12 = getQtyisue(date12);
                            }
                            LstEEP[3].DEC = Qty12;

                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.DEC != 0)
                            {
                                Proqty12 = item.DEC;

                            }
                            else
                            {
                                Proqty12 = getProQty(date12);
                            }
                            LstEEP[1].DEC = Proqty12;
                        }
                        #endregion

                        #region "Month 01"
                        string year01 = "01/" + (yearNOW + 1);
                        DateTime date01 = DateTime.Parse(year01);
                        double Qty01 = 0;
                        double Proqty01 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.JAN != 0)
                            {
                                Qty01 = item.JAN;
                            }
                            else
                            {
                                Qty01 = getQtyisue(date01);
                            }
                            LstEEP[3].JAN = Qty01;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.JAN != 0)
                            {
                                Proqty01 = item.JAN;

                            }
                            else
                            {
                                Proqty01 = getProQty(date01);
                            }
                            LstEEP[1].JAN = Proqty01;
                        }
                        #endregion

                        #region "Month 02"
                        string year02 = "02/" + (yearNOW + 1);
                        DateTime date02 = DateTime.Parse(year02);
                        double Qty02 = 0;
                        double Proqty02 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.FEB != 0)
                            {
                                Qty02 = item.FEB;
                            }
                            else
                            {
                                Qty02 = getQtyisue(date02);
                            }
                            LstEEP[3].FEB = Qty02;

                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.FEB != 0)
                            {
                                Proqty02 = item.FEB;

                            }
                            else
                            {
                                Proqty02 = getProQty(date02);
                            }
                            LstEEP[1].FEB = Proqty02;
                        }

                        #endregion

                        #region "Month 03"
                        string year03 = "03/" + (yearNOW + 1);
                        DateTime date03 = DateTime.Parse(year03);
                        double Qty03 = 0;
                        double Proqty03 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.MAR != 0)
                            {
                                Qty03 = item.MAR;
                            }
                            else
                            {
                                Qty03 = getQtyisue(date03);
                            }
                            LstEEP[3].MAR = Qty03;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.MAR != 0)
                            {
                                Proqty03 = item.MAR;

                            }
                            else
                            {
                                Proqty03 = getProQty(date03);
                            }
                            LstEEP[1].MAR = Proqty03;
                        }

                        #endregion

                        #region "Month 04"
                        string year04 = "04/" + (yearNOW + 1);
                        DateTime date04 = DateTime.Parse(year04);
                        double Qty04 = 0;
                        double Proqty04 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.APR != 0)
                            {
                                Qty04 = item.APR;
                            }
                            else
                            {
                                Qty04 = getQtyisue(date04);
                            }
                            LstEEP[3].APR = Qty04;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.APR != 0)
                            {
                                Proqty04 = item.APR;

                            }
                            else
                            {
                                Proqty04 = getProQty(date04);
                            }
                            LstEEP[1].APR = Proqty04;

                        }

                        #endregion

                        #region "Month 05"
                        string year05 = "05/" + (yearNOW + 1);
                        DateTime date05 = DateTime.Parse(year05);
                        double Qty05 = 0;
                        double Proqty05 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.MAY != 0)
                            {
                                Qty05 = item.MAY;
                            }
                            else
                            {
                                Qty05 = getQtyisue(date05);
                            }
                            LstEEP[3].MAY = Qty05;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.MAY != 0)
                            {
                                Proqty05 = item.MAY;

                            }
                            else
                            {
                                Proqty05 = getProQty(date05);
                            }
                            LstEEP[1].MAY = Proqty05;
                        }

                        #endregion

                        #region "Month 06"
                        string year06 = "06/" + (yearNOW + 1);
                        DateTime date06 = DateTime.Parse(year06);
                        double Qty06 = 0;
                        double Proqty06 = 0;

                        if (item.TYPE == "Qty issued to Prod. (K)")
                        {
                            if (item.JUN != 0)
                            {
                                Qty06 = item.JUN;
                            }
                            else
                            {
                                Qty06 = getQtyisue(date06);
                            }
                            LstEEP[3].JUN = Qty06;
                        }
                        if (item.TYPE == "Escaping to Prod. Qty")
                        {
                            if (item.JUN != 0)
                            {
                                Proqty06 = item.JUN;

                            }
                            else
                            {
                                Proqty06 = getProQty(date06);
                            }
                            LstEEP[1].JUN = Proqty06;
                        }
                        #endregion
                    }
                }

                foreach (var item in LstEEP)
                {
                    #region "Month 07"
                    double EEPQty07 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.JUL != 0)
                        {
                            EEPQty07 = item.JUL;
                        }
                        else if (LstEEP[1].JUL != 0 && LstEEP[3].JUL != 0)
                        {
                            EEPQty07 = Math.Round(((LstEEP[1].JUL / LstEEP[3].JUL) * 1000000), 1);
                        }
                        LstEEP[0].JUL = EEPQty07;
                    }
                    #endregion

                    #region "Month 08"
                    double EEPQty08 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.AUG != 0)
                        {
                            EEPQty08 = item.AUG;
                        }
                        else if (LstEEP[1].AUG != 0 && LstEEP[3].AUG != 0)
                        {
                            EEPQty08 = Math.Round(((LstEEP[1].AUG / LstEEP[3].AUG) * 1000000), 1);
                        }
                        LstEEP[0].AUG = EEPQty08;
                    }
                    #endregion

                    #region "Month 09"
                    double EEPQty09 = 0;

                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.SEP != 0)
                        {
                            EEPQty09 = item.SEP;
                        }
                        else if ((LstEEP[1].SEP != 0 && LstEEP[3].SEP != 0))
                        {
                            EEPQty09 = Math.Round(((LstEEP[1].SEP / LstEEP[3].SEP) * 1000000), 1);
                        }
                        LstEEP[0].SEP = EEPQty09;

                    }
                    #endregion

                    #region "Month 10"
                    double EEPQty10 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.OCT != 0)
                        {
                            EEPQty10 = item.OCT;
                        }
                        else if ((LstEEP[1].OCT != 0 && LstEEP[3].OCT != 0))
                        {
                            EEPQty10 = Math.Round(((LstEEP[1].OCT / LstEEP[3].OCT) * 1000000), 1);
                        }
                        LstEEP[0].OCT = EEPQty10;
                    }
                    #endregion

                    #region "Month 11"
                    double EEPQty11 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.NOV != 0)
                        {
                            EEPQty11 = item.NOV;
                        }
                        else if ((LstEEP[1].NOV != 0 && LstEEP[3].NOV != 0))
                        {
                            EEPQty11 = Math.Round(((LstEEP[1].NOV / LstEEP[3].NOV) * 1000000), 1);
                        }
                        LstEEP[0].NOV = EEPQty11;
                    }
                    #endregion

                    #region "Month 12"
                    double EEPQty12 = 0;

                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.DEC != 0)
                        {
                            EEPQty12 = item.DEC;
                        }
                        else if ((LstEEP[1].DEC != 0 && LstEEP[3].DEC != 0))
                        {
                            EEPQty12 = Math.Round(((LstEEP[1].DEC / LstEEP[3].DEC) * 1000000), 1);
                        }

                        LstEEP[0].DEC = EEPQty12;
                    }
                    #endregion

                    #region "Month 01"
                    double EEPQty01 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.JAN != 0)
                        {
                            EEPQty01 = item.JAN;
                        }
                        else if ((LstEEP[1].JAN != 0 && LstEEP[3].JAN != 0))
                        {
                            EEPQty01 = Math.Round(((LstEEP[1].JAN / LstEEP[3].JAN) * 1000000), 1);
                        }
                        LstEEP[0].JAN = EEPQty01;
                    }
                    #endregion

                    #region "Month 02"
                    double EEPQty02 = 0;

                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.FEB != 0)
                        {
                            EEPQty02 = item.FEB;
                        }
                        else if ((LstEEP[1].FEB != 0 && LstEEP[3].FEB != 0))
                        {
                            EEPQty02 = Math.Round(((LstEEP[1].FEB / LstEEP[3].FEB) * 1000000), 1);
                        }
                        LstEEP[0].FEB = EEPQty02;
                    }
                    #endregion

                    #region "Month 03"
                    double EEPQty03 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.MAR != 0)
                        {
                            EEPQty03 = item.MAR;
                        }
                        else if ((LstEEP[1].MAR != 0 && LstEEP[3].MAR != 0))
                        {
                            EEPQty03 = Math.Round(((LstEEP[1].MAR / LstEEP[3].MAR) * 1000000), 1);
                        }
                        LstEEP[0].MAR = EEPQty03;
                    }
                    #endregion

                    #region "Month 04"
                    double EEPQty04 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.APR != 0)
                        {
                            EEPQty04 = item.APR;
                        }
                        else if ((LstEEP[1].APR != 0 && LstEEP[3].APR != 0))
                        {
                            EEPQty04 = Math.Round(((LstEEP[1].APR / LstEEP[3].APR) * 1000000), 1);
                        }
                        LstEEP[0].APR = EEPQty04;
                    }
                    #endregion

                    #region "Month 05"
                    double EEPQty05 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.MAY != 0)
                        {
                            EEPQty05 = item.MAY;
                        }
                        else if ((LstEEP[1].MAY != 0 && LstEEP[3].MAY != 0))
                        {
                            EEPQty05 = Math.Round(((LstEEP[1].MAY / LstEEP[3].MAY) * 1000000), 1);
                        }
                        LstEEP[0].MAY = EEPQty05;
                    }
                    #endregion

                    #region "Month 06"
                    double EEPQty06 = 0;
                    if (item.TYPE == "Escaping PPM to Prod.")
                    {
                        if (item.JUN != 0)
                        {
                            EEPQty06 = item.JUN;
                        }
                        else if ((LstEEP[1].JUN != 0 && LstEEP[3].JUN != 0))
                        {
                            EEPQty06 = Math.Round(((LstEEP[1].JUN / LstEEP[3].JUN) * 1000000), 1);
                        }
                        LstEEP[0].JUN = EEPQty06;
                    }
                    #endregion
                }

                #region "Customize"
                //for (int i = 0; i < LstEEP.Count; i++)
                //{
                //    if (yearNOW == (int.Parse(yearselect) - 1))
                //    {
                //        for (int j = 0; j <= 12; j++)
                //        {
                //            DateTime date = DateTime;
                //            double EEPQty = 0;
                //            double Qty = 0;
                //            double Proqty = 0;

                //            if (LstEEP[i].TYPE == "Qty issued to Prod. (K)")
                //            {
                //                //07
                //                if (LstEEP[i].JUL != 0)
                //                {
                //                    Qty = LstEEP[i].JUL;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //08
                //                if (LstEEP[i].AUG != 0)
                //                {
                //                    Qty = LstEEP[i].AUG;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //09
                //                if (LstEEP[i].SEP != 0)
                //                {
                //                    Qty = LstEEP[i].SEP;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //10
                //                if (LstEEP[i].OCT != 0)
                //                {
                //                    Qty = LstEEP[i].OCT;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //11
                //                if (LstEEP[i].NOV != 0)
                //                {
                //                    Qty = LstEEP[i].NOV;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //12
                //                if (LstEEP[i].DEC != 0)
                //                {
                //                    Qty = LstEEP[i].DEC;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //01
                //                if (LstEEP[i].JAN != 0)
                //                {
                //                    Qty = LstEEP[i].JAN;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //02
                //                if (LstEEP[i].FEB != 0)
                //                {
                //                    Qty = LstEEP[i].FEB;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //03
                //                if (LstEEP[i].MAR != 0)
                //                {
                //                    Qty = LstEEP[i].MAR;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //   04
                //                if (LstEEP[i].APR != 0)
                //                {
                //                    Qty = LstEEP[i].APR;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //   05
                //                if (LstEEP[i].MAY != 0)
                //                {
                //                    Qty = LstEEP[i].MAY;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //                //   06
                //                if (LstEEP[i].JUN != 0)
                //                {
                //                    Qty = LstEEP[i].JUN;
                //                }
                //                else
                //                {
                //                    Qty = getQtyisue(date);
                //                }
                //            }

                //            if (LstEEP[i].TYPE == "Escaping to Prod. Qty")
                //            {
                //                //07
                //                if (LstEEP[i].JUL != 0)
                //                {
                //                    Proqty = LstEEP[i].JUL;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //08
                //                if (LstEEP[i].AUG != 0)
                //                {
                //                    Proqty = LstEEP[i].AUG;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //09
                //                if (LstEEP[i].SEP != 0)
                //                {
                //                    Proqty = LstEEP[i].SEP;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //10
                //                if (LstEEP[i].OCT != 0)
                //                {
                //                    Proqty = LstEEP[i].OCT;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //11
                //                if (LstEEP[i].NOV != 0)
                //                {
                //                    Proqty = LstEEP[i].NOV;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //12
                //                if (LstEEP[i].DEC != 0)
                //                {
                //                    Proqty = LstEEP[i].DEC;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //01
                //                if (LstEEP[i].JAN != 0)
                //                {
                //                    Proqty = LstEEP[i].JAN;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //02
                //                if (LstEEP[i].FEB != 0)
                //                {
                //                    Proqty = LstEEP[i].FEB;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //03
                //                if (LstEEP[i].MAR != 0)
                //                {
                //                    Proqty = LstEEP[i].MAR;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //   04
                //                if (LstEEP[i].APR != 0)
                //                {
                //                    Proqty = LstEEP[i].APR;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //   05
                //                if (LstEEP[i].MAY != 0)
                //                {
                //                    Proqty = LstEEP[i].MAY;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //                //   06
                //                if (LstEEP[i].JUN != 0)
                //                {
                //                    Proqty = LstEEP[i].JUN;
                //                }
                //                else
                //                {
                //                    Proqty = getProQty(date);
                //                }
                //            }
                //            if (LstEEP[i].TYPE == "Escaping PPM to Prod.")
                //            {
                //                if (Qty == 0 && Proqty == 0)
                //                {
                //                    EEPQty = 0;
                //                }
                //                else
                //                {
                //                    EEPQty = Math.Round(((Proqty / Qty) * 1000000), 1);
                //                }
                //            }
                //            //Set value to list
                //            foreach (var item1 in lstMonth)
                //            {
                //                if (item1 == "JUL")
                //                {
                //                    LstEEP[0].JUL = Qty;
                //                    LstEEP[1].JUL = Proqty;
                //                    LstEEP[2].JUL = EEPQty;
                //                }
                //                if (item1 == "AUG")
                //                {
                //                    LstEEP[0].AUG = Qty;
                //                    LstEEP[1].AUG = Proqty;
                //                    LstEEP[2].AUG = EEPQty;
                //                }
                //                if (item1 == "SEP")
                //                {
                //                    LstEEP[0].SEP = Qty;
                //                    LstEEP[1].SEP = Proqty;
                //                    LstEEP[2].SEP = EEPQty;
                //                }
                //                if (item1 == "OCT")
                //                {
                //                    LstEEP[0].OCT = Qty;
                //                    LstEEP[1].OCT = Proqty;
                //                    LstEEP[2].OCT = EEPQty;
                //                }
                //                if (item1 == "NOV")
                //                {
                //                    LstEEP[0].NOV = Qty;
                //                    LstEEP[1].NOV = Proqty;
                //                    LstEEP[2].NOV = EEPQty;
                //                }
                //                if (item1 == "DEC")
                //                {
                //                    LstEEP[0].DEC = Qty;
                //                    LstEEP[1].DEC = Proqty;
                //                    LstEEP[2].DEC = EEPQty;
                //                }
                //                if (item1 == "JAN")
                //                {
                //                    LstEEP[0].JAN = Qty;
                //                    LstEEP[1].JAN = Proqty;
                //                    LstEEP[2].JAN = EEPQty;
                //                }
                //                if (item1 == "FEB")
                //                {
                //                    LstEEP[0].FEB = Qty;
                //                    LstEEP[1].FEB = Proqty;
                //                    LstEEP[2].FEB = EEPQty;
                //                }
                //                if (item1 == "APR")
                //                {
                //                    LstEEP[0].APR = Qty;
                //                    LstEEP[1].APR = Proqty;
                //                    LstEEP[2].APR = EEPQty;
                //                }
                //                if (item1 == "MAY")
                //                {
                //                    LstEEP[0].MAY = Qty;
                //                    LstEEP[1].MAY = Proqty;
                //                    LstEEP[2].MAY = EEPQty;
                //                }
                //                if (item1 == "JUNE")
                //                {
                //                    LstEEP[0].JUN = Qty;
                //                    LstEEP[1].JUN = Proqty;
                //                    LstEEP[2].JUN = EEPQty;
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion
            }

            else
            {
                EEPreportviewModel listdate1 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Qty issued to Prod. (K)",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate1);

                EEPreportviewModel listdate2 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escaping to Prod. Qty",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate2);

                EEPreportviewModel listdate3 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escaping PPM to Prod.",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate3);

                EEPreportviewModel listdate4 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escapping PPM (Target)",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };

                LstEEP.Add(listdate4);
                //thang 7
                string year = "07/" + yearNOW;
                DateTime date = DateTime.Parse(year);
                double Qty07 = getQtyisue(date);
                LstEEP[0].JUL = Qty07;
                double Proqty07 = getProQty(date);
                LstEEP[1].JUL = Proqty07;
                double EEPQty = 0;
                if (Qty07 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty07 / Qty07) * 1000000), 1);
                }
                LstEEP[2].JUL = EEPQty;
                //thang 8
                string year08 = "08/" + yearNOW;
                DateTime date08 = DateTime.Parse(year08);
                double Qty08 = getQtyisue(date08);
                LstEEP[0].AUG = Qty08;
                double Proqty08 = getProQty(date08);
                LstEEP[1].AUG = Proqty08;
                if (Qty08 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty08 / Qty08) * 1000000), 1);
                }
                LstEEP[2].AUG = EEPQty;
                //thang 9
                string year09 = "09/" + yearNOW;
                DateTime date09 = DateTime.Parse(year09);
                double Qty09 = getQtyisue(date09);
                LstEEP[0].SEP = Qty09;
                double Proqty09 = getProQty(date09);
                LstEEP[1].SEP = Proqty09;
                if (Qty09 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty09 / Qty09) * 1000000), 1);
                }
                LstEEP[2].SEP = EEPQty;
                //thang 10
                string year10 = "10/" + yearNOW;
                DateTime date10 = DateTime.Parse(year10);
                double Qty10 = getQtyisue(date10);
                LstEEP[0].OCT = Qty10;
                double Proqty10 = getProQty(date10);
                LstEEP[1].OCT = Proqty10;
                if (Qty10 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty10 / Qty10) * 1000000), 1);
                }
                LstEEP[2].OCT = EEPQty;
                //thang 11
                string year11 = "11/" + yearNOW;
                DateTime date11 = DateTime.Parse(year11);
                double Qty11 = getQtyisue(date11);
                LstEEP[0].NOV = Qty11;
                double Proqty11 = getProQty(date11);
                LstEEP[1].NOV = Proqty11;
                if (Qty11 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty11 / Qty11) * 1000000), 1);
                }
                LstEEP[2].NOV = EEPQty;
                //thang 12
                string year12 = "12/" + yearNOW;
                DateTime date12 = DateTime.Parse(year12);
                double Qty12 = getQtyisue(date12);
                LstEEP[0].DEC = Qty12;
                double Proqty12 = getProQty(date12);
                LstEEP[1].DEC = Proqty12;
                if (Qty12 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty12 / Qty12) * 1000000), 1);
                }
                LstEEP[2].DEC = EEPQty;
                //thang 01
                string year01 = "01/" + yearselect;
                DateTime date01 = DateTime.Parse(year01);
                double Qty01 = getQtyisue(date01);
                LstEEP[0].JAN = Qty01;
                double Proqty01 = getProQty(date01);
                LstEEP[1].JAN = Proqty01;
                if (Qty01 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty01 / Qty01) * 1000000), 1);
                }
                LstEEP[2].JAN = EEPQty;
                //thang 02
                string year02 = "02/" + yearselect;
                DateTime date02 = DateTime.Parse(year02);
                double Qty02 = getQtyisue(date02);
                LstEEP[0].FEB = Qty02;
                double Proqty02 = getProQty(date02);
                LstEEP[1].FEB = Proqty02;
                if (Qty02 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty02 / Qty02) * 1000000), 1);
                }
                LstEEP[2].FEB = EEPQty;
                //thang 03
                string year03 = "03/" + yearselect;
                DateTime date03 = DateTime.Parse(year03);
                double Qty03 = getQtyisue(date03);
                LstEEP[0].MAR = Qty03;
                double Proqty03 = getProQty(date03);
                LstEEP[1].MAR = Proqty03;
                if (Qty03 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty03 / Qty03) * 1000000), 1);
                }
                LstEEP[2].MAR = EEPQty;
                //thang 04
                string year04 = "04/" + yearselect;
                DateTime date04 = DateTime.Parse(year04);
                double Qty04 = getQtyisue(date04);
                LstEEP[0].APR = Qty04;
                double Proqty04 = getProQty(date04);
                LstEEP[1].APR = Proqty04;
                if (Qty04 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty04 / Qty04) * 1000000), 1);
                }
                LstEEP[2].APR = EEPQty;
                //thang 05
                string year05 = "05/" + yearselect;
                DateTime date05 = DateTime.Parse(year05);
                double Qty05 = getQtyisue(date05);
                LstEEP[0].MAY = Qty05;
                double Proqty05 = getProQty(date05);
                LstEEP[1].MAY = Proqty05;
                if (Qty05 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty05 / Qty05) * 1000000), 1);
                }
                LstEEP[2].MAY = EEPQty;
                //thang 06
                string year06 = "06/" + yearselect;
                DateTime date06 = DateTime.Parse(year06);
                double Qty06 = getQtyisue(date06);
                LstEEP[0].JUN = Qty06;
                double Proqty06 = getProQty(date06);
                LstEEP[1].JUN = Proqty06;
                if (Qty06 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty06 / Qty06) * 1000000), 1);
                }
                LstEEP[2].JUN = EEPQty;

            }
            ////GET LIST yEAR ago
            List<EEP_REPORT> lstyear1 = _db.EEP_REPORT.Where(n => n.FY == yearnam1 && (n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)")).ToList();
            ////two year ago 
            List<EEP_REPORT> lstyear2 = _db.EEP_REPORT.Where(n => n.FY == yearnam2 && (n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)")).ToList();
            //three year
            List<EEP_REPORT> lstyear3 = _db.EEP_REPORT.Where(n => n.FY == yearnam3 && (n.TYPE == "Qty issued to Prod. (K)" || n.TYPE == "Escaping to Prod. Qty" || n.TYPE == "Escaping PPM to Prod." || n.TYPE == "Escapping PPM (Target)")).ToList();
            List<EEPreportviewModel> list = new List<EEPreportviewModel>();
            EEPreportviewModel a1 = LstEEP.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
            double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
            a1.FYCurrent = qtysum;
            EEP_REPORT a11 = lstyear1.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
            EEP_REPORT a21 = lstyear2.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
            EEP_REPORT a23 = lstyear3.Where(x => x.TYPE == "Qty issued to Prod. (K)").FirstOrDefault();
            double qtyyear1;
            double qtyyear2;
            double qtyyear3;
            if (a11 != null)
            {
                qtyyear1 = Math.Round((a11.JUL + a11.AUG + a11.SEP + a11.OCT + a11.NOV + a11.DEC + a11.JAN + a11.FEB + a11.MAR + a11.APR + a11.APR + a11.MAY + a11.JUN), 1);
            }
            else
            {
                qtyyear1 = 0;
            }
            if (a21 != null)
            {
                qtyyear2 = Math.Round((a21.JUL + a21.AUG + a21.SEP + a21.OCT + a21.NOV + a21.DEC + a21.JAN + a21.FEB + a21.MAR + a21.APR + a21.APR + a21.MAY + a21.JUN), 1);
            }
            else
            {
                qtyyear2 = 0;
            }
            if (a23 != null)
            {
                qtyyear3 = Math.Round((a23.JUL + a23.AUG + a23.SEP + a23.OCT + a23.NOV + a23.DEC + a23.JAN + a23.FEB + a23.MAR + a23.APR + a23.APR + a23.MAY + a23.JUN), 1);
            }
            else
            {
                qtyyear3 = 0;
            }
            a1.FY1 = qtyyear1;
            a1.FY2 = qtyyear2;
            a1.FY3 = qtyyear3;
            list.Add(a1);
            EEPreportviewModel a2 = LstEEP.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();

            double qtytwo = Math.Round((a2.JUL + a2.AUG + a2.SEP + a2.OCT + a2.NOV + a2.DEC + a2.JAN + a2.FEB + a2.MAR + a2.APR + a2.APR + a2.MAY + a2.JUN), 1);
            a2.FYCurrent = qtytwo;
            EEP_REPORT a22 = lstyear1.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();
            EEP_REPORT a32 = lstyear2.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();
            EEP_REPORT a42 = lstyear3.Where(x => x.TYPE == "Escaping to Prod. Qty").FirstOrDefault();
            double qtyfyear1;
            double qtyfyear2;
            double qtyfyear3;
            //NAM 1
            if (a22 != null)
            {
                qtyfyear1 = Math.Round((a22.JUL + a22.AUG + a22.SEP + a22.OCT + a22.NOV + a22.DEC + a22.JAN + a22.FEB + a22.MAR + a22.APR + a22.APR + a22.MAY + a22.JUN), 1);
            }
            else
            {
                qtyfyear1 = 0;
            }
            //NAM 2
            if (a32 != null)
            {
                qtyfyear2 = Math.Round((a32.JUL + a32.AUG + a32.SEP + a32.OCT + a32.NOV + a32.DEC + a32.JAN + a32.FEB + a32.MAR + a32.APR + a32.APR + a32.MAY + a32.JUN), 1);
            }
            else
            {
                qtyfyear2 = 0;
            }
            //NAM3
            if (a42 != null)
            {
                qtyfyear3 = Math.Round((a42.JUL + a42.AUG + a42.SEP + a42.OCT + a42.NOV + a42.DEC + a42.JAN + a42.FEB + a42.MAR + a42.APR + a42.APR + a42.MAY + a42.JUN), 1);
            }
            else
            {
                qtyfyear3 = 0;
            }
            a2.FY1 = qtyfyear1;
            a2.FY2 = qtyfyear2;
            a2.FY3 = qtyfyear3;
            list.Add(a2);
            EEPreportviewModel a3 = LstEEP.Where(x => x.TYPE == "Escaping PPM to Prod.").FirstOrDefault();
            double qtypercent = Math.Round(((qtytwo / qtysum) * 1000000), 1);
            a3.FYCurrent = qtypercent;
            double qtyFy1;
            //NAM1
            if (qtyyear1 != 0 && qtyfyear1 != 0)
            {
                qtyFy1 = Math.Round(((qtyfyear1 / qtyyear1) * 1000000), 1);
            }
            else
            {
                qtyFy1 = 0;
            }
            //NAM 2
            double qtyFy2;
            if (qtyyear2 != 0 && qtyfyear2 != 0)
            {
                qtyFy2 = Math.Round(((qtyfyear2 / qtyyear2) * 1000000), 1);
            }
            else
            {
                qtyFy2 = 0;
            }
            //NAM3
            double qtyFy3;
            if (qtyyear3 != 0 && qtyfyear3 != 0)
            {
                qtyFy3 = Math.Round(((qtyfyear3 / qtyyear3) * 1000000), 1);
            }
            else
            {
                qtyFy3 = 0;
            }
            a3.FY1 = qtyFy1;
            a3.FY2 = qtyFy2;
            a3.FY3 = qtyFy3;
            list.Add(a3);
            EEPreportviewModel a4 = LstEEP.Where(x => x.TYPE == "Escapping PPM (Target)").FirstOrDefault();
            a4.FY1 = 0;
            a4.FY2 = 0;
            a4.FY3 = 0;
            list.Add(a4);
            return list;
        }

        public List<EEPreportviewModel> GetdataEEPtocomponent(string yearselect)
        {
            int yearNOW = int.Parse(yearselect) - 1;
            string[] type = new string[] { "Qty issued to Component Line(K)", "Escaping to Component Line Qty", "Escaping PPM to Component Line", "Escapping to Component PPM (Target)" };
            List<EEPreportviewModel> LstEEP = _db.EEP_REPORT.Where(n => n.FY == yearselect && (n.TYPE == "Qty issued to Component Line(K)" || n.TYPE == "Escaping to Component Line Qty" || n.TYPE == "Escaping PPM to Component Line" || n.TYPE == "Escapping to Component PPM (Target)")).
                                             Select(n => new EEPreportviewModel
                                             {
                                                 FY = n.FY,
                                                 TYPE = n.TYPE,
                                                 FY1 = 0,
                                                 FY2 = 0,
                                                 FY3 = 0,
                                                 FY4 = 0,
                                                 FYCurrent = 0,
                                                 JUL = n.JUL,
                                                 AUG = n.AUG,
                                                 SEP = n.SEP,
                                                 OCT = n.OCT,
                                                 NOV = n.NOV,
                                                 DEC = n.DEC,
                                                 JAN = n.JAN,
                                                 FEB = n.FEB,
                                                 MAR = n.MAR,
                                                 APR = n.APR,
                                                 MAY = n.MAY,
                                                 JUN = n.JUN
                                             }).ToList();
            if (LstEEP.Count > 0)
            {
                foreach (var item in LstEEP)
                {
                    if (yearNOW == (int.Parse(yearselect) - 1))
                    {
                        #region "Month 07"
                        string year07 = "07/" + yearNOW;
                        DateTime date07 = DateTime.Parse(year07);
                        double Qty07 = 0;
                        double Proqty07 = 0;
                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.JUL != 0)
                            {
                                Qty07 = item.JUL;
                            }
                            else
                            {
                                Qty07 = getQtyisuetocomponent(date07);
                            }
                            LstEEP[3].JUL = Qty07;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.JUL != 0)
                            {
                                Proqty07 = item.JUL;

                            }
                            else
                            {
                                Proqty07 = getProQtytoComponent(date07);
                            }
                            LstEEP[1].JUL = Proqty07;
                        }
                        #endregion

                        #region "Month 08"
                        string year08 = "08/" + yearNOW;
                        DateTime date08 = DateTime.Parse(year08);
                        double Qty08 = 0;
                        double Proqty08 = 0;


                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.AUG != 0)
                            {
                                Qty08 = item.AUG;
                            }
                            else
                            {
                                Qty08 = getQtyisuetocomponent(date08);
                            }
                            LstEEP[3].AUG = Qty08;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.AUG != 0)
                            {
                                Proqty08 = item.AUG;
                            }
                            else
                            {
                                Proqty08 = getProQtytoComponent(date08);
                            }
                            LstEEP[1].AUG = Proqty08;
                        }
                        #endregion

                        #region "Month 09"
                        string year09 = "09/" + yearNOW;
                        DateTime date09 = DateTime.Parse(year09);
                        double Qty09 = 0;
                        double Proqty09 = 0;


                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.SEP != 0)
                            {
                                Qty09 = item.SEP;
                            }
                            else
                            {
                                Qty09 = getQtyisuetocomponent(date09);
                            }
                            LstEEP[3].SEP = Qty09;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.SEP != 0)
                            {
                                Proqty09 = item.SEP;

                            }
                            else
                            {
                                Proqty09 = getProQtytoComponent(date09);
                            }
                            LstEEP[1].SEP = Proqty09;
                        }
                        #endregion

                        #region "Month 10"
                        string year10 = "10/" + yearNOW;
                        DateTime date10 = DateTime.Parse(year10);
                        double Qty10 = 0;
                        double Proqty10 = 0;


                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.OCT != 0)
                            {
                                Qty10 = item.OCT;
                            }
                            else
                            {
                                Qty10 = getQtyisuetocomponent(date10);
                            }
                            LstEEP[3].OCT = Qty10;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.OCT != 0)
                            {
                                Proqty10 = item.OCT;

                            }
                            else
                            {
                                Proqty10 = getProQtytoComponent(date10);
                            }
                            LstEEP[1].OCT = Proqty10;
                        }
                        #endregion

                        #region "Month 11"
                        string year11 = "11/" + yearNOW;
                        DateTime date11 = DateTime.Parse(year11);
                        double Qty11 = 0;
                        double Proqty11 = 0;


                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.SEP != 0)
                            {
                                Qty11 = item.SEP;
                            }
                            else
                            {
                                Qty11 = getQtyisuetocomponent(date11);
                            }
                            LstEEP[3].SEP = Qty11;

                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.SEP != 0)
                            {
                                Proqty11 = item.SEP;

                            }
                            else
                            {
                                Proqty11 = getProQtytoComponent(date11);
                            }
                            LstEEP[1].SEP = Proqty11;
                        }
                        #endregion

                        #region "Month 12"
                        string year12 = "12/" + yearNOW;
                        DateTime date12 = DateTime.Parse(year12);
                        double Qty12 = 0;
                        double Proqty12 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.DEC != 0)
                            {
                                Qty12 = item.DEC;
                            }
                            else
                            {
                                Qty12 = getQtyisuetocomponent(date12);
                            }
                            LstEEP[3].DEC = Qty12;

                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.DEC != 0)
                            {
                                Proqty12 = item.DEC;

                            }
                            else
                            {
                                Proqty12 = getProQtytoComponent(date12);
                            }
                            LstEEP[1].DEC = Proqty12;
                        }

                        #endregion

                        #region "Month 01"
                        string year01 = "01/" + (yearNOW + 1);
                        DateTime date01 = DateTime.Parse(year01);
                        double Qty01 = 0;
                        double Proqty01 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.JAN != 0)
                            {
                                Qty01 = item.JAN;
                            }
                            else
                            {
                                Qty01 = getQtyisuetocomponent(date01);
                            }
                            LstEEP[3].JAN = Qty01;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.JAN != 0)
                            {
                                Proqty01 = item.JAN;

                            }
                            else
                            {
                                Proqty01 = getProQtytoComponent(date01);
                            }
                            LstEEP[1].JAN = Proqty01;
                        }

                        #endregion

                        #region "Month 02"
                        string year02 = "02/" + (yearNOW + 1);
                        DateTime date02 = DateTime.Parse(year02);
                        double Qty02 = 0;
                        double Proqty02 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.FEB != 0)
                            {
                                Qty02 = item.FEB;
                            }
                            else
                            {
                                Qty02 = getQtyisuetocomponent(date02);
                            }
                            LstEEP[3].FEB = Qty02;

                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.FEB != 0)
                            {
                                Proqty02 = item.FEB;

                            }
                            else
                            {
                                Proqty02 = getProQtytoComponent(date02);
                            }
                            LstEEP[1].FEB = Proqty02;
                        }

                        #endregion

                        #region "Month 03"
                        string year03 = "03/" + (yearNOW + 1);
                        DateTime date03 = DateTime.Parse(year03);
                        double Qty03 = 0;
                        double Proqty03 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.MAR != 0)
                            {
                                Qty03 = item.MAR;
                            }
                            else
                            {
                                Qty03 = getQtyisuetocomponent(date03);
                            }
                            LstEEP[3].MAR = Qty03;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.MAR != 0)
                            {
                                Proqty03 = item.MAR;

                            }
                            else
                            {
                                Proqty03 = getProQtytoComponent(date03);
                            }
                            LstEEP[1].MAR = Proqty03;
                        }

                        #endregion

                        #region "Month 04"
                        string year04 = "04/" + (yearNOW + 1);
                        DateTime date04 = DateTime.Parse(year04);
                        double Qty04 = 0;
                        double Proqty04 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.APR != 0)
                            {
                                Qty04 = item.APR;
                            }
                            else
                            {
                                Qty04 = getQtyisuetocomponent(date04);
                            }
                            LstEEP[3].APR = Qty04;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.APR != 0)
                            {
                                Proqty04 = item.APR;

                            }
                            else
                            {
                                Proqty04 = getProQtytoComponent(date04);
                            }
                            LstEEP[1].APR = Proqty04;

                        }
                        #endregion

                        #region "Month 05"
                        string year05 = "05/" + (yearNOW + 1);
                        DateTime date05 = DateTime.Parse(year05);
                        double Qty05 = 0;
                        double Proqty05 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.MAY != 0)
                            {
                                Qty05 = item.MAY;
                            }
                            else
                            {
                                Qty05 = getQtyisuetocomponent(date05);
                            }
                            LstEEP[3].MAY = Qty05;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.MAY != 0)
                            {
                                Proqty05 = item.MAY;

                            }
                            else
                            {
                                Proqty05 = getProQtytoComponent(date05);
                            }
                            LstEEP[1].MAY = Proqty05;
                        }
                        #endregion

                        #region "Month 06"
                        string year06 = "06/" + (yearNOW + 1);
                        DateTime date06 = DateTime.Parse(year06);
                        double Qty06 = 0;
                        double Proqty06 = 0;

                        if (item.TYPE == "Qty issued to Component Line(K)")
                        {
                            if (item.JUN != 0)
                            {
                                Qty06 = item.JUN;
                            }
                            else
                            {
                                Qty06 = getQtyisuetocomponent(date06);
                            }
                            LstEEP[3].JUN = Qty06;
                        }
                        if (item.TYPE == "Escaping to Component Line Qty")
                        {
                            if (item.JUN != 0)
                            {
                                Proqty06 = item.JUN;

                            }
                            else
                            {
                                Proqty06 = getProQtytoComponent(date06);
                            }
                            LstEEP[1].JUN = Proqty06;
                        }
                        #endregion
                    }

                }

                //Get Escaping PPM to Component Line
                foreach (var item in LstEEP)
                {
                    #region "Month 07"
                    double EEPQty07 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.JUL != 0)
                        {
                            EEPQty07 = item.JUL;
                        }
                        else if (LstEEP[1].JUL != 0 && LstEEP[3].JUL != 0)
                        {
                            EEPQty07 = Math.Round(((LstEEP[1].JUL / LstEEP[3].JUL) * 1000000), 1);
                        }
                        LstEEP[0].JUL = EEPQty07;
                    }
                    #endregion

                    #region "Month 08"
                    double EEPQty08 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.AUG != 0)
                        {
                            EEPQty08 = item.AUG;
                        }
                        else if (LstEEP[1].AUG != 0 && LstEEP[3].AUG != 0)
                        {
                            EEPQty08 = Math.Round(((LstEEP[1].AUG / LstEEP[3].AUG) * 1000000), 1);
                        }
                        LstEEP[0].AUG = EEPQty08;
                    }
                    #endregion

                    #region "Month 09"
                    double EEPQty09 = 0;

                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.SEP != 0)
                        {
                            EEPQty09 = item.SEP;
                        }
                        else if ((LstEEP[1].SEP != 0 && LstEEP[3].SEP != 0))
                        {
                            EEPQty09 = Math.Round(((LstEEP[1].SEP / LstEEP[3].SEP) * 1000000), 1);
                        }
                        LstEEP[0].SEP = EEPQty09;

                    }
                    #endregion

                    #region "Month 10"
                    double EEPQty10 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.OCT != 0)
                        {
                            EEPQty10 = item.OCT;
                        }
                        else if ((LstEEP[1].OCT != 0 && LstEEP[3].OCT != 0))
                        {
                            EEPQty10 = Math.Round(((LstEEP[1].OCT / LstEEP[3].OCT) * 1000000), 1);
                        }
                        LstEEP[0].OCT = EEPQty10;
                    }
                    #endregion

                    #region "Month 11"
                    double EEPQty11 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.NOV != 0)
                        {
                            EEPQty11 = item.NOV;
                        }
                        else if ((LstEEP[1].NOV != 0 && LstEEP[3].NOV != 0))
                        {
                            EEPQty11 = Math.Round(((LstEEP[1].NOV / LstEEP[3].NOV) * 1000000), 1);
                        }
                        LstEEP[0].NOV = EEPQty11;
                    }
                    #endregion

                    #region "Month 12"
                    double EEPQty12 = 0;

                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.DEC != 0)
                        {
                            EEPQty12 = item.DEC;
                        }
                        else if ((LstEEP[1].DEC != 0 && LstEEP[3].DEC != 0))
                        {
                            EEPQty12 = Math.Round(((LstEEP[1].DEC / LstEEP[3].DEC) * 1000000), 1);
                        }

                        LstEEP[0].DEC = EEPQty12;
                    }
                    #endregion

                    #region "Month 01"
                    double EEPQty01 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.JAN != 0)
                        {
                            EEPQty01 = item.JAN;
                        }
                        else if ((LstEEP[1].JAN != 0 && LstEEP[3].JAN != 0))
                        {
                            EEPQty01 = Math.Round(((LstEEP[1].JAN / LstEEP[3].JAN) * 1000000), 1);
                        }
                        LstEEP[0].JAN = EEPQty01;
                    }
                    #endregion

                    #region "Month 02"
                    double EEPQty02 = 0;

                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.FEB != 0)
                        {
                            EEPQty02 = item.FEB;
                        }
                        else if ((LstEEP[1].FEB != 0 && LstEEP[3].FEB != 0))
                        {
                            EEPQty02 = Math.Round(((LstEEP[1].FEB / LstEEP[3].FEB) * 1000000), 1);
                        }
                        LstEEP[0].FEB = EEPQty02;
                    }
                    #endregion

                    #region "Month 03"
                    double EEPQty03 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.MAR != 0)
                        {
                            EEPQty03 = item.MAR;
                        }
                        else if ((LstEEP[1].MAR != 0 && LstEEP[3].MAR != 0))
                        {
                            EEPQty03 = Math.Round(((LstEEP[1].MAR / LstEEP[3].MAR) * 1000000), 1);
                        }
                        LstEEP[0].MAR = EEPQty03;
                    }
                    #endregion

                    #region "Month 04"
                    double EEPQty04 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.APR != 0)
                        {
                            EEPQty03 = item.APR;
                        }
                        else if ((LstEEP[1].APR != 0 && LstEEP[3].APR != 0))
                        {
                            EEPQty04 = Math.Round(((LstEEP[1].APR / LstEEP[3].APR) * 1000000), 1);
                        }
                        LstEEP[0].APR = EEPQty04;
                    }
                    #endregion

                    #region "Month 05"
                    double EEPQty05 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.MAY != 0)
                        {
                            EEPQty03 = item.MAY;
                        }
                        else if ((LstEEP[1].MAY != 0 && LstEEP[3].MAY != 0))
                        {
                            EEPQty05 = Math.Round(((LstEEP[1].MAY / LstEEP[3].MAY) * 1000000), 1);
                        }
                        LstEEP[0].MAY = EEPQty05;
                    }
                    #endregion
                    #region "Month 06"
                    double EEPQty06 = 0;
                    if (item.TYPE == "Escaping PPM to Component Line")
                    {
                        if (item.JUN != 0)
                        {
                            EEPQty06 = item.JUN;
                        }
                        else if ((LstEEP[1].JUN != 0 && LstEEP[3].JUN != 0))
                        {
                            EEPQty06 = Math.Round(((LstEEP[1].JUN / LstEEP[3].JUN) * 1000000), 1);
                        }
                        LstEEP[0].JUN = EEPQty06;
                    }
                    #endregion
                }
            }
            else
            {
                EEPreportviewModel listdate1 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Qty issued to Component Line(K)",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate1);

                EEPreportviewModel listdate2 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escaping to Component Line Qty",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate2);

                EEPreportviewModel listdate3 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escaping PPM to Component Line",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate3);

                EEPreportviewModel listdate4 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escapping to Component PPM (Target)",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate4);
                //thang 7
                string year07 = "07/" + (int.Parse(yearselect) - 1);
                DateTime date07 = DateTime.Parse(year07);
                double Qty07 = getQtyisuetocomponent(date07);
                LstEEP[0].JUL = Qty07;
                double Proqty07 = getProQtytoComponent(date07);
                LstEEP[1].JUL = Proqty07;
                double EEPQty = 0;
                if (Qty07 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty07 / Qty07) * 1000000), 1);
                }
                LstEEP[2].JUL = EEPQty;
                //thang 8
                string year08 = "08/" + (int.Parse(yearselect) - 1);
                DateTime date08 = DateTime.Parse(year08);
                double Qty08 = getQtyisuetocomponent(date08);
                LstEEP[0].AUG = Qty08;
                double Proqty08 = getProQtytoComponent(date08);
                LstEEP[1].AUG = Proqty08;
                if (Qty08 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty08 / Qty08) * 1000000), 1);
                }
                LstEEP[2].AUG = EEPQty;
                //thang 9
                string year09 = "09/" + (int.Parse(yearselect) - 1);
                DateTime date09 = DateTime.Parse(year09);
                double Qty09 = getQtyisuetocomponent(date09);
                LstEEP[0].SEP = Qty09;
                double Proqty09 = getProQtytoComponent(date09);
                LstEEP[1].SEP = Proqty09;
                if (Qty09 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty09 / Qty09) * 1000000), 1);
                }
                LstEEP[2].SEP = EEPQty;
                //thang 10
                string year10 = "10/" + (int.Parse(yearselect) - 1);
                DateTime date10 = DateTime.Parse(year10);
                double Qty10 = getQtyisuetocomponent(date10);
                LstEEP[0].OCT = Qty10;
                double Proqty10 = getProQtytoComponent(date10);
                LstEEP[1].OCT = Proqty10;
                if (Qty10 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty10 / Qty10) * 1000000), 1);
                }
                LstEEP[2].OCT = EEPQty;
                //thang 11
                string year11 = "11/" + (int.Parse(yearselect) - 1);
                DateTime date11 = DateTime.Parse(year11);
                double Qty11 = getQtyisuetocomponent(date11);
                LstEEP[0].NOV = Qty11;
                double Proqty11 = getProQtytoComponent(date11);
                LstEEP[1].NOV = Proqty11;
                if (Qty11 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty11 / Qty11) * 1000000), 1);
                }
                LstEEP[2].NOV = EEPQty;
                //thang 12
                string year12 = "12/" + (int.Parse(yearselect) - 1);
                DateTime date12 = DateTime.Parse(year12);
                double Qty12 = getQtyisuetocomponent(date12);
                LstEEP[0].DEC = Qty12;
                double Proqty12 = getProQtytoComponent(date12);
                LstEEP[1].DEC = Proqty12;
                if (Qty12 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty12 / Qty12) * 1000000), 1);
                }
                LstEEP[2].DEC = EEPQty;
                //thang 01
                string year01 = "01/" + yearselect;
                DateTime date01 = DateTime.Parse(year01);
                double Qty01 = getQtyisuetocomponent(date01);
                LstEEP[0].JAN = Qty01;
                double Proqty01 = getProQtytoComponent(date01);
                LstEEP[1].JAN = Proqty01;
                if (Qty01 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty01 / Qty01) * 1000000), 1);
                }
                LstEEP[2].JAN = EEPQty;
                //thang 02
                string year02 = "02/" + yearselect;
                DateTime date02 = DateTime.Parse(year02);
                double Qty02 = getQtyisuetocomponent(date02);
                LstEEP[0].FEB = Qty02;
                double Proqty02 = getProQtytoComponent(date02);
                LstEEP[1].FEB = Proqty02;
                if (Qty02 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty02 / Qty02) * 1000000), 1);
                }
                LstEEP[2].FEB = EEPQty;
                //thang 03
                string year03 = "03/" + yearselect;
                DateTime date03 = DateTime.Parse(year03);
                double Qty03 = getQtyisuetocomponent(date03);
                LstEEP[0].MAR = Qty03;
                double Proqty03 = getProQtytoComponent(date03);
                LstEEP[1].MAR = Proqty03;
                if (Qty03 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty03 / Qty03) * 1000000), 1);
                }
                LstEEP[2].MAR = EEPQty;
                //thang 04
                string year04 = "04/" + yearselect;
                DateTime date04 = DateTime.Parse(year04);
                double Qty04 = getQtyisuetocomponent(date04);
                LstEEP[0].APR = Qty04;
                double Proqty04 = getProQtytoComponent(date04);
                LstEEP[1].APR = Proqty04;
                if (Qty04 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty04 / Qty04) * 1000000), 1);
                }
                LstEEP[2].APR = EEPQty;
                //thang 05
                string year05 = "05/" + yearselect;
                DateTime date05 = DateTime.Parse(year05);
                double Qty05 = getQtyisuetocomponent(date05);
                LstEEP[0].MAY = Qty05;
                double Proqty05 = getProQtytoComponent(date05);
                LstEEP[1].MAY = Proqty05;
                if (Qty05 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty05 / Qty05) * 1000000), 1);
                }
                LstEEP[2].MAY = EEPQty;
                //thang 06
                string year06 = "06/" + yearselect;
                DateTime date06 = DateTime.Parse(year06);
                double Qty06 = getQtyisuetocomponent(date06);
                LstEEP[0].JUN = Qty06;
                double Proqty06 = getProQtytoComponent(date06);
                LstEEP[1].JUN = Proqty06;
                if (Qty06 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty06 / Qty06) * 1000000), 1);
                }
                LstEEP[2].JUN = EEPQty;
            }

            List<EEPreportviewModel> listRP = new List<EEPreportviewModel>();
            EEPreportviewModel a1 = LstEEP.Where(x => x.TYPE == "Qty issued to Component Line(K)").FirstOrDefault();
            double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
            a1.FYCurrent = qtysum;
            listRP.Add(a1);
            EEPreportviewModel a2 = LstEEP.Where(x => x.TYPE == "Escaping to Component Line Qty").FirstOrDefault();
            double qtytwo = Math.Round((a2.JUL + a2.AUG + a2.SEP + a2.OCT + a2.NOV + a2.DEC + a2.JAN + a2.FEB + a2.MAR + a2.APR + a2.APR + a2.MAY + a2.JUN), 1);
            a2.FYCurrent = qtytwo;
            listRP.Add(a2);
            EEPreportviewModel a3 = LstEEP.Where(x => x.TYPE == "Escaping PPM to Component Line").FirstOrDefault();
            double qtypercent = 0;
            if (qtytwo != 0 && qtysum != 0)
            {
                qtypercent = Math.Round(((qtytwo / qtysum) * 1000000), 1);
            }
            else
            {
                qtypercent = 0;
            }
            a3.FYCurrent = qtypercent;
            listRP.Add(a3);
            EEPreportviewModel a4 = LstEEP.Where(x => x.TYPE == "Escapping to Component PPM (Target)").FirstOrDefault();
            listRP.Add(a4);

            return listRP;
        }
        public List<EEPreportviewModel> GetdataEEPtoSystem(string yearselect)
        {
            int yearNOW = int.Parse(yearselect) - 1;
            List<EEPreportviewModel> LstEEP = _db.EEP_REPORT.Where(n => n.FY == yearselect && (n.TYPE == "Qty issued to System Line(K)" || n.TYPE == "Escaping to System Line Qty" || n.TYPE == "Escaping PPM to System Line" || n.TYPE == "Escapping to System PPM (Target)")).
                                             Select(n => new EEPreportviewModel
                                             {
                                                 FY = n.FY,
                                                 TYPE = n.TYPE,
                                                 FY1 = 0,
                                                 FY2 = 0,
                                                 FY3 = 0,
                                                 FY4 = 0,
                                                 FYCurrent = 0,
                                                 JUL = n.JUL,
                                                 AUG = n.AUG,
                                                 SEP = n.SEP,
                                                 OCT = n.OCT,
                                                 NOV = n.NOV,
                                                 DEC = n.DEC,
                                                 JAN = n.JAN,
                                                 FEB = n.FEB,
                                                 MAR = n.MAR,
                                                 APR = n.APR,
                                                 MAY = n.MAY,
                                                 JUN = n.JUN
                                             }).ToList();
            if (LstEEP.Count > 0)
            {

                foreach (var item in LstEEP)
                {
                    if (yearNOW == (int.Parse(yearselect) - 1))
                    {
                        #region "Month 07"
                        string year07 = "07/" + yearNOW;
                        DateTime date07 = DateTime.Parse(year07);
                        double Qty07 = 0;
                        double Proqty07 = 0;
                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.JUL != 0)
                            {
                                Qty07 = item.JUL;
                            }
                            else
                            {
                                Qty07 = getQtyisuetosystem(date07);
                            }
                            LstEEP[3].JUL = Qty07;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.JUL != 0)
                            {
                                Proqty07 = item.JUL;

                            }
                            else
                            {
                                Proqty07 = getProQtytoSysTem(date07);
                            }
                        }

                        #endregion

                        #region "Month 08"
                        string year08 = "08/" + yearNOW;
                        DateTime date08 = DateTime.Parse(year08);
                        double Qty08 = 0;
                        double Proqty08 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.AUG != 0)
                            {
                                Qty08 = item.AUG;
                            }
                            else
                            {
                                Qty08 = getQtyisuetosystem(date08);
                            }
                            LstEEP[3].AUG = Qty08;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.AUG != 0)
                            {
                                Proqty08 = item.AUG;
                            }
                            else
                            {
                                Proqty08 = getProQtytoSysTem(date08);
                            }
                            LstEEP[1].AUG = Proqty08;
                        }

                        #endregion

                        #region "Month 09"
                        string year09 = "09/" + yearNOW;
                        DateTime date09 = DateTime.Parse(year09);
                        double Qty09 = 0;
                        double Proqty09 = 0;


                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.SEP != 0)
                            {
                                Qty09 = item.SEP;
                            }
                            else
                            {
                                Qty09 = getQtyisuetosystem(date09);
                            }
                            LstEEP[3].SEP = Qty09;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.SEP != 0)
                            {
                                Proqty09 = item.SEP;

                            }
                            else
                            {
                                Proqty09 = getProQtytoSysTem(date09);
                            }
                            LstEEP[1].SEP = Proqty09;
                        }

                        #endregion

                        #region "Month 10"
                        string year10 = "10/" + yearNOW;
                        DateTime date10 = DateTime.Parse(year10);
                        double Qty10 = 0;
                        double Proqty10 = 0;


                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.OCT != 0)
                            {
                                Qty10 = item.OCT;
                            }
                            else
                            {
                                Qty10 = getQtyisuetosystem(date10);
                            }
                            LstEEP[3].OCT = Qty10;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.OCT != 0)
                            {
                                Proqty10 = item.OCT;

                            }
                            else
                            {
                                Proqty10 = getProQtytoSysTem(date10);
                            }
                            LstEEP[1].OCT = Proqty10;
                        }

                        #endregion

                        #region "Month 11"
                        string year11 = "11/" + yearNOW;
                        DateTime date11 = DateTime.Parse(year11);
                        double Qty11 = 0;
                        double Proqty11 = 0;


                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.SEP != 0)
                            {
                                Qty11 = item.SEP;
                            }
                            else
                            {
                                Qty11 = getQtyisuetosystem(date11);
                            }
                            LstEEP[3].SEP = Qty11;

                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.SEP != 0)
                            {
                                Proqty11 = item.SEP;

                            }
                            else
                            {
                                Proqty11 = getProQtytoSysTem(date11);
                            }
                            LstEEP[1].SEP = Proqty11;
                        }

                        #endregion

                        #region "Month 12"
                        string year12 = "12/" + yearNOW;
                        DateTime date12 = DateTime.Parse(year12);
                        double Qty12 = 0;
                        double Proqty12 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.DEC != 0)
                            {
                                Qty12 = item.DEC;
                            }
                            else
                            {
                                Qty12 = getQtyisuetosystem(date12);
                            }
                            LstEEP[3].DEC = Qty12;

                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.DEC != 0)
                            {
                                Proqty12 = item.DEC;

                            }
                            else
                            {
                                Proqty12 = getProQtytoSysTem(date12);
                            }
                            LstEEP[1].DEC = Proqty12;
                        }

                        #endregion

                        #region "Month 01"
                        string year01 = "01/" + (yearNOW + 1);
                        DateTime date01 = DateTime.Parse(year01);
                        double Qty01 = 0;
                        double Proqty01 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.JAN != 0)
                            {
                                Qty01 = item.JAN;
                            }
                            else
                            {
                                Qty01 = getQtyisuetosystem(date01);
                            }
                            LstEEP[3].JAN = Qty01;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.JAN != 0)
                            {
                                Proqty01 = item.JAN;

                            }
                            else
                            {
                                Proqty01 = getProQtytoSysTem(date01);
                            }
                            LstEEP[1].JAN = Proqty01;
                        }
                        #endregion

                        #region "Month 02"
                        string year02 = "02/" + (yearNOW + 1);
                        DateTime date02 = DateTime.Parse(year02);
                        double EEPQty02 = 0;
                        double Qty02 = 0;
                        double Proqty02 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.FEB != 0)
                            {
                                Qty02 = item.FEB;
                            }
                            else
                            {
                                Qty02 = getQtyisuetosystem(date02);
                            }
                            LstEEP[3].FEB = Qty02;

                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.FEB != 0)
                            {
                                Proqty02 = item.FEB;

                            }
                            else
                            {
                                Proqty02 = getProQtytoSysTem(date02);
                            }
                            LstEEP[1].FEB = Proqty02;
                        }

                        #endregion

                        #region "Month 03"
                        string year03 = "03/" + (yearNOW + 1);
                        DateTime date03 = DateTime.Parse(year03);
                        double Qty03 = 0;
                        double Proqty03 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.MAR != 0)
                            {
                                Qty03 = item.MAR;
                            }
                            else
                            {
                                Qty03 = getQtyisuetosystem(date03);
                            }
                            LstEEP[3].MAR = Qty03;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.MAR != 0)
                            {
                                Proqty03 = item.MAR;

                            }
                            else
                            {
                                Proqty03 = getProQtytoSysTem(date03);
                            }
                            LstEEP[1].MAR = Proqty03;
                        }

                        #endregion

                        #region "Month 04"
                        string year04 = "04/" + (yearNOW + 1);
                        DateTime date04 = DateTime.Parse(year04);
                        double Qty04 = 0;
                        double Proqty04 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.APR != 0)
                            {
                                Qty04 = item.APR;
                            }
                            else
                            {
                                Qty04 = getQtyisuetosystem(date04);
                            }
                            LstEEP[3].APR = Qty04;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.APR != 0)
                            {
                                Proqty04 = item.APR;

                            }
                            else
                            {
                                Proqty04 = getProQtytoSysTem(date04);
                            }
                            LstEEP[1].APR = Proqty04;

                        }

                        #endregion

                        #region "Month 05"
                        string year05 = "05/" + (yearNOW + 1);
                        DateTime date05 = DateTime.Parse(year05);
                        double Qty05 = 0;
                        double Proqty05 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.MAY != 0)
                            {
                                Qty05 = item.MAY;
                            }
                            else
                            {
                                Qty05 = getQtyisuetosystem(date05);
                            }
                            LstEEP[3].MAY = Qty05;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.MAY != 0)
                            {
                                Proqty05 = item.MAY;

                            }
                            else
                            {
                                Proqty05 = getProQtytoSysTem(date05);
                            }
                            LstEEP[1].MAY = Proqty05;
                        }

                        #endregion

                        #region "Month 06"
                        string year06 = "06/" + (yearNOW + 1);
                        DateTime date06 = DateTime.Parse(year06);
                        double Qty06 = 0;
                        double Proqty06 = 0;

                        if (item.TYPE == "Qty issued to System Line(K)")
                        {
                            if (item.JUN != 0)
                            {
                                Qty06 = item.JUN;
                            }
                            else
                            {
                                Qty06 = getQtyisuetosystem(date06);
                            }
                            LstEEP[3].JUN = Qty06;
                        }
                        if (item.TYPE == "Escaping to System Line Qty")
                        {
                            if (item.JUN != 0)
                            {
                                Proqty06 = item.JUN;

                            }
                            else
                            {
                                Proqty06 = getProQtytoSysTem(date06);
                            }
                            LstEEP[1].JUN = Proqty06;
                        }

                        #endregion
                    }

                }

                //Get Escaping PPM to System Line
                foreach (var item in LstEEP)
                {
                    #region "Month 07"
                    double EEPQty07 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.JUL != 0)
                        {
                            EEPQty07 = item.JUL;
                        }
                        else if (LstEEP[1].JUL != 0 && LstEEP[3].JUL != 0)
                        {
                            EEPQty07 = Math.Round(((LstEEP[1].JUL / LstEEP[3].JUL) * 1000000), 1);
                        }
                        LstEEP[0].JUL = EEPQty07;
                    }
                    #endregion

                    #region "Month 08"
                    double EEPQty08 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.AUG != 0)
                        {
                            EEPQty08 = item.AUG;
                        }
                        else if (LstEEP[1].AUG != 0 && LstEEP[3].AUG != 0)
                        {
                            EEPQty08 = Math.Round(((LstEEP[1].AUG / LstEEP[3].AUG) * 1000000), 1);
                        }
                        LstEEP[0].AUG = EEPQty08;
                    }
                    #endregion

                    #region "Month 09"
                    double EEPQty09 = 0;

                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.SEP != 0)
                        {
                            EEPQty09 = item.SEP;
                        }
                        else if ((LstEEP[1].SEP != 0 && LstEEP[3].SEP != 0))
                        {
                            EEPQty09 = Math.Round(((LstEEP[1].SEP / LstEEP[3].SEP) * 1000000), 1);
                        }
                        LstEEP[0].SEP = EEPQty09;

                    }
                    #endregion

                    #region "Month 10"
                    double EEPQty10 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.OCT != 0)
                        {
                            EEPQty10 = item.OCT;
                        }
                        else if ((LstEEP[1].OCT != 0 && LstEEP[3].OCT != 0))
                        {
                            EEPQty10 = Math.Round(((LstEEP[1].OCT / LstEEP[3].OCT) * 1000000), 1);
                        }
                        LstEEP[0].OCT = EEPQty10;
                    }
                    #endregion

                    #region "Month 11"
                    double EEPQty11 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.NOV != 0)
                        {
                            EEPQty11 = item.NOV;
                        }
                        else if ((LstEEP[1].NOV != 0 && LstEEP[3].NOV != 0))
                        {
                            EEPQty11 = Math.Round(((LstEEP[1].NOV / LstEEP[3].NOV) * 1000000), 1);
                        }
                        LstEEP[0].NOV = EEPQty11;
                    }
                    #endregion

                    #region "Month 12"
                    double EEPQty12 = 0;

                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.DEC != 0)
                        {
                            EEPQty12 = item.DEC;
                        }
                        else if ((LstEEP[1].DEC != 0 && LstEEP[3].DEC != 0))
                        {
                            EEPQty12 = Math.Round(((LstEEP[1].DEC / LstEEP[3].DEC) * 1000000), 1);
                        }

                        LstEEP[0].DEC = EEPQty12;
                    }
                    #endregion

                    #region "Month 01"
                    double EEPQty01 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.JAN != 0)
                        {
                            EEPQty01 = item.JAN;
                        }
                        else if ((LstEEP[1].JAN != 0 && LstEEP[3].JAN != 0))
                        {
                            EEPQty01 = Math.Round(((LstEEP[1].JAN / LstEEP[3].JAN) * 1000000), 1);
                        }
                        LstEEP[0].JAN = EEPQty01;
                    }
                    #endregion

                    #region "Month 02"
                    double EEPQty02 = 0;

                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.FEB != 0)
                        {
                            EEPQty02 = item.FEB;
                        }
                        else if ((LstEEP[1].FEB != 0 && LstEEP[3].FEB != 0))
                        {
                            EEPQty02 = Math.Round(((LstEEP[1].FEB / LstEEP[3].FEB) * 1000000), 1);
                        }
                        LstEEP[0].FEB = EEPQty02;
                    }
                    #endregion

                    #region "Month 03"
                    double EEPQty03 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.MAR != 0)
                        {
                            EEPQty03 = item.MAR;
                        }
                        else if ((LstEEP[1].MAR != 0 && LstEEP[3].MAR != 0))
                        {
                            EEPQty03 = Math.Round(((LstEEP[1].MAR / LstEEP[3].MAR) * 1000000), 1);
                        }
                        LstEEP[0].MAR = EEPQty03;
                    }
                    #endregion

                    #region "Month 04"
                    double EEPQty04 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.APR != 0)
                        {
                            EEPQty03 = item.APR;
                        }
                        else if ((LstEEP[1].APR != 0 && LstEEP[3].APR != 0))
                        {
                            EEPQty04 = Math.Round(((LstEEP[1].APR / LstEEP[3].APR) * 1000000), 1);
                        }
                        LstEEP[0].APR = EEPQty04;
                    }
                    #endregion

                    #region "Month 05"
                    double EEPQty05 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.MAY != 0)
                        {
                            EEPQty03 = item.MAY;
                        }
                        else if ((LstEEP[1].MAY != 0 && LstEEP[3].MAY != 0))
                        {
                            EEPQty05 = Math.Round(((LstEEP[1].MAY / LstEEP[3].MAY) * 1000000), 1);
                        }
                        LstEEP[0].MAY = EEPQty05;
                    }
                    #endregion
                    #region "Month 06"
                    double EEPQty06 = 0;
                    if (item.TYPE == "Escaping PPM to System Line")
                    {
                        if (item.JUN != 0)
                        {
                            EEPQty06 = item.JUN;
                        }
                        else if ((LstEEP[1].JUN != 0 && LstEEP[3].JUN != 0))
                        {
                            EEPQty06 = Math.Round(((LstEEP[1].JUN / LstEEP[3].JUN) * 1000000), 1);
                        }
                        LstEEP[0].JUN = EEPQty06;
                    }
                    #endregion
                }
            }
            else
            {

                EEPreportviewModel listdate1 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Qty issued to System Line(K)",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate1);

                EEPreportviewModel listdate2 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escaping to System Line Qty",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate2);

                EEPreportviewModel listdate3 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escaping PPM to System Line",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate3);

                EEPreportviewModel listdate4 = new EEPreportviewModel
                {
                    FY = yearselect,
                    TYPE = "Escapping to System PPM (Target)",
                    FY1 = 0,
                    FY2 = 0,
                    FY3 = 0,
                    FYCurrent = 0,
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0
                };
                LstEEP.Add(listdate4);
                //thang 7
                string year07 = "07/" + (int.Parse(yearselect) - 1);
                DateTime date07 = DateTime.Parse(year07);
                double Qty07 = getQtyisuetosystem(date07);
                LstEEP[0].JUL = Qty07;
                double Proqty07 = getProQtytoSysTem(date07);
                LstEEP[1].JUL = Proqty07;
                double EEPQty = 0;
                if (Qty07 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty07 / Qty07) * 1000000), 1);
                }
                LstEEP[2].JUL = EEPQty;
                //thang 8
                string year08 = "08/" + (int.Parse(yearselect) - 1);
                DateTime date08 = DateTime.Parse(year08);
                double Qty08 = getQtyisuetosystem(date08);
                LstEEP[0].AUG = Qty08;
                double Proqty08 = getProQtytoSysTem(date08);
                LstEEP[1].AUG = Proqty08;
                if (Qty08 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty08 / Qty08) * 1000000), 1);
                }
                LstEEP[2].AUG = EEPQty;
                //thang 9
                string year09 = "09/" + (int.Parse(yearselect) - 1);
                DateTime date09 = DateTime.Parse(year09);
                double Qty09 = getQtyisuetosystem(date09);
                LstEEP[0].SEP = Qty09;
                double Proqty09 = getProQtytoSysTem(date09);
                LstEEP[1].SEP = Proqty09;
                if (Qty09 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty09 / Qty09) * 1000000), 1);
                }
                LstEEP[2].SEP = EEPQty;
                //thang 10
                string year10 = "10/" + (int.Parse(yearselect) - 1);
                DateTime date10 = DateTime.Parse(year10);
                double Qty10 = getQtyisuetosystem(date10);
                LstEEP[0].OCT = Qty10;
                double Proqty10 = getProQtytoSysTem(date10);
                LstEEP[1].OCT = Proqty10;
                if (Qty10 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty10 / Qty10) * 1000000), 1);
                }
                LstEEP[2].OCT = EEPQty;
                //thang 11
                string year11 = "11/" + (int.Parse(yearselect) - 1);
                DateTime date11 = DateTime.Parse(year11);
                double Qty11 = getQtyisuetosystem(date11);
                LstEEP[0].NOV = Qty11;
                double Proqty11 = getProQtytoSysTem(date11);
                LstEEP[1].NOV = Proqty11;
                if (Qty11 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty11 / Qty11) * 1000000), 1);
                }
                LstEEP[2].NOV = EEPQty;
                //thang 12
                string year12 = "12/" + (int.Parse(yearselect) - 1);
                DateTime date12 = DateTime.Parse(year12);
                double Qty12 = getQtyisuetosystem(date12);
                LstEEP[0].DEC = Qty12;
                double Proqty12 = getProQtytoSysTem(date12);
                LstEEP[1].DEC = Proqty12;
                if (Qty12 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty12 / Qty12) * 1000000), 1);
                }
                LstEEP[2].DEC = EEPQty;
                //thang 01
                string year01 = "01/" + yearselect;
                DateTime date01 = DateTime.Parse(year01);
                double Qty01 = getQtyisuetosystem(date01);
                LstEEP[0].JAN = Qty01;
                double Proqty01 = getProQtytoSysTem(date01);
                LstEEP[1].JAN = Proqty01;
                if (Qty01 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty01 / Qty01) * 1000000), 1);
                }
                LstEEP[2].JAN = EEPQty;
                //thang 02
                string year02 = "02/" + yearselect;
                DateTime date02 = DateTime.Parse(year02);
                double Qty02 = getQtyisuetosystem(date02);
                LstEEP[0].FEB = Qty02;
                double Proqty02 = getProQtytoSysTem(date02);
                LstEEP[1].FEB = Proqty02;
                if (Qty02 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty02 / Qty02) * 1000000), 1);
                }
                LstEEP[2].FEB = EEPQty;
                //thang 03
                string year03 = "03/" + yearselect;
                DateTime date03 = DateTime.Parse(year03);
                double Qty03 = getQtyisuetosystem(date03);
                LstEEP[0].MAR = Qty03;
                double Proqty03 = getProQtytoSysTem(date03);
                LstEEP[1].MAR = Proqty03;
                if (Qty03 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty03 / Qty03) * 1000000), 1);
                }
                LstEEP[2].MAR = EEPQty;
                //thang 04
                string year04 = "04/" + yearselect;
                DateTime date04 = DateTime.Parse(year04);
                double Qty04 = getQtyisuetosystem(date04);
                LstEEP[0].APR = Qty04;
                double Proqty04 = getProQtytoSysTem(date04);
                LstEEP[1].APR = Proqty04;
                if (Qty04 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty04 / Qty04) * 1000000), 1);
                }
                LstEEP[2].APR = EEPQty;
                //thang 05
                string year05 = "05/" + yearselect;
                DateTime date05 = DateTime.Parse(year05);
                double Qty05 = getQtyisuetosystem(date05);
                LstEEP[0].MAY = Qty05;
                double Proqty05 = getProQtytoSysTem(date05);
                LstEEP[1].MAY = Proqty05;
                if (Qty05 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty05 / Qty05) * 1000000), 1);
                }
                LstEEP[2].MAY = EEPQty;
                //thang 06
                string year06 = "06/" + yearselect;
                DateTime date06 = DateTime.Parse(year06);
                double Qty06 = getQtyisuetosystem(date06);
                LstEEP[0].JUN = Qty06;
                double Proqty06 = getProQtytoSysTem(date06);
                LstEEP[1].JUN = Proqty06;
                if (Qty06 == 0)
                {
                    EEPQty = 0;
                }
                else
                {
                    EEPQty = Math.Round(((Proqty06 / Qty06) * 1000000), 1);
                }
                LstEEP[2].JUN = EEPQty;
            }
            List<EEPreportviewModel> listRP = new List<EEPreportviewModel>();
            EEPreportviewModel a1 = LstEEP.Where(x => x.TYPE == "Qty issued to System Line(K)").FirstOrDefault();
            double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
            a1.FYCurrent = qtysum;
            listRP.Add(a1);
            EEPreportviewModel a2 = LstEEP.Where(x => x.TYPE == "Escaping to System Line Qty").FirstOrDefault();
            double qtytwo = Math.Round((a2.JUL + a2.AUG + a2.SEP + a2.OCT + a2.NOV + a2.DEC + a2.JAN + a2.FEB + a2.MAR + a2.APR + a2.APR + a2.MAY + a2.JUN), 1);
            a2.FYCurrent = qtytwo;
            listRP.Add(a2);
            EEPreportviewModel a3 = LstEEP.Where(x => x.TYPE == "Escaping PPM to System Line").FirstOrDefault();
            double qtypercent = Math.Round(((qtytwo / qtysum) * 1000000), 1);
            a3.FYCurrent = qtypercent;
            listRP.Add(a3);
            EEPreportviewModel a4 = LstEEP.Where(x => x.TYPE == "Escapping to System PPM (Target)").FirstOrDefault();
            listRP.Add(a4);
            return listRP;
        }
        public void UpdateEEP(EEP_REPORT ncrday)
        {

            if (!UpdateDatabase)
            {

                EEP_REPORT data = _db.EEP_REPORT.Where(n => n.FY == ncrday.FY && n.TYPE == ncrday.TYPE).FirstOrDefault();
                if (data != null)
                {
                    data.FY = ncrday.FY;
                    data.TYPE = ncrday.TYPE;
                    data.JUL = ncrday.JUL;
                    data.AUG = ncrday.AUG;
                    data.SEP = ncrday.SEP;
                    data.OCT = ncrday.OCT;
                    data.NOV = ncrday.NOV;
                    data.DEC = ncrday.DEC;
                    data.JAN = ncrday.JAN;
                    data.FEB = ncrday.FEB;
                    data.MAR = ncrday.MAR;
                    data.APR = ncrday.APR;
                    data.MAY = ncrday.MAY;
                    data.JUN = ncrday.JUN;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    _db.EEP_REPORT.Add(ncrday);
                    _db.SaveChanges();
                }
            }
        }
        //get receiver qty for supplier strategy
        private double Getqtyreceiversuppplier(int year, string CCN, int month)
        {
            //dieu kien de get ra list vendor. theo nam. 
            List<string> vendorstrate = _db.SUPPLIER_PPM.Where(x => x.FY == year && x.CCN == CCN).Select(x => x.VENDOR).ToList();
            string lstidvendor = string.Join(";", vendorstrate);
            List<sp_SupplierPPMReportReceiverQTY_Result> lststratery = _db.sp_SupplierPPMReportReceiverQTY(year, month, lstidvendor).ToList();
            return lststratery.Count <= 0 ? 0 : lststratery.Sum(x => x.REC_QTY);
        }
        private double Getqtyrejectsuppplier(int year, string CCN, int month)
        {
            List<sp_SupplierPPMReportRejStratery_Result> lstdet = _db.sp_SupplierPPMReportRejStratery(year, month, CCN).ToList();
            return lstdet.Count <= 0 ? 0 : (double)lstdet.Sum(x => x.REJ_QTY);
        }
        //get receiver qty for supplier nonestrategy
        private double Getqtyreceiversupppliernon(int year, string CCN, int month)
        {
            List<sp_SupplierPPMReportReceiverQTYNon_Result> lststratery = _db.sp_SupplierPPMReportReceiverQTYNon(year, month, CCN).ToList();
            return lststratery.Count <= 0 ? 0 : lststratery.Sum(x => x.REC_QTY);
        }
        private double Getqtyrejectsupppliernon(int year, string CCN, int month)
        {
            List<sp_SupplierPPMReportRejNonStratery_Result> lstdet = _db.sp_SupplierPPMReportRejNonStratery(year, month, CCN).ToList();
            return lstdet.Count <= 0 ? 0 : (double)lstdet.Sum(x => x.REJ_QTY);
        }
        public List<SupplierforPPMViewModel> GetdataSupplierPPM(int yearselect, string CCN)
        {
            DateTime a = new DateTime(1,1,2018) ;
            DateTime b = new DateTime(1, 1, 2019);
            List<sp_Report_LotRejectRate_Result> listresult = _db.sp_Report_LotRejectRate(a,b, "%", "%").ToList();
            return null;
        }
        public List<sp_Report_PPMByPart_Result> GetdataSupplierPPMTest(string part,string CCN,DateTime dateSta,DateTime dateDue)
        {
          //  DateTime a = new DateTime(2018,01,01);
           // DateTime b = new DateTime(2019,01,01);

            List<sp_Report_PPMByPart_Result> listresult = _db.sp_Report_PPMByPart(dateSta, dateDue, part, CCN).ToList();
            foreach (var item in listresult)
            {
                if(item.Name != "")
                {
                    item.Vendor = item.Vendor.Trim();
                }

            }

            return listresult;
        }
        public void UpdateSupplierPPM(SUPPLIER_PPM_RP ncrday)
        {

            if (!UpdateDatabase)
            {

                SUPPLIER_PPM_RP data = _db.SUPPLIER_PPM_RP.Where(n => n.FY == ncrday.FY && n.TYPE == ncrday.TYPE).FirstOrDefault();
                if (data != null)
                {
                    data.FY = ncrday.FY;
                    data.TYPE = ncrday.TYPE;
                    data.JUL = ncrday.JUL;
                    data.AUG = ncrday.AUG;
                    data.SEP = ncrday.SEP;
                    data.OTC = ncrday.OTC;
                    data.NOV = ncrday.NOV;
                    data.DEC = ncrday.DEC;
                    data.JAN = ncrday.JAN;
                    data.FEB = ncrday.FEB;
                    data.MAR = ncrday.MAR;
                    data.APR = ncrday.APR;
                    data.MAY = ncrday.MAY;
                    data.JUN = ncrday.JUN;
                    data.Sort = ncrday.Sort;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    _db.SUPPLIER_PPM_RP.Add(ncrday);
                    _db.SaveChanges();
                }
            }
        }
        public List<SelectListItem> GetdropdownCCN()
        {
            List<SelectListItem> listvendor = _db.CCNs.Select(x => new SelectListItem
            {
                Value = x.CCN1.Trim(),
                Text = (x.CCN1.Trim()),
            }).ToList();
            return listvendor;
        }
        public List<SelectListItem> GetdropdownCCN(string ccn)
        {
            List<SelectListItem> listvendor = _db.CCNs.Select(x => new SelectListItem
            {
                Value = x.CCN1.Trim(),
                Text = (x.CCN1.Trim()),
                Selected = (x.CCN1.Trim()) == ccn
            }).ToList();
            return listvendor;
        }
        public bool InsertAndDelete(string CCN, int FY, List<SUPPLIER_PPM> entity)
        {
            using (DbContextTransaction tran = _db.Database.BeginTransaction())
            {
                try
                {
                    List<SUPPLIER_PPM> lstselect = _db.SUPPLIER_PPM.Where(n => n.FY == FY && n.CCN.Trim() == CCN).ToList();
                    //Get array id of SUPPLIER_PPM delete
                   // string[] lstiddelete = entity.Select(x=>  x.VENDOR.Trim() ).ToArray();
                   //tuan lua
                      var arrVendor = entity.Select(x => $"{x.VENDOR.Trim()}-{x.PUR_LOC.Trim()}" ).ToArray();
                    // select 2 cai giong nhau r so sanh
                    var vendorarr = lstselect.Select(x => $"{x.VENDOR.Trim()}-{x.PUR_LOC.Trim()}").ToArray();
                    IEnumerable<SUPPLIER_PPM> lstdelete = lstselect.Where(x => arrVendor !=vendorarr);
                    lstdelete.Select(x => x.VENDOR.Trim()).ToArray();
                    //delete
                    string[] arrIdhaddeleted = _db.SUPPLIER_PPM.RemoveRange(lstdelete).Select(x => x.VENDOR.Trim()).ToArray();
                    IEnumerable<string> arrIdafterdeleted = lstselect.Where(x => !arrIdhaddeleted.Contains(x.VENDOR.Trim())).Select(x => x.VENDOR.Trim());
                    IEnumerable<SUPPLIER_PPM> lstAdd = entity.Where(x => !arrIdafterdeleted.Contains(x.VENDOR.Trim()));

                    _db.SUPPLIER_PPM.AddRange(lstAdd);
                    _db.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    new LogWriter("ReportNcrService : InsertAndDelete").LogWrite(ex.ToString());
                    return false;
                }
            }
        }
        public List<StrateryViewModel> GetDataReadSupplier(int year, string ccn)
        {
            //  List<StrateryViewModel> lstview = new List<StrateryViewModel>();
            List<string> vendorstrate = _db.SUPPLIER_PPM.Where(x => x.FY == year && x.CCN == ccn).Select(x => x.VENDOR).ToList();
            string lstidvendor = string.Join(";", vendorstrate);
            List<sp_SupplierPPMReportReceiverQTYDataRaw_Result> lstview1 = _db.sp_SupplierPPMReportReceiverQTYDataRaw(year, lstidvendor).ToList();
            List<StrateryViewModel> lst = (from ls in lstview1
                                           select (new StrateryViewModel
                                           {
                                               Supplier = ls.VENDOR,
                                               Date = ls.POSTING_DATE,
                                               ReceivedQty = ls.REC_QTY
                                           })).ToList();
            return lst;
        }
        public List<StrateryViewModel> GetDataReadSupplierNone(int year, string ccn)
        {
            //   List<StrateryViewModel> lstview = new List<StrateryViewModel>();

            List<sp_SupplierPPMReportRejQTYDataRaw_Result> lstview1 = _db.sp_SupplierPPMReportRejQTYDataRaw(year, ccn).ToList();
            List<StrateryViewModel> lst = (from ls in lstview1
                                           select (new StrateryViewModel
                                           {
                                               Supplier = ls.VENDOR,
                                               RejectQTy = ls.REJ_QTY,
                                               Date = ls.DATEAPPROVAL
                                           })).ToList();
            return lst;
        }

        //4 panel
        //get qtyrecceiver by 1 supplier
        private double Getqtyreceiverby1suppplier(int month, int year, string id)
        {
            //dieu kien de get ra list vendor. theo nam. 
            List<sp_Onesupplier4panelRecQty_Result> lststratery = _db.sp_Onesupplier4panelRecQty(year, month, id).ToList();

            return lststratery.Count() <= 0 ? 0 : lststratery.Sum(x => x.REC_QTY);
        }
        //get reject qty by 1 supplier
        private double Getqtyrejectby1suppplier(int month, int year, string id)
        {

            List<sp_Onesupplier4panelRejQty_Result> lstdet = _db.sp_Onesupplier4panelRejQty(year, month, id).ToList();
            return lstdet.Count() <= 0 ? 0 : (double)lstdet.Sum(x => x.REJ_QTY);
        }
        public List<OneSupplierforPPM> GetDataReadSupplierOne(int FY, string id, int month)
        {
            // lấy list dữ liệu từ tháng 6 của năm tryền vào đén t6 năm kế tiếp    
            List<OneSupplierforPPM> LstONESUPPLLIER_PPMRB = _db.ONESUPPLLIER_PPMRB.Where(n => n.FY == FY && id.Trim() == n.IDVendor.Trim()).
                                              Select(n => new OneSupplierforPPM
                                              {
                                                  FY = n.FY,
                                                  TYPE = n.Type,
                                                  JUL = n.JUL,
                                                  AUG = n.AUG,
                                                  SEP = n.SEP,
                                                  OCT = n.OCT,
                                                  NOV = n.NOV,
                                                  DEC = n.DEC,
                                                  JAN = n.JAN,
                                                  FEB = n.FEB,
                                                  MAR = n.MAR,
                                                  APR = n.APR,
                                                  MAY = n.MAY,
                                                  JUN = n.JUN,
                                                  IDVendor = n.IDVendor,

                                              }).ToList();
            if (LstONESUPPLLIER_PPMRB.Count < 1)
            {
                OneSupplierforPPM listdate1 = new OneSupplierforPPM
                {
                    FY = FY,
                    TYPE = "REC",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    IDVendor = id,
                };
                LstONESUPPLLIER_PPMRB.Add(listdate1);
                OneSupplierforPPM listdate2 = new OneSupplierforPPM
                {
                    FY = FY,
                    TYPE = "REJEC",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    IDVendor = id,
                };
                LstONESUPPLLIER_PPMRB.Add(listdate2);
                OneSupplierforPPM listdate3 = new OneSupplierforPPM
                {
                    FY = FY,
                    TYPE = "TARGET",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    IDVendor = id,
                };
                LstONESUPPLLIER_PPMRB.Add(listdate3);
                OneSupplierforPPM listdate4 = new OneSupplierforPPM
                {
                    FY = FY,
                    TYPE = "PPM YTD",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OCT = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    IDVendor = id,
                };
                LstONESUPPLLIER_PPMRB.Add(listdate4);
                //thang 07
                if (month < 7)
                {
                    month = month + 12;
                }
                for (int i = 7; i < 19; i++)
                {
                    double JulReceiver = 0;
                    double JulRecject = 0;

                    if (i > month)
                    {
                        break;
                    }
                    if (month > 12)
                    {
                        //Thang nam sau
                        JulReceiver = Getqtyreceiverby1suppplier(i - 12, FY - 1, id);
                        JulRecject = Getqtyrejectby1suppplier(i - 12, FY - 1, id);
                    }
                    else
                    {
                        // Thang nam hien tai
                        JulReceiver = Getqtyreceiverby1suppplier(i, FY, id);
                        JulRecject = Getqtyrejectby1suppplier(i, FY, id);
                    }
                    if (JulReceiver > 0d)
                    {
                        switch (i)
                        {
                            case 7:
                                LstONESUPPLLIER_PPMRB[0].JUL = JulReceiver;
                                break;
                            case 8:
                                LstONESUPPLLIER_PPMRB[0].AUG = JulReceiver;
                                break;
                            case 9:
                                LstONESUPPLLIER_PPMRB[0].SEP = JulReceiver;
                                break;
                            case 10:
                                LstONESUPPLLIER_PPMRB[0].OCT = JulReceiver;
                                break;
                            case 11:
                                LstONESUPPLLIER_PPMRB[0].NOV = JulReceiver;
                                break;
                            case 12:
                                LstONESUPPLLIER_PPMRB[0].DEC = JulReceiver;
                                break;
                            case 13:
                                LstONESUPPLLIER_PPMRB[0].JAN = JulReceiver;
                                break;
                            case 14:
                                LstONESUPPLLIER_PPMRB[0].FEB = JulReceiver;
                                break;
                            case 15:
                                LstONESUPPLLIER_PPMRB[0].MAR = JulReceiver;
                                break;
                            case 16:
                                LstONESUPPLLIER_PPMRB[0].APR = JulReceiver;
                                break;
                            case 17:
                                LstONESUPPLLIER_PPMRB[0].JAN = JulReceiver;
                                break;
                            case 18:
                                LstONESUPPLLIER_PPMRB[0].JUN = JulReceiver;
                                break;
                            default:
                                break;
                        }
                    }

                    if (JulRecject > 0d)
                    {
                        switch (i)
                        {
                            case 7:
                                LstONESUPPLLIER_PPMRB[1].JUL = JulRecject;
                                break;
                            case 8:
                                LstONESUPPLLIER_PPMRB[1].AUG = JulRecject;
                                break;
                            case 9:
                                LstONESUPPLLIER_PPMRB[1].SEP = JulRecject;
                                break;
                            case 10:
                                LstONESUPPLLIER_PPMRB[1].OCT = JulRecject;
                                break;
                            case 11:
                                LstONESUPPLLIER_PPMRB[1].NOV = JulRecject;
                                break;
                            case 12:
                                LstONESUPPLLIER_PPMRB[1].DEC = JulRecject;
                                break;
                            case 13:
                                LstONESUPPLLIER_PPMRB[1].JAN = JulRecject;
                                break;
                            case 14:
                                LstONESUPPLLIER_PPMRB[1].FEB = JulRecject;
                                break;
                            case 15:
                                LstONESUPPLLIER_PPMRB[1].MAR = JulRecject;
                                break;
                            case 16:
                                LstONESUPPLLIER_PPMRB[1].APR = JulRecject;
                                break;
                            case 17:
                                LstONESUPPLLIER_PPMRB[1].JAN = JulRecject;
                                break;
                            case 18:
                                LstONESUPPLLIER_PPMRB[1].JUN = JulRecject;
                                break;
                            default:
                                break;
                        }
                    }
                    double PPMYTD0 = Math.Round((JulRecject / JulReceiver * 1000000), 1);
                    if (JulReceiver != 0 && JulRecject != 0)
                    {
                        switch (i)
                        {
                            case 7:
                                LstONESUPPLLIER_PPMRB[3].JUL = PPMYTD0;
                                break;
                            case 8:
                                LstONESUPPLLIER_PPMRB[3].AUG = PPMYTD0;
                                break;
                            case 9:
                                LstONESUPPLLIER_PPMRB[3].SEP = PPMYTD0;
                                break;
                            case 10:
                                LstONESUPPLLIER_PPMRB[3].OCT = PPMYTD0;
                                break;
                            case 11:
                                LstONESUPPLLIER_PPMRB[3].NOV = PPMYTD0;
                                break;
                            case 12:
                                LstONESUPPLLIER_PPMRB[3].DEC = PPMYTD0;
                                break;
                            case 13:
                                LstONESUPPLLIER_PPMRB[3].JAN = PPMYTD0;
                                break;
                            case 14:
                                LstONESUPPLLIER_PPMRB[3].FEB = PPMYTD0;
                                break;
                            case 15:
                                LstONESUPPLLIER_PPMRB[3].MAR = PPMYTD0;
                                break;
                            case 16:
                                LstONESUPPLLIER_PPMRB[3].APR = PPMYTD0;
                                break;
                            case 17:
                                LstONESUPPLLIER_PPMRB[3].JAN = PPMYTD0;
                                break;
                            case 18:
                                LstONESUPPLLIER_PPMRB[3].JUN = PPMYTD0;
                                break;
                            default:
                                break;
                        }
                    }
                    // }

                }

                //double JulReceiver07 = Getqtyreceiverby1suppplier(07, FY, id);
                //double JulRecject07 = Getqtyrejectby1suppplier(07, FY, id);
                //if (JulReceiver07 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].JUL = JulReceiver07;
                //}
                //if (JulRecject07 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].JUL = JulRecject07;
                //}
                //double PPMYTD = Math.Round((JulRecject07 / JulReceiver07 * 1000000), 1);
                //if (JulReceiver07 != 0 && JulRecject07 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].JUL = PPMYTD;
                //}
                //thang 8
                //double JulReceiver08 = Getqtyreceiverby1suppplier(08, FY, id);
                //double JulRecject08 = Getqtyrejectby1suppplier(08, FY, id);
                //if (JulReceiver08 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].AUG = JulReceiver08;
                //}
                //if (JulRecject08 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].AUG = JulRecject08;
                //}
                //double PPMYTD08 = Math.Round((JulRecject08 / JulReceiver08 * 1000000), 1);
                //if (JulReceiver08 != 0 && JulRecject08 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].AUG = PPMYTD08;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].AUG = 0;
                //}
                //thang 09
                //double JulReceiver09 = Getqtyreceiverby1suppplier(09, FY, id);
                //double JulRecject09 = Getqtyrejectby1suppplier(09, FY, id);
                //if (JulReceiver09 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].SEP = JulReceiver09;
                //}
                //if (JulRecject09 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].SEP = JulRecject09;
                //}
                //double PPMYTD09 = Math.Round((JulRecject09 / JulReceiver09 * 1000000), 1);
                //if (JulReceiver09 != 0 && JulRecject09 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].SEP = PPMYTD09;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].SEP = 0;
                //}
                //thang 10
                //double JulReceiver10 = Getqtyreceiverby1suppplier(10, FY, id);
                //double JulRecject10 = Getqtyrejectby1suppplier(10, FY, id);
                //if (JulReceiver10 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].OCT = JulReceiver10;
                //}
                //if (JulRecject10 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].OCT = JulRecject10;
                //}
                //double PPMYTD10 = Math.Round((JulRecject10 / JulReceiver10 * 1000000), 1);
                //if (JulReceiver10 != 0 && JulRecject10 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].OCT = PPMYTD10;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].OCT = 0;
                //}
                // thang 11
                //double JulReceiver11 = Getqtyreceiverby1suppplier(11, FY, id);
                //double JulRecject11 = Getqtyrejectby1suppplier(11, FY, id);
                //if (JulReceiver11 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].NOV = JulReceiver11;
                //}
                //if (JulRecject11 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].NOV = JulRecject11;
                //}
                //double PPMYTD11 = Math.Round((JulRecject11 / JulReceiver11 * 1000000), 1);
                //if (JulReceiver11 != 0 && JulRecject11 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].NOV = PPMYTD11;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].NOV = 0;
                //}
                //thang 12
                //double JulReceiver12 = Getqtyreceiverby1suppplier(12, FY, id);
                //double JulRecject12 = Getqtyrejectby1suppplier(12, FY, id);
                //if (JulReceiver12 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].DEC = JulReceiver12;
                //}
                //if (JulRecject12 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].DEC = JulRecject12;
                //}
                //double PPMYTD12 = Math.Round((JulRecject12 / JulReceiver12 * 1000000), 1);
                //if (JulReceiver12 != 0 && JulRecject12 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].DEC = PPMYTD12;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].DEC = 0;
                //}
                //thang01
                //double JulReceiver01 = Getqtyreceiverby1suppplier(01, FY + 1, id);
                //double JulRecject01 = Getqtyrejectby1suppplier(01, FY + 1, id);
                //if (JulReceiver01 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].JAN = JulReceiver01;
                //}
                //if (JulRecject01 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].JAN = JulRecject01;
                //}
                //double PPMYTD01 = Math.Round((JulRecject01 / JulReceiver01 * 1000000), 1);
                //if (JulReceiver01 != 0 && JulRecject01 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].JAN = PPMYTD01;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].JAN = 0;
                //}

                //thang 2
                //double JulReceiver02 = Getqtyreceiverby1suppplier(02, FY + 1, id);
                //double JulRecject02 = Getqtyrejectby1suppplier(02, FY + 1, id);
                //if (JulReceiver02 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].FEB = JulReceiver02;
                //}
                //if (JulRecject02 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].FEB = JulRecject02;
                //}
                //double PPMYTD02 = Math.Round((JulRecject02 / JulReceiver02 * 1000000), 1);
                //if (JulReceiver02 != 0 && JulRecject02 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].FEB = PPMYTD02;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].FEB = 0;
                //}

                //thang 3
                //double JulReceiver03 = Getqtyreceiverby1suppplier(03, FY + 1, id);
                //double JulRecject03 = Getqtyrejectby1suppplier(03, FY + 1, id);
                //if (JulReceiver03 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].MAR = JulReceiver03;
                //}
                //if (JulRecject03 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].MAR = JulRecject03;
                //}
                //double PPMYTD03 = Math.Round((JulRecject03 / JulReceiver03 * 1000000), 1);
                //if (JulReceiver03 != 0 && JulRecject03 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].MAR = PPMYTD03;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].MAR = 0;
                //}

                //thang4 
                //double JulReceiver04 = Getqtyreceiverby1suppplier(04, FY + 1, id);
                //double JulRecject04 = Getqtyrejectby1suppplier(04, FY + 1, id);
                //if (JulReceiver04 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].APR = JulReceiver04;
                //}
                //if (JulRecject04 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].APR = JulRecject04;
                //}
                //double PPMYTD04 = Math.Round((JulRecject04 / JulReceiver04 * 1000000), 1);
                //if (JulReceiver04 != 0 && JulRecject04 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].APR = PPMYTD04;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].APR = 0;
                //}

                //thang 5
                //double JulReceiver05 = Getqtyreceiverby1suppplier(05, FY + 1, id);
                //double JulRecject05 = Getqtyrejectby1suppplier(05, FY + 1, id);
                //if (JulReceiver05 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].MAY = JulReceiver05;
                //}
                //if (JulRecject05 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].MAY = JulRecject05;
                //}
                //double PPMYTD05 = Math.Round((JulRecject05 / JulReceiver05 * 1000000), 1);
                //if (JulReceiver05 != 0 && JulRecject05 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].MAY = PPMYTD05;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].MAY = 0;
                //}

                //thang 6
                //double JulReceiver06 = Getqtyreceiverby1suppplier(06, FY + 1, id);
                //double JulRecject06 = Getqtyrejectby1suppplier(06, FY + 1, id);
                //if (JulReceiver06 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[0].JUN = JulReceiver06;
                //}
                //if (JulRecject06 > 0d)
                //{
                //    LstONESUPPLLIER_PPMRB[1].JUN = JulRecject06;
                //}
                //double PPMYTD06 = Math.Round((JulRecject06 / JulReceiver06 * 1000000), 1);
                //if (JulReceiver06 != 0 && JulRecject06 != 0)
                //{
                //    LstONESUPPLLIER_PPMRB[3].JUN = PPMYTD06;
                //}
                //else
                //{
                //    LstONESUPPLLIER_PPMRB[3].JUN = 0;
                //}
            }
            //  }
            //     var lst = new List<OneSupplierforPPM> { LstONESUPPLLIER_PPMRB[3] };

            List<OneSupplierforPPM> listRP = new List<OneSupplierforPPM>();
            OneSupplierforPPM a1 = LstONESUPPLLIER_PPMRB.Where(x => x.TYPE == "REC").FirstOrDefault();
            //  double qtysum = Math.Round((a1.JUL + a1.AUG + a1.SEP + a1.OCT + a1.NOV + a1.DEC + a1.JAN + a1.FEB + a1.MAR + a1.APR + a1.APR + a1.MAY + a1.JUN), 1);
            // a1.FYCurrent = qtysum;
            listRP.Add(a1);
            OneSupplierforPPM a2 = LstONESUPPLLIER_PPMRB.Where(x => x.TYPE == "REJEC").FirstOrDefault();
            listRP.Add(a2);
            OneSupplierforPPM a3 = LstONESUPPLLIER_PPMRB.Where(x => x.TYPE == "TARGET").FirstOrDefault();
            listRP.Add(a3);
            OneSupplierforPPM a4 = LstONESUPPLLIER_PPMRB.Where(x => x.TYPE == "PPM YTD").FirstOrDefault();
            listRP.Add(a4);
            return listRP;
        }
        public void UpdateSupplierPPMforOne(ONESUPPLLIER_PPMRB ncrday)
        {

            if (!UpdateDatabase)
            {

                ONESUPPLLIER_PPMRB data = _db.ONESUPPLLIER_PPMRB.Where(n => n.FY == ncrday.FY && n.Type == ncrday.Type && n.IDVendor.Trim() == ncrday.IDVendor.Trim()).FirstOrDefault();
                if (data != null)
                {
                    data.FY = ncrday.FY;
                    data.Type = ncrday.Type;
                    data.JUL = ncrday.JUL;
                    data.AUG = ncrday.AUG;
                    data.SEP = ncrday.SEP;
                    data.OCT = ncrday.OCT;
                    data.NOV = ncrday.NOV;
                    data.DEC = ncrday.DEC;
                    data.JAN = ncrday.JAN;
                    data.FEB = ncrday.FEB;
                    data.MAR = ncrday.MAR;
                    data.APR = ncrday.APR;
                    data.MAY = ncrday.MAY;
                    data.JUN = ncrday.JUN;
                    data.IDVendor = ncrday.IDVendor;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    _db.ONESUPPLLIER_PPMRB.Add(ncrday);
                    _db.SaveChanges();
                }
            }
        }
        public List<topmonthViewmodel> getLstTopQTybmonth(DateTime month, int qty, string id)
        {
            int monthFY = month.Month;
            int FY = DateTime.Now.Year;  // Change Year : month.Year
            List<topmonthViewmodel> lst = new List<topmonthViewmodel>();
            List<Dictionary<string, List<string>>> lstNCRNUM_PartNum = new List<Dictionary<string, List<string>>>();
            List<NCR_HDR> ncrhdr = _db.NCR_HDR.Where(x => x.DateApproval.Value.Month == monthFY && x.DateApproval.Value.Year == FY
           && x.STATUS == StatusInDB.DispositionApproved && x.VENDOR.Trim() == id.Trim() && x.SAMPLE_INSP == true).ToList();

            foreach (NCR_HDR item in ncrhdr)
            {
                topmonthViewmodel doublicatePartNum = lst.FirstOrDefault(x => x.PartNum == item.MI_PART_NO);
                if (doublicatePartNum != null)
                {
                    doublicatePartNum.Value += item.REJ_QTY;
                }
                else
                {
                    topmonthViewmodel topmonthViewmodelTMP = new topmonthViewmodel
                    {
                        PartNum = item.MI_PART_NO,
                        Value = item.REJ_QTY
                    };
                    lst.Add(topmonthViewmodelTMP);
                }
            }

            //  string[] arrPartNum_duplicate = 
            string[] arrPartNum_unique = _db.NCR_HDR.Where(n => n.STATUS == StatusInDB.DispositionApproved && n.DateApproval.Value.Month == monthFY && n.VENDOR.Trim() == id.Trim() && n.PERCENT_INSP == true).Select(x => x.MI_PART_NO.Trim()).ToArray();
            foreach (string item in arrPartNum_unique)
            {
                List<string> lstNcrNumTMP = _db.NCR_HDR.Where(x => x.MI_PART_NO.Equals(item) && x.VENDOR.Trim() == id.Trim()).Select(n => n.NCR_NUM).ToList();
                Dictionary<string, List<string>> Dic = new Dictionary<string, List<string>>
                {
                    { item.Trim(), lstNcrNumTMP }
                };
                lstNCRNUM_PartNum.Add(Dic);
            }

            #region mustn't open region !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            foreach (Dictionary<string, List<string>> item in lstNCRNUM_PartNum)
            {
                string PartNum = item.Keys.ToArray()[0];
                string[] lstNCRNUM = item[PartNum].ToArray();
                topmonthViewmodel topmonthViewmodelTMP = new topmonthViewmodel
                {
                    PartNum = PartNum
                };
                // :=[[
                foreach (string num in lstNCRNUM)
                {
                    List<NCR_DET> lstDet = _db.NCR_DET.Where(x => x.NCR_NUM.Equals(num) & x.DATEAPPROVAL.Value.Year == FY & x.DATEAPPROVAL.Value.Month == monthFY).ToList();
                    // :=]]]
                    foreach (NCR_DET det in lstDet)
                    {
                        int length = det.NC_DESC.Split(';').Length;
                        topmonthViewmodelTMP.Value += (length * det.QTY);
                    }
                }

                lst.Add(topmonthViewmodelTMP);
            }
            #endregion

            return lst.OrderByDescending(x => x.Value).Take(qty).ToList();
        }

        //qts
        public List<topmonthViewmodel> getLstTopQTybmonthqts(DateTime month, int qty, string id)
        {
            int monthFY = month.Month;
            int FY = month.Year;
            List<topmonthViewmodel> lst = new List<topmonthViewmodel>();
            List<Dictionary<string, List<string>>> lstNCRNUM_PartNum = new List<Dictionary<string, List<string>>>();
            //GET DATA
            List<NCR_HDR> NCRHDRs = _db.NCR_HDR.Where(x => x.DateApproval.Value.Year == FY && x.DateApproval.Value.Month == monthFY && x.STATUS == StatusInDB.DispositionApproved && x.VENDOR.Trim() == id.Trim()).ToList();

            //GET DATA AQL
            List<NCR_HDR> NCRAQL = NCRHDRs.Where(x => x.SAMPLE_INSP == true).ToList();
            //GET DATA 100%
            List<NCR_HDR> NCR100 = NCRHDRs.Where(x => x.PERCENT_INSP == true).ToList();
            string[] arrNCRNUM100 = NCR100.Select(x => x.NCR_NUM).ToArray();
            List<NCR_DET> DET100s = _db.NCR_DET.Where(x => arrNCRNUM100.Contains(x.NCR_NUM)).ToList();

            //AQL
            foreach (NCR_HDR item in NCRAQL)
            {
                topmonthViewmodel doublicatePartNum = lst.FirstOrDefault(x => x.PartNum == item.MI_PART_NO);
                if (doublicatePartNum != null)
                {
                    int index = lst.FindIndex(x => x.PartNum == item.MI_PART_NO);
                    lst[index].Value += item.REJ_QTY;
                    //det
                    //for det: length * det +
                }
                else
                {
                    topmonthViewmodel topmonthViewmodelTMP = new topmonthViewmodel
                    {
                        PartNum = item.MI_PART_NO,
                        Value = item.REJ_QTY
                    };
                    lst.Add(topmonthViewmodelTMP);
                }
            }

            //100%
            foreach (NCR_HDR ncr100 in NCR100)
            {
                topmonthViewmodel doublicatePartNum = lst.FirstOrDefault(x => x.PartNum == ncr100.MI_PART_NO);
                if (doublicatePartNum != null)
                {
                    int index = lst.FindIndex(x => x.PartNum == ncr100.MI_PART_NO);
                    //doublicatePartNum.Value += ncr100.REJ_QTY;
                    //det
                    List<NCR_DET> DETOfNCR100 = DET100s.Where(x => x.NCR_NUM.Equals(ncr100.NCR_NUM)).ToList();
                    //for det: length * det +
                    foreach (NCR_DET det in DETOfNCR100)
                    {
                        int length = det.NC_DESC.Split(';').Length;
                        lst[index].Value = (det.QTY * length);
                    }
                }
                else
                {
                    topmonthViewmodel topmonthViewmodelTMP = new topmonthViewmodel
                    {
                        PartNum = ncr100.MI_PART_NO,
                        Value = 0
                    };
                    //det
                    List<NCR_DET> DETOfNCR100 = DET100s.Where(x => x.NCR_NUM.Equals(ncr100.NCR_NUM)).ToList();
                    //for det: length * det +
                    foreach (NCR_DET det in DETOfNCR100)
                    {
                        int length = det.NC_DESC.Split(';').Length;
                        topmonthViewmodelTMP.Value = (det.QTY * length);
                    }
                    lst.Add(topmonthViewmodelTMP);
                }
            }
            return lst.OrderByDescending(x => x.Value).Take(qty).ToList();
        }
        //get top n by year
        public List<topmonthViewmodel> getLstTopQTybyYear(int year, int qty, string id)
        {
            List<topmonthViewmodel> lst = new List<topmonthViewmodel>();
            List<Dictionary<string, List<string>>> lstNCRNUM_PartNum = new List<Dictionary<string, List<string>>>();
            //GET DATA
            List<NCR_HDR> NCRHDRs = _db.NCR_HDR.Where(x => x.DateApproval.Value.Year == year && x.STATUS == StatusInDB.DispositionApproved && x.VENDOR.Trim() == id.Trim()).ToList();

            //GET DATA AQL
            List<NCR_HDR> NCRAQL = NCRHDRs.Where(x => x.SAMPLE_INSP == true).ToList();
            //GET DATA 100%
            List<NCR_HDR> NCR100 = NCRHDRs.Where(x => x.PERCENT_INSP == true).ToList();
            string[] arrNCRNUM100 = NCR100.Select(x => x.NCR_NUM).ToArray();
            List<NCR_DET> DET100s = _db.NCR_DET.Where(x => arrNCRNUM100.Contains(x.NCR_NUM)).ToList();

            //AQL
            foreach (NCR_HDR item in NCRAQL)
            {
                topmonthViewmodel doublicatePartNum = lst.FirstOrDefault(x => x.PartNum == item.MI_PART_NO);
                if (doublicatePartNum != null)
                {
                    int index = lst.FindIndex(x => x.PartNum == item.MI_PART_NO);
                    lst[index].Value += item.REJ_QTY;
                    //det
                    //for det: length * det +
                }
                else
                {
                    topmonthViewmodel topmonthViewmodelTMP = new topmonthViewmodel
                    {
                        PartNum = item.MI_PART_NO,
                        Value = item.REJ_QTY
                    };
                    lst.Add(topmonthViewmodelTMP);
                }
            }

            //100%
            foreach (NCR_HDR ncr100 in NCR100)
            {
                topmonthViewmodel doublicatePartNum = lst.FirstOrDefault(x => x.PartNum == ncr100.MI_PART_NO);
                if (doublicatePartNum != null)
                {
                    int index = lst.FindIndex(x => x.PartNum == ncr100.MI_PART_NO);
                    //doublicatePartNum.Value += ncr100.REJ_QTY;
                    //det
                    List<NCR_DET> DETOfNCR100 = DET100s.Where(x => x.NCR_NUM.Equals(ncr100.NCR_NUM)).ToList();
                    //for det: length * det +
                    foreach (NCR_DET det in DETOfNCR100)
                    {
                        int length = det.NC_DESC.Split(';').Length;
                        lst[index].Value = (det.QTY * length);
                    }
                }
                else
                {
                    topmonthViewmodel topmonthViewmodelTMP = new topmonthViewmodel
                    {
                        PartNum = ncr100.MI_PART_NO,
                        Value = 0
                    };
                    //det
                    List<NCR_DET> DETOfNCR100 = DET100s.Where(x => x.NCR_NUM.Equals(ncr100.NCR_NUM)).ToList();
                    //for det: length * det +
                    foreach (NCR_DET det in DETOfNCR100)
                    {
                        int length = det.NC_DESC.Split(';').Length;
                        topmonthViewmodelTMP.Value = (det.QTY * length);
                    }
                    lst.Add(topmonthViewmodelTMP);
                }
            }
            return lst.OrderByDescending(x => x.Value).Take(qty).ToList();
        }
        public bool checkidtrung4panel(List<PANEL_EXCEL> model)
        {
            if (model.Count > 0)
            {
                int Month = model[0].Month.Month;
                int Year = model[0].Month.Year;
                string Vendor = model[0].VENDOR.Trim();
                PANEL_EXCEL lst = _db.PANEL_EXCEL.Where(x => x.Month.Month == Month && x.Month.Year == Year && x.VENDOR.Trim() == Vendor).FirstOrDefault();
                if (lst != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Update4Panel(List<PANEL_EXCEL> lstUpdate)
        {
            using (DbContextTransaction tran = _db.Database.BeginTransaction())
            {
                try
                {
                    int FM = lstUpdate[0].Month.Month;
                    int FY = lstUpdate[0].Month.Year;
                    List<PANEL_EXCEL> lstDelete = _db.PANEL_EXCEL.Where(x => x.Month.Year == FY & x.Month.Month == FM).ToList();
                    _db.PANEL_EXCEL.RemoveRange(lstDelete);
                    _db.PANEL_EXCEL.AddRange(lstUpdate);
                    _db.SaveChanges();
                    tran.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    new LogWriter("Update4Panel").LogWrite(ex.ToString());
                    return false;
                }
            }
        }
        public bool SaveDataTo4Panel(List<PANEL_EXCEL> model)
        {
            try
            {
                _db.PANEL_EXCEL.AddRange(model);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                new LogWriter("SaveData").LogWrite(ex.ToString());
                return false;
            }
        }
        public void SaveDataRAWORUPDATE(PANEL_RP ncrday)
        {

            if (!UpdateDatabase)
            {

                PANEL_RP data = _db.PANEL_RP.Where(n => n.Vendor == ncrday.Vendor && n.FY == ncrday.FY && n.TYPE == ncrday.TYPE).FirstOrDefault();
                if (data != null)
                {
                    data.FY = ncrday.FY;
                    data.TYPE = ncrday.TYPE;
                    data.JUL = ncrday.JUL;
                    data.AUG = ncrday.AUG;
                    data.SEP = ncrday.SEP;
                    data.OTC = ncrday.OTC;
                    data.NOV = ncrday.NOV;
                    data.DEC = ncrday.DEC;
                    data.JAN = ncrday.JAN;
                    data.FEB = ncrday.FEB;
                    data.MAR = ncrday.MAR;
                    data.APR = ncrday.APR;
                    data.MAY = ncrday.MAY;
                    data.JUN = ncrday.JUN;
                    data.Vendor = ncrday.Vendor;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    _db.PANEL_RP.Add(ncrday);
                    _db.SaveChanges();
                }
            }
        }
        public List<PANEL_RP> getDatarawOTD(DateTime month, string vendor)
        {
            int FY = month.Year;
            List<PANEL_RP> lstView = _db.PANEL_RP.Where(n => n.FY == FY && n.Vendor.Trim() == vendor.Trim()).ToList();
            if (lstView.Count < 1)
            {
                PANEL_RP listdate1 = new PANEL_RP
                {
                    FY = FY,
                    TYPE = "Early",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OTC = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    Vendor = vendor
                };
                lstView.Add(listdate1);
                PANEL_RP listdate2 = new PANEL_RP
                {
                    FY = FY,
                    TYPE = "Late",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OTC = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    Vendor = vendor
                };
                lstView.Add(listdate2);
                PANEL_RP listdate3 = new PANEL_RP
                {
                    FY = FY,
                    TYPE = "OnTime",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OTC = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    Vendor = vendor
                };
                lstView.Add(listdate3);
                PANEL_RP listdate4 = new PANEL_RP
                {
                    FY = FY,
                    TYPE = "Total",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OTC = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    Vendor = vendor
                };
                lstView.Add(listdate4);
                PANEL_RP listdate5 = new PANEL_RP
                {
                    FY = FY,
                    TYPE = "TarGet",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OTC = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    Vendor = vendor
                };
                lstView.Add(listdate5);
                PANEL_RP listdate6 = new PANEL_RP
                {
                    FY = FY,
                    TYPE = "Actual",
                    JUL = 0,
                    AUG = 0,
                    SEP = 0,
                    OTC = 0,
                    NOV = 0,
                    DEC = 0,
                    JAN = 0,
                    FEB = 0,
                    MAR = 0,
                    APR = 0,
                    MAY = 0,
                    JUN = 0,
                    Vendor = vendor
                };
                lstView.Add(listdate6);
                //thang 07
                int jul = 07;
                PANEL_EXCEL lst07 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == jul && n.Month.Year == FY).FirstOrDefault();
                if (lst07 != null)
                {
                    lstView[0].JUL = lst07.EARLY;
                    lstView[1].JUL = lst07.LATE;
                    lstView[2].JUL = lst07.ONTIME;
                    lstView[3].JUL = lst07.TOTAL;
                    lstView[4].JUL = lst07.TARGET;
                    lstView[5].JUL = lst07.ACTUAL;
                }
                //thang 08
                int aug = 08;
                PANEL_EXCEL lst08 = _db.PANEL_EXCEL.FirstOrDefault(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == aug && n.Month.Year == FY);
                if (lst08 != null)
                {
                    lstView[0].AUG = lst08.EARLY;
                    lstView[1].AUG = lst08.LATE;
                    lstView[2].AUG = lst08.ONTIME;
                    lstView[3].AUG = lst08.TOTAL;
                    lstView[4].AUG = lst08.TARGET;
                    lstView[5].AUG = lst08.ACTUAL;
                }

                //thang 09
                int sep = 9;
                PANEL_EXCEL lst09 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == sep && n.Month.Year == FY).FirstOrDefault();
                if (lst09 != null)
                {
                    lstView[0].SEP = lst09.EARLY;
                    lstView[1].SEP = lst09.LATE;
                    lstView[2].SEP = lst09.ONTIME;
                    lstView[3].SEP = lst09.TOTAL;
                    lstView[4].SEP = lst09.TARGET;
                    lstView[5].SEP = lst09.ACTUAL;
                }
                //thang 10
                int otc = 10;
                PANEL_EXCEL lst10 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == otc && n.Month.Year == FY).FirstOrDefault();
                if (lst10 != null)
                {
                    lstView[0].OTC = lst10.EARLY;
                    lstView[1].OTC = lst10.LATE;
                    lstView[2].OTC = lst10.ONTIME;
                    lstView[3].OTC = lst10.TOTAL;
                    lstView[4].OTC = lst10.TARGET;
                    lstView[5].OTC = lst10.ACTUAL;
                }
                //thang 11
                int NOV = 11;
                PANEL_EXCEL lst11 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == NOV && n.Month.Year == FY).FirstOrDefault();
                if (lst11 != null)
                {
                    lstView[0].NOV = lst11.EARLY;
                    lstView[1].NOV = lst11.LATE;
                    lstView[2].NOV = lst11.ONTIME;
                    lstView[3].NOV = lst11.TOTAL;
                    lstView[4].NOV = lst11.TARGET;
                    lstView[5].NOV = lst11.ACTUAL;
                }
                //thang 12
                int dec = 12;
                PANEL_EXCEL lst12 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == dec && n.Month.Year == FY).FirstOrDefault();
                if (lst12 != null)
                {
                    lstView[0].DEC = lst12.EARLY;
                    lstView[1].DEC = lst12.LATE;
                    lstView[2].DEC = lst12.ONTIME;
                    lstView[3].DEC = lst12.TOTAL;
                    lstView[4].DEC = lst12.TARGET;
                    lstView[5].DEC = lst12.ACTUAL;
                }
                //thang 1
                int jan = 1;
                PANEL_EXCEL lst01 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == jan && n.Month.Year == FY + 1).FirstOrDefault();
                if (lst01 != null)
                {
                    lstView[0].JAN = lst01.EARLY;
                    lstView[1].JAN = lst01.LATE;
                    lstView[2].JAN = lst01.ONTIME;
                    lstView[3].JAN = lst01.TOTAL;
                    lstView[4].JAN = lst01.TARGET;
                    lstView[5].JAN = lst01.ACTUAL;
                }
                //thang 2
                int feb = 2;
                PANEL_EXCEL lst02 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == feb && n.Month.Year == FY + 1).FirstOrDefault();
                if (lst02 != null)
                {
                    lstView[0].FEB = lst02.EARLY;
                    lstView[1].FEB = lst02.LATE;
                    lstView[2].FEB = lst02.ONTIME;
                    lstView[3].FEB = lst02.TOTAL;
                    lstView[4].FEB = lst02.TARGET;
                    lstView[5].FEB = lst02.ACTUAL;
                }
                //thang 3
                int mar = 3;
                PANEL_EXCEL lst03 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == mar && n.Month.Year == FY + 1).FirstOrDefault();
                if (lst03 != null)
                {
                    lstView[0].MAR = lst03.EARLY;
                    lstView[1].MAR = lst03.LATE;
                    lstView[2].MAR = lst03.ONTIME;
                    lstView[3].MAR = lst03.TOTAL;
                    lstView[4].MAR = lst03.TARGET;
                    lstView[5].MAR = lst03.ACTUAL;
                }
                //thang4
                int apr = 4;
                PANEL_EXCEL lst04 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == apr && n.Month.Year == FY + 1).FirstOrDefault();
                if (lst04 != null)
                {
                    lstView[0].APR = lst04.EARLY;
                    lstView[1].APR = lst04.LATE;
                    lstView[2].APR = lst04.ONTIME;
                    lstView[3].APR = lst04.TOTAL;
                    lstView[4].APR = lst04.TARGET;
                    lstView[5].APR = lst04.ACTUAL;
                }
                //thang 5
                int may = 5;
                PANEL_EXCEL lst05 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == may && n.Month.Year == FY + 1).FirstOrDefault();
                if (lst05 != null)
                {
                    lstView[0].MAY = lst05.EARLY;
                    lstView[1].MAY = lst05.LATE;
                    lstView[2].MAY = lst05.ONTIME;
                    lstView[3].MAY = lst05.TOTAL;
                    lstView[4].MAY = lst05.TARGET;
                    lstView[5].MAY = lst05.ACTUAL;
                }
                //thang 6
                int jun = 6;
                PANEL_EXCEL lst06 = _db.PANEL_EXCEL.Where(n => n.VENDOR.Trim() == vendor.Trim() && n.Month.Month == jun && n.Month.Year == FY + 1).FirstOrDefault();
                if (lst06 != null)
                {
                    lstView[0].MAY = lst06.EARLY;
                    lstView[1].MAY = lst06.LATE;
                    lstView[2].MAY = lst06.ONTIME;
                    lstView[3].MAY = lst06.TOTAL;
                    lstView[4].MAY = lst06.TARGET;
                    lstView[5].MAY = lst06.ACTUAL;
                }
            }
            return lstView;
        }
        public List<Scarviewmodel> getLstScarProblem(DateTime year, string vendor)
        {
            int FY = year.Year;
            int month = year.Month;
            List<Scarviewmodel> lst = new List<Scarviewmodel>();
            var cscarid = _db.SCARnNCRs.Select(x=>x.ScarId).ToArray();
            List<SCARINFO> lstscar = _db.SCARINFOes.Where(n => cscarid.Contains(n.SCAR_ID) &&  n.VENDOR.Trim() == vendor && n.WRITTENDATE.Year == FY && n.WRITTENDATE.Month == month).ToList();
            foreach (SCARINFO item in lstscar)
            {
                string defect = _db.NCs.Where(n => n.NC_CODE.Trim() == item.DEFECTCODE.Trim()).Select(n => n.NC_DESC).FirstOrDefault();
                if (defect != null)
                {
                    lst.Add(new Scarviewmodel
                    {
                        No = defect,
                        Corective = $@"SCAR: {item.SCAR_ID}; {item.PROBLEM}; {item.MI_PART_NO} ",
                        TargetDate = item.DATERESPOND,
                        ActualDate = item.DATERESPOND,
                        Status = item.STATUS,
                        Owner = item.QUALITY
                    });
                }
            }
            return lst;
        }
        public List<ImprovestrackingViewmodel> getlistimprovestracking(int year, int month, string vendor)
        {
            int nexyYear = year + 1;
            List<ImprovestrackingViewmodel> lstview = new List<ImprovestrackingViewmodel>();
            List<NCR_HDR> lstNCR100 = _db.NCR_HDR.Where(n => n.VENDOR.Trim() == vendor.Trim() && ((n.DateApproval.Value.Month > 6 & n.DateApproval.Value.Year == year) || (n.DateApproval.Value.Month < 7 & n.DateApproval.Value.Year == nexyYear)) && n.PERCENT_INSP == true).ToList();
            string lstNCR_NU100 = string.Join(";", lstNCR100.Select(n => n.NCR_NUM).ToArray());
            string[] arrNCR_NUM100 = lstNCR100.Select(n => n.NCR_NUM).ToArray();
            string lstNCR_DESC = string.Join(";", _db.NCR_DET.Where(n => arrNCR_NUM100.Contains(n.NCR_NUM) && n.RESPONSE == "A").Select(n => n.NC_DESC).ToArray());
            List<ImprovestrackingViewmodel> res = null;
            if (lstNCR_DESC != "" && lstNCR_NU100 != "")
            {
                res = _db.Database.SqlQuery<ImprovestrackingViewmodel>($@"exec sp_IMPROVEMENT_TRACKING '{lstNCR_DESC}', '{lstNCR_NU100}'").ToList();
            }
            //AQL
            List<NCR_HDR> lstNCRAQL = _db.NCR_HDR.Where(n => n.VENDOR.Trim() == vendor.Trim() && ((n.DateApproval.Value.Month > 6 & n.DateApproval.Value.Year == year) || (n.DateApproval.Value.Month < 7 & n.DateApproval.Value.Year == nexyYear)) && n.SAMPLE_INSP == true).ToList();
            string lstNCR_NUAQL = string.Join(";", lstNCRAQL.Select(n => n.NCR_NUM).ToArray());
            string[] arrNCR_NUMAQL = lstNCRAQL.Select(n => n.NCR_NUM).ToArray();
            string lstNCR_DESCAQL = string.Join(";", _db.NCR_DET.Where(n => arrNCR_NUMAQL.Contains(n.NCR_NUM) && n.RESPONSE == "A").Select(n => n.NC_DESC).ToArray());
            List<ImprovestrackingViewmodel> resAQL = null;
            if (lstNCR_DESCAQL != "" && lstNCR_NUAQL != "")
            {
                resAQL = _db.Database.SqlQuery<ImprovestrackingViewmodel>($@"exec sp_IMPROVEMENT_TRACKINGAQL '{lstNCR_DESCAQL}', '{lstNCR_NUAQL}'").ToList();
            }
            for (int i = 0; i < res.Count; i++)
            {
                res[i].NC_NAME = res[i].NC_NAME;
                ImprovestrackingViewmodel a = resAQL.Where(x => x.NC_NAME == res[i].NC_NAME).FirstOrDefault();
                if (a != null)
                {
                   double b=  res[i].JUL += a.JUL;
                    res[i].JUL = b;
                    res[i].AUG += a.AUG;
                    res[i].SEP += a.SEP;
                    res[i].OCT += a.OCT;
                    res[i].NOV += a.NOV;
                    res[i].DEC += a.DEC;
                    res[i].JAN += a.JAN;
                    res[i].FEB += a.FEB;
                    res[i].MAR += a.MAR;
                    res[i].APR += a.APR;
                    res[i].MAY += a.MAY;
                    res[i].JUN += a.JUN;
                    res[i].SUMYTD += a.SUMYTD;
                }
            }

            //
            string[] arrName = res.Select(x => x.NC_NAME).ToArray();
            List<ImprovestrackingViewmodel> non = resAQL.Where(x => !arrName.Contains(x.NC_NAME)).ToList();

            if (non.Count > 0 & non != null)
            {
                res.Concat(non);
            }
            res.Add(new ImprovestrackingViewmodel {
                No = "DELIVERED",
                JUL = Getqtyreceiverby1suppplier(7, 2018, vendor),
                AUG = Getqtyreceiverby1suppplier(8, 2018, vendor),
                SEP = Getqtyreceiverby1suppplier(9, 2018, vendor),
                OCT = Getqtyreceiverby1suppplier(10, 2018, vendor),
                NOV = Getqtyreceiverby1suppplier(11, 2018, vendor),
                DEC = Getqtyreceiverby1suppplier(12, 2018, vendor),
                JAN = Getqtyreceiverby1suppplier(1, 2019, vendor),
                FEB = Getqtyreceiverby1suppplier(2, 2019, vendor),
                MAR = Getqtyreceiverby1suppplier(3, 2019, vendor),
                APR = Getqtyreceiverby1suppplier(4, 2019, vendor),
                MAY = Getqtyreceiverby1suppplier(5, 2019, vendor),
                JUN = Getqtyreceiverby1suppplier(6, 2019, vendor),
                  SUMYTD = Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor)+ Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019 , vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor) + Getqtyreceiverby1suppplier(3, 2019, vendor) + Getqtyreceiverby1suppplier(4, 2019, vendor) + Getqtyreceiverby1suppplier(5, 2019, vendor) + Getqtyreceiverby1suppplier(6, 2019, vendor)
            });
            res.Add(new ImprovestrackingViewmodel
            {
                No = "PPMYTD",
                JUL = Math.Round(res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() / Getqtyreceiverby1suppplier(7, 2018, vendor) * 1000000),
                AUG = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor)) * 1000000) ,
                SEP = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) ) * 1000000),              
                OCT = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor)) * 1000000),
                NOV = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor)) * 1000000) ,
                DEC = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault()+ res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor)) * 1000000),
                JAN = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor)) * 1000000),
                FEB = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault()+ res.Where(x => x.No == "DEFECTIVE").Select(x => x.FEB).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor)) * 1000000),
                MAR = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.FEB).FirstOrDefault() +res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAR).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor) + Getqtyreceiverby1suppplier(3, 2019, vendor)) * 1000000),
                APR = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.FEB).FirstOrDefault() +res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.APR).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor) + Getqtyreceiverby1suppplier(3, 2019, vendor) + Getqtyreceiverby1suppplier(4, 2019, vendor)) * 1000000),               
                MAY = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.FEB).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.APR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAY).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor) + Getqtyreceiverby1suppplier(3, 2019, vendor) + Getqtyreceiverby1suppplier(4, 2019, vendor) + Getqtyreceiverby1suppplier(5, 2019, vendor)) * 1000000),
                JUN = Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.FEB).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.APR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAY).FirstOrDefault() +res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUN).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor) + Getqtyreceiverby1suppplier(3, 2019, vendor) + Getqtyreceiverby1suppplier(4, 2019, vendor) + Getqtyreceiverby1suppplier(5, 2019, vendor) + Getqtyreceiverby1suppplier(6, 2019, vendor)) * 1000000),
                SUMYTD= Math.Round((res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUL).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.AUG).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.SEP).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.OCT).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.NOV).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.DEC).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.JAN).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.FEB).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.APR).FirstOrDefault() + res.Where(x => x.No == "DEFECTIVE").Select(x => x.MAY).FirstOrDefault() +res.Where(x => x.No == "DEFECTIVE").Select(x => x.JUN).FirstOrDefault()) / (Getqtyreceiverby1suppplier(7, 2018, vendor) + Getqtyreceiverby1suppplier(8, 2018, vendor) + Getqtyreceiverby1suppplier(9, 2018, vendor) + Getqtyreceiverby1suppplier(10, 2018, vendor) + Getqtyreceiverby1suppplier(11, 2018, vendor) + Getqtyreceiverby1suppplier(12, 2018, vendor) + Getqtyreceiverby1suppplier(1, 2019, vendor) + Getqtyreceiverby1suppplier(2, 2019, vendor) + Getqtyreceiverby1suppplier(3, 2019, vendor) + Getqtyreceiverby1suppplier(4, 2019, vendor) + Getqtyreceiverby1suppplier(5, 2019, vendor) + Getqtyreceiverby1suppplier(6, 2019, vendor)) * 1000000),
          
            });
            return res;
        }
        public List<DataRawViewmodel> GetDataRawRejQty4panelPPM(int year, int month, string id)
        {
            List<sp_Onesupplier4panelRecQtyDataRaw_Result> lst = _db.sp_Onesupplier4panelRecQtyDataRaw(year, month, id).ToList();
            List<DataRawViewmodel> res = (from ls in lst
                                          select (new DataRawViewmodel
                                          {
                                              NCRNUM = ls.NCR_NUM,
                                              LOT = ls.LOT,
                                              DEFECT = ls.DEFECT,
                                              DISCRIPTION = ls.disposition,
                                              ITEMDESC = ls.ITEM_DESC,
                                              MIPART = ls.MI_PART_NO,
                                              RECEIVER = ls.RECEIVER,
                                              RECQTY = ls.REC_QTY,
                                              REJQTY = ls.REJ_QTY,
                                              INS_QTY = ls.INS_QTY,
                                              NCDESC = ls.NC,
                                              DATEAPRROVAL = ls.DATEAPPROVAL,
                                          })).ToList();
            return res;
        }

        public List<string> GetPartbyDate(string dateSta, string dateDue)
        {
            try
            {
                string sqlQuery = $"select DISTINCT ITEM from RECEIVER WHERE POSTING_DATE >= '{ dateSta}' and POSTING_DATE  <= '{dateDue}'  UNION select DISTINCT MI_PART_NO from NCR_HDR WHERE DateApproval >= '{dateSta}' and DateApproval <= '{dateDue}'";
                List<string> listpart = _db.Database.SqlQuery<string>(sqlQuery).ToList();
                return listpart;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
            public List<string> Getsupplierbytype(string type,string dateSta, string dateDue)
            {
                try
                {
                if(type == "All")
                {//
                    string sqlQuery = $"select DISTINCT ITEM from RECEIVER WHERE POSTING_DATE >= '{ dateSta}' and POSTING_DATE  <= '{dateDue}'  UNION select DISTINCT MI_PART_NO from NCR_HDR WHERE DateApproval >= '{dateSta}' and DateApproval <= '{dateDue}'";
                    List<string> listpart = _db.Database.SqlQuery<string>(sqlQuery).ToList();
                    return listpart;
                }
                   else if(type == "Strategic")
                {//
                    string sqlQuery = $"select DISTINCT ITEM from RECEIVER WHERE POSTING_DATE >= '{ dateSta}' and POSTING_DATE  <= '{dateDue}'  UNION select DISTINCT MI_PART_NO from NCR_HDR WHERE DateApproval >= '{dateSta}' and DateApproval <= '{dateDue}'";
                    List<string> listpart = _db.Database.SqlQuery<string>(sqlQuery).ToList();
                    return listpart;
                }
                else
                {
                    string sqlQuery = $"select DISTINCT ITEM from RECEIVER WHERE POSTING_DATE >= '{ dateSta}' and POSTING_DATE  <= '{dateDue}'  UNION select DISTINCT MI_PART_NO from NCR_HDR WHERE DateApproval >= '{dateSta}' and DateApproval <= '{dateDue}'";
                    List<string> listpart = _db.Database.SqlQuery<string>(sqlQuery).ToList();
                    return listpart;
                }
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
                
        }
    }
}
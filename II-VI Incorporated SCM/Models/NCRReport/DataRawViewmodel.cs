using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCRReport
{
    public class DataRawViewmodel
    {
        public string NCRNUM { get; set; }
        public string SEC { get; set; }
        public string MIPART { get; set; }
        public string VENDOR { get; set; }
        public string RECEIVER { get; set; }
        public string LOT { get; set; }
        public string ITEMDESC { get; set; }
        public double? RECQTY { get; set; }
        public double? REJQTY { get; set; }
        public double? INS_QTY { get; set; }
        public string DISCRIPTION { get; set; }
        public string DEFECT { get; set; }
        public string NCDESC { get; set; }
        public DateTime? DATEAPRROVAL { get; set; }

    }
}
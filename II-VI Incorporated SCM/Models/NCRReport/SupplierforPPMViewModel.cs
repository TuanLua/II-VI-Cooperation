using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCRReport
{
    public class SupplierforPPMViewModel
    {
        public int FY { get; set; }
        public double FYCurrent { get; set; }
        public string TYPE { get; set; }
        public string TYPEPARENT { get; set; }
        public double JUL { get; set; }
        public double AUG { get; set; }
        public double SEP { get; set; }
        public double OCT { get; set; }
        public double NOV { get; set; }
        public double DEC { get; set; }
        public double JAN { get; set; }
        public double FEB { get; set; }
        public double MAR { get; set; }
        public double APR { get; set; }
        public double MAY { get; set; }
        public int? Sort { get; set; }
        public double JUN { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCRReport
{
    public class StrateryViewModel
    {
        public string Supplier { get; set; }
        public string CCN { get; set; }
        public DateTime? Date { get; set; }
        public double? ReceivedQty { get; set; }
        public double? RejectQTy { get; set; }
    }
}
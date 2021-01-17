using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class ListViewPorento
    {
        public string Description { get; set; }
        public string NC_CODEDES { get; set; }
        public double TotalQty { get; set; }
        public double TotalAccQty { get; set; }
        public double PercenAccQty { get; set; }
    }
}
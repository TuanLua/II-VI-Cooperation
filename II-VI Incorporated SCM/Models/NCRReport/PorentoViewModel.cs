using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCRReport
{
    public class PorentoViewModel
    {
        public string NCR_NUM { get; set; }
        public string SEC { get; set; }
        public string ITEM { get; set; }
        public double QTY { get; set; }
        //public List<ListViewPorento> NCCODE { get; set; }
        public string Description { get; set; }
        public string NC_CODEDES { get; set; }
        public double TotalQty { get; set; }
        public double TotalAccQty { get; set; }
        public double PercenAccQty { get; set; }
    }

}

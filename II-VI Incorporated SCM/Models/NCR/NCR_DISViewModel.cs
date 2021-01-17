using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class NCR_DISViewModel
    {
        public string Id { get; set; }
        public string ITEM { get; set; }
        public double QTY { get; set; }
        public string ADD_INS { get; set; }
        public string INSPECTOR { get; set; }
        public string REMARK { get; set; }
        public DateTime? DATEAPPROVAL { get; set; }
    }
}
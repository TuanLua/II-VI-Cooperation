using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class NcrDisViewmodel
    {
        public string Id { get; set; }
        public string NCR_NUM { get; set; }
        public string SEC { get; set; }
        public string ITEM { get; set; }
        public double QTY { get; set; }
        public string ADD_INS { get; set; }
        public string ADD_INS_NAME { get; set; }
        public string INSPECTOR { get; set; }
        public Nullable<System.DateTime> INS_DATE { get; set; }
        public string REMARK { get; set; }
        public Nullable<System.DateTime> MFG { get; set; }
        public Nullable<System.DateTime> QUALITY { get; set; }
        public Nullable<System.DateTime> PURCHASING { get; set; }
        public Nullable<System.DateTime> ENGIEERING { get; set; }
        public Nullable<System.DateTime> DATEAPPROVAL { get; set; }
        
        public int NO { get; set; }
    }
}
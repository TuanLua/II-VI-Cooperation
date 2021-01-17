using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class ApprovalDetViewmodel
    {
        public int ID { get; set; }
        public string NCR_NUMBER { get; set; }
        public string QUALITY { get; set; }
        public string ENGIEERING { get; set; }
        public string MFG { get; set; }
        public string PURCHASING { get; set; }
        public bool QUALITY_COMFIRM { get; set; }
        public bool ENGIEERING_CONFIRM { get; set; }
        public bool MFG_CONFIRM { get; set; }
        public bool PURCHASING_CONFIRM { get; set; }
        public Nullable<System.DateTime> QUALITY_DATE { get; set; }
        public Nullable<System.DateTime> ENGIEERING_DATE { get; set; }
        public Nullable<System.DateTime> MFG_DATE { get; set; }
        public Nullable<System.DateTime> PURCHASING_DATE { get; set; }
        public string Signature { get; set; }
    }
}
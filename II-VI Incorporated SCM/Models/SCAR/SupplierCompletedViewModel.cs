using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SCAR
{
    public class SupplierCompletedViewModel
    {
        public SCARInfoViewModel SCARInfo { get; set; }
        public SCAR_RESULT_D0 D0 { get; set; }
        public SCAR_RESULT_D1 D1 { get; set; }
        public SCAR_RESULT_D2 D2 { get; set; }
        public List<SCAR_RESULT_D3> D3 { get; set; }
        public SCAR_RESULT_D4 D4 { get; set; }
        public List<SCAR_RESULT_D5> D5 { get; set; }
        public List<SCAR_RESULT_D6> D6 { get; set; }
        public List<SCAR_RESULT_D7> D7 { get; set; }
        public SCARINFO D8 { get; set; }
    }
}
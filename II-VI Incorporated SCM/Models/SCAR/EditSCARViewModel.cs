using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SCAR
{
    public class EditSCARViewModel
    {
        public string SCAR_ID { get; set; }
        public string PROBLEM { get; set; }
        public DateTime DATEPROBLEM { get; set; }
        public DateTime DATERESPOND { get; set; }
        public string RECURING_PROBLEM { get; set; }
    }
}
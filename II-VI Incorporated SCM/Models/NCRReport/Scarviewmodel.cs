using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCRReport
{
    public class Scarviewmodel
    {
        public string No { get; set; }
        public string Corective { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime ActualDate { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }
    }
}
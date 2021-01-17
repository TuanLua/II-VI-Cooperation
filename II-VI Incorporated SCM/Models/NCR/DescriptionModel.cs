using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class DescriptionModel
    {
        public string label { get; set; }
        public List<Children> children { get; set; }
    }

    public class Children
    {
        public string label { get; set; }
        public string value { get; set; }
        public bool selected { get; set; }
        public string title { get; set; }
      //  public bool disabled { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SCAR
{
    public class SentMailViewModel
    {
        public int SCARID { get; set; }
        public string SCAR_ID { get; set; }
        public string SENTTO { get; set; }
        public string CC { get; set; }
        public string SUBJECT { get; set; }
        public string CONTENT { get; set; }
        public string FILENAME { get; set; }
        public string NCRFILE { get; set; }
        public string FILESCAR { get; set; }
        public HttpPostedFileBase SCARFILE { get; set; }
    }
}
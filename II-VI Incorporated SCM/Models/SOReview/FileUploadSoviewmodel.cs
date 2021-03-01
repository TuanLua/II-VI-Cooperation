using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SOReview
{
    public class FileUploadSoviewmodel
    {
        public string SONO { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }

        public DateTime DateDownLoad { get; set; }
    }
}
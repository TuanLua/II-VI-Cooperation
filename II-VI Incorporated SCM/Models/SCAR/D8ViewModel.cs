using II_VI_Incorporated_SCM.Library.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SCAR
{
    public class D8ViewModel
    {
        public string SCAR_ID { get; set; }
        public  int ID { get; set; }
        public string CONTENTD8 { get; set; }
        public string SUPPLIER_REPRESENTATIVE { get; set; }
        public Nullable<System.DateTime> DATE_D8 { get; set; }
        public string ACKNOWLEDGEMENT { get; set; }
        public string SCAR_STATUS { get; set; }
        [Required]
        [MaximumFileSizeValidator(10)]
        [ValidFileTypeValidator("pdf")]
        public HttpPostedFileBase File_Upload { get; set; }
    }
}
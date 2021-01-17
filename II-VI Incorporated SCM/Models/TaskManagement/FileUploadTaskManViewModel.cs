using II_VI_Incorporated_SCM.Library.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class FileUploadTaskManViewModel
    {
        public TASKDOCUMENT TaskDocument { get; set; }

        [Required]
        [MaximumFileSizeValidator(10)]
        [ValidFileTypeValidator("pdf")]
        public HttpPostedFileBase File_Upload { get; set; }
    }
}
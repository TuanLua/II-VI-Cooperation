using II_VI_Incorporated_SCM.Library.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.RTV
{
	public class RTVProccessViewModel
	{
	    public string NCR_NUMBER { get; set; }
	    public bool Shipped { get; set; }
	    public string TypeRTV { get; set; }
	    public Nullable<decimal> Qty { get; set; }
        public string Remark { get; set; }
	    public string CreditNote { get; set; }
	    public string CreditFile { get; set; }
	    public string RTVStatus { get; set; }

        [MaximumFileSizeValidator(10)]
        [ValidFileTypeValidator("pdf")]
        public HttpPostedFileBase File_Upload { get; set; }

        [MaximumFileSizeValidator(10)]
        [ValidFileTypeValidator("pdf")]
        public HttpPostedFileBase File_Upload1{ get; set; }
    }
}
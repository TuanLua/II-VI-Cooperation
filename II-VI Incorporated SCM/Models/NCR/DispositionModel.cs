using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class DispositionModel
    {
        public string NCR_NUM { get; set; }
        public bool NOT_REQUIRED { get; set; }
        public bool REQUIRED { get; set; }
        public bool NOTIFICATION_ONLY { get; set; }
        public string ISSUED_REQUEST_NO { get; set; }
        public DateTime ISSUED_REQUEST_DATE { get; set; }
        public string REMOVED_FROM { get; set; }
        public string BOOK_INV { get; set; }
        public string ISSUE_MEMO_NO { get; set; }
        public DateTime ISSUE_MEMO_DATE { get; set; }
        public string NOTES { get; set; }
        public string FOLLOW_UP_NOTES { get; set; }
        public string SHIPPING_METHOD { get; set; }
        public string RETURN_NUMBER { get; set; }

        public List<ResDispModel> lstResDis { get; set; }
    }
}
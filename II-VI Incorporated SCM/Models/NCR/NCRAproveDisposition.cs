using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.NCR
{
    public class NCRAproveDisposition
    {
        public List<UserApproval> Approvers { get; set; }
        public bool IsAllApprove { get; set; }
        public string ApprovalDate { get; set; }
    }
}
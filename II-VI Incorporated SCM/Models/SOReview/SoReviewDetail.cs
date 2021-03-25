using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.SOReview
{
    public class SoReviewDetail
    {
        public long ID { get; set; }
        public string SONO { get; set; }

        public DateTime DateDownLoad { get; set; }
        public string ItemReview { get; set; }

        public string DeptReview { get; set; }

        public string ReviewResult { get; set; }

        public string Comment { get; set; }

        public string LastReview { get; set; }

        public string LastComment { get; set; }

        public string PlanShipDate { get; set;}
        public string IsLock { get; set; }
        public string Item { get; set; }
        public List<EvidenceView> ModelEvidence { get; set; }
        public List<tbl_SOR_Attached_ForItemReview> OldEvidence { get; set; }

    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace II_VI_Incorporated_SCM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_SOR_Cur_Review_List
    {
        public long REVIEW_ID { get; set; }
        public System.DateTime DOWNLOAD_DATE { get; set; }
        public string SO_NO { get; set; }
        public string ANALYST { get; set; }
        public string SO_ON_HOLD { get; set; }
        public string SHIP_TO { get; set; }
        public string ITEM { get; set; }
        public string FAI { get; set; }
        public Nullable<bool> NEW_REVIEW { get; set; }
        public string LAST_BUILD_DR_REV { get; set; }
        public string LAST_REVIEW_DR_REV { get; set; }
        public string DR_REV { get; set; }
        public Nullable<System.DateTime> REQUIRED_DATE { get; set; }
        public Nullable<System.DateTime> SCHEDULE_DATE { get; set; }
        public Nullable<System.DateTime> PROMISE_DATE { get; set; }
        public Nullable<double> ORD_QTY { get; set; }
        public Nullable<double> BLC_QTY { get; set; }
        public Nullable<double> BLC_VALUE { get; set; }
        public Nullable<double> ONHAND_QTY { get; set; }
        public string REVIEW_STATUS { get; set; }
        public string PLAN_SHIP_DATE { get; set; }
        public string LINE { get; set; }
    }
}

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
    
    public partial class PARTIAL_INS_RESULT
    {
        public Nullable<int> ID { get; set; }
        public int PartialID { get; set; }
        public Nullable<System.DateTime> InsDate { get; set; }
        public Nullable<double> PartialIns { get; set; }
        public Nullable<double> PartialAcc { get; set; }
        public Nullable<double> PartialRej { get; set; }
        public string Inspector { get; set; }
        public string Remark { get; set; }
    }
}

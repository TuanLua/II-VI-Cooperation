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
    
    public partial class tbl_SQE_SupplierLog_Attached
    {
        public int ID { get; set; }
        public string Supplier_Code { get; set; }
        public string File_Name { get; set; }
        public string File_Rev { get; set; }
        public string File_Type { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> Upload_At { get; set; }
        public string Upload_By { get; set; }
        public string Upload_For { get; set; }
    }
}

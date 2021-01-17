using System;
using System.ComponentModel.DataAnnotations;

namespace II_VI_Incorporated_SCM.Models
{
    public class NCRPRO
    {
        public string No
        {
            get;
            set;
        }
        public string PartNumBer
        {
            get;
            set;
        }
        public string Supplier
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
       
        [Required]
        public DateTime? Date
        {
            get;
            set;
        }
        public int? InspQty
        {
            get;
            set;
        }
        public int? RejectQty
        {
            get;
            set;
        }
        public string Receiver
        {
            get;
            set;
        }
        public string Inspector
        {
            get;
            set;
        }
        public int? PO
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
    }
}
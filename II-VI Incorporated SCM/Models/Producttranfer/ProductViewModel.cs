using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.Producttranfer
{
    public class ProductViewModel
    {
        public string Part_Num { get; set; }
        public string Description { get; set; }
        public string Plan_Type { get; set; }
        public DateTime? Date { get; set; }
        public string Initial_Note { get; set; }
        public string Build_Loc { get; set; }
        public string Wc { get; set; }
        public Double? Plan_Yield { get; set; }
        public string Vn_Owner { get; set; }
        public string Setion { get; set; }
        public string PE_Note { get; set; }
        public string Initail_User { get; set; }
        public string PE_User { get; set; }
        public string StatusInfo { get; set; }
        public string StatusStep { get; set; }
        public bool StepStatus { get; set; }
        public string type { get; set; }
        public List<string> Listpart { get; set; }
        public List<checklistViewmodel> Checklist { get; set; }
        public double? Yield { get; set; }
        public string GM { get; set; }
        public string Conlusion { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string filename { get; set; }
        public string edit { get; set; }
        //checksheet
        public string Item_Desc { get; set; }
        public string Data_Type { get; set; }
        public string Item_Remark { get; set; }
        public bool? Is_Require { get; set; }
        public string Item_Value { get; set; }
        public byte? Item_Index { get; set; }
    }

    public class checklistViewmodel
    {
        public string Part_Num { get; set; }
        public byte Item_Idx { get; set; }
        public string Item_Desc { get; set; }
        public string Data_Type { get; set; }
        public string Item_Remark { get; set; }
        public bool? Is_Require { get; set; }
        public string Item_Value { get; set; }

    }
    public class checklistview
    {
        public int key { get; set; }
        public string value { get; set; }
    }
    public class ListTaskModel
    {
        public string ID { get; set; }
        public List<string> TASK_NAME { get; set; }
    }
}
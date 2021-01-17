using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskListViewModel
    {
        public int TopicID { get; set; }
        public string Topic { get; set; }
        public string TYPE { get; set; }
        public string WRITTENBY { get; set; }
        public Nullable<System.DateTime> WRITEDATE { get; set; }
        public string Task_Detail { get; set; }
        public Nullable<int> Level { get; set; }
        public string Reference { get; set; }
    }
}
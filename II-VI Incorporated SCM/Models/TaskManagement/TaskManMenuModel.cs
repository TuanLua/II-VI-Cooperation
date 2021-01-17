using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskManMenuModel
    {
        public IEnumerable<TASKLIST> lsTaskList { get; set; }
        public int NCRCount { get; set; }
        public int SCARCount { get; set; }
        public int SUPPCount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskListReporModel
    {
        public string Department { get; set; }
        public string TaskName { get; set; }
        public string IdTask { get; set; }
        public string Topic { get; set; }
        public string Type { get; set; }
        public DateTime EstimateEndDate { get; set; }
        public string Status { get; set; }
        public int Aging { get; set; }

    }

}
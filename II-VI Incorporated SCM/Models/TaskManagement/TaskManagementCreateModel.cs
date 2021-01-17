using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskManagementCreateModel
    {
        public TASKLIST TaskList { get; set; }
        public TaskDetailViewModel TaskDetail { get; set; }
    }
}
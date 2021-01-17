using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskStatisticalModel
    {
        string Department { get; set; }
        int Closed { get; set; }
        int Late { get; set; }
        int Open { get; set; }
    }
}
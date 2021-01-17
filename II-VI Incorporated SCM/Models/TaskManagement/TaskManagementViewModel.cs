using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
	public class TaskmanagementViewmodel
	{
        public string RefNUMBER { get; set; }
        public string Taskname { get; set; }
        public string TaskDescription { get; set; }
        public string Owner { get; set; }
        public string Assignee { get; set; }
        public string Approve { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public string Late { get; set; }
        public DateTime? ActualStarDay { get; set; }
        public DateTime? ActualEndDay { get; set; }
        public string Priority { get; set; }
        public int Taskno { get; set; }
        public int? TaskDetailID { get; set; }
        public DateTime? INSDATEs { get; set; }
        public string Type { get; set; }
        public int? Level { get; set; }
        public int? TopicID { get; set; }
        public string Topic { get; set; }
        public bool hasChildren { get; set; }
    }
    //public class TaskmanagementTest
    //{
    //    public string RefNUMBER { get; set; }
    //    public string Taskname { get; set; }
    //    public string TaskDescription { get; set; }
    //    public string Owner { get; set; }
    //    public string Assignee { get; set; }
    //    public string Approve { get; set; }
    //    public DateTime? StartDay { get; set; }
    //    public DateTime? DueDate { get; set; }
    //    public string Status { get; set; }
    //    public DateTime? ActualStarDay { get; set; }
    //    public DateTime? ActualEndDay { get; set; }
    //    public string Priority { get; set; }
    //    public int Taskno { get; set; }
    //    public string TaskDetailID { get; set; }
    //    public DateTime? INSDATEs { get; set; }
    //    public string Type { get; set; }
    //    public int? Level { get; set; }
    //    public string TopicID { get; set; }
    //    public bool hasChildren { get; set; }
    //}

    public class TaskmanagementTest
    {
        public int? IDTask { get; set; }
        public int? Reference { get; set; }
        public int? Level { get; set; }
        public string Type { get; set; }
        public int Topic { get; set; }
        public string Taskname { get; set; }
        public bool hasChildren { get; set; }
    }
}
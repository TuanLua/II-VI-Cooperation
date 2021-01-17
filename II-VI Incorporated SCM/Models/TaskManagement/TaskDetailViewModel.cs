using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskDetailViewModel
    {
        public int TaskID { get; set; }
        public Nullable<int> TopicID { get; set; }
        [Required]
        [Display(Name = "TASK NAME")]
        public string TASKNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string OWNER { get; set; }
        [Required]
        [Display(Name = "ASSIGNEE")]
        public string ASSIGNEE { get; set; }
        [Required]
        [Display(Name = "APPROVE")]
        public string APPROVE { get; set; }
        [Display(Name = "START DATE")]
        public Nullable<System.DateTime> EstimateStartDate { get; set; }
        [Required]
        [Display(Name = "DUE DATE")]
        public Nullable<System.DateTime> EstimateEndDate { get; set; }
        public Nullable<System.DateTime> ActualStartDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        [Display(Name = "PROGESS")]
        public Nullable<int> PROCESS { get; set; }
        [Required]
        [Display(Name = "EST_COMPLETEION DATE")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string STATUS { get; set; }
        public string PRIORITY { get; set; }
    }
}
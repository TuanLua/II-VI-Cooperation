using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.TaskManagement
{
    public class TaskManagementNCRViewModel
    {
        public TASKLIST TaskList { get; set; }
        //public TASKDETAIL TaskDetail { get; set; }
        public TaskDetailViewModel TaskDetail { get; set; }
        public IEnumerable<TASKCOMMENT> TaskComments { get; set; }
        public IEnumerable<TASKDOCUMENT> TaskDocuments { get; set; }
        public int DocumentCount { get; set; }
        public string OwnerName { get; set; }
        public string AssigneeName { get; set; }
        public string OpproverName { get; set; }
        public string LastComment { get; set; }
        public string CONTENTCOMMENT { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        [Required]
        [Display(Name = "APPROVE")]
        public List<string> ListApprove { get; set; }
        public FileUploadTaskManViewModel FileUpload { get; set; }
    }
}
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.Account;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using II_VI_Incorporated_SCM.Models.NCR;
using System.Globalization;

namespace II_VI_Incorporated_SCM.Services
{
    //interface
    public interface ITaskManagementService
    {
        TaskListViewModel GetTaskListID(int ID);
        TASKLIST getcurrentTask();
        string GetTypeCategory(string Num);
        TaskListViewModel GetTaskList(string Num);
        string GetUrlCategory(string TypeCate);
        List<SelectListItem> GetdropdownTypeTask();
        TASKLIST GetTaskListByTaskNONCR(string taskNO);
        List<TaskmanagementViewmodel> GetListTask();
        List<TaskmanagementViewmodel> GetListTask(string keyword);
        List<string> GetCheckedbyid(string id);
        List<string> GetPhaseTaskbyid(string id);
        List<string> GetTypeTaskbyid(string id);
        IEnumerable<TaskManagementNCRViewModel> GetListTaskMantNCRByID(int taskID);
        List<TaskmanagementViewmodel> getlisttasklist();
        IEnumerable<AspNetUserViewModel> GetListUserViews();
        bool AddTaskManNCR(TaskManagementNCRViewModel model,string iduser);
        TASKDOCUMENT SaveFileTaskMan(FileUploadTaskManViewModel fileUploadModel);
        TASKDOCUMENT GetTaskDocFileWithFileID(int fileId);
        bool DeleteTaskDocFileWithFileID(int fileId);
        TASKCOMMENT SaveCommentTaskMan(TASKCOMMENT commentUploadModel);
        TaskManagementNCRViewModel GetTaskManNCRByTaskID(int taskID);
        TASKLIST GetTaskListByTaskID(int taskID);
        bool UpdateStatusTaskMan(List<int> id, string status);
        bool DeleteTaskMan(List<int> id,string iduser);
        bool ExistsTask(string NCR_NUM);
        List<Children> GetDropdownlistUser();
        TASKDETAIL GetTaskDetailByTaskDetailID(int taskDetailID);
        List<Children> GetDropdownlistUserEdit(int taskDetailID);
        TASKLIST GetTaskListByTaskNO(string taskNO,  string type ="",string uid = "");
        List<TaskmanagementViewmodel> GetListTaskNCR();
        Result AddTaskNCRDispositionApproval(string nCRNUM, string Partial);
        bool UpdateStatusTaskManHis(List<int> id, string status, string iduser,string comment);
        bool CheckOwnerTask(int Idtask,string iduser);
        bool CHeckApproveTask(int IDTask, string iduser);
        bool CHeckAssigneeTask(int IDTask, string iduser);
        bool SaveTaskList(string type, string topic, string topicdetail, string userid,string reference,string phase,string phase2);
        bool CheckOwnerCreateFileTask(int Idtask, string iduser);
        Result AddTaskProductTranfer(string PartNum, string iduser, string Onwer);
        Result AddTaskProductTranfer_BuyPart(string PartNum, string iduser);
        int TaskIDCurrent();
        List<AspNetUser> getuser();
        string GetcurrentTaskListCreate();
        List<SelectListItem> GetDeptList();
        List<SelectListItem> GetUserList();
        List<sp_TaskList_Report_Result> get_TaskList_Report(string Dept, string TaskStatus, DateTime DateFrom, DateTime DateTo);
        List<sp_TaskStatistical_Report_Result> get_TaskStatistical_Report(string Dept, string TaskStatus, DateTime DateFrom, DateTime DateTo);
        List<sp_Task_Statistical_ByUser_Result> Get_Task_Statistical_ByUser(string user);
        List<sp_Task_Statistical_ByDept_Result> Get_Task_Statistical_ByDept(string dept);
        List<sp_Task_Statistical_ByDetail_Result> Get_Task_Statistical_ByDetail(string SearchType, string SearchValue, string TaskStatus);
        List<sp_Task_Search_Result> Get_Task_BykeyWord(string key);
        IEnumerable<TaskManagementNCRViewModel> GetListTaskMantSoreviewByID(string taskNo);
    }

    //services
    public class TaskManagementService : ITaskManagementService
    {
        #region InitDB
        private IIVILocalDB _db;

        public TaskManagementService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        #endregion

        #region Services
        /// <summary>
        /// Get List TaskMant NCR
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaskManagementNCRViewModel> GetListTaskMantNCRByID(int taskID)
        {
            List<TaskManagementNCRViewModel> lsTaskMantNCR = new List<TaskManagementNCRViewModel>();
            foreach (var taskDetail in _db.TASKDETAILs.ToList())
            {
                if (taskDetail.TopicID == taskID)
                {
                    TaskManagementNCRViewModel taskNCRView = new TaskManagementNCRViewModel();

                    //TakList
                    TASKLIST taskList = new TASKLIST();
                    taskList = GetTaskListByID(taskID);
                    taskNCRView.TaskList = taskList;
                    //taskNCRView.TaskList = new TASKLIST();
                    //taskNCRView.TaskList.ID = taskDetail.TASKID;
                    //taskNCRView.TaskList.TASKNO = taskDetail.TASKNO;
                    //taskNCRView.TaskList.TYPE = taskDetail.TYPE;
                    //taskNCRView.TaskList.WRITTENBY = taskDetail.WRITTENBY;
                    //taskNCRView.TaskList.WRITEDATE = taskDetail.WRITEDATE;

                    //TaskDetail
                    //taskNCRView.TaskDetail = taskDetail;
                    taskNCRView.TaskDetail = new TaskDetailViewModel
                    {
                        TaskID = taskDetail.IDTask,
                        TopicID = taskDetail.TopicID,
                        TASKNAME = taskDetail.TASKNAME,
                        DESCRIPTION = taskDetail.DESCRIPTION,
                        OWNER = taskDetail.OWNER,
                        ASSIGNEE = taskDetail.ASSIGNEE,
                        APPROVE = taskDetail.APPROVE,
                        EstimateStartDate = taskDetail.EstimateStartDate,
                        EstimateEndDate = taskDetail.EstimateEndDate,
                        ActualStartDate = taskDetail.ActualStartDate,
                        ActualEndDate = taskDetail.ActualEndDate,
                        PROCESS = taskDetail.PROCESS,
                        CreateDate = taskDetail.CreatedDate,
                        STATUS = taskDetail.STATUS,
                        PRIORITY = taskDetail.PRIORITY
                    };

                    string[] lsApp;
                    if (taskNCRView.TaskDetail.APPROVE != null)
                    {
                        taskNCRView.ListApprove = new List<string>();
                        lsApp = taskDetail.APPROVE.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                        foreach (var app in lsApp)
                        {
                            taskNCRView.ListApprove.Add(app);
                            taskNCRView.OpproverName = taskNCRView.OpproverName + GetUserNameByID(app) + "<br/>";
                        }
                        taskNCRView.OpproverName = taskNCRView.OpproverName.Substring(0, taskNCRView.OpproverName.Length - 5);
                    }
                    //TASKDETAIL taskDetail = new TASKDETAIL();
                    //taskDetail = GetTaskDetailByID(taskDetail.ID);
                    //taskNCRView.TaskDetail = new TASKDETAIL();
                    //taskNCRView.TaskDetail.ID = taskDetail.ID;
                    //taskNCRView.TaskDetail.TASKID = taskDetail.TASKID;
                    //taskNCRView.TaskDetail.TASKNAME = taskDetail.TASKNAME;
                    //taskNCRView.TaskDetail.DESCRIPTION = taskDetail.DESCRIPTION;
                    //taskNCRView.TaskDetail.OWNER = taskDetail.OWNER;
                    //taskNCRView.TaskDetail.ASSIGNEE = taskDetail.ASSIGNEE;
                    //taskNCRView.TaskDetail.APPROVE = taskDetail.APPROVE;
                    //taskNCRView.TaskDetail.STARTDATE = taskDetail.STARTDATE;
                    //taskNCRView.TaskDetail.DUEDATE = taskDetail.DUEDATE;
                    //taskNCRView.TaskDetail.CORRECTSTARTDATE = taskDetail.CORRECTSTARTDATE;
                    //taskNCRView.TaskDetail.CORRECTENDDATE = taskDetail.CORRECTENDDATE;
                    //taskNCRView.TaskDetail.PROCESS = taskDetail.PROCESS;
                    //taskNCRView.TaskDetail.EST_COMPLETEIONDATE = taskDetail.EST_COMPLETEIONDATE;
                    //taskNCRView.TaskDetail.STATUS = taskDetail.STATUS;
                    //taskNCRView.TaskDetail.PRIORITY = taskDetail.PRIORITY;

                    //List TaskComment
                    List<TASKCOMMENT> lsComment = new List<TASKCOMMENT>();
                    lsComment = GetListTaskCommentByID(taskDetail.IDTask).ToList();
                    taskNCRView.TaskComments = new List<TASKCOMMENT>();
                    taskNCRView.TaskComments = lsComment;

                    if (lsComment.Count > 0)
                    {
                        string contentComm = lsComment.LastOrDefault().CONTENTCOMMENT;
                        string ownerComm = GetUserNameByID(lsComment.LastOrDefault().WRITTENBY);
                        string lastComm = contentComm + ".<br/> by: " + ownerComm;
                        taskNCRView.LastComment = lastComm;
                    }

                    //taskNCRView.LastComment = "";

                    //List TaskDocument
                    List<TASKDOCUMENT> lsDocument = new List<TASKDOCUMENT>();
                    lsDocument = GetListTaskDoucumentByID(taskDetail.IDTask).ToList();
                    taskNCRView.TaskDocuments = new List<TASKDOCUMENT>();
                    taskNCRView.TaskDocuments = lsDocument;

                    taskNCRView.DocumentCount = lsDocument.Count;
                    taskNCRView.OwnerName = GetUserNameByID(taskDetail.OWNER);
                    taskNCRView.AssigneeName = GetUserNameByID(taskDetail.ASSIGNEE);
                    //taskNCRView.OpproverName = GetUserNameByID(taskDetail.APPROVE);

                    //foreach (var app in lsApp)
                    //{
                    //    taskNCRView.OpproverName = taskNCRView.OpproverName + GetUserNameByID(app) + "<br/>";
                    //}
                    //taskNCRView.OpproverName = taskNCRView.OpproverName.Substring(0, taskNCRView.OpproverName.Length - 5);

                    lsTaskMantNCR.Add(taskNCRView);
                }
            }
            return lsTaskMantNCR;
        }

        public IEnumerable<TaskManagementNCRViewModel> GetListTaskMantSoreviewByID(string taskNo)
        {
            int taskID = _db.TASKLISTs.Where(x => x.Topic == taskNo).FirstOrDefault().TopicID;
            List<TaskManagementNCRViewModel> lsTaskMantNCR = new List<TaskManagementNCRViewModel>();
            foreach (var taskDetail in _db.TASKDETAILs.ToList())
            {
                if (taskDetail.TopicID == taskID)
                {
                    TaskManagementNCRViewModel taskNCRView = new TaskManagementNCRViewModel();

                    //TakList
                    TASKLIST taskList = new TASKLIST();
                    taskList = GetTaskListByID(taskID);
                    taskNCRView.TaskList = taskList;
                    //taskNCRView.TaskList = new TASKLIST();
                    //taskNCRView.TaskList.ID = taskDetail.TASKID;
                    //taskNCRView.TaskList.TASKNO = taskDetail.TASKNO;
                    //taskNCRView.TaskList.TYPE = taskDetail.TYPE;
                    //taskNCRView.TaskList.WRITTENBY = taskDetail.WRITTENBY;
                    //taskNCRView.TaskList.WRITEDATE = taskDetail.WRITEDATE;

                    //TaskDetail
                    //taskNCRView.TaskDetail = taskDetail;
                    taskNCRView.TaskDetail = new TaskDetailViewModel
                    {
                        TaskID = taskDetail.IDTask,
                        TopicID = taskDetail.TopicID,
                        TASKNAME = taskDetail.TASKNAME,
                        DESCRIPTION = taskDetail.DESCRIPTION,
                        OWNER = taskDetail.OWNER,
                        ASSIGNEE = taskDetail.ASSIGNEE,
                        APPROVE = taskDetail.APPROVE,
                        EstimateStartDate = taskDetail.EstimateStartDate,
                        EstimateEndDate = taskDetail.EstimateEndDate,
                        ActualStartDate = taskDetail.ActualStartDate,
                        ActualEndDate = taskDetail.ActualEndDate,
                        PROCESS = taskDetail.PROCESS,
                        CreateDate = taskDetail.CreatedDate,
                        STATUS = taskDetail.STATUS,
                        PRIORITY = taskDetail.PRIORITY
                    };

                    string[] lsApp;
                    if (taskNCRView.TaskDetail.APPROVE != null)
                    {
                        taskNCRView.ListApprove = new List<string>();
                        lsApp = taskDetail.APPROVE.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                        foreach (var app in lsApp)
                        {
                            taskNCRView.ListApprove.Add(app);
                            taskNCRView.OpproverName = taskNCRView.OpproverName + GetUserNameByID(app) + "<br/>";
                        }
                        taskNCRView.OpproverName = taskNCRView.OpproverName.Substring(0, taskNCRView.OpproverName.Length - 5);
                    }
                    //TASKDETAIL taskDetail = new TASKDETAIL();
                    //taskDetail = GetTaskDetailByID(taskDetail.ID);
                    //taskNCRView.TaskDetail = new TASKDETAIL();
                    //taskNCRView.TaskDetail.ID = taskDetail.ID;
                    //taskNCRView.TaskDetail.TASKID = taskDetail.TASKID;
                    //taskNCRView.TaskDetail.TASKNAME = taskDetail.TASKNAME;
                    //taskNCRView.TaskDetail.DESCRIPTION = taskDetail.DESCRIPTION;
                    //taskNCRView.TaskDetail.OWNER = taskDetail.OWNER;
                    //taskNCRView.TaskDetail.ASSIGNEE = taskDetail.ASSIGNEE;
                    //taskNCRView.TaskDetail.APPROVE = taskDetail.APPROVE;
                    //taskNCRView.TaskDetail.STARTDATE = taskDetail.STARTDATE;
                    //taskNCRView.TaskDetail.DUEDATE = taskDetail.DUEDATE;
                    //taskNCRView.TaskDetail.CORRECTSTARTDATE = taskDetail.CORRECTSTARTDATE;
                    //taskNCRView.TaskDetail.CORRECTENDDATE = taskDetail.CORRECTENDDATE;
                    //taskNCRView.TaskDetail.PROCESS = taskDetail.PROCESS;
                    //taskNCRView.TaskDetail.EST_COMPLETEIONDATE = taskDetail.EST_COMPLETEIONDATE;
                    //taskNCRView.TaskDetail.STATUS = taskDetail.STATUS;
                    //taskNCRView.TaskDetail.PRIORITY = taskDetail.PRIORITY;

                    //List TaskComment
                    List<TASKCOMMENT> lsComment = new List<TASKCOMMENT>();
                    lsComment = GetListTaskCommentByID(taskDetail.IDTask).ToList();
                    taskNCRView.TaskComments = new List<TASKCOMMENT>();
                    taskNCRView.TaskComments = lsComment;

                    if (lsComment.Count > 0)
                    {
                        string contentComm = lsComment.LastOrDefault().CONTENTCOMMENT;
                        string ownerComm = GetUserNameByID(lsComment.LastOrDefault().WRITTENBY);
                        string lastComm = contentComm + ".<br/> by: " + ownerComm;
                        taskNCRView.LastComment = lastComm;
                    }

                    //taskNCRView.LastComment = "";

                    //List TaskDocument
                    List<TASKDOCUMENT> lsDocument = new List<TASKDOCUMENT>();
                    lsDocument = GetListTaskDoucumentByID(taskDetail.IDTask).ToList();
                    taskNCRView.TaskDocuments = new List<TASKDOCUMENT>();
                    taskNCRView.TaskDocuments = lsDocument;

                    taskNCRView.DocumentCount = lsDocument.Count;
                    taskNCRView.OwnerName = GetUserNameByID(taskDetail.OWNER);
                    taskNCRView.AssigneeName = GetUserNameByID(taskDetail.ASSIGNEE);
                    //taskNCRView.OpproverName = GetUserNameByID(taskDetail.APPROVE);

                    //foreach (var app in lsApp)
                    //{
                    //    taskNCRView.OpproverName = taskNCRView.OpproverName + GetUserNameByID(app) + "<br/>";
                    //}
                    //taskNCRView.OpproverName = taskNCRView.OpproverName.Substring(0, taskNCRView.OpproverName.Length - 5);

                    lsTaskMantNCR.Add(taskNCRView);
                }
            }
            return lsTaskMantNCR;
        }
        public List<TaskmanagementViewmodel> getlisttasklist()
        {
            var result =from task in _db.TASKLISTs
                        join p in _db.AspNetUsers on task.WRITTENBY equals p.Id into ps
                        from p in ps.DefaultIfEmpty()
                        select new TaskmanagementViewmodel { 
                        Taskname = task.Topic,
                        INSDATEs = task.WRITEDATE,
                        Owner = p.FullName,
                        TaskDescription = task.Task_Detail,
                        Taskno =task.TopicID,
                        Type = task.TYPE,
                        Level = task.Level,
                        RefNUMBER = task.Reference
                        };
            return result.ToList();
        }
        /// <summary>
        /// Get List User Views
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AspNetUserViewModel> GetListUserViews()
        {
            List<AspNetUserViewModel> lsUser = new List<AspNetUserViewModel>();
            foreach (var item in _db.AspNetUsers.ToList())
            {
                lsUser.Add(new AspNetUserViewModel
                {
                    Id = item.Id,
                    FullName = item.FullName
                });
            }
            return lsUser;
        }

        /// <summary>
        /// Add TaskManNCR
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
         public List<AspNetUser> getuser()
        {
            var result = _db.AspNetUsers.ToList();
            return result;
        }
        public List<SelectListItem> GetdropdownTypeTask()
        {
            List<SelectListItem> listvendor = _db.TASK_CATEGORY.Where(x=>x.IS_ACTIVE == true).Select(x => new SelectListItem
            {
                Value = x.CATEGORY,
                Text = x.CATEGORY.Trim()
            }).ToList();
            return listvendor;
        }
        public int TaskIDCurrent()
        {
            var result = _db.TASKDETAILs.ToList();
            result.OrderByDescending(x => x.IDTask);
            return result.LastOrDefault().IDTask;
        }
        public List<string> GetCheckedbyid(string id)
        {
            try
            {
                List<string> listpart = _db.TASK_CATEGORY.Where(x => x.CATEGORY == id && x.REF_NUM == "R").Select(x => x.CATEGORY).ToList();
                return listpart;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        public List<string> GetTypeTaskbyid(string id)
        {
            try
            {
                List<string> listpart=null;//= _db.v_TASK_REF.Where(x => x.TASK_TYPE == id).Select(x => x.REF_NUM).ToList();
                return listpart;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        public List<string> GetPhaseTaskbyid(string id)
        {
            try
            {
             var listpart = _db.TASK_PHASE.Where(x => x.CATEGORY.ToString() == id).ToList();
                var result  = listpart.OrderByDescending(s => s.PHASE_ID);
                List<string> listphase = result.Select(x => x.PHASE).ToList();
                return listphase;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        public bool AddTaskManNCR(TaskManagementNCRViewModel model,string iduser)
        {
            try
            {
                int currentTaskListID = 0;
                int currentTaskDetailID = 0;
                if (model.TaskDetail.ActualEndDate != null && model.TaskDetail.STATUS == "Created")
                {
                    model.TaskDetail.STATUS = "Completed";
                }
                if (model.TaskList.TopicID == 0)
                {
                    _db.TASKLISTs.Add(model.TaskList);
                    _db.SaveChanges();
                    currentTaskListID = model.TaskList.TopicID;

                    //model.TaskDetail.TASKID = currentID;
                    //dbContext.TASKDETAILs.Add(model.TaskDetail);
                    //dbContext.SaveChanges();
                }
                else
                {
                    ////var taskList = dbContext.TASKLISTs.Where(m => m.ID == model.TaskList.ID).FirstOrDefault();
                    ////taskList = model.TaskList;
                    //dbContext.Entry(model.TaskList).State = EntityState.Modified;
                    //dbContext.SaveChanges();
                    currentTaskListID = model.TaskList.TopicID;

                    ////var taskDetail = dbContext.TASKDETAILs.Where(m => m.TASKID == model.TaskList.ID).FirstOrDefault();
                    ////taskDetail = model.TaskDetail;
                    //model.TaskDetail.TASKID = currentID;
                    //dbContext.Entry(model.TaskDetail).State = EntityState.Modified;
                    //dbContext.SaveChanges();
                }

                if (model.TaskDetail.TaskID == 0)
                {
                    model.TaskDetail.TopicID = currentTaskListID;
                    TASKDETAIL taskDetail = new TASKDETAIL
                    {
                        IDTask = model.TaskDetail.TaskID,
                        TopicID = currentTaskListID,
                        TASKNAME = model.TaskDetail.TASKNAME,
                        DESCRIPTION = model.TaskDetail.DESCRIPTION,
                        OWNER = model.TaskDetail.OWNER,
                        ASSIGNEE = model.TaskDetail.ASSIGNEE,
                        APPROVE = model.TaskDetail.APPROVE,
                        EstimateStartDate = model.TaskDetail.EstimateStartDate,
                        EstimateEndDate = model.TaskDetail.EstimateEndDate,
                        ActualStartDate = model.TaskDetail.ActualStartDate,
                        ActualEndDate = model.TaskDetail.ActualEndDate,
                        PROCESS = model.TaskDetail.PROCESS,
                        CreatedDate = DateTime.Now,
                        STATUS = model.TaskDetail.STATUS,
                        PRIORITY = model.TaskDetail.PRIORITY,
                        Level =1
                    };
                    _db.TASKDETAILs.Add(taskDetail);

                    //Thi.Nguyen: Update eSuggestion status after created task
                    if (model.TaskList.TYPE == "eSuggestion")
                    {
                        var rst = _db.tbl_Inv_Apr_Process.Where(x => x.Sug_ID == model.TaskList.Topic && x.Step_ID == "2").ToList();
                        //Each step just have one row data
                        if (rst.Count > 0) return true;
                        tbl_Inv_Apr_Process data = new tbl_Inv_Apr_Process();
                        data.Status = "AssignedTask";
                        data.Step_ID = "2";
                        data.Sug_ID = model.TaskList.Topic;
                        data.Step_Idx = "2";
                        data.Action_Date = DateTime.Now;
                        //Remove because this function also use for AssignedTask: data.Comment = "Similar to others: "+comment;
                        data.Comment = "";
                        _db.tbl_Inv_Apr_Process.Add(data);
                        //Update current step up suggestion
                        var Suggestion = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_ID == model.TaskList.Topic).FirstOrDefault();
                        Suggestion.Cur_Step = "2";
                        _db.sp_Inv_SendMail_Reject(model.TaskList.Topic, 2, "AssignedTask");
                    }
                    //End Update

                    _db.SaveChanges();
                    currentTaskDetailID = taskDetail.IDTask;
                    
                }
                else
                {
                    model.TaskDetail.TopicID = currentTaskListID;
                   // TASKDETAIL taskDetailCHeck = _db.TASKDETAILs.Where(x => x.IDTask == model.TaskDetail.TaskID).FirstOrDefault();
                    if (model.TaskDetail.STATUS == "Created")
                    {
                        TASKDETAIL taskDetail = new TASKDETAIL
                        {
                            IDTask = model.TaskDetail.TaskID,
                            TopicID = currentTaskListID,
                            TASKNAME = model.TaskDetail.TASKNAME,
                            DESCRIPTION = model.TaskDetail.DESCRIPTION,
                            OWNER = model.TaskDetail.OWNER,
                            ASSIGNEE = model.TaskDetail.ASSIGNEE,
                            APPROVE = model.TaskDetail.APPROVE,
                            EstimateStartDate = model.TaskDetail.EstimateStartDate,
                            EstimateEndDate = model.TaskDetail.EstimateEndDate,
                            ActualStartDate = model.TaskDetail.ActualStartDate,
                            ActualEndDate = model.TaskDetail.ActualEndDate,
                            PROCESS = model.TaskDetail.PROCESS,
                            CreatedDate = DateTime.Now,
                            STATUS = model.TaskDetail.STATUS,
                            PRIORITY = model.TaskDetail.PRIORITY,
                            Level = 1
                        };
                         _db.Entry(taskDetail).State = EntityState.Modified;
                        _db.SaveChanges();
                        currentTaskDetailID = taskDetail.IDTask;

                        
                    }
                    if (model.TaskDetail.STATUS == "Completed")
                    {
                        TASKDETAIL taskDetail = _db.TASKDETAILs.Where(x => x.IDTask == model.TaskDetail.TaskID).FirstOrDefault();
                        {
                            taskDetail.ActualStartDate = model.TaskDetail.ActualStartDate;
                            taskDetail.ActualEndDate = model.TaskDetail.ActualEndDate;
                            taskDetail.STATUS = model.TaskDetail.STATUS;
                            taskDetail.Level = 1;
                        };
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        _db.SaveChanges();
                        currentTaskDetailID = taskDetail.IDTask;
                    }
                    if (model.TaskDetail.STATUS == "Reject" || model.TaskDetail.STATUS == "Reopen")
                    {
                        TASKDETAIL taskDetail = _db.TASKDETAILs.Where(x => x.IDTask == model.TaskDetail.TaskID).FirstOrDefault();
                        {
                            taskDetail.STATUS = "Created";
                            taskDetail.Level = 1;
                        };
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        _db.SaveChanges();
                        currentTaskDetailID = taskDetail.IDTask;
                    }
                    if (model.TaskDetail.STATUS == "Hold")
                    {
                        TASKDETAIL taskDetail = _db.TASKDETAILs.Where(x => x.IDTask == model.TaskDetail.TaskID).FirstOrDefault();
                        {
                           // taskDetail.ActualStartDate = model.TaskDetail.ActualStartDate;
                          //  taskDetail.ActualEndDate = model.TaskDetail.ActualEndDate;
                            taskDetail.STATUS = model.TaskDetail.STATUS;
                            taskDetail.Level = 1;
                        };
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        _db.SaveChanges();
                        currentTaskDetailID = taskDetail.IDTask;
                    }
                    if (model.TaskDetail.STATUS == "Cancel")
                    {
                        TASKDETAIL taskDetail = _db.TASKDETAILs.Where(x => x.IDTask == model.TaskDetail.TaskID).FirstOrDefault();
                        {
                          //  taskDetail.ActualStartDate = model.TaskDetail.ActualStartDate;
                          //  taskDetail.ActualEndDate = model.TaskDetail.ActualEndDate;
                            taskDetail.STATUS = model.TaskDetail.STATUS;
                            taskDetail.Level = 1;
                        };
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        _db.SaveChanges();
                        currentTaskDetailID = taskDetail.IDTask;
                    }
                    if (model.TaskDetail.STATUS == "Closed")
                    {
                        TASKDETAIL taskDetail = _db.TASKDETAILs.Where(x => x.IDTask == model.TaskDetail.TaskID).FirstOrDefault();
                        {
                          //  taskDetail.ActualStartDate = model.TaskDetail.ActualStartDate;
                         //   taskDetail.ActualEndDate = model.TaskDetail.ActualEndDate;
                            taskDetail.STATUS = model.TaskDetail.STATUS;
                            taskDetail.Level = 1;
                        };
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        _db.SaveChanges();
                        currentTaskDetailID = taskDetail.IDTask;
                    }
                }
                TaskHi taskHis = new TaskHi
                {
                    TaskIDHIs = currentTaskDetailID,
                    DateCreate = DateTime.Now,
                    Status = model.TaskDetail.STATUS,
                    UserCreate = iduser,
                    Comment = "",
                    Level = 1
                };
                _db.TaskHis.Add(taskHis);
                _db.SaveChanges();
                if (model.TaskDocuments != null)
                {
                    foreach (var doc in model.TaskDocuments)
                    {
                        var docDetail = _db.TASKDOCUMENTs.Where(m => m.ID == doc.ID).FirstOrDefault();
                        if (docDetail != null)
                        {
                            docDetail.TASKID_DETAIL = currentTaskDetailID;

                            _db.Entry(docDetail).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                }

                if (model.LastComment != null)
                {
                   ////// foreach (var comm in model.TaskComments)
                  ////  {
                        // var commDetail = _db.TASKCOMMENTs.Where(m => m.ID == comm.ID).FirstOrDefault();
                        TASKCOMMENT taskcmt = new TASKCOMMENT();
                        taskcmt.WRITEDATE = DateTime.Now;
                        taskcmt.WRITTENBY = iduser;
                        taskcmt.CONTENTCOMMENT = model.LastComment;
                        taskcmt.TASKID_DETAIL = currentTaskDetailID;
                    taskcmt.TASK_STATUS = model.TaskDetail.STATUS;
                        _db.TASKCOMMENTs.Add(taskcmt);
                            _db.SaveChanges();
                        //}
                  //  }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Save File TaskMan
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <param name="fileUploadModel"></param>
        /// <returns></returns>
        public TASKDOCUMENT SaveFileTaskMan(FileUploadTaskManViewModel fileUploadModel)
        {
            string filePath = ConfigurationManager.AppSettings["UploadFile"];
            Guid guiId = Guid.NewGuid();
            DateTime now = DateTime.Now;
            string currentDate = now.Year + "-" + now.Month + "-" + now.Day + "-" + now.Hour + "-" + now.Minute + "-" + now.Second + "-" + now.Millisecond;
            try
            {
                TASKDOCUMENT taskDoc = new TASKDOCUMENT();
                taskDoc.ID = fileUploadModel.TaskDocument.ID;
                taskDoc.FILENAME = fileUploadModel.TaskDocument.FILENAME;
                taskDoc.DESCRIPTION = fileUploadModel.TaskDocument.DESCRIPTION;
                taskDoc.REV = fileUploadModel.TaskDocument.REV;
                taskDoc.REVCOMMENT = fileUploadModel.TaskDocument.REVCOMMENT;
                if (fileUploadModel.File_Upload != null)
                {
                    taskDoc.SIZE = fileUploadModel.File_Upload.ContentLength;
                    taskDoc.FILEPATH = currentDate + "_" + Path.GetFileName(fileUploadModel.File_Upload.FileName);
                }
                taskDoc.WRITEDATE = DateTime.Now;
                taskDoc.DATEMODIFY = DateTime.Now;
                taskDoc.WRITTENBY = fileUploadModel.TaskDocument.WRITTENBY;
                int id = CreateTaskDocFile(taskDoc);

                if (fileUploadModel.File_Upload != null)
                {
                    string FolderPath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                    string _FileName = Path.GetFileName(fileUploadModel.File_Upload.FileName);
                    string _path = Path.Combine(FolderPath, currentDate + "_" + _FileName);
                    fileUploadModel.File_Upload.SaveAs(_path);
                }

                return _db.TASKDOCUMENTs.Where(m => m.ID == id).FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get TaskDocFile With FileID
        /// By: Sil
        /// Date: 2018/06/08
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public TASKDOCUMENT GetTaskDocFileWithFileID(int fileId)
        {
            return _db.TASKDOCUMENTs.Where(m => m.ID == fileId).FirstOrDefault();
        }

        /// <summary>
        /// Delete TaskDocFile With FileID
        /// By: Sil
        /// Date: 2018/06/08
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public bool DeleteTaskDocFileWithFileID(int fileId)
        {
            try
            {
                var taskDoc = _db.TASKDOCUMENTs.Where(m => m.ID == fileId).FirstOrDefault();
                _db.TASKDOCUMENTs.Attach(taskDoc);
                _db.TASKDOCUMENTs.Remove(taskDoc);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Save Comment TaskMan
        /// By: Sil
        /// Date: 2018/06/13
        /// </summary>
        /// <param name="commentUploadModel"></param>
        /// <returns></returns>
        public TASKCOMMENT SaveCommentTaskMan(TASKCOMMENT commentUploadModel)
        {
            try
            {
                TASKCOMMENT taskComm = new TASKCOMMENT();
                taskComm.ID = commentUploadModel.ID;
                taskComm.CONTENTCOMMENT = commentUploadModel.CONTENTCOMMENT;
                taskComm.WRITTENBY = commentUploadModel.WRITTENBY;
                taskComm.WRITEDATE = DateTime.Now;

                int id = CreateTaskComment(taskComm);
                return _db.TASKCOMMENTs.Where(m => m.ID == id).FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get TaskManNCR By TaskID
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public TaskManagementNCRViewModel GetTaskManNCRByTaskID(int taskID)
        {
            try
            {
                DateTime DateNow = DateTime.Today;
              //  string datenow = Convert.ToDateTime(DateNow).ToString("yyyy/MM/dd");
                //  DateTime dt = DateTime.ParseExact(datenow, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                TaskManagementNCRViewModel taskNCREdit = new TaskManagementNCRViewModel();
                TASKDETAIL taskDetail = _db.TASKDETAILs.Where(m => m.IDTask == taskID).FirstOrDefault();
                //string datenow1 = Convert.ToDateTime(taskDetail.EstimateStartDate).ToString("yyyy/MM/dd");

              //  if (String.Compare(datenow, datenow1, true)<=0)
                //{
                //    Console.Write("date now <datenow1");
                //}
                
                //  string Estimate = Convert.ToDateTime(taskDetail.EstimateEndDate).ToString("yyyy/MM/dd");

                if (taskDetail.STATUS.Trim() == TaskManagementStatus.New)
                {
                    taskDetail.STATUS = "Un-Assigned";
                }
                if (taskDetail.STATUS.Trim() == "Un-Assigned" && DateNow <= taskDetail.EstimateEndDate && taskDetail.EstimateEndDate != null)
                {
                    taskDetail.STATUS = "In-Process";
                }
                //if (taskDetail.STATUS.Trim() == TaskManagementStatus.Complete && taskDetail.ActualEndDate <= taskDetail.EstimateEndDate)
                //{
                //    taskDetail.STATUS = "Done";

                //}
                //if (taskDetail.STATUS.Trim() == TaskManagementStatus.Complete && taskDetail.ActualEndDate >= taskDetail.EstimateEndDate)
                //{
                //    taskDetail.STATUS = "Closed";

                //}
                if (taskDetail.STATUS.Trim() == "Un-Assigned" && DateNow > taskDetail.EstimateEndDate && taskDetail.ActualEndDate == null && taskDetail.EstimateEndDate != null)
                {
                    taskDetail.STATUS = "Late";

                }
                //if (taskDetail.STATUS.Trim() == TaskManagementStatus.Cancel &&  taskDetail.EstimateEndDate != null)
                //{
                //    taskDetail.STATUS = "Cancel";

                //}
                taskNCREdit.TaskDetail = new TaskDetailViewModel
                {
                    TaskID = taskDetail.IDTask,
                    TopicID = taskDetail.TopicID,
                    TASKNAME = taskDetail.TASKNAME,
                    DESCRIPTION = taskDetail.DESCRIPTION,
                    OWNER = taskDetail.OWNER,
                    ASSIGNEE = taskDetail.ASSIGNEE,
                    APPROVE = taskDetail.APPROVE,
                    EstimateStartDate = taskDetail.EstimateStartDate,
                    EstimateEndDate = taskDetail.EstimateEndDate,
                    ActualStartDate = taskDetail.ActualStartDate,
                    ActualEndDate = taskDetail.ActualEndDate,
                    PROCESS = taskDetail.PROCESS,
                    CreateDate = taskDetail.CreatedDate,
                    STATUS = taskDetail.STATUS,
                    PRIORITY = taskDetail.PRIORITY
                };
                if (taskNCREdit.TaskDetail.APPROVE != null)
                {
                    taskNCREdit.ListApprove = new List<string>();
                    string[] lsApp = taskNCREdit.TaskDetail.APPROVE.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                    foreach (var app in lsApp)
                    {
                        taskNCREdit.ListApprove.Add(app);
                        taskNCREdit.OpproverName = taskNCREdit.OpproverName + GetUserNameByID(app) + "; ";
                    }
                    taskNCREdit.OpproverName = taskNCREdit.OpproverName.Substring(0, taskNCREdit.OpproverName.Length - 2);
                }

                taskNCREdit.TaskList = _db.TASKLISTs.Where(m => m.TopicID == taskNCREdit.TaskDetail.TopicID).FirstOrDefault();

                List<TASKDOCUMENT> lsDoc = new List<TASKDOCUMENT>();
                lsDoc = _db.TASKDOCUMENTs.Where(m => m.TASKID_DETAIL == taskID).ToList();
                taskNCREdit.TaskDocuments = lsDoc;

                List<TASKCOMMENT> lsComm = new List<TASKCOMMENT>();
                lsComm = _db.TASKCOMMENTs.Where(m => m.TASKID_DETAIL == taskID).ToList();
                taskNCREdit.TaskComments = lsComm;

                taskNCREdit.OwnerName = GetUserNameByID(taskNCREdit.TaskDetail.OWNER);

                return taskNCREdit;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get TaskList By TaskID
        /// By: Sil
        /// Date: 2018/06/19
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public TASKLIST GetTaskListByTaskID(int taskID)
        {
            try
            {
                return _db.TASKLISTs.Find(taskID);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetTypeCategory(string Num)
        {
            var TypeCate = _db.TASKLISTs.Where(x => x.Reference == Num).Select(x => x.TYPE).FirstOrDefault();
            //var result = _db.TASK_CATEGORY.Where(x => x.CATEGORY == TypeCate).Select(x => x.URL_VIEW).FirstOrDefault();
            return TypeCate;
        }
        public string GetUrlCategory(string TypeCate)
        {
            //var TypeCate = _db.TASKLISTs.Where(x => x.Topic == Num).Select(x => x.TYPE).FirstOrDefault();
            var result = _db.TASK_CATEGORY.Where(x => x.CATEGORY == TypeCate).Select(x => x.URL_VIEW).FirstOrDefault();
            return result;
        }
        public TaskListViewModel GetTaskList(string Num)
        {
            var Topic = (from list in _db.TASKLISTs
                         join us in _db.AspNetUsers on list.WRITTENBY equals us.Id
                         where (list.Reference == Num)
                         select new TaskListViewModel
                         {
                             Topic = list.Topic,
                             WRITTENBY = us.FullName,
                             Task_Detail = list.Task_Detail,
                             Reference = list.Reference,
                             WRITEDATE = list.WRITEDATE,
                             TYPE = list.TYPE
                         }).FirstOrDefault();
            return Topic;
        }
        public TaskListViewModel GetTaskListID(int ID)
        {
            var Topic = (from list in _db.TASKLISTs
                         join us in _db.AspNetUsers on list.WRITTENBY equals us.Id
                         where (list.TopicID == ID)
                         select new TaskListViewModel
                         {
                             Topic = list.Topic,
                             WRITTENBY = us.FullName,
                             Task_Detail = list.Task_Detail,
                             Reference = list.Reference,
                             WRITEDATE = list.WRITEDATE,
                             TYPE = list.TYPE
                         }).FirstOrDefault();
            return Topic;
        }
        //public  GetTaskType(int taskID)
        //{
        //    try
        //    {
        //        return _db.TASK.Find(taskID);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Get Dropdownlist User
        /// By: Sil
        /// Date: 2018/06/21
        /// </summary>
        /// <returns></returns>
        public List<Children> GetDropdownlistUser()
        {
            var lsUser = _db.AspNetUsers.ToList();
            List<Children> listDropdownlistUser = new List<Children>();
            foreach (var item in lsUser)
            {
                listDropdownlistUser.Add(new Children
                {
                    label = item.FullName,
                    value = item.Id,
                    selected = false,
                });
            }
            return listDropdownlistUser;
        }

        /// <summary>
        /// Get Dropdownlist UserEdit
        /// By: Sil
        /// Date: 2018/06/22
        /// </summary>
        /// <param name="taskDetailID"></param>
        /// <returns></returns>
        public List<Children> GetDropdownlistUserEdit(int taskDetailID)
        {
            var lsUser = _db.AspNetUsers.ToList();
            List<Children> listDropdownlistUserEdit = new List<Children>();
            foreach (var item in lsUser)
            {
                listDropdownlistUserEdit.Add(new Children
                {
                    label = item.FullName,
                    value = item.Id,
                    selected = false,
                });
            }

            TASKDETAIL taskDetail = new TASKDETAIL();
            taskDetail = GetTaskDetailByTaskDetailID(taskDetailID);
            if (taskDetail.APPROVE != null)
            {
                string[] lsApp = taskDetail.APPROVE.Split(new string[] { "<br/>" }, StringSplitOptions.None);

                foreach (var item in listDropdownlistUserEdit)
                {
                    if (lsApp.Contains(item.value))
                    {
                        listDropdownlistUserEdit.Where(m => m.value == item.value).FirstOrDefault().selected = true;
                    }
                }
            }
            return listDropdownlistUserEdit;
        }
        public TASKLIST getcurrentTask()
        {
            var tasklist = _db.TASKLISTs.ToList();
            var result = tasklist.OrderByDescending(x => x.TopicID).FirstOrDefault();
            return result;
        }
        public TASKLIST GetTaskListByTaskNO(string taskNO, string type, string uid)
        {
            try
            {
                //check NCR co task hay ko.
                //var ckTaskList = _db.TASKLISTs.FirstOrDefault(x => x.Reference.Trim().Equals(taskNO.Trim()));
                var ckTaskList = _db.TASKLISTs.FirstOrDefault(x => x.Reference.Trim().Equals(taskNO.Trim()) && x.TYPE == type);
                if (ckTaskList == null && type == "NCR")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "NCR",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });

                    _db.SaveChanges();
                }
                else if (ckTaskList == null && type == "MEETINGNOTE")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "MEETING NOTE",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });

                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "SCAR")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "SCAR",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });

                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "OTHER")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "OTHER",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });

                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "ProductTranfer")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "ProductTranfer",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });

                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "eSuggestion")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "eSuggestion",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });

                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "PCN")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "PCN",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });
                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "Improve_Qualify")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "Improve_Qualify",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });
                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "Improve_Action")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "Improve_Action",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });
                    _db.SaveChanges();

                }
                else if (ckTaskList == null && type == "SoReview")
                {
                    _db.TASKLISTs.Add(new TASKLIST
                    {
                        Topic = taskNO.Trim(),
                        TYPE = "SoReview",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = uid,
                        Level = 1,
                        Reference = taskNO
                    });
                    _db.SaveChanges();

                }
                return ckTaskList;

            }
            catch (Exception)
            {
                return null;
            }
        }
        //public TASKLIST GetTaskListByTaskNO(string taskNO, string type, string uid)
        //{
        //    try
        //    {
        //        //check co task hay ko.
        //        var ckTaskList = _db.TASKLISTs.FirstOrDefault(x => x.Reference.Trim().Equals(taskNO.Trim()) && x.TYPE == type);
        //        if (ckTaskList == null && type == "NCR")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "NCR",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });

        //            _db.SaveChanges();
        //        }
        //        else if (ckTaskList == null && type == "MEETINGNOTE")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "MEETING NOTE",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });

        //            _db.SaveChanges();

        //        }
        //        else if (ckTaskList == null && type == "SCAR")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "SCAR",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });

        //            _db.SaveChanges();

        //        }
        //        else if (ckTaskList == null && type == "OTHER")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "OTHER",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });

        //            _db.SaveChanges();

        //        }
        //        else if (ckTaskList == null && type == "ProductTranfer")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "ProductTranfer",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });
        //        }
        //        else if (ckTaskList == null && type == "PCN")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "PCN",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });
        //            _db.SaveChanges();

        //        }
        //        else if (ckTaskList == null && type == "Improve_Qualify")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "Improve_Qualify",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });
        //            _db.SaveChanges();

        //        }
        //        else if (ckTaskList == null && type == "Improve_Action")
        //        {
        //            _db.TASKLISTs.Add(new TASKLIST
        //            {
        //                Topic = taskNO.Trim(),
        //                TYPE = "Improve_Action",
        //                WRITEDATE = DateTime.Now,
        //                WRITTENBY = uid,
        //                Level = 1,
        //                Reference = taskNO
        //            });
        //            _db.SaveChanges();

        //        }
        //        return ckTaskList;

        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}


        
        public TASKLIST GetTaskListByTaskNONCR(string taskNO)
        {
            try
            {
                //check NCR co task hay ko.
                var ckTaskList = _db.TASKLISTs.FirstOrDefault(x => x.Topic.Trim().Equals(taskNO.Trim()));
               if(ckTaskList!= null)
                {
                    return ckTaskList;

                }
                else{
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<TaskmanagementViewmodel> GetListTask()
        {
            var result = from task in _db.TASKLISTs
                         join taskdetail in _db.TASKDETAILs on task.TopicID equals taskdetail.TopicID
                           into joined3
                         from task1 in joined3.DefaultIfEmpty()
                         join asp in _db.AspNetUsers on task1.OWNER equals asp.Id
                         into joined
                         from j in joined.DefaultIfEmpty()
                         join asp1 in _db.AspNetUsers on task1.ASSIGNEE equals asp1.Id
                          into joined1
                         from j1 in joined1.DefaultIfEmpty()
                         join asp2 in _db.AspNetUsers on task1.APPROVE equals asp2.Id
                        into joined2
                         from j2 in joined2.DefaultIfEmpty()
                       
                         select (new TaskmanagementViewmodel
                         {
                             RefNUMBER = task.Reference,
                             Topic=task.Topic.Replace("PVC - Process name: ", ""),
                             Taskname = task1.TASKNAME,
                             TaskDescription = task1.DESCRIPTION,
                             Type=task.TYPE,
                             Owner = j.FullName,
                             Assignee = j1.FullName,
                             Approve = j2.FullName,
                             StartDay = task1.EstimateStartDate,
                             DueDate = task1.EstimateEndDate,
                             Status = task1.STATUS,
                             Late = task1.ActualEndDate!=null? task1.ActualEndDate.Value>task1.EstimateEndDate.Value?"Late":"":DateTime.Now>task1.EstimateEndDate.Value?"Late":"",
                             ActualStarDay = task1.ActualStartDate,
                             ActualEndDay = task1.ActualEndDate,
                             Priority = task1.PRIORITY,
                             Taskno = task.TopicID,
                             TaskDetailID = task1.IDTask,
                             INSDATEs = task.WRITEDATE
                         });

            return result.ToList();
        }
        public List<TaskmanagementViewmodel> GetListTask(string keyword)
        {
            var result = from task in _db.TASKLISTs
                         join taskdetail in _db.TASKDETAILs on task.TopicID equals taskdetail.TopicID
                           into joined3
                         from task1 in joined3.DefaultIfEmpty()
                         join asp in _db.AspNetUsers on task1.OWNER equals asp.Id
                         into joined
                         from j in joined.DefaultIfEmpty()
                         join asp1 in _db.AspNetUsers on task1.ASSIGNEE equals asp1.Id
                          into joined1
                         from j1 in joined1.DefaultIfEmpty()
                         join asp2 in _db.AspNetUsers on task1.APPROVE equals asp2.Id
                        into joined2
                         from j2 in joined2.DefaultIfEmpty()
                         where (task1.TASKNAME+ " "+task1.DESCRIPTION+" "+ j.FullName+ " " + j1.FullName+ " " + j2.FullName+ " " + task1.STATUS).Contains(keyword)
                         select (new TaskmanagementViewmodel
                         {
                             RefNUMBER = task.Reference,
                             Taskname = task1.TASKNAME,
                             TaskDescription = task1.DESCRIPTION,
                             Owner = j.FullName,
                             Assignee = j1.FullName,
                             Approve = j2.FullName,
                             StartDay = task1.EstimateStartDate,
                             DueDate = task1.EstimateEndDate,
                             Status = task1.STATUS,
                             ActualStarDay = task1.ActualStartDate,
                             ActualEndDay = task1.ActualEndDate,
                             Priority = task1.PRIORITY,
                             Taskno = task.TopicID,
                             TaskDetailID = task1.IDTask,
                             INSDATEs = task.WRITEDATE
                         });

            return result.ToList();
        }
        public List<TaskmanagementViewmodel> GetListTaskNCR()
        {
            var result = from task in _db.TASKLISTs
                         join taskdetail in _db.TASKDETAILs on task.TopicID equals taskdetail.TopicID
                         join asp in _db.AspNetUsers on taskdetail.OWNER equals asp.Id
                         into joined
                         from j in joined.DefaultIfEmpty()
                         join asp1 in _db.AspNetUsers on taskdetail.ASSIGNEE equals asp1.Id
                          into joined1
                         from j1 in joined1.DefaultIfEmpty()
                         join asp2 in _db.AspNetUsers on taskdetail.APPROVE equals asp2.Id
                        into joined2
                         from j2 in joined2.DefaultIfEmpty()
                         where (task.TYPE == "NCR")
                         select (new TaskmanagementViewmodel
                         {
                             RefNUMBER = task.Topic,
                             Taskname = taskdetail.TASKNAME,
                             TaskDescription = taskdetail.DESCRIPTION,
                             Owner = j.FullName,
                             Assignee = j1.FullName,
                             Approve = j2.FullName,
                             StartDay = taskdetail.EstimateStartDate,
                             DueDate = taskdetail.EstimateEndDate,
                             Status = taskdetail.STATUS,
                             ActualStarDay = taskdetail.ActualStartDate,
                             ActualEndDay = taskdetail.ActualEndDate,
                             Priority = taskdetail.PRIORITY,
                             Taskno = task.TopicID,
                             TaskDetailID = taskdetail.IDTask,
                         });

            return result.ToList();
        }

        #endregion

        #region Function
        /// <summary>
        /// Get TaskDetail By TaskID
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        private TASKDETAIL GetTaskDetailByID(int taskID)
        {
            TASKDETAIL taskDetail = new TASKDETAIL();
            taskDetail = _db.TASKDETAILs.Where(m => m.TopicID == taskID).FirstOrDefault();
            return taskDetail;
        }

        /// <summary>
        /// Get TaskList By TaskID
        /// By: Sil
        /// Date: 2018/06/19
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        private TASKLIST GetTaskListByID(int taskID)
        {
            TASKLIST taskList = new TASKLIST();
            taskList = _db.TASKLISTs.Where(m => m.TopicID == taskID).FirstOrDefault();
            return taskList;
        }
        /// <summary>
        /// Get List of Department
        /// By: Thi.Nguyen
        /// Date: 2019/12/14
        /// </summary>
        /// <returns>Department list</returns>
        public List<SelectListItem> GetDeptList()
        {
            List<SelectListItem> listpart = _db.sp_Inv_Get_Dept("").Select(x => new SelectListItem
            {
                Value = x.DepartmentName,
                Text = x.DepartmentName,
            }).ToList();
            return listpart;
        }
        /// <summary>
        /// Get List of User
        /// By: Thi.Nguyen
        /// Date: 2020/01/13
        /// </summary>
        /// <returns>Department list</returns>
        public List<SelectListItem> GetUserList()
        {
            List<SelectListItem> listUser = _db.AspNetUsers.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.FullName,
            }).ToList();
            return listUser;
        }
        /// <summary>
        /// Get List TaskComment By TaskID
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        private IEnumerable<TASKCOMMENT> GetListTaskCommentByID(int taskID)
        {
            List<TASKCOMMENT> lsComment = new List<TASKCOMMENT>();
            foreach (var comment in _db.TASKCOMMENTs.Where(m => m.TASKID_DETAIL == taskID).ToList())
            {
                lsComment.Add(comment);
            }
            return lsComment;
        }

        /// <summary>
        /// Get List TaskDoucument By TaskID
        /// By: Sil
        /// Date: 2018/06/01
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        private IEnumerable<TASKDOCUMENT> GetListTaskDoucumentByID(int taskID)
        {
            List<TASKDOCUMENT> lsDocument = new List<TASKDOCUMENT>();
            foreach (var document in _db.TASKDOCUMENTs.Where(m => m.TASKID_DETAIL == taskID).ToList())
            {
                lsDocument.Add(document);
            }
            return lsDocument;
        }

        /// <summary>
        /// Get UserName By ID
        /// By: Sil
        /// Date: 2018/06/07
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private string GetUserNameByID(string userID)
        {
            if (userID == null || userID == "")
            {
                return "";
            }
            else
            {
                var u = _db.AspNetUsers.Where(m => m.Id == userID).FirstOrDefault();
                return u != null ? u.FullName : "";
            }
        }

        /// <summary>
        /// Create Task Document File
        /// By: Sil
        /// Date: 2018/06/07
        /// </summary>
        /// <param name="taskDoc"></param>
        /// <returns></returns>
        public int CreateTaskDocFile(TASKDOCUMENT taskDoc)
        {
            try
            {
                TASKDOCUMENT taskDocCheck = _db.TASKDOCUMENTs.Where(m => m.ID == taskDoc.ID).FirstOrDefault();
                if (taskDocCheck == null)
                {
                    _db.TASKDOCUMENTs.Add(taskDoc);
                    _db.SaveChanges();
                    return taskDoc.ID;
                }
                else
                {
                    if (taskDoc.SIZE != null)
                    {
                        taskDocCheck.SIZE = taskDoc.SIZE;
                    }
                    if (taskDoc.FILEPATH != null)
                    {
                        taskDocCheck.FILEPATH = taskDoc.FILEPATH;
                    }
                    taskDocCheck.FILENAME = taskDoc.FILENAME;
                    taskDocCheck.DESCRIPTION = taskDoc.DESCRIPTION;
                    taskDocCheck.REV = taskDoc.REV;
                    taskDocCheck.REVCOMMENT = taskDoc.REVCOMMENT;
                    taskDocCheck.DATEMODIFY = DateTime.Now;

                    _db.Entry(taskDocCheck).State = EntityState.Modified;
                    _db.SaveChanges();

                    return taskDoc.ID;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Create TaskComment
        /// By: Sil
        /// Date: 2018/06/13
        /// </summary>
        /// <param name="taskComm"></param>
        /// <returns></returns>
        public int CreateTaskComment(TASKCOMMENT taskComm)
        {
            try
            {
                _db.TASKCOMMENTs.Add(taskComm);
                _db.SaveChanges();
                return taskComm.ID;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        /// <summary>
        /// Update Status TaskMan
        /// By: Sil
        /// Date: 2018/06/20
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateStatusTaskManHis(List<int> id, string status,string iduser,string comment)
        {
            try
            {
                List<int> keys = new List<int>(id);
                foreach (int key in keys)
                {
                    TASKDETAIL taskDetail = _db.TASKDETAILs.Where(m => m.IDTask == key).FirstOrDefault();
                  
                    if(status == TaskManagementStatus.Cancel)
                    {
                        taskDetail.STATUS = status;
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        TaskHi taskHis = new TaskHi
                        {
                            TaskIDHIs = key,
                            DateCreate = DateTime.Now,
                            Status = status,
                            UserCreate = iduser,
                            Comment = comment,
                            Level = 1
                        };
                        _db.TaskHis.Add(taskHis);
                        _db.SaveChanges();
                    }
                    if (status == TaskManagementStatus.Approve)
                    {
                        taskDetail.STATUS = "Closed";
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        TaskHi taskHis = new TaskHi
                        {
                            TaskIDHIs = key,
                            DateCreate = DateTime.Now,
                            Status = status,
                            UserCreate = iduser,
                            Comment = comment,
                            Level = 1
                        };
                        _db.TaskHis.Add(taskHis);
                        _db.SaveChanges();
                    }
                    if (status == TaskManagementStatus.Hold)
                    {
                        taskDetail.STATUS = status;
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        TaskHi taskHis = new TaskHi
                        {
                            TaskIDHIs = key,
                            DateCreate = DateTime.Now,
                            Status = status,
                            UserCreate = iduser,
                            Comment = comment,
                            Level = 1
                        };
                        _db.TaskHis.Add(taskHis);
                        _db.SaveChanges();
                    }
                    if (status == TaskManagementStatus.Reopen)
                    {
                        taskDetail.STATUS = "Created";
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        TaskHi taskHis = new TaskHi
                        {
                            TaskIDHIs = key,
                            DateCreate = DateTime.Now,
                            Status = status,
                            UserCreate = iduser,
                            Comment = comment,
                            Level = 1
                        };
                        _db.TaskHis.Add(taskHis);
                        _db.SaveChanges();
                    }
                    if (status == TaskManagementStatus.Reject)
                    {
                        taskDetail.STATUS = status;
                        _db.Entry(taskDetail).State = EntityState.Modified;
                        TaskHi taskHis = new TaskHi
                        {
                            TaskIDHIs = key,
                            DateCreate = DateTime.Now,
                            Status = status,
                            UserCreate = iduser,
                            Comment = comment,
                            Level = 1
                        };
                        _db.TaskHis.Add(taskHis);
                        _db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool UpdateStatusTaskMan(List<int> id, string status)
        {
            try
            {
                List<int> keys = new List<int>(id);
                foreach (int key in keys)
                {
                    TASKDETAIL taskDetail = _db.TASKDETAILs.Where(m => m.IDTask == key).FirstOrDefault();
                    taskDetail.STATUS = status;
                    _db.Entry(taskDetail).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete TaskManagement
        /// By: Sil
        /// Date: 2018/06/20
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTaskMan(List<int> id,string iduser)
        {
            try
            {
                List<int> keys = new List<int>(id);
                foreach (int key in keys)
                {
                    TASKDETAIL taskDetail = _db.TASKDETAILs.Where(m => m.IDTask == key).FirstOrDefault();

                    _db.TASKDETAILs.Attach(taskDetail);
                    _db.TASKDETAILs.Remove(taskDetail);
                    TaskHi taskHis = new TaskHi
                    {
                        TaskIDHIs = key,
                        DateCreate = DateTime.Now,
                        Status = "Delete",
                        UserCreate = iduser,
                        Comment = "",
                        Level = 1
                    };
                    _db.TaskHis.Add(taskHis);
                    _db.SaveChanges();

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
      
        /// <summary>
        /// Get TaskDetail By TaskDetailID
        /// By: Sil
        /// Date: 2018/06/22
        /// </summary>
        /// <param name="taskDetailID"></param>
        /// <returns></returns>
        public TASKDETAIL GetTaskDetailByTaskDetailID(int taskDetailID)
        {
            try
            {
                return _db.TASKDETAILs.Where(m => m.IDTask == taskDetailID).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ExistsTask(string NCR_NUM)
        {
            //int getidtask = dbContext.TASKLISTs.Where(n => n.TASKNO == NCR_NUM).Select(n => n.ID).FirstOrDefault();
            //var count = dbContext.TASKDETAILs.Where(x => x.TASKID == getidtask).ToList();
            //var countcomplete = dbContext.TASKDETAILs.Where(x => x.TASKID == getidtask && x.STATUS == StatusInDB.Complete).ToList();
            //if (count.Count() == countcomplete.Count())
            //{

            //    return true;
            //}
            //return false;

            var task = _db.TASKLISTs.Join(_db.TASKDETAILs, t => t.TopicID, d => d.TopicID, (t, d) => new { t, d })
                                          .Count(x => x.t.Topic.Equals(NCR_NUM.Trim()) & x.d.STATUS != StatusInDB.Complete);
            return task > 0;
        }

        public Result AddTaskNCRDispositionApproval(string nCRNUM, string Partial)
        {
            var _log = new LogWriter("AddTaskNCRDispositionApproval" + nCRNUM);

            if (string.IsNullOrEmpty(nCRNUM)) return new Result { success = false, message = $"{nCRNUM} is not null" };
            var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim()));
            if (string.IsNullOrEmpty(nCRNUM)) return new Result { success = false, message = $"{nCRNUM} is not exist" };

            //var INSP = _db.AspNetUsers.FirstOrDefault(x=>x.Id.Equals(NCR.INSPECTOR));
            //var OPE = _db.AspNetUsers.FirstOrDefault(x=>x.Id.Equals(INSP.OPE));

            var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(nCRNUM.Trim())).ToList();

            using (var tranj = _db.Database.BeginTransaction())
            {
                var TaskList = _db.TASKLISTs.FirstOrDefault(x => x.Topic.Trim() == nCRNUM.Trim());

                if (NCR.STATUS.Trim() == StatusInDB.DispositionApproved || Partial == "ApprovePartial")
                {
                    if (TaskList == null)
                    {
                        TaskList = new TASKLIST
                        {
                            Topic = NCR.NCR_NUM,
                            TYPE = "NCR",
                            WRITEDATE = DateTime.Now,
                            WRITTENBY = NCR.UserConfirm,
                            Reference = NCR.NCR_NUM,
                            Level= 1
                        };

                        _db.TASKLISTs.Add(TaskList);
                        _db.SaveChanges();
                    }


                    var currentTaskListID = TaskList.TopicID;
                    string GroupMRBWHID = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.WHMRB)).Id;
                    string GroupSQEID = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.SQE)).Id;

                    ///// 2
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = "GLOVIA transaction",
                        OWNER = NCR.UserConfirm,
                        ASSIGNEE = NCR.UserConfirm,
                        STATUS = "Created",
                        CreatedDate= DateTime.Now,
                        APPROVE = NCR.UserConfirm,
                        Level =1,
                    });

                    var Check = DETs.FirstOrDefault(x => x.RESPONSE.Trim() == CONFIRMITY_RESPON.ID_VENDOR);
                    //var SQEAssignees = _db.ApplicationUserGroups.Where(x => x.ApplicationGroupId.Equals(GroupSQEID)).ToList();
                    var SQEAssignees = _db.APPROVALs.Where(x => x.RoleId.Equals(GroupSQEID) & x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();

                    if (NCR.REQUIRED.Value == true)
                    {

                        if (Check != null)
                        {

                            foreach (var Assignee in SQEAssignees)
                            {
                                _db.TASKDETAILs.Add(new TASKDETAIL
                                {
                                    TopicID = currentTaskListID,
                                    TASKNAME = "Create SCAR",
                                    OWNER = NCR.UserConfirm,
                                    ASSIGNEE = Assignee.UserId,
                                    STATUS = "Created",
                                   APPROVE= NCR.UserConfirm,
                                    Level =1,
                                    CreatedDate = DateTime.Now
                                });
                            }
                        }
                        else
                        {
                            _db.TASKDETAILs.Add(new TASKDETAIL
                            {
                                TopicID = currentTaskListID,
                                TASKNAME = "Create internal CAR",
                                OWNER = NCR.UserConfirm,
                                ASSIGNEE = NCR.UserConfirm,
                                APPROVE = NCR.UserConfirm,
                                STATUS = "Created",
                                Level =1
                            });
                        }
                    }

                    if (NCR.NOTIFICATION_ONLY.Value == true)
                    {
                        if (Check == null)
                        {
                            _db.TASKDETAILs.Add(new TASKDETAIL
                            {
                                TopicID = currentTaskListID,
                                TASKNAME = "Notify NCR to internal",
                                OWNER = NCR.UserConfirm,
                                ASSIGNEE = NCR.UserConfirm,
                                APPROVE = NCR.UserConfirm,
                                STATUS = "Created",
                                CreatedDate = DateTime.Now,
                                Level =1
                            });
                        }
                        else
                        {
                            foreach (var Assignee in SQEAssignees)
                            {
                                _db.TASKDETAILs.Add(new TASKDETAIL
                                {
                                    TopicID = currentTaskListID,
                                    TASKNAME = "Notify NCR to Supplier",
                                    OWNER = NCR.UserConfirm,
                                    ASSIGNEE = Assignee.UserId,
                                    APPROVE = NCR.UserConfirm,
                                    STATUS = "Created",
                                    CreatedDate = DateTime.Now,
                                    Level =1
                                });
                            }
                        }
                    }
                    _db.SaveChanges();
                    tranj.Commit();

                    return new Result
                    {
                        success = true,
                        message = "Add task",
                        obj = StatusInDB.DispositionApproved
                    };
                }
                else return new Result
                {
                    success = true,
                    message = "",
                    obj = ""
                };
            }
        }
        #endregion
       public bool CheckOwnerTask(int Idtask,string iduser)
        {
            var Task = _db.TASKDETAILs.Where(x => x.IDTask == Idtask).FirstOrDefault();
            if(Task.OWNER == iduser)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckOwnerCreateFileTask(int Idtask, string iduser)
        {
            var Task = _db.TASKDOCUMENTs.Where(x => x.TASKID_DETAIL== Idtask).FirstOrDefault();
            if (Task!= null && Task.WRITTENBY == iduser)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CHeckApproveTask(int IDTask,string iduser){
            bool Check = false;
            var Detail = _db.TASKDETAILs.Where(x => x.IDTask == IDTask).FirstOrDefault();
            if(Detail != null && Detail.APPROVE != null)
            {
           string [] lsApp= Detail.APPROVE.Split(new string[] { "<br/>" }, StringSplitOptions.None);
            foreach (var app in lsApp)
            {
                if (app == iduser)
                {
                  return Check= true;
                }
                else
                {
                    return Check= false;
                }
            }
            return Check;
            }
            return Check;
        }
        public bool CHeckAssigneeTask(int IDTask, string iduser)
        {
            var Detail = _db.TASKDETAILs.Where(x => x.IDTask == IDTask).FirstOrDefault();
                if  (Detail.ASSIGNEE == iduser)
                {
                    return  true;
                }
                else
                {
                    return  false;
                }
        }
        public string GetcurrentTaskListCreate()
        {
            var result = _db.TASKLISTs.ToList();
            var num = result.OrderByDescending(x => x.WRITEDATE).ToList();
            var numend = num.FirstOrDefault().Reference;
            return numend;
        }
        public bool SaveTaskList(string type,string topic,string topicdetail,string userid,string reference,string phase,string reference2)
        {
            try { 
            TASKLIST tasklist = new TASKLIST();
                if(reference != null &&  !string.IsNullOrEmpty(phase))
                {
                    tasklist.Topic = '['+phase+']' +'-'+ topic;
                    tasklist.Reference = reference;
                }
                else if(!string.IsNullOrEmpty(reference) && string.IsNullOrEmpty(phase))
                {
                    tasklist.Reference = reference;
                    tasklist.Topic =  topic;
                }
                else if(!string.IsNullOrEmpty(reference2)  && string.IsNullOrEmpty(phase))
                {
                    tasklist.Reference = reference2;
                    tasklist.Topic = topic;
                }
                else if (!string.IsNullOrEmpty(reference2) && !string.IsNullOrEmpty(phase) )
                {
                    tasklist.Reference = reference2;
                    tasklist.Topic = '[' + phase + ']' + '-' + topic;
                }
                tasklist.Task_Detail = topicdetail;
            tasklist.Level = 1;
            tasklist.TYPE = type;
            tasklist.WRITEDATE = DateTime.Now;
            tasklist.WRITTENBY = userid;
                //tasklist.TYPE = type;
            _db.TASKLISTs.Add(tasklist);
                _db.SaveChanges();
             //   var idcurrentTaskList = _db.TASKLISTs.ToList();
            //    idcurrentTaskList.OrderByDescending(x=>x.TopicID).Select(x=>x.TopicID).FirstOrDefault();
            //    int idcurrent = idcurrentTaskList.OrderByDescending(x => x.TopicID).Select(x => x.TopicID).FirstOrDefault();
            //    TASKDETAIL taskdetail = new TASKDETAIL();
            //    taskdetail.TASKNAME = topic;
            //    taskdetail.DESCRIPTION = topicdetail;
            //    taskdetail.CreatedDate = DateTime.Now;
            //    taskdetail.ASSIGNEE = userid;
            //    taskdetail.Level = 1;
            //    taskdetail.TopicID = idcurrent;
            //    _db.TASKDETAILs.Add(taskdetail);
            //_db.SaveChanges();
            return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
               
        public List<sp_TaskList_Report_Result> get_TaskList_Report(string Dept,string TaskStatus,DateTime DateFrom, DateTime DateTo)
        {
            var taskRpt = _db.sp_TaskList_Report(Dept, TaskStatus, DateFrom, DateTo).ToList();
            return taskRpt;
        }
        public List<sp_TaskStatistical_Report_Result> get_TaskStatistical_Report(string Dept, string TaskStatus, DateTime DateFrom, DateTime DateTo)
        {
            var taskRpt = _db.sp_TaskStatistical_Report(Dept, TaskStatus, DateFrom, DateTo).ToList();
            return taskRpt;
        }
        public List<sp_Task_Statistical_ByDept_Result> Get_Task_Statistical_ByDept(string dept)
        {
            var taskRpt = _db.sp_Task_Statistical_ByDept(dept).ToList();
            return taskRpt;
        }
        public List<sp_Task_Statistical_ByDetail_Result>  Get_Task_Statistical_ByDetail(string SearchType, string SearchValue, string TaskStatus)
        {
            var taskRpt = _db.sp_Task_Statistical_ByDetail(SearchType, SearchValue, TaskStatus).ToList();
            return taskRpt;
        }
        public List<sp_Task_Statistical_ByUser_Result> Get_Task_Statistical_ByUser(string user)
        {
            var taskRpt = _db.sp_Task_Statistical_ByUser(user).ToList();
            return taskRpt;
        }
        public List<sp_Task_Search_Result> Get_Task_BykeyWord(string key)
        {
            var taskRpt = _db.sp_Task_Search(key).ToList();
            return taskRpt;
        }
        public Result AddTaskProductTranfer_BuyPart(string PartNum, string iduser)
        {
            if (string.IsNullOrEmpty(PartNum)) return new Result { success = false, message = $"{PartNum} is not null" };
            var usertask = _db.tbl_PT_Dept_PIC_BuyPart.ToList();
            //var usertask = _db.tbl_PT_Dept_PIC.ToList();
            using (var tranj = _db.Database.BeginTransaction())
            {
                var TaskList = _db.TASKLISTs.FirstOrDefault(x => x.Reference.Trim() == PartNum.Trim());
                if (TaskList == null)
                {
                    TaskList = new TASKLIST
                    {
                        Topic = PartNum,
                        TYPE = "ProductTranfer",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = iduser,
                        Reference = PartNum,
                        Level = 1
                    };

                    _db.TASKLISTs.Add(TaskList);
                    _db.SaveChanges();
                }
                var currentTaskListID = TaskList.TopicID;
                // QA
                foreach (var item in usertask)
                {
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = item.Task_Default,
                        OWNER = item.PIC,
                        ASSIGNEE = item.PIC,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        APPROVE = item.PIC,
                        Level = 1,
                    });
                }
                _db.SaveChanges();
                tranj.Commit();
                return new Result
                {
                    success = true,
                    message = "Add task",
                };
            }
        }
        public Result AddTaskProductTranfer(string PartNum,string iduser,string Onwer)
        {
            var _log = new LogWriter("AddTaskProductTranfer" + PartNum);

            if (string.IsNullOrEmpty(PartNum)) return new Result { success = false, message = $"{PartNum} is not null" };
            var QA = _db.tbl_PT_Dept_PIC.Where(x => x.Dept_ID == "QA").ToList();
            var ADC = _db.tbl_PT_Dept_PIC.Where(x => x.Dept_ID == "ADC").ToList();
            var PIC = _db.tbl_PT_Dept_PIC.Where(x => x.Dept_ID == "PIC").ToList();
            var FA = _db.tbl_PT_Dept_PIC.Where(x => x.Dept_ID == "FA").ToList();
            var Buyer = _db.tbl_PT_Dept_PIC.Where(x => x.Dept_ID == "Buyer").ToList();
            using (var tranj = _db.Database.BeginTransaction())
            {
                var TaskList = _db.TASKLISTs.FirstOrDefault(x => x.Reference.Trim() == PartNum.Trim());
                if (TaskList == null)
                {
                    TaskList = new TASKLIST
                    {
                        Topic = PartNum,
                        TYPE = "ProductTranfer",
                        WRITEDATE = DateTime.Now,
                        WRITTENBY = iduser,
                        Reference = PartNum,
                        Level = 1
                    };

                    _db.TASKLISTs.Add(TaskList);
                    _db.SaveChanges();
                }
                else
                {
                    var taskdetail = _db.TASKDETAILs.FirstOrDefault(x => x.TopicID == TaskList.TopicID);
                    if(taskdetail!=null)
                    return new Result
                    {
                        success = true,
                        message = "Auto Task has created",
                    };
                }
                var currentTaskListID = TaskList.TopicID;
                // QA
                foreach (var item in QA)
                {
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = item.Task_Default,
                        OWNER = item.PIC,
                        ASSIGNEE = item.PIC,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        APPROVE = Onwer,
                        Level = 1,
                    });
                }
                foreach (var item in ADC)
                {
                    //ADC
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = item.Task_Default,
                        OWNER = item.PIC,
                        ASSIGNEE = item.PIC,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        APPROVE = Onwer,
                        Level = 1,
                    });
                }
                //PIC
                foreach (var item in PIC)
                {
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = item.Task_Default,
                        OWNER = item.PIC,
                        ASSIGNEE = item.PIC,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        APPROVE = Onwer,
                        Level = 1,
                    });

                }

                //FA
                foreach (var item in FA)
                {
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = item.Task_Default,
                        OWNER = item.PIC,
                        ASSIGNEE = item.PIC,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        APPROVE = Onwer,
                        Level = 1,
                    });
                }
                //Buyer
                foreach (var item in Buyer)
                {
                    _db.TASKDETAILs.Add(new TASKDETAIL
                    {
                        TopicID = currentTaskListID,
                        TASKNAME = item.Task_Default,
                        OWNER = item.PIC,
                        ASSIGNEE = item.PIC,
                        STATUS = "Created",
                        CreatedDate = DateTime.Now,
                        APPROVE = Onwer,
                        Level = 1,
                    });
                }
                _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        message = "Add task",
                    };
                }
        }
    }
}
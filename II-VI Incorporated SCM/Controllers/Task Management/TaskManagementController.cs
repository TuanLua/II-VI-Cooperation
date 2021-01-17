using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using II_VI_Incorporated_SCM.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.Task_Management
{
    public class TaskManagementController : Controller
    {
        private readonly ITaskManagementService _iTaskManagementService;
        private readonly IEmailService _emailService;

        public TaskManagementController(ITaskManagementService iTaskManagementService, IEmailService emailService)
        {
            _iTaskManagementService = iTaskManagementService;
            _emailService = emailService;
        }
        // GET: TaskManagement
        public ActionResult Index()
        {
            ViewBag.TypeTask = _iTaskManagementService.GetdropdownTypeTask();
            return View();
        }
        public ActionResult CheckRequired(string id)
        {
            LogWriter log = new LogWriter("GetpartbyDate - Start write log get type task");
            try
            {
                List<string> part = _iTaskManagementService.GetCheckedbyid(id);
                return Json(new
                {
                    success = true,
                    data = part
                });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
        public ActionResult GetTypeTask(string id)
        {
            LogWriter log = new LogWriter("GetpartbyDate - Start write log get type task");
            try
            {
                List<string> part = _iTaskManagementService.GetTypeTaskbyid(id);
                return Json(new
                {
                    success = true,
                    data = part
                });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
        public ActionResult GetPhaseTask(string id)
        {
            LogWriter log = new LogWriter("GetpartbyDate - Start write log Getsupplierbytype");
            try
            {
                List<string> part = _iTaskManagementService.GetPhaseTaskbyid(id);
                return Json(new
                {
                    success = true,
                    data = part
                });
            }
            catch (Exception ex)
            {
                log.LogWrite(ex.ToString());
                return Json(new { success = false });
            }
        }
        public ActionResult TaskManagementNcr(int TaskID)

        {
            TASKLIST taskList = _iTaskManagementService.GetTaskListByTaskID(TaskID);
            ViewBag.TaskManID = TaskID;
            ViewBag.TaskNo = taskList.Topic;
            return View();
        }

        /// <summary>
        /// Fill Data To GridKendo TaskNCR
        /// By: Sil
        /// Date: 2018/05/28
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult ReadTaksMantNCR([DataSourceRequest] DataSourceRequest request, int taskID)
        {
            return Json(_iTaskManagementService.GetListTaskMantNCRByID(taskID).ToDataSourceResult(request));
        }
        public JsonResult ReadTaksList([DataSourceRequest] DataSourceRequest request)
        {
            List<TaskmanagementViewmodel> result = _iTaskManagementService.getlisttasklist();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult Index([DataSourceRequest] DataSourceRequest request, int? id)
        //{
        //    var dataContext = new IIVILocalDB();

        //    var result = dataContext.TASKDETAILs.ToTreeDataSourceResult(request,
        //        e => e.IDTask,
        //        e => e.TopicID,
        //        e => id.HasValue ? e.TopicID == id : e.TopicID == null,
        //        e => e
        //    );

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult All([DataSourceRequest] DataSourceRequest request, int? id)
        {
            IIVILocalDB dataContext = new IIVILocalDB();
            //var lsttopicid = dataContext.TASKLISTs.ToList().Select(x=>x.TopicID);
            //var result = (from b in dataContext.TASKDETAILs //on e.TopicID equals b.TopicID
            //              select new TaskmanagementViewmodel
            //              {
            //                  TopicID = b.TopicID,
            //                  Taskname = b.TASKNAME + b.IDTask,
            //                  ActualEndDay = b.ActualEndDate,
            //                  ActualStarDay = b.ActualStartDate,
            //                  Type = b.DESCRIPTION,
            //                  TaskDetailID = b.IDTask,
            //                  Level = b.Level,
            //                  hasChildren = b.TopicID.HasValue
            //              }).ToList();
            //foreach (var item in result)
            //{
            //    foreach (var item1 in lsttopicid)
            //    {
            //        if (item.TopicID == item1)
            //        {
            //            item.TopicID = null;
            //        }

            //    }
            //}
            string query = "select a.IDTask,a.Reference,a.Type,a.Taskname,a.Level from( select TopicID as 'IDTask',Reference, level,'Topic' 'Type',Topic 'Taskname' from TASKLIST  where topicid = 8089  union all select IDTask,TopicID,Level,'Task' 'Type',TASKNAME 'Taskname' from TASKDETAIL where TopicID in (select topicId from TASKLIST where topicid = 8089 )) as a ";
            List<TaskmanagementTest> result = dataContext.Database.SqlQuery<TaskmanagementTest>(query).ToList();
            //foreach (var item in result)
            //{
            //    if(item.Reference == null && item.IDTask != 8089 )
            //    {
            //        item.Reference = 1;
            //    }

            //}
            result.ToTreeDataSourceResult(request,
                  e => e.IDTask,
                  e => e.Reference,
                  e => id.HasValue ? e.IDTask == id : e.IDTask == null,
                 e => e
              );

            return Json(result.OrderByDescending(x => x.IDTask), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult get_TaskList_Report([DataSourceRequest] DataSourceRequest request, string Dept, string TaskStatus, DateTime DateFrom, DateTime DateTo)
        {
            List<sp_TaskList_Report_Result> taskrpt = _iTaskManagementService.get_TaskList_Report(Dept, TaskStatus, DateFrom, DateTo);
            return Json(taskrpt.ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult get_TaskStatistical_Report([DataSourceRequest] DataSourceRequest request, string Dept, string TaskStatus, DateTime DateFrom, DateTime DateTo)

        {
            List<sp_TaskStatistical_Report_Result> taskrpt = _iTaskManagementService.get_TaskStatistical_Report(Dept, TaskStatus, DateFrom, DateTo);
            return Json(taskrpt.ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult Get_Task_Statistical_ByUser([DataSourceRequest] DataSourceRequest request, string user)
        {
            List<sp_Task_Statistical_ByUser_Result> taskrpt = _iTaskManagementService.Get_Task_Statistical_ByUser(user);
            return Json(taskrpt.ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult Get_Task_Statistical_ByDept([DataSourceRequest] DataSourceRequest request, string dept)
        {
            List<sp_Task_Statistical_ByDept_Result> taskrpt = _iTaskManagementService.Get_Task_Statistical_ByDept(dept);
            return Json(taskrpt.ToDataSourceResult(request));
        }
        public JsonResult Get_Task_Statistical_ByDetail([DataSourceRequest] DataSourceRequest request, string SearchType,string SearchValue, string TaskStatus)
        {
            List<sp_Task_Statistical_ByDetail_Result> taskrpt = _iTaskManagementService.Get_Task_Statistical_ByDetail(SearchType,SearchValue,TaskStatus);
            return Json(taskrpt.ToDataSourceResult(request));
        }
        public JsonResult Remote_Data_Binding_Get_Employees([DataSourceRequest] DataSourceRequest request, int? id)
        {
            IIVILocalDB dataContext = new IIVILocalDB();
            // lay id mau: id = 591;
            List<TaskmanagementViewmodel> result = //(from e in dataContext.TASKLISTs
                                                   //join
                         (from b in dataContext.TASKDETAILs //on e.TopicID equals b.TopicID
                          select new TaskmanagementViewmodel
                          {
                              TopicID = b.TopicID,
                              Taskname = b.TASKNAME + b.IDTask,
                              ActualEndDay = b.ActualEndDate,
                              ActualStarDay = b.ActualStartDate,
                              Type = b.DESCRIPTION,
                              TaskDetailID = b.IDTask,
                              Level = b.Level
                              //  hasChildren = b.TopicID.ToString().Any()
                          }).ToList();

            return Json(result.ToTreeDataSourceResult(request, e => e.TaskDetailID,//con,
                e => e.TopicID,//cha,
                 e => id.HasValue ? e.TaskDetailID == id : e.TaskDetailID == 591,
                e => e), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateTaskManagementNcr(int taskID)
        {
            TaskManagementNCRViewModel taskNCRCreate = new TaskManagementNCRViewModel();
            TASKLIST taskList = new TASKLIST();
            taskList = _iTaskManagementService.GetTaskListByTaskID(taskID);
            taskNCRCreate.TaskList = taskList;
            taskNCRCreate.TaskDetail = new TaskDetailViewModel
            {
                OWNER = User.Identity.GetUserId()
            };
            taskNCRCreate.OwnerName = User.Identity.GetUserName();
            //taskNCREdit.TaskComments = new List<TASKCOMMENT>();
            //taskNCREdit.TaskDocuments = new List<TASKDOCUMENT>();
            SetViewBag(taskNCRCreate);
            return View(taskNCRCreate);
        }

        public ActionResult CreateTaskManagementNcrByTaskNo(string taskNO, string type)
        {
            try
            {
                TaskManagementNCRViewModel taskNCRCreate = new TaskManagementNCRViewModel();
                TASKLIST taskList = new TASKLIST();
                taskList = _iTaskManagementService.GetTaskListByTaskNO(taskNO, type, User.Identity.GetUserId());
                if (taskList == null /*&& type == "NCR"*/)
                {
                    taskList = _iTaskManagementService.getcurrentTask();
                }
                //if (taskList == null && type == "MEETINGNOTE")
                //{
                //    taskList = _iTaskManagementService.getcurrentTask();
                //}
                taskNCRCreate.TaskList = taskList;
                taskNCRCreate.TaskDetail = new TaskDetailViewModel
                {
                    OWNER = User.Identity.GetUserId()
                };
                taskNCRCreate.OwnerName = User.Identity.GetUserName();
                //taskNCREdit.TaskComments = new List<TASKCOMMENT>();
                //taskNCREdit.TaskDocuments = new List<TASKDOCUMENT>();
                SetViewBag(taskNCRCreate);
                return View("CreateTaskManagementNcr", taskNCRCreate);
            }
            catch (Exception ex)
            {
                LogWriter _log = new LogWriter("CreateTaskManagementNcrByTaskNo");
                _log.LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    DbEntityValidationException e = (DbEntityValidationException)ex;
                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            //    ve.PropertyName, ve.ErrorMessage);
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
                return View("CreateTaskManagementNcr", new TaskManagementNCRViewModel());
            }

        }

        [HttpPost]
        public ActionResult SaveTaskManagementNCR(TaskManagementNCRViewModel taskNCRModel)
        {
            TaskManagementNCRViewModel createTaskNCRModel = new TaskManagementNCRViewModel
            {
                TaskList = taskNCRModel.TaskList
            };
            createTaskNCRModel.TaskList.TYPE = taskNCRModel.TaskList.TYPE;
            createTaskNCRModel.TaskList.WRITEDATE = DateTime.Now;
            createTaskNCRModel.TaskList.WRITTENBY = User.Identity.GetUserId();
            if (taskNCRModel.ListApprove != null)
            {
                string approver = string.Empty;
                foreach (string app in taskNCRModel.ListApprove)
                {
                    approver = approver + app + "<br/>";
                }
                approver = approver.Substring(0, approver.Length - 5);
                taskNCRModel.TaskDetail.APPROVE = approver;
            }
            else
            {
                string approver = taskNCRModel.TaskDetail.APPROVE;
                if (approver != null)
                {
                    approver = approver.Replace(";", "<br/>");
                    taskNCRModel.TaskDetail.APPROVE = approver;
                }

            }
            createTaskNCRModel.TaskDetail = taskNCRModel.TaskDetail;
            //createTaskNCRModel.TaskDetail.OWNER = User.Identity.GetUserId();

            createTaskNCRModel.TaskDocuments = taskNCRModel.TaskDocuments;
            createTaskNCRModel.TaskComments = taskNCRModel.TaskComments;
            createTaskNCRModel.LastComment = taskNCRModel.LastComment;
            string iduser = User.Identity.GetUserId();
            //Email khi completed or approve,reject,
            string path = Server.MapPath(ConfigurationManager.AppSettings["MailTempaltePath"]);
            bool result = _iTaskManagementService.AddTaskManNCR(createTaskNCRModel, iduser);
            List<AspNetUser> Users = _iTaskManagementService.getuser();

            if (result && taskNCRModel.TaskDetail.STATUS == "Completed")
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["Completed"];
                string MailTemplate = EmailCreateConfig.Split('|')[2];
                string linkTask = Url.Action("EditTaskManagementNcr", "TaskManagement", new { taskID = taskNCRModel.TaskDetail.TaskID }, Request.Url.Scheme);
                AspNetUser Useraprove = Users.Where(x => x.Id == taskNCRModel.TaskDetail.APPROVE).FirstOrDefault();
                AspNetUser UserOwner = Users.Where(x => x.Id == taskNCRModel.TaskDetail.OWNER).FirstOrDefault();

                //_emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: Useraprove.FullName, email: Useraprove.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment, user: "");
                _emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: Useraprove.FullName, email: Useraprove.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TASKNAME, comment: taskNCRModel.LastComment, user: "");

            }
            if (result && taskNCRModel.TaskDetail.STATUS == "Created")
            {
                int TaskIDcurrentCreated = _iTaskManagementService.TaskIDCurrent();
                if (taskNCRModel.TaskDetail.TaskID != 0)
                {
                    TaskIDcurrentCreated = taskNCRModel.TaskDetail.TaskID;
                }
                string EmailCreateConfig = ConfigurationManager.AppSettings["Created"];
                string MailTemplate = EmailCreateConfig.Split('|')[2];
                string linkTask = Url.Action("EditTaskManagementNcr", "TaskManagement", new { taskID = TaskIDcurrentCreated }, Request.Url.Scheme);
                AspNetUser Userasignee = Users.Where(x => x.Id == taskNCRModel.TaskDetail.ASSIGNEE).FirstOrDefault();
                AspNetUser UserOwner = Users.Where(x => x.Id == taskNCRModel.TaskDetail.OWNER).FirstOrDefault();
                //_emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: Userasignee.FullName, email: Userasignee.Email, TaskID: TaskIDcurrentCreated.ToString(), linkNCR: linkTask, TaskName: TaskIDcurrentCreated.ToString(), comment: taskNCRModel.LastComment, user: "");
                _emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: Userasignee.FullName, email: Userasignee.Email, TaskID: TaskIDcurrentCreated.ToString(), linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TASKNAME, comment: taskNCRModel.LastComment, user: "");
                // _emailService.SendEmailCompletedTask(mailTemplate: MailTemplate, mailPath: path, RecipientName: UserOwner.FullName, email: UserOwner.Email, TaskID: TaskIDcurrentCreated.ToString(), linkTask: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment, user:"");

            }
            if (result && taskNCRModel.TaskDetail.STATUS == "Closed")//approve
            {
                // int TaskIDcurrentCreated = _iTaskManagementService.TaskIDCurrent();
                string EmailCreateConfig = ConfigurationManager.AppSettings["Approve"];
                string MailTemplate = EmailCreateConfig.Split('|')[2];
                string linkTask = Url.Action("EditTaskManagementNcr", "TaskManagement", new { taskID = taskNCRModel.TaskDetail.TaskID }, Request.Url.Scheme);
                AspNetUser UserAssignee = Users.Where(x => x.Id == taskNCRModel.TaskDetail.ASSIGNEE).FirstOrDefault();
                AspNetUser UserOwner = Users.Where(x => x.Id == taskNCRModel.TaskDetail.OWNER).FirstOrDefault();

                //_emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: UserAssignee.FullName, email: UserAssignee.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment, user: "");
                _emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: UserAssignee.FullName, email: UserAssignee.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TASKNAME, comment: taskNCRModel.LastComment, user: "");
                //  _emailService.SendEmailCompletedTask(mailTemplate: MailTemplate, mailPath: path, RecipientName: UserOwner.FullName, email: UserOwner.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkTask: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment,user:"");
            }
            if (result && taskNCRModel.TaskDetail.STATUS == "Reject")
            {
                //int TaskIDcurrentCreated = _iTaskManagementService.TaskIDCurrent();
                string EmailCreateConfig = ConfigurationManager.AppSettings["Reject"];
                string MailTemplate = EmailCreateConfig.Split('|')[2];
                string linkTask = Url.Action("EditTaskManagementNcr", "TaskManagement", new { taskID = taskNCRModel.TaskDetail.TaskID }, Request.Url.Scheme);
                AspNetUser UserAssignee = Users.Where(x => x.Id == taskNCRModel.TaskDetail.ASSIGNEE).FirstOrDefault();
                AspNetUser UserApprove = Users.Where(x => x.Id == taskNCRModel.TaskDetail.APPROVE).FirstOrDefault();
                AspNetUser UserOwner = Users.Where(x => x.Id == taskNCRModel.TaskDetail.OWNER).FirstOrDefault();
                //_emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: UserAssignee.FullName, email: UserAssignee.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment, user: UserApprove.FullName);
                _emailService.SendEmailCompletedTaskCreated(mailTemplate: MailTemplate, mailPath: path, Owner: UserOwner.Email, RecipientName: UserAssignee.FullName, email: UserAssignee.Email, TaskID: taskNCRModel.TaskDetail.TASKNAME, linkNCR: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment, user: UserApprove.FullName);
                // _emailService.SendEmailCompletedTask(mailTemplate: MailTemplate, mailPath: path, RecipientName: UserOwner.FullName, email: UserOwner.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkTask: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment,user:UserApprove.FullName);
            }
            if (result && taskNCRModel.TaskDetail.STATUS == "Reopen")
            {
                string EmailCreateConfig = ConfigurationManager.AppSettings["Reopen"];
                string MailTemplate = EmailCreateConfig.Split('|')[2];
                string linkTask = Url.Action("EditTaskManagementNcr", "TaskManagement", new { taskID = taskNCRModel.TaskDetail.TaskID }, Request.Url.Scheme);
                AspNetUser Userasignee = Users.Where(x => x.Id == taskNCRModel.TaskDetail.ASSIGNEE).FirstOrDefault();
                //_emailService.SendEmailCompletedTask(mailTemplate: MailTemplate, mailPath: path, RecipientName: Userasignee.FullName, email: Userasignee.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkTask: linkTask, TaskName: taskNCRModel.TaskDetail.TaskID.ToString(), comment: taskNCRModel.LastComment, user: "");
                _emailService.SendEmailCompletedTask(mailTemplate: MailTemplate, mailPath: path, RecipientName: Userasignee.FullName, email: Userasignee.Email, TaskID: taskNCRModel.TaskDetail.TaskID.ToString(), linkTask: linkTask, TaskName: taskNCRModel.TaskDetail.TASKNAME, comment: taskNCRModel.LastComment, user: "");

            }
            if (createTaskNCRModel.TaskList.TYPE == "NCR")
            {
                return RedirectToAction("ViewApproval", "NCRApproval", new { NCR_NUM = taskNCRModel.TaskList.Topic });
            }

            if (createTaskNCRModel.TaskList.TYPE == "MEETING NOTE")
            {
                return RedirectToAction("ViewMeetingNote", "MeetingNote", new { MeetingNum = taskNCRModel.TaskList.Topic });
            }
            //return Json(new { success = true });
            return RedirectToAction("TaskManagementNcr", "TaskManagement", new { TaskID = taskNCRModel.TaskList.TopicID });
        }

        public ActionResult EditTaskManagementNcr(int taskID)
        {
            try
            {
                TaskManagementNCRViewModel taskNCREdit = new TaskManagementNCRViewModel();
                taskNCREdit = _iTaskManagementService.GetTaskManNCRByTaskID(taskID);
                SetViewBag(taskNCREdit);
                ViewBag.ApproveName = taskNCREdit.OpproverName;
                ViewBag.Owner = _iTaskManagementService.CheckOwnerTask(taskID, User.Identity.GetUserId());
                ViewBag.Assignee = _iTaskManagementService.CHeckAssigneeTask(taskID, User.Identity.GetUserId());
                ViewBag.Approve = _iTaskManagementService.CHeckApproveTask(taskID, User.Identity.GetUserId());
                ViewBag.UserCreateFile = _iTaskManagementService.CheckOwnerCreateFileTask(taskID, User.Identity.GetUserId());
                if (taskNCREdit.TaskDetail.APPROVE != null)
                {
                    string approver = taskNCREdit.TaskDetail.APPROVE;
                    approver = approver.Replace("<br/>", ";");
                    taskNCREdit.TaskDetail.APPROVE = approver;
                }
                return View(taskNCREdit);
            }
            catch (Exception ex)
            {
                new LogWriter("EditTaskManagementNcr").LogWrite(ex.ToString());
                return View();
            }
        }

        [HttpPost]
        public JsonResult UploadFileTaskMan(FileUploadTaskManViewModel fileUploadModel)
        {
            try
            {
                fileUploadModel.TaskDocument.WRITTENBY = User.Identity.GetUserId();
                if (fileUploadModel.TaskDocument.DESCRIPTION == null)
                {
                    fileUploadModel.TaskDocument.DESCRIPTION = string.Empty;
                }
                if (fileUploadModel.TaskDocument.REVCOMMENT == null)
                {
                    fileUploadModel.TaskDocument.REVCOMMENT = string.Empty;
                }
                TASKDOCUMENT doc = _iTaskManagementService.SaveFileTaskMan(fileUploadModel);
                return Json(new { success = true, documentTask = doc });
            }
            catch (Exception)
            {
                return Json(new { success = false, documentTask = 0 });
            }
        }

        [HttpPost]
        public JsonResult UploadCommentTaskMan(TASKCOMMENT commentUploadModel/*, string CONTENTCOMMENT*/)
        {
            try
            {
                commentUploadModel.WRITTENBY = User.Identity.GetUserId();
                commentUploadModel.CONTENTCOMMENT = commentUploadModel.CONTENTCOMMENT.Replace("\n", "<br/>");
                TASKCOMMENT comm = _iTaskManagementService.SaveCommentTaskMan(commentUploadModel);
                //string userName = User.Identity.GetUserName();

                return Json(new { success = true, commentTask = comm/*, userName = userName*/ });
            }
            catch (Exception)
            {
                return Json(new { success = false, commentTask = 0, userName = "" });
            }
        }

        public FileContentResult DownloadFile(int fileId)
        {
            string filePath = ConfigurationManager.AppSettings["UploadFile"];
            if (fileId != -1)
            {
                TASKDOCUMENT sf = _iTaskManagementService.GetTaskDocFileWithFileID(fileId);
                if (sf != null)
                {
                    string filePathFull = Server.MapPath(filePath + "/" + sf.FILEPATH);
                    byte[] file = GetMediaFileContent(filePathFull);
                    return File(file, MimeMapping.GetMimeMapping(sf.FILEPATH), sf.FILEPATH);
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        [HttpPost]
        public JsonResult DeleteFile(int fileId)
        {
            try
            {
                bool isDelete = _iTaskManagementService.DeleteTaskDocFileWithFileID(fileId);
                return Json(new { success = isDelete });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult ChangeStatusTaskMan(List<int> id, int status)
        {
            //Approver = 1, Complete = 2, Reject = 3, Reopen = 4, Cancel = 5
            bool result = false;
            if (status == 1)
            {
                result = _iTaskManagementService.UpdateStatusTaskMan(id, TaskManagementStatus.Approve);
            }
            else if (status == 2)
            {
                result = _iTaskManagementService.UpdateStatusTaskMan(id, TaskManagementStatus.Complete);
            }
            else if (status == 3)
            {
                result = _iTaskManagementService.UpdateStatusTaskMan(id, TaskManagementStatus.Reject);
            }
            else if (status == 4)
            {
                result = _iTaskManagementService.UpdateStatusTaskMan(id, TaskManagementStatus.Reopen);
            }
            else if (status == 5)
            {
                result = _iTaskManagementService.UpdateStatusTaskMan(id, TaskManagementStatus.Cancel);
            }

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteTaskMan(List<int> id)
        {
            string iduser = User.Identity.GetUserId();
            bool result = _iTaskManagementService.DeleteTaskMan(id, iduser);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        public JsonResult CancelTaskMan(List<int> id)
        {
            string status = "Cancel";
            string iduser = User.Identity.GetUserId();
            bool result = _iTaskManagementService.UpdateStatusTaskManHis(id, status, iduser, "");

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        public JsonResult HoldTaskMan(List<int> id, string comment)
        {
            string status = "Hold";
            string iduser = User.Identity.GetUserId();
            bool result = _iTaskManagementService.UpdateStatusTaskManHis(id, status, iduser, comment);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        public JsonResult RejectTaskMan(List<int> id, string comment)
        {
            string status = "Reject";
            string iduser = User.Identity.GetUserId();
            bool result = _iTaskManagementService.UpdateStatusTaskManHis(id, status, iduser, comment);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        public JsonResult ReopenTaskMan(List<int> id, string comment)
        {
            string status = "Reopen";
            string iduser = User.Identity.GetUserId();
            bool result = _iTaskManagementService.UpdateStatusTaskManHis(id, status, iduser, comment);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        public JsonResult ApproveTaskMan(List<int> id, string comment)
        {
            string status = "Approve";
            string iduser = User.Identity.GetUserId();
            bool result = _iTaskManagementService.UpdateStatusTaskManHis(id, status, iduser, comment);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        public JsonResult GetDropdownListUser()
        {
            List<Models.NCR.Children> list = _iTaskManagementService.GetDropdownlistUser();
            return Json(list);
        }

        public JsonResult GetDropdownListUserEdit(int taskDetailID)
        {
            //TASKDETAIL taskDetail = new TASKDETAIL();
            //taskDetail = _iTaskManagementService.GetTaskDetailByTaskDetailID(taskDetailID);

            List<Models.NCR.Children> list = _iTaskManagementService.GetDropdownlistUserEdit(taskDetailID);
            return Json(list);
        }

        #region Function
        public static byte[] GetMediaFileContent(string filename)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(
                        filename,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read))
            {
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                return fileData;
            }
        }

        public void SetViewBag(TaskManagementNCRViewModel taskMan)
        {
            ViewBag.CurrentUser = taskMan.OwnerName;
            ViewBag.CurrentUserCreate = User.Identity.GetUserName();
            ViewBag.CurrentUserID = User.Identity.GetUserId();
            ViewBag.ListUser = _iTaskManagementService.GetListUserViews();
            Dictionary<string, string> status = new Dictionary<string, string>();
            //== 0 => create => ownwer: Edit, Force Complete, Delete (New, Reopen, Cancel)
            if (taskMan.TaskDetail.TaskID == 0)
            {
                //  status.Add(TaskManagementStatus.New, taskMan.TaskDetail.STATUS);
                //  status.Add(TaskManagementStatus.Reopen, TaskManagementStatus.Reopen);
                //  status.Add(TaskManagementStatus.Cancel, TaskManagementStatus.Cancel);
                ViewBag.Status = status;
            }
            //!= 0 => edit
            //else if (taskMan.TaskDetail.TaskID != 0)
            //{
            //    //ownwer: Edit, Force Complete, Cancel, Delete (New, Reopen, Cancel)
            //    if (taskMan.TaskDetail.OWNER == User.Identity.GetUserId())
            //    {
            //        ViewBag.CanSave = "True";
            //        ViewBag.CanDeleteAndForceCom = "True";
            //        if (taskMan.TaskDetail.STATUS == TaskManagementStatus.Approve || taskMan.TaskDetail.STATUS == TaskManagementStatus.Reject)
            //        {
            //            status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //        }
            //        else
            //        {
            //            if (/*taskMan.TaskDetail.STATUS == TaskManagementStatus.InComplete || */taskMan.TaskDetail.STATUS == TaskManagementStatus.Complete)
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //               // status.Add(TaskManagementStatus.New, TaskManagementStatus.New);
            //              //  status.Add(TaskManagementStatus.Reopen, TaskManagementStatus.Reopen);
            //              //  status.Add(TaskManagementStatus.Cancel, TaskManagementStatus.Cancel);
            //            }
            //            else
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //              //  status.Add(TaskManagementStatus.Reopen, TaskManagementStatus.Reopen);
            //              //  status.Add(TaskManagementStatus.Cancel, TaskManagementStatus.Cancel);
            //            }
            //        }
            //    }
            //    //assigne: Edit neu chua Approve or Reject (Complete)
            //    if (taskMan.TaskDetail.ASSIGNEE == User.Identity.GetUserId())
            //    {
            //        ViewBag.CanSave = "True";
            //        if (ViewBag.CanDeleteAndForceCom != "True")
            //        {
            //            ViewBag.CanDeleteAndForceCom = "False";
            //        }
            //        if (taskMan.TaskDetail.STATUS == TaskManagementStatus.Approve || taskMan.TaskDetail.STATUS == TaskManagementStatus.Reject)
            //        {
            //            if (!status.ContainsKey(taskMan.TaskDetail.STATUS))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //        }
            //        else
            //        {
            //            //check co chua, neu co r ma add thi bi loi
            //            if (!status.ContainsKey(taskMan.TaskDetail.STATUS))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //            if (!status.ContainsKey(TaskManagementStatus.Complete))
            //            {
            //                    status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //        }
            //    }

            //    //kiem tra user co nam trong ls Appver khong
            //    bool isApprove = false;
            //    if (taskMan.TaskDetail.APPROVE != null)
            //    {
            //        string[] lsApp = taskMan.TaskDetail.APPROVE.Split(new string[] { "<br/>" }, StringSplitOptions.None);
            //        foreach (var app in lsApp)
            //        {
            //            if (app == User.Identity.GetUserId())
            //            {
            //                isApprove = true;
            //                break;
            //            }
            //        }
            //    }
            //    //approver: Approve or Reject, sau khi approve hay reject thi khong duoc sua nua
            //    if (isApprove)
            //    {
            //        ViewBag.CanSave = "True";
            //        if (ViewBag.CanDeleteAndForceCom != "True")
            //        {
            //            ViewBag.CanDeleteAndForceCom = "False";
            //        }
            //        if (taskMan.TaskDetail.STATUS == TaskManagementStatus.Approve || taskMan.TaskDetail.STATUS == TaskManagementStatus.Reject)
            //        {
            //            if (!status.ContainsKey(taskMan.TaskDetail.STATUS))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //        }
            //        //if (taskMan.TaskDetail.STATUS == TaskManagementStatus.InComplete)
            //        //{
            //        //    if (!status.ContainsKey(taskMan.TaskDetail.STATUS))
            //        //    {
            //        //        status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //        //    }
            //        //}
            //        if (taskMan.TaskDetail.STATUS == TaskManagementStatus.New || taskMan.TaskDetail.STATUS == TaskManagementStatus.Reopen || taskMan.TaskDetail.STATUS == TaskManagementStatus.Cancel)
            //        {
            //            if (!status.ContainsKey(taskMan.TaskDetail.STATUS))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //        }
            //        if (taskMan.TaskDetail.STATUS == TaskManagementStatus.Complete)
            //        {
            //            if (!status.ContainsKey(taskMan.TaskDetail.STATUS))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //            if (!status.ContainsKey(TaskManagementStatus.Approve))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //            if (!status.ContainsKey(TaskManagementStatus.Reject))
            //            {
            //                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
            //            }
            //        }
            //    }

            //}
            else
            {
                status.Add(taskMan.TaskDetail.STATUS, taskMan.TaskDetail.STATUS);
                ViewBag.Status = status;
            }
            Dictionary<string, string> priority = new Dictionary<string, string>
            {
                { TaskManagementPriority.Low, TaskManagementPriority.Low },
                { TaskManagementPriority.Normal, TaskManagementPriority.Normal },
                { TaskManagementPriority.High, TaskManagementPriority.High },
                { TaskManagementPriority.Urgent, TaskManagementPriority.Urgent },
                { TaskManagementPriority.Immediate, TaskManagementPriority.Immediate }
            };
            ViewBag.Priority = priority;
            Dictionary<string, string> type = new Dictionary<string, string>
            {
                { TaskManagementType.Document, TaskManagementType.Document }
            };
            ViewBag.Type = type;
        }
        #endregion
        public ActionResult ListTaskmanagement()
        {
            return View();
        }
        public JsonResult ReadListTaskMantNCR([DataSourceRequest] DataSourceRequest request)
        {

            return Json(_iTaskManagementService.GetListTaskNCR().ToDataSourceResult(request));
        }
        public JsonResult ReadListTaskMant([DataSourceRequest] DataSourceRequest request)
        {

            return Json(_iTaskManagementService.GetListTask().ToDataSourceResult(request));
        }
        public JsonResult SearchListTaskMant([DataSourceRequest] DataSourceRequest request,string keyword)
        {

            return Json(_iTaskManagementService.GetListTask(keyword).ToDataSourceResult(request));
        }
        [HttpPost]
        public JsonResult CreateTaskList(string type, string topic, string topicDetail, string reference, string phase, string reference2)
        {
            string idUser = User.Identity.GetUserId();
            bool chek = _iTaskManagementService.SaveTaskList(type, topic, topicDetail, idUser, reference, phase, reference2);
            string num = _iTaskManagementService.GetcurrentTaskListCreate();
            
            if (chek && type == "OTHER")
            {
                return Json(new { result = true, data = num });
            }
            else if(!chek)
            {
                return Json(new { result = false });

            }
            else
            {
                return Json(new { result = true, data =""});
            }
        }
        [HttpPost]
        public ActionResult GetUrlViewTask(string TaskNum)
        {
            string type = _iTaskManagementService.GetTypeCategory(TaskNum); ;
            string url1 = _iTaskManagementService.GetUrlCategory(type);
            //string[] arrListStr = url.Split(';');
            //string Contronler = arrListStr[0];
            //string Action = arrListStr[1];
            //var Num = arrListStr[2];
            // if(type == "NCR")
            // {
            return Json(new { url = url1 });
            //sCaR
           // return Json(new { url = Url.Action(Action, Contronler, new { SCAR = TaskNum }) });
            //  }
            //  return View();

            //return Redirect(Url.Action(Action, Contronler, new { NCR_NUM = TaskNum }));

        }
        public ActionResult ViewTaskList(string TaskNum)
        {
            TaskListViewModel model = _iTaskManagementService.GetTaskList(TaskNum);
            ViewBag.Topic = model.Topic;
            ViewBag.TaskDetail = model.Task_Detail;
            ViewBag.Ref = model.Reference;
            ViewBag.User = model.WRITTENBY;
            ViewBag.Date = model.WRITEDATE;
            ViewBag.Type = model.TYPE;
            ViewBag.TaskID = model.TopicID;
            ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(TaskNum.Trim());
            return View();
        }
        [HttpGet]
        public ActionResult ViewTaskListID(int TaskID)
        {
            TaskListViewModel model = _iTaskManagementService.GetTaskListID(TaskID);
            ViewBag.Topic = model.Topic;
            ViewBag.TaskDetail = model.Task_Detail;
            ViewBag.Ref = model.Reference;
            ViewBag.User = model.WRITTENBY;
            ViewBag.Date = model.WRITEDATE;
            ViewBag.Type = model.TYPE;
            ViewBag.TaskID = model.TopicID;
            ViewBag.TaskList = _iTaskManagementService.GetTaskListByTaskNO(model.Reference.Trim());
            return View();
        }
        public ActionResult TaskReport()
        {
            ViewBag.DeptList = _iTaskManagementService.GetDeptList();
            return View();
        }
        public ActionResult TaskListReport()
        {
            ViewBag.DeptList = _iTaskManagementService.GetDeptList();
            return View();
        }
        public ActionResult Task_Statistical_ByUser()
        {
            ViewBag.UserList = _iTaskManagementService.GetUserList();
            return View();
        }
        public ActionResult Task_Statistical_ByDept()
        {
            ViewBag.DeptList = _iTaskManagementService.GetDeptList();
            return View();
        }
        public ActionResult Task_Statistical_ByDetail(string SearchType, string SearchValue, string TaskStatus)
        {
            ViewBag.SearchType = SearchType;
            ViewBag.TaskStatus = TaskStatus;
            ViewBag.SearchValue = SearchValue;
            ViewBag.TypeTask = _iTaskManagementService.GetdropdownTypeTask();
            return View();
        }
        public ActionResult Task_Search_Result()
        {
            return View();
        }

    }
}
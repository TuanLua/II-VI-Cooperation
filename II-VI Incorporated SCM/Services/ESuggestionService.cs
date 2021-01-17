using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.ESuggestion;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IESuggestionService
    {
        List<SelectListItem> getuserboard(string id);
        List<SelectListItem> GetDropdownlistUser();
        List<tbl_Inv_Step1_SubmitSuggestion> GetListSearch(string title, string ideal);
        List<sp_Inv_SugList_Result> GetListIndex();
        List<SelectListItem> GetDropdownlistUserSponsor();
        bool UpdateStatusSimalar(string id, string status,string comment);
        List<SelectListItem> GetDropdownlistUserCoacher();
        List<SelectListItem> GetDropdownlistUserBoard();
        List<SelectListItem> GetDropdownlistImple();
        List<SelectListItem> selected(string ID);
        List<SelectListItem> GetDeptByUserID();
        List<SelectListItem> GetDropdownlistMember();
        bool SaveBoardSponsor(ESuggestinSponsorViewModel model, string username);
        //bool SaveSubmitSuggestion(E_SuggestionCreateViewmodel model, string username);
        bool SaveEsuggesttion(E_SuggestionCreateViewmodel model);
        bool SaveBoardDirector(BoardirectorViewmodel model, string username);
        bool SaveProcessing(ProcessingViewModel model, string username);
        bool SaveLeader(LeaderViewmodel model);
        bool SavaCostSaving(CostSavingmodel model);
        ESuggestinSponsorViewModel getSponsor(string IDSuggestion);
        BoardirectorViewmodel getBoardirector(string IDSuggestion);
        ProcessingViewModel getprocess(string IDSuggestion);
        List<sp_Inv_GetStepInfor_Result> GetListManagement(string step, string id);
        CostSavingmodel getCostSaving(string Sug_ID);
        //List<sp_Inv_Report_Suggestion_Result> ReadEsuggestionData(DateTime dtFrom, DateTime dtTo, string DeptList, string Imp_Method);
        string gettitlebyid(string SuggestionID);
        string getIdeabyid(string SuggestionID);
        int getStepEsugestion(string SuggestionID);
        string GetRoleByUserID(string UserID);
        string GetDeptByUserID(string UserID);
        string GetRequestorInfo(string UserID);
        

    }
    public class ESuggestionService : IESuggestionService
    {
        private readonly IIVILocalDB _db;
        public ESuggestionService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public List<SelectListItem> GetDropdownlistUser()
        {
            List<SelectListItem> listvendor = _db.AspNetUsers.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.FullName.Trim(),
            }).ToList();
            return listvendor;
        }
        public List<SelectListItem> GetDropdownlistMember()
        {
            List<SelectListItem> listvendor = _db.V_AspNetUsers.Select(x => new SelectListItem
            {
                Value = x.id,
                Text = x.FullName.Trim(),
            }).ToList();
            return listvendor;
        }
        public List<SelectListItem> GetDeptByUserID()
        {
            List<SelectListItem> listpart = _db.sp_Inv_Get_Dept("").Select(x => new SelectListItem
            {
                Value = x.DepartmentName,
                Text = x.Dept_Code,
            }).ToList();
            return listpart;
        }
        public string GetDeptByUserID(string UserID)
        {
            string strDept, strUseCode;
            var result = _db.AspNetUsers.Where(x => x.Id == UserID).FirstOrDefault();
            if (result != null)
                strUseCode = result.OperatorID;
            else strUseCode = "";
            var deparr = _db.sp_Inv_Get_Dept(strUseCode).FirstOrDefault();
            if (deparr != null && deparr.Dept_Code == null) strDept = ""; else strDept = deparr.Dept_Code;
            return strDept;
        }
        public string GetRequestorInfo(string UserID)
        {
            string strReqtorInfo;
            var ReqtorInfo = _db.sp_Inv_Get_Dept(UserID).FirstOrDefault();
            if (ReqtorInfo != null)
            {
                strReqtorInfo = ReqtorInfo.OperatorName != null ? ReqtorInfo.OperatorName : "";
                strReqtorInfo += "#";
                strReqtorInfo += ReqtorInfo.Dept_Code != null ? ReqtorInfo.Dept_Code : "";
                strReqtorInfo += "#";
                strReqtorInfo += ReqtorInfo.DepartmentName != null ? ReqtorInfo.DepartmentName : "";//Temporory Departmentname is email, will change late
                return strReqtorInfo;
            }
            return "";
        }

        public List<SelectListItem> GetDropdownlistUserSponsor()
        {
            List<SelectListItem> user = (from a in _db.AspNetUsers
                                         join role in _db.tbl_Inv_Role on a.Id equals role.User_ID
                                         where (role.User_Role == "Sponsor")
                                         select (new SelectListItem
                                         {
                                             Value = a.Id,
                                             Text = a.FullName.Trim(),
                                         })).ToList();
            return user;
        }
        public List<SelectListItem> GetDropdownlistUserBoard()
        {
            List<SelectListItem> user = (from a in _db.AspNetUsers
                                         join role in _db.tbl_Inv_Role on a.Id equals role.User_ID
                                         where (role.User_Role == "Director")
                                         select (new SelectListItem
                                         {
                                             Value = a.Id,
                                             Text = a.FullName.Trim(),
                                         })).ToList();
            return user;
        }
        private bool updatestatusbystept(string IDSuggestion, string Step)
        {
            var Suggestion = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_ID == IDSuggestion).FirstOrDefault();
            Suggestion.Cur_Step = Step;
            _db.SaveChanges();
            return true;
        }
        public List<SelectListItem> GetDropdownlistImple()
        {
            List<SelectListItem> user = (from a in _db.tbl_Inv_ImpMethod
                                         select (new SelectListItem
                                         {
                                             Value = a.Method_ID.ToString(),
                                             Text = a.Method_name.Trim(),
                                         })).ToList();
            return user;
        }
        public List<SelectListItem> GetDropdownlistUserCoacher()
        {
            List<SelectListItem> user = (from a in _db.AspNetUsers
                                         join role in _db.tbl_Inv_Role on a.Id equals role.User_ID
                                         where (role.User_Role == "Coacher")
                                         select (new SelectListItem
                                         {
                                             Value = a.Id,
                                             Text = a.FullName.Trim(),
                                         })).ToList();
            return user;
        }
        public List<tbl_Inv_Step1_SubmitSuggestion> GetListSearch(string title, string idea)

        {
            var result= _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_title.Contains(title) || x.Idea_impr.Contains(idea)).ToList();
            if (idea == "") { result = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_title.Contains(title)).ToList(); }
            else if (title == "") result = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Idea_impr.Contains(idea)).ToList();
            //else result = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_title.Contains(title) || x.Idea_impr.Contains(idea)).ToList();
            return result;
        }
        public List<sp_Inv_GetStepInfor_Result> GetListManagement(string step, string id)
        {
            var result = _db.sp_Inv_GetStepInfor(step, id).ToList();

            return result;
        }
        public List<sp_Inv_SugList_Result> GetListIndex()
        {
            var result = _db.sp_Inv_SugList().ToList();
            return result;
        }
        public bool UpdateStatusSimalar(string id, string status,string comment)
        {
            string index = "";
            //var result = _db.tbl_Inv_Apr_Process.Where(x => x.Step_ID == id && x.Step_ID == "Step2").ToList();
            //result.OrderByDescending(x => x.Step_Idx);
            //if (result.Count > 0)
            //{
            //    index = result.Select(x => x.Step_Idx).FirstOrDefault();
            //}
            //else
            //{
            //    index = "1";
            //}
            var result = _db.tbl_Inv_Apr_Process.Where(x => x.Sug_ID == id && x.Step_ID == "2").ToList();
            //Each step just have one row data
            if (result.Count > 0) return true;
            //Thi.nguyen: With AssignedTask status, just update current step after save task
            if (status == "AssignedTask") return true;
            tbl_Inv_Apr_Process data = new tbl_Inv_Apr_Process();
            data.Status = status;
            data.Step_ID = "2";
            data.Sug_ID = id;
            data.Step_Idx = "2";
            data.Action_Date = DateTime.Now;
            
            data.Comment = comment;
            _db.tbl_Inv_Apr_Process.Add(data);
            _db.SaveChanges();
            updatestatusbystept(id, "2");
            if (status == "Approve")
                _db.sp_Inv_SendMail_Approve(id, 2, status);
            else _db.sp_Inv_SendMail_Reject(id, 2, status);
            return true;
        }
        public bool SavaCostSaving(CostSavingmodel model)
        {
            var checkexist = _db.tbl_Inv_Cost_Saving.Where(x => x.Sug_ID == model.Sug_ID).FirstOrDefault();
            try
            {

                if (checkexist != null)
                {
                    checkexist.Jan = model.Jan;
                    checkexist.Feb = model.Feb;
                    checkexist.Mar = model.Mar;
                    checkexist.Apr = model.Apr;
                    checkexist.May = model.May;
                    checkexist.Jun = model.Jun;
                    checkexist.Jul = model.Jul;
                    checkexist.Aug = model.Aug;
                    checkexist.Sep = model.Sep;
                    checkexist.Oct = model.Oct;
                    checkexist.Nov = model.Nov;
                    checkexist.Dec = model.Dec;
                    checkexist.User_Input = model.User_Input;
                    _db.SaveChanges();
                }
                else
                {
                    tbl_Inv_Cost_Saving data = new tbl_Inv_Cost_Saving();
                    data.Sug_ID = model.Sug_ID;
                    data.Jan = model.Jan;
                    data.Feb = model.Feb;
                    data.Mar = model.Mar;
                    data.Apr = model.Apr;
                    data.May = model.May;
                    data.Jun = model.Jun;
                    data.Jul = model.Jul;
                    data.Aug = model.Aug;
                    data.Sep = model.Sep;
                    data.Oct = model.Oct;
                    data.Nov = model.Nov;
                    data.Dec = model.Dec;
                    data.User_Input = model.User_Input;
                    data.Date_Input = DateTime.Now;
                    _db.tbl_Inv_Cost_Saving.Add(data);
                    _db.SaveChanges();
                }
                _db.sp_Inv_Cal_ScoreCost_4Mem(model.Sug_ID);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveEsuggesttion(E_SuggestionCreateViewmodel model)
        {
            //tbl_Inv_Step1_SubmitSuggestion data = new tbl_Inv_Step1_SubmitSuggestion();
            //data.Sug_ID =
            //    data.Sug_title = model.Sug_title;
            //data.Sug_Type = model.Sug_Type;
            //data.Req_Email = model.Req_Email;
            //data.Submit_date = DateTime.Now;
            //data.Idea_impr = model.Idea_impr;
            //data.Exp_End_date = model.Exp_End_date;
            //data.Exp_Start_Date = model.Exp_Start_Date;
            //data.Cur_prob = model.Cur_prob;
            //data.Cur_Step = "1";
            string Att_File_Current = "";
            string Att_File_Idea = "";
            if (model.ModelEvidence != null)
            {
                foreach (var item in model.ModelEvidence)
                {
                    Att_File_Current += item.EvidenceFile.FileName + "/";
                }
                Att_File_Current = Att_File_Current.Substring(0, Att_File_Current.Length - 1);
            }
            if (model.ModelEvidenceIdea != null)
            {
                foreach (var item in model.ModelEvidenceIdea)
                {
                    Att_File_Idea += item.EvidenceFile.FileName + "/";
                }
                Att_File_Idea = Att_File_Idea.Substring(0, Att_File_Idea.Length - 1);
            }
            _db.sp_Inv_Create_Sug(model.Sug_Type, model.Sug_title, model.Cur_prob, model.Idea_impr, "1", model.Exp_Start_Date, model.Exp_End_date, model.Submit_date, model.Submitter, model.Requestor, model.rqtor_name, model.Req_Dept, model.Req_Email, Att_File_Current, Att_File_Idea);
            
            return true;
        }
        public bool SaveBoardDirector(BoardirectorViewmodel model, string username)
        {
            // get index
            string index = "";
            //var result = _db.tbl_Inv_Apr_Process.Where(x => x.Step_ID == model.Sug_ID && x.Step_ID == "4").ToList();
            //result.OrderByDescending(x => x.Step_Idx);
            //if (result.Count > 0)
            //{
            //    index = result.Select(x => x.Step_Idx).FirstOrDefault();
            //}
            //else
            //{
            //    index = "1";
            //}

            try
            {
                var checkedit = _db.tbl_Inv_Step4_DirApr.Where(x => x.Sug_ID == model.Sug_ID && x.Director == username).FirstOrDefault();
                var process4 = _db.tbl_Inv_Apr_Process.Where(x => x.Sug_ID == model.Sug_ID && x.Step_ID == "4").FirstOrDefault();

                if (!string.IsNullOrEmpty(model.Status))
                    if (model.Status != "Approve")
                    {
                        if (checkedit != null)
                        {
                            checkedit.Sug_ID = model.Sug_ID;
                            checkedit.Stra_com = model.Stra_com;
                            checkedit.Stra_Link = model.Stra_Link;
                            checkedit.App_com = model.App_com;
                            checkedit.App_eva = model.App_eva;
                            checkedit.Apr_Status = false;// model.Apr_Status;
                            checkedit.Director = username;
                            checkedit.Comment = model.Comment;
                            checkedit.Date_Action = DateTime.Now;
                            _db.SaveChanges();
                        }
                        else
                        {
                            tbl_Inv_Step4_DirApr data = new tbl_Inv_Step4_DirApr();
                            data.Sug_ID = model.Sug_ID;
                            data.Stra_com = model.Stra_com;
                            data.Stra_Link = model.Stra_Link;
                            data.App_com = model.App_com;
                            data.App_eva = model.App_eva;
                            data.Apr_Status = false;// model.Apr_Status;
                            data.Comment = model.Comment;
                            data.Director = username;
                            data.Date_Action = DateTime.Now;
                            _db.tbl_Inv_Step4_DirApr.Add(data);
                            _db.SaveChanges();

                        }
                        updatestatusbystept(model.Sug_ID, "4");

                        if (model.Status == "Approve")
                            _db.sp_Inv_SendMail_Approve(model.Sug_ID, 4, model.Status);
                        else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 4, model.Status);

                        tbl_Inv_Apr_Process process = new tbl_Inv_Apr_Process();
                        process.Sug_ID = model.Sug_ID;
                        process.Action_Date = DateTime.Now;
                        process.Comment = model.Comment;
                        process.Step_ID = "4";
                        process.Status = model.Status;
                        process.Step_Idx = "4";
                        _db.tbl_Inv_Apr_Process.Add(process);
                        _db.SaveChanges();
                    }
                    else
                    {
                        if (checkedit != null)
                        {
                            checkedit.Sug_ID = model.Sug_ID;
                            checkedit.Stra_com = model.Stra_com;
                            checkedit.Stra_Link = model.Stra_Link;
                            checkedit.App_com = model.App_com;
                            checkedit.App_eva = model.App_eva;
                            checkedit.Apr_Status = true;// model.Apr_Status;
                            checkedit.Director = username;
                            checkedit.Comment = model.Comment;
                            checkedit.Date_Action = DateTime.Now;


                            _db.SaveChanges();
                        }
                        else
                        {
                            tbl_Inv_Step4_DirApr data = new tbl_Inv_Step4_DirApr();
                            data.Sug_ID = model.Sug_ID;
                            data.Stra_com = model.Stra_com;
                            data.Stra_Link = model.Stra_Link;
                            data.App_com = model.App_com;
                            data.App_eva = model.App_eva;
                            data.Apr_Status = true;// model.Apr_Status;
                            data.Comment = model.Comment;
                            data.Director = username;
                            data.Date_Action = DateTime.Now;
                            _db.tbl_Inv_Step4_DirApr.Add(data);
                            _db.SaveChanges();


                        }
                        var boarduser = _db.tbl_Inv_BoardDirector.Where(x => x.Sug_ID == model.Sug_ID).ToList();
                        var boardstep = _db.tbl_Inv_Step4_DirApr.Where(x => x.Sug_ID == model.Sug_ID).ToList();
                        if (boardstep.Count == boarduser.Count && model.Status == "Approve")
                        {
                            updatestatusbystept(model.Sug_ID, "4");

                            if (model.Status == "Approve")
                                _db.sp_Inv_SendMail_Approve(model.Sug_ID, 4, model.Status);
                            else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 4, model.Status);

                            tbl_Inv_Apr_Process process = new tbl_Inv_Apr_Process();
                            process.Sug_ID = model.Sug_ID;
                            process.Action_Date = DateTime.Now;
                            process.Comment = model.Comment;
                            process.Step_ID = "4";
                            process.Status = model.Status;
                            process.Step_Idx = "4";
                            _db.tbl_Inv_Apr_Process.Add(process);
                            _db.SaveChanges();
                        }

                    }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            // checkedit = _db.tbl_Inv_Step4_DirApr.Where(x => x.Sug_ID == model.Sug_ID&& x.Director == username).FirstOrDefault();
            ////add data table
            //if(checkedit != null)
            //{
            //    tbl_Inv_Step4_DirApr data = new tbl_Inv_Step4_DirApr();
            //    checkedit.Sug_ID = model.Sug_ID;
            //    checkedit.Stra_com = model.Stra_com;
            //    checkedit.Stra_Link = model.Stra_Link;
            //    checkedit.App_com = model.App_com;
            //    checkedit.App_eva = model.App_eva;
            //    checkedit.Apr_Status = model.Apr_Status;
            //    checkedit.Director = username;
            //    checkedit.Comment = model.Comment;
            //    _db.SaveChanges();
            //    var boarduser = _db.tbl_Inv_BoardDirector.Where(x => x.Sug_ID == model.Sug_ID).ToList();
            //    var boardstep = _db.tbl_Inv_Step4_DirApr.Where(x => x.Sug_ID == model.Sug_ID).ToList();
            //    if (boardstep.Count == boarduser.Count && model.Status == "Approve")
            //    {
            //        // add action in process
            //        if (!string.IsNullOrEmpty(model.Status))
            //        {
            //            var process4 = _db.tbl_Inv_Apr_Process.Where(x=>x.Sug_ID==model.Sug_ID && x.Step_ID=="4").FirstOrDefault();
            //            process4.Sug_ID = model.Sug_ID;
            //            process4.Action_Date = DateTime.Now;
            //            process4.Comment = model.Comment;
            //            process4.Step_ID = "4";
            //            process4.Status = model.Status;
            //            process4.Step_Idx = index;
            //            //_db.tbl_Inv_Apr_Process.Add(process4);
            //            _db.SaveChanges();
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(model.Status))
            //        {
            //            tbl_Inv_Apr_Process process4 = new tbl_Inv_Apr_Process();
            //            process4.Sug_ID = model.Sug_ID;
            //            process4.Action_Date = DateTime.Now;
            //            process4.Comment = model.Comment;
            //            process4.Step_ID = "4";
            //            process4.Status = model.Status;
            //            process4.Step_Idx = index;
            //            _db.tbl_Inv_Apr_Process.Add(process4);
            //            _db.SaveChanges();
            //            if (model.Status == "Approve")
            //            {
            //                updatestatusbystept(model.Sug_ID, "4");
            //            }
            //        }
            //    }


            //    return true;
            //}
            //else
            //{
            //    tbl_Inv_Step4_DirApr data = new tbl_Inv_Step4_DirApr();
            //    data.Sug_ID = model.Sug_ID;
            //    data.Stra_com = model.Stra_com;
            //    data.Stra_Link = model.Stra_Link;
            //    data.App_com = model.App_com;
            //    data.App_eva = model.App_eva;
            //    data.Apr_Status = model.Apr_Status;
            //    data.Director = username;
            //    _db.tbl_Inv_Step4_DirApr.Add(data);
            //    _db.SaveChanges();
            //    //check so luong board du thi moi update
            //    var boarduser = _db.tbl_Inv_BoardDirector.Where(x => x.Sug_ID == model.Sug_ID).ToList();
            //    var boardstep = _db.tbl_Inv_Step4_DirApr.Where(x => x.Sug_ID == model.Sug_ID).ToList();
            //    if (boardstep.Count == boarduser.Count && model.Status == "Approve")
            //    {
            //        // add action in process
            //        if (!string.IsNullOrEmpty(model.Status))
            //        {
            //            tbl_Inv_Apr_Process process4 = new tbl_Inv_Apr_Process();
            //            process4.Sug_ID = model.Sug_ID;
            //            process4.Action_Date = DateTime.Now;
            //            process4.Comment = model.Comment;
            //            process4.Step_ID = "4";
            //            process4.Status = model.Status;
            //            process4.Step_Idx = index;
            //            _db.tbl_Inv_Apr_Process.Add(process4);
            //            _db.SaveChanges();
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(model.Status))
            //        {
            //            tbl_Inv_Apr_Process process4 = new tbl_Inv_Apr_Process();
            //            process4.Sug_ID = model.Sug_ID;
            //            process4.Action_Date = DateTime.Now;
            //            process4.Comment = model.Comment;
            //            process4.Step_ID = "4";
            //            process4.Status = model.Status;
            //            process4.Step_Idx = index;
            //            _db.tbl_Inv_Apr_Process.Add(process4);
            //            _db.SaveChanges();
            //            if (model.Status == "Approve")
            //            {
            //                updatestatusbystept(model.Sug_ID, "4");
            //            }
            //        }
            //    }


            //    return true;
            //}

        }
        public bool SaveLeader(LeaderViewmodel model)
        {
            string index = "";
            //var result = _db.tbl_Inv_Apr_Process.Where(x => x.Step_ID == model.Sug_ID && x.Step_ID == "Step2").ToList();
            //result.OrderByDescending(x => x.Step_Idx);
            //if (result.Count > 0)
            //{
            //    index = result.Select(x => x.Step_Idx).FirstOrDefault();
            //}
            //else
            //{
            //    index = "1";
            //}
            //remove trc khi add neu edit
            try
            {
                var lstremove = _db.tbl_Inv_Step5_ProLeaderApr.Where(x => x.Sug_ID == model.Sug_ID).ToList();
                if (lstremove.Count > 0)
                {
                    _db.tbl_Inv_Step5_ProLeaderApr.RemoveRange(lstremove);
                    _db.SaveChanges();
                }
                List<tbl_Inv_Step5_ProLeaderApr> list = new List<tbl_Inv_Step5_ProLeaderApr>();
                tbl_Inv_Step5_ProLeaderApr data = new tbl_Inv_Step5_ProLeaderApr();
                foreach (var item in model.Member)
                {
                    data = new tbl_Inv_Step5_ProLeaderApr();
                    data.Sug_ID = model.Sug_ID;
                    data.Member = item;
                    list.Add(data);
                }
                _db.tbl_Inv_Step5_ProLeaderApr.AddRange(list);


                if (!string.IsNullOrEmpty(model.Status))
                {
                    tbl_Inv_Apr_Process process = new tbl_Inv_Apr_Process();
                    process.Sug_ID = model.Sug_ID;
                    process.Action_Date = DateTime.Now;
                    process.Step_ID = "5";
                    process.Status = model.Status;
                    process.Step_Idx = "5";
                    _db.tbl_Inv_Apr_Process.Add(process);
                    updatestatusbystept(model.Sug_ID, "5");

                    if (model.Status == "Approve")
                        _db.sp_Inv_SendMail_Approve(model.Sug_ID, 5, "Kicked off");
                    else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 5, "Kicked off");
                }

                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return true;
            }
        }
        public bool SaveProcessing(ProcessingViewModel model, string username)
        {
            // get index
            string index = "";
            //var result = _db.tbl_Inv_Apr_Process.Where(x => x.Step_ID == model.Sug_ID && x.Step_ID == "Step2").ToList();
            //result.OrderByDescending(x => x.Step_Idx);
            //if (result.Count > 0)
            //{
            //    index = result.Select(x => x.Step_Idx).FirstOrDefault();
            //}
            //else
            //{
            //    index = "1";
            //}
            var checkedit = _db.tbl_Inv_Step2_CIAproval.Where(x => x.Sug_ID == model.Sug_ID).FirstOrDefault();
            if (checkedit != null)
            {

                checkedit.Sug_ID = model.Sug_ID;
                checkedit.Coacher = model.Coacher;
                checkedit.Sponsor = model.Sponsor;
                checkedit.Imp_Method = model.Imp_Method;
                checkedit.Comment = model.Comment;
                _db.SaveChanges();
                //save data board director
                var checkboard = _db.tbl_Inv_BoardDirector.Where(x => x.Sug_ID == model.Sug_ID).ToList();
                if (checkboard.Count > 0)
                {
                    _db.tbl_Inv_BoardDirector.RemoveRange(checkboard);
                    _db.SaveChanges();
                }
                if (model.Board.Count > 0)
                {
                    List<tbl_Inv_BoardDirector> lst = new List<tbl_Inv_BoardDirector>();
                    foreach (var item in model.Board)
                    {
                        tbl_Inv_BoardDirector director = new tbl_Inv_BoardDirector();
                        director.Sug_ID = model.Sug_ID;
                        director.Director = item;
                        lst.Add(director);
                    }
                    _db.tbl_Inv_BoardDirector.AddRange(lst);
                    _db.SaveChanges();
                }
                // add action in process
                if (!string.IsNullOrEmpty(model.Status))
                {
                    tbl_Inv_Apr_Process process = new tbl_Inv_Apr_Process();
                    process.Sug_ID = model.Sug_ID;
                    process.Action_Date = DateTime.Now;
                    process.Comment = model.Comment;
                    process.Step_ID = "2";
                    process.Status = model.Status;
                    process.Step_Idx = "2";
                    _db.tbl_Inv_Apr_Process.Add(process);
                    _db.SaveChanges();
                    //if (model.Status == "Approve")
                    //{
                        updatestatusbystept(model.Sug_ID, "2");
                    //}
                    if (model.Status == "Approve")
                        _db.sp_Inv_SendMail_Approve(model.Sug_ID, 2, model.Status);
                    else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 2, model.Status);
                }

                return true;
            }
            else
            {

                //add data table
                tbl_Inv_Step2_CIAproval data = new tbl_Inv_Step2_CIAproval();
                //{
                data.Sug_ID = model.Sug_ID;
                data.Coacher = model.Coacher;
                data.Sponsor = model.Sponsor;
                data.Imp_Method = model.Imp_Method;
                data.Comment = model.Comment;
                _db.tbl_Inv_Step2_CIAproval.Add(data);
                _db.SaveChanges();
                //save data board director
                var checkboard = _db.tbl_Inv_BoardDirector.Where(x => x.Sug_ID == model.Sug_ID).ToList();
                if (checkboard.Count > 0)
                {
                    _db.tbl_Inv_BoardDirector.RemoveRange(checkboard);
                    _db.SaveChanges();
                }
                if (model.Board.Count > 0)
                {
                    List<tbl_Inv_BoardDirector> lst = new List<tbl_Inv_BoardDirector>();
                    foreach (var item in model.Board)
                    {
                        tbl_Inv_BoardDirector director = new tbl_Inv_BoardDirector();
                        director.Sug_ID = model.Sug_ID;
                        director.Director = item;
                        lst.Add(director);
                    }
                    _db.tbl_Inv_BoardDirector.AddRange(lst);
                    _db.SaveChanges();
                }
                // add action in process
                if (!string.IsNullOrEmpty(model.Status))
                {
                    tbl_Inv_Apr_Process process = new tbl_Inv_Apr_Process();
                    process.Sug_ID = model.Sug_ID;
                    process.Action_Date = DateTime.Now;
                    process.Comment = model.Comment;
                    process.Step_ID = "2";
                    process.Status = model.Status;
                    process.Step_Idx = "2";
                    _db.tbl_Inv_Apr_Process.Add(process);
                    _db.SaveChanges();
                    //if (model.Status == "Approve")
                    //{
                        updatestatusbystept(model.Sug_ID, "2");
                    //}
                    if (model.Status == "Approve")
                        _db.sp_Inv_SendMail_Approve(model.Sug_ID, 2, model.Status);
                    else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 2, model.Status);
                }

                return true;
            }

        }

        public bool SaveBoardSponsor(ESuggestinSponsorViewModel model, string username)
        {
            // get index max
            string index = "";
            //var result = _db.tbl_Inv_Apr_Process.Where(x => x.Step_ID == model.Sug_ID && x.Step_ID == "Step3").ToList();
            //result.OrderByDescending(x => x.Step_Idx);
            //if (result.Count > 0)
            //{
            //    index = result.Select(x => x.Step_Idx).FirstOrDefault();
            //}
            //else
            //{
            //    index = "1";
            //}
            var checkedit = _db.tbl_Inv_Step3_SponsorApr.Where(x => x.Sug_ID == model.Sug_ID).FirstOrDefault();
            if (checkedit != null)
            {
                //add data table
                checkedit.Sug_ID = model.Sug_ID;
                checkedit.Legal_com = model.Legal_com;
                checkedit.Legal_Rel = model.Legal_Rel;
                checkedit.Pro_Leader = model.ProLeader;
                checkedit.Imp_Start = model.Imp_Start;
                checkedit.Imp_End = model.Imp_End;
                checkedit.Res_avai = model.Res_avai;
                checkedit.Res_com = model.Res_com;
                checkedit.Safe_com = model.Safe_com;
                checkedit.Safe_Rel = model.Safe_Rel;
                checkedit.Tech_abi = model.Tech_abi;
                checkedit.Tech_com = model.Tech_com;
                checkedit.Fin_abi = model.Fin_abi;
                checkedit.Fin_com = model.Fin_com;
                checkedit.Eco_ben = model.Eco_ben;
                checkedit.Eco_com = model.Eco_com;
                checkedit.Comment = model.Comment;
                _db.SaveChanges();
                // add action in process
                if (!string.IsNullOrEmpty(model.Status))
                {
                    //Update after pending
                    var stepChk = _db.tbl_Inv_Apr_Process.Where(x => x.Sug_ID == model.Sug_ID && x.Step_ID == "3").FirstOrDefault();
                    if (stepChk != null)
                    {
                        //stepChk.Sug_ID = model.Sug_ID;
                        stepChk.Action_Date = DateTime.Now;
                        stepChk.Comment = model.Comment;
                        //stepChk.Step_ID = "3";
                        stepChk.Status = model.Status;
                        stepChk.Step_Idx = "3";
                    }
                    else
                    {
                        tbl_Inv_Apr_Process process3 = new tbl_Inv_Apr_Process();
                        process3.Sug_ID = model.Sug_ID;
                        process3.Action_Date = DateTime.Now;
                        process3.Comment = model.Comment;
                        process3.Step_ID = "3";
                        process3.Status = model.Status;
                        process3.Step_Idx = "3";
                        _db.tbl_Inv_Apr_Process.Add(process3);
                    }
                    _db.SaveChanges();
                    //if (model.Status == "Approve")
                    //{
                    updatestatusbystept(model.Sug_ID, "3");
                    //}
                    if (model.Status == "Approve")
                        _db.sp_Inv_SendMail_Approve(model.Sug_ID, 3, model.Status);
                    else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 3, model.Status);
                }
                return true;
            }
            else
            {
                //edit data table
                tbl_Inv_Step3_SponsorApr data = new tbl_Inv_Step3_SponsorApr
                {

                    Sug_ID = model.Sug_ID,
                    Legal_com = model.Legal_com,
                    Legal_Rel = model.Legal_Rel,
                    Pro_Leader = model.ProLeader,
                    Imp_Start = model.Imp_Start,
                    Imp_End = model.Imp_End,
                    Res_avai = model.Res_avai,
                    Res_com = model.Res_com,
                    Safe_com = model.Safe_com,
                    Safe_Rel = model.Safe_Rel,
                    Tech_abi = model.Tech_abi,
                    Tech_com = model.Tech_com,
                    Fin_abi = model.Fin_abi,
                    Fin_com = model.Fin_com,
                    Eco_ben = model.Eco_ben,
                    Eco_com = model.Eco_com,
                    Comment = model.Comment
                };
                _db.tbl_Inv_Step3_SponsorApr.Add(data);
                _db.SaveChanges();
                // add action in process
                if (!string.IsNullOrEmpty(model.Status))
                {
                    tbl_Inv_Apr_Process process3 = new tbl_Inv_Apr_Process();
                    process3.Sug_ID = model.Sug_ID;
                    process3.Action_Date = DateTime.Now;
                    process3.Comment = model.Comment;
                    process3.Step_ID = "3";
                    process3.Status = model.Status;
                    process3.Step_Idx = "3";
                    _db.tbl_Inv_Apr_Process.Add(process3);
                    _db.SaveChanges();
                    //if (model.Status == "Approve")
                    //{
                        updatestatusbystept(model.Sug_ID, "3");
                    //}
                    if (model.Status == "Approve")
                        _db.sp_Inv_SendMail_Approve(model.Sug_ID, 3, model.Status);
                    else _db.sp_Inv_SendMail_Reject(model.Sug_ID, 3, model.Status);
                }
                return true;
            }


        }
        public List<SelectListItem> selected(string ID)
        {

            List<SelectListItem> listResult = (from a in _db.tbl_Inv_Step5_ProLeaderApr
                                               join b in _db.AspNetUsers on a.Member equals b.Id
                                               where (a.Sug_ID == ID)
                                               select (new SelectListItem
                                               {
                                                   Value = a.Member,
                                                   Text = b.FullName.Trim(),
                                               })).ToList();
            return listResult;
        }
        //public ESuggestinSponsorViewModel getSponsor(string IDSuggestion)
        //{
        //    ESuggestinSponsorViewModel data = new ESuggestinSponsorViewModel();
        //    var model1 = (from model in _db.tbl_Inv_Step3_SponsorApr
        //                  join b in _db.AspNetUsers on model.Pro_Leader equals b.Id
        //                  where (model.Sug_ID == IDSuggestion)
        //                  select (new ESuggestinSponsorViewModel
        //                  {
        //                      Legal_com = model.Legal_com,
        //                      Legal_Rel = model.Legal_Rel == true ? true : false,
        //                      ProLeader = b.FullName,
        //                      Imp_End = model.Imp_End,
        //                      Imp_Start = model.Imp_Start,
        //                      Res_avai = model.Res_avai == true ? true : false,
        //                      Res_com = model.Res_com,
        //                      Safe_com = model.Safe_com,
        //                      Safe_Rel = model.Safe_Rel == true ? true : false,
        //                      Tech_abi = model.Tech_abi == true ? true : false,
        //                      Tech_com = model.Tech_com,
        //                      Fin_abi = model.Fin_abi == true ? true : false,
        //                      Fin_com = model.Fin_com,
        //                      Eco_ben = model.Eco_ben == true ? true : false,
        //                      Eco_com = model.Eco_com,
        //                      ProLeaderValue = model.Pro_Leader,
        //                      Comment = model.Comment,
        //                      Sug_ID = model.Sug_ID,
        //                  })).FirstOrDefault();

        //    return model1;
        //}
        public ESuggestinSponsorViewModel getSponsor(string IDSuggestion)
        {
            ESuggestinSponsorViewModel data = new ESuggestinSponsorViewModel();
            var model1 = (from model in _db.tbl_Inv_Step3_SponsorApr
                          join b in _db.AspNetUsers on model.Pro_Leader equals b.Id
                          where (model.Sug_ID == IDSuggestion)
                          select (new ESuggestinSponsorViewModel
                          {
                              Legal_com = model.Legal_com,
                              Legal_Rel = model.Legal_Rel == true ? true : false,
                              ProLeader = model.Pro_Leader,
                              Imp_End = model.Imp_End,
                              Imp_Start = model.Imp_Start,
                              Res_avai = model.Res_avai == true ? true : false,
                              Res_com = model.Res_com,
                              Safe_com = model.Safe_com,
                              Safe_Rel = model.Safe_Rel == true ? true : false,
                              Tech_abi = model.Tech_abi == true ? true : false,
                              Tech_com = model.Tech_com,
                              Fin_abi = model.Fin_abi == true ? true : false,
                              Fin_com = model.Fin_com,
                              Eco_ben = model.Eco_ben == true ? true : false,
                              Eco_com = model.Eco_com,
                              ProLeaderValue = model.Pro_Leader,
                              //Comment = "",
                              Comment = model.Comment,
                              Sug_ID = model.Sug_ID,
                          })).FirstOrDefault();

            return model1;
        }
        public BoardirectorViewmodel getBoardirector(string IDSuggestion)
        {

            BoardirectorViewmodel data = new BoardirectorViewmodel();
            var result = _db.tbl_Inv_Step4_DirApr.Where(x => x.Sug_ID == IDSuggestion).FirstOrDefault();
            data.Sug_ID = result.Sug_ID;
            data.Stra_Link = result.Stra_Link == true ? true : false;
            data.Stra_com = result.Stra_com;
            data.App_com = result.App_com;
            data.App_eva = result.App_eva == true ? true : false;
            data.Director = result.Director;
            //  data.Apr_Status = result.Apr_Status;
            return data;
        }
        public CostSavingmodel getCostSaving(string IDSuggestion)
        {
            CostSavingmodel data = new CostSavingmodel();
            var model = _db.tbl_Inv_Cost_Saving.Where(x => x.Sug_ID == IDSuggestion).FirstOrDefault();
            if (model != null)
            {
                data.Sug_ID = model.Sug_ID;
                data.Jan = (float)model.Jan;
                data.Feb = (float)model.Feb;
                data.Mar = (float)model.Mar;
                data.Apr = (float)model.Apr;
                data.May = (float)model.May;
                data.Jun = (float)model.Jun;
                data.Jul = (float)model.Jul;
                data.Aug = (float)model.Aug;
                data.Sep = (float)model.Sep;
                data.Oct = (float)model.Oct;
                data.Nov = (float)model.Nov;
                data.Dec = (float)model.Dec;
                data.User_Input = model.User_Input;
                data.Date_Input = DateTime.Now;
            }
            return data;
        }
        public ProcessingViewModel getprocess(string IDSuggestion)
        {
            ProcessingViewModel data = new ProcessingViewModel();
            var model = _db.tbl_Inv_Step2_CIAproval.Where(x => x.Sug_ID == IDSuggestion).FirstOrDefault();
            if (model.Sug_ID is null)
            {
                data.Sug_ID = IDSuggestion;
                data.Imp_Method = "";
                data.Sponsor = "";
                data.Coacher = "";
                data.Comment = "";
            }
            else
            {
                data.Sug_ID = model.Sug_ID;
                data.Imp_Method = model.Imp_Method;
                data.Sponsor = model.Sponsor;
                data.Coacher = model.Coacher;
                data.Comment = model.Comment;
            }
            return data;
        }
        public List<SelectListItem> getuserboard(string id)
        {
            var result = (from a in _db.tbl_Inv_BoardDirector
                          join us in _db.AspNetUsers on a.Director equals us.Id
                          where (a.Sug_ID == id)
                          select (new SelectListItem
                          {
                              Value = a.Director,
                              Text = us.FullName.Trim(),
                          })).ToList();
            return result;
        }
        public string gettitlebyid(string SuggestionID)
        {
            var result = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_ID == SuggestionID).FirstOrDefault();
            var title = result.Sug_title;
            return title;
        }
        public string getIdeabyid(string SuggestionID)
        {
            var result = _db.tbl_Inv_Step1_SubmitSuggestion.Where(x => x.Sug_ID == SuggestionID).FirstOrDefault();
            var Idea = result.Idea_impr;
            return Idea;
        }
        //public string getSponsorbySugID(string Sug_ID)
        //{
        //    var result = _db.tbl_Inv_Step2_CIAproval.Where(x => x.Sug_ID == Sug_ID).FirstOrDefault();
        //    var sponsor = result.Sponsor;
        //    return ID
        //}
        public int getStepEsugestion(string SuggestionID)
        {
            var result = _db.tbl_Inv_Apr_Process.Where(x => x.Sug_ID == SuggestionID).ToList();
            return result.Count;
        }
        public string GetRoleByUserID(string UserID)
        {
            var result = _db.tbl_Inv_Role.Where(x => x.User_ID == UserID && x.User_Role!="Coacher").FirstOrDefault();
            if (result != null)
            {
                var UserRole = result.User_Role;
                return UserRole;
            }
            return "";
        }
        //public List<sp_Inv_Report_Suggestion_Result> ReadEsuggestionData(DateTime dtFrom, DateTime dtTo, string DeptList, string Imp_Method)
        //{
        //    return _db.sp_Inv_Report_Suggestion(dtFrom, dtTo, DeptList, Imp_Method).ToList();
        //}
    }
}

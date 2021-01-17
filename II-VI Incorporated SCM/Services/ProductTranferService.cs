using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.Producttranfer;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IProductTranferService
    {
        Result SaveProductstranfer(tbl_PT_Infor model,string type);
        Result UpdateProductstranfer(tbl_PT_Infor model, string type);
        List<sp_PT_GetProTransfer_List_Result> getlistdata();
        List<TaskmanagementViewmodel> GetListTaskProduct(string PartNum);
        ProductViewModel getproduct(string Parnum);
        List<checklistViewmodel> getchecklist(string Parnum);
        Result SaveChecklist(List<checklistview> result, string partnum,string iduser);
        List<SelectListItem> GetdropdownPart();
        List<ProductViewModel> getlistdatabypart(string part);
        Result SaveCopyCheckSheet(string PartNum, string PartCurrent,string iduser);
        List<SelectListItem> GetdropdownBuild();
        List<SelectListItem> GetdropdownWork();
        List<SelectListItem> GetListUserRoleOwner(string Id);
        List<SelectListItem> GetListTaskProduct();
        Result UpdateConlusionProductstranfer(tbl_PT_Infor model, string type,string iduser);
         tbl_PT_Infor GetFileWithFile(string fileId, string filename);
        List<string> getrole(string Iduser);
        bool GetStepstautusedit(string PartNum);
        Result SaveEditProductstranfer(tbl_PT_Infor model, string type);
        Result SaveItemCheckSheet(string item, string required, string remark, string partnum, string type);
        Result DeletedItemCheckList(string part, string Item_Desc, Byte Index);
        bool GetStep1stautussave(string PartNum);
         bool GetStep2stautussave(string PartNum);
        bool GetStep3stautussave(string PartNum);
        bool GetStep4stautussave(string PartNum);
        bool IsUsercreate(string iduser,string part);
        bool IsUserOwner(string iduser,string part);
        bool IsUserUpdate(string iduser, string part);
        bool IsUpdate(string part);
        Result EditItemCheckList(string part, string Item_Desc, Byte Index);
       List<ProductViewModel> getlistdatabyuser(string id);
        List<SelectListItem> GetdropdownSection();
        //List<string> getchecklist(string Parnum, string id);
        bool SaveData(List<tbl_PT_Infor> model);
        int sentmailsubmitInitial(string iduser, string email);
        string getemailbyid(string id);
        int sentmailsubmitvnower(string part, string email);
        string sentmailsubmitchecktask(string partnum);
        List<string> getpartbyuser(string Parnum, string id);
        List<tbl_PT_Infor> getTransferbyuser(string Parnum, string id);
    }
    public class ProductTranferService : IProductTranferService
    {
        private IIVILocalDB _db;
        public ProductTranferService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public Result SaveProductstranfer(tbl_PT_Infor model,string type)
        {
            LogWriter _log = new LogWriter("Create product tranfer");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    // save add status 
                    if (type != "Submit")
                    {
                        tbl_PT_Infor check = _db.tbl_PT_Infor.Where(x => x.Part_Num == model.Part_Num).FirstOrDefault();
                        if (check != null)
                        {
                            return new Result
                            {
                                success = false,
                                message = "Part has been created Product Tranfer !!!"
                            };
                        }
                        //save info
                        _db.tbl_PT_Infor.Add(model);
                        List<tbl_PT_Status> lst = new List<tbl_PT_Status>();
                    tbl_PT_Status data1 = new tbl_PT_Status
                    {
                        Part_Num = model.Part_Num,
                        Step_Idx = 1,
                        Step_Name = "Initial_Info",
                        Step_Status = false,
                        Date_Finish = null
                    };
                    lst.Add(data1);
                    tbl_PT_Status data2 = new tbl_PT_Status();
                        data2.Part_Num = model.Part_Num;
                        data2.Step_Idx = 2;
                        data2.Step_Name = "Update_Info";
                        data2.Step_Status = false;
                        data2.Date_Finish = null;
                    lst.Add(data2);
                    tbl_PT_Status data3 = new tbl_PT_Status();
                        data3.Part_Num = model.Part_Num;
                        data3.Step_Idx = 3;
                        data3.Step_Name = "Make_CheckList";
                    data3.Step_Status = false;
                        data3.Date_Finish = null;
                    lst.Add(data3);
                    tbl_PT_Status data4 = new tbl_PT_Status();
                        data4.Part_Num = model.Part_Num;
                        data4.Step_Idx = 4;
                        data4.Step_Name = "Conclusion";
                       // data4.Step_Status = false;
                        data4.Date_Finish = null;
                    lst.Add(data4);
                    _db.tbl_PT_Status.AddRange(lst);
                        _db.SaveChanges();
                        tranj.Commit();
                        return new Result
                        {
                            message = $@"Create Product Tranfer success!",
                            success = true,
                        };
                    }
                    else
                    {
                        //submit list part by user create
                        List<string> listpart = getpartbyuser(model.Part_Num, model.Initail_User);
                        if(listpart.Count > 0)
                        {
                            foreach (var item in listpart)
                            {
                                var dataupdate = _db.tbl_PT_Status.Where(x => x.Part_Num == item && x.Step_Idx == 1 && x.Step_Name == "Initial_Info").FirstOrDefault();
                                dataupdate.Step_Status = true;
                                dataupdate.Date_Finish = DateTime.Now;
                                dataupdate.User_Finish = model.Initail_User;

                            }
                        }

                        _db.SaveChanges();
                        tranj.Commit();
                        return new Result
                        {
                            message = $@"submit",
                            success = true,
                        };
                    }
                 
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Save product Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }

        public Result SaveEditProductstranfer(tbl_PT_Infor model, string type)
        {
            LogWriter _log = new LogWriter("Edit product tranfer");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    tbl_PT_Infor check = _db.tbl_PT_Infor.Where(x => x.Part_Num == model.Part_Num).FirstOrDefault();
                    check.Part_Num = model.Part_Num;
                    check.Initial_Note = model.Initial_Note;
                    check.Plan_Type = model.Plan_Type;
                    check.Description = model.Description;
                    check.Date = model.Date;
                    _db.Entry(check).State = EntityState.Modified;
                    _db.SaveChanges();

                    if(type == "submit")
                    {
                        //submit list part by user create
                        List<string> listpart = getpartbyuser(model.Part_Num, model.Initail_User);
                        if (listpart.Count > 0)
                        {
                            foreach (var item in listpart)
                            {
                                var dataupdate = _db.tbl_PT_Status.Where(x => x.Part_Num == item && x.Step_Idx == 1 && x.Step_Name == "Initial_Info").FirstOrDefault();
                                dataupdate.Step_Status = true;
                                dataupdate.Date_Finish = DateTime.Now;
                                dataupdate.User_Finish = check.Initail_User;                                
                            }
                        }
                        _db.SaveChanges();
                        tranj.Commit();
                        return new Result
                        {
                            message = $@"submit",
                            success = true,
                        };

                    }
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"Edit Product Tranfer success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Edit product Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public Result UpdateProductstranfer(tbl_PT_Infor model, string type)
        {
            LogWriter _log = new LogWriter("Update product tranfer");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    tbl_PT_Infor data = _db.tbl_PT_Infor.Where(x => x.Part_Num == model.Part_Num).FirstOrDefault();
                    //update info
                    data.Plan_Yield = model.Plan_Yield;
                    data.PE_Note = model.PE_Note;
                    data.PE_User = model.PE_User;
                    data.Setion = model.Setion;
                    data.Build_Loc = model.Build_Loc;
                    data.Wc = model.Wc;
                    data.Vn_Owner = model.Vn_Owner;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                    // save add status 
                    if (type == "Submit")
                    {
                        var status = _db.tbl_PT_Status.Where(x => x.Part_Num == model.Part_Num && x.Step_Idx == 2  && x.Step_Name == "Update_Info").FirstOrDefault();
                        status.User_Finish = model.PE_User;
                        status.Date_Finish = DateTime.Now;
                        status.Step_Status = true;
                    }
                   
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"Update Product Tranfer success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Update product Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }

        public List<ProductViewModel> getlistdatabyuser(string id)
        {
            List<ProductViewModel> lst = (from nc in _db.tbl_PT_Infor
                                          join status in _db.tbl_PT_Status on nc.Part_Num equals status.Part_Num
                                          join asp in _db.AspNetUsers on nc.Initail_User equals asp.Id
                                          into joined
                                          from j in joined.DefaultIfEmpty()
                                          where(nc.Initail_User == id && status.Step_Idx == 1 && status.Step_Status == false)
                                          select new ProductViewModel
                                          {
                                              Part_Num = nc.Part_Num,
                                              PE_Note = nc.PE_Note,
                                              Plan_Type = nc.Plan_Type,
                                              Description = nc.Description,
                                              Date = nc.Date,
                                              Initail_User = j.FullName,
                                              StatusInfo = nc.Status,
                                          }).ToList();
            return lst;
        }
        public List<sp_PT_GetProTransfer_List_Result> getlistdata()
        {
            List<sp_PT_GetProTransfer_List_Result> lst = _db.sp_PT_GetProTransfer_List().ToList();
            return lst;
        }
        public List<TaskmanagementViewmodel> GetListTaskProduct(string PartNum)
        {
            IQueryable<TaskmanagementViewmodel> result = from task in _db.TASKLISTs
                                                         join taskdetail in _db.TASKDETAILs on task.TopicID equals taskdetail.TopicID
                                                         join taskcomment in _db.TASKCOMMENTs.Where(m=>m.TASK_STATUS== "Completed") on taskdetail.IDTask equals taskcomment.TASKID_DETAIL
                                                         into taskinfo
                                                         from k in taskinfo.DefaultIfEmpty()
                                                         join asp in _db.AspNetUsers on taskdetail.OWNER equals asp.Id
                                                         into joined
                                                         from j in joined.DefaultIfEmpty()
                                                         join asp1 in _db.AspNetUsers on taskdetail.ASSIGNEE equals asp1.Id
                                                          into joined1
                                                         from j1 in joined1.DefaultIfEmpty()
                                                         join asp2 in _db.AspNetUsers on taskdetail.APPROVE equals asp2.Id
                                                        into joined2
                                                         from j2 in joined2.DefaultIfEmpty()
                                                         where (task.TYPE == "ProductTranfer" && task.Reference == PartNum )
                                                         select (new TaskmanagementViewmodel
                                                         {
                                                             RefNUMBER = task.Topic,
                                                             Taskname = taskdetail.TASKNAME,
                                                             TaskDescription = k.CONTENTCOMMENT,
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
        public ProductViewModel getproduct (string Parnum)
        {
            var nc = _db.tbl_PT_Infor.Where(x => x.Part_Num == Parnum).FirstOrDefault();
            if (nc != null)
            {
                ProductViewModel data = new ProductViewModel()
                {
                    Part_Num = nc.Part_Num,
                    PE_Note = nc.PE_Note,
                    Plan_Type = nc.Plan_Type,
                    Description = nc.Description,
                    Date = nc.Date,
                    Build_Loc = nc.Build_Loc,
                    Plan_Yield = nc.Plan_Yield,
                    Setion = nc.Setion,
                    Initial_Note = nc.Initial_Note,
                    Vn_Owner = nc.Vn_Owner,
                    Wc = nc.Wc,
                    GM = nc.GM,
                    Conlusion = nc.Conclution,
                    filename = nc.FileConclusion,
                    Yield = nc.Yield,
                    StatusInfo = nc.Status
                };
                return data;
            }
            else
            {
                ProductViewModel data1 = new ProductViewModel();
                return data1;
            }
        }

        public List<string> getpartbyuser(string Parnum,string id)
        {
           var lst = (from nc in _db.tbl_PT_Infor
                                          join status in _db.tbl_PT_Status on nc.Part_Num equals status.Part_Num
                                          join asp in _db.AspNetUsers on nc.Initail_User equals asp.Id
                                          into joined
                                          from j in joined.DefaultIfEmpty()
                                          where (nc.Initail_User == id && status.Step_Idx == 1 && status.Step_Status == false && nc.Status == "In Process")
                                          select 
                                               nc.Part_Num
                                              
                                          ).ToList();
            if(lst.Count > 0)
            {
                return lst;
            }
            else
            {
                return new List<string>();
            }
           
        }
        public List<tbl_PT_Infor> getTransferbyuser(string Parnum, string id)
        {
            var lst = (from nc in _db.tbl_PT_Infor
                       join status in _db.tbl_PT_Status on nc.Part_Num equals status.Part_Num
                       join asp in _db.AspNetUsers on nc.Initail_User equals asp.Id
                       into joined
                       from j in joined.DefaultIfEmpty()
                       where (nc.Initail_User == id && status.Step_Idx == 1 && status.Step_Status == false && nc.Status == "In Process")
                       select nc ).ToList();
            if (lst.Count > 0)
            {
                return lst;
            }
            else
            {
                return new List<tbl_PT_Infor>();
            }

        }
        public List<checklistViewmodel> getchecklist(string Parnum)
        {
            var lst = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == Parnum).Select(x=>new checklistViewmodel
            {
                Part_Num = x.Part_Num,
                Data_Type = x.Data_Type,
                Is_Require = x.Is_Require,
                Item_Desc = x.Item_Desc,
                Item_Idx = x.Item_Idx,
                Item_Remark = x.Item_Remark,
                Item_Value = x.Item_Value,
                
            }).ToList();

            return lst;
        }
        public Result SaveChecklist(List<checklistview> result, string partnum,string iduser)
        {
            LogWriter _log = new LogWriter("Update Checklist product tranfer");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in result)
                    {
                        var data = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == partnum  && x.Item_Idx == item.key).FirstOrDefault();
                        if(data!= null)
                        {
                            //if(item.value == "true")
                            //{
                            //    item.value = '1';
                            //}
                            data.Item_Value = item.value;
                            _db.Entry(data).State = EntityState.Modified;
                            //Step status
                            var status = _db.tbl_PT_Status.Where(x => x.Part_Num == partnum && x.Step_Idx == 3 && x.Step_Name == "Make_CheckList").FirstOrDefault();
                            status.User_Finish = iduser;
                            status.Date_Finish = DateTime.Now;
                            status.Step_Status = true;
                            _db.SaveChanges();
                        }
                    }
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"Update Checklist Product Tranfer success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Update Checklist product Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public List<SelectListItem> GetdropdownPart()
        {

            List<SelectListItem> listvendor = _db.tbl_PT_CheckSheet.Select(x => new SelectListItem
            {
                Value = x.Part_Num.Trim(),
                Text = (x.Part_Num.Trim()),
            }).Distinct().ToList();
            return listvendor;
        }
        public List<ProductViewModel> getlistdatabypart(string part)
        {
            List<ProductViewModel> lst = (from nc in _db.tbl_PT_CheckSheet
                                          where(nc.Part_Num == part)
                                          select new ProductViewModel
                                          {
                                              Part_Num = nc.Part_Num,
                                             Item_Desc = nc.Item_Desc,
                                             Item_Value = nc.Item_Value,
                                             Is_Require = nc.Is_Require,
                                             Data_Type = nc.Data_Type,
                                             Item_Index = nc.Item_Idx
                                          }).ToList();
            return lst;
        }
        public Result SaveCopyCheckSheet(string PartNum , string PartCurrent,string iduser)
        {
            LogWriter _log = new LogWriter("SaveCopyCheckSheet");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    NumberStyles style;
                    CultureInfo culture;
                    byte index = 0;
                    style = NumberStyles.Integer;
                    culture = CultureInfo.CreateSpecificCulture("fr-FR");
                    var indexcurr = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == PartNum).OrderByDescending(x => x.Item_Idx).ToList();
                    
                    List<tbl_PT_CheckSheet> lst = new List<tbl_PT_CheckSheet>();
                    var data = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == PartNum).ToList();

                    int newindex = 0;
                    if (indexcurr.Count >0)
                    {
                        string aaa = indexcurr.FirstOrDefault().Item_Idx.ToString();
                        newindex = int.Parse(aaa) + 1;
                        
                    }
                    for (int i = 0; i < data.Count; i++)
                    {
                        string ccc = newindex.ToString();
                        index = Byte.Parse(ccc, style, culture);

                        tbl_PT_CheckSheet model = new tbl_PT_CheckSheet();
                        model.Part_Num = PartCurrent;
                        model.Item_Desc = data[i].Item_Desc;
                        model.Item_Idx = index;
                        model.Item_Remark = data[i].Item_Remark;
                        model.Is_Require = data[i].Is_Require;
                        model.Data_Type = data[i].Data_Type;

                        newindex++;
                        lst.Add(model);
                    }
                    _db.tbl_PT_CheckSheet.AddRange(lst);

                    //Update Status
                    var status = _db.tbl_PT_Status.Where(x => x.Part_Num == PartCurrent && x.Step_Idx == 3 && x.Step_Name == "Make_CheckList").FirstOrDefault();
                    status.User_Finish = iduser;
                    status.Date_Finish = DateTime.Now;
                    status.Step_Status = true;

                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"SaveCopyCheckSheet success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "SaveCopyCheckSheet Error",
                        obj = ex,
                        success = false
                    };
                }
            }
            }
        public List<SelectListItem> GetdropdownBuild()
        {

            List<SelectListItem> listvendor = _db.Families.Select(x => new SelectListItem
            {
                Value = x.Name.Trim(),
                Text = (x.Name.Trim()),
            }).Distinct().ToList();
            return listvendor;
        }
        public List<SelectListItem> GetdropdownSection()
        {

            List<SelectListItem> listvendor = _db.tbl_TP_Section.Select(x => new SelectListItem
            {
                Value = x.Section.Trim(),
                Text = (x.Section.Trim()),
            }).Distinct().ToList();
            return listvendor;
        }
        public List<SelectListItem> GetdropdownWork()
        {
            //List<SelectListItem> listvendor;
            List<SelectListItem> listvendor = _db.tbl_PT_Work_Center.Select(x => new SelectListItem
            {
                Value = x.Name.Trim(),
                Text = (x.Name.Trim()),
            }).Distinct().ToList();
            return listvendor;
        }
        public List<SelectListItem> GetListTaskProduct()
        {
            List<SelectListItem> listvendor = _db.tbl_TP_Section.Select(x => new SelectListItem
            {
                Value = x.id.ToString(),
                Text = (x.Section.Trim()),
            }).Distinct().ToList();
            return listvendor;
        }
        public List<SelectListItem> GetListUserRoleOwner(string Id)
        {
            Models.ApplicationGroup ApplicationGroup = _db.ApplicationGroups.Where(g => g.Name == "VN Owner").FirstOrDefault();
                List<string> ListUserId = ApplicationGroup.ApplicationUserGroups.Select(ap => ap.ApplicationUserId).ToList();
                IEnumerable<AspNetUser> lstUser=  _db.AspNetUsers.Where(u => ListUserId.Contains(u.Id));
            List<SelectListItem> listvendor = lstUser.Select(x => new SelectListItem
            {
                Value = x.Id.Trim(),
                Text = (x.FullName.Trim()),
            }).Distinct().ToList();
            return listvendor;
        }
        public Result UpdateConlusionProductstranfer(tbl_PT_Infor model, string type,string iduser)
        {
            LogWriter _log = new LogWriter("Update conlusion product tranfer");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var day = DateTime.Now.Day.ToString();
                    var motnh = DateTime.Now.Month.ToString();
                    var year = DateTime.Now.Year.ToString();
                    int ToppicID = 0;
                    var ToppicID1 = _db.TASKLISTs.Where(x => x.Reference == model.Part_Num).FirstOrDefault();
                        if(ToppicID1 != null)
                    {
                         ToppicID = ToppicID1.TopicID;
                    }
                    var Task = _db.TASKDETAILs.Where(x => x.TopicID == ToppicID && ((x.STATUS != "Closed" ) && (x.STATUS != "Completed"))).ToList();
                    var Stepcheck = _db.tbl_PT_Status.Where(x => x.Part_Num == model.Part_Num && x.Step_Idx == 3 && x.Date_Finish == null).FirstOrDefault();
                    var ExistCheckSheet = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == model.Part_Num).ToList();
                    tbl_PT_Infor data = _db.tbl_PT_Infor.Where(x => x.Part_Num == model.Part_Num).FirstOrDefault();
                    //update info
                    data.GM = model.GM;
                    data.Conclution = model.Conclution;
                    data.Yield = model.Yield;
                    data.FileConclusion = model.FileConclusion;
                    data.Status = "Completed";
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                    // save add status 
                    if (type == "Cancel")
                    {
                        //PT_status
                        var status = _db.tbl_PT_Status.Where(x => x.Part_Num == model.Part_Num && x.Step_Idx == 4 && x.Step_Name == "Conclusion").FirstOrDefault();
                        status.User_Finish = iduser;
                        status.Date_Finish = DateTime.Now;
                        status.Step_Status = false;
                        data.Status = "Cancel";

                        //update task cancle
                        var TaskClose = _db.TASKDETAILs.Where(x => x.TopicID == ToppicID).ToList();
                        foreach (var item in TaskClose)
                        {
                            item.STATUS = "Cancel";
                        }
                        _db.SaveChanges();
                        tranj.Commit();

                        return new Result
                        {
                            message = $@"Cancel",
                            success = true,
                        };
                }
                    else
                    {
                        if (Task.Count > 0)
                        {
                            return new Result
                            {
                                message = $@"Task is not Completed . Please completed All !",
                                success = false,
                            };
                        }
                        if (ExistCheckSheet.Count>0 && Stepcheck != null)
                        {
                            return new Result
                            {
                                message = $@"Please save update checksheet !",
                                success = false,
                            };
                        }
                        var status = _db.tbl_PT_Status.Where(x => x.Part_Num == model.Part_Num && x.Step_Idx == 4 && x.Step_Name == "Conclusion").FirstOrDefault();
                        status.User_Finish = iduser;
                        status.Date_Finish = DateTime.Now;
                        status.Step_Status = true;
                        _db.SaveChanges();
                        tranj.Commit();

                        return new Result
                        {
                            message = $@"Completed conlusion Product Tranfer success!",
                            success = true,
                        };
                    }
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Update conlusion product Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public tbl_PT_Infor GetFileWithFile(string fileId, string filename)
        {
            return _db.tbl_PT_Infor.Where(m => m.Part_Num == fileId && m.FileConclusion == filename).FirstOrDefault();
        }
        public List<string> getrole(string Iduser)
        {
            var roleid = _db.ApplicationUserGroups.Where(x => x.ApplicationUserId == Iduser).Select(x=>x.ApplicationGroupId).ToArray();
            List<string> rolename = _db.ApplicationGroups.Where(x => roleid.Contains(x.Id)).Select(x => x.Name).ToList();
            return rolename;
        }
        public bool GetStepstautusedit(string PartNum)
        {
            var edit = _db.tbl_PT_Status.Where(x => x.Part_Num == PartNum && x.Step_Idx == 1).FirstOrDefault();
            if (edit != null) {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool GetStep1stautussave(string PartNum)
        {
            var edit = _db.tbl_PT_Status.Where(x => x.Part_Num == PartNum && x.Step_Idx == 1).FirstOrDefault();
            if (edit != null && edit.Date_Finish == null)
            {
                return false;
            }
            else if( edit != null && edit.Date_Finish != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool GetStep2stautussave(string PartNum)
        {
            var edit = _db.tbl_PT_Status.Where(x => x.Part_Num == PartNum && x.Step_Idx == 2 && x.Date_Finish == null).FirstOrDefault();
            if (edit != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool GetStep3stautussave(string PartNum)
        {
            var edit = _db.tbl_PT_Status.Where(x => x.Part_Num == PartNum && x.Step_Idx == 3 && x.Date_Finish == null).FirstOrDefault();
            if (edit != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
            public bool GetStep4stautussave(string PartNum)
        {
            var edit = _db.tbl_PT_Status.Where(x => x.Part_Num == PartNum && x.Step_Idx == 4 && x.Date_Finish == null).FirstOrDefault();
            if (edit != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public Result SaveItemCheckSheet(string item, string required, string remark, string partnum, string type)
        {
            LogWriter _log = new LogWriter("SaveCopyCheckSheet");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    NumberStyles style;
                    CultureInfo culture;
                    byte index = 0;
                    style = NumberStyles.Integer;
                    culture = CultureInfo.CreateSpecificCulture("fr-FR");
                    var indexcurr = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == partnum).OrderByDescending(x => x.Item_Idx).ToList();
                    if(indexcurr.Count >0)
                    {
                        string aaa = indexcurr.FirstOrDefault().Item_Idx.ToString();
                        int bbb = int.Parse(aaa) + 1;
                       string ccc= bbb.ToString();
                        index = Byte.Parse(ccc , style, culture);
                    }
                    bool c_required = true;
                    if(required == "false")
                    {
                        c_required = false;
                    }
                    tbl_PT_CheckSheet data = new tbl_PT_CheckSheet();
                    data.Part_Num = partnum;
                    data.Item_Desc = item;
                    data.Is_Require =  c_required;
                    data.Item_Remark = remark;
                    data.Item_Idx = index ;
                    data.Data_Type = type;
                    _db.tbl_PT_CheckSheet.Add(data);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"SaveCopyCheckSheet success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "SaveCopyCheckSheet Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public Result DeletedItemCheckList(string part, string Item_Desc, Byte Index)
        {
            LogWriter _log = new LogWriter("SaveCopyCheckSheet");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                        var data = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == part && x.Item_Desc == Item_Desc.Trim() && x.Item_Idx == Index).FirstOrDefault();
                        _db.tbl_PT_CheckSheet.Remove(data);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"Delete success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Delete Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public Result EditItemCheckList(string part, string Item_Desc, Byte Index)
        {
            LogWriter _log = new LogWriter("SaveCopyCheckSheet");
            using (System.Data.Entity.DbContextTransaction tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var data = _db.tbl_PT_CheckSheet.Where(x => x.Part_Num == part && x.Item_Idx == Index).FirstOrDefault();
                    data.Item_Desc = Item_Desc;
                    _db.Entry(data).State = EntityState.Modified;
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = $@"Delete success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.ToString());
                    tranj.Rollback();
                    return new Result
                    {
                        message = "Delete Error",
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public bool IsUsercreate(string iduser ,string part)
        {
            var result = _db.tbl_PT_Infor.Where(x => x.Initail_User == iduser && x.Part_Num == part).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsUserUpdate(string iduser, string part)
        {
            var result = _db.tbl_PT_Infor.Where(x => x.PE_User == iduser && x.Part_Num == part).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsUserOwner(string iduser,string part)
        {
            var result = _db.tbl_PT_Infor.Where(x => x.Vn_Owner == iduser && x.Part_Num == part).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsUpdate(string part)
        {
            var result = _db.tbl_PT_Infor.Where(x=> x.Part_Num == part).FirstOrDefault();
            if (result != null)
            {
                if(result.Build_Loc != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }
        //public bool SaveData(List<tbl_PT_Infor> model)
        //{
        //    try
        //    {
        //        List<tbl_PT_Infor> save = new List<tbl_PT_Infor>();
        //        var part = _db.tbl_PT_Infor.Select(x=>x.Part_Num).ToList();
        //        model.Distinct();
        //        foreach (var item in model)
        //        {
        //            if (!part.Contains(item.Part_Num))
        //            {
        //                //add part vào list
        //                save.Add(item);
        //                //save status cho tung part add
        //                List<tbl_PT_Status> lst = new List<tbl_PT_Status>();
        //                tbl_PT_Status data1 = new tbl_PT_Status
        //                {
        //                    Part_Num = item.Part_Num,
        //                    Step_Idx = 1,
        //                    Step_Name = "Initial_Info",
        //                    Step_Status = false,
        //                    Date_Finish = null
        //                };
        //                lst.Add(data1);
        //                tbl_PT_Status data2 = new tbl_PT_Status();
        //                data2.Part_Num = item.Part_Num;
        //                data2.Step_Idx = 2;
        //                data2.Step_Name = "Update_Info";
        //                data2.Step_Status = false;
        //                data2.Date_Finish = null;
        //                lst.Add(data2);
        //                tbl_PT_Status data3 = new tbl_PT_Status();
        //                data3.Part_Num = item.Part_Num;
        //                data3.Step_Idx = 3;
        //                data3.Step_Name = "Make_CheckList";
        //                data3.Step_Status = false;
        //                data3.Date_Finish = null;
        //                lst.Add(data3);
        //                tbl_PT_Status data4 = new tbl_PT_Status();
        //                data4.Part_Num = item.Part_Num;
        //                data4.Step_Idx = 4;
        //                data4.Step_Name = "Conclusion";
        //                data4.Date_Finish = null;
        //                lst.Add(data4);
        //                _db.tbl_PT_Status.AddRange(lst);
        //            }
        //        }
        //        _db.tbl_PT_Infor.AddRange(save);
        //        _db.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        new LogWriter("SaveData Error").LogWrite(ex.ToString());
        //        return false;
        //    }
        //}

        public bool SaveData(List<tbl_PT_Infor> model)
        {
            try
            {
                List<tbl_PT_Infor> save = new List<tbl_PT_Infor>();
                var part = _db.tbl_PT_Infor.Select(x => x.Part_Num).ToArray();
                var lstModel = model.DistinctBy(x =>x.Part_Num).Where(x =>x.Part_Num!="").ToList();
                foreach (var item in lstModel)
                {
                    if (!part.Contains(item.Part_Num))
                    {
                        //add part vào list
                        save.Add(item);
                        //save status cho tung part add
                        List<tbl_PT_Status> lst = new List<tbl_PT_Status>();
                        tbl_PT_Status data1 = new tbl_PT_Status
                        {
                            Part_Num = item.Part_Num,
                            Step_Idx = 1,
                            Step_Name = "Initial_Info",
                            Step_Status = false,
                            Date_Finish = null
                        };
                        lst.Add(data1);
                        tbl_PT_Status data2 = new tbl_PT_Status();
                        data2.Part_Num = item.Part_Num;
                        data2.Step_Idx = 2;
                        data2.Step_Name = "Update_Info";
                        data2.Step_Status = false;
                        data2.Date_Finish = null;
                        lst.Add(data2);
                        tbl_PT_Status data3 = new tbl_PT_Status();
                        data3.Part_Num = item.Part_Num;
                        data3.Step_Idx = 3;
                        data3.Step_Name = "Make_CheckList";
                        data3.Step_Status = false;
                        data3.Date_Finish = null;
                        lst.Add(data3);
                        tbl_PT_Status data4 = new tbl_PT_Status();
                        data4.Part_Num = item.Part_Num;
                        data4.Step_Idx = 4;
                        data4.Step_Name = "Conclusion";
                        data4.Date_Finish = null;
                        lst.Add(data4);
                        _db.tbl_PT_Status.AddRange(lst);
                    }
                }
                _db.tbl_PT_Infor.AddRange(save);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                new LogWriter("SaveData Error").LogWrite(ex.ToString());
                return false;
            }
        }


        public int sentmailsubmitInitial(string iduser,string email)
        {
            int sent = _db.sp_SentMailSubmitProducTranfer(iduser, email);
            return sent;
        }

        public int sentmailsubmitvnower(string part, string email)
        {
            int sent = _db.sp_SentmailVnOwer(part, email);
            return sent;
        }
        public string getemailbyid(string id)
        {
            string email = _db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Email).FirstOrDefault();
            return email;
        }

        public string sentmailsubmitchecktask(string partnum)
        {
            var listusseraddtask = _db.tbl_PT_Dept_PIC.ToList();
           var lst =  listusseraddtask.Select(x => x.PIC).ToList();
            var listsent= lst.Distinct();
            foreach (var item in listsent)
            {
                _db.sp_Inv_SentmailSubmitTasKProductTranfer(partnum, item);
            }
            return "";

        }
    }
}
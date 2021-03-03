using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.SOReview;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Services
{
    public interface ISoReviewService
    {
        #region SOReview
        List<SelectListItem> GetDropdownlistUser();

        List<SelectListItem> GetReviewResult();

        List<sp_GetSoReview_Result> GetListSoReview();

        string GetDepart(string userID);
        List<SoReviewDetail> GetSoReviewDetail(string soNo, DateTime dateReview, string status);

        List<tbl_SOR_Attached_ForItemReview> GetListFileItem(int itemindex);

        Result UpdateDataSoReview(SoReviewDetail picData, int picID);

        Result UpdateSoReviewFinish(SoReviewDetail picData);

        Result AddTaskForItemReview(string SoNo, string Date, string itemreview, string userID,string assignee);

        Result SaveFileAttachedItemReview(tbl_SOR_Attached_ForItemReview picData);

        tbl_SOR_Attached_ForItemReview GetFileWithFileID(int fileId);

        Result DeleteDataFileofItemReview(string id);
        #endregion

        #region MasterData

        #region PIC Review

        List<PICReviewmodel> GetListPIC();
        Result SaveDataPICReview(PICReviewmodel picData);
        Result UpdateDataPICReview(PICReviewmodel picData, int picID);

        Result DeleteDataPICReview(string id);
        #endregion

        #region ItemReview
        List<ItemReviewmodel> GetListItem();
        Result SaveDataItemReview(ItemReviewmodel picData);
        Result UpdateDataItemReview(ItemReviewmodel picData, int picID);
        Result DeleteDataItemReview(string id);
        #endregion

        #region Family
        List<FamilyReviewmodel> GetListFamily();
        Result SaveDataFamilyReview(FamilyReviewmodel picData);
        Result UpdateDataFamilyReview(FamilyReviewmodel picData, int picID);

        Result DeleteDataFamilyReview(string id);
        #endregion

        #endregion

        #region Report
        List<SelectListItem> GetdropdownPart();
        List<SelectListItem> GetdropdownSoReview();
        #endregion

    }
    public class SoReviewService : ISoReviewService
    {
        private readonly IIVILocalDB _db;
        public SoReviewService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        #region SOReview
        public List<SelectListItem> GetDropdownlistUser()
        {
            List<SelectListItem> listuser = _db.AspNetUsers.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.FullName.Trim(),
            }).ToList();
            return listuser;
        }

        public List<SelectListItem> GetReviewResult()
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            SelectListItem s1 = new SelectListItem();
            s1.Value = "Y";
            s1.Text = "Y";
            lstData.Add(s1);
            SelectListItem s2 = new SelectListItem();
            s2.Value = "N";
            s2.Text = "N";
            lstData.Add(s2);
            return lstData;
        }

        public List<sp_GetSoReview_Result> GetListSoReview()
        {
            List<sp_GetSoReview_Result> data = _db.sp_GetSoReview().ToList();
            return data;
        }

        public List<SoReviewDetail> GetSoReviewDetail(string soNo, DateTime dateReview, string status)
        {
                var data = (from  aaa in _db.tbl_SOR_Cur_Review_Detail  
                        join bbb in _db.tbl_SOR_His_Review_Detail on aaa.SO_NO equals bbb.SO_NO 
                        where aaa.DOWNLOAD_DATE == bbb.DOWNLOAD_DATE && aaa.DOWNLOAD_DATE == dateReview
                        select  new SoReviewDetail
                        {
                            ID = aaa.ITEM_REVIEW_ID,
                            SONO = aaa.SO_NO,
                            ItemReview = aaa.ITEM_REVIEW,
                            DeptReview = aaa.DEPT_REVIEW,
                            Comment = aaa.COMMENT,
                            LastComment = bbb.COMMENT,
                            LastReview = bbb.RESULT,
                            ReviewResult = bbb.RESULT == true ? "True" : "Fasle",
                            IsLock = aaa.ISLOCK == true ? "True" : "Fasle"
                        }).ToList();
                return data;
        }

        public string GetDepart(string userID)
        {
            var user = _db.tbl_SOR_Review_Pic.Where(x => x.Pic_Rv == userID).FirstOrDefault();
            if (user != null) {
             return   user.Dept_Rv;
            }
            else
            {
                return "";
            }
        }

        public List<tbl_SOR_Attached_ForItemReview> GetListFileItem(int  itemidex)
        {
            // get date certification
            var data = _db.tbl_SOR_Attached_ForItemReview.Where(x => x.Item_Idx == itemidex).ToList();
            return data;
        }

        public tbl_SOR_Attached_ForItemReview GetFileWithFileID(int fileId)
        {
            return _db.tbl_SOR_Attached_ForItemReview.Where(m => m.ID == fileId).FirstOrDefault();
        }

        public Result SaveFileAttachedItemReview(tbl_SOR_Attached_ForItemReview picData)
        {
            var _log = new LogWriter("AddData");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.tbl_SOR_Attached_ForItemReview.Add(picData);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception AddData!",
                        obj = -1
                    };
                }
            }
        }

        public Result AddTaskForItemReview(string SoNo, string Date, string itemreview, string userID,string Assignee)
        {
            var _log = new LogWriter("Updatedata");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                   var currentTaskListID = 0;
                    var taskNO = SoNo +"-" + Date + "-"+ itemreview;
                    var ckTaskList = _db.TASKLISTs.FirstOrDefault(x => x.Reference.Trim().Equals(taskNO.Trim()) && x.TYPE == "SoReview");
                    if (ckTaskList == null)
                    {
                        var model = new TASKLIST
                        {
                            Topic = taskNO.Trim(),
                            TYPE = "SoReview",
                            WRITEDATE = DateTime.Now,
                            WRITTENBY = userID,
                            Level = 1,
                            Reference = taskNO
                        };
                        _db.TASKLISTs.Add(model);
                        _db.SaveChanges();
                        currentTaskListID = model.TopicID;
                        TASKDETAIL taskDetail = new TASKDETAIL
                        {
                            TopicID = currentTaskListID,
                            TASKNAME = "Task Review Item "+ itemreview,
                            DESCRIPTION = "",
                            OWNER = userID,
                            ASSIGNEE = Assignee,
                            APPROVE = userID,
                            EstimateStartDate = DateTime.Now,
                            EstimateEndDate = DateTime.Now.AddDays(7),
                            ActualStartDate = DateTime.Now,
                            ActualEndDate = DateTime.Now.AddDays(7),
                            CreatedDate = DateTime.Now,
                            STATUS = "Create",
                            Level = 1
                        };
                        _db.TASKDETAILs.Add(taskDetail);
                        _db.SaveChanges();
                           tranj.Commit();
                    }
                    return new Result
                    {
                        success = false,
                        message = "Task has created!",
                        obj = -1
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
            }
        }

        public Result DeleteDataFileofItemReview(string id)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var checkData = _db.tbl_SOR_Attached_ForItemReview.FirstOrDefault(x => x.ID.ToString().Trim() == id.Trim());
                    _db.tbl_SOR_Attached_ForItemReview.Remove(checkData);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = "Delete success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        message = ex.ToString(),
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        #endregion

        #region MasterData

        #region PIC Review

        public List<PICReviewmodel> GetListPIC()
        {
            var picData = (from tbl in _db.tbl_SOR_Review_Pic
                           join user in _db.AspNetUsers on tbl.Pic_Rv equals user.Id
                           select (new PICReviewmodel
                           {

                               ID = tbl.Pic_Inx,
                               Dept = tbl.Dept_Rv,
                               Pic = user.FullName,
                               PicID = tbl.Pic_Rv
                           })).ToList();
            return picData;
        }
        public Result SaveDataPICReview(PICReviewmodel picData)
        {
            var _log = new LogWriter("AddData");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var dataInsert = new tbl_SOR_Review_Pic();
                    dataInsert.Dept_Rv = picData.Dept;
                    dataInsert.Pic_Rv = picData.Pic;
                    _db.tbl_SOR_Review_Pic.Add(dataInsert);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception AddData!",
                        obj = -1
                    };
                }
            }
        }
        public Result UpdateSoReviewFinish(SoReviewDetail picData)
        {
            var _log = new LogWriter("Updatedata");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var data = _db.tbl_SOR_Cur_Review_Detail.Where(x => x.SO_NO == picData.SONO && x.DOWNLOAD_DATE == picData.DateDownLoad).ToList();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if(item.RESULT == null)
                            {
                                return new Result
                                {
                                    success = false,
                                    message = "Please review all item !",
                                };
                            }
                            else
                            {
                                var dataSoreview = _db.tbl_SOR_Cur_Review_List.Where(x => x.SO_NO == picData.SONO && x.DOWNLOAD_DATE == picData.DateDownLoad).FirstOrDefault();
                                dataSoreview.PLAN_SHIP_DATE = picData.PlanShipDate;
                                dataSoreview.REVIEW_STATUS = "Done";

                                //move to history ????


                                _db.SaveChanges();
                                tranj.Commit();
                                return new Result
                                {
                                    success = true,
                                };
                            }
                        }
                      
                    }
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
            }
        }
        public Result UpdateDataSoReview(SoReviewDetail picData, int picID)
        {
            var _log = new LogWriter("Updatedata");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    if(picData.IsLock == "True")
                    {
                        return new Result
                        {
                            success = false,
                            message = "Item review locked by Planer!",
                            obj = -1
                        };
                    }
                    var data = _db.tbl_SOR_Cur_Review_Detail.Where(x => x.ITEM_REVIEW_ID == picID).FirstOrDefault();
                    if(data != null)
                    {
                        bool result = false;
                        if (picData.ReviewResult == "Y")
                        {
                            result = true;
                        }
                        data.COMMENT = picData.Comment;
                        data.RESULT = result;
                        _db.SaveChanges();
                        tranj.Commit();
                        return new Result
                        {
                            success = true,
                        };
                    }
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
            }
        }
        public Result DeleteDataPICReview(string id)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var checkData = _db.tbl_SOR_Review_Pic.FirstOrDefault(x => x.Pic_Inx.ToString().Trim() == id.Trim());
                    _db.tbl_SOR_Review_Pic.Remove(checkData);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = "Delete success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        message = ex.ToString(),
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        public Result UpdateDataPICReview(PICReviewmodel picData, int picID)
        {
            var _log = new LogWriter("Updatedata");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var data = _db.tbl_SOR_Review_Pic.Where(x => x.Pic_Inx == picID).FirstOrDefault();
                    data.Dept_Rv = picData.Dept;
                    data.Pic_Rv = picData.Pic;
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
            }
        }

        #endregion

        #region Item Review
        public List<ItemReviewmodel> GetListItem()
        {
            var picData = _db.tbl_SOR_Item_Review.
                          Select(x => new ItemReviewmodel
                          {

                              ID = x.Item_Idx,
                              Dept = x.Dept_Rv,
                              ItemReview = x.Item_Rv,
                              Isdefault = x.Default == true ? "Y" : "N"
                          }).ToList();
            return picData;
        }
        public Result SaveDataItemReview(ItemReviewmodel picData)
        {
            var _log = new LogWriter("AddData");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var dataInsert = new tbl_SOR_Item_Review();
                    dataInsert.Dept_Rv = picData.Dept;
                    dataInsert.Item_Rv = picData.ItemReview;
                  //  dataInsert.Default = picData.Isdefault;
                    _db.tbl_SOR_Item_Review.Add(dataInsert);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception AddData!",
                        obj = -1
                    };
                }
            }
        }
        public Result UpdateDataItemReview(ItemReviewmodel picData, int picID)
        {
            var _log = new LogWriter("Updatedata");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var data = _db.tbl_SOR_Item_Review.Where(x => x.Item_Idx == picID).FirstOrDefault();
                    data.Dept_Rv = picData.Dept;
                    data.Item_Rv = picData.ItemReview;
                  //  data.Default = picData.Isdefault;
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
            }
        }
        public Result DeleteDataItemReview(string id)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var checkData = _db.tbl_SOR_Item_Review.FirstOrDefault(x => x.Item_Idx.ToString().Trim() == id.Trim());
                    _db.tbl_SOR_Item_Review.Remove(checkData);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = "Delete success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        message = ex.ToString(),
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        #endregion

        #region Family

        public List<FamilyReviewmodel> GetListFamily()
        {
            var picData = _db.tbl_SOR_Family_Setup_Qty.
                          Select(x => new FamilyReviewmodel
                          {
                             ID = x.Family_inx,
                            Family = x.Family,
                            MaxQty = x.Setup_Qty
                          }).ToList();
            return picData;
        }
        public Result SaveDataFamilyReview(FamilyReviewmodel picData)
        {
            var _log = new LogWriter("AddData");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var dataInsert = new tbl_SOR_Family_Setup_Qty();
                    dataInsert.Family = picData.Family;
                    dataInsert.Setup_Qty = picData.MaxQty;
                    _db.tbl_SOR_Family_Setup_Qty.Add(dataInsert);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception AddData!",
                        obj = -1
                    };
                }
            }
        }
        public Result UpdateDataFamilyReview(FamilyReviewmodel picData, int picID)
        {
            var _log = new LogWriter("Updatedata");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var data = _db.tbl_SOR_Family_Setup_Qty.Where(x => x.Family_inx == picID).FirstOrDefault();
                    data.Family = picData.Family;
                    data.Setup_Qty = picData.MaxQty;
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception Updatedata!",
                        obj = -1
                    };
                }
            }
        }
        public Result DeleteDataFamilyReview(string id)
        {
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var checkData = _db.tbl_SOR_Family_Setup_Qty.FirstOrDefault(x => x.Family.Trim() == id.Trim());
                    _db.tbl_SOR_Family_Setup_Qty.Remove(checkData);
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        message = "Delete success!",
                        success = true,
                    };
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        message = ex.ToString(),
                        obj = ex,
                        success = false
                    };
                }
            }
        }
        #endregion

        #endregion

        #region Report
        public List<SelectListItem> GetdropdownPart()
        {
            List<SelectListItem> listpart = _db.tbl_SOR_Cur_Review_List.Select(x => new SelectListItem
            {
                Value = x.ANALYST,
                Text = x.ANALYST
            }).ToList();
            return listpart;
        }
        public List<SelectListItem> GetdropdownSoReview()
        {
            List<SelectListItem> lstSo = _db.tbl_SOR_Cur_Review_List.Select(x => new SelectListItem
            {
                Value = x.SO_NO,
                Text = x.SO_NO
            }).ToList();
            return lstSo;
        }

        #endregion
    }
}
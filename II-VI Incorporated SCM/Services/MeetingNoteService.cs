using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.MeetingNote;
using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IMeetingNoteService
    {
        List<MeetingNoteViewmodel> getlistdata();
        List<SelectListItem> selected(string meetingnum);
        MEETING_ATT GetFileWithFileMeetingNum(string fileId, string filename);
        List<SelectListItem> GetDropdownlistUser();
        string GetAutoMeetingNUM();
        List<MEETING_ATT> GetUploadedEvidence(string meetingnum);
        MeetingNoteViewmodel getMeetingbyNUm(string meetingnum);
        Result SaveMeetingNote(MeetingNoteViewmodel meetingNoteModel, string iduser, List<MEETING_ATT> nCR_EVIs,string meetingnumauto);
        Result EDITMeetingNote(MeetingNoteViewmodel meetingNoteModel, string iduser, List<MEETING_ATT> nCR_EVIs, string meetingnumauto);
        bool UpdateStatusMeeting(string Meetingnote);
        bool CheckOnwerCreate(string id);
    }
    public class MeetingNoteService : IMeetingNoteService
    {
        private readonly IIVILocalDB _db;
        public MeetingNoteService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }
        public List<MeetingNoteViewmodel> getlistdata()
        {
            var listmeeting = (from nc in _db.MEETING_NOTE
                               join asp in _db.AspNetUsers on nc.CREATED_BY equals asp.Id
                               into joined
                               from j in joined.DefaultIfEmpty()
                               select new MeetingNoteViewmodel
                               {
                                   MINUTES_NUM = nc.MINUTES_NUM,
                                   CREATED_BY=j.FullName,
                                   CREATED_DATE = nc.CREATED_DATE,
                                   STATUS = nc.STATUS,
                                   SUBJECT = nc.SUBJECT,
                                   MINUTES_CONTENT = nc.MINUTES_CONTENT,
                                   MEETING_DATE = nc.MEETING_DATE
                               }).ToList();
            return listmeeting;
                          }
        public MeetingNoteViewmodel getMeetingbyNUm(string meetingnum)
        {
            var result = (from meeting in _db.MEETING_NOTE 
                          join att in _db.MEETING_ATTENDANT on meeting.MINUTES_NUM equals att.MINUTES_NUM
                          join asp in _db.AspNetUsers on meeting.CREATED_BY equals asp.Id
                              into joined
                          from j in joined.DefaultIfEmpty()
                          where(meeting.MINUTES_NUM == meetingnum)
                          select new MeetingNoteViewmodel
                          {
                              MINUTES_NUM = meeting.MINUTES_NUM,
                              CREATED_BY = j.FullName,
                              CREATED_DATE = meeting.CREATED_DATE,
                              STATUS = meeting.STATUS,
                              SUBJECT = meeting.SUBJECT,
                              MINUTES_CONTENT = meeting.MINUTES_CONTENT,
                              MEETING_DATE = meeting.MEETING_DATE,
                              
                          }).FirstOrDefault();
            return result;
        }
        public List<MEETING_ATT> GetUploadedEvidence(string meetingnum)
        {
            var lstEvidence = _db.MEETING_ATT.Where(x => x.MINUTES_NUM.Equals(meetingnum)).ToList();
            return lstEvidence;
        }
             public MEETING_ATT GetFileWithFileMeetingNum(string fileId,string filename)
        {
            return _db.MEETING_ATT.Where(m => m.MINUTES_NUM == fileId && m.FILE_NAME == filename).FirstOrDefault();
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
        public Result SaveMeetingNote(MeetingNoteViewmodel meetingNoteModel,string iduser, List<MEETING_ATT> nCR_EVIs,string meetingnumauto)
        {
            var _log = new LogWriter("AddMeeting");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    MEETING_NOTE createMeetingModel = new MEETING_NOTE();
                    createMeetingModel.MINUTES_NUM = meetingnumauto;
                    createMeetingModel.MINUTES_CONTENT = meetingNoteModel.MINUTES_CONTENT;
                    createMeetingModel.SUBJECT = meetingNoteModel.SUBJECT;
                    createMeetingModel.STATUS = "Created";
                    createMeetingModel.CREATED_DATE = DateTime.Today;
                    createMeetingModel.CREATED_BY = iduser;
                    createMeetingModel.MEETING_DATE = meetingNoteModel.MEETING_DATE;
                    _db.MEETING_NOTE.Add(createMeetingModel);
                   
                    //Meeting attendant
                    List<MEETING_ATTENDANT> ateendent = new List<MEETING_ATTENDANT>();

                    foreach (var item in meetingNoteModel.ATTENDANT)
                    {
                        MEETING_ATTENDANT att = new MEETING_ATTENDANT();
                        att.ATTENDANT = item;
                        var user = _db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                        att.ATTEND_NAME = user.FullName;
                        att.MINUTES_NUM = meetingnumauto;
                        att.EMAIL = user.Email;
                        ateendent.Add(att);
                    }
                    _db.MEETING_ATTENDANT.AddRange(ateendent);
                    //meeting attach
                    if (nCR_EVIs.Count > 0)
                    {
                        foreach (var evi in nCR_EVIs)
                        {
                            _db.MEETING_ATT.Add(evi);
                        }
                    }
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        // obj = newApprover.ID
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception UpdateUserApproval!",
                        obj = -1
                    };
                }
            }
        }
        public Result EDITMeetingNote(MeetingNoteViewmodel meetingNoteModel, string iduser, List<MEETING_ATT> nCR_EVIs, string meetingnumauto)
        {
            var _log = new LogWriter("AddMeeting");
            using (var tranj = _db.Database.BeginTransaction())
            {
                try
                {
                    var createMeetingModel = _db.MEETING_NOTE.Where(x => x.MINUTES_NUM == meetingnumauto).FirstOrDefault();
                    createMeetingModel.MINUTES_CONTENT = meetingNoteModel.MINUTES_CONTENT;
                    createMeetingModel.MINUTES_NUM = meetingnumauto;
                    createMeetingModel.SUBJECT = meetingNoteModel.SUBJECT;
                    createMeetingModel.MEETING_DATE = meetingNoteModel.MEETING_DATE;
                    createMeetingModel.CREATED_BY = iduser;
                    createMeetingModel.STATUS = "Created";
                    _db.Entry(createMeetingModel).State = EntityState.Modified;
                    //remove list atendant cu 
                    var delete = _db.MEETING_ATTENDANT.Where(x => x.MINUTES_NUM == meetingnumauto).ToList();
                    _db.MEETING_ATTENDANT.RemoveRange(delete);
                    // add Meeting attendant
                    List<MEETING_ATTENDANT> ateendent = new List<MEETING_ATTENDANT>();

                    foreach (var item in meetingNoteModel.ATTENDANT)
                    {
                        MEETING_ATTENDANT att = new MEETING_ATTENDANT();
                        att.ATTENDANT = item;
                        var user = _db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                        att.ATTEND_NAME = user.FullName;
                        att.MINUTES_NUM = meetingnumauto;
                        att.EMAIL = user.Email;
                        ateendent.Add(att);
                    }
                    _db.MEETING_ATTENDANT.AddRange(ateendent);
                    //meeting attach remove 
                    if (meetingNoteModel.FILE_NAME != null)
                    {
                        var EVIIDDelete = meetingNoteModel.FILE_NAME.ToArray();
                        var EVIDelete = _db.MEETING_ATT.Where(x => !EVIIDDelete.Contains(x.FILE_NAME) & x.MINUTES_NUM.Equals(meetingnumauto)).ToList();
                        if (EVIDelete.Count > 0) _db.MEETING_ATT.RemoveRange(EVIDelete);
                    }
                    else
                    {
                        var EVIDelete = _db.MEETING_ATT.Where(x => x.MINUTES_NUM.Equals(meetingnumauto)).ToList();
                        if (EVIDelete.Count > 0) _db.MEETING_ATT.RemoveRange(EVIDelete);
                    }
                    //meeting attach add
                    if (nCR_EVIs.Count > 0)
                    {
                        foreach (var evi in nCR_EVIs)
                        {
                            _db.MEETING_ATT.Add(evi);
                        }
                    }
                    _db.SaveChanges();
                    tranj.Commit();
                    return new Result
                    {
                        success = true,
                        // obj = newApprover.ID
                    };
                }
                catch (Exception ex)
                {
                    tranj.Rollback();
                    _log.LogWrite(ex.ToString());
                    return new Result
                    {
                        success = false,
                        message = "Exception UpdateUserApproval!",
                        obj = -1
                    };
                }
            }
        }
        public bool UpdateStatusMeeting(string Meetingnote)
        {
            var result = _db.MEETING_NOTE.Where(x => x.MINUTES_NUM == Meetingnote).FirstOrDefault();
            if(result != null) { 
            result.STATUS = "Published";
            _db.Entry(result).State = EntityState.Modified;
            _db.SaveChanges();
             var sentmail = _db.sp_SentMailMeetingPublish(Meetingnote);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckOnwerCreate(string id)
        {
            var resul = _db.MEETING_NOTE.Where(x => x.CREATED_BY == id).FirstOrDefault();
            if(resul != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string[] ArrayChar =
         {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
            "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };
        public bool CheckExistMINUTENUM(string ncrNum)
        {
            return _db.MEETING_NOTE.Where(x => x.MINUTES_NUM == ncrNum).FirstOrDefault() == null ? false : true;
        }
        public string GetAutoMeetingNUM()
        {
            var MM = DateTime.Today.ToString("yyMM");
            string crNum = "MN" +MM+ "0001";
            string char2 = "", char3 = "", char4 = "", char5 = "";
            while (CheckExistMINUTENUM(crNum))
            {
                if (crNum != "")
                {
                    char[] array = crNum.ToCharArray();
                    char2 = array[6].ToString();
                    char3 = array[7].ToString();
                    char4 = array[8].ToString();
                    char5 = array[9].ToString();
                   // char6 = array[6].ToString();
                  // char7 = array[7].ToString();
                            if (Array.IndexOf(ArrayChar, array[9].ToString()) == 35)
                            {
                                char5 = "0";
                                if (Array.IndexOf(ArrayChar, array[8].ToString()) == 35)
                                {
                                    char4 = "0";
                                    if (Array.IndexOf(ArrayChar, array[7].ToString()) == 35)
                                    {
                                        char3 = "0";
                                        if (Array.IndexOf(ArrayChar, array[6].ToString()) == 35)
                                        {
                                            char2 = "";
                                    char3 = "";
                                    char4 = "";
                                    char5 = "";
                                    break;
                                        }
                                        else
                                        {
                                            char2 = ArrayChar[Array.IndexOf(ArrayChar, array[6].ToString()) + 1];
                                        }
                                    }
                                    else
                                    {
                                        char3 = ArrayChar[Array.IndexOf(ArrayChar, array[7].ToString()) + 1];
                                    }
                                }
                                else
                                {
                                    char4 = ArrayChar[Array.IndexOf(ArrayChar, array[8].ToString()) + 1];
                                }
                            }
                            else
                            {
                                char5 = ArrayChar[Array.IndexOf(ArrayChar, array[9].ToString()) + 1];
                            }
                        }
                crNum = "MN" + MM + char2 + char3 + char4 + char5;
            }
            return crNum;
        }
        public List<SelectListItem> selected(string meetingnum)
        {

            var lstselected = _db.MEETING_ATTENDANT.Where(x => x.MINUTES_NUM == meetingnum).ToList();

            List<SelectListItem> listResult = new List<SelectListItem>();
            //if (!lstdefectncr.Contains(item1.Trim()))
            //{

            //}
            listResult = lstselected.Select(x => new SelectListItem
            {
                Value =x.ATTENDANT,
                Text = x.ATTEND_NAME,
                Selected = true
            }).ToList();
            return listResult;
        }
    }
}
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IEmailService
    {
        void TestSentEmail();
        void TestSentEmail(string content);
        void SendEmailForgotPassword(string email, string link);
        void SendEmailSubmitWithoutMeeting(string email, string message);
        void SendEmailSubmitChangeItemToChairman(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR);
        bool SentMailRemind(string ncrnum, string url, string path, string linkNCR, string comment);
        bool SentMailRemindSQE(string ncrnum, string url, string path, string linkNCR, string comment);
        void SendEmailApprovalChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR);
        void SendEmailRejectChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR, string comment);
        void SendEmailAcknowChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR);
        void SendEmailAcknowapprovalChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR, string form);
        void SendEmailCreateNCR(string mailTemplate, string mailPath, string RecipientName, string email, string NCRnumber, string linkNCR, string comment);
        
        void SendEmailReAssignNCR(string mailTemplate, string mailPath, string RecipientName, string email, string NCRnumber, string linkNCR, string comment, string reason,string user);
        void SendEmailDispositionApproval(string mailTemplate, string mailPath, string RecipientName, string email, string NCRnumber, string linkNCR, string comment);
        void SendEmailDispositionApproval(string mailTemplate, string mailPath, string NCRNUM);
        Result SendEmailFromConfirmNCR(string ncrnum, string path, string tname, string linkNCR);
        void SendEmailDispoitionToChairman(string nCRNUM, string linkNCR, List<AspNetUser> chairmans, string path);
        void SendEmailToConfirmNCR(string ncrnum, string linkNCR, string path, string mailTemplate);
        void SendEmailRejectNCR(string mailTemplate, string mailPath, string NCRnumber, string linkNCR, string comment, string reason,string user, List<AspNetUser> eNGs, AspNetUser oPE, List<AspNetUser> approvers);
        void SendEmailConfirmAccount(string callbackUrl, string Email, string Fullname);
        void SendEmailAutoAssignTask(string mailTemplate, string mailPath, string NCRNUM);
        void SendEmailCompletedTask(string mailTemplate, string mailPath, string RecipientName, string email, string TaskID, string linkTask, string TaskName, string comment,string user);
        void SendEmailCompletedTaskCreated(string mailTemplate, string mailPath, string Owner, string RecipientName, string email, string TaskID, string linkNCR, string TaskName, string comment, string user);

    }
    public class EmailService : IEmailService
    {
        private IIVILocalDB _db;
        public EmailService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public bool SentMailRemind(string ncrnum, string url, string path, string linkNCR, string comment)
        {
            try
            {
                var arrRoleID = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.SQE).Id;
                var arrUserId = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(ncrnum) & x.isActive == true & x.RoleId != arrRoleID).Select(x => x.UserId).ToArray();

                var arrMail = _db.AspNetUsers.Where(x => arrUserId.Contains(x.Id)).ToList();
                //string Url = ConfigurationManager.AppSettings["URLSentMail"];
                foreach (var mail in arrMail)
                {
                    SendTemplate("2", path, "", mail.FullName, mail.Email, ncrnum, linkNCR, comment, "");
                }

                return true;
            }
            catch (Exception ex)
            {
                var _log = new LogWriter("EmailService - SentMailRemind" + ex.ToString());
                _log.LogWrite(ex.InnerException.Message);
                return false;
            }
        }

        public bool SentMailRemindSQE(string ncrnum, string url, string path, string linkNCR, string comment)
        { //get email
            try
            {
                var RoleSQEID = _db.ApplicationGroups.FirstOrDefault(n => n.Name == UserGroup.SQE).Id;
                var arrUserID = _db.APPROVALs.Where(x => x.RoleId == RoleSQEID & x.NCR_NUMBER == ncrnum.Trim() & x.isActive == true).Select(x => x.UserId).ToArray();
                var Users = _db.AspNetUsers.Where(x => arrUserID.Contains(x.Id)).ToList();
                foreach (var user in Users)
                {
                    SendTemplate("2", path, "", user.FullName, user.Email, ncrnum, linkNCR, comment, "");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void TestSentEmail()
        {
            _db.spSendEmail("IQC", "tung.vu@thlsoft.com", null, "Hello Mr.Tung Nui", null, "Test Email", null);
        }

        public void TestSentEmail(string content)
        {
            _db.spSendEmail("IQC", "trung.vu@thlsoft.com", null, content, null, "Test Email", null);
        }

        public void SentEmailSubmitEdit(string id)
        {
            _db.spSendEmail("IQC", id, null, "Hello Mr.Tung Nui", null, "Test Email", null);
        }

        public void SendEmailForgotPassword(string email, string link)
        {
            _db.spSendEmail("IQC", email, null, link, "HTML", "Reset Password", null);
        }
        public void SendEmailSubmitWithoutMeeting(string email, string message)
        {
            _db.spSendEmail("IQC", email, null, message, "HTML", "Submit NCR", null);
        }

        public void SendEmailSubmitChangeItemToChairman(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR)
        {
            SendTemplate("7", pathTemplate, subject, recipientName, emailAddress, NCRnum, linkNCR, "", "");
        }
        public void SendEmailRejectChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR,string comment)
        {
            SendTemplate("10", pathTemplate, subject, recipientName, emailAddress, NCRnum, linkNCR, "",
                comment);
        }
        public void SendEmailAcknowChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR)
        {
            SendTemplate("9", pathTemplate, subject, recipientName, emailAddress, NCRnum, linkNCR, "", "");
        }
        public void SendEmailAcknowapprovalChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR,string form)
        {
            SendTemplate("9", pathTemplate, subject, recipientName, emailAddress, NCRnum, linkNCR, "", "",form);
        }
        public void SendEmailApprovalChangeItem(string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR)
        {
            SendTemplate("8", pathTemplate, subject, recipientName, emailAddress, NCRnum, linkNCR, "", "");
        }
        private void SendTemplate(string templateName, string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR, string comment = "", string reason = "", string user = "", string form = "")
        {
            var _log = new LogWriter("EmailService - SendTemplate");
            try
            {
                if (comment != "")
                {
                    comment = "Be noted: " + comment;
                    user = "Re-assign by:" + user;
                }
                var email = new Templates().GetEmailByMailName(templateName, pathTemplate);

                email.MailAddress = emailAddress;

                email.Subject = email.Subject.Replace(Email.Keys.StringNCRNUM, NCRnum);

                string bodytmp = email.Body;
                bodytmp = bodytmp.Replace(Email.Keys.StringNCRNUM, NCRnum);
                bodytmp = bodytmp.Replace(Email.Keys.StringRecipientName, recipientName);
                bodytmp = bodytmp.Replace(Email.Keys.StringComment, comment);
                bodytmp = bodytmp.Replace(Email.Keys.StringReason, reason);
                bodytmp = bodytmp.Replace(Email.Keys.StringUser, user);
                bodytmp = bodytmp.Replace(Email.Keys.StringForm, form);
                if (bodytmp.IndexOf(Email.Keys.LinkNCRNUM, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    string a = bodytmp.Substring(bodytmp.IndexOf(Email.Keys.LinkNCRNUM)).Replace(Email.Keys.LinkNCRNUM, string.Empty);
                    string aText = a.Substring(1, a.IndexOf("@@")).Replace("@", "");
                    string key = $"{Email.Keys.LinkNCRNUM}|{aText}@@";
                    string link = $@"<a href='{linkNCR}' >{aText}</a>";
                    bodytmp = bodytmp.Replace(key, link);
                }
                bodytmp = bodytmp.Replace("\n", "<br />");

                _db.spSendEmail(email.ProfileName, emailAddress, null, bodytmp, email.BodyFormat, email.Subject, null);
                _db.MAILHISTORies.Add(new MAILHISTORY
                {
                    Content = bodytmp,
                    CreateDate = DateTime.Now,
                    Subject = email.Subject,
                    To = emailAddress
                });
                _db.SaveChanges();
            }
            catch(Exception ex)
            {
                _log.LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    var e = (DbEntityValidationException)ex;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
            }
        }
        private void SendTemplateTask(string templateName, string pathTemplate, string subject, string recipientName, string emailAddress, string NCRnum, string linkNCR, string TaskName, string comment = "", string reason = "", string user = "", string form = "")
        {
            var _log = new LogWriter("EmailService - SendTemplate");
            try
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    comment = "Be noted: " + comment;
                    user = "Reject by:" + user;
                }
                var email = new Templates().GetEmailByMailName(templateName, pathTemplate);

                email.MailAddress = emailAddress;

                email.Subject = email.Subject.Replace(Email.Keys.StringNCRNUM, TaskName);

                string bodytmp = email.Body;
                bodytmp = bodytmp.Replace(Email.Keys.StringNCRNUM, TaskName);
                //bodytmp = bodytmp.Replace(Email.Keys.StringNCRNUM, );
                bodytmp = bodytmp.Replace(Email.Keys.StringRecipientName, recipientName);
                bodytmp = bodytmp.Replace(Email.Keys.StringComment, comment);
                bodytmp = bodytmp.Replace(Email.Keys.StringReason, reason);
                bodytmp = bodytmp.Replace(Email.Keys.StringUser, user);
                bodytmp = bodytmp.Replace(Email.Keys.StringForm, form);
                if (bodytmp.IndexOf(Email.Keys.LinkNCRNUM, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    string a = bodytmp.Substring(bodytmp.IndexOf(Email.Keys.LinkNCRNUM)).Replace(Email.Keys.LinkNCRNUM, string.Empty);
                    string aText = a.Substring(1, a.IndexOf("@@")).Replace("@", "");
                    string key = $"{Email.Keys.LinkNCRNUM}|{aText}@@";
                    string link = $@"<a href='{linkNCR}' >{aText}</a>";
                    bodytmp = bodytmp.Replace(key, link);
                }
                bodytmp = bodytmp.Replace("\n", "<br />");

                _db.spSendEmail(email.ProfileName, emailAddress, null, bodytmp, email.BodyFormat, email.Subject, null);
                _db.MAILHISTORies.Add(new MAILHISTORY
                {
                    Content = bodytmp,
                    CreateDate = DateTime.Now,
                    Subject = email.Subject,
                    To = emailAddress
                });
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    var e = (DbEntityValidationException)ex;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
            }
        }
        private void SendTemplateTaskCreated(string templateName, string pathTemplate, string subject,string Owner, string recipientName, string emailAddress, string NCRnum, string linkNCR, string TaskName, string comment = "", string reason = "", string user = "", string form = "")
        {
            var _log = new LogWriter("EmailService - SendTemplate");
            try
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    comment = "Be noted: " + comment;
                    user = "Reject by:" + user;
                }
                var email = new Templates().GetEmailByMailName(templateName, pathTemplate);

                email.MailAddress = emailAddress;

                email.Subject = email.Subject.Replace(Email.Keys.StringNCRNUM, TaskName);

                string bodytmp = email.Body;
                bodytmp = bodytmp.Replace(Email.Keys.StringNCRNUM, TaskName);
                //bodytmp = bodytmp.Replace(Email.Keys.StringNCRNUM, );
                bodytmp = bodytmp.Replace(Email.Keys.StringRecipientName, recipientName);
                bodytmp = bodytmp.Replace(Email.Keys.StringComment, comment);
                bodytmp = bodytmp.Replace(Email.Keys.StringReason, reason);
                bodytmp = bodytmp.Replace(Email.Keys.StringUser, user);
                bodytmp = bodytmp.Replace(Email.Keys.StringForm, form);
                if (bodytmp.IndexOf(Email.Keys.LinkNCRNUM, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    string a = bodytmp.Substring(bodytmp.IndexOf(Email.Keys.LinkNCRNUM)).Replace(Email.Keys.LinkNCRNUM, string.Empty);
                    string aText = a.Substring(1, a.IndexOf("@@")).Replace("@", "");
                    string key = $"{Email.Keys.LinkNCRNUM}|{aText}@@";
                    string link = $@"<a href='{linkNCR}' >{aText}</a>";
                    bodytmp = bodytmp.Replace(key, link);
                }
                bodytmp = bodytmp.Replace("\n", "<br />");

                _db.spSendEmail(email.ProfileName, emailAddress,Owner, bodytmp, email.BodyFormat, email.Subject, null);
                _db.MAILHISTORies.Add(new MAILHISTORY
                {
                    Content = bodytmp,
                    CreateDate = DateTime.Now,
                    Subject = email.Subject,
                    To = emailAddress
                });
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    var e = (DbEntityValidationException)ex;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
            }
        }
        private string GetTemplate()
        {

            return "";
        }

        public void SendEmailCreateNCR(string mailTemplate, string mailPath, string RecipientName, string email, string NCRnumber, string linkNCR, string comment)
        {
            SendTemplate(mailTemplate, mailPath, "", RecipientName, email, NCRnumber, linkNCR, comment);
        }

        public void SendEmailReAssignNCR(string mailTemplate, string mailPath, string RecipientName, string email, string NCRnumber, string linkNCR, string comment, string reason,string user)
        {
            SendTemplate(mailTemplate, mailPath, "", RecipientName, email, NCRnumber, linkNCR, comment, reason,user);
        }

        public void SendEmailDispositionApproval(string mailTemplate, string mailPath, string RecipientName, string email, string NCRnumber, string linkNCR, string comment)
        {
            SendTemplate(mailTemplate, mailPath, "", RecipientName, email, NCRnumber, linkNCR, comment);
        }
        public void SendEmailCompletedTask(string mailTemplate, string mailPath, string RecipientName, string email, string TaskID, string linkNCR, string TaskName, string comment,string user)
        {
            string reason = "";
            SendTemplateTask(mailTemplate, mailPath, "", RecipientName, email, TaskID, linkNCR, TaskName, comment,reason,user);
        }
        public void SendEmailCompletedTaskCreated(string mailTemplate, string mailPath,string Owner ,string RecipientName, string email, string TaskID, string linkNCR, string TaskName, string comment, string user)
        {
            string reason = "";
            SendTemplateTaskCreated(mailTemplate, mailPath,"", Owner, RecipientName, email, TaskID, linkNCR, TaskName, comment, reason, user);
        }
        public void SendEmailDispositionApproval(string mailTemplate, string mailPath, string NCRNUM)
        {
            string EmailConfigSCAR = ConfigurationManager.AppSettings["requiredSCAR"];

            string _mailTemplate = EmailConfigSCAR.Split('|')[2];
            var NCR = _db.NCR_HDR.FirstOrDefault(x=>x.NCR_NUM.Equals(NCRNUM));
            var checkIsVendor = _db.NCR_DET.Where(x => x.RESPONSE.Trim() == CONFIRMITY_RESPON.ID_VENDOR & x.NCR_NUM.Equals(NCRNUM)).ToList();

            var QUALITYASSURANCEID = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.QUALITYASSURANCE);
            //var SQEID = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.SQE);

            //var ApproverIDsSQE = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(NCRNUM) & x.isActive == true & x.RoleId == SQEID.Id).Select(x=>x.UserId).ToArray();
            var ApproverIDsQUALITY = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(NCRNUM) & x.isActive == true & x.RoleId == QUALITYASSURANCEID.Id).Select(x => x.UserId).ToArray();


            if (NCR.REQUIRED.Value == true & checkIsVendor.Count > 0)
            {
                var UserMail = _db.AspNetUsers.Where(x => ApproverIDsQUALITY.Contains(x.Id)).ToList();
                foreach (var mail in UserMail)
                {
                    SendTemplate(_mailTemplate, mailPath, "", mail.FullName, mail.Email, NCRNUM, mailTemplate, NCR.Comment);
                }
            }

        }

        public Result SendEmailFromConfirmNCR(string ncrnum, string path, string tname, string linkNCR)
        {
            var _log = new LogWriter("SendEmailFromConfirmNCR");
          //  var ENG = _db.ApplicationGroups.FirstOrDefault(x=>x.Name == UserGroup.ENGINEERING);
            try
            {
                _log.LogWrite("Get NCR " + ncrnum);
                var ApproverIds = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(ncrnum.Trim()) && x.isActive == true).Select(x=>x.UserId).ToArray();
                var Approvers =_db.AspNetUsers.Where(x => ApproverIds.Contains(x.Id)).ToList();
                var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Equals(ncrnum));
                foreach (var app in Approvers)
                {
                    SendTemplate(tname, path, "", app.FullName, app.Email, ncrnum, linkNCR, comment: NCR.Comment);
                }

                return new Result
                {
                    success = true,
                    message = "Send email successful",
                };
            }
            catch(Exception ex)
            {
                _log.LogWrite(ex.ToString() + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return new Result
                {
                    success = false,
                    message = ex.Message
                };
            }
        }

        public void SendEmailDispoitionToChairman(string nCRNUM, string linkNCR, List<AspNetUser> chairmans, string path)
        {
            var _log = new LogWriter("SendEmailFromConfirmNCR");
            try
            {
                string EmailConfigSCAR = ConfigurationManager.AppSettings["c"];
                foreach (var u in chairmans)
                {
                    SendTemplate(EmailConfigSCAR.Split('|')[2], path, "", u.FullName, u.Email, nCRNUM, linkNCR);
                }
            }
            catch(Exception ex)
            {
                _log.LogWrite(ex.ToString() + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public void SendEmailToConfirmNCR(string ncrnum, string linkNCR, string path, string mailTemplate)
        {
            var _log = new LogWriter("SendEmailToConfirmNCR");
            var IDMRBWH = _db.ApplicationGroups.FirstOrDefault(x=>x.Name == UserGroup.WHMRB);
            var userTMP = _db.ApplicationUserGroups.Where(x=>x.ApplicationGroupId == IDMRBWH.Id).ToList();
            var arrID = userTMP.Select(x => x.ApplicationUserId).ToArray();
            var Users = _db.AspNetUsers.Where(x => arrID.Contains(x.Id)).ToList();
            var NCR = _db.NCR_HDR.FirstOrDefault(x=>x.NCR_NUM.Trim().Equals(ncrnum.Trim()));
            
            try
            {
                foreach (var User in Users)
                {
                    SendTemplate(mailTemplate, path, "", User.FullName, User.Email, ncrnum, linkNCR, NCR.Comment);
                }
            }
            catch (Exception ex)
            {
                _log.LogWrite(ex.ToString() + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public void SendEmailRejectNCR(string mailTemplate, string mailPath, string NCRnumber, string linkNCR, string comment, string reason,string user, List<AspNetUser> eNGs, AspNetUser oPE, List<AspNetUser> approvers)
        {
            SendTemplate(mailTemplate, mailPath, "", oPE.FullName, oPE.Email, NCRnumber, linkNCR, comment, reason,user);

            foreach (var eNG in eNGs)
            {
                SendTemplate("5", mailPath, "", eNG.FullName, eNG.Email, NCRnumber, linkNCR, comment, reason,user);
            }

            foreach (var approver in approvers)
            {
                SendTemplate(mailTemplate, mailPath, "", approver.FullName, approver.Email, NCRnumber, linkNCR, comment, reason,user);
            }
        }

        public void SendEmailConfirmAccount(string callbackUrl, string Email, string Fullname)
        {
            int i = _db.spSendEmail("IQC", Email, null, callbackUrl, "HTML", "Confirm Account", null);
            if (i == 1)
            {
                _db.MAILHISTORies.Add(new MAILHISTORY
                {
                    Content = callbackUrl,
                    CreateDate = DateTime.Now,
                    Subject = "Confirm Account " + Email,
                    To = Email
                });
                _db.SaveChanges();
            }
        }

        public void SendEmailAutoAssignTask(string mailTemplate, string mailPath, string NCRNUM)
        {
            var NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(NCRNUM.Trim()));

            var DETs = _db.NCR_DET.Where(x => x.NCR_NUM.Trim().Equals(NCRNUM.Trim())).ToList();

            string GroupMRBWHID = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.WHMRB)).Id;
            string GroupSQEID = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.SQE)).Id;

            string EmailCreateConfigSCAR = ConfigurationManager.AppSettings["requiredSCAR"];
            string EmailCreateConfigNotify = ConfigurationManager.AppSettings["requiredNotify"];

            string MailTemplate = EmailCreateConfigSCAR.Split('|')[2];
            var Users = _db.AspNetUsers.ToList();

            var uWH = Users.FirstOrDefault(x => x.Id.Equals(NCR.UserConfirm));

            var Check = DETs.FirstOrDefault(x => x.RESPONSE.Trim() == CONFIRMITY_RESPON.ID_VENDOR);
            //var SQEAssignees = _db.ApplicationUserGroups.Where(x => x.ApplicationGroupId.Equals(GroupSQEID)).ToList();
            if ((NCR.REQUIRED.Value == true & Check != null))
            {
                SendTemplate(MailTemplate, mailPath, "", uWH.FullName, uWH.Email, NCRNUM, mailTemplate, NCR.Comment);

                var SQEAssignees = _db.APPROVALs.Where(x => x.RoleId.Equals(GroupSQEID) & x.NCR_NUMBER.Trim().Equals(NCRNUM.Trim()) & x.isActive == true).ToList();
                foreach (var Assignee in SQEAssignees)
                {
                    var uSQE = Users.FirstOrDefault(x => x.Id.Equals(Assignee.UserId));
                    SendTemplate(MailTemplate, mailPath, "", uSQE.FullName, uSQE.Email, NCRNUM, mailTemplate, NCR.Comment);
                }
            }
            else if((NCR.NOTIFICATION_ONLY.Value == true & Check != null))
            {
                MailTemplate = EmailCreateConfigNotify.Split('|')[2];
                SendTemplate(MailTemplate, mailPath, "", uWH.FullName, uWH.Email, NCRNUM, mailTemplate, NCR.Comment);

                var SQEAssignees = _db.APPROVALs.Where(x => x.RoleId.Equals(GroupSQEID) & x.NCR_NUMBER.Trim().Equals(NCRNUM.Trim()) & x.isActive == true).ToList();
                foreach (var Assignee in SQEAssignees)
                {
                    var uSQE = Users.FirstOrDefault(x => x.Id.Equals(Assignee.UserId));
                    SendTemplate(MailTemplate, mailPath, "", uSQE.FullName, uSQE.Email, NCRNUM, mailTemplate, NCR.Comment);
                }
            }

            #region
            //if (NCR.REQUIRED.Value == true)
            //{

            //    if (Check != null)
            //    {

            //        foreach (var Assignee in SQEAssignees)
            //        {
            //            //_db.TASKDETAILs.Add(new TASKDETAIL
            //            //{
            //            //    TASKID = currentTaskListID,
            //            //    TASKNAME = "Create SCAR",
            //            //    OWNER = NCR.UserConfirm,
            //            //    ASSIGNEE = Assignee.UserId,
            //            //    STATUS = "New"
            //            //});

            //            var uSQE = Users.FirstOrDefault(x => x.Id.Equals(Assignee.UserId));
            //            SendTemplate(MailTemplate, mailPath, "", uSQE.FullName, uSQE.Email, NCRNUM, mailTemplate, NCR.Comment);
            //        }
            //    }
            //    else
            //    {

            //        //2
            //        //_db.TASKDETAILs.Add(new TASKDETAIL
            //        //{
            //        //    //TASKID = currentTaskListID,
            //        //    //TASKNAME = "Create internal CAR",
            //        //    //OWNER = NCR.UserConfirm,
            //        //    //ASSIGNEE = NCR.UserConfirm,
            //        //    //STATUS = "New"
            //        //});
            //    }
            //}

            //if (NCR.NOTIFICATION_ONLY.Value == true)
            //{
            //    if (Check == null)
            //    {
            //        //2
            //        //_db.TASKDETAILs.Add(new TASKDETAIL
            //        //{
            //        //    TASKID = currentTaskListID,
            //        //    TASKNAME = "Notify NCR to internal",
            //        //    OWNER = NCR.UserConfirm,
            //        //    ASSIGNEE = NCR.UserConfirm,
            //        //    STATUS = "New"
            //        //});
            //    }
            //    else
            //    {
            //        foreach (var Assignee in SQEAssignees)
            //        {
            //            //_db.TASKDETAILs.Add(new TASKDETAIL
            //            //{
            //            //    TASKID = currentTaskListID,
            //            //    TASKNAME = "Notify NCR to Supplier",
            //            //    OWNER = NCR.UserConfirm,
            //            //    ASSIGNEE = Assignee.UserId,
            //            //    STATUS = "New"
            //            //});
            //        }
            //    }
            //}
            #endregion
        }
    }

}
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.Account;
using II_VI_Incorporated_SCM.Models.NCR;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
//using II_VI_Incorporated_SCM.Models.Account;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IUserService
    {
        Models.ApplicationGroup GetGroupByName(string grpname);
        string GetNameById(string Id);
        string GetSignatureById(string Id);

        bool CheckGroupRoleForUser(string UserId, string GroupName);

        IEnumerable<AspNetUser> GetListUserByGroupName(string GroupName);

        List<AspNetUser> GetAllUser();
        List<AspNetUser> GetAllUserByRole(string roleName);

        List<AspNetUserViewModel> GetListUserQTYASS(string roleName);

        List<AspNetUser> GetListUserOPE();
        List<GetUserByRoleGroupId_Result> GetUserByRoleGroupIdEng(string Id, string ncrnum);

        void UpdateOPEUser(string userId, string opeId);

        AspNetUser GetUserInfomation(string userId);
        string GetUserENGIEERINGSubmit(string NCR_Num);
        bool UpdateFile(string avatarName, string signatureName, string userId);
        bool ISENGINEERING(List<UserApproveViewModel> UserApprove);
        bool ISPURCHASING(string RoleUserId);
        bool ISENGINEERING(string RoleUserId);
        //bool ISENGINEERING(string RoleUserId);
        string GetMRBRoleId();
        List<Models.ApplicationGroup> GetSelectListAllRole();
        IEnumerable<AspNetUser> GetUserByRoleGroupId(string Id);
        IEnumerable<AspNetUser> GetUserByRoleGroupId(string Id, string ncrnum);
        string GetENGINEERINGRoleId();
        string GetCHAIRMANRoleId();
        List<UserDispositionModelView> GetAllUserWithRole();
        List<AspNetUser> GetUsersByRoleName(string[] arrRoleName);
        bool CheckIsApprover(string v, string ncrnum);
        List<AspNetUser> GetApproverByNCRNUM(string NCRNUM, string[] v);
        AspNetUser GetSubmiterNCR(string NCRNUM);
        List<AspNetUser> GetUserOfNCR(string ncrnum);
        List<AspNetUser> GetChairmanOfNCR(string nCRNUM);
        List<AspNetUser> GetENGOfNCR(string nCR_NUM);
        AspNetUser GetUserById(string v);
        List<AspNetUser> GetQUALITYASSURANCEUser(List<AspNetUser> u);
        string[] GetRoleByGroupRole(string id, string[] selectedGroups);
        string[] GetRoleByUserId(string id);
        void AddRole(string uid);
        List<AspNetUser> GetUserOfNCRApproval(string ncrnum);
        List<UserViewmodel> getlistUser();
    }

    public class UserService : IUserService
    {
        private IIVILocalDB _db;

        public UserService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public bool CheckGroupRoleForUser(string UserId, string GroupName)
        {
            List<string> ListGroupRole = _db.ApplicationUserGroups.Where(u => u.ApplicationUserId == UserId).Select(g => g.ApplicationGroup).Select(ap => ap.Name).ToList();
            return ListGroupRole.Contains(GroupName);
        }

        public IEnumerable<AspNetUser> GetListUserByGroupName(string GroupName)
        {
            Models.ApplicationGroup ApplicationGroup = _db.ApplicationGroups.Where(g => g.Name == GroupName).FirstOrDefault();
            if (ApplicationGroup != null)
            {
                List<string> ListUserId = ApplicationGroup.ApplicationUserGroups.Select(ap => ap.ApplicationUserId).ToList();
                return _db.AspNetUsers.Where(u => ListUserId.Contains(u.Id));
            }
            return new List<AspNetUser>();
        }


        public Models.ApplicationGroup GetGroupByName(string grpname)
        {
            try
            {
                return _db.ApplicationGroups.Where(o => o.Name == grpname).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateOPEUser(string userId, string opeId)
        {
            AspNetUser data = _db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
            if (data != null)
            {
                _db.AspNetUsers.Attach(data);
                data.OPE = opeId;
                _db.SaveChanges();
            }
        }

        public string GetNameById(string Id)
        {
            try
            {
                return _db.AspNetUsers.Where(m => m.Id == Id).FirstOrDefault().FullName;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public string GetSignatureById(string Id)
        {
            try
            {
                return _db.AspNetUsers.Where(m => m.Id == Id).FirstOrDefault().Signature;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public bool GetgroupUser(string UserID, string Groupname)
        {
            try
            {
                //UserID = "";
                List<string> GroupsOfUser = _db.ApplicationUserGroups.Where(gr => gr.ApplicationUserId == UserID)
                    .Select(gr => gr.ApplicationGroup).Select(ap => ap.Name).ToList();
                if (GroupsOfUser.Contains(Groupname))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<AspNetUser> GetAllUser()
        {
            List<AspNetUser> list = new List<AspNetUser>();
            try
            {
                list = _db.AspNetUsers.ToList();
                return list;
            }
            catch (Exception)
            {
                return new List<AspNetUser>();
            }
        }

        public List<AspNetUser> GetListUserOPE()
        {
            List<AspNetUser> list = new List<AspNetUser>();

            Models.ApplicationGroup data = _db.ApplicationGroups.Where(x => x.Name == "OPE").FirstOrDefault();
            string idOPE = data != null ? data.Id : "";
            if (idOPE != "")
            {
                List<string> listIdUser = _db.ApplicationUserGroups.Where(x => x.ApplicationGroupId == idOPE).Select(x => x.ApplicationUserId).ToList();
                list = _db.AspNetUsers.Where(x => listIdUser.Contains(x.Id)).ToList();
            }

            return list;
        }

        public List<AspNetUserViewModel> GetListUserQTYASS(string roleName)
        {
            List<AspNetUserViewModel> list = new List<AspNetUserViewModel>
            {
                new AspNetUserViewModel { FullName = "None selected", Id = "" }
            };
            string roleId = _db.ApplicationGroups.Where(x => x.Name == roleName).FirstOrDefault() != null ? _db.ApplicationGroups.Where(x => x.Name == roleName).FirstOrDefault().Id : "";
            List<Models.ApplicationGroupRole> listGroupId = _db.ApplicationGroupRoles.Where(x => x.ApplicationGroupId == roleId).ToList();
            listGroupId = listGroupId == null ? new List<Models.ApplicationGroupRole>() : listGroupId;
            List<Models.ApplicationUserGroup> allUserId = _db.ApplicationUserGroups.ToList();
            var ListUserId = listGroupId.Join(allUserId, p => p.ApplicationGroupId, c => c.ApplicationGroupId, (p, c) => new
            {
                ApplicationGroupId = p.ApplicationGroupId,
                ApplicationUserId = c.ApplicationUserId,
            }).ToList();

            List<AspNetUser> allUser = _db.AspNetUsers.ToList();
            list.AddRange(ListUserId.Join(allUser, p => p.ApplicationUserId, c => c.Id, (p, c) => new
            {
                Id = p.ApplicationUserId,
                FullName = c.FullName,
            }).Select(item => new AspNetUserViewModel { FullName = item.FullName, Id = item.Id }).ToList());

            return list;
        }

        public AspNetUser GetUserInfomation(string userId)
        {
            return _db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
        }
        public string GetUserENGIEERINGSubmit(string NCR_Num)
        {
            string en = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim() == NCR_Num.Trim()).Select(x => x.ENGIEERING).FirstOrDefault();
            return en;
        }

        public bool UpdateFile(string avatarName, string signatureName, string userId)
        {
            try
            {
                AspNetUser user = _db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    _db.AspNetUsers.Attach(user);
                    user.Avatar = avatarName;
                    user.Signature = signatureName;
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<AspNetUser> GetAllUserByRole(string roleName)
        {
            return _db.AspNetRoles.Single(m => m.Name.Equals(roleName)).AspNetUsers.ToList();
        }

        public bool ISENGINEERING(List<UserApproveViewModel> UserApprove)
        {
            if (UserApprove == null || UserApprove.Count <= 0)
            {
                return false;
            }

            string ENGINEERINGRoleId = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.ENGINEERING)).Id;
            List<UserApproveViewModel> checkENGINEERING = UserApprove.Where(x => x.RoleId.Trim().Equals(ENGINEERINGRoleId)).ToList();
            return checkENGINEERING.Count() > 0;
        }

        public bool ISPURCHASING(string RoleUserId)
        {
            Models.ApplicationGroup RolePURCHASING = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.PURCHASING));
            return RolePURCHASING != null ? RolePURCHASING.Id == RoleUserId : false;
        }

        public bool ISENGINEERING(string RoleUserId)
        {
            Models.ApplicationGroup RoleENGINEERING = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.ENGINEERING));
            return RoleENGINEERING != null ? RoleENGINEERING.Id == RoleUserId : false;
        }

        public List<Models.ApplicationGroup> GetSelectListAllRole()
        {
            string[] role = new string[] { "Inspector", "SuperAdmins", "OPE", "MRB" };
            return _db.ApplicationGroups.Where(x => !role.Contains(x.Name.Trim())).ToList();
        }

        public IEnumerable<AspNetUser> GetUserByRoleGroupId(string Id)
        {
            //var arrUserId = _db.ApplicationUserGroups.Where(x => x.ApplicationGroupId.Trim().Equals(Id.Trim())).Select(x=>x.ApplicationUserId).ToArray();
            //return _db.AspNetUsers.Where(u => arrUserId.Contains(u.Id));
            Models.ApplicationGroup ApplicationGroup = _db.ApplicationGroups.Where(g => g.Id == Id).FirstOrDefault();
            if (ApplicationGroup != null)
            {
                List<string> ListUserId = ApplicationGroup.ApplicationUserGroups.Select(ap => ap.ApplicationUserId).ToList();
                return _db.AspNetUsers.Where(u => ListUserId.Contains(u.Id));
            }
            return new List<AspNetUser>();
        }

        public IEnumerable<AspNetUser> GetUserByRoleGroupId(string Id, string ncrnum)
        {
            var arrApprover = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim() == ncrnum.Trim() & x.isActive == true).Select(x => x.UserId).ToArray();

            Models.ApplicationGroup ApplicationGroup = _db.ApplicationGroups.Where(g => g.Id == Id).FirstOrDefault();
            if (ApplicationGroup != null)
            {
                List<string> ListUserId = ApplicationGroup.ApplicationUserGroups.Select(ap => ap.ApplicationUserId).ToList();
                return !string.IsNullOrEmpty(ncrnum) ? _db.AspNetUsers.Where(u => ListUserId.Contains(u.Id) & !arrApprover.Contains(u.Id)) :
                        _db.AspNetUsers.Where(u => ListUserId.Contains(u.Id));
            }
            return new List<AspNetUser>();
        }
        public List<GetUserByRoleGroupId_Result> GetUserByRoleGroupIdEng(string Id, string ncrnum)
        {
            var listuser = _db.GetUserByRoleGroupId(Id).ToList();
            return listuser;
           // return new List<AspNetUser>();
        }
        public string GetENGINEERINGRoleId()
        {
            return _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.ENGINEERING)).Id;
        }
        public string GetMRBRoleId()
        {
            return _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.MRBTeam)).Id;
        }
        public List<UserDispositionModelView> GetAllUserWithRole()
        {
            List<UserDispositionModelView> res = (from user in _db.AspNetUsers
                                                  join r in _db.ApplicationUserGroups on user.Id equals r.ApplicationUserId
                                                  select new UserDispositionModelView
                                                  {
                                                      UserId = user.Id,
                                                      RoleName = user.FullName,
                                                      RoleId = r.ApplicationGroupId
                                                  }).ToList();
            return res;
        }

        public string GetCHAIRMANRoleId()
        {
            return _db.ApplicationGroups.FirstOrDefault(x => x.Name.Trim().Equals(UserGroup.CHAIRMAN)).Id;
        }

        public List<AspNetUser> GetUsersByRoleName(string[] arrRoleName)
        {
            string[] arrRoleId = _db.ApplicationGroups.Where(x => arrRoleName.Contains(x.Name.Trim())).Select(x => x.Id).ToArray();
            string[] arrUserId = _db.ApplicationUserGroups.Where(x => arrRoleId.Contains(x.ApplicationGroupId)).Select(x => x.ApplicationUserId).ToArray();
            return _db.AspNetUsers.Where(x => arrUserId.Contains(x.Id)).ToList();
        }

        public bool CheckIsApprover(string v, string ncrnum)
        {
            APPROVAL approver = _db.APPROVALs.FirstOrDefault(x => x.UserId.Equals(v) & x.NCR_NUMBER == ncrnum & x.isActive == true);
            return approver != null;
        }

        public List<AspNetUser> GetApproverByNCRNUM(string NCRNUM, string[] v = null)
        {
            if (v == null)
            {
                v = new string[] { };
            }

            IQueryable<APPROVAL> approver = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim() == NCRNUM.Trim() & x.isActive == true & !v.Contains(x.RoleId));
            string[] arrApprover = approver.Select(x => x.UserId).ToArray();
            List<AspNetUser> Approvers = _db.AspNetUsers.Where(x => arrApprover.Contains(x.Id)).ToList();
            return Approvers;
        }

        //public List<AspNetUser> GetSubmiterNCR(string NCRNUM)
        public AspNetUser GetSubmiterNCR(string NCRNUM)
        {
            NCR_HDR NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(NCRNUM.Trim()));
            if (NCR == null)
            {
                return null;
            }
            else
            {
                //var INSPECTOR = _db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(NCR.INSPECTOR));
                //if (string.IsNullOrEmpty(INSPECTOR.OPE)) return null;
                AspNetUser OPE = _db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(NCR.USERSUBMIT));

                return OPE;
            }
        }

        public List<AspNetUser> GetUserOfNCR(string ncrnum)
        {
            NCR_HDR NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(ncrnum.Trim()));
            if (NCR == null)
            {
                return new List<AspNetUser>();
            }

            string[] arrNId = new string[] { NCR.USERSUBMIT, NCR.USERDISPO, NCR.UserConfirm };

            List<APPROVAL> approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(ncrnum.Trim()) & x.isActive == true).ToList();
            IEnumerable<string> arrAppId = approvers.Select(x => x.UserId);
            IEnumerable<string> arrId = arrAppId.Concat(arrNId);

            List<AspNetUser> resApp = _db.AspNetUsers.Where(x => arrId.Contains(x.Id)).ToList();
            return resApp;
        }
        public List<AspNetUser> GetUserOfNCRApproval(string ncrnum)
        {
            Models.ApplicationGroup Chairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.CHAIRMAN));
            NCR_HDR NCR = _db.NCR_HDR.FirstOrDefault(x => x.NCR_NUM.Trim().Equals(ncrnum.Trim()));
            if (NCR == null)
            {
                return new List<AspNetUser>();
            }

            string[] arrNId = new string[] { NCR.USERSUBMIT, NCR.USERDISPO, NCR.UserConfirm };

            List<APPROVAL> approvers = _db.APPROVALs.Where(x => x.NCR_NUMBER.Trim().Equals(ncrnum.Trim()) & x.isActive == true && x.RoleId !=Chairman.Id).ToList();
            IEnumerable<string> arrAppId = approvers.Select(x => x.UserId);
            IEnumerable<string> arrId = arrAppId.Concat(arrNId);

            List<AspNetUser> resApp = _db.AspNetUsers.Where(x => arrId.Contains(x.Id)).ToList();
            return resApp;
        }

        public List<AspNetUser> GetChairmanOfNCR(string nCRNUM)
        {
            Models.ApplicationGroup Chairman = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.CHAIRMAN));
            List<APPROVAL> Chairmans = _db.APPROVALs.Where(x => x.RoleId.Equals(Chairman.Id) & x.NCR_NUMBER.Trim().Equals(nCRNUM.Trim()) & x.isActive == true).ToList();
            IEnumerable<string> arrId = Chairmans.Select(x => x.UserId);
            List<AspNetUser> res = _db.AspNetUsers.Where(x => arrId.Contains(x.Id)).ToList();
            return res;
        }

        List<AspNetUser> IUserService.GetENGOfNCR(string nCR_NUM)
        {
            Models.ApplicationGroup engID = _db.ApplicationGroups.FirstOrDefault(x => x.Name == UserGroup.ENGINEERING);
            string[] arrId = _db.APPROVALs.Where(x => x.NCR_NUMBER.Equals(nCR_NUM) & x.RoleId.Equals(engID.Id)).Select(x => x.UserId).ToArray();
            List<AspNetUser> Users = _db.AspNetUsers.Where(x => arrId.Contains(x.Id)).ToList();

            return Users;
        }

        public AspNetUser GetUserById(string v)
        {
            return _db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(v));
        }

        public List<AspNetUser> GetQUALITYASSURANCEUser(List<AspNetUser> v)
        {
            string role = _db.ApplicationGroups.FirstOrDefault(x => x.Name.Equals(UserGroup.QUALITYASSURANCE)).Id;
            string[] arrId = v.Select(x => x.Id).ToArray();
            string[] U = _db.ApplicationUserGroups.Where(x => x.ApplicationGroupId.Equals(role) & arrId.Contains(x.ApplicationUserId)).Select(x => x.ApplicationUserId).ToArray();

            return v.Where(x => U.Contains(x.Id)).ToList();
        }

        public string[] GetRoleByGroupRole(string id, string[] selectedGroups)
        {
            string[] arrResult = { };
            var user = _db.AspNetUsers.FirstOrDefault(x=>x.Id.Equals(id));
            if(user != null)
            {
                arrResult = _db.ApplicationGroupRoles.Where(x => selectedGroups.Contains(x.ApplicationGroupId)).Select(x => x.ApplicationRoleId).ToArray(); ;
                //var arrRoles = _db.ApplicationGroupRoles.Where(x => selectedGroups.Contains(x.ApplicationGroupId)).Select(x=>x.ApplicationRoleId).ToArray();
                //arrResult = _db.AspNetRoles.Where(x => arrRoles.Contains(x.Id)).Select(x => x.Name).ToArray();
            }

            return arrResult;
        }

        public string[] GetRoleByUserId(string id)
        {
            var groupRole = _db.ApplicationUserGroups.Where(x => x.ApplicationUserId.Equals(id)).Select(x => x.ApplicationGroupId).ToArray();
            return _db.ApplicationGroupRoles.Where(x => groupRole.Contains(x.ApplicationGroupId)).Select(x => x.ApplicationRoleId).ToArray();
        }

        public void AddRole(string uid)
        {

        }
        public List<UserViewmodel> getlistUser()
        {
            string query = $"select us.FullName as Fullname,l.Name  as Role ,ua.Fullname as OPEName from ApplicationGroups l left join ApplicationUserGroups r on l.ID = r.ApplicationGroupid left join AspNetUsers us on r.ApplicationUserId = us.Id left join AspNetUsers ua on us.ope = ua.id";
            List<UserViewmodel> Listuser = new List<UserViewmodel>();

              Listuser = _db.Database.SqlQuery<UserViewmodel>(query).ToList();
            return Listuser;
        }
    }
}
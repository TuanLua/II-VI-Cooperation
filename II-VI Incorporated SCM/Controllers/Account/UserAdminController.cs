using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using II_VI_Incorporated_SCM.Models.Account;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using II_VI_Incorporated_SCM.Services;

namespace II_VI_Incorporated_SCM.Controllers.Account
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        private readonly IUserService _iUserService;
        public UsersAdminController(IUserService iUserService)
        {
            _iUserService = iUserService;
        }

        public UsersAdminController(ApplicationUserManager userManager, 
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // Add the Group Manager (NOTE: only access through the public
        // Property, not by the instance variable!)
        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                    .Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }


        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            // Show the groups the user belongs to:
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            ViewBag.GroupNames = userGroups.Select(u => u.Name).ToList();
            return View(user);
        }


        public ActionResult Create()
        {
            // Show a list of available groups:
            ViewBag.GroupsList = 
                new SelectList(this.GroupManager.Groups, "Id", "Name");
            var listOPE = _iUserService.GetListUserOPE();
            ViewBag.OPEUserList = new List<SelectListItem>();
            foreach (var item in listOPE)
            {
                var listItem = new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id,
                };
                ViewBag.OPEUserList.Add(listItem);
            }

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, 
            params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                {   
                    FullName = userViewModel.Fullname,
                    Email = userViewModel.Email,
                    UserName = userViewModel.Username,     
                    OPE = userViewModel.OPE                
                };
                var adminresult = await UserManager
                    .CreateAsync(user, userViewModel.Password);

                //Add User to the selected Groups 
                if (adminresult.Succeeded)
                {
                    if (selectedGroups != null)
                    {
                        selectedGroups = selectedGroups ?? new string[] { };
                        await this.GroupManager
                            .SetUserGroupsAsync(user.Id, selectedGroups);
                    }
                    return RedirectToAction("Index");
                }
                AddErrors(adminresult);
            }
            ViewBag.GroupsList = new SelectList(
                await RoleManager.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Display a list of available Groups:
            var allGroups = this.GroupManager.Groups;
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            var listOPE = _iUserService.GetListUserOPE();

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Fullname = user.FullName,
                Username = user.UserName,
                OPE = user.OPE,
            };

            foreach (var group in allGroups)
            {
                var listItem = new SelectListItem()
                {
                    Text = group.Name,
                    Value = group.Id,
                    Selected = userGroups.Any(g => g.Id == group.Id)
                };
                model.GroupsList.Add(listItem);
            }

            model.OPEUserList = new List<SelectListItem>();
            foreach (var item in listOPE)
            {
                var listItem = new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id,
                };
                model.OPEUserList.Add(listItem);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Fullname,Username,Email,Id,OPE")] EditUserViewModel editUser, params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Update the User:
                user.UserName = editUser.Username;
                user.FullName = editUser.Fullname;
                user.Email = editUser.Email;
                user.OPE = editUser.OPE;
                await this.UserManager.UpdateAsync(user);
                _iUserService.UpdateOPEUser(editUser.Id, editUser.OPE);
                // Update the Groups:
                selectedGroups = selectedGroups ?? new string[] { };
                await this.GroupManager.SetUserGroupsAsync(user.Id, selectedGroups);

                //Update the Role of group role
                //string[] arrRoles = new HashSet<string>(_iUserService.GetRoleByGroupRole(user.Id, selectedGroups)).ToArray();

                //var roles = await UserManager.GetRolesAsync(user.Id);
                //await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());

                //await GroupManager.RefreshUserGroupRolesAsync(user.Id);
                //await UserManager.AddToRolesAsync(user.Id, arrRoles);
                //GroupManager.SetUserRole(id: user.Id, arrRoles);
                //_iUserService.AddRole(user.Id);

                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }


        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Remove all the User Group references:
                await this.GroupManager.ClearUserGroupsAsync(id);

                // Then Delete the User:
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

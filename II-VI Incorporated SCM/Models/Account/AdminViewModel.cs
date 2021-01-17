using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Models.Account
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Module")]
        public string Group { get; set; }

        public bool Selected { get; set; }
    }

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }

        [Required(AllowEmptyStrings = false)]
        public string Fullname { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public string OPE { get; set; }

        // We will still use this, so leave it here:
        public ICollection<SelectListItem> RolesList { get; set; }

        // Add a GroupsList Property:
        public ICollection<SelectListItem> GroupsList { get; set; }

        //Add list OPE User
        public List<SelectListItem> OPEUserList { get; set; }
    }

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<RoleViewModel>();
        }
        //[Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public ICollection<SelectListItem> UsersList { get; set; }
        public ICollection<RoleViewModel> RolesList { get; set; }
    }
}
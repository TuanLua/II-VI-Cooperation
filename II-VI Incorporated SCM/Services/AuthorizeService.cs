//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace II_VI_Incorporated_SCM.Services
//{
//    public class AuthorizeService
//    {
//        private ApplicationUserManager _userManager;
//        private ApplicationSignInManager _signInManager;
//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set { _signInManager = value; }
//        }

//    }
//}
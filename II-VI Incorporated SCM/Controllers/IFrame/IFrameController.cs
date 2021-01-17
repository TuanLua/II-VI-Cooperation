using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.IFrame
{
    public class IFrameController : Controller
    {
        // GET: IFrame
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public ActionResult Redirect(string url)
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
        {
            ViewBag.Url = url;
            return View();
        }
    }
}
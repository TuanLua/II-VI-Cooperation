using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Areas.Innovation.Controllers
{
    [Authorize]
    public class IdeaSuggestionController : Controller
    {
        // GET: Innovation/IdeaSuggestion
        public ActionResult Index()
        {
            return View();
        }
    }
}
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Models.TaskManagement;
using II_VI_Incorporated_SCM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Controllers.Account
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHomeService _iHomeService;
        public HomeController(IHomeService iHomeService)
        {
            _iHomeService  = iHomeService;
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetMenus()
        {
            TaskManMenuModel menuTaskManagement = new TaskManMenuModel();

            IEnumerable<TASKLIST> lsTaskList = new List<TASKLIST>();
            lsTaskList = _iHomeService.GetAllTaskManagement();
            foreach (var item in lsTaskList)
            {
                if(item.TYPE == TaskManagement.NCR)
                {
                    menuTaskManagement.NCRCount++;
                }
                else if (item.TYPE == TaskManagement.SCAR)
                {
                    menuTaskManagement.SCARCount++;
                }
                else if (item.TYPE == TaskManagement.Supplier)
                {
                    menuTaskManagement.SUPPCount++;
                }
            }
            menuTaskManagement.lsTaskList = lsTaskList;

            return PartialView("Menu", menuTaskManagement);
        }
    }
}
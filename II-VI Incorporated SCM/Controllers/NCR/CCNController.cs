using System.Web.Mvc;
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Services;
namespace II_VI_Incorporated_SCM.Controllers.NCR
{
    [Authorize]
    public class CcnController : Controller
    {
        private readonly ICCNService _ICCNService;
        public CcnController(ICCNService ICCNService)
        {
            _ICCNService = ICCNService;
        }

        [HttpPost]
        public JsonResult GetCCN()
        {
            var list = _ICCNService.GetListCCN();
          
            return Json(new { success = true, data = list });
        }
    }
}
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM.Areas.Innovation
{
    public class InnovationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Innovation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Innovation_default",
                "Innovation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }      ,
                new string[] { "II_VI_Incorporated_SCM.Areas.Innovation.Controllers" }
            );
        }
    }
}
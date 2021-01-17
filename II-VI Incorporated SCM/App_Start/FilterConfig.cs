using System.Web;
using System.Web.Mvc;

namespace II_VI_Incorporated_SCM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

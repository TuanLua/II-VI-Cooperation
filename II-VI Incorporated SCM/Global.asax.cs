using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using II_VI_Incorporated_SCM.Services;

namespace II_VI_Incorporated_SCM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            II_VI_DbFactory.Builder();
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            //log the error!
            var _log = new LogWriter("Catch System");
            _log.LogWrite(ex.ToString());
            if (ex is DbEntityValidationException)
            {
                var e = (DbEntityValidationException)ex;
                foreach (var eve in e.EntityValidationErrors)
                {
                    _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage));
                    }
                }
            }
        }
    }
}

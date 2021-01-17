using II_VI_Incorporated_SCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Services
{
    public interface IHomeService
    {
        IEnumerable<TASKLIST> GetAllTaskManagement();
    }

    public class HomeService : IHomeService
    {
        #region InitDB
        private IIVILocalDB dbContext;

        public HomeService(IDbFactory dbFactory)
        {
            dbContext = dbFactory.Init();
        }
        #endregion

        #region Service
        public IEnumerable<TASKLIST> GetAllTaskManagement()
        {
            List<TASKLIST> lsTaskMan = new List<TASKLIST>();
            lsTaskMan = dbContext.TASKLISTs.ToList();
            return lsTaskMan;
        }
        #endregion

        #region Function
        #endregion
    }
}
using II_VI_Incorporated_SCM.Models;
using II_VI_Incorporated_SCM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Library
{
    public class Library
    {
        private IIVILocalDB _db;

        public Library(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        //public string GetAutoNCR_NUM()
        //{

        //}
    }
}
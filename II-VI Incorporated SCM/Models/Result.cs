using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models
{
    public class Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object obj { get; set; }

        public Result()
        {
            success = false;
            message = "";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample
{
    public class DBConfig
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["WfDBConnectionString"].ToString();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample.Models
{
    public class QueryEntity 
    {
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string ProcessGUID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
using Slickflow.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample.Models
{
    public class WfRunner
    {
        public WfRunner()
        {
            NextActivityPerformers = new List<Point>();
        }
        public int ID { get; set; }
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string ProcessGUID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public IList<Point> NextActivityPerformers { get; set; }
        public string JumpbackActivityGUID { get; set; }     //回跳的节点GUID
    }

    public class Point
    {
        public string PathID { get; set; }
        public string UserID{get;set;}
        public string UserName{get;set;}
    }

}
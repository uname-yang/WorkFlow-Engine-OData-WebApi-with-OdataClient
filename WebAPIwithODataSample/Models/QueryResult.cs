using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIwithODataSample.Models
{
    public class QueryResult
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string ProcessGUID { get; set; }
        public string Version { get; set; }
        public int ProcessInstanceID { get; set; }
        public string ActivityGUID { get; set; }
        public int ActivityInstanceID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityType { get; set; }
        public short TaskType { get; set; }
        public string AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public string EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public short TaskState { get; set; }
        public short ActivityState { get; set; }
        public short ProcessState { get; set; }
    }
}
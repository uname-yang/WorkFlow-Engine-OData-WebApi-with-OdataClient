using AutoMapper;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPIwithODataSample.Models;

namespace WebAPIwithODataSample.Controllers
{
    public class QueryEntitiesController : ODataController
    {

        // GET api/<controller>/5
        public IHttpActionResult Get()
        {
            return Ok("value");
        }

        #region 任务数据读取操作
        [HttpGet]
        [ODataRoute("GetRunningTasks(query={query})")]
        public IEnumerable<QueryResult> GetRunningTasks([FromODataUri]QueryEntity query)
        {
            IWorkflowService service = new WorkflowService();
            Mapper.CreateMap<TaskViewEntity, QueryResult>();
            Mapper.CreateMap<QueryEntity, TaskQueryEntity>();
            var que = Mapper.Map<TaskQueryEntity>(query);
            try
            {
                var result = service.GetRunningTasks(que);
                if (result == null)
                {
                    return new List<QueryResult>().AsEnumerable();
                }
                else
                {
                    var aa = Mapper.Map<IList<QueryResult>>(result);
                    return aa.AsEnumerable();
                }
            }
            catch 
            {
                return new List<QueryResult>().AsEnumerable();
            }
        }

        [HttpGet]
        [ODataRoute("GetReadyTasks(query={query})")]
           public IEnumerable<QueryResult> GetReadyTasks([FromODataUri]QueryEntity query)
        {
            IWorkflowService service = new WorkflowService();
            Mapper.CreateMap<TaskViewEntity, QueryResult>();
            Mapper.CreateMap<QueryEntity, TaskQueryEntity>();
            var que = Mapper.Map<TaskQueryEntity>(query);
            try
            {
                var result = service.GetReadyTasks(que);
                if (result == null)
                {
                    return new List<QueryResult>().AsEnumerable();
                }
                else
                {
                    var aa = Mapper.Map<IList<QueryResult>>(result);
                    return aa.AsEnumerable();
                }
            }
            catch 
            {
                return new List<QueryResult>().AsEnumerable();
            }
        }
        #endregion
    }
}
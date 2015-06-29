using AutoMapper;
using Newtonsoft.Json;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Common;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPIwithODataSample.Extension;
using WebAPIwithODataSample.Models;

namespace WebAPIwithODataSample.Controllers
{
    public class WfRunnersController : ODataController
    {
        #region Workflow Api访问操作
        // 启动流程
        [HttpPost]
        [ODataRoute("StartProcess")]
        public IHttpActionResult StartProcess(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 

            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();
            IDictionary<string, Slickflow.Engine.Common.PerformerList> NextActivityPerformers = WfAppRunner.NextActivityPerformers;

            int ProcInstID;
            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                WfExecutedResult result = wfService.StartProcess(conn, WfAppRunner, trans);

                int newProcessInstanceID = result.ProcessInstanceIDStarted;

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    ProcInstID = newProcessInstanceID;
                    //return Ok(ProcInstID);
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            conn.Open();
            try
            {
               WfAppRunner.NextActivityPerformers = NextActivityPerformers;

               trans = conn.BeginTransaction();
                WfExecutedResult result = wfService.RunProcessApp(conn, WfAppRunner, trans);

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    return Ok(ProcInstID);
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        /// 运行流程
        [HttpPost]
        [ODataRoute("RunProcess")]
        public IHttpActionResult RunProcessApp(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 

            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.RunProcessApp(conn, WfAppRunner, trans);

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    return Ok("Sucess");
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // 撤销流程
        [HttpPost]
        [ODataRoute("WithdrawProcess")]
        public IHttpActionResult WithdrawProcess(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.WithdrawProcess(conn, WfAppRunner, trans);

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    return Ok("Sucess");
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        
        // 退回流程 <=>撤销
        [HttpPost]
        [ODataRoute("SendBackProcess")]
        public IHttpActionResult SendBackProcess(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.SendBackProcess(conn, WfAppRunner, trans);

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    return Ok("Sucess");
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // 流程跳转
        [HttpPost]
        [ODataRoute("JumpProcess")]
        public IHttpActionResult JumpProcess(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.JumpProcess(conn, WfAppRunner, trans);

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    return Ok("Sucess");
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        //反签
        [HttpPost]
        [ODataRoute("ReverseProcess")]
        public IHttpActionResult ReverseProcess(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.ReverseProcess(conn, WfAppRunner, trans);

                if (result.Status == WfExecutedStatus.Success)
                {
                    trans.Commit();
                    return Ok();
                }
                else
                {
                    trans.Rollback();
                    return BadRequest(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return BadRequest(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        //废弃
        [HttpPost]
        [ODataRoute("DiscardProcess")]
        public IHttpActionResult DiscardProcess(ODataActionParameters parameters)
        {
            WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
            var WfAppRunner = WfRunner.ToAppRunner(); 
            IWorkflowService service = new WorkflowService();
            try
            {
                var result = service.DiscardProcess(WfAppRunner);
            return Ok();
            }
            catch (Exception w)
            {
                return BadRequest(w.Message);
            }
        }

        //[HttpPost]
        //[ODataRoute("SuspendProcess")]
        //public IHttpActionResult SuspendProcess(ODataActionParameters parameters)
        //{
        //    WfRunner WfRunner = parameters["WfRunner"] as WfRunner;
        //    var WfAppRunner = WfRunner.ToAppRunner(); 
        //    IWorkflowService service = new WorkflowService();
        //    try
        //    {
        //        var result = service.SuspendProcess(WfAppRunner.TaskID,WfAppRunner);
        //    return Ok();
        //    }
        //    catch (Exception w)
        //    {
        //        return BadRequest(w.Message);
        //    }
        //}      
        #endregion Workflow Api访问操作

    }
}
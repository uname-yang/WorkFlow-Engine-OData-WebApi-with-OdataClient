/*
* Slickflow 工作流引擎遵循LGPL协议，也可联系作者商业授权并获取技术支持；
* 除此之外的使用则视为不正当使用，请您务必避免由此带来的商业版权纠纷。
* 
The Slickflow project.
Copyright (C) 2014  .NET Workflow Engine Library

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, you can access the official
web page about lgpl: https://www.gnu.org/licenses/lgpl.html
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Threading;
using Slickflow.Engine.Core;
using Slickflow.Engine.Common;
using Slickflow.Engine.Utility;
using Slickflow.Data;
using Slickflow.Engine.Xpdl;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Business.Manager;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Core.Event;
using Slickflow.Engine.Parser;

namespace Slickflow.Engine.Service
{
    /// <summary>
    /// 服务扩展类
    /// </summary>
    public partial class WorkflowService : IWorkflowService
    {
        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public WfExecutedResult SignForwardProcess(WfAppRunner runner)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            IDbTransaction trans = null;

            try
            {
                trans = conn.BeginTransaction();
                var result = SignForwardProcess(conn, runner, trans);

                if (result.Status == WfExecutedStatus.Success)
                    trans.Commit();
                else
                    trans.Rollback();

                return result;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="runner"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public WfExecutedResult SignForwardProcess(IDbConnection conn, WfAppRunner runner, IDbTransaction trans)
        {
            try
            {
                IDbSession session = SessionFactory.CreateSession(conn, trans);
                var runtimeInstance = WfRuntimeManagerFactory.CreateRuntimeInstanceSignForward(runner, ref _signforwardResult);

                if (_signforwardResult.Status == WfExecutedStatus.Exception)
                {
                    return _signforwardResult;
                }

                runtimeInstance.OnWfProcessExecuted += runtimeInstance_OnWfProcessSignForward;
                runtimeInstance.Execute(session);

                waitHandler.WaitOne();
            }
            catch (System.Exception e)
            {
                _signforwardResult.Status = WfExecutedStatus.Failed;
                _signforwardResult.Message = string.Format("流程加签发生错误，内部异常:{0}", e.Message);
                LogManager.RecordLog(WfDefine.WF_PROCESS_ERROR, LogEventType.Error, LogPriority.High, runner, e);
            }
            return _signforwardResult;
        }

        private void runtimeInstance_OnWfProcessSignForward(object sender, WfEventArgs args)
        {
            _signforwardResult = args.WfExecutedResult;
            waitHandler.Set();
        }
    }
}

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
using System.Threading;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slickflow.Engine.Common;
using Slickflow.Engine.Utility;
using Slickflow.Data;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Business.Manager;
using Slickflow.Engine.Xpdl;
using Slickflow.Engine.Core.Event;
using Slickflow.Engine.Core.Pattern;

namespace Slickflow.Engine.Core
{
    /// <summary>
    /// 退回流程运行时
    /// </summary>
    internal class WfRuntimeManagerSendBackMI : WfRuntimeManager
    {
        /// <summary>
        /// 多实例节点（会签，加签）情况下的退回处理
        /// </summary>
        /// <param name="session"></param>
        internal override void ExecuteInstanceImp(IDbSession session)
        {
            //创建撤销到上一步的节点记录
            var nodeMediatorBackward = new NodeMediatorBackward(base.BackwardContext, session);
            nodeMediatorBackward.CreateBackwardActivityTaskOfInnerMultipleInstance(
                base.BackwardContext.ProcessInstance,
                base.BackwardContext.BackwardToTaskActivityInstance,
                BackwardTypeEnum.SendbackOfMI,
                base.BackwardContext.BackwardFromActivityInstance.ID,
                base.ActivityResource,
                session);

            //置当前节点为退回状态
            var aim = new ActivityInstanceManager();
            var runningNode = BackwardContext.BackwardFromActivityInstance;
            aim.SendBack(runningNode.ID, base.ActivityResource.AppRunner, session);

            //创建新的一条待办状态的记录，用于下次执行
            var newSuspendNode = aim.CreateActivityInstanceObject(runningNode);
            newSuspendNode.ActivityState = (short)ActivityStateEnum.Suspended;
            newSuspendNode.MIHostActivityInstanceID = runningNode.MIHostActivityInstanceID;
            newSuspendNode.CompleteOrder = runningNode.CompleteOrder;
            newSuspendNode.ComplexType = runningNode.ComplexType;
            newSuspendNode.SignForwardType = runningNode.SignForwardType;
            newSuspendNode.AssignedToUserIDs = runningNode.AssignedToUserIDs;
            newSuspendNode.AssignedToUserNames = runningNode.AssignedToUserNames;

            aim.Insert(newSuspendNode, session);

            //同时为此活动实例，创建新的任务
            var tm = new TaskManager();
            tm.Renew(base.BackwardContext.BackwardFromActivityInstance, newSuspendNode, base.AppRunner, session);

            //构造回调函数需要的数据
            WfExecutedResult result = base.WfExecutedResult;
            result.BackwardTaskReciever = base.BackwardContext.BackwardTaskReciever;
            result.Status = WfExecutedStatus.Success;
        }
    }
}

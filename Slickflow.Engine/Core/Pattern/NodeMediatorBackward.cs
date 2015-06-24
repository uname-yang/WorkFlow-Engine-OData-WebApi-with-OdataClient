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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Slickflow.Engine.Common;
using Slickflow.Engine.Utility;
using Slickflow.Data;
using Slickflow.Engine.Xpdl;
using Slickflow.Engine.Xpdl.Node;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Business.Manager;

namespace Slickflow.Engine.Core.Pattern
{
    /// <summary>
    /// 退回处理时的节点调节器
    /// </summary>
    internal class NodeMediatorBackward : NodeMediator
    {
        internal NodeMediatorBackward(BackwardContext backwardContext, IDbSession session)
            : base(backwardContext, session)
        {
            
        }

        internal override void ExecuteWorkItem()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建退回时的流转节点对象、任务和转移数据
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="backMostPreviouslyActivityInstanceID"></param>
        /// <param name="transitionGUID"></param>
        /// <param name="transitionType"></param>
        /// <param name="flyingType"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CreateBackwardActivityTaskTransitionInstance(ProcessInstanceEntity processInstance,
            ActivityInstanceEntity fromActivityInstance,
            BackwardTypeEnum backwardType,
            int backMostPreviouslyActivityInstanceID,
            string transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            ActivityResource activityResource,
            IDbSession session)
        {
            //实例化Activity
            var toActivityInstance = base.CreateBackwardToActivityInstanceObject(processInstance,
                backwardType,
                backMostPreviouslyActivityInstanceID,
                activityResource.AppRunner);

            //进入准备运行状态
            toActivityInstance.ActivityState = (short)ActivityStateEnum.Ready;
            toActivityInstance.AssignedToUserIDs = base.GenerateActivityAssignedUserIDs(
                activityResource.NextActivityPerformers[base.BackwardContext.BackwardToTaskActivity.ActivityGUID]);
            toActivityInstance.AssignedToUserNames = base.GenerateActivityAssignedUserNames(
                activityResource.NextActivityPerformers[base.BackwardContext.BackwardToTaskActivity.ActivityGUID]);

            //插入活动实例数据
            base.ActivityInstanceManager.Insert(toActivityInstance,
                session);

            //插入任务数据
            base.CreateNewTask(toActivityInstance, activityResource, session);

            //插入转移数据
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                activityResource.AppRunner,
                session);
        }

         /// <summary>
        /// 退回是会签情况下的处理：
        /// 要退回的节点是会签节点
        /// 1) 全部实例化会签节点下的多实例节点
        /// 2) 只取得办理完成的节点，而且保证CompleteOrder的唯一性
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="originalBackwardToActivityInstance"></param>
        /// <param name="backwardType"></param>
        /// <param name="backSrcActivityInstanceID"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CreateBackwardActivityTaskRepeatedSignTogetherMultipleInstance(ProcessInstanceEntity processInstance,
            ActivityEntity backwardToTaskActvity,
            ActivityInstanceEntity fromActivityInstance,
            BackwardTypeEnum backwardType,
            ActivityInstanceEntity previousMainInstance,
            string transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            ActivityResource activityResource,
            IDbSession session)
        {
            //上一步节点是会签节点的退回处理
            //需要重新实例化会签节点上的所有办理人的任务
            //重新封装任务办理人为AssignedToUsers, AssignedToUsernames
            var performerList = AntiGenerateActivityPerformerList(previousMainInstance);

            activityResource.NextActivityPerformers.Clear();
            activityResource.NextActivityPerformers = new Dictionary<string, PerformerList>();
            activityResource.NextActivityPerformers.Add(backwardToTaskActvity.ActivityGUID, performerList);

            //重新生成会签节点的多实例数据
            CreateMultipleInstance(backwardToTaskActvity, processInstance, fromActivityInstance,
                transitionGUID, transitionType, flyingType, activityResource, session);
        }

        /// <summary>
        /// 退回是加签情况下的处理：
        /// 要退回的节点是加签节点
        /// 只实例化当初的加签主节点
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="backwardToTaskActvity"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="backwardType"></param>
        /// <param name="previousMainInstance"></param>
        /// <param name="transitionGUID"></param>
        /// <param name="transitionType"></param>
        /// <param name="flyingType"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CreateBackwardActivityTaskRepateSignForwardMainNodeOnly(ProcessInstanceEntity processInstance,
            ActivityEntity backwardToTaskActvity,
            ActivityInstanceEntity fromActivityInstance,
            BackwardTypeEnum backwardType,
            ActivityInstanceEntity previousMainInstance,
            string transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            ActivityResource activityResource,
            IDbSession session)
        {
            // 退回是加签情况下的处理：
            // 要退回的节点是加签节点
            // 只实例化当初的加签主节点
            //重新封装任务办理人为AssignedToUsers, AssignedToUsernames
            var performerList = AntiGenerateActivityPerformerList(previousMainInstance);

            activityResource.NextActivityPerformers.Clear();
            activityResource.NextActivityPerformers = new Dictionary<string, PerformerList>();
            activityResource.NextActivityPerformers.Add(backwardToTaskActvity.ActivityGUID, performerList);

            //实例化Activity
            var toActivityInstance = base.CreateBackwardToActivityInstanceObject(processInstance,
                backwardType,
                previousMainInstance.ID,
                activityResource.AppRunner);

            //进入准备运行状态
            toActivityInstance.ActivityState = (short)ActivityStateEnum.Ready;
            toActivityInstance.AssignedToUserIDs = previousMainInstance.AssignedToUserIDs;
            toActivityInstance.AssignedToUserNames = previousMainInstance.AssignedToUserNames;
            toActivityInstance.ComplexType = previousMainInstance.ComplexType;
            toActivityInstance.CompleteOrder = previousMainInstance.CompleteOrder;
            toActivityInstance.SignForwardType = previousMainInstance.SignForwardType;

            //插入活动实例数据
            base.ActivityInstanceManager.Insert(toActivityInstance,
                session);

            //插入任务数据
            base.CreateNewTask(toActivityInstance, activityResource, session);

            //插入转移数据
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                activityResource.AppRunner,
                session);
        }

        /// <summary>
        /// 创建多实例节点之间回滚时的活动实例，任务数据
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="originalBackwardToActivityInstance"></param>
        /// <param name="backwardType"></param>
        /// <param name="backSrcActivityInstanceID"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CreateBackwardActivityTaskOfInnerMultipleInstance(ProcessInstanceEntity processInstance,
            ActivityInstanceEntity originalBackwardToActivityInstance,
            BackwardTypeEnum backwardType,
            int backSrcActivityInstanceID,
            ActivityResource activityResource,
            IDbSession session)
        {
            //创建回滚到的节点信息
            var rollbackPreviousActivityInstance = base.CreateBackwardToActivityInstanceObject(processInstance,
                backwardType,
                backSrcActivityInstanceID,
                activityResource.AppRunner);

            rollbackPreviousActivityInstance.ActivityState = (short)ActivityStateEnum.Ready;
            rollbackPreviousActivityInstance.MIHostActivityInstanceID = originalBackwardToActivityInstance.MIHostActivityInstanceID;
            rollbackPreviousActivityInstance.CompleteOrder = originalBackwardToActivityInstance.CompleteOrder;
            rollbackPreviousActivityInstance.ComplexType = originalBackwardToActivityInstance.ComplexType;
            rollbackPreviousActivityInstance.SignForwardType = originalBackwardToActivityInstance.SignForwardType;
            rollbackPreviousActivityInstance.AssignedToUserIDs = originalBackwardToActivityInstance.AssignedToUserIDs;      //多实例节点为单一用户任务
            rollbackPreviousActivityInstance.AssignedToUserNames = originalBackwardToActivityInstance.AssignedToUserNames;

            //插入新活动实例数据
            base.ActivityInstanceManager.Insert(rollbackPreviousActivityInstance,
                session);

            //创建新任务数据
            base.CreateNewTask(rollbackPreviousActivityInstance, activityResource, session);
        }
    }
}

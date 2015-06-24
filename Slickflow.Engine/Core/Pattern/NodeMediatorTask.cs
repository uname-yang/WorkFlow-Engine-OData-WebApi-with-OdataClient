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
using Slickflow.Engine.Core.Result;

namespace Slickflow.Engine.Core.Pattern
{
    /// <summary>
    /// 任务节点执行器
    /// </summary>
    internal class NodeMediatorTask : NodeMediator
    {
        internal NodeMediatorTask(ActivityForwardContext forwardContext, IDbSession session)
            : base(forwardContext, session)
        {

        }

        internal NodeMediatorTask(IDbSession session)
            : base(session)
        {

        }

        /// <summary>
        /// 执行普通任务节点
        /// 1. 当设置任务完成时，同时设置活动完成
        /// 2. 当实例化活动数据时，产生新的任务数据
        /// </summary>
        internal override void ExecuteWorkItem()
        {
            try
            {
                //完成当前的任务节点
                bool canContinueForwardCurrentNode = CompleteWorkItem(ActivityForwardContext.TaskView,
                    ActivityForwardContext.ActivityResource,
                    this.Session);

                if (canContinueForwardCurrentNode)
                {
                    bool isJumpforward = ActivityForwardContext.TaskView == null ? true : false;
                    //获取下一步节点列表：并继续执行
                    ContinueForwardCurrentNode(isJumpforward);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 完成任务实例
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>        
        internal bool CompleteWorkItem(TaskViewEntity taskView,
            ActivityResource activityResource,
            IDbSession session)
        {
            bool canContinueForwardCurrentNode = true;

            //流程强制拉取向前跳转时，没有运行人的任务实例
            if (taskView != null)
            {
                //完成本任务，返回任务已经转移到下一个会签任务，不继续执行其它节点
                base.TaskManager.Complete(taskView.TaskID, activityResource.AppRunner, session);
            }

            //设置活动节点的状态为完成状态
            base.ActivityInstanceManager.Complete(base.Linker.FromActivityInstance.ID,
                activityResource.AppRunner,
                session);

            base.Linker.FromActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;

            //多实例会签和加签处理
            //先判断是否是会签和加签类型
            //主节点不为空时不发起加签可以正常运行
            var complexType = base.Linker.FromActivity.ActivityTypeDetail.ComplexType;
            if (complexType == ComplexTypeEnum.SignTogether
                || complexType == ComplexTypeEnum.SignForward && base.Linker.FromActivityInstance.MIHostActivityInstanceID != null)   
            {
                //取出主节点信息
                var mainNodeIndex = base.Linker.FromActivityInstance.MIHostActivityInstanceID.Value;
                var mainActivityInstance = base.ActivityInstanceManager.GetById(mainNodeIndex);

                //取出处于多实例节点列表
                var sqList = base.ActivityInstanceManager.GetActivityMulitipleInstanceWithState(
                    mainNodeIndex,
                    base.Linker.FromActivityInstance.ProcessInstanceID,
                    (short)ActivityStateEnum.Suspended,
                    session).ToList<ActivityInstanceEntity>();

                //串行会签和并行会签的处理
                if (complexType == ComplexTypeEnum.SignTogether)
                {
                    if (base.Linker.FromActivity.ActivityTypeDetail.MergeType == MergeTypeEnum.Sequence)    //串行会签处理
                    {
                        short maxOrder = 0;
                        if (sqList != null && sqList.Count > 0)
                        {
                            //取出最大执行节点
                            maxOrder = (short)sqList.Max<ActivityInstanceEntity>(t => t.CompleteOrder.Value);
                        }
                        else
                        {
                            //最后一个执行节点
                            maxOrder = (short)base.Linker.FromActivityInstance.CompleteOrder.Value;
                        }

                        //串行会签通过率（按人数判断）
                        if (mainActivityInstance.CompleteOrder != null && mainActivityInstance.CompleteOrder <= maxOrder)
                        {
                            maxOrder = (short)mainActivityInstance.CompleteOrder;
                        }

                        if (base.Linker.FromActivityInstance.CompleteOrder < maxOrder)
                        {
                            //设置下一个任务进入准备状态
                            var currentNodeIndex = (short)base.Linker.FromActivityInstance.CompleteOrder.Value;
                            var nextActivityInstance = sqList[0];     //始终取第一条挂起实例
                            nextActivityInstance.ActivityState = (short)ActivityStateEnum.Ready;
                            base.ActivityInstanceManager.Update(nextActivityInstance, session);

                            canContinueForwardCurrentNode = false;
                            base.WfNodeMediatedResult.Feedback = WfNodeMediatedFeedback.ForwardToNextSequenceTask;
                        }
                        else if (base.Linker.FromActivityInstance.CompleteOrder == maxOrder)
                        {
                            //完成最后一个会签任务，会签主节点状态由挂起设置为准备状态
                            mainActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                            base.ActivityInstanceManager.Update(mainActivityInstance, session);
                        }
                    }
                    else if (base.Linker.FromActivity.ActivityTypeDetail.MergeType == MergeTypeEnum.Parallel)   //并行会签处理
                    {
						if (mainActivityInstance.CompleteOrder == null)//并行会签未设置通过率的判断
                            mainActivityInstance.CompleteOrder = 1;
                        var allCount = sqList.Count();
                        var completedCount = sqList.Where<ActivityInstanceEntity>(w => w.ActivityState == (short)ActivityStateEnum.Completed)
                            .ToList<ActivityInstanceEntity>()
                            .Count();
                        if ((completedCount * 0.01) / (allCount * 0.01) >= mainActivityInstance.CompleteOrder)
                        {
                            //如果超过约定的比例数，则执行下一步节点
                            mainActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                            base.ActivityInstanceManager.Update(mainActivityInstance, session);
                        }
                        else
                        {
                            canContinueForwardCurrentNode = false;
                            base.WfNodeMediatedResult.Feedback = WfNodeMediatedFeedback.WaitingForCompletedMore;
                        }

                    }
                }
                else if (complexType == ComplexTypeEnum.SignForward)                            //加签的处理
                {
                    //判断加签是否全部完成，如果是，则流转到下一步，否则不能流转
                    var signforwardType = (SignForwardTypeEnum)Enum.Parse(typeof(SignForwardTypeEnum), 
                        base.ActivityForwardContext.FromActivityInstance.SignForwardType.Value.ToString());

                    if (signforwardType == SignForwardTypeEnum.SignForwardBehind
                        || signforwardType == SignForwardTypeEnum.SignForwardBefore)
                    {
                        short maxOrder = 0;
                        if (sqList != null && sqList.Count > 0)
                        {
                            //取出最大执行节点
                            maxOrder = (short)sqList.Max<ActivityInstanceEntity>(t => t.CompleteOrder.Value);
                        }
                        else
                        {
                            //最后一个执行节点
                            maxOrder = (short)base.Linker.FromActivityInstance.CompleteOrder.Value;
                        }

                        //加签通过率
                        if (mainActivityInstance.CompleteOrder != null)
                        {
                            maxOrder = (short)mainActivityInstance.CompleteOrder;
                        }

                        if (base.Linker.FromActivityInstance.CompleteOrder < maxOrder)
                        {
                            //设置下一个节点进入等待办理状态
                            var currentNodeIndex = (short)base.Linker.FromActivityInstance.CompleteOrder.Value;
                            var nextActivityInstance = sqList[0];
                            nextActivityInstance.ActivityState = (short)ActivityStateEnum.Ready;
                            base.ActivityInstanceManager.Update(nextActivityInstance, session);

                            canContinueForwardCurrentNode = false;
                            base.WfNodeMediatedResult.Feedback = WfNodeMediatedFeedback.ForwardToNextSequenceTask;
                        }
                        else if (base.Linker.FromActivityInstance.CompleteOrder == maxOrder)
                        {
                            //最后一个节点执行完，主节点进入完成状态，整个流程向下执行
                            mainActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                            base.ActivityInstanceManager.Update(mainActivityInstance, session);
                        }
                    }
                    else if (signforwardType == SignForwardTypeEnum.SignForwardParallel)
                    {
                        //并行加签，按照通过率来决定是否标识当前节点完成
                        var allCount = sqList.Count();
                        var completedCount = sqList.Where<ActivityInstanceEntity>(w => w.ActivityState == (short)ActivityStateEnum.Completed)
                            .ToList<ActivityInstanceEntity>()
                            .Count();

                        if ((completedCount * 0.01) / (allCount * 0.01) >= mainActivityInstance.CompleteOrder)
                        {
                            base.ActivityForwardContext.FromActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                            mainActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                            base.ActivityInstanceManager.Update(mainActivityInstance, base.Session);
                        }
                        else
                        {
                            canContinueForwardCurrentNode = false;
                            base.WfNodeMediatedResult.Feedback = WfNodeMediatedFeedback.WaitingForCompletedMore;
                        }
                    }
                }
            }
            return canContinueForwardCurrentNode;
        }

        /// <summary>
        /// 创建活动任务转移实例数据
        /// </summary>
        /// <param name="toActivity"></param>
        /// <param name="processInstance"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="transitionGUID"></param>
        /// <param name="transitionType"></param>
        /// <param name="flyingType"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal override void CreateActivityTaskTransitionInstance(ActivityEntity toActivity,
            ProcessInstanceEntity processInstance,
            ActivityInstanceEntity fromActivityInstance,
            string transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            ActivityResource activityResource,
            IDbSession session)
        {
            //判断是否是会签节点，如果是创建会签节点
            if (toActivity.ActivityTypeDetail != null
                && toActivity.ActivityTypeDetail.ComplexType == ComplexTypeEnum.SignTogether)
            {
                //创建会签节点的主节点，以及会签主节点下的实例子节点记录
                CreateMultipleInstance(toActivity, processInstance, fromActivityInstance,
                    transitionGUID, transitionType, flyingType, activityResource, session);
            }
            else
            {
                //实例化Activity
                var toActivityInstance = base.CreateActivityInstanceObject(toActivity, processInstance, activityResource.AppRunner);

                //进入运行状态
                toActivityInstance.ActivityState = (short)ActivityStateEnum.Ready;
                toActivityInstance.AssignedToUserIDs = GenerateActivityAssignedUserIDs(
                    activityResource.NextActivityPerformers[toActivity.ActivityGUID]);
                toActivityInstance.AssignedToUserNames = GenerateActivityAssignedUserNames(
                    activityResource.NextActivityPerformers[toActivity.ActivityGUID]);

                //插入活动实例数据
                base.ActivityInstanceManager.Insert(toActivityInstance, session);

                //插入任务数据
                base.CreateNewTask(toActivityInstance, activityResource, session);

                //插入转移数据
                InsertTransitionInstance(processInstance,
                    transitionGUID,
                    fromActivityInstance,
                    toActivityInstance,
                    transitionType,
                    flyingType,
                    activityResource.AppRunner,
                    session);
            }
        }
    }
}

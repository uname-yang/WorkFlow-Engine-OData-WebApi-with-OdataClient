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
using Slickflow.Data;
using Slickflow.Engine.Common;
using Slickflow.Engine.Utility;
using Slickflow.Engine.Xpdl;
using Slickflow.Engine.Xpdl.Node;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Business.Manager;
using Slickflow.Engine.Core.Result;

namespace Slickflow.Engine.Core.Pattern
{
    /// <summary>
    /// 节点执行器的抽象类
    /// </summary>
    internal abstract class NodeMediator 
    {
        #region 属性列表
        private ActivityForwardContext _activityForwardContext;
        protected ActivityForwardContext ActivityForwardContext
        {
            get
            {
                return _activityForwardContext;
            }
        }

        private BackwardContext _backwardContext;
        protected BackwardContext BackwardContext
        {
            get
            {
                return _backwardContext;
            }
        }

        private IDbSession _session;
        protected IDbSession Session
        {
            get
            {
                return _session;
            }
        }

        private Linker _linker;
        internal Linker Linker
        {
            get
            {
                if (_linker == null)
                    _linker = new Linker();

                return _linker;
            }
        }

        /// <summary>
        /// 活动节点实例管理对象
        /// </summary>
        private ActivityInstanceManager activityInstanceManager;
        internal ActivityInstanceManager ActivityInstanceManager
        {
            get
            {
                if (activityInstanceManager == null)
                {
                    activityInstanceManager = new ActivityInstanceManager();
                }
                return activityInstanceManager;
            }
        }

        private TaskManager taskManager;
        internal TaskManager TaskManager
        {
            get
            {
                if (taskManager == null)
                {
                    taskManager = new TaskManager();
                }
                return taskManager;
            }
        }

        private ProcessInstanceManager processInstanceManager;
        internal ProcessInstanceManager ProcessInstanceManager
        {
            get
            {
                if (processInstanceManager == null)
                    processInstanceManager = new ProcessInstanceManager();
                return processInstanceManager;
            }
        }

        private WfNodeMediatedResult _wfNodeMediatedResult;
        internal WfNodeMediatedResult WfNodeMediatedResult
        {
            get
            {
                if (_wfNodeMediatedResult == null)
                {
                    _wfNodeMediatedResult = new WfNodeMediatedResult();
                }
                return _wfNodeMediatedResult;
            }
        }

        #endregion

        #region 抽象方法列表
        /// <summary>
        /// 执行节点方法
        /// </summary>
        internal abstract void ExecuteWorkItem();
        #endregion
        /// <summary>
        /// 向前流转时的NodeMediator的构造函数
        /// </summary>
        /// <param name="forwardContext"></param>
        /// <param name="session"></param>
        internal NodeMediator(ActivityForwardContext forwardContext, IDbSession session)
        {
            _activityForwardContext = forwardContext;
            _session = session;
            Linker.FromActivity = forwardContext.Activity;
        }

        /// <summary>
        /// 退回处理时的NodeMediator的构造函数
        /// </summary>
        /// <param name="backwardContext"></param>
        /// <param name="session"></param>
        internal NodeMediator(BackwardContext backwardContext, IDbSession session)
        {
            _session = session;
            _backwardContext = backwardContext;
            Linker.FromActivity = backwardContext.BackwardFromActivity;
        }

        internal NodeMediator(IDbSession session)
        {
            _session = session;
        }

        #region 流程执行逻辑
        /// <summary>
        /// 遍历执行当前节点后面的节点
        /// </summary>
        /// <param name="previousActivityInstance"></param>
        /// <param name="currentNode"></param>
        internal virtual void ContinueForwardCurrentNode(bool isJumpforward)
        {
            var activityResource = ActivityForwardContext.ActivityResource;

            if (isJumpforward == false)
            {
                //非跳转模式的正常运行
                var nextActivityMatchedResult = this.ActivityForwardContext.ProcessModel.GetNextActivityList(
                    this.Linker.FromActivityInstance.ActivityGUID,
                    activityResource.ConditionKeyValuePair,
                    activityResource,
                    (a, b) => a.NextActivityPerformers.ContainsKey(b.ActivityGUID));

                if (nextActivityMatchedResult.MatchedType != NextActivityMatchedType.Successed
                    || nextActivityMatchedResult.Root.HasChildren == false)
                {
                    throw new WfRuntimeException("没有匹配的后续流转节点，流程虽然能处理当前节点，但是无法流转到下一步！");
                }

                ContinueForwardCurrentNodeRecurisivly(this.Linker.FromActivity,
                    this.Linker.FromActivityInstance,
                    nextActivityMatchedResult.Root,
                    activityResource.ConditionKeyValuePair,
                    isJumpforward);
            }
            else
            {
                //跳转模式运行
                var root = NextActivityComponentFactory.CreateNextActivityComponent();
                var nextActivityComponent = NextActivityComponentFactory.CreateNextActivityComponent(
                    this.Linker.FromActivity,
                    this.Linker.ToActivity);
                
                root.Add(nextActivityComponent);

                ContinueForwardCurrentNodeRecurisivly(this.Linker.FromActivity,
                    this.Linker.FromActivityInstance,
                    root,
                    activityResource.ConditionKeyValuePair,
                    isJumpforward);
            }
        }

        /// <summary>
        /// 递归执行节点
        /// 1)创建普通节点的任务
        /// 2)创建会签节点的任务
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fromNode"></param>
        /// <param name="conditionKeyValuePair"></param>
        protected void ContinueForwardCurrentNodeRecurisivly(ActivityEntity fromActivity,
            ActivityInstanceEntity fromActivityInstance,
            NextActivityComponent root,
            IDictionary<string, string> conditionKeyValuePair,
            Boolean isJumpforward)
        {
            foreach (NextActivityComponent comp in root)
            {
                if (comp.HasChildren)
                {
                    //此节点类型为分支或合并节点类型：首先需要实例化当前节点(自动完成)
                    NodeMediatorGateway gatewayNodeMediator = NodeMediatorGatewayFactory.CreateGatewayNodeMediator(comp.Activity, 
                        this.ActivityForwardContext.ProcessModel, 
                        Session);

                    ICompleteAutomaticlly autoGateway = (ICompleteAutomaticlly)gatewayNodeMediator;
                    GatewayExecutedResult gatewayResult = autoGateway.CompleteAutomaticlly(
                        ActivityForwardContext.ProcessInstance,
                        comp.Transition.TransitionGUID,
                        fromActivityInstance,
                        ActivityForwardContext.ActivityResource,
                        Session);

                    if (gatewayResult.Status == GatewayExecutedStatus.Successed)
                    {
                        //遍历后续子节点
                        ContinueForwardCurrentNodeRecurisivly(fromActivity,
                            gatewayNodeMediator.GatewayActivityInstance,
                            comp,
                            conditionKeyValuePair,
                            isJumpforward);
                    }
                    else
                    {
                        WfRuntimeException e = new WfRuntimeException("第一个满足条件的节点已经被成功执行，此后的节点被阻止在XOrJoin节点!");
                        LogManager.RecordLog("递归执行节点方法异常", LogEventType.Exception, LogPriority.Normal, null, e);
                    }
                }
                else if (comp.Activity.IsSimpleWorkItem)
                {
                    //此节点类型为任务节点：根据fromActivityInstance的类型判断是否可以创建任务
                    if (fromActivityInstance.ActivityState == (short)ActivityStateEnum.Completed)
                    {
                        //创建新任务节点
                        NodeMediator taskNodeMediator = new NodeMediatorTask(Session);
                        taskNodeMediator.CreateActivityTaskTransitionInstance(comp.Activity,
                            ActivityForwardContext.ProcessInstance,
                            fromActivityInstance,
                            comp.Transition.TransitionGUID,
                            comp.Transition.DirectionType == TransitionDirectionTypeEnum.Loop ?
                                TransitionTypeEnum.Loop : TransitionTypeEnum.Forward, //根据Direction方向确定是否是自身循环
                            isJumpforward == true ? 
                                TransitionFlyingTypeEnum.ForwardFlying : TransitionFlyingTypeEnum.NotFlying,
                            ActivityForwardContext.ActivityResource,
                            Session);
                    }
                    else
                    {
                        //下一步的任务节点没有创建，需给出提示信息
                        if ((fromActivity.GatewayDirectionType | GatewayDirectionEnum.AllJoinType)
                            == GatewayDirectionEnum.AllJoinType)
                        {
                            WfRuntimeException e = new WfRuntimeException("等待其它需要合并的分支!");
                            LogManager.RecordLog("递归执行节点方法异常", LogEventType.Exception, LogPriority.Normal, null, e);
                        }
                    }
                }
                else if (comp.Activity.ActivityType == ActivityTypeEnum.SubProcessNode)
                {
                    //节点类型为subprocessnode
                    if (fromActivityInstance.ActivityState == (short)ActivityStateEnum.Completed)
                    {
                        //实例化subprocess节点数据
                        NodeMediator subNodeMediator = new NodeMediatorSubProcess(Session);
                        subNodeMediator.CreateActivityTaskTransitionInstance(comp.Activity,
                            ActivityForwardContext.ProcessInstance,
                            fromActivityInstance,
                            comp.Transition.TransitionGUID,
                            comp.Transition.DirectionType == TransitionDirectionTypeEnum.Loop ?
                                TransitionTypeEnum.Loop : TransitionTypeEnum.Forward,
                            TransitionFlyingTypeEnum.NotFlying,
                            ActivityForwardContext.ActivityResource,
                            Session);
                    }
                }
                else if (comp.Activity.ActivityType == ActivityTypeEnum.EndNode)
                {
                    //此节点为完成结束节点，结束流程
                    var endMediator = new NodeMediatorEnd(ActivityForwardContext, Session);
                    endMediator.Linker.ToActivity = comp.Activity;
                    endMediator.CompleteAutomaticlly(ActivityForwardContext.ProcessInstance,
                        comp.Transition.TransitionGUID,
                        fromActivityInstance,
                        ActivityForwardContext.ActivityResource,
                        Session);
                }
                else
                {
                    WfRuntimeException e = new WfRuntimeException(string.Format("XML文件定义了未知的节点类型，执行失败，节点类型信息：{0}",
                        comp.Activity.ActivityType.ToString()));
                    LogManager.RecordLog("递归执行节点方法异常", LogEventType.Exception, LogPriority.Normal, null, e);
                }
            }
        }

        /// <summary>
        /// 创建工作项及转移数据
        /// </summary>
        /// <param name="toActivity"></param>
        /// <param name="processInstance"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="transitionGUID"></param>
        /// <param name="transitionType"></param>
        /// <param name="flyingType"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal virtual void CreateActivityTaskTransitionInstance(ActivityEntity toActivity,
            ProcessInstanceEntity processInstance,
            ActivityInstanceEntity fromActivityInstance,
            String transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            ActivityResource activityResource,
            IDbSession session){ }

        /// <summary>
        /// 创建任务的虚方法
        /// 1. 对于自动执行的工作项，无需重写该方法
        /// 2. 对于人工执行的工作项，需要重写该方法，插入待办的任务数据
        /// </summary>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        internal virtual void CreateNewTask(ActivityInstanceEntity toActivityInstance,
            ActivityResource activityResource,
            IDbSession session)
        {
            if (activityResource.NextActivityPerformers == null)
            {
                throw new WorkflowException("无法创建任务，流程流转下一步的办理人员不能为空！");
            }

            TaskManager.Insert(toActivityInstance,
                activityResource.NextActivityPerformers[toActivityInstance.ActivityGUID],
                activityResource.AppRunner,
                session);
        }

        /// <summary>
        /// 创建节点对象
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        protected ActivityInstanceEntity CreateActivityInstanceObject(ActivityEntity activity,
            ProcessInstanceEntity processInstance,
            WfAppRunner runner)
        {
            ActivityInstanceEntity entity = ActivityInstanceManager.CreateActivityInstanceObject(processInstance.AppName,
                processInstance.AppInstanceID,
                processInstance.ID,
                activity,
                runner);

            return entity;
        }

        /// <summary>
        /// 会签类型的主节点, 多实例节点处理
        /// 创建会签节点的主节点，以及会签主节点下的实例子节点记录
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromToTransition"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CreateMultipleInstance(ActivityEntity toActivity,
            ProcessInstanceEntity processInstance,
            ActivityInstanceEntity fromActivityInstance,
            String transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            ActivityResource activityResource,
            IDbSession session)
        {
            //实例化主节点Activity
            var toActivityInstance = CreateActivityInstanceObject(toActivity,
                processInstance, activityResource.AppRunner);

            //更新主节点实例数据
            toActivityInstance.ActivityState = (short)ActivityStateEnum.Suspended;
            toActivityInstance.ComplexType = (short)ComplexTypeEnum.SignTogether;

            //if (toActivity.ActivityTypeDetail.MergeType == MergeTypeEnum.Parallel)
            //{
            toActivityInstance.CompleteOrder = toActivity.ActivityTypeDetail.CompleteOrder;
            //}
            toActivityInstance.AssignedToUserIDs = GenerateActivityAssignedUserIDs(
                activityResource.NextActivityPerformers[toActivity.ActivityGUID]);
            toActivityInstance.AssignedToUserNames = GenerateActivityAssignedUserNames(
                activityResource.NextActivityPerformers[toActivity.ActivityGUID]);

            //插入主节点实例数据
            ActivityInstanceManager.Insert(toActivityInstance, session);

            //插入主节点转移数据
            InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                activityResource.AppRunner,
                session);

            //插入会签子节点实例数据
            var plist = activityResource.NextActivityPerformers[toActivity.ActivityGUID];
            ActivityInstanceEntity entity = new ActivityInstanceEntity();
            for (short i = 0; i < plist.Count; i++)
            {
                entity = ActivityInstanceManager.CreateActivityInstanceObject(toActivityInstance);
                entity.AssignedToUserIDs = plist[i].UserID;
                entity.AssignedToUserNames = plist[i].UserName;
                entity.MIHostActivityInstanceID = toActivityInstance.ID;
                entity.CompleteOrder = (short)(i + 1);

                //如果是串行会签，只有第一个节点处于运行状态，其它节点挂起
                if ((i > 0) && (toActivity.ActivityTypeDetail.MergeType == MergeTypeEnum.Sequence))
                {
                    entity.ActivityState = (short)ActivityStateEnum.Suspended;
                }

                //插入活动实例数据，并返回活动实例ID
                entity.ID = ActivityInstanceManager.Insert(entity, session);

                //插入任务数据
                TaskManager.Insert(entity, plist[i], activityResource.AppRunner, session);
            }
        }

        /// <summary>
        /// 创建退回类型的活动实例对象
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="backSrcActivityInstanceID">退回的活动实例ID</param>
        /// <param name="logonUser">登录用户</param>
        /// <returns></returns>
        protected ActivityInstanceEntity CreateBackwardToActivityInstanceObject(ProcessInstanceEntity processInstance,
            BackwardTypeEnum backwardType,
            int backSrcActivityInstanceID,
            WfAppRunner runner)
        {
            ActivityInstanceEntity entity = ActivityInstanceManager.CreateBackwardActivityInstanceObject(
                processInstance.AppName,
                processInstance.AppInstanceID,
                processInstance.ID,
                this.BackwardContext.BackwardToTaskActivity,
                backwardType,
                backSrcActivityInstanceID,
                runner);

            return entity;
        }

        /// <summary>
        /// 插入连线实例的方法
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromToTransition"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="toActivityInstance"></param>
        /// <param name="conditionParseResult"></param>
        /// <param name="wfLinqDataContext"></param>
        /// <returns></returns>
        internal virtual void InsertTransitionInstance(ProcessInstanceEntity processInstance,
            String transitionGUID,
            ActivityInstanceEntity fromActivityInstance,
            ActivityInstanceEntity toActivityInstance,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            WfAppRunner runner,
            IDbSession session)
        {
            var tim = new TransitionInstanceManager();
            var transitionInstanceObject = tim.CreateTransitionInstanceObject(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                runner,
                (byte)ConditionParseResultEnum.Passed);

            tim.Insert(session.Connection, transitionInstanceObject, session.Transaction);
        }

        /// <summary>
        /// 生成任务办理人ID字符串列表
        /// </summary>
        /// <param name="performerList"></param>
        /// <returns></returns>
        protected string GenerateActivityAssignedUserIDs(PerformerList performerList)
        {
            StringBuilder strBuilder = new StringBuilder(1024);
            foreach (var performer in performerList)
            {
                if (strBuilder.ToString() != "")
                    strBuilder.Append(",");
                strBuilder.Append(performer.UserID);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 生成办理人名称的字符串列表
        /// </summary>
        /// <param name="performerList"></param>
        /// <returns></returns>
        protected string GenerateActivityAssignedUserNames(PerformerList performerList)
        {
            StringBuilder strBuilder = new StringBuilder(1024);
            foreach (var performer in performerList)
            {
                if (strBuilder.ToString() != "")
                    strBuilder.Append(",");
                strBuilder.Append(performer.UserName);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 由节点分配的人员信息生成PerformerList数据结构
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <returns></returns>
        protected PerformerList AntiGenerateActivityPerformerList(ActivityInstanceEntity activityInstance)
        {
            var performerList = new PerformerList();

            if (!string.IsNullOrEmpty(activityInstance.AssignedToUserIDs)
                && !string.IsNullOrEmpty(activityInstance.AssignedToUserNames))
            {
                var assignedToUserIDs = activityInstance.AssignedToUserIDs.Split(',');
                var assignedToUserNames = activityInstance.AssignedToUserNames.Split(',');

                for (var i = 0; i < assignedToUserIDs.Count(); i++)
                {
                    performerList.Add(new Performer(assignedToUserIDs[i], assignedToUserNames[i]));
                }
            }
            return performerList;
        }
        #endregion

        /// <summary>
        /// 根据节点执行结果类型，生成消息
        /// </summary>
        /// <returns></returns>
        internal string GetNodeMediatedMessage()
        {
            var message = string.Empty;
            if (WfNodeMediatedResult.Feedback == WfNodeMediatedFeedback.ForwardToNextSequenceTask)
            {
                message = "串行会(加)签，设置下一个执行节点的任务进入运行状态！";
            }
            else if (WfNodeMediatedResult.Feedback == WfNodeMediatedFeedback.WaitingForCompletedMore)
            {
                message = "并行会(加)签，等待节点到达足够多的完成比例！";
            }

            return message;
        }
    }
}

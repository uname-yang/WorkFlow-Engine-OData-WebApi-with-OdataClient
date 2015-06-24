using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slickflow.Engine.Common;
using Slickflow.Engine.Xpdl.Node;

namespace Slickflow.Engine.Xpdl
{
    /// <summary>
    /// 节点类型详细信息
    /// </summary>
    public class ActivityTypeDetail
    {
        public ActivityTypeEnum ActivityType { get; set; }
        public ComplexTypeEnum ComplexType { get; set; }
        public MergeTypeEnum MergeType { get; set; }
        public Nullable<float> CompleteOrder { get; set; }
        public SignForwardTypeEnum SignForwardType { get; set; }
        public SkipInfo SkipInfo { get; set; }
    }

    /// <summary>
    /// 节点的其它附属类型
    /// </summary>
    public enum ComplexTypeEnum
    {
        /// <summary>
        /// 多实例-会签节点
        /// </summary>
        SignTogether = 1,

        /// <summary>
        /// 多实例-加签节点
        /// </summary>
        SignForward = 2
    }

    /// <summary>
    /// 会签节点合并类型
    /// </summary>
    public enum MergeTypeEnum
    {
        /// <summary>
        /// 串行
        /// </summary>
        Sequence = 1,

        /// <summary>
        /// 并行
        /// </summary>
        Parallel = 2
    }

    /// <summary>
    /// 加签类型
    /// </summary>
    public enum SignForwardTypeEnum
    {
        /// <summary>
        /// 前加签
        /// </summary>
        SignForwardBefore = 1,

        /// <summary>
        /// 后加签
        /// </summary>
        SignForwardBehind = 2,

        /// <summary>
        /// 并行加签
        /// </summary>
        SignForwardParallel = 3
    }

    /// <summary>
    /// 节点类型上描述的跳转信息
    /// </summary>
    public class SkipInfo
    {
        public Boolean IsSkip { get; set; }
        public string Skipto { get; set; }
    }
}

using System;

namespace JW.RequestRelay.Util.Models.Input
{
    /// <summary>
    /// 数据创建时间 
    /// </summary>
    public interface ICreateTimeInput
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime? EndTime { get; set; }
    }
}

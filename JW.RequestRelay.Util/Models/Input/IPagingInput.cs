namespace JW.RequestRelay.Util.Models.Input
{
    /// <summary>
    /// 分页
    /// </summary>
    public interface IPagingInput
    {
        /// <summary>
        /// 页码
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// 返回数据条数
        /// </summary>
        int Limit { get; set; }

        /// <summary>
        /// 输出参数 不需要传入
        /// </summary>
        long Total { get; set; }
    }
}

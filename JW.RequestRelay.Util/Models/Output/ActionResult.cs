using System;

namespace JW.RequestRelay.Util.Models.Output
{
    public partial class ActionResult
    {
        public ActionResult()
        {
            this.Status = true;
        }
        /// <summary>
        /// 业务执行结果
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        ///消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 异常编码
        /// </summary>
        public ValueType Code { get; set; }
    }

    public partial class ActionResult<T>
    {
        public ActionResult()
        {
            this.Status = true;
        }
        /// <summary>
        /// 业务执行结果
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 异常编码
        /// </summary>
        public ValueType Code { get; set; }
    }
}

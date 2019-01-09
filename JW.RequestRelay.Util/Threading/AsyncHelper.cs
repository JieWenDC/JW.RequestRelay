using System;
using System.Reflection;
using System.Threading.Tasks;
using Nito.AsyncEx;
using System.Threading;
using JW.RequestRelay.Util.Logging;

namespace JW.RequestRelay.Util.Threading
{
    /// <summary>
    /// Provides some helper methods to work with async methods.
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// 检查指定方法是不是异步方法
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        /// <summary>
        /// 以任务的形式运行指定委托方法
        /// 不引发异常，异步执行
        /// </summary>
        /// <param name="action"></param>
        public static void TaskRun(WaitCallback action, object state)
        {
            ThreadPool.QueueUserWorkItem((_state) =>
            {
                try
                {
                    action(_state);
                }
                catch (Exception ex)
                {
                    Log4netHelper.Fatal(ex);
                }
            }, state);
        }

        /// <summary>
        /// 以任务的形式运行指定委托方法
        /// 不引发异常，异步执行
        /// </summary>
        /// <param name="action"></param>
        public static void TaskRun(Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log4netHelper.Fatal(ex);
                }
            });
        }

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }

        /// <summary>
        /// 获取当前线程信息
        /// </summary>
        /// <returns></returns>
        public static string GetThreadInfo(string sign)
        {
            var currentThread = System.Threading.Thread.CurrentThread;

            int workerThreads = 0;//可用辅助线程的数目
            int completionPortThreads = 0;//可用异步 I/O 线程的数目
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);

            int completionPortThreadsMax = 0;//线程池中异步 I/O 线程的最大数目
            int workerThreadsMax = 0;//线程池中辅助线程的最大数目
            ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);

            return string.Format("Time:{0} No:{1:00} ThreadCount：{2:00} {3} {4}",
                DateTime.Now.Ticks,
                currentThread.ManagedThreadId,
                 workerThreadsMax - workerThreads, sign, Environment.NewLine);
        }
    }
}

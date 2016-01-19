using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aton.AtonSocket.Core.Utility
{
    /// <summary>
    /// 异步扩展类
    /// </summary>
    public static class AsyncUtility
    {
        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static Task Run(Action task)
        {
            return Run(task, TaskCreationOptions.None);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="taskOption">The task option.</param>
        /// <returns></returns>
        public static Task Run(Action task, TaskCreationOptions taskOption)
        {
            return Run(task, taskOption, null);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task Run(Action task, Action<Exception> exceptionHandler)
        {
            return Run(task, TaskCreationOptions.None, exceptionHandler);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="taskOption">The task option.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task Run(Action task, TaskCreationOptions taskOption, Action<Exception> exceptionHandler)
        {

            return Task.Factory.StartNew(task, taskOption).ContinueWith(t =>
                {
                    exceptionHandler(t.Exception.InnerException);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static Task Run(Action<object> task, object state)
        {
            return Run(task, state, TaskCreationOptions.None);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <param name="taskOption">The task option.</param>
        /// <returns></returns>
        public static Task Run(Action<object> task, object state, TaskCreationOptions taskOption)
        {
            return Run(task, state, taskOption, null);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task Run(Action<object> task, object state, Action<Exception> exceptionHandler)
        {
            return Run(task, state, TaskCreationOptions.None, exceptionHandler);
        }


        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <param name="taskOption">The task option.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task Run(Action<object> task, object state, TaskCreationOptions taskOption, Action<Exception> exceptionHandler)
        {
            return Task.Factory.StartNew(task, state, taskOption).ContinueWith(t =>
            {
                exceptionHandler(t.Exception.InnerException);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// 无限循环任务
        /// </summary>
        /// <param name="task">the task</param>
        /// <param name="state"></param>
        /// <param name="cancelToken"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        public static Task LongRun(Action<object> task, object state, System.Threading.CancellationToken cancelToken, Action<Exception> exceptionHandler)
        {
            return Task.Factory.StartNew(n=>
            {
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    task(state);
                }
            }, cancelToken, TaskCreationOptions.LongRunning).ContinueWith(t => { exceptionHandler(t.Exception.InnerException); });
        }

        /// <summary>
        /// 无限循环任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="state"></param>
        /// <param name="cancelToken"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        public static Task LongRun(Action<object> task, object state, System.Threading.CancellationToken cancelToken)
        {
            return Task.Factory.StartNew(n =>
            {
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    task(state);
                }
            }, cancelToken, TaskCreationOptions.LongRunning);
        }
    }
}

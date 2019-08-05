using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RequestTester.Managers
{

    public static class ThreadManager
    {
        public delegate void ResultCallback<TRESULT>(TRESULT result);
        public delegate void ResultCallback();

        /// <summary>
        /// Run cold tasks, <maxParallel> at same time
        /// </summary>
        /// <param name="tasks">cold (not runned) tasks</param>
        /// <param name="maxParallel">max parallel tasks</param>
        /// <param name="callback">callback on task complete</param>
        /// <param name="cancellationToken">cancellationToken</param>
        public static async Task Run(IEnumerable<Task> tasks, int maxParallel, ResultCallback callback, CancellationToken cancellationToken)
        {
            var taskList = new List<Task>(maxParallel);

            foreach (var task in tasks)
            {
                if (cancellationToken.IsCancellationRequested)
                    continue;

                taskList.Add(task);
                task.Start();
                if (taskList.Count == maxParallel)
                {
                    var result = await Task.WhenAny(taskList).ConfigureAwait(false);
                    taskList.Remove(result);

                    callback?.Invoke();
                }
            }

            while (taskList.Count > 0)
            {
                var result = await Task.WhenAny(taskList).ConfigureAwait(false);
                taskList.Remove(result);

                callback?.Invoke();
            }
        }

        /// <summary>
        /// Run cold tasks, <maxParallel> at same time
        /// </summary>
        /// <param name="tasks">cold (not runned) tasks</param>
        /// <param name="maxParallel">max parallel tasks</param>
        /// <param name="callback">callback on task complete</param>
        /// <param name="cancellationToken">cancellationToken</param>
        public static async Task Run<TRESULT>(IEnumerable<Task<TRESULT>> tasks, int maxParallel, ResultCallback<TRESULT> callback, CancellationToken cancellationToken)
        {
            var taskList = new List<Task<TRESULT>>(maxParallel);

            foreach (var task in tasks)
            {
                if (cancellationToken.IsCancellationRequested)
                    continue;

                taskList.Add(task);
                task.Start();
                if (taskList.Count == maxParallel)
                {
                    var result = await Task.WhenAny(taskList).ConfigureAwait(false);
                    taskList.Remove(result);

                    callback?.Invoke(result.Result);
                }
            }

            while(taskList.Count > 0)
            {
                var result = await Task.WhenAny(taskList).ConfigureAwait(false);
                taskList.Remove(result);

                callback?.Invoke(result.Result);
            }
        }
    }
}

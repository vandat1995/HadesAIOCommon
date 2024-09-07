using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace HadesAIOCommon.Concurrent
{
    public class HadesExecutor : IExecutor
    {
        public int MaxParallelism { get; set; }
        public int DelayTask { get; set; } = 100;
        public bool IsCompleted => finished;

        private readonly object mutex = new();
        private readonly Queue<HadesTask> tasksQueue = new();
        private readonly HashSet<HadesTask> runningTasks = new();
        private readonly ConcurrentQueue<HadesTask> completedTasks = new();

        private readonly int timeWait;
        private bool finished;

        public HadesExecutor()
        {
            timeWait = 20;
            MaxParallelism = Environment.ProcessorCount - 1;
        }
        public HadesExecutor(int maxParallelism) : this()
        {
            MaxParallelism = maxParallelism;
        }
        public HadesExecutor(int maxParallelism, int timeWait) : this()
        {
            MaxParallelism = maxParallelism;
            this.timeWait = timeWait;
        }
        public void Stop()
        {
            lock (mutex)
            {
                tasksQueue.Clear();
            }
        }
        public void Abort()
        {
        }

        public void Run()
        {
            finished = false;
            StartMonitoring();
            Task.Run(async () =>
            {
                while (tasksQueue.Count > 0)
                {
                    var hadesTask = tasksQueue.Dequeue();
                    lock (mutex)
                    {
                        runningTasks.Add(hadesTask);
                    }

                    if (hadesTask.Task != null)
                    {
                        hadesTask.Task.Start();
                        _ = hadesTask.Task.ContinueWith(taskResult => hadesTask.CompletedTaskCallback?.Invoke(hadesTask));
                        await Task.Delay(DelayTask);
                    }
                    while (runningTasks.Count == MaxParallelism)
                    {
                        await Task.Delay(timeWait);
                        //Thread.Sleep(timeWait);
                    }
                }
            });

            while (!finished)
            {
                Task.Delay(500).Wait();
            }
        }

        public void SubmitTask(Action action)
        {
            var t = new HadesTask(this)
            {
                Task = new Task(action)
            };
            SubmitTask(t);
        }
        private void SubmitTask(HadesTask hadesTask)
        {
            tasksQueue.Enqueue(hadesTask);
        }

        public void MakeCompleteTask(HadesTask hadesTask)
        {
            hadesTask.IsDone = true;
            completedTasks.Enqueue(hadesTask);
        }

        private void StartMonitoring()
        {
            Task.Run(async () =>
            {
                while (tasksQueue.Count > 0 || runningTasks.Count > 0 || !completedTasks.IsEmpty)
                {
                    while (!completedTasks.IsEmpty)
                    {
                        completedTasks.TryDequeue(out HadesTask hadesTask);
                        lock (mutex)
                        {
                            runningTasks.Remove(hadesTask);
                        }
                    }
                    await Task.Delay(timeWait);
                }
                finished = true;
            });
        }
        private static void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Thread-{Thread.CurrentThread.ManagedThreadId} - {msg}");
        }
    }
}

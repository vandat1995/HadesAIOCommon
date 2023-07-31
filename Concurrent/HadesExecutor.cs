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
        public int DelayTask { get; set; } = 10;
        public bool IsCompleted => finished;
        private bool isStopped = false;

        private readonly object MUTEX = new();
        private readonly Queue<HadesTask> tasksQueue = new();
        //private readonly HashSet<HadesTask> runningTasks = new();
        //private ConcurrentQueue<HadesTask> completedTasks = new();
        private readonly HashSet<string> runningTasks = new();
        private readonly ConcurrentQueue<string> completedTasks = new();

        private readonly int timeWait;
        private bool finished;

        public HadesExecutor()
        {
            timeWait = 10;
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

        public void Run()
        {
            if (isStopped)
            {
                return;
            }
            finished = false;
            StartMonitoring();
            Task.Run(async () =>
            {
                while (tasksQueue.Count > 0)
                {
                    var hadesTask = tasksQueue.Dequeue();
                    lock (MUTEX)
                    {
                        runningTasks.Add(hadesTask.Id);
                    }

                    if (hadesTask.Task != null)
                    {
                        hadesTask.Task.Start();
                        await Task.Delay(timeWait);
                    }

                    while (runningTasks.Count == MaxParallelism)
                    {
                        await Task.Delay(timeWait);
                    }
                }
            });
            while (!finished)
            {
                Thread.Sleep(timeWait);
            }
        }

        public void Stop()
        {
            lock (MUTEX)
            {
                isStopped = true;
                tasksQueue.Clear();
                //runningTasks.Clear();
                //completedTasks = new ConcurrentQueue<HadesTask>();
            }
        }
        public void Abort()
        {
        }

        public void SubmitTask(Action action)
        {
            var t = new HadesTask(this);
            action += () => t.CompletedTaskCallback?.Invoke(t);
            t.Task = new(action);
            SubmitTask(t);
        }
        public void SubmitTask(HadesTask hadesTask)
        {
            tasksQueue.Enqueue(hadesTask);
        }

        public void MakeCompleteTask(HadesTask hadesTask)
        {
            hadesTask.IsDone = true;
            completedTasks.Enqueue(hadesTask.Id);
        }


        private void StartMonitoring()
        {
            Task.Run(async () =>
            {
                while (tasksQueue.Count > 0 || runningTasks.Count > 0 || completedTasks.Count > 0)
                {
                    while (completedTasks.Count > 0)
                    {
                        var success = completedTasks.TryDequeue(out string hadesTask);
                        lock (MUTEX)
                        {
                            var isSuccess = runningTasks.Remove(hadesTask);
                            if (!isSuccess)
                            {
                            }
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

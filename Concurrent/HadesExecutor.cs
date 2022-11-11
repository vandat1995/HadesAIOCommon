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
        public bool IsCompleted => finished;

        private readonly object MUTEX = new();
        private readonly Queue<HadesTask> tasksQueue = new();
        private readonly HashSet<HadesTask> runningTasks = new();
        private ConcurrentQueue<HadesTask> completedTasks = new();

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
            finished = false;
            StartMonitor();
            while (tasksQueue.Count > 0)
            {
                var HadesTask = tasksQueue.Dequeue();

                lock (MUTEX)
                {
                    runningTasks.Add(HadesTask);
                }

                if (HadesTask.Task != null)
                {
                    HadesTask.Task.Start();
                    HadesTask.Task.ContinueWith(taskResult => HadesTask.CompletedTaskCallback?.Invoke(HadesTask));
                }

                while (runningTasks.Count == MaxParallelism)
                {
                    Thread.Sleep(timeWait);
                }
            }

            while (!finished)
            {
                Thread.Sleep(timeWait);
            }
        }
        public void Stop()
        {
            lock (MUTEX)
            {
                tasksQueue.Clear();
                runningTasks.Clear();
                completedTasks = new ConcurrentQueue<HadesTask>();
            }
        }

        public void Abort()
        {
        }

        public void SubmitTask(Action action)
        {
            HadesTask HadesTask = new(this)
            {
                Task = new Task(action)
            };
            SubmitTask(HadesTask);
        }
        public void SubmitTask(HadesTask hadesTask)
        {
            tasksQueue.Enqueue(hadesTask);
        }

        public void MakeCompleteTask(HadesTask hadesTask)
        {
            completedTasks.Enqueue(hadesTask);
        }


        private void StartMonitor()
        {
            Task.Run(async () =>
            {
                //Log("Begin monitor DaExecutor");
                while (tasksQueue.Count > 0 || runningTasks.Count > 0 || completedTasks.Count > 0)
                {
                    while (completedTasks.Count > 0)
                    {
                        bool success = completedTasks.TryDequeue(out HadesTask HadesTask);
                        lock (MUTEX)
                        {
                            runningTasks.Remove(HadesTask);
                        }
                    }
                    await Task.Delay(timeWait);
                }
                finished = true;
                //Log("Finish monitor DaExecutor");
            });
        }

        private static void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Thread-{Thread.CurrentThread.ManagedThreadId} - {msg}");
        }
    }
}

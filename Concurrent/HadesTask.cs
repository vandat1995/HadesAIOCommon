using System;
using System.Threading.Tasks;

namespace HadesAIOCommon.Concurrent
{
    public class HadesTask
    {
        public HadesTask(HadesExecutor executor)
        {
            CompletedTaskCallback = executor.MakeCompleteTask;
        }

        public bool IsDone { get; set; } = false;
        public string Id { get; } = Guid.NewGuid().ToString();
        public Task? Task { get; set; }

        public Action<HadesTask> CompletedTaskCallback { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HadesAIOCommon.Concurrent
{
    public class HadesTask
    {
        public HadesTask(IExecutor executor)
        {
            CompletedTaskCallback = executor.MakeCompleteTask;
        }

        public string Id { get; } = Guid.NewGuid().ToString();
        public Task? Task { get; set; }

        public Action<HadesTask> CompletedTaskCallback { get; }
    }
}

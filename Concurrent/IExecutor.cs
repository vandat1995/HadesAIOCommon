using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HadesAIOCommon.Concurrent
{
    public interface IExecutor
    {
        void Run();
        void Stop();
        void Abort();
        void SubmitTask(Action action);
        void MakeCompleteTask(HadesTask task);
    }
}

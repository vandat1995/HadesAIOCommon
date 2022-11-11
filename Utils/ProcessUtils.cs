using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace HadesAIOCommon.Utils
{
    public class ProcessUtils
    {
        public static Process? FindProcessByWindowTitle(string windowTitle, int maxTimes = 1)
        {
            for (int i = 0; i < maxTimes; i++)
            {
                var procs = Process.GetProcesses();
                var proc = procs.Where(p => p.MainWindowTitle == windowTitle)
                                .DefaultIfEmpty(null)
                                .FirstOrDefault();
                if (proc != null)
                {
                    return proc;
                }
                Thread.Sleep(1000);
            }
            return null;
        }

        public static void KillProcessTree(Process process)
        {
            KillProcessTree(process.Id);
        }
        public static void KillProcessTree(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                foreach (var item in processes)
                {
                    KillProcessTree(item.Id);
                }
            }
        }
        public static void KillProcessTree(int processId)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/PID {processId} /T /F",
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();
        }


    }
}

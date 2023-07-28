using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HadesAIOCommon.Utils
{
    public static class FileUtils
    {
        private static readonly object Mutex = new();

        public static IEnumerable<string> GetFilesInDirectory(string dir)
        {
            return Directory.GetFiles(dir)
                .Where(x => !Path.GetFileName(x).StartsWith("."));
        }

        public static bool IsEmpty(string pathToFile)
        {
            return !File.Exists(pathToFile) || new FileInfo(pathToFile).Length == 0;
        }

        public static void Delete(string pathToFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pathToFile) || !File.Exists(pathToFile)) return;
                File.Delete(pathToFile);
            }
            catch
            {
                // ignored
            }
        }

        public static void SafeAppendLine(string path, string content)
        {
            lock (Mutex)
            {
                File.AppendAllText(path, string.Concat(content, Environment.NewLine));
            }
        }

        public static void SafeAppendLines(string path, IEnumerable<string> contents)
        {
            lock (Mutex)
            {
                File.AppendAllLines(path, contents);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

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

        public static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            // Kiểm tra xem file zip có tồn tại không
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine("File zip không tồn tại.");
                return;
            }

            // Tạo thư mục đích nếu chưa tồn tại
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            // Giải nén file zip
            ZipFile.ExtractToDirectory(zipFilePath, extractPath, true);
            Console.WriteLine("Giải nén thành công.");
        }

        public static List<string> GetZipContents(string zipFilePath)
        {
            var entries = new List<string>();

            // Kiểm tra xem file zip có tồn tại không
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("File zip không tồn tại.", zipFilePath);
            }

            // Mở file zip để đọc
            using ZipArchive archive = ZipFile.OpenRead(zipFilePath);
            // Lấy danh sách các entry trong file zip
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                entries.Add(entry.FullName);
            }

            return entries;
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

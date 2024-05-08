using System;
using System.IO;

namespace HadesAIOCommon.Constants
{
    public static class HadesAIOConst
    {
        static HadesAIOConst()
        {
            Directory.CreateDirectory(OutputDir);
        }

        public static readonly string OutputDir = AppDomain.CurrentDomain.BaseDirectory + "output_data";

        public const string DEFAULT_CHROME_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36";

    }
}

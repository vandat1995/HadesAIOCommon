using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HadesAIOCommon.Caching
{
    public class Cache
    {
        private static readonly string dir = AppDomain.CurrentDomain.BaseDirectory + "cache" + Path.DirectorySeparatorChar;

        static Cache()
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

    }
}

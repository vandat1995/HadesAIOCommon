using System;
using System.IO;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl">time to live in minutes</param>
        public static void Put(string key, string value, int ttl = 0)
        {
            if (value == null)
            {
                return;
            }
            CacheData cacheData = new CacheData(key, value);
            if (ttl > 0)
            {
                cacheData.expires = cacheData.time.AddMinutes(ttl);
            }
            Write(key, cacheData.ToString());
        }

        public static string Get(string key, string? defaultValue = null)
        {
            string val = Read(key);
            if (string.IsNullOrWhiteSpace(val) && defaultValue != null)
            {
                return defaultValue;
            }
            return val;
        }
        public static T? Get<T>(string key, object? defaultValue = null)
        {
            string val = Read(key);
            if (string.IsNullOrEmpty(val))
            {
                if (defaultValue != null)
                {
                    return (T)defaultValue;
                }
                return default;
            }
            if (typeof(T).Equals(typeof(bool)))
            {
                val = val.ToLower();
            }
            return JSON.Parse<T>(val);
        }

        public static bool Has(string key)
        {
            string path = dir + key;
            if (File.Exists(path))
            {
                return true;
            }
            return false;
        }

        public static void Forget(string key)
        {
            string path = dir + key;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static DateTime GetLastWriteTime(string key)
        {
            string path = dir + key;
            if (File.Exists(path))
            {
                return File.GetLastWriteTime(path);
            }
            return DateTime.Now;
        }

        private static void Write(string key, string value)
        {
            string path = dir + key;
            File.WriteAllText(path, value);
        }
        private static string Read(string key)
        {
            string path = dir + key;
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            string cacheValue = File.ReadAllText(path);
            try
            {
                CacheData? cacheData = JSON.Parse<CacheData>(cacheValue);
                if (cacheData?.expires > DateTime.Now)
                {
                    return cacheData.value;
                }
                Forget(key);
            }
            catch (Exception)
            {
            }
            return cacheValue;
        }


    }

    
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.Caching
{
    public class CacheData
    {
        public CacheData(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        public string key { get; set; }
        public string value { get; set; }
        public DateTime time { get; set; } = DateTime.Now;
        public DateTime expires { get; set; } = DateTime.MaxValue;

        public override string ToString()
        {
            return JSON.Stringify(this);
        }
    }
}

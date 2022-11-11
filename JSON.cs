using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon
{
    public class JSON
    {
        public static string Stringify(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T? Parse<T>(object json) where T : class
        {
            if (json.GetType() == typeof(string))
            {
                return JsonConvert.DeserializeObject<T>((string)json);
            }
            return JsonConvert.DeserializeObject<T>(Stringify(json));
        }
    }
}

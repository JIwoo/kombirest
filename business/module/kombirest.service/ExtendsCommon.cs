using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kombirest.service
{
    public static class ExtendsCommon
    {
        public static List<dynamic> GetGeneric<T>(this List<dynamic> n, int s = 0)
        {
            return n == null ? new List<dynamic>() : JsonConvert.DeserializeObject<List<dynamic>>(n[s]);
        }
    }
}

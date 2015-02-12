using System;
using System.IO;
using Newtonsoft.Json;

namespace PL_Course.Infrastructure
{
    public static class StreamEx
    {

        public static T ReadFromJsonStream<T>(this Stream st)
        {
            return JsonConvert.DeserializeObject<T>(st.ReadToEnd());
        }

        public static object ReadFromJsonStream(this Stream st, string messageType)
        {
            return JsonConvert.DeserializeObject(st.ReadToEnd(), Type.GetType(messageType));
        }

        public static string ReadToEnd(this Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
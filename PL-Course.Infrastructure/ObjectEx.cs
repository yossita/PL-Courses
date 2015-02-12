using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PL_Course.Infrastructure
{
    public static class ObjectEx
    {

        public static Stream ToJsonStream<T>(this T obj)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(obj.ToJsonString()));
        }

        public static string ToJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string GetMessageType(this object obj)
        {
            return obj.GetType().AssemblyQualifiedName;
        }

    }
}

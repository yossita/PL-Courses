using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PL_Course.Infrastructure
{
    public static class ObjectEx
    {

        public static Stream ConvertToJsonStream<T>(this T obj)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static T ConvertFromJsonStream<T>(this Stream st)
        {
            using (var reader = new StreamReader(st))
            {
                var content = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

    }
}

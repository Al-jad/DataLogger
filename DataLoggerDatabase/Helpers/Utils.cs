using Newtonsoft.Json;

namespace DataLoggerDatabase.Helpers
{
    public static class StringUtils
    {
        public static string SerializeObject<T>(T obj)
        {
            var serializedObject = JsonConvert.SerializeObject(obj,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );
            return serializedObject;
        }

    }
}
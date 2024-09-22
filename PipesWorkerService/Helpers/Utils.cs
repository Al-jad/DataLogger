using Newtonsoft.Json;

namespace PipesWorkerService.Helpers
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




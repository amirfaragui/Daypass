using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
  static class TempDataExtensions
  {
    static readonly JsonSerializerSettings _settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

    public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
    {
      tempData[key] = JsonConvert.SerializeObject(value, _settings);
    }

    public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
    {
      object o;
      tempData.TryGetValue(key, out o);
      return o == null ? null : JsonConvert.DeserializeObject<T>((string)o, _settings);
    }
  }
}

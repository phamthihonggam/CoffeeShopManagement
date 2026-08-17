using System.Text.Json;

namespace CoffeeShopManagement.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            if (value == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
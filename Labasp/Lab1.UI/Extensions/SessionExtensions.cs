using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Lab1.UI.Extensions
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

        public static void Set<T>(this ISession session, string key, T value)
        {
            if (value == null)
            {
                session.Remove(key);
                return;
            }

            var json = JsonSerializer.Serialize(value, SerializerOptions);
            session.SetString(key, json);
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return string.IsNullOrEmpty(json)
                ? default
                : JsonSerializer.Deserialize<T>(json, SerializerOptions);
        }
    }
}


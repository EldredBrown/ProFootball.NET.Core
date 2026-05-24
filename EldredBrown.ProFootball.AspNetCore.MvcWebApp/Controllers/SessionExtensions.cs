using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    // Helper extensions
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
                // Optionally set MaxDepth if you know the graph is deep but acyclic:
                // MaxDepth = 128
            };
            session.SetString(key, JsonSerializer.Serialize(value, options));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

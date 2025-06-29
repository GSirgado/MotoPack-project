using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MotoPack_project // Usa o namespace raiz do teu projeto
{
    public static class SessionExtensions
    {
        // Guarda um objeto genérico na sessão como JSON
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Recupera um objeto da sessão, desserializando o JSON
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

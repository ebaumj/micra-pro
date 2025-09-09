using System.Text.Json;

namespace MicraPro.Auth.Domain.ValueObjects;

internal abstract record ResponseBody<T>
    where T : ResponseBody<T>
{
    public static implicit operator string(ResponseBody<T> body) =>
        JsonSerializer.Serialize((T)body);
}

using System.Net.Http;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

internal static class JsonResponse
{
    public static Guid GetGuid(JsonElement el, string camelName, string pascalName)
    {
        if (el.TryGetProperty(camelName, out var a))
            return a.GetGuid();
        return el.GetProperty(pascalName).GetGuid();
    }

  public static Guid GetIdFromCreatedLocation(HttpResponseMessage response)
    {
        var loc = response.Headers.Location;
        if (loc is null)
            throw new InvalidOperationException("Resposta 201 sem header Location.");
        var seg = loc.ToString().TrimEnd('/').Split('/').Last();
        return Guid.Parse(seg);
    }
}

using System.Net.Http;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

internal static class JsonResponse
{
    public static int GetInt32(JsonElement el, string camelName, string pascalName)
    {
        if (el.TryGetProperty(camelName, out var a))
            return a.GetInt32();
        return el.GetProperty(pascalName).GetInt32();
    }

    /// <summary>Extrai o último segmento numérico de um Location 201 (ex.: .../api/Account/3).</summary>
    public static int GetIdFromCreatedLocation(HttpResponseMessage response)
    {
        var loc = response.Headers.Location;
        if (loc is null)
            throw new InvalidOperationException("Resposta 201 sem header Location.");
        var seg = loc.ToString().TrimEnd('/').Split('/').Last();
        return int.Parse(seg, System.Globalization.CultureInfo.InvariantCulture);
    }
}

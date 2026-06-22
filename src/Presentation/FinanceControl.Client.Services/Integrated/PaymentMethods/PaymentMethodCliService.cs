using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.PaymentMethods;
using FinanceControl.Contracts.Dtos.PaymentMethods;

namespace FinanceControl.Client.Services.Integrated.PaymentMethods;

public sealed class PaymentMethodCliService(HttpClient httpClient) : IPaymentMethodCliService
{
    private const string BaseRoute = "api/PaymentMethods";

    public Task<IReadOnlyList<PaymentMethodDto>?> ListAsync(bool includeInactive = false, Guid? userId = null)
    {
        var parts = new List<string>();
        if (includeInactive) parts.Add("includeInactive=true");
        if (userId.HasValue) parts.Add($"userId={userId.Value}");
        var query = parts.Count > 0 ? "?" + string.Join("&", parts) : "";
        return httpClient.GetFromJsonAsync<IReadOnlyList<PaymentMethodDto>>(BaseRoute + query)!;
    }

    public Task<PaymentMethodDto?> GetByIdAsync(Guid id) =>
        httpClient.GetFromJsonAsync<PaymentMethodDto>($"{BaseRoute}/{id}");

    public Task<HttpResponseMessage> CreateAsync(PaymentMethodCreateDto dto) =>
        httpClient.PostAsJsonAsync(BaseRoute, dto);

    public Task<HttpResponseMessage> UpdateAsync(Guid id, PaymentMethodUpdateDto dto) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto);

    public Task<HttpResponseMessage> DeleteAsync(Guid id) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}");
}

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

public class UserPasswordUpdateTests : IClassFixture<FinanceApiFactory>
{
    private readonly FinanceApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public UserPasswordUpdateTests(FinanceApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Put_UpdatePassword_ReturnsBadRequest_WhenCurrentPasswordIsIncorrect()
    {
        using var client = _factory.CreateApiClient();
        var (userId, _, _, _) = await IntegrationSeed.EnsureUserAndCategoryAsync(_factory.Services);
        var email = await GetUserEmailAsync(client, userId);

        var res = await client.PutAsJsonAsync($"/User/{userId}/user-update", new
        {
            userId,
            userName = "Usuario atualizado",
            email,
            password = "NovaSenha123!",
            currentPassword = "SenhaErrada123!",
            isActive = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var error = await res.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        Assert.Equal("A senha atual informada está incorreta.", error.GetProperty("message").GetString());

        var login = await client.PostAsJsonAsync("/User/login", new
        {
            email,
            password = "Senha1234!"
        });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatePassword_ReturnsOk_WhenCurrentPasswordMatches()
    {
        using var client = _factory.CreateApiClient();
        var (userId, _, _, _) = await IntegrationSeed.EnsureUserAndCategoryAsync(_factory.Services);
        var email = await GetUserEmailAsync(client, userId);

        var res = await client.PutAsJsonAsync($"/User/{userId}/user-update", new
        {
            userId,
            userName = "Usuario atualizado",
            email,
            password = "NovaSenha123!",
            currentPassword = "Senha1234!",
            isActive = true
        });

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var login = await client.PostAsJsonAsync("/User/login", new
        {
            email,
            password = "NovaSenha123!"
        });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    private static async Task<string> GetUserEmailAsync(HttpClient client, Guid userId)
    {
        var user = await client.GetFromJsonAsync<JsonElement>($"/User/{userId}", JsonOpts);
        if (user.TryGetProperty("userEmail", out var camelCaseEmail))
            return camelCaseEmail.GetString() ?? string.Empty;

        return user.GetProperty("UserEmail").GetString() ?? string.Empty;
    }
}

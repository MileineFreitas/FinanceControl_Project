using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

public class UserProfilePhotoUpdateTests : IClassFixture<FinanceApiFactory>
{
    private readonly FinanceApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public UserProfilePhotoUpdateTests(FinanceApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Put_UpdateProfilePhoto_PersistsLongDataUrl()
    {
        using var client = _factory.CreateApiClient();
        var (userId, _, _, _) = await IntegrationSeed.EnsureUserAndCategoryAsync(_factory.Services);
        var user = await client.GetFromJsonAsync<JsonElement>($"/User/{userId}", JsonOpts);

        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(new string('A', 1200)));
        var profilePhoto = $"data:image/png;base64,{base64}";

        var res = await client.PutAsJsonAsync($"/User/{userId}/user-update", new
        {
            userId,
            userName = user.GetProperty("userName").GetString(),
            email = user.GetProperty("userEmail").GetString(),
            profilePhoto,
            isActive = user.GetProperty("isActive").GetBoolean()
        });

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var updated = await client.GetFromJsonAsync<JsonElement>($"/User/{userId}", JsonOpts);
        Assert.Equal(profilePhoto, updated.GetProperty("profilePhoto").GetString());
    }
}

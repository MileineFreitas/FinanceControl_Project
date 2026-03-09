namespace FinanceControl.Blazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string userName, string password)
    {
        // TODO: chamar API de login (ex: POST api/auth/login)
        await Task.CompletedTask;
        return false;
    }
}

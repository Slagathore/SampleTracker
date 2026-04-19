using System.Text.Json;

namespace SampleTracker.API.Services;

/// <summary>
/// Fetches secrets from Azure Key Vault using the App Service Managed Identity endpoint.
/// No SDK dependency — uses the IMDS token endpoint directly.
/// Only active when KeyVault:Uri is configured; silently skipped locally.
/// </summary>
public static class KeyVaultService
{
    public static async Task<string?> TryGetSecretAsync(string vaultUri, string secretName)
    {
        using var http = new HttpClient();

        // Step 1: get a Managed Identity token for Key Vault
        var tokenUrl = "http://169.254.169.254/metadata/identity/oauth2/token" +
                       "?api-version=2018-02-01&resource=https://vault.azure.net";

        http.DefaultRequestHeaders.Add("Metadata", "true");
        var tokenResponse = await http.GetStringAsync(tokenUrl);
        var token = JsonDocument.Parse(tokenResponse)
                                .RootElement
                                .GetProperty("access_token")
                                .GetString();

        // Step 2: fetch the secret
        http.DefaultRequestHeaders.Remove("Metadata");
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var secretUrl = $"{vaultUri.TrimEnd('/')}/secrets/{secretName}?api-version=7.4";
        var secretResponse = await http.GetStringAsync(secretUrl);
        return JsonDocument.Parse(secretResponse)
                           .RootElement
                           .GetProperty("value")
                           .GetString();
    }
}

namespace A3.MinimalApiValidation.Tests;

using System.Net;
using System.Net.Http.Json;

internal record ValidationProblemDetails(int Status, Dictionary<string, string[]> Errors);

internal static class Extensions
{
    public static async Task EnsureErrorFor(this HttpResponseMessage response, string key, params string[] otherKeys)
    {
        var count = 1 + otherKeys.Length;

        Assert.False(response.IsSuccessStatusCode, "Expected unsuccessful status code, but it was successful.");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(body);
        Assert.Equal(400, body.Status);
        Assert.Equal(count, body.Errors.Count);

        var loweredKeys = body.Errors.Keys.Select(x => x.ToLowerInvariant()).ToList();
        Assert.True(
            loweredKeys.Contains(key.ToLowerInvariant()),
            $"Expected error for key '{key}', but it was not found.");

        foreach (var otherKey in otherKeys)
        {
            Assert.True(
                loweredKeys.Contains(otherKey.ToLowerInvariant()),
                $"Expected error for key '{otherKey}', but it was not found.");
        }
    }
}

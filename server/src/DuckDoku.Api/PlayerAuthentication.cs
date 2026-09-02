using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api;

public static class PlayerAuthentication
{
    private const string TokenPrefix = "Ducky ";

    public static string CreateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public static async Task<Device?> FindDeviceAsync(
        HttpContext context, 
        AppDbContext database)
    {
        string? header = context.Request.Headers.Authorization;

        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith(TokenPrefix, StringComparison.Ordinal))
        {
            return null;
        }

        string token = header.Substring(TokenPrefix.Length).Trim();

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        return await database.Devices
            .FirstOrDefaultAsync(device => device.Token == token);
    }
}
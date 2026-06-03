using System.Security.Cryptography;
using System.Text;

namespace GymManagementSystem.BusinessLayer.Helpers.Attendance;

public static class PayloadHelper
{
    private const string Prefix = "GYMYCHECKIN";

    public static string BuildPayload(int bookingId, string signature)
        => $"{Prefix}:{bookingId}:{signature}";

    public static (int bookingId, string signature)? Parse(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload)) return null;

        var parts = payload.Split(':');
        if (parts.Length != 3 || parts[0] != Prefix) return null;
        if (!int.TryParse(parts[1], out var bookingId)) return null;

        return (bookingId, parts[2]);
    }

    public static string ComputeSignature(int bookingId, string secretKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(bookingId.ToString()));
        return Convert.ToHexString(hash).Substring(0, 16).ToLowerInvariant();
    }

    public static bool VerifySignature(int bookingId, string signature, string secretKey)
        => string.Equals(
            signature,
            ComputeSignature(bookingId, secretKey),
            StringComparison.OrdinalIgnoreCase);
}

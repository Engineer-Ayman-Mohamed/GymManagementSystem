using GymManagementSystem.BusinessLayer.DTOs.Attendance;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface IAttendanceService
{
    string BuildPayload(int bookingId, string secretKey);
    byte[] GenerateQRCode(string payload);
    Task<CheckInResponse> CheckInAsync(string payload, string secretKey, CancellationToken ct = default);
}
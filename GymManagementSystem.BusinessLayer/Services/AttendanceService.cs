using GymManagementSystem.BusinessLayer.DTOs.Attendance;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Helpers.Attendance;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class AttendanceService : IAttendanceService
{
    private readonly IGenericRepository<Booking> _repository;
    private readonly ILogger<AttendanceService> _logger;

    public AttendanceService(IGenericRepository<Booking> repository,
        ILogger<AttendanceService> logger
    ) {
        _repository = repository;
        _logger = logger;
    }

    public string BuildPayload(int bookingId, string secretKey)
    {
        var signrature = PayloadHelper.ComputeSignature(bookingId, secretKey);
        return PayloadHelper.BuildPayload(bookingId, signrature);
    }
    
    public byte[] GenerateQRCode(string payload)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(20);
    }

    public async Task<CheckInResponse> CheckInAsync(string payload, string secretKey, CancellationToken ct = default)
    {
         var parsed = PayloadHelper.Parse(payload);
        if (parsed is null)
        {
            _logger.LogWarning("Check-in rejected: invalid format. Payload={Payload}", payload);
            return new CheckInResponse { Result = CheckInResult.InvalidFormat };
        }

        var (bookingId, signature) = parsed.Value;

        if (!PayloadHelper.VerifySignature(bookingId, signature, secretKey))
        {
            _logger.LogWarning("Check-in rejected: invalid signature. BookingId={BookingId}", bookingId);
            return new CheckInResponse { Result = CheckInResult.InvalidSignature };
        }

        var booking = await _repository.GetByIdWithIncludesAsync(
            bookingId,
            ct,
            b => b.Member,
            b => b.Session,
            b => b.Session.Trainer,
            b => b.Session.Category);

        if (booking is null)
        {
            _logger.LogWarning("Check-in rejected: booking not found. BookingId={BookingId}", bookingId);
            return new CheckInResponse { Result = CheckInResult.NotFound };
        }

        if (booking.IsAttended)
        {
            _logger.LogWarning("Check-in rejected: already attended. BookingId={BookingId}", bookingId);
            return new CheckInResponse
            {
                Result = CheckInResult.AlreadyAttended,
                MemberName = booking.Member.Name,
                SessionDescription = booking.Session.Description
            };
        }

        var today = DateTime.UtcNow.Date;
        if (booking.Session.StartDate.Date != today)
        {
            _logger.LogWarning(
                "Check-in rejected: session not today. BookingId={BookingId}, SessionStart={SessionStart}, Today={Today}",
                bookingId, booking.Session.StartDate.Date, today);
            return new CheckInResponse
            {
                Result = CheckInResult.SessionNotToday,
                MemberName = booking.Member.Name,
                SessionDescription = booking.Session.Description,
                SessionDate = booking.Session.StartDate.ToString("yyyy-MM-dd")
            };
        }

        booking.IsAttended = true;
        booking.CheckedInAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(booking, ct);

        _logger.LogInformation(
            "Check-in successful: BookingId={BookingId}, Member={Member}, Session={Session}",
            bookingId, booking.Member.Name, booking.Session.Description);

        return new CheckInResponse
        {
            Result = CheckInResult.Success,
            MemberName = booking.Member.Name,
            MemberEmail = booking.Member.Email,
            SessionDescription = booking.Session.Description,
            SessionDate = booking.Session.StartDate.ToString("yyyy-MM-dd HH:mm"),
            TrainerName = booking.Session.Trainer.Name,
            CategoryName = booking.Session.Category.CategoryName,
            CheckedInAt = booking.CheckedInAt
        };
    }
}
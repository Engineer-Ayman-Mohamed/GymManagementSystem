using GymManagementSystem.BusinessLayer.DTOs.Attendance;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class AttendanceController : Controller
{
    private readonly IAttendanceService _attendanceService;
    private readonly IConfiguration _configuration;

    public AttendanceController(
        IAttendanceService attendanceService,
        IConfiguration configuration)
    {
        _attendanceService = attendanceService;
        _configuration = configuration;
    }

    private string SecretKey => _configuration["Attendance:SecretKey"]!;

    [HttpGet]
    public IActionResult MyQr(int bookingId)
    {
        var payload = _attendanceService.BuildPayload(bookingId, SecretKey);
        var qrBytes = _attendanceService.GenerateQRCode(payload);
        var qrBase64 = Convert.ToBase64String(qrBytes);

        ViewBag.QrImage = qrBase64;
        ViewBag.BookingId = bookingId;
        ViewBag.Payload = payload;

        return View();
    }

    [HttpGet]
    public IActionResult Scan()
    {
        return View();
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CheckIn(string payload, CancellationToken ct)
    {
        var result = await _attendanceService.CheckInAsync(payload, SecretKey, ct);

        TempData["CheckInResult"] = result.Result.ToString();

        if (result.Result == CheckInResult.Success)
        {
            TempData["CheckInMessage"] =
                $"Welcome {result.MemberName}! " +
                $"Checked in for {result.SessionDescription} " +
                $"({result.CategoryName} with {result.TrainerName}) " +
                $"at {result.CheckedInAt:HH:mm}.";
        }
        else
        {
            TempData["CheckInMessage"] = GetErrorMessage(result);
        }

        return RedirectToAction(nameof(Result));
    }

    [HttpGet]
    public IActionResult Result()
    {
        ViewBag.Result = TempData["CheckInResult"]?.ToString();
        ViewBag.Message = TempData["CheckInMessage"]?.ToString();
        return View();
    }

    private static string GetErrorMessage(CheckInResponse result) => result.Result switch
    {
        CheckInResult.InvalidFormat => "Invalid QR code format.",
        CheckInResult.InvalidSignature => "Invalid QR code signature. This code may be tampered with.",
        CheckInResult.NotFound => "Booking not found.",
        CheckInResult.AlreadyAttended => $"{result.MemberName} has already checked in for {result.SessionDescription}.",
        CheckInResult.SessionNotToday => $"This session is scheduled for {result.SessionDate}. Check-in is only allowed on the session day.",
        _ => "An unknown error occurred."
    };
}
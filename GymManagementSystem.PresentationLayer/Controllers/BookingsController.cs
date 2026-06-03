using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.Booking;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IExportService _exportService;

    public BookingsController(IBookingService bookingService, IExportService exportService)
    {
        _bookingService = bookingService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var bookings = await _bookingService.GetAllAsync(ct);
        return View(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> GetBookings(CancellationToken ct)
    {
        var bookings = await _bookingService.GetAllAsync(ct);
        return Json(bookings);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var booking = await _bookingService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Booking created for member ID {booking.MemberId}.";
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
        catch (ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    public async Task<IActionResult> Edit([FromRoute] int id, CancellationToken ct)
    {
        var booking = await _bookingService.GetByIdAsync(id, ct);
        if (booking is null) return NotFound();
        var request = new UpdateBookingRequest
        {
            BookingDate = booking.BookingDate,
            IsAttended = booking.IsAttended,
            CheckedInAt = booking.CheckedInAt,
            MemberId = booking.MemberId,
            SessionId = booking.SessionId
        };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, UpdateBookingRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _bookingService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Booking updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var booking = await _bookingService.GetByIdAsync(id, ct);
        if (booking is null) return NotFound();
        return View(booking);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _bookingService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Booking deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var booking = await _bookingService.GetByIdAsync(id, ct);
        if (booking is null) return NotFound();
        return View(booking);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var bookings = await _bookingService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            bookings, BookingExport.GetColumns(), ExportFormat.Excel, "Bookings", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"bookings_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var bookings = await _bookingService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            bookings, BookingExport.GetColumns(), ExportFormat.Pdf, "Bookings", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"bookings_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.Session;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class SessionsController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IExportService _exportService;

    public SessionsController(ISessionService sessionService, IExportService exportService)
    {
        _sessionService = sessionService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var sessions = await _sessionService.GetAllAsync(ct);
        return View(sessions);
    }

    [HttpPost]
    public async Task<IActionResult> GetSessions(CancellationToken ct)
    {
        var sessions = await _sessionService.GetAllAsync(ct);
        return Json(sessions);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSessionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var session = await _sessionService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Session '{session.Description}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    public async Task<IActionResult> Edit([FromRoute] int id, CancellationToken ct)
    {
        var session = await _sessionService.GetByIdAsync(id, ct);
        if (session is null) return NotFound();
        var request = new UpdateSessionRequest
        {
            Description = session.Description,
            Capacity = session.Capacity,
            StartDate = session.StartDate,
            EndDate = session.EndDate,
            TrainerId = session.TrainerId,
            CategoryId = session.CategoryId
        };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, UpdateSessionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _sessionService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Session updated successfully.";
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
        var session = await _sessionService.GetByIdAsync(id, ct);
        if (session is null) return NotFound();
        return View(session);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _sessionService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Session deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var session = await _sessionService.GetByIdAsync(id, ct);
        if (session is null) return NotFound();
        return View(session);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var sessions = await _sessionService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            sessions, SessionExport.GetColumns(), ExportFormat.Excel, "Sessions", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"sessions_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var sessions = await _sessionService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            sessions, SessionExport.GetColumns(), ExportFormat.Pdf, "Sessions", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"sessions_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

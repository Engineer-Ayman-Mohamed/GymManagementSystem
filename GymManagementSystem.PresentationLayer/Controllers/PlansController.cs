using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.Plan;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class PlansController : Controller
{
    private readonly IPlanService _planService;
    private readonly IExportService _exportService;

    public PlansController(IPlanService planService, IExportService exportService)
    {
        _planService = planService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var plans = await _planService.GetAllAsync(ct);
        return View(plans);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePlanRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var plan = await _planService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Plan '{plan.Name}' created successfully.";
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
        var plan = await _planService.GetByIdAsync(id, ct);
        if (plan is null) return NotFound();
        var request = new UpdatePlanRequest
        {
            Name = plan.Name,
            Description = plan.Description,
            DurationDays = plan.DurationDays,
            Price = plan.Price,
            IsActive = plan.IsActive
        };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, UpdatePlanRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _planService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Plan updated successfully.";
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
        var plan = await _planService.GetByIdAsync(id, ct);
        if (plan is null) return NotFound();
        return View(plan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _planService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Plan deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var plan = await _planService.GetByIdAsync(id, ct);
        if (plan is null) return NotFound();
        return View(plan);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var plans = await _planService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            plans, PlanExport.GetColumns(), ExportFormat.Excel, "Plans", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"plans_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var plans = await _planService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            plans, PlanExport.GetColumns(), ExportFormat.Pdf, "Plans", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"plans_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

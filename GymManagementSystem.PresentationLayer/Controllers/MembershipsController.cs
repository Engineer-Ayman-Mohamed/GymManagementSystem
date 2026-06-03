using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.Membership;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class MembershipsController : Controller
{
    private readonly IMembershipService _membershipService;
    private readonly IExportService _exportService;

    public MembershipsController(IMembershipService membershipService, IExportService exportService)
    {
        _membershipService = membershipService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var memberships = await _membershipService.GetAllAsync(ct);
        return View(memberships);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMembershipRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var membership = await _membershipService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Membership created for member ID {membership.MemberId}.";
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
        var membership = await _membershipService.GetByIdAsync(id, ct);
        if (membership is null) return NotFound();
        var request = new UpdateMembershipRequest
        {
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            MemberId = membership.MemberId,
            PlanId = membership.PlanId
        };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, UpdateMembershipRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _membershipService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Membership updated successfully.";
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
        var membership = await _membershipService.GetByIdAsync(id, ct);
        if (membership is null) return NotFound();
        return View(membership);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _membershipService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Membership deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var membership = await _membershipService.GetByIdAsync(id, ct);
        if (membership is null) return NotFound();
        return View(membership);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var memberships = await _membershipService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            memberships, MembershipExport.GetColumns(), ExportFormat.Excel, "Memberships", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"memberships_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var memberships = await _membershipService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            memberships, MembershipExport.GetColumns(), ExportFormat.Pdf, "Memberships", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"memberships_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

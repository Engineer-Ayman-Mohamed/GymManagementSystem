using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.HealthRecord;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class HealthRecordsController : Controller
{
    private readonly IHealthRecordService _healthRecordService;
    private readonly IExportService _exportService;

    public HealthRecordsController(IHealthRecordService healthRecordService, IExportService exportService)
    {
        _healthRecordService = healthRecordService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var records = await _healthRecordService.GetAllAsync(ct);
        return View(records);
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var record = await _healthRecordService.GetByIdAsync(id, ct);
        if (record is null) return NotFound();
        return View(record);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int memberId, CancellationToken ct)
    {
        var existing = await _healthRecordService.GetByMemberIdAsync(memberId, ct);
        if (existing is not null)
        {
            TempData["StatusMessage"] = "This member already has a health record.";
            return RedirectToAction("Details", "Members", new { id = memberId });
        }
        var request = new CreateHealthRecordRequest { MemberId = memberId };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHealthRecordRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var record = await _healthRecordService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Health record created for {record.MemberName}.";
            return RedirectToAction("Details", "Members", new { id = request.MemberId });
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
        var record = await _healthRecordService.GetByIdAsync(id, ct);
        if (record is null) return NotFound();
        var request = new UpdateHealthRecordRequest
        {
            Height = record.Height,
            Weight = record.Weight,
            BloodType = record.BloodType,
            Note = record.Note
        };
        ViewBag.MemberId = record.MemberId;
        ViewBag.MemberName = record.MemberName;
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, UpdateHealthRecordRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _healthRecordService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Health record updated successfully.";
            var record = await _healthRecordService.GetByIdAsync(id, ct);
            return RedirectToAction("Details", "Members", new { id = record!.MemberId });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var record = await _healthRecordService.GetByIdAsync(id, ct);
        if (record is null) return NotFound();
        return View(record);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        HealthRecordDto? record = null;
        try
        {
            record = await _healthRecordService.GetByIdAsync(id, ct);
            await _healthRecordService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Health record deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction("Details", "Members", new { id = record?.MemberId });
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var records = await _healthRecordService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            records, HealthRecordExport.GetColumns(), ExportFormat.Excel, "HealthRecords", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"health_records_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var records = await _healthRecordService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            records, HealthRecordExport.GetColumns(), ExportFormat.Pdf, "HealthRecords", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"health_records_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

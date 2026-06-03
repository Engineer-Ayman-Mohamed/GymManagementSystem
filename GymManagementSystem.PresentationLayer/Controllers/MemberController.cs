using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.Member;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IExportService _exportService;
    private readonly IHealthRecordService _healthRecordService;

    public MembersController(IMemberService memberService, IExportService exportService, IHealthRecordService healthRecordService)
    {
        _memberService = memberService;
        _exportService = exportService;
        _healthRecordService = healthRecordService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var members = await _memberService.GetAllAsync(ct);
        return View(members);
    }

    [HttpPost]
    public async Task<IActionResult> GetMembers(CancellationToken ct)
    {
        var members = await _memberService.GetAllAsync(ct);
        return Json(members);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMemberRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var member = await _memberService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Member '{member.Name}' created successfully.";
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
        var member = await _memberService.GetByIdAsync(id, ct);
        if (member is null) return NotFound();
        var request= new UpdateMemberRequest
        {
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            DateOfBirth = member.DateOfBirth,
            Gender = Enum.TryParse<DataLayer.Enums.Gender>(member.Gender, out var gender) ? gender : null,
            Photo = member.Photo,
            JoinDate = member.JoinDate,
            BuildingNumber = member.BuildingNumber,
            Street = member.Street,
            City = member.City
        };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [FromRoute] int id,
        UpdateMemberRequest request,
        CancellationToken ct
        )
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _memberService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Member updated successfully.";
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
        var member = await _memberService.GetByIdAsync(id, ct);
        if (member is null) return NotFound();
        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _memberService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Member deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var member = await _memberService.GetByIdAsync(id, ct);
        if (member is null)
            return NotFound();

        var healthRecord = await _healthRecordService.GetByMemberIdAsync(id, ct);
        ViewData["HealthRecord"] = healthRecord;

        return View(member);
    }
    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var members = await _memberService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            members, MemberExport.GetColumns(), ExportFormat.Excel, "Members", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"members_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var members = await _memberService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            members, MemberExport.GetColumns(), ExportFormat.Pdf, "Members", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"members_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

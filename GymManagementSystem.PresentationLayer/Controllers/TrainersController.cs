using System.Net.Http.Headers;
using GymManagementSystem.BusinessLayer.DTOs.Trainer;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class TrainersController : Controller
{
    private readonly ITrainerService _trainerService;
    private readonly IExportService _exportService;

    public TrainersController(ITrainerService trainerService, IExportService exportService)
    {
        _trainerService = trainerService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var trainers = await _trainerService.GetAllAsync(ct);
        return View(trainers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTrainerRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            var trainer = await _trainerService.CreateAsync(request, ct);
            TempData["StatusMessage"] = $"Trainer '{trainer.Name}' created successfully.";
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
        var trainer = await _trainerService.GetByIdAsync(id, ct);
        if (trainer is null) return NotFound();
        var request = new UpdateTrainerRequest
        {
            Name = trainer.Name,
            Email = trainer.Email,
            Phone = trainer.Phone,
            DateOfBirth = trainer.DateOfBirth,
            Gender = Enum.TryParse<DataLayer.Enums.Gender>(trainer.Gender, out var gender) ? gender : null,
            BuildingNumber = trainer.BuildingNumber,
            Street = trainer.Street,
            City = trainer.City,
            Specialty = Enum.TryParse<DataLayer.Enums.Specialty>(trainer.Specialty, out var specialty) ? specialty : null,
            HireDate = trainer.HireDate
        };
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, UpdateTrainerRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _trainerService.UpdateAsync(id, request, ct);
            TempData["StatusMessage"] = "Trainer updated successfully.";
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
        var trainer = await _trainerService.GetByIdAsync(id, ct);
        if (trainer is null) return NotFound();
        return View(trainer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _trainerService.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Trainer deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct)
    {
        var trainer = await _trainerService.GetByIdAsync(id, ct);
        if (trainer is null) return NotFound();
        return View(trainer);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var trainers = await _trainerService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            trainers, TrainerExport.GetColumns(), ExportFormat.Excel, "Trainers", ct);

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"trainers_{timestamp}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(CancellationToken ct)
    {
        var trainers = await _trainerService.GetAllAsync(ct);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var bytes = await _exportService.ExportAsync(
            trainers, TrainerExport.GetColumns(), ExportFormat.Pdf, "Trainers", ct);

        var cd = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"trainers_{timestamp}.pdf"
        };
        Response.Headers.ContentDisposition = cd.ToString();
        return File(bytes, "application/pdf");
    }
}

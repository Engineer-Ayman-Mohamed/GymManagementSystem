using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using GymManagementSystem.PresentationLayer.Models;

namespace GymManagementSystem.PresentationLayer.Controllers;

public class HomeController : Controller
{
    private readonly IGenericRepository<Member> _memberRepo;
    private readonly IGenericRepository<Session> _sessionRepo;
    private readonly IGenericRepository<Booking> _bookingRepo;
    private readonly IGenericRepository<Trainer> _trainerRepo;

    public HomeController(
        IGenericRepository<Member> memberRepo,
        IGenericRepository<Session> sessionRepo,
        IGenericRepository<Booking> bookingRepo,
        IGenericRepository<Trainer> trainerRepo)
    {
        _memberRepo = memberRepo;
        _sessionRepo = sessionRepo;
        _bookingRepo = bookingRepo;
        _trainerRepo = trainerRepo;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var members = await _memberRepo.GetAllAsync(ct);
        var sessions = await _sessionRepo.GetAllAsync(ct);
        var bookings = await _bookingRepo.GetAllAsync(ct);
        var trainers = await _trainerRepo.GetAllAsync(ct);

        ViewData["MemberCount"] = members.Count;
        ViewData["SessionCount"] = sessions.Count;
        ViewData["BookingCount"] = bookings.Count;
        ViewData["TrainerCount"] = trainers.Count;
        ViewData["CheckedInCount"] = bookings.Count(b => b.IsAttended);
        ViewData["UpcomingSessions"] = sessions.Count(s => s.StartDate > DateTime.UtcNow);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var model = new ErrorViewModel
        {
            RequestId = TempData["RequestId"] as string
                ?? Activity.Current?.Id
                ?? HttpContext.TraceIdentifier,
            StatusCode = int.TryParse(TempData["StatusCode"]?.ToString(), out var sc) ? sc : 0,
            ErrorMessage = TempData["ErrorMessage"] as string,
            ExceptionDetails = TempData["ExceptionDetails"] as string
        };

        if (model.StatusCode > 0)
        {
            HttpContext.Response.StatusCode = model.StatusCode;
        }

        return View(model);
    }
}

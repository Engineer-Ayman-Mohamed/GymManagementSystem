using GymManagementSystem.BusinessLayer.DTOs.Booking;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class BookingService : IBookingService
{
    private readonly IGenericRepository<Booking> _repository;
    private readonly IGenericRepository<Session> _sessionRepository;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IGenericRepository<Booking> repository,
        IGenericRepository<Session> sessionRepository,
        IGenericRepository<Member> memberRepository,
        ILogger<BookingService> logger)
    {
        _repository = repository;
        _sessionRepository = sessionRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BookingDto>> GetAllAsync(CancellationToken ct = default)
    {
        var bookings = await _repository.GetAllWithIncludesAsync(
            ct,
            b => b.Member,
            b => b.Session,
            b => b.Session.Trainer,
            b => b.Session.Category);
        return bookings.Select(b => MapToDto(b, b.Member, b.Session)).ToList();
    }

    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var booking = await _repository.GetByIdWithIncludesAsync(id, ct, b => b.Member, b => b.Session, b => b.Session.Trainer, b => b.Session.Category);
        return booking is null ? null : MapToDto(booking, booking.Member, booking.Session);
    }

    public async Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken ct = default)
    {
        var memberExists = await _memberRepository.ExistsAsync(m => m.Id == request.MemberId, ct);
        if (!memberExists)
            throw new NotFoundException($"Member with ID {request.MemberId} not found.");

        var session = await _sessionRepository.GetByIdWithIncludesAsync(
            request.SessionId, ct, s => s.Bookings!);
        if (session is null)
            throw new NotFoundException($"Session with ID {request.SessionId} not found.");

        var activeBookings = session.Bookings?
            .Count(b => !b.IsDeleted) ?? 0;
        if (activeBookings >= session.Capacity)
            throw new ConflictException($"Session '{session.Description}' has reached maximum capacity ({session.Capacity}).");

        var duplicate = session.Bookings?
            .Any(b => b.MemberId == request.MemberId && !b.IsDeleted) ?? false;
        if (duplicate)
            throw new ConflictException($"Member with ID {request.MemberId} is already booked for this session.");

        var booking = new Booking
        {
            BookingDate = request.BookingDate,
            MemberId = request.MemberId,
            SessionId = request.SessionId,
            IsAttended = false,
            CheckedInAt = null
        };

        var created = await _repository.AddAsync(booking, ct);
        _logger.LogInformation(
            "Created booking {BookingId}: Member={MemberId}, Session={SessionId}",
            created.Id, created.MemberId, created.SessionId);

        return MapToDto(created, created.Member, created.Session);
    }

    public async Task<BookingDto> UpdateAsync(int id, UpdateBookingRequest request, CancellationToken ct = default)
    {
        var booking = await _repository.GetByIdWithIncludesAsync(
            id, ct, b => b.Member, b => b.Session, b => b.Session.Trainer, b => b.Session.Category);
        if (booking is null)
            throw new NotFoundException($"Booking with ID {id} not found.");

        if (request.BookingDate.HasValue) booking.BookingDate = request.BookingDate.Value;
        if (request.IsAttended.HasValue) booking.IsAttended = request.IsAttended.Value;
        if (request.CheckedInAt.HasValue) booking.CheckedInAt = request.CheckedInAt;
        if (request.MemberId.HasValue) booking.MemberId = request.MemberId.Value;
        if (request.SessionId.HasValue) booking.SessionId = request.SessionId.Value;

        await _repository.UpdateAsync(booking, ct);
        _logger.LogInformation("Updated booking {BookingId}", id);

        return MapToDto(booking, booking.Member, booking.Session);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var booking = await _repository.GetByIdAsync(id, ct);
        if (booking is null)
            throw new NotFoundException($"Booking with ID {id} not found.");

        await _repository.SoftDeleteAsync(booking, ct);
        _logger.LogInformation("Soft-deleted booking {BookingId}", id);
    }

    private static BookingDto MapToDto(Booking booking, Member? member, Session? session) => new()
    {
        Id = booking.Id,
        BookingDate = booking.BookingDate,
        IsAttended = booking.IsAttended,
        CheckedInAt = booking.CheckedInAt,
        MemberId = booking.MemberId,
        MemberName = member?.Name ?? string.Empty,
        MemberEmail = member?.Email ?? string.Empty,
        SessionId = booking.SessionId,
        SessionDescription = session?.Description ?? string.Empty,
        SessionStartDate = session?.StartDate ?? default,
        SessionEndDate = session?.EndDate ?? default,
        TrainerName = session?.Trainer?.Name ?? string.Empty,
        CategoryName = session?.Category?.CategoryName ?? string.Empty,
        CreatedAt = booking.CreatedAt
    };
}

using GymManagementSystem.BusinessLayer.DTOs.Session;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class SessionService : ISessionService
{
    private readonly IGenericRepository<Session> _repository;
    private readonly ILogger<SessionService> _logger;

    public SessionService(IGenericRepository<Session> repository, ILogger<SessionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SessionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var sessions = await _repository.GetAllWithIncludesAsync(ct, s => s.Trainer, s => s.Category);
        return sessions.Select(MapToDto).ToList();
    }

    public async Task<SessionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var session = await _repository.GetByIdAsync(id, ct);
        return session is null ? null : MapToDto(session);
    }

    public async Task<SessionDto> CreateAsync(CreateSessionRequest request, CancellationToken ct = default)
    {
        var session = new Session
        {
            Description = request.Description,
            Capacity = request.Capacity,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TrainerId = request.TrainerId,
            CategoryId = request.CategoryId
        };

        var created = await _repository.AddAsync(session, ct);
        _logger.LogInformation("Created session {SessionId}: {Description}", created.Id, created.Description);

        return MapToDto(created);
    }

    public async Task<SessionDto> UpdateAsync(int id, UpdateSessionRequest request, CancellationToken ct = default)
    {
        var session = await _repository.GetByIdAsync(id, ct);
        if (session is null)
            throw new NotFoundException($"Session with ID {id} not found.");

        if (request.Description is not null) session.Description = request.Description;
        if (request.Capacity.HasValue) session.Capacity = request.Capacity.Value;
        if (request.StartDate.HasValue) session.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) session.EndDate = request.EndDate.Value;
        if (request.TrainerId.HasValue) session.TrainerId = request.TrainerId.Value;
        if (request.CategoryId.HasValue) session.CategoryId = request.CategoryId.Value;

        await _repository.UpdateAsync(session, ct);
        _logger.LogInformation("Updated session {SessionId}", id);

        return MapToDto(session);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var session = await _repository.GetByIdAsync(id, ct);
        if (session is null)
            throw new NotFoundException($"Session with ID {id} not found.");

        await _repository.SoftDeleteAsync(session, ct);
        _logger.LogInformation("Soft-deleted session {SessionId}", id);
    }

    private static SessionDto MapToDto(Session session) => new()
    {
        Id = session.Id,
        Description = session.Description,
        Capacity = session.Capacity,
        StartDate = session.StartDate,
        EndDate = session.EndDate,
        TrainerId = session.TrainerId,
        TrainerName = session.Trainer?.Name ?? string.Empty,
        CategoryId = session.CategoryId,
        CategoryName = session.Category?.CategoryName ?? string.Empty,
        CreatedAt = session.CreatedAt
    };
}

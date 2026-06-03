using GymManagementSystem.BusinessLayer.DTOs.Plan;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class PlanService : IPlanService
{
    private readonly IGenericRepository<Plan> _repository;
    private readonly ILogger<PlanService> _logger;

    public PlanService(IGenericRepository<Plan> repository, ILogger<PlanService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlanDto>> GetAllAsync(CancellationToken ct = default)
    {
        var plans = await _repository.GetAllAsync(ct);
        return plans.Select(MapToDto).ToList();
    }

    public async Task<PlanDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct);
        return plan is null ? null : MapToDto(plan);
    }

    public async Task<PlanDto> CreateAsync(CreatePlanRequest request, CancellationToken ct = default)
    {
        var nameExists = await _repository.ExistsAsync(p => p.Name == request.Name, ct);
        if (nameExists)
            throw new ConflictException($"Plan name '{request.Name}' already exists.");

        var plan = new Plan
        {
            Name = request.Name,
            Description = request.Description,
            DurationDays = request.DurationDays,
            Price = request.Price,
            IsActive = request.IsActive
        };

        var created = await _repository.AddAsync(plan, ct);
        _logger.LogInformation("Created plan {PlanId}: {Name}", created.Id, created.Name);

        return MapToDto(created);
    }

    public async Task<PlanDto> UpdateAsync(int id, UpdatePlanRequest request, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct);
        if (plan is null)
            throw new NotFoundException($"Plan with ID {id} not found.");

        if (request.Name is not null && !string.Equals(request.Name, plan.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await _repository.ExistsAsync(p => p.Name == request.Name && p.Id != id, ct);
            if (nameExists)
                throw new ConflictException($"Plan name '{request.Name}' is already in use.");
        }

        if (request.Name is not null) plan.Name = request.Name;
        if (request.Description is not null) plan.Description = request.Description;
        if (request.DurationDays.HasValue) plan.DurationDays = request.DurationDays.Value;
        if (request.Price.HasValue) plan.Price = request.Price.Value;
        if (request.IsActive.HasValue) plan.IsActive = request.IsActive.Value;

        await _repository.UpdateAsync(plan, ct);
        _logger.LogInformation("Updated plan {PlanId}", id);

        return MapToDto(plan);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var plan = await _repository.GetByIdAsync(id, ct);
        if (plan is null)
            throw new NotFoundException($"Plan with ID {id} not found.");

        await _repository.SoftDeleteAsync(plan, ct);
        _logger.LogInformation("Soft-deleted plan {PlanId}", id);
    }

    private static PlanDto MapToDto(Plan plan) => new()
    {
        Id = plan.Id,
        Name = plan.Name,
        Description = plan.Description,
        DurationDays = plan.DurationDays,
        Price = plan.Price,
        IsActive = plan.IsActive,
        CreatedAt = plan.CreatedAt
    };
}

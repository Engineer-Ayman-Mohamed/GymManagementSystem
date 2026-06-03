using GymManagementSystem.BusinessLayer.DTOs.Plan;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface IPlanService
{
    Task<IReadOnlyList<PlanDto>> GetAllAsync(CancellationToken ct = default);
    Task<PlanDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PlanDto> CreateAsync(CreatePlanRequest request, CancellationToken ct = default);
    Task<PlanDto> UpdateAsync(int id, UpdatePlanRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

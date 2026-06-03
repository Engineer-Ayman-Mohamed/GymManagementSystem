using GymManagementSystem.BusinessLayer.DTOs.Trainer;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface ITrainerService
{
    Task<IReadOnlyList<TrainerDto>> GetAllAsync(CancellationToken ct = default);
    Task<TrainerDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TrainerDto> CreateAsync(CreateTrainerRequest request, CancellationToken ct = default);
    Task<TrainerDto> UpdateAsync(int id, UpdateTrainerRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

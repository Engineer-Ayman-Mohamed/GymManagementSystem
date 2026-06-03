using GymManagementSystem.BusinessLayer.DTOs.Session;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface ISessionService
{
    Task<IReadOnlyList<SessionDto>> GetAllAsync(CancellationToken ct = default);
    Task<SessionDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SessionDto> CreateAsync(CreateSessionRequest request, CancellationToken ct = default);
    Task<SessionDto> UpdateAsync(int id, UpdateSessionRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

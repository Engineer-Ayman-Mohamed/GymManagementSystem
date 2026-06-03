using GymManagementSystem.BusinessLayer.DTOs.HealthRecord;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface IHealthRecordService
{
    Task<IReadOnlyList<HealthRecordDto>> GetAllAsync(CancellationToken ct = default);
    Task<HealthRecordDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<HealthRecordDto?> GetByMemberIdAsync(int memberId, CancellationToken ct = default);
    Task<HealthRecordDto> CreateAsync(CreateHealthRecordRequest request, CancellationToken ct = default);
    Task<HealthRecordDto> UpdateAsync(int id, UpdateHealthRecordRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

using GymManagementSystem.BusinessLayer.DTOs.Membership;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface IMembershipService
{
    Task<IReadOnlyList<MembershipDto>> GetAllAsync(CancellationToken ct = default);
    Task<MembershipDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MembershipDto> CreateAsync(CreateMembershipRequest request, CancellationToken ct = default);
    Task<MembershipDto> UpdateAsync(int id, UpdateMembershipRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

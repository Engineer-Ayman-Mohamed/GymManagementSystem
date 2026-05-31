using GymManagementSystem.BusinessLayer.DTOs.Member;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface IMemberService
{
    Task<IReadOnlyList<MemberDto>> GetAllAsync(CancellationToken ct = default);
    Task<MemberDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken ct = default);
    Task<MemberDto> UpdateAsync(int id, UpdateMemberRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

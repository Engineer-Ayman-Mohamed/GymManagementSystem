using GymManagementSystem.BusinessLayer.DTOs.Membership;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class MembershipService : IMembershipService
{
    private readonly IGenericRepository<Membership> _repository;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly IGenericRepository<Plan> _planRepository;
    private readonly ILogger<MembershipService> _logger;

    public MembershipService(
        IGenericRepository<Membership> repository,
        IGenericRepository<Member> memberRepository,
        IGenericRepository<Plan> planRepository,
        ILogger<MembershipService> logger)
    {
        _repository = repository;
        _memberRepository = memberRepository;
        _planRepository = planRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MembershipDto>> GetAllAsync(CancellationToken ct = default)
    {
        var memberships = await _repository.GetAllWithIncludesAsync(ct, m => m.Member, m => m.Plan);
        return memberships.Select(m => MapToDto(m, m.Member, m.Plan)).ToList();
    }

    public async Task<MembershipDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var membership = await _repository.GetByIdWithIncludesAsync(id, ct, m => m.Member, m => m.Plan);
        return membership is null ? null : MapToDto(membership, membership.Member, membership.Plan);
    }

    public async Task<MembershipDto> CreateAsync(CreateMembershipRequest request, CancellationToken ct = default)
    {
        var memberExists = await _memberRepository.ExistsAsync(m => m.Id == request.MemberId, ct);
        if (!memberExists)
            throw new NotFoundException($"Member with ID {request.MemberId} not found.");

        var planExists = await _planRepository.ExistsAsync(p => p.Id == request.PlanId, ct);
        if (!planExists)
            throw new NotFoundException($"Plan with ID {request.PlanId} not found.");

        var overlap = await _repository.ExistsAsync(
            m => m.MemberId == request.MemberId &&
                 m.StartDate < request.EndDate &&
                 m.EndDate > request.StartDate &&
                 !m.IsDeleted, ct);
        if (overlap)
            throw new ConflictException($"Member with ID {request.MemberId} already has an overlapping membership for this period.");

        var membership = new Membership
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MemberId = request.MemberId,
            PlanId = request.PlanId
        };

        var created = await _repository.AddAsync(membership, ct);
        _logger.LogInformation("Created membership {MembershipId}: Member={MemberId}, Plan={PlanId}",
            created.Id, created.MemberId, created.PlanId);

        return MapToDto(created, created.Member, created.Plan);
    }

    public async Task<MembershipDto> UpdateAsync(int id, UpdateMembershipRequest request, CancellationToken ct = default)
    {
        var membership = await _repository.GetByIdWithIncludesAsync(id, ct, m => m.Member, m => m.Plan);
        if (membership is null)
            throw new NotFoundException($"Membership with ID {id} not found.");

        if (request.StartDate.HasValue) membership.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) membership.EndDate = request.EndDate.Value;
        if (request.MemberId.HasValue) membership.MemberId = request.MemberId.Value;
        if (request.PlanId.HasValue) membership.PlanId = request.PlanId.Value;

        await _repository.UpdateAsync(membership, ct);
        _logger.LogInformation("Updated membership {MembershipId}", id);

        return MapToDto(membership, membership.Member, membership.Plan);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var membership = await _repository.GetByIdAsync(id, ct);
        if (membership is null)
            throw new NotFoundException($"Membership with ID {id} not found.");

        await _repository.SoftDeleteAsync(membership, ct);
        _logger.LogInformation("Soft-deleted membership {MembershipId}", id);
    }

    private static MembershipDto MapToDto(Membership membership, Member? member, Plan? plan) => new()
    {
        Id = membership.Id,
        StartDate = membership.StartDate,
        EndDate = membership.EndDate,
        MemberId = membership.MemberId,
        MemberName = member?.Name ?? string.Empty,
        MemberEmail = member?.Email ?? string.Empty,
        PlanId = membership.PlanId,
        PlanName = plan?.Name ?? string.Empty,
        PlanPrice = plan?.Price ?? 0,
        CreatedAt = membership.CreatedAt
    };
}

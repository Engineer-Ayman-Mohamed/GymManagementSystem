using GymManagementSystem.BusinessLayer.DTOs.Member;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class MemberService : IMemberService
{
    private readonly IGenericRepository<Member> _repository;
    private readonly ILogger<MemberService> _logger;

    public MemberService(IGenericRepository<Member> repository, ILogger<MemberService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MemberDto>> GetAllAsync(CancellationToken ct = default)
    {
        var members = await _repository.GetAllAsync(ct);
        return members.Select(MapToDto).ToList();
    }

    public async Task<MemberDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var member = await _repository.GetByIdAsync(id, ct);
        return member is null ? null : MapToDto(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken ct = default)
    {
        var emailExists = await _repository.ExistsAsync(m => m.Email == request.Email, ct);
        if (emailExists)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var phoneExists = await _repository.ExistsAsync(m => m.Phone == request.Phone, ct);
        if (phoneExists)
            throw new ConflictException($"Phone '{request.Phone}' is already registered.");

        var member = new Member
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Photo = request.Photo,
            JoinDate = request.JoinDate,
            Address = new Address
            {
                BuildingNumber = request.BuildingNumber,
                Street = request.Street,
                City = request.City
            }
        };

        var created = await _repository.AddAsync(member, ct);
        _logger.LogInformation("Created member {MemberId} ({Name})", created.Id, created.Name);

        return MapToDto(created);
    }

    public async Task<MemberDto> UpdateAsync(int id, UpdateMemberRequest request, CancellationToken ct = default)
    {
        var member = await _repository.GetByIdAsync(id, ct);
        if (member is null)
            throw new NotFoundException($"Member with ID {id} not found.");

        if (!string.Equals(request.Email, member.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _repository.ExistsAsync(m => m.Email == request.Email && m.Id != id, ct);
            if (emailExists)
                throw new ConflictException($"Email '{request.Email}' is already in use.");
        }

        if (request.Phone != member.Phone)
        {
            var phoneExists = await _repository.ExistsAsync(m => m.Phone == request.Phone && m.Id != id, ct);
            if (phoneExists)
                throw new ConflictException($"Phone '{request.Phone}' is already in use.");
        }

        if (request.Name is not null) member.Name = request.Name;
        if (request.Email is not null) member.Email = request.Email;
        if (request.Phone is not null) member.Phone = request.Phone;
        if (request.DateOfBirth.HasValue) member.DateOfBirth = request.DateOfBirth.Value;
        if (request.Gender.HasValue) member.Gender = request.Gender.Value;
        if (request.Photo is not null) member.Photo = request.Photo;
        if (request.JoinDate.HasValue) member.JoinDate = request.JoinDate.Value;
        if (request.BuildingNumber.HasValue) member.Address.BuildingNumber = request.BuildingNumber.Value;
        if (request.Street is not null) member.Address.Street = request.Street;
        if (request.City is not null) member.Address.City = request.City;

        await _repository.UpdateAsync(member, ct);
        _logger.LogInformation("Updated member {MemberId}", id);

        return MapToDto(member);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var member = await _repository.GetByIdAsync(id, ct);
        if (member is null)
            throw new NotFoundException($"Member with ID {id} not found.");

        await _repository.SoftDeleteAsync(member, ct);
        _logger.LogInformation("Soft-deleted member {MemberId}", id);
    }

    private static MemberDto MapToDto(Member member) => new()
    {
        Id = member.Id,
        Name = member.Name,
        Email = member.Email,
        Phone = member.Phone,
        DateOfBirth = member.DateOfBirth,
        Gender = member.Gender.ToString(),
        Photo = member.Photo,
        JoinDate = member.JoinDate,
        BuildingNumber = member.Address.BuildingNumber,
        Street = member.Address.Street,
        City = member.Address.City,
        CreatedAt = member.CreatedAt
    };
}

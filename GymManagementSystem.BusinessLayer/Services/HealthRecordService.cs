using GymManagementSystem.BusinessLayer.DTOs.HealthRecord;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class HealthRecordService : IHealthRecordService
{
    private readonly IGenericRepository<HealthRecord> _repository;
    private readonly IGenericRepository<Member> _memberRepository;
    private readonly ILogger<HealthRecordService> _logger;

    public HealthRecordService(
        IGenericRepository<HealthRecord> repository,
        IGenericRepository<Member> memberRepository,
        ILogger<HealthRecordService> logger)
    {
        _repository = repository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<HealthRecordDto>> GetAllAsync(CancellationToken ct = default)
    {
        var records = await _repository.GetAllWithIncludesAsync(ct, r => r.Member);
        return records.Select(MapToDto).ToList();
    }

    public async Task<HealthRecordDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var record = await _repository.GetByIdWithIncludesAsync(id, ct, r => r.Member);
        return record is null ? null : MapToDto(record);
    }

    public async Task<HealthRecordDto?> GetByMemberIdAsync(int memberId, CancellationToken ct = default)
    {
        var records = await _repository.FindAsync(r => r.MemberId == memberId, ct);
        var record = records.FirstOrDefault();
        if (record is null) return null;
        var member = await _memberRepository.GetByIdAsync(memberId, ct);
        return new HealthRecordDto
        {
            Id = record.Id,
            Height = record.Height,
            Weight = record.Weight,
            BloodType = record.BloodType,
            Note = record.Note,
            MemberId = record.MemberId,
            MemberName = member?.Name ?? "Unknown",
            CreatedAt = record.CreatedAt
        };
    }

    public async Task<HealthRecordDto> CreateAsync(CreateHealthRecordRequest request, CancellationToken ct = default)
    {
        var memberExists = await _memberRepository.ExistsAsync(m => m.Id == request.MemberId, ct);
        if (!memberExists)
            throw new NotFoundException($"Member with ID {request.MemberId} not found.");

        var existing = await _repository.FindAsync(r => r.MemberId == request.MemberId, ct);
        if (existing.Any())
            throw new ConflictException($"Member ID {request.MemberId} already has a health record.");

        var record = new HealthRecord
        {
            Height = request.Height,
            Weight = request.Weight,
            BloodType = request.BloodType,
            Note = request.Note,
            MemberId = request.MemberId
        };

        var created = await _repository.AddAsync(record, ct);
        var member = await _memberRepository.GetByIdAsync(request.MemberId, ct);
        _logger.LogInformation("Created health record {RecordId} for member {MemberId}", created.Id, request.MemberId);

        return new HealthRecordDto
        {
            Id = created.Id,
            Height = created.Height,
            Weight = created.Weight,
            BloodType = created.BloodType,
            Note = created.Note,
            MemberId = created.MemberId,
            MemberName = member?.Name ?? "Unknown",
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<HealthRecordDto> UpdateAsync(int id, UpdateHealthRecordRequest request, CancellationToken ct = default)
    {
        var record = await _repository.GetByIdWithIncludesAsync(id, ct, r => r.Member);
        if (record is null)
            throw new NotFoundException($"Health record with ID {id} not found.");

        if (request.Height.HasValue) record.Height = request.Height.Value;
        if (request.Weight.HasValue) record.Weight = request.Weight.Value;
        if (request.BloodType is not null) record.BloodType = request.BloodType;
        if (request.Note is not null) record.Note = request.Note;

        await _repository.UpdateAsync(record, ct);
        _logger.LogInformation("Updated health record {RecordId}", id);

        return MapToDto(record);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var record = await _repository.GetByIdAsync(id, ct);
        if (record is null)
            throw new NotFoundException($"Health record with ID {id} not found.");

        await _repository.SoftDeleteAsync(record, ct);
        _logger.LogInformation("Soft-deleted health record {RecordId}", id);
    }

    private static HealthRecordDto MapToDto(HealthRecord record) => new()
    {
        Id = record.Id,
        Height = record.Height,
        Weight = record.Weight,
        BloodType = record.BloodType,
        Note = record.Note,
        MemberId = record.MemberId,
        MemberName = record.Member?.Name ?? "Unknown",
        CreatedAt = record.CreatedAt
    };
}

using GymManagementSystem.BusinessLayer.DTOs.Trainer;
using GymManagementSystem.BusinessLayer.Exceptions;
using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class TrainerService : ITrainerService
{
    private readonly IGenericRepository<Trainer> _repository;
    private readonly ILogger<TrainerService> _logger;

    public TrainerService(IGenericRepository<Trainer> repository, ILogger<TrainerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TrainerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var trainers = await _repository.GetAllAsync(ct);
        return trainers.Select(MapToDto).ToList();
    }

    public async Task<TrainerDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(id, ct);
        return trainer is null ? null : MapToDto(trainer);
    }

    public async Task<TrainerDto> CreateAsync(CreateTrainerRequest request, CancellationToken ct = default)
    {
        var emailExists = await _repository.ExistsAsync(t => t.Email == request.Email, ct);
        if (emailExists)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var phoneExists = await _repository.ExistsAsync(t => t.Phone == request.Phone, ct);
        if (phoneExists)
            throw new ConflictException($"Phone '{request.Phone}' is already registered.");

        var trainer = new Trainer
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Specialty = request.Specialty,
            HireDate = request.HireDate,
            Address = new Address
            {
                BuildingNumber = request.BuildingNumber,
                Street = request.Street,
                City = request.City
            }
        };

        var created = await _repository.AddAsync(trainer, ct);
        _logger.LogInformation("Created trainer {TrainerId} ({Name})", created.Id, created.Name);

        return MapToDto(created);
    }

    public async Task<TrainerDto> UpdateAsync(int id, UpdateTrainerRequest request, CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(id, ct);
        if (trainer is null)
            throw new NotFoundException($"Trainer with ID {id} not found.");

        if (!string.Equals(request.Email, trainer.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _repository.ExistsAsync(t => t.Email == request.Email && t.Id != id, ct);
            if (emailExists)
                throw new ConflictException($"Email '{request.Email}' is already in use.");
        }

        if (request.Phone != trainer.Phone)
        {
            var phoneExists = await _repository.ExistsAsync(t => t.Phone == request.Phone && t.Id != id, ct);
            if (phoneExists)
                throw new ConflictException($"Phone '{request.Phone}' is already in use.");
        }

        if (request.Name is not null) trainer.Name = request.Name;
        if (request.Email is not null) trainer.Email = request.Email;
        if (request.Phone is not null) trainer.Phone = request.Phone;
        if (request.DateOfBirth.HasValue) trainer.DateOfBirth = request.DateOfBirth.Value;
        if (request.Gender.HasValue) trainer.Gender = request.Gender.Value;
        if (request.Specialty.HasValue) trainer.Specialty = request.Specialty.Value;
        if (request.HireDate.HasValue) trainer.HireDate = request.HireDate.Value;
        if (request.BuildingNumber.HasValue) trainer.Address.BuildingNumber = request.BuildingNumber.Value;
        if (request.Street is not null) trainer.Address.Street = request.Street;
        if (request.City is not null) trainer.Address.City = request.City;

        await _repository.UpdateAsync(trainer, ct);
        _logger.LogInformation("Updated trainer {TrainerId}", id);

        return MapToDto(trainer);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var trainer = await _repository.GetByIdAsync(id, ct);
        if (trainer is null)
            throw new NotFoundException($"Trainer with ID {id} not found.");

        await _repository.SoftDeleteAsync(trainer, ct);
        _logger.LogInformation("Soft-deleted trainer {TrainerId}", id);
    }

    private static TrainerDto MapToDto(Trainer trainer) => new()
    {
        Id = trainer.Id,
        Name = trainer.Name,
        Email = trainer.Email,
        Phone = trainer.Phone,
        DateOfBirth = trainer.DateOfBirth,
        Gender = trainer.Gender.ToString(),
        BuildingNumber = trainer.Address.BuildingNumber,
        Street = trainer.Address.Street,
        City = trainer.Address.City,
        Specialty = trainer.Specialty.ToString(),
        HireDate = trainer.HireDate,
        CreatedAt = trainer.CreatedAt
    };
}

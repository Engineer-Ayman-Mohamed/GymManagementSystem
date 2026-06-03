using GymManagementSystem.DataLayer.Enums;

namespace GymManagementSystem.BusinessLayer.DTOs.Trainer;

public class CreateTrainerRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public int BuildingNumber { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public Specialty Specialty { get; set; }
    public DateTime HireDate { get; set; }
}

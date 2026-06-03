using GymManagementSystem.DataLayer.Enums;

namespace GymManagementSystem.BusinessLayer.DTOs.Trainer;

public class TrainerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public int BuildingNumber { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public DateTime HireDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

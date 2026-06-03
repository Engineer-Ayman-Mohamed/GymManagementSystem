using GymManagementSystem.DataLayer.Enums;

namespace GymManagementSystem.BusinessLayer.DTOs.Trainer;

public class UpdateTrainerRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public int? BuildingNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public Specialty? Specialty { get; set; }
    public DateTime? HireDate { get; set; }
}

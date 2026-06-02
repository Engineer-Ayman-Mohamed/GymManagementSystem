using GymManagementSystem.DataLayer.Enums;

namespace GymManagementSystem.BusinessLayer.DTOs.Member;

public class CreateMemberRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Photo { get; set; }
    public DateTime JoinDate { get; set; }
    public int BuildingNumber { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
}

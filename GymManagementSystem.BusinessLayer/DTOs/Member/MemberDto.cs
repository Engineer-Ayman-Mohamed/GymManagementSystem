namespace GymManagementSystem.BusinessLayer.DTOs.Member;

public class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string? Photo { get; set; }
    public DateTime JoinDate { get; set; }
    public int BuildingNumber { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

namespace GymManagementSystem.BusinessLayer.DTOs.HealthRecord;

public class HealthRecordDto
{
    public int Id { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string BloodType { get; set; } = null!;
    public string? Note { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

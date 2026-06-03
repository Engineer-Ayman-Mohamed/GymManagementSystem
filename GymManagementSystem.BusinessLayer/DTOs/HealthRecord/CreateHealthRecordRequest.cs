namespace GymManagementSystem.BusinessLayer.DTOs.HealthRecord;

public class CreateHealthRecordRequest
{
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string BloodType { get; set; } = null!;
    public string? Note { get; set; }
    public int MemberId { get; set; }
}
